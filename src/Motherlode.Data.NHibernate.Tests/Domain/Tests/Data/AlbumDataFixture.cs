using Motherlode.Data.NHibernate.Tests.Cfg.TestFixtureBases;
using Motherlode.Data.NHibernate.Tests.Domain.Model;
using NHibernate;
using NUnit.Framework;
using SharpTestsEx;

namespace Motherlode.Data.NHibernate.Tests.Domain.Tests.Data
{
    [TestFixture]
    public class AlbumDataFixture : IdentityFluentConfigurationTestFixtureBase
    {
        #region Public Methods and Operators

        [Test]
        public void can_get_album_1()
        {
            using (ISession session = this.SessionFactory.OpenSession())
            {
                var album = session.Get<Album>(1);
                album.Artist.Id.Should().Be.EqualTo(1);
                album.Title.Should().Be.EqualTo("For Those About To Rock We Salute You");
                album.Tracks.Count.Should().Be.EqualTo(10);
                album.Tracks[0].Name.Should().Be.EqualTo("For Those About To Rock (We Salute You)");
                album.Tracks[0].MediaType.Id.Should().Be.EqualTo(1);
                album.Tracks[0].Composer.Should().Be.EqualTo("Angus Young, Malcolm Young, Brian Johnson");
            }
        }

        #endregion
    }
}
