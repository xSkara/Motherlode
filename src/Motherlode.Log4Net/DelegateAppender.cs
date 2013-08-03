// ****************************************************************
// Copyright 2012, Stephan Burguchev
// e-mail: me@sburg.net
// ****************************************************************
// *

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using log4net.Appender;
using log4net.Core;

namespace Motherlode.Log4Net
{
    /// <summary>
    ///     The appender that enables us to programmatically receive the log messages to perform some
    ///     additional actions with it like displaying inside the GUI.
    /// </summary>
    public class DelegateAppender : AppenderSkeleton
    {
        #region Public Events

        /// <summary>Event queue for all listeners interested in log message appending events.</summary>
        public event EventHandler<LogMessageAppendedEventArgs> OnAppend;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether to use a post or send method to fire the events
        ///     within the synchronization context having been set.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if asynchronous invoke should be performed, <see langword="false" /> if not.
        /// </value>
        public bool AsyncInvoke { get; set; }

        /// <summary>
        ///     Gets or sets the synchronization context object to synchronize the
        ///     <see cref="OnAppend" /> event with a target thread.
        /// </summary>
        /// <value>The synchronization context object.</value>
        public SynchronizationContext SyncContext { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Subclasses of <see cref="T:log4net.Appender.AppenderSkeleton" /> should implement this
        ///     method to perform actual logging.
        /// </summary>
        /// <param name="loggingEvent">The event to append.</param>
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP0100:AdvancedNamingRules",
            Justification = "Reviewed. Suppression is OK here.")]
        protected override void Append(LoggingEvent loggingEvent)
        {
            string renderedMessage = this.RenderLoggingEvent(loggingEvent);
            Dictionary<string, object> properties = loggingEvent.GetProperties().Cast<DictionaryEntry>().
                                                                 ToDictionary(e => e.Key.ToString(), e => e.Value);
            if (this.OnAppend == null)
            {
                return;
            }

            var ea = new LogMessageAppendedEventArgs(renderedMessage, properties, loggingEvent.Level);
            if (this.SyncContext == null)
            {
                this.OnAppend(this, ea);
            }
            else
            {
                if (this.AsyncInvoke)
                {
                    this.SyncContext.Post(o => this.OnAppend(this, ea), null);
                }
                else
                {
                    this.SyncContext.Send(o => this.OnAppend(this, ea), null);
                }
            }
        }

        #endregion
    }
}
