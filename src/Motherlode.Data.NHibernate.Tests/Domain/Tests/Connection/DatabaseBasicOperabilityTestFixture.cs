using System.Configuration;
using System.Data.SqlServerCe;
using Motherlode.Data.NHibernate.Tests.Cfg;
using NUnit.Framework;

namespace Motherlode.Data.NHibernate.Tests.Domain.Tests.Connection
{
    [TestFixture]
    public class DatabaseBasicOperabilityTestFixture
    {
        #region Public Methods and Operators

        [Ignore]
        public void UpgradeDatabase()
        {
            new SqlCeEngine(ConfigurationManager.ConnectionStrings["default"].ConnectionString).Upgrade();
            new SqlCeEngine(
                string.Format(
                    @"Data Source={0}\..\..\Domain\Database\Chinook_SqlServerCompact.sdf;", Environment.TargetDirectory)).
                Upgrade();
        }

        [Test]
        public void database_should_be_correctly_opened_with_default_connection_string()
        {
            using (var connection = new SqlCeConnection(ConfigurationManager.ConnectionStrings["default"].ConnectionString))
            {
                connection.Open();
            }
        }

        #endregion
    }
}
