// ****************************************************************
// Copyright 2012, Stephan Burguchev
// e-mail: me@sburg.net
// ****************************************************************
// *

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Motherlode.Common
{
    /// <summary>
    ///     The class if enabled will attempt to load missing assemblies from the embedded resources.
    /// </summary>
    /// <remarks>
    ///     Assemblies are looked up from the <see cref="Source" /> assembly and must reside in the
    ///     <see cref="Namespace" /> namespace. The platform specific assemblies must reside in the
    ///     platform specific subnamespace of the namespace. The subnamespaces are 'x64' and 'x86' for
    ///     the x64 and x86 respectively.
    /// </remarks>
    public static class EmbeddedAssemblyLoader
    {
        #region Constants and Fields

        /// <summary>A sub-namespace for the x64 specific assemblies.</summary>
        public const string X64SubNamespace = "x64";

        /// <summary>A sub-namespace for the x86 specific assemblies.</summary>
        public const string X86SubNamespace = "x86";

        private static readonly Dictionary<string, Assembly> _cache = new Dictionary<string, Assembly>();
        private static readonly object _lock = new object();
        private static bool _isEnabled;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="EmbeddedAssemblyLoader" />
        ///     will attempt to load missing assemblies from the platform specific directory.
        ///     The default value is <c>false</c>.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the missing assembly resolver is enabled; otherwise, <c>false</c>.
        /// </value>
        public static bool Enable
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                lock (_lock)
                {
                    if (_isEnabled != value)
                    {
                        if (value)
                        {
                            AppDomain.CurrentDomain.AssemblyResolve += resolver;
                        }
                        else
                        {
                            AppDomain.CurrentDomain.AssemblyResolve -= resolver;
                        }
                        _isEnabled = value;
                    }
                }
            }
        }

        /// <summary>Gets or sets the namespace where the embedded assemblies reside.</summary>
        /// <value>The embedded assemblies namespace.</value>
        public static string Namespace { get; set; }

        /// <summary>Gets or sets the assembly where the embedded assemblies reside.</summary>
        /// <value>The assembly where the embedded assemblies reside.</value>
        public static Assembly Source { get; set; }

        #endregion

        #region Methods

        private static Assembly loadFromResource(string resourceName)
        {
            if (Source.GetManifestResourceNames().Contains(resourceName))
            {
                using (var ms = new MemoryStream())
                {
                    Source.GetManifestResourceStream(resourceName).CopyTo(ms);
                    return Assembly.Load(ms.ToArray());
                }
            }

            return null;
        }

        private static Assembly resolver(object sender, ResolveEventArgs args)
        {
            if (Source == null ||
                Namespace == null)
            {
                throw new InvalidOperationException("The loader is not initialized.");
            }

            string assemblyName = args.Name.Split(new[] { ',' }, 2)[0];

            Assembly assembly;
            if (_cache.TryGetValue(assemblyName, out assembly))
            {
                return assembly;
            }

            string psSubNamespace = Environment.Is64BitProcess
                                        ? X64SubNamespace
                                        : X86SubNamespace;
            string dllResource = string.Concat(Namespace, ".", assemblyName, ".dll");
            string exeResource = string.Concat(Namespace, ".", assemblyName, ".exe");
            string platformSpecificDllResource = string.Concat(Namespace, ".", psSubNamespace, ".", assemblyName, ".dll");
            string platformSpecificExeResource = string.Concat(Namespace, ".", psSubNamespace, ".", assemblyName, ".exe");

            assembly = loadFromResource(dllResource) ??
                       loadFromResource(exeResource) ??
                       loadFromResource(platformSpecificDllResource) ??
                       loadFromResource(platformSpecificExeResource);
            if (assembly != null)
            {
                _cache[assemblyName] = assembly;
            }

            return assembly;
        }

        #endregion
    }
}
