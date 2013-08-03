using Motherlode.Data.NHibernate.Tests.Cfg.TestFixtureBases;
using Motherlode.Data.NHibernate.Tests.Domain.Model;
using NHibernate;
using NUnit.Framework;

namespace Motherlode.Data.NHibernate.Tests.Domain.Tests.Data
{
    [TestFixture]
    public class ArtistDataFixture : IdentityFluentConfigurationTestFixtureBase
    {
        #region Public Methods and Operators

        [Test]
        public void can_get_artist_1()
        {
            using (ISession session = this.SessionFactory.OpenSession())
            {
                var artist = session.Get<Artist>(1);

                Assert.AreEqual("AC/DC", artist.Name);
            }
        }

        #endregion
    }
}
