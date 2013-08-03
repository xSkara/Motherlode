// ****************************************************************
// Copyright 2012, Stephan Burguchev
// e-mail: me@sburg.net
// ****************************************************************
// *

using System;
using Motherlode.Common;

namespace Motherlode.Log4Net
{
    /// <summary>Additional information for email log message formatting events.</summary>
    public class EmailLogMessageFormattingEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <c>EmailLogMessageFormattingEventArgs</c> class.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        public EmailLogMessageFormattingEventArgs(string subject, string body)
        {
            Guard.IsNotNull(() => subject);
            Guard.IsNotNull(() => body);

            this.Subject = subject;
            this.Body = body;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the message body.</summary>
        /// <value>The message body.</value>
        public string Body { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the firing event is completely handled by the
        ///     active handler and the other handlers will be skept.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if the event is handled, <see langword="false" /> if not.
        /// </value>
        public bool Handled { get; set; }

        /// <summary>Gets or sets the subject of the message.</summary>
        /// <value>The subject of the message.</value>
        public string Subject { get; set; }

        #endregion
    }
}
