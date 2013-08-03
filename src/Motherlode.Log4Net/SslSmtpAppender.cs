// ****************************************************************
// Copyright 2012, Stephan Burguchev
// e-mail: me@sburg.net
// ****************************************************************
// *

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using log4net.Appender;
using log4net.Core;

namespace Motherlode.Log4Net
{
    /// <summary>
    ///     <para>
    ///         The standard log4net SmtpAppender doesn't support SSL authentication, which is required
    ///         to send email via gmail.
    ///     </para>
    ///     <para>
    ///         This appender uses the SmtpClient (only available in .NET 2.0) to send SMTP mail that
    ///         is secured via SSL.  This is needed to talk to the gmail SMTP server.
    ///     </para>
    ///     <para>
    ///         This code is heavily based on that posted by Ron Grabowski at: http://mail-
    ///         archives.apache.org/mod_mbox/logging-log4net-
    ///         user/200602.mbox/%3C20060216123155.22007.qmail@web32202.mail.mud.yahoo.com%3E.
    ///     </para>
    /// </summary>
    [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP0100:AdvancedNamingRules",
        Justification = "Reviewed. Suppression is OK here.")]
    public class SslSmtpAppender : SmtpAppender
    {
        #region Public Events

        /// <summary>
        ///     Event queue for all listeners interested in formatting message being sent.
        /// </summary>
        public event EventHandler<EmailLogMessageFormattingEventArgs> EmailLogMessageFormatting;

        #endregion

        #region Public Properties

        /// <summary>Gets or sets a value indicating whether the SSL is enabled.</summary>
        /// <value>
        ///     <see langword="true" /> if SSL is enabled, <see langword="false" /> if not.
        /// </value>
        public bool EnableSsl { get; set; }

        #endregion

        #region Methods

        /// <summary>Sends the contents of the cyclic buffer as an e-mail message.</summary>
        /// <param name="events">The logging events to send.</param>
        protected override void SendBuffer(LoggingEvent[] events)
        {
            try
            {
                var writer = new StringWriter(CultureInfo.InvariantCulture);
                string t = this.Layout.Header;
                if (t != null)
                {
                    writer.Write(t);
                }

                foreach (LoggingEvent eEvent in events)
                {
                    // Render the event and append the text to the buffer
                    this.RenderLoggingEvent(writer, eEvent);
                }

                t = this.Layout.Footer;
                if (t != null)
                {
                    writer.Write(t);
                }

                // Use SmtpClient so we can use SSL.
                var client = new SmtpClient(this.SmtpHost, this.Port);
                client.EnableSsl = this.EnableSsl;
                client.Credentials = new NetworkCredential(this.Username, this.Password);
                string subject = this.Subject;
                string body = writer.ToString();

                if (this.EmailLogMessageFormatting != null)
                {
                    var args = new EmailLogMessageFormattingEventArgs(subject, body);
                    foreach (EventHandler<EmailLogMessageFormattingEventArgs> beforeSend in
                        this.EmailLogMessageFormatting.GetInvocationList())
                    {
                        beforeSend(this, args);
                        if (args.Handled)
                        {
                            break;
                        }
                    }

                    subject = args.Subject;
                    body = args.Body;
                }

                var mail = new MailMessage(this.From, this.To, subject, body);
                client.Send(mail);
            }
            catch (Exception e)
            {
                this.ErrorHandler.Error("Error occurred while sending e-mail notification from SmtpClientSmtpAppender.", e);
            }
        }

        #endregion
    }
}
