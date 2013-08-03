using Motherlode.Data.NHibernate.Tests.Cfg.TestFixtureBases;
using Motherlode.Data.NHibernate.Tests.Domain.Model;
using NHibernate;
using NUnit.Framework;
using SharpTestsEx;

namespace Motherlode.Data.NHibernate.Tests.Domain.Tests.Data
{
    [TestFixture]
    public class EmployeeDataFixture : IdentityFluentConfigurationTestFixtureBase
    {
        #region Public Methods and Operators

        [Test]
        public void can_get_employee_1()
        {
            using (ISession session = this.SessionFactory.OpenSession())
            {
                var employee = session.Get<Employee>(2);
                employee.FirstName.Should().Be.EqualTo("Nancy");
                employee.LastName.Should().Be.EqualTo("Edwards");
            }
        }

        #endregion
    }
}
