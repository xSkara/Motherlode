// ****************************************************************
// Copyright 2012, Stephan Burguchev
// e-mail: me@sburg.net
// ****************************************************************
// *

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;

namespace Motherlode.Common
{
    /// <summary>Provides some useful implements missing in the FCL.</summary>
    public static class PathHelper
    {
        #region Public Properties

        /// <summary>
        ///     Gets the path for the executable file that started the application, including the executable
        ///     name.
        /// </summary>
        /// <value>The path for the executable file that started the application.</value>
        public static string ExecutablePath
        {
            get
            {
                return getExecutablePath();
            }
        }

        /// <summary>
        ///     Gets the path for the executable file that started the application, not including the
        ///     executable name.
        /// </summary>
        /// <value>
        ///     The path for the executable file that started the application.
        /// </value>
        public static string StartupPath
        {
            get
            {
                return Path.GetDirectoryName(getExecutablePath());
            }
        }

        #endregion

        #region Methods

        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP0100:AdvancedNamingRules",
            Justification = "Reviewed. Suppression is OK here.")]
        [DllImport(@"kernel32.dll", CharSet = CharSet.Auto)]
        private static extern int GetModuleFileName(IntPtr hModule, StringBuilder buffer, int length);

        private static string getExecutablePath()
        {
            string executablePath;
            Assembly assembly1 = Assembly.GetEntryAssembly();
            if (assembly1 == null)
            {
                var builder1 = new StringBuilder(260);
                GetModuleFileName(IntPtr.Zero, builder1, builder1.Capacity);
                executablePath = Path.GetFullPath(builder1.ToString());
            }
            else
            {
                string text1 = assembly1.EscapedCodeBase;
                var uri1 = new Uri(text1);
                executablePath = uri1.Scheme == "file"
                                     ? getLocalPath(text1)
                                     : uri1.ToString();
            }

            var uri2 = new Uri(executablePath);
            if (uri2.Scheme == "file")
            {
                new FileIOPermission(FileIOPermissionAccess.PathDiscovery, executablePath).Demand();
            }

            return executablePath;
        }

        private static string getLocalPath(string fileName)
        {
            var uri1 = new Uri(fileName);
            return uri1.LocalPath + uri1.Fragment;
        }

        #endregion
    }
}
