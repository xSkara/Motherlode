// ****************************************************************
// Copyright 2012, Stephan Burguchev
// e-mail: me@sburg.net
// ****************************************************************
// *

using System;
using System.Collections.Generic;
using log4net.Core;

namespace Motherlode.Log4Net
{
    /// <summary>Additional information for log message appended events.</summary>
    public class LogMessageAppendedEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <c>LogMessageAppendedEventArgs</c> class.
        /// </summary>
        /// <param name="appendedData">The log message having been appended.</param>
        /// <param name="properties">The properties of the log message.</param>
        /// <param name="level">The level of the log message.</param>
        public LogMessageAppendedEventArgs(string appendedData, Dictionary<string, object> properties, Level level)
        {
            this.AppendedData = appendedData;
            this.Properties = properties;
            this.Level = level;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the log message having been appended.</summary>
        /// <value>The log message having been appended.</value>
        public string AppendedData { get; private set; }

        /// <summary>Gets or sets the level of the log message.</summary>
        /// <value>The level of the log message.</value>
        public Level Level { get; private set; }

        /// <summary>Gets or sets the properties of the log message.</summary>
        /// <value>The properties of the log message.</value>
        public Dictionary<string, object> Properties { get; private set; }

        #endregion
    }
}
