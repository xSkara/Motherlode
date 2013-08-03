// ****************************************************************
// Copyright 2012, Stephan Burguchev
// e-mail: me@sburg.net
// ****************************************************************
// *

using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using Microsoft.Win32;
using Motherlode.Common.Extensions;
using Ninject.Extensions.Logging;

namespace Motherlode.Common
{
    /// <summary>
    ///     Information about the system. The initial intention is to collect comprehensive information
    ///     about the system to send it with an error report.
    /// </summary>
    public class SystemInfo
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <c>SystemInfo</c> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public SystemInfo(ILogger logger)
        {
            Guard.IsNotNull(() => logger);

            this.OsVersion = Environment.OSVersion.ToString();
            this.Is64BitOs = Environment.Is64BitOperatingSystem;
            this.Is64BitProcess = Environment.Is64BitProcess;
            this.UserName = Environment.UserName;
            this.MachineName = Environment.MachineName;

            // Computer model and memory information
            try
            {
                var query = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem");
                ManagementObjectCollection queryCollection = query.Get();
                if (queryCollection.Count > 0)
                {
                    ManagementObject mo = queryCollection.Cast<ManagementObject>().First();
                    this.TotalPhysicalMemory = long.Parse(mo["totalphysicalmemory"].ToString());
                }
            }
            catch (Exception ex)
            {
                logger.Warn("Failed to retrieve system information from Win32_ComputerSystem WMI table.", ex);
            }

            // Processor type and speed
            var processors = new List<string>();
            try
            {
                var query = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
                ManagementObjectCollection queryCollection = query.Get();
                foreach (ManagementObject mo in queryCollection)
                {
                    string processor = mo["Name"].ToString();
                    processors.Add(processor);
                }

                this.Processors = processors.ToArray();
            }
            catch (Exception ex)
            {
                logger.Warn("Failed to retrieve system information from Win32_Processor WMI table.", ex);
            }

            // Disk drives, total capacity and free space
            var logicalDisks = new List<DiskInfo>();
            try
            {
                var query = new ManagementObjectSearcher("SELECT * FROM Win32_LogicalDisk");
                ManagementObjectCollection queryCollection = query.Get();
                foreach (ManagementObject mo in queryCollection)
                {
                    string deviceId = mo["DeviceID"].ToString();
                    string description = mo["Description"].ToString();
                    long freeSpace = long.Parse((mo["Freespace"] ?? 0).ToString());
                    long size = long.Parse((mo["Size"] ?? 0).ToString());
                    logicalDisks.Add(new DiskInfo(deviceId, description, freeSpace, size));
                }

                this.LogicalDisks = logicalDisks.OrderBy(ld => ld.DeviceId).ToArray();
            }
            catch (Exception ex)
            {
                logger.Warn("Failed to retrieve system information from Win32_LogicalDisk WMI table.", ex);
            }

            // Installed apps
            var installedApps = new List<string>();
            try
            {
                using (
                    RegistryKey regKey = Registry.LocalMachine.OpenSubKey(
                        @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"))
                {
                    foreach (string appKeyName in regKey.GetSubKeyNames().Where(n => !n.StartsWith("KB")))
                    {
                        using (RegistryKey appRegKey = regKey.OpenSubKey(appKeyName))
                        {
                            object value = appRegKey.GetValue("DisplayName");
                            if (value != null)
                            {
                                installedApps.Add(value.ToString());
                            }
                        }
                    }

                    this.InstalledApps = installedApps.OrderBy(n => n).ToArray();
                }
            }
            catch (Exception ex)
            {
                logger.Warn("Failed to retrieve system information from Win32_LogicalDisk WMI table.", ex);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the model of this computer.</summary>
        /// <value>The computer model.</value>
        public string ComputerModel { get; private set; }

        /// <summary>Gets the string array containing names of the applications installed on this computer.</summary>
        /// <value>The installed applications information.</value>
        public string[] InstalledApps { get; private set; }

        /// <summary>
        ///     Gets the value indicating whether the current operating system is a 64-bit operating
        ///     system.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the current operating system is a 64-bit operating system; otherwise,
        ///     <c>false</c>.
        /// </value>
        public bool Is64BitOs { get; private set; }

        /// <summary>
        ///     Gets the value indicating whether the current process is a 64-bit process.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the current process is a 64-bit process; otherwise, <c>false</c>.
        /// </value>
        public bool Is64BitProcess { get; private set; }

        /// <summary>
        ///     Gets the array of <c>DiskInfo</c> objects containing the information of logical disks
        ///     installed on this computer.
        /// </summary>
        /// <value>The logical disks information.</value>
        public DiskInfo[] LogicalDisks { get; private set; }

        /// <summary>Gets the NetBIOS name of this local computer.</summary>
        /// <value>A string containing the name of this local computer.</value>
        public string MachineName { get; private set; }

        /// <summary>Gets the version of the operating system.</summary>
        /// <value>The operating system version.</value>
        public string OsVersion { get; private set; }

        /// <summary>
        ///     Gets the string array briefly describing the processors installed on this computer.
        /// </summary>
        /// <value>The processors description.</value>
        public string[] Processors { get; private set; }

        /// <summary>
        ///     Gets the total number of physical memory on this computer. The size presented in
        ///     bytes.
        /// </summary>
        /// <value>The total number of physical memory in bytes.</value>
        public long TotalPhysicalMemory { get; private set; }

        /// <summary>
        ///     Gets the user name of the person who is currently logged on to the Windows operating
        ///     system.
        /// </summary>
        /// <value>The user name of the person who is logged on to Windows.</value>
        public string UserName { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="includingInstalledAppsInfo">
        ///     If set to <c>true</c> the string will include an information about the applications installed
        ///     on the machine.
        /// </param>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public string ToString(bool includingInstalledAppsInfo = false)
        {
            var sb = new StringBuilder();
            sb.AppendLine("------------- System Information Report -------------");
            sb.AppendLine();

            sb.AppendLine("## Application Info ##");
            sb.AppendFormat(
                "Executing Process Bitness: {0}\r\n",
                this.Is64BitProcess ? 64 : 32);
            sb.AppendLine();
            sb.AppendLine("## OS Info ##");
            sb.AppendFormat("Operating System: {0}\r\n", this.OsVersion);
            sb.AppendFormat(
                "OS Bitness: {0}\r\n",
                this.Is64BitOs ? 64 : 32);
            sb.AppendFormat("Username: {0}\r\n", this.UserName);
            sb.AppendFormat("Machine Name: {0}\r\n", this.MachineName);
            sb.AppendLine();
            sb.AppendLine("## Hardware info ##");
            sb.AppendFormat("Computer Model: {0}\r\n", this.ComputerModel);
            sb.AppendFormat("Total Physical Memory: {0}\r\n", this.TotalPhysicalMemory.ToDataSize());
            for (int i = 0; i < this.Processors.Length; i++)
            {
                sb.AppendFormat("Processor #{0}: {1}\r\n", i, this.Processors[i]);
            }

            sb.AppendLine();
            sb.AppendLine("## Logical Disks ##");
            for (int i = 0; i < this.LogicalDisks.Length; i++)
            {
                DiskInfo disk = this.LogicalDisks[i];
                sb.AppendFormat(
                    "LogicalDisk #{0}: [DeviceID : {1}, FreeSpace : {2}, Size : {3}, Description : {4}]\r\n",
                    i,
                    disk.DeviceId,
                    disk.FreeSpace.ToDataSize(),
                    disk.Size.ToDataSize(),
                    disk.Description);
            }

            if (includingInstalledAppsInfo)
            {
                sb.AppendLine();
                sb.AppendLine("## Installed Applications ##");
                for (int i = 0; i < this.InstalledApps.Length; i++)
                {
                    sb.AppendFormat("{0}. {1}\r\n", i + 1, this.InstalledApps[i]);
                }
            }

            sb.AppendLine();
            sb.AppendLine("-----------------------------------------------------");
            return sb.ToString();
        }

        #endregion

        /// <summary>Information about a logical disk.</summary>
        public class DiskInfo
        {
            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <c>DiskInfo</c> class.
            /// </summary>
            /// <param name="deviceId">The identifier of the device.</param>
            /// <param name="description">The description.</param>
            /// <param name="freeSpace">The free space in bytes.</param>
            /// <param name="size">The total number of disk memory in bytes.</param>
            /// <exception cref="ArgumentNullException">
            ///     Thrown when one or more required arguments are null.
            /// </exception>
            public DiskInfo(string deviceId, string description, long freeSpace, long size)
            {
                Guard.IsNotNull(() => deviceId);
                Guard.IsNotNull(() => description);

                this.DeviceId = deviceId;
                this.Description = description;
                this.FreeSpace = freeSpace;
                this.Size = size;
            }

            #endregion

            #region Public Properties

            /// <summary>Gets the description.</summary>
            /// <value>The description.</value>
            public string Description { get; private set; }

            /// <summary>Gets the identifier of the device.</summary>
            /// <value>The identifier of the device.</value>
            public string DeviceId { get; private set; }

            /// <summary>Gets the free space (in bytes) left on disk.</summary>
            /// <value>The free space left on disk.</value>
            public long FreeSpace { get; private set; }

            /// <summary>Gets or sets the total number of disk memory in bytes.</summary>
            /// <value>The total number of disk memory.</value>
            public long Size { get; private set; }

            #endregion
        }
    }
}
