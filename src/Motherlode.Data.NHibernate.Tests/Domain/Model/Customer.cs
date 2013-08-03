namespace Motherlode.Data.NHibernate.Tests.Domain.Model
{
    public class Customer : Person
    {
        #region Public Properties

        [DomainSignature]
        public virtual string Company { get; set; }

        [DomainSignature]
        public virtual Employee SupportRepresentant { get; set; }

        #endregion
    }
}
