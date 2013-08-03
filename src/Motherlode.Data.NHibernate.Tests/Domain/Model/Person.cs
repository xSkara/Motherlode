namespace Motherlode.Data.NHibernate.Tests.Domain.Model
{
    public class Person : Entity
    {
        #region Public Properties

        [DomainSignature]
        public virtual string Address { get; set; }

        [DomainSignature]
        public virtual string City { get; set; }

        [DomainSignature]
        public virtual string Country { get; set; }

        [DomainSignature]
        public virtual string Email { get; set; }

        [DomainSignature]
        public virtual string Fax { get; set; }

        [DomainSignature]
        public virtual string FirstName { get; set; }

        [DependsOn("FirstName")]
        [DependsOn("LastName")]
        public virtual string FullName
        {
            get
            {
                return string.Format("{0} {1}", this.FirstName, this.LastName);
            }
        }

        [DomainSignature]
        public virtual string LastName { get; set; }

        [DomainSignature]
        public virtual string Phone { get; set; }

        [DomainSignature]
        public virtual string PostalCode { get; set; }

        [DomainSignature]
        public virtual string State { get; set; }

        #endregion
    }
}
