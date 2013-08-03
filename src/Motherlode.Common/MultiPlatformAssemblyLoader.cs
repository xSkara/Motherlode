// ****************************************************************
// Copyright 2012, Stephan Burguchev
// e-mail: me@sburg.net
// ****************************************************************
// *

using System;
using System.IO;
using System.Reflection;

namespace Motherlode.Common
{
    /// <summary>
    ///     The class if enabled will attempt to load missing assemblies from the platform specific
    ///     directory.
    /// </summary>
    /// <remarks>
    ///     The platform specific assemblies will be looked up in the directories specified by the
    ///     <see cref="X64AssembliesDirectory" /> and <see cref="X86AssembliesDirectory" /> properties. If
    ///     the value of the properties is relative the path for the executable file that started the
    ///     application will be the base. The default values are "platform\x64" and "platform\x86" respectively.
    /// </remarks>
    public static class MultiPlatformAssemblyLoader
    {
        #region Constants and Fields

        private static readonly object _lock = new object();
        private static bool _isEnabled;

        #endregion

        #region Constructors and Destructors

        static MultiPlatformAssemblyLoader()
        {
            X64AssembliesDirectory = @"platform\x64";
            X86AssembliesDirectory = @"platform\x86";
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="MultiPlatformAssemblyLoader" />
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

        /// <summary>Gets or sets the path for the x64 assemblies directory.</summary>
        /// <value>The path for the x64 assemblies directory.</value>
        public static string X64AssembliesDirectory { get; set; }

        /// <summary>Gets or sets the path for the x86 assemblies directory.</summary>
        /// <value>The path for the x86 assemblies directory.</value>
        public static string X86AssembliesDirectory { get; set; }

        #endregion

        #region Methods

        private static Assembly resolver(object sender, ResolveEventArgs args)
        {
            string shortAssemblyName = new AssemblyName(args.Name).Name;

            string platformSpecificPath;
            if (Environment.Is64BitProcess)
            {
                platformSpecificPath = Path.IsPathRooted(X64AssembliesDirectory)
                                           ? Path.Combine(X64AssembliesDirectory, shortAssemblyName)
                                           : Path.Combine(PathHelper.StartupPath, X64AssembliesDirectory, shortAssemblyName);
            }
            else
            {
                platformSpecificPath = Path.IsPathRooted(X86AssembliesDirectory)
                                           ? Path.Combine(X86AssembliesDirectory, shortAssemblyName)
                                           : Path.Combine(PathHelper.StartupPath, X86AssembliesDirectory, shortAssemblyName);
            }

            if (File.Exists(platformSpecificPath + ".dll"))
            {
                return Assembly.LoadFile(platformSpecificPath + ".dll");
            }

            if (File.Exists(platformSpecificPath + ".exe"))
            {
                return Assembly.LoadFile(platformSpecificPath + ".exe");
            }

            return null;
        }

        #endregion
    }
}
