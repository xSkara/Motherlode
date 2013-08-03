// ****************************************************************
// Copyright 2012, Stephan Burguchev
// e-mail: me@sburg.net
// ****************************************************************
// *

using System.IO;
using System.Linq;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Repository.Hierarchy;

// The assembly attribute should reside in the assembly where log4net methods is called from for the first time
// (LogManager.GetLogger in this case).
// App.config log4net section should be in the exe file.

////[assembly: log4net.Config.XmlConfigurator(ConfigFileExtension = "log4net")] // appName.log4net

////[assembly: XmlConfigurator] // app.config

namespace Motherlode.Log4Net
{
    /// <summary>
    ///     The helper class provides some useful methods to configure the log4net engine and use it in the
    ///     simplest cases (when DI and per-class loggers are not involved).
    /// </summary>
    public class Logger
    {
        #region Constants and Fields

        private static readonly ILog _root;

        #endregion

        #region Constructors and Destructors

        static Logger()
        {
            // log4net.Config.XmlConfigurator.Configure(Assembly.GetExecutingAssembly().
            //    GetManifestResourceStream("dbMigrator.log4net.config.xml"));
            // log4net.Config.XmlConfigurator.Configure(
            // new FileInfo(Path.Combine(PathHelper.StartupPath, "log4net.config.xml")));
            _root = LogManager.GetLogger("root");
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the root logger.</summary>
        /// <value>The root logger.</value>
        public static ILog Root
        {
            get
            {
                return _root;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Configure the logging engine with the application configuration file. The method is more
        ///     explicit and easy to understand that the default one.
        /// </summary>
        public static void ConfigureWithAppConfig()
        {
            XmlConfigurator.Configure();
        }

        /// <summary>Gets an appender by name.</summary>
        /// <param name="appenderName">Name of the appender to retrieve.</param>
        /// <returns>The requested appender.</returns>
        /// <exception cref="LoggerException">
        ///     Thrown when the appender having the specified name is not found or a logging engine error occurs.
        /// </exception>
        public static IAppender GetAppender(string appenderName)
        {
            var hier = LogManager.GetRepository() as Hierarchy;
            if (hier == null)
            {
                throw new LoggerException("Error while loggers heirarchy retrieving.");
            }

            IAppender appender = hier.GetAppenders().SingleOrDefault(a => a.Name == appenderName);
            if (appender == null)
            {
                throw new LoggerException(string.Format("Error while log4net appender '{0}' retrieving.", appenderName));
            }

            return appender;
        }

        /// <summary>Gets a logger by name.</summary>
        /// <param name="loggerName">Name of the logger.</param>
        /// <returns>The logger.</returns>
        public static ILog GetLogger(string loggerName)
        {
            return LogManager.GetLogger(loggerName);
        }

        /// <summary>Reconfigures the log4net engine using the given configuration stream.</summary>
        /// <param name="configStream">The configuration stream.</param>
        public static void Reconfigure(Stream configStream)
        {
            XmlConfigurator.Configure(configStream);
        }

        /// <summary>Reconfigures the log4net engine using the given configuration file.</summary>
        /// <param name="filename">The filename of the configuration file.</param>
        public static void Reconfigure(string filename)
        {
            XmlConfigurator.Configure(new FileInfo(filename));
        }

        /// <summary>Sets a threshold for an appender.</summary>
        /// <param name="appenderName">Name of the appender to modify.</param>
        /// <param name="threshold">The threshold level.</param>
        public static void SetThresholdForAppender(string appenderName, Level threshold)
        {
            var hier = LogManager.GetRepository() as Hierarchy;
            if (hier == null)
            {
                return;
            }

            var appender = (AppenderSkeleton)hier.GetAppenders().SingleOrDefault(a => a.Name == appenderName);
            if (appender == null)
            {
                return;
            }

            appender.Threshold = threshold;
            appender.ActivateOptions();
        }

        #endregion
    }
}
