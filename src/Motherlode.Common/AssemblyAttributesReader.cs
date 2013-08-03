// ****************************************************************
// Copyright 2012, Stephan Burguchev
// e-mail: me@sburg.net
// ****************************************************************
// *

using System;
using System.IO;
using System.Reflection;

namespace Motherlode.Common
{
    /// <summary>
    ///     The reader that enables to retrieve common assembly attributes.
    /// </summary>
    public class AssemblyAttributesReader
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <c>AssemblyAttributesReader</c> class with the
        ///     <paramref name="assembly" />.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public AssemblyAttributesReader(Assembly assembly)
        {
            Guard.IsNotNull(() => assembly);

            this.Assembly = assembly;
        }

        /// <summary>
        ///     Initializes a new instance of the <c>AssemblyAttributesReader</c> class with the entry assembly.
        /// </summary>
        public AssemblyAttributesReader()
        {
            this.Assembly = Assembly.GetEntryAssembly();
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the assembly being examined.</summary>
        /// <value>The assembly.</value>
        public Assembly Assembly { get; private set; }

        /// <summary>Gets the company produced the assembly.</summary>
        /// <value>The assembly company.</value>
        public string AssemblyCompany
        {
            get
            {
                var title = this.getAssemblyAttribute<AssemblyCompanyAttribute>();
                if (title == null ||
                    string.IsNullOrEmpty(title.Company))
                {
                    return "";
                }
                return title.Company;
            }
        }

        /// <summary>Gets the assembly configuration.</summary>
        /// <value>The assembly configuration.</value>
        public string AssemblyConfiguration
        {
            get
            {
                var title = this.getAssemblyAttribute<AssemblyConfigurationAttribute>();
                if (title == null ||
                    string.IsNullOrEmpty(title.Configuration))
                {
                    return "";
                }
                return title.Configuration;
            }
        }

        /// <summary>Gets the assembly copyright text.</summary>
        /// <value>The assembly copyright text.</value>
        public string AssemblyCopyright
        {
            get
            {
                var title = this.getAssemblyAttribute<AssemblyCopyrightAttribute>();
                if (title == null ||
                    string.IsNullOrEmpty(title.Copyright))
                {
                    return "";
                }
                return title.Copyright;
            }
        }

        /// <summary>Gets the information describing the assembly.</summary>
        /// <value>Information describing the assembly.</value>
        public string AssemblyDescription
        {
            get
            {
                var title = this.getAssemblyAttribute<AssemblyDescriptionAttribute>();
                if (title == null ||
                    string.IsNullOrEmpty(title.Description))
                {
                    return "";
                }
                return title.Description;
            }
        }

        /// <summary>Gets the product containing the assembly.</summary>
        /// <value>The product containing the assembly.</value>
        public string AssemblyProduct
        {
            get
            {
                var title = this.getAssemblyAttribute<AssemblyProductAttribute>();
                if (title == null ||
                    string.IsNullOrEmpty(title.Product))
                {
                    return "";
                }
                return title.Product;
            }
        }

        /// <summary>Gets the assembly title.</summary>
        /// <value>The assembly title.</value>
        public string AssemblyTitle
        {
            get
            {
                string defaultTitle = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().CodeBase);
                var title = this.getAssemblyAttribute<AssemblyTitleAttribute>();
                if (title == null ||
                    string.IsNullOrEmpty(title.Title))
                {
                    // If there was no Title attribute, or if the Title attribute was the empty string, return the .exe name
                    return defaultTitle;
                }

                // If it is not an empty string, return it
                return title.Title;
            }
        }

        /// <summary>Gets the assembly version.</summary>
        /// <value>The assembly version.</value>
        public string AssemblyVersion
        {
            get
            {
                var attr = this.getAssemblyAttribute<AssemblyVersionAttribute>();
                if (attr == null ||
                    string.IsNullOrEmpty(attr.Version))
                {
                    var attr2 = this.getAssemblyAttribute<AssemblyFileVersionAttribute>();
                    if (attr2 == null ||
                        string.IsNullOrEmpty(attr2.Version))
                    {
                        return "";
                    }
                    return attr2.Version;
                }
                return attr.Version;
            }
        }

        #endregion

        #region Methods

        private TAttr getAssemblyAttribute<TAttr>() where TAttr : Attribute
        {
            // Get all Title attributes on this assembly
            object[] attributes = this.Assembly.GetCustomAttributes(typeof(TAttr), false);

            // If there is at least one attribute
            if (attributes.Length > 0)
            {
                // Select the first one
                var attribute = (TAttr)attributes[0];
                return attribute;
            }

            return null;
        }

        #endregion
    }
}
