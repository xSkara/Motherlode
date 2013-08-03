// ****************************************************************
// Copyright 2012, Stephan Burguchev
// e-mail: me@sburg.net
// ****************************************************************
// *

using System;
using System.Runtime.Serialization;

namespace Motherlode.Log4Net
{
    /// <summary>Exception for signalling logger errors.</summary>
    [Serializable]
    public class LoggerException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <c>LoggerException</c> class.
        /// </summary>
        public LoggerException()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <c>LoggerException</c> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public LoggerException(string message) : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <c>LoggerException</c> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public LoggerException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        ///     Specialised constructor for use only by derived classes. Serialization constructor.
        /// </summary>
        /// <param name="info">The serialization information.</param>
        /// <param name="context">The streaming context.</param>
        protected LoggerException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        #endregion
    }
}
