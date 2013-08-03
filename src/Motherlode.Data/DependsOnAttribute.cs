using System;

namespace Motherlode.Data
{
    /// <summary>Attribute to describe the properties that the attributed property depends on.</summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public sealed class DependsOnAttribute : Attribute
    {
        #region Constructors and Destructors

        public DependsOnAttribute(string propertyName)
        {
            this.PropertyName = propertyName;
        }

        #endregion

        #region Public Properties

        public string PropertyName { get; private set; }

        #endregion
    }
}
