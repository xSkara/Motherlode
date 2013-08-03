// ****************************************************************
// Copyright 2012, Stephan Burguchev
// e-mail: me@sburg.net
// ****************************************************************
// *

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Motherlode.Common
{
    /// <summary>The format provider formatting a number as a size of information element.</summary>
    public class DataSizeFormatProvider : IFormatProvider, ICustomFormatter
    {
        #region Constants and Fields

        private const string FileSizeFormat = "ds";
        private const decimal OneGigaByte = OneMegaByte * 1024M;

        private const decimal OneKiloByte = 1024M;

        private const decimal OneMegaByte = OneKiloByte * 1024M;

        private const decimal OneTeraByte = OneGigaByte * 1024M;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Converts the value of a specified object to an equivalent string representation using
        ///     specified format and culture-specific formatting information.
        /// </summary>
        /// <param name="format">A format string containing formatting specifications.</param>
        /// <param name="arg">An object to format.</param>
        /// <param name="formatProvider">
        ///     An object that supplies format information about the current instance.
        /// </param>
        /// <returns>
        ///     The string representation of the value of <paramref name="arg" />, formatted as specified by
        ///     <paramref name="format" /> and <paramref name="formatProvider" />.
        /// </returns>
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (format == null ||
                !format.StartsWith(FileSizeFormat))
            {
                return defaultFormat(format, arg, formatProvider);
            }

            if (arg is string)
            {
                return defaultFormat(format, arg, formatProvider);
            }

            decimal size;

            try
            {
                size = Convert.ToDecimal(arg);
            }
            catch (InvalidCastException)
            {
                return defaultFormat(format, arg, formatProvider);
            }

            var suffixText = new Dictionary<string, string[]>
                {
                    { "en", new[] { " TB", " GB", " MB", " KB", " B" } },
                    { "ru", new[] { " าม", " รม", " ฬม", " สม", " ม" } },
                };
            string[] suffixesArray = suffixText[CultureInfo.CurrentCulture.TwoLetterISOLanguageName];

            string suffix;
            if (size > OneTeraByte)
            {
                size /= OneTeraByte;
                suffix = suffixesArray[0];
            }
            else if (size > OneGigaByte)
            {
                size /= OneGigaByte;
                suffix = suffixesArray[1];
            }
            else if (size > OneMegaByte)
            {
                size /= OneMegaByte;
                suffix = suffixesArray[2];
            }
            else if (size > OneKiloByte)
            {
                size /= OneKiloByte;
                suffix = suffixesArray[3];
            }
            else
            {
                suffix = suffixesArray[4];
            }

            return string.Format("{0}{1}", Math.Round(size, 2), suffix);
        }

        /// <summary>
        ///     Returns an object that provides formatting services for the specified type.
        /// </summary>
        /// <param name="formatType">
        ///     An object that specifies the type of format object to return.
        /// </param>
        /// <returns>
        ///     An instance of the object specified by <paramref name="formatType" />, if the
        ///     <see cref="T:System.IFormatProvider" /> implementation can supply that type of object;
        ///     otherwise, null.
        /// </returns>
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
            {
                return this;
            }

            return null;
        }

        #endregion

        #region Methods

        private static string defaultFormat(string format, object arg, IFormatProvider formatProvider)
        {
            var formattableArg = arg as IFormattable;
            if (formattableArg != null)
            {
                return formattableArg.ToString(format, formatProvider);
            }

            return arg.ToString();
        }

        #endregion
    }
}
