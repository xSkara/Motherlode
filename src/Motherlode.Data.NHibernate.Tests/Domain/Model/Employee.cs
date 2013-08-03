using System;

namespace Motherlode.Data.NHibernate.Tests.Domain.Model
{
    public class Employee : Person
    {
        #region Public Properties

        [DomainSignature]
        public virtual DateTime BirthDate { get; set; }

        [DomainSignature]
        public virtual DateTime HireDate { get; set; }

        [DomainSignature]
        public virtual Employee ReportsTo { get; set; }

        [DomainSignature]
        public virtual string Title { get; set; }

        #endregion
    }
}
