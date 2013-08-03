using Motherlode.Data.NHibernate.Tests.Cfg.TestFixtureBases;
using Motherlode.Data.NHibernate.Tests.Domain.Model;
using NHibernate;
using NUnit.Framework;
using SharpTestsEx;

namespace Motherlode.Data.NHibernate.Tests.Domain.Tests.Data
{
    [TestFixture]
    public class MediaTypeDataFixture : IdentityFluentConfigurationTestFixtureBase
    {
        #region Public Methods and Operators

        [Test]
        public void can_get_mediatype_1()
        {
            using (ISession session = this.SessionFactory.OpenSession())
            {
                var mediaType = session.Get<MediaType>(1);
                mediaType.Name.Should().Be.EqualTo("MPEG audio file");
            }
        }

        #endregion
    }
}
