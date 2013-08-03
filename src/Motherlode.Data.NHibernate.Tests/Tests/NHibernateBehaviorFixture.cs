using System.Data;
using Motherlode.Data.NHibernate.Tests.Cfg.TestFixtureBases;
using Motherlode.Data.NHibernate.Tests.Domain.Model;
using NHibernate;
using NUnit.Framework;

namespace Motherlode.Data.NHibernate.Tests.Tests
{
    [TestFixture]
    public class NHibernateBehaviorFixture : HiLoFluentConfigurationTestFixtureBase
    {
        #region Public Methods and Operators

        [Test]
        public void connection_can_be_recreated_to_assure_that_it_works()
        {
            using (ISession session = this.SessionFactory.OpenSession())
            {
                Playlist playlist;
                using (global::NHibernate.ITransaction tx = session.BeginTransaction())
                {
                    playlist = session.Get<Playlist>(1);
                    tx.Commit();
                }

                playlist.Name = "asd";

                IDbConnection connection = session.Connection;
                connection.Close();
                session.Disconnect();
                session.Reconnect();

                using (global::NHibernate.ITransaction tx = session.BeginTransaction())
                {
                    tx.Commit();
                }
            }
        }

        [Test]
        public void delete_operation_is_irreversible()
        {
            using (ISession session = this.SessionFactory.OpenSession())
            {
                Playlist playlist;
                using (global::NHibernate.ITransaction tx = session.BeginTransaction())
                {
                    playlist = session.Get<Playlist>(17);
                    tx.Commit();
                }

                session.Delete(playlist);

                Assert.Throws<HibernateException>(() => session.Save(playlist));
            }
        }

        #endregion
    }
}
