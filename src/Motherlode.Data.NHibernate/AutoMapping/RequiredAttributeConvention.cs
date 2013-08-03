using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Motherlode.Data.NHibernate.AutoMapping
{
    public class RequiredAttributeConvention : AttributePropertyConvention<RequiredAttribute>
    {
        #region Methods

        protected override void Apply(RequiredAttribute attribute, IPropertyInstance instance)
        {
            instance.Not.Nullable();
        }

        #endregion
    }
}
