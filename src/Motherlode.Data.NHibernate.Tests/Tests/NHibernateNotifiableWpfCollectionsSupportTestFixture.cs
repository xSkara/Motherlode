using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Motherlode.Data.NHibernate.Tests.Cfg.Providers;
using Motherlode.Data.NHibernate.Tests.Domain.Model;
using Motherlode.Data.NHibernate.Wpf.CollectionsTypeFactory;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Linq;
using NUnit.Framework;
using SharpTestsEx;

namespace Motherlode.Data.NHibernate.Tests.Tests
{
    [TestFixture]
    public class NHibernateNotifiableWpfCollectionsSupportTestFixture
    {
        #region Public Methods and Operators

        [Test]
        public void collections_inside_loaded_objects_implement_INotifyCollectionChanged()
        {
            ISessionFactory sessionFactory = new HiLoWpfConfigurationProvider().Create().BuildSessionFactory();

            Environment.BytecodeProvider.CollectionTypeFactory.Should().Be.OfType<WpfCollectionTypeFactory>();

            using (ISession session = sessionFactory.OpenSession())
            {
                using (global::NHibernate.ITransaction tx = session.BeginTransaction())
                {
                    List<Playlist> playlists = session.Query<Playlist>().Take(1).ToList();
                    var mediaType = session.Get<MediaType>(1);

                    foreach (Playlist playlist in playlists)
                    {
                        (playlist.Tracks is INotifyCollectionChanged).Should().Be.True();

                        int callCount = 0;
                        (playlist.Tracks as INotifyCollectionChanged).CollectionChanged += (sender, args) => { callCount++; };

                        var track = new Track
                            {
                                Name = "Track!",
                                MediaType = mediaType,
                            };
                        session.Save(track);

                        playlist.Tracks.Add(track);
                        playlist.Tracks.Remove(playlist.Tracks.First());

                        callCount.Should().Be.EqualTo(2);
                    }

                    tx.Commit();
                }
            }

            sessionFactory.Close();
        }

        #endregion
    }
}
