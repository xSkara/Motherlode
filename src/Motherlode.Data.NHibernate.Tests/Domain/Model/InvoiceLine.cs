namespace Motherlode.Data.NHibernate.Tests.Domain.Model
{
    public class InvoiceLine : Entity
    {
        #region Public Properties

        [DomainSignature]
        public virtual Invoice Invoice { get; set; }

        [DomainSignature]
        public virtual int Quantity { get; set; }

        [DomainSignature]
        public virtual Track Track { get; set; }

        [DomainSignature]
        public virtual decimal UnitPrice { get; set; }

        #endregion
    }
}
