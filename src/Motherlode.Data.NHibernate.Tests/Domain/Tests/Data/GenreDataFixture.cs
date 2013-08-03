using Motherlode.Data.NHibernate.Tests.Cfg.TestFixtureBases;
using Motherlode.Data.NHibernate.Tests.Domain.Model;
using NHibernate;
using NUnit.Framework;
using SharpTestsEx;

namespace Motherlode.Data.NHibernate.Tests.Domain.Tests.Data
{
    [TestFixture]
    public class GenreDataFixture : IdentityFluentConfigurationTestFixtureBase
    {
        #region Public Methods and Operators

        [Test]
        public void can_get_genre_1()
        {
            using (ISession session = this.SessionFactory.OpenSession())
            {
                var genre = session.Get<Genre>(1);

                genre.Name.Should().Be.EqualTo("Rock");
            }
        }

        #endregion
    }
}
