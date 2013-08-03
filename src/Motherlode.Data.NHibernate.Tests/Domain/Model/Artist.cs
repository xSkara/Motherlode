namespace Motherlode.Data.NHibernate.Tests.Domain.Model
{
    public class Artist : Entity
    {
        #region Public Properties

        [DomainSignature]
        public virtual string Name { get; set; }

        #endregion
    }
}
