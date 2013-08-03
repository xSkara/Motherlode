using System.Collections.Generic;
using System.Linq;
using Motherlode.Data.NHibernate.Tests.Cfg.Providers;
using Motherlode.Data.NHibernate.Tests.Domain.Model;
using Motherlode.Log4Net;
using NHibernate;
using NUnit.Framework;
using SharpTestsEx;

namespace Motherlode.Data.NHibernate.Tests.Tests
{
    [TestFixture]
    public class NHibernateGeneratorsBehaviorFixture
    {
        #region Constants and Fields

        private DelegateAppender _appender;
        private int _count;

        #endregion

        #region Public Methods and Operators

        [Ignore]
        public void HiLoGeneratorEnablesToUseAdoNetBatch()
        {
            ISessionFactory sessionFactory = new HiLoConfigurationProvider(true).Create().BuildSessionFactory();

            using (ISession session = sessionFactory.OpenSession())
            {
                Artist[] artists = createArtists();

                using (global::NHibernate.ITransaction tx = session.BeginTransaction())
                {
                    foreach (Artist artist in artists)
                    {
                        session.Save(artist);
                    }

                    tx.Commit();
                }
            }

            sessionFactory.Close();
        }

        [Test]
        public void HiLo_generator_should_correctly_generate_identifiers()
        {
            ISessionFactory sessionFactory = new HiLoConfigurationProvider().Create().BuildSessionFactory();

            using (ISession session = sessionFactory.OpenSession())
            {
                Artist[] artists = createArtists();
                Genre[] genres = createGenres();

                using (global::NHibernate.ITransaction tx = session.BeginTransaction())
                {
                    foreach (Artist artist in artists)
                    {
                        session.Save(artist);
                    }

                    foreach (Genre genre in genres)
                    {
                        session.Save(genre);
                    }

                    tx.Commit();
                }

                foreach (Artist artist in artists)
                {
                    artist.Id.Should().Be.GreaterThan(0);
                }

                foreach (Genre genre in genres)
                {
                    genre.Id.Should().Be.GreaterThan(0);
                }
            }

            sessionFactory.Close();
        }

        [Test]
        public void HiLo_generator_should_push_data_only_when_transaction_is_committed()
        {
            ISessionFactory sessionFactory = new HiLoConfigurationProvider().Create().BuildSessionFactory();
            Artist[] artists = createArtists();
            List<Artist> toBeSaved = artists.Take(3).ToList();
            List<Artist> toBeRejected = artists.Skip(3).ToList();

            try
            {
                // Verifying that data modification queries are executed only when commit flushing is performed
                using (ISession session = sessionFactory.OpenSession())
                {
                    this._count = 0;
                    session.FlushMode = FlushMode.Commit;

                    var persistedArtist = session.Get<Artist>(15);
                    var persistedInvoiceLine = session.Get<InvoiceLine>(15);

                    // Making a dirty entity
                    persistedArtist.Name = "Dirty Buddy Guy";

                    // Deleting an entity
                    session.Delete(persistedInvoiceLine);

                    // Saving some entities
                    foreach (Artist artist in toBeSaved)
                    {
                        session.Save(artist);
                    }

                    using (global::NHibernate.ITransaction tx = session.BeginTransaction())
                    {
                        this._count.Should().Be.EqualTo(0);
                        tx.Commit();
                        this._count.Should().Be.EqualTo(5);
                    }
                }

                // Verifying that no queries are executed without commit flushing
                using (ISession session = sessionFactory.OpenSession())
                {
                    this._count = 0;
                    session.FlushMode = FlushMode.Commit;
                    foreach (Artist artist in toBeRejected)
                    {
                        session.Save(artist);
                    }
                }

                this._count.Should().Be.EqualTo(0);

                // Verifying that the previously saved data are located in the database
                using (ISession session = sessionFactory.OpenSession())
                {
                    using (session.BeginTransaction())
                    {
                        foreach (Artist artist in toBeSaved)
                        {
                            var persistedArtist = session.Get<Artist>(artist.Id);
                            persistedArtist.Should().Not.Be.Null();
                        }

                        foreach (Artist artist in toBeRejected)
                        {
                            var persistedArtist = session.Get<Artist>(artist.Id);
                            persistedArtist.Should().Be.Null();
                        }
                    }
                }
            }
            finally
            {
                sessionFactory.Close();
            }
        }

        [SetUp]
        public void SetUp()
        {
            Logger.ConfigureWithAppConfig();
            this._appender = (DelegateAppender)Logger.GetAppender("DelegateAppender");
            this._appender.OnAppend += this.onLoggerMessageAppended;
        }

        [TearDown]
        public void TearDown()
        {
            this._appender.OnAppend -= this.onLoggerMessageAppended;
        }

        [Test]
        public void identity_generator_pushes_data_even_if_transaction_has_not_been_committed()
        {
            ISessionFactory sessionFactory = new IdentityConfigurationProvider().Create().BuildSessionFactory();
            Artist[] artists = createArtists();
            List<Artist> toBeSaved = artists.Take(3).ToList();
            List<Artist> toBeRejected = artists.Skip(3).ToList();

            using (ISession session = sessionFactory.OpenSession())
            {
                this._count = 0;
                session.FlushMode = FlushMode.Commit;
                using (global::NHibernate.ITransaction tx = session.BeginTransaction())
                {
                    foreach (Artist artist in toBeSaved)
                    {
                        session.Save(artist);
                    }

                    this._count.Should().Be.EqualTo(3);
                    tx.Commit();
                    this._count.Should().Be.EqualTo(3);
                }
            }

            using (ISession session = sessionFactory.OpenSession())
            {
                this._count = 0;
                session.FlushMode = FlushMode.Commit;
                foreach (Artist artist in toBeRejected)
                {
                    session.Save(artist);
                }

                this._count.Should().Be.EqualTo(2);
            }

            using (ISession session = sessionFactory.OpenSession())
            {
                using (session.BeginTransaction())
                {
                    foreach (Artist artist in toBeSaved)
                    {
                        var persistedArtist = session.Get<Artist>(artist.Id);
                        persistedArtist.Should().Not.Be.Null();
                    }

                    foreach (Artist artist in toBeRejected)
                    {
                        var persistedArtist = session.Get<Artist>(artist.Id);
                        persistedArtist.Should().Not.Be.Null();
                    }
                }
            }

            sessionFactory.Close();
        }

        #endregion

        #region Methods

        private static Artist[] createArtists()
        {
            return new[]
                {
                    new Artist
                        {
                            Name = "Test 1"
                        },
                    new Artist
                        {
                            Name = "Test 2"
                        },
                    new Artist
                        {
                            Name = "Test 3"
                        },
                    new Artist
                        {
                            Name = "Test 4"
                        },
                    new Artist
                        {
                            Name = "Test 5"
                        },
                };
        }

        private static Genre[] createGenres()
        {
            return new[]
                {
                    new Genre
                        {
                            Name = "Genre 1",
                        },
                    new Genre
                        {
                            Name = "Genre 2",
                        },
                    new Genre
                        {
                            Name = "Genre 3",
                        },
                    new Genre
                        {
                            Name = "Genre 4",
                        },
                    new Genre
                        {
                            Name = "Genre 5",
                        },
                };
        }

        private void onLoggerMessageAppended(object sender, LogMessageAppendedEventArgs args)
        {
            if (args.AppendedData.StartsWith("INSERT INTO") || args.AppendedData.StartsWith("DELETE FROM") ||
                args.AppendedData.StartsWith("UPDATE"))
            {
                this._count++;
            }
        }

        #endregion
    }
}
