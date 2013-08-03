using System;
using System.Linq;
using FluentNHibernate;
using FluentNHibernate.Automapping;

namespace Motherlode.Data.NHibernate.AutoMapping
{
    public class AutomappingConfiguration : DefaultAutomappingConfiguration
    {
        #region Constants and Fields

        private readonly Type[] _entitiesToIgnore;

        #endregion

        #region Constructors and Destructors

        public AutomappingConfiguration(params Type[] entitiesToIgnore)
        {
            this._entitiesToIgnore = entitiesToIgnore;
        }

        #endregion

        #region Public Methods and Operators

        public override bool ShouldMap(Type type)
        {
            if (this._entitiesToIgnore.Contains(type))
            {
                return false;
            }

            return typeof(Entity).IsAssignableFrom(type);
        }

        public override bool ShouldMap(Member member)
        {
            return member.IsProperty &&
                   (this.IsId(member) || member.MemberInfo.GetCustomAttributes(typeof(DomainSignatureAttribute), true).Any());
        }

        #endregion
    }
}
