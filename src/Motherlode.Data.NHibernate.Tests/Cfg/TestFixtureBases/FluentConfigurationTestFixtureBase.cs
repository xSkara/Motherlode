using NHibernate;
using NUnit.Framework;

namespace Motherlode.Data.NHibernate.Tests.Cfg.TestFixtureBases
{
    public class FluentConfigurationTestFixtureBase<T> where T : IConfigurationProvider, new()
    {
        #region Properties

        protected ISessionFactory SessionFactory { get; private set; }

        #endregion

        #region Public Methods and Operators

        [TestFixtureSetUp]
        public void SetUp()
        {
            this.SessionFactory = new T().Create().BuildSessionFactory();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            this.SessionFactory.Close();
        }

        #endregion
    }
}
