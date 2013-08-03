using System;
using Motherlode.Data.NHibernate.Tests.Cfg.TestFixtureBases;
using Motherlode.Data.NHibernate.Tests.Domain.Model;
using NHibernate;
using NUnit.Framework;
using SharpTestsEx;

namespace Motherlode.Data.NHibernate.Tests.Domain.Tests.Data
{
    [TestFixture]
    public class InvoiceDataFixture : IdentityFluentConfigurationTestFixtureBase
    {
        #region Public Methods and Operators

        [Test]
        public void can_get_invoice_1()
        {
            using (ISession session = this.SessionFactory.OpenSession())
            {
                var invoice = session.Get<Invoice>(1);
                invoice.Customer.Id.Should().Be.EqualTo(2);
                invoice.InvoiceDate.Should().Be.EqualTo(new DateTime(2007, 01, 01));
                /*invoice.BillingAddress.Should().Be.EqualTo("3 Chatham Street");
                invoice.BillingCity.Should().Be.EqualTo("Dublin");
                invoice.BillingState.Should().Be.EqualTo("Dublin");
                invoice.BillingCountry.Should().Be.EqualTo("Ireland");
                invoice.BillingPostalCode.Should().Be.Null();
                invoice.Total.Should().Be.EqualTo((decimal)3.96);
                invoice.Lines.Count.Should().Be.EqualTo(4);*/

                InvoiceLine invoiceLine = invoice.Lines[0];
                invoiceLine.Id.Should().Be.EqualTo(1);
                invoiceLine.UnitPrice.Should().Be.EqualTo((decimal)0.99);
                invoiceLine.Quantity.Should().Be.EqualTo(1);
            }
        }

        #endregion
    }
}
