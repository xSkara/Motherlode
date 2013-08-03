// ****************************************************************
// Copyright 2012, Stephan Burguchev
// e-mail: me@sburg.net
// ****************************************************************
// *

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Motherlode.Common.Extensions
{
    [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP0100:AdvancedNamingRules",
        Justification = "Reviewed. Suppression is OK here.")]
    public static class IEnumerableExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        ///     An IEnumerable&lt;T&gt; extension method that applies an operation to all items in
        ///     this collection.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="collection">The collection to act on.</param>
        /// <param name="action">The action.</param>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (T item in collection)
            {
                action(item);
            }
        }

        /// <summary>
        ///     An <see cref="IEnumerable" /> extension method that applies an operation to all items in this
        ///     collection.
        /// </summary>
        /// <param name="enumerable">The enumerable to act on.</param>
        /// <param name="action">The action.</param>
        public static void ForEach(this IEnumerable enumerable, Action<object> action)
        {
            foreach (object item in enumerable)
            {
                action(item);
            }
        }

        #endregion
    }
}
