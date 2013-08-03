// ****************************************************************
// Copyright 2012, Stephan Burguchev
// e-mail: me@sburg.net
// ****************************************************************
// *

namespace Motherlode.Common
{
    /// <summary>
    ///     A static wrapper for the <c>AssemblyAttributesReader</c> class.
    /// </summary>
    public static class AssemblyAttributes
    {
        #region Constants and Fields

        private static readonly AssemblyAttributesReader _reader = new AssemblyAttributesReader();

        #endregion

        #region Public Properties

        /// <summary>Gets the company produced the assembly.</summary>
        /// <value>The assembly company.</value>
        public static string AssemblyCompany
        {
            get
            {
                return _reader.AssemblyCompany;
            }
        }

        /// <summary>Gets the assembly configuration.</summary>
        /// <value>The assembly configuration.</value>
        public static string AssemblyConfiguration
        {
            get
            {
                return _reader.AssemblyConfiguration;
            }
        }

        /// <summary>Gets the assembly copyright text.</summary>
        /// <value>The assembly copyright text.</value>
        public static string AssemblyCopyright
        {
            get
            {
                return _reader.AssemblyCopyright;
            }
        }

        /// <summary>Gets the information describing the assembly.</summary>
        /// <value>Information describing the assembly.</value>
        public static string AssemblyDescription
        {
            get
            {
                return _reader.AssemblyDescription;
            }
        }

        /// <summary>Gets the product containing the assembly.</summary>
        /// <value>The product containing the assembly.</value>
        public static string AssemblyProduct
        {
            get
            {
                return _reader.AssemblyProduct;
            }
        }

        /// <summary>Gets the assembly title.</summary>
        /// <value>The assembly title.</value>
        public static string AssemblyTitle
        {
            get
            {
                return _reader.AssemblyTitle;
            }
        }

        /// <summary>Gets the assembly version.</summary>
        /// <value>The assembly version.</value>
        public static string AssemblyVersion
        {
            get
            {
                return _reader.AssemblyVersion;
            }
        }

        #endregion
    }
}
