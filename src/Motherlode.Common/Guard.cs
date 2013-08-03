// ****************************************************************
// Copyright 2012, Stephan Burguchev
// e-mail: me@sburg.net
// ****************************************************************
// *

using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace Motherlode.Common
{
    /// <summary>
    ///     The guard provides static methods to simplify the arguments check required for public
    ///     interface.
    /// </summary>
    /// <remarks>
    ///     This class provides helper methods for asserting the validity of arguments. It can be used to
    ///     reduce the number of laborious <c>if</c>, <c>throw</c> sequences in your code.
    /// </remarks>
    public static class Guard
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Ensures the specified enumeration argument is a valid member of the
        ///     <typeparamref name="TEnum" /> enumeration. If it is not, an <see cref="ArgumentException" /> is
        ///     thrown.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This method can be used to validate all publicly-supplied enumeration values. Without such
        ///         an assertion, it is possible to cast any <c>int</c> value to the enumeration type and pass
        ///         it in.
        ///     </para>
        ///     <para>
        ///         This method works for both flags and non-flags enumerations. In the case of a flags
        ///         enumeration, any combination of values in the enumeration is accepted. In the case of a non-
        ///         flags enumeration, the specified value must be equal to one of the values in the enumeration.
        ///     </para>
        /// </remarks>
        /// <typeparam name="TEnum">The enumeration type.</typeparam>
        /// <param name="arg">The expression containing the parameter variable.</param>
        /// <exception cref="ArgumentException">
        ///     Thrown if specified argument is not a valid member of the
        ///     <typeparamref name="TEnum" /> enumeration.
        /// </exception>
        [DebuggerHidden]
        public static void IsEnumMember<TEnum>(Expression<Func<TEnum>> arg)
            where TEnum : struct, IConvertible
        {
            TEnum enumValue = arg.Compile()();
            if (Attribute.IsDefined(typeof(TEnum), typeof(FlagsAttribute), false))
            {
                // Flag enumeration - we can only get here if TEnum is a valid enumeration type, since the FlagsAttribute can
                // only be applied to enumerations
                bool throwEx;
                long longValue = enumValue.ToInt64(CultureInfo.InvariantCulture);

                if (longValue == 0)
                {
                    // Only throw if zero isn't defined in the enum - we have to convert zero to the underlying type of the enum
                    throwEx = !Enum.IsDefined(
                        typeof(TEnum),
                        ((IConvertible)0).ToType(Enum.GetUnderlyingType(typeof(TEnum)), CultureInfo.InvariantCulture));
                }
                else
                {
                    longValue = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Aggregate(
                        longValue, (current, value) => current & ~value.ToInt64(CultureInfo.InvariantCulture));

                    // Throw if there is a value left over after removing all valid values
                    throwEx = longValue != 0;
                }

                if (throwEx)
                {
                    var param = (MemberExpression)arg.Body;
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Enum value '{0}' is not valid for flags enumeration '{1}'.",
                            enumValue,
                            typeof(TEnum).FullName),
                        param.Member.Name);
                }
            }
            else
            {
                // Not a flag enumeration
                if (!Enum.IsDefined(typeof(TEnum), enumValue))
                {
                    var param = (MemberExpression)arg.Body;
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Enum value '{0}' is not defined for enumeration '{1}'.",
                            enumValue,
                            typeof(TEnum).FullName),
                        param.Member.Name);
                }
            }
        }

        /// <summary>
        ///     Ensures the specified argument is non-<see langword="null" />. If it is, an
        ///     <see cref="ArgumentNullException" /> is thrown.
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="arg">The expression containing the parameter variable.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if specified argument is <see langword="null" />.
        /// </exception>
        [DebuggerHidden]
        public static void IsNotNull<T>(Expression<Func<T>> arg) where T : class
        {
            if (arg.Compile()() != null)
            {
                return;
            }

            var param = (MemberExpression)arg.Body;
            throw new ArgumentNullException(param.Member.Name);
        }

        [DebuggerHidden]
        public static void IsNotNull<T>(Expression<Func<T>> arg, T value) where T : class
        {
            if (value != null)
            {
                return;
            }

            var param = (MemberExpression)arg.Body;
            throw new ArgumentNullException(param.Member.Name);
        }

        /// <summary>
        ///     Ensures the specified nullable value argument is non-<see langword="null" />. If it is, an
        ///     <see cref="ArgumentNullException" /> is thrown.
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="arg">The expression containing the parameter variable.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if specified argument is <see langword="null" />.
        /// </exception>
        [DebuggerHidden]
        public static void IsNotNull<T>(Expression<Func<T?>> arg) where T : struct
        {
            if (arg.Compile()().HasValue)
            {
                return;
            }

            var param = (MemberExpression)arg.Body;
            throw new ArgumentNullException(param.Member.Name);
        }

        /// <summary>
        ///     Ensures the specified argument is non-<see langword="null" />. If it is, an
        ///     <see cref="ArgumentNullException" /> is thrown.
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="value">Parameter value.</param>
        /// <param name="paramName">Parameter name.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if specified argument is <see langword="null" />.
        /// </exception>
        [DebuggerHidden]
        public static void IsNotNull<T>(T value, string paramName) where T : class
        {
            if (value != null)
            {
                return;
            }

            throw new ArgumentNullException(paramName);
        }

        /// <summary>
        ///     Ensures the specified argument is non-<see langword="null" />. If it is, an
        ///     <see cref="ArgumentNullException" /> is thrown.
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="value">Parameter value.</param>
        /// <param name="paramName">Parameter name.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if specified argument is <see langword="null" />.
        /// </exception>
        [DebuggerHidden]
        public static void IsNotNull<T>(T? value, string paramName) where T : struct
        {
            if (value.HasValue)
            {
                return;
            }

            throw new ArgumentNullException(paramName);
        }

        /// <summary>
        ///     Ensures the specified argument is non-<see langword="null" /> and not an empty <c>string</c>.
        ///     If it is, an <see cref="ArgumentNullException" /> is thrown.
        /// </summary>
        /// <param name="arg">The expression containing the parameter variable.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if specified argument is <see langword="null" /> or an empty <c>string</c>.
        /// </exception>
        [DebuggerHidden]
        public static void IsNotNullOrEmpty(Expression<Func<string>> arg)
        {
            if (!string.IsNullOrEmpty(arg.Compile()()))
            {
                return;
            }

            var param = (MemberExpression)arg.Body;
            string paramName = param.Member.Name;
            throw new ArgumentNullException(paramName, "Value cannot be null or empty.");
        }

        /// <summary>
        ///     Ensures the specified argument is non-<see langword="null" /> and not an empty <c>string</c>.
        ///     If it is, an <see cref="ArgumentNullException" /> is thrown.
        /// </summary>
        /// <param name="value">Parameter value.</param>
        /// <param name="paramName">Parameter name.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if specified argument is <see langword="null" /> or an empty <c>string</c>.
        /// </exception>
        [DebuggerHidden]
        public static void IsNotNullOrEmpty(string value, string paramName)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return;
            }

            throw new ArgumentNullException(paramName, "Value cannot be null or empty.");
        }

        /// <summary>
        ///     Ensures the specified argument is non-<see langword="null" />, not an empty <c>string</c> and
        ///     contains not only the whitespace characters. If it is, an <see cref="ArgumentNullException" /> is
        ///     thrown.
        /// </summary>
        /// <param name="arg">The expression containing the parameter variable.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if specified argument is <see langword="null" />, an empty <c>string</c> or a string
        ///     containing whitespaces only.
        /// </exception>
        [DebuggerHidden]
        public static void IsNotNullOrWhiteSpace(Expression<Func<string>> arg)
        {
            if (!string.IsNullOrWhiteSpace(arg.Compile()()))
            {
                return;
            }

            var param = (MemberExpression)arg.Body;
            string paramName = param.Member.Name;
            throw new ArgumentNullException(paramName, "Value cannot be null or whitespace.");
        }

        /// <summary>
        ///     Ensures the specified argument is non-<see langword="null" />, not an empty <c>string</c> and
        ///     contains not only the whitespace characters. If it is, an <see cref="ArgumentNullException" /> is
        ///     thrown.
        /// </summary>
        /// <param name="value">Parameter value.</param>
        /// <param name="paramName">Parameter name.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if specified argument is <see langword="null" />, an empty <c>string</c> or a string
        ///     containing whitespaces only.
        /// </exception>
        [DebuggerHidden]
        public static void IsNotNullOrWhiteSpace(string value, string paramName)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            throw new ArgumentNullException(paramName, "Value cannot be null or whitespace.");
        }

        #endregion
    }
}
