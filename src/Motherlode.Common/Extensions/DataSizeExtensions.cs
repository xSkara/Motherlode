// ****************************************************************
// Copyright 2012, Stephan Burguchev
// e-mail: me@sburg.net
// ****************************************************************
// *

namespace Motherlode.Common.Extensions
{
    /// <summary>DataSizeFormatProvider class extension methods.</summary>
    public static class DataSizeExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        ///     A long extension method that converts the <paramref name="sizeInBytes" /> value to the data size
        ///     string representation.
        /// </summary>
        /// <param name="sizeInBytes">
        ///     The <paramref name="sizeInBytes" /> to act on.
        /// </param>
        /// <returns>
        ///     <paramref name="sizeInBytes" /> as a data size string.
        /// </returns>
        public static string ToDataSize(this long sizeInBytes)
        {
            return string.Format(new DataSizeFormatProvider(), "{0:ds}", sizeInBytes);
        }

        #endregion
    }
}
