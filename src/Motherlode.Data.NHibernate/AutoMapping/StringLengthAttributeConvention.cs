using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Motherlode.Data.NHibernate.AutoMapping
{
    public class StringLengthAttributeConvention : AttributePropertyConvention<StringLengthAttribute>
    {
        #region Methods

        protected override void Apply(StringLengthAttribute attribute, IPropertyInstance instance)
        {
            instance.Length(attribute.MaximumLength);
        }

        #endregion
    }
}
