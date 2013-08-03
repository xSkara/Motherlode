// ****************************************************************
// Copyright 2012, Stephan Burguchev
// e-mail: me@sburg.net
// ****************************************************************
// *

using System.IO;
using Ninject.Extensions.Logging;
using Trinet.Core.IO.Ntfs;

namespace Motherlode.Common
{
    /// <summary>Provides solution for common security tasks.</summary>
    public class Security
    {
        #region Constants and Fields

        private readonly ILogger _logger;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <c>Security</c> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public Security(ILogger logger)
        {
            Guard.IsNotNull(() => logger);

            this._logger = logger;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Removes the zone identifier from a file that places the file into the full trusted zone.
        /// </summary>
        /// <remarks>
        ///     It is required when the file has been downloaded from the Internet for example. The CAS
        ///     policies must be disabled also that is default behavior in the .NET 4 and higher. The
        ///     ReadOnly file attribute will be removed if it's set.
        /// </remarks>
        /// <param name="fileInfo">The fileInfo of the file to be processed.</param>
        /// <returns>
        ///     <c>true</c> if it succeeds, <c>false</c> if it fails.
        /// </returns>
        public bool RemoveZoneIdentifier(FileInfo fileInfo)
        {
            Guard.IsNotNull(() => fileInfo);

            const string streamName = "Zone.Identifier";
            string filename = fileInfo.FullName;

            if (!File.Exists(filename))
            {
                return false;
            }

            if (!FileSystem.AlternateDataStreamExists(filename, streamName))
            {
                return false;
            }

            // Clearing the read-only attribute, if set
            FileAttributes attributes = File.GetAttributes(filename);
            if (FileAttributes.ReadOnly ==
                (FileAttributes.ReadOnly & attributes))
            {
                attributes &= ~FileAttributes.ReadOnly;
                File.SetAttributes(filename, attributes);
            }

            if (FileSystem.DeleteAlternateDataStream(filename, streamName))
            {
                this._logger.Info(
                    "The ZoneIdentifier NTFS stream has been removed from the file '{0}'.",
                    filename);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Removes the zone identifier from files residing within the <paramref name="dirInfo" />
        ///     directory. The action will place the files into the full trusted zone.
        /// </summary>
        /// <remarks>
        ///     It is required when the files has been downloaded from the Internet for example. The CAS
        ///     policies must be disabled also that is default behavior in the .NET 4 and higher.
        /// </remarks>
        /// <param name="dirInfo">DirectoryInfo of the directory which files to be processed.</param>
        /// <param name="recursive">
        ///     <c>true</c> to process the subdirectories recursively. The default value is <c>false</c>.
        /// </param>
        /// <returns>
        ///     <c>true</c> if it succeeds, <c>false</c> if it fails.
        /// </returns>
        public bool RemoveZoneIdentifier(DirectoryInfo dirInfo, bool recursive = false)
        {
            Guard.IsNotNull(() => dirInfo);

            const string streamName = "Zone.Identifier";
            bool anyFileHasBeenChanged = false;
            string dirname = dirInfo.FullName;

            if (!Directory.Exists(dirname))
            {
                return false;
            }

            var directoryInfo = new DirectoryInfo(dirname);

            // performing main actions
            foreach (FileInfo fileInfo in directoryInfo.GetFiles())
            {
                bool result = FileSystem.AlternateDataStreamExists(fileInfo.FullName, streamName);
                if (result)
                {
                    // Clearing the read-only attribute, if set
                    FileAttributes attributes = File.GetAttributes(fileInfo.FullName);
                    if (FileAttributes.ReadOnly ==
                        (FileAttributes.ReadOnly & attributes))
                    {
                        attributes &= ~FileAttributes.ReadOnly;
                        File.SetAttributes(fileInfo.FullName, attributes);
                    }

                    if (FileSystem.DeleteAlternateDataStream(fileInfo.FullName, streamName))
                    {
                        this._logger.Info(
                            "The ZoneIdentifier NTFS stream has been removed from the file '{0}'.",
                            fileInfo.FullName);
                        anyFileHasBeenChanged = true;
                    }
                }
            }

            if (!recursive)
            {
                return anyFileHasBeenChanged;
            }

            // recursive call
            foreach (DirectoryInfo subDir in directoryInfo.GetDirectories())
            {
                this.RemoveZoneIdentifier(subDir, true);
            }

            return anyFileHasBeenChanged;
        }

        #endregion
    }
}
