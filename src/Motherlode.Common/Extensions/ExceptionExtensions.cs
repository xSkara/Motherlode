// ****************************************************************
// Copyright 2012, Stephan Burguchev
// e-mail: me@sburg.net
// ****************************************************************
// *

using System;
using System.Text;

namespace Motherlode.Common.Extensions
{
    /// <summary>Exception class extension methods.</summary>
    public static class ExceptionExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        ///     An Exception extension method that returns the full exception message including the inner
        ///     exception messages.
        /// </summary>
        /// <param name="ex">The exception to act on.</param>
        /// <returns>The full exception message.</returns>
        public static string GetFullMessage(this Exception ex)
        {
            string message = ex.Message.Trim();
            var sb = new StringBuilder(
                message.EndsWith(".")
                    ? message
                    : message + ".");
            while ((ex = ex.InnerException) != null)
            {
                message = ex.Message.Trim();
                sb.Append(
                    " " + (message.EndsWith(".")
                               ? message
                               : message + "."));
            }

            return sb.ToString();
        }

        #endregion
    }
}
