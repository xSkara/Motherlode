namespace Motherlode.Data.NHibernate.Tests.Domain.Model
{
    public class Track : Entity
    {
        //public virtual int TrackId { get; private set; }

        #region Public Properties

        [DomainSignature]
        public virtual Album Album { get; set; }

        [DomainSignature]
        public virtual int Bytes { get; set; }

        [DomainSignature]
        public virtual string Composer { get; set; }

        [DomainSignature]
        public virtual Genre Genre { get; set; }

        [DomainSignature]
        public virtual MediaType MediaType { get; set; }

        [DomainSignature]
        public virtual int Milliseconds { get; set; }

        [DomainSignature]
        public virtual string Name { get; set; }

        [DomainSignature]
        public virtual decimal UnitPrice { get; set; }

        #endregion
    }
}
