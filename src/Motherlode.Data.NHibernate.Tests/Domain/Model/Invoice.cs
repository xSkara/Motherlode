using System;
using System.Collections.Generic;
using System.Linq;

namespace Motherlode.Data.NHibernate.Tests.Domain.Model
{
    public class Invoice : Entity
    {
        #region Constructors and Destructors

        public Invoice()
        {
            this.Lines = new List<InvoiceLine>();
        }

        #endregion

        #region Public Properties

        [DomainSignature]
        public virtual string BillingAddress { get; set; }

        [DomainSignature]
        public virtual string BillingCity { get; set; }

        [DomainSignature]
        public virtual string BillingCountry { get; set; }

        [DomainSignature]
        public virtual string BillingPostalCode { get; set; }

        [DomainSignature]
        public virtual string BillingState { get; set; }

        [DomainSignature]
        public virtual Customer Customer { get; set; }

        [DomainSignature]
        public virtual DateTime InvoiceDate { get; set; }

        [DomainSignature]
        public virtual IList<InvoiceLine> Lines { get; protected set; }

        public virtual decimal Total
        {
            get
            {
                return this.Lines.Sum(l => l.Quantity * l.UnitPrice);
            }
        }

        #endregion

        #region Public Methods and Operators

        public virtual void AddLine(InvoiceLine line)
        {
            line.Invoice = this;
            this.Lines.Add(line);
        }

        #endregion
    }
}
