using System.Linq;
using Motherlode.Data.NHibernate.Tests.Cfg.TestFixtureBases;
using Motherlode.Data.NHibernate.Tests.Domain.Model;
using NHibernate;
using NUnit.Framework;
using SharpTestsEx;

namespace Motherlode.Data.NHibernate.Tests.Domain.Tests.Data
{
    [TestFixture]
    public class PlaylistDataFixture : IdentityFluentConfigurationTestFixtureBase
    {
        #region Public Methods and Operators

        [Test]
        public void can_get_Playlist_1()
        {
            using (ISession session = this.SessionFactory.OpenSession())
            {
                var playlist = session.Get<Playlist>(1);
                playlist.Name.Should().Be.EqualTo("Music");
                playlist.Tracks.Count.Should().Be.EqualTo(3290);
                playlist.Tracks.First().Name.Should().Be.EqualTo("For Those About To Rock (We Salute You)");
                playlist.Tracks.First().MediaType.Id.Should().Be.EqualTo(1);
                playlist.Tracks.First().Composer.Should().Be.EqualTo("Angus Young, Malcolm Young, Brian Johnson");
            }
        }

        #endregion
    }
}
