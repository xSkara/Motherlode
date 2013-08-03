using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Motherlode.Data.NHibernate.Tests.Domain.Model;
using Motherlode.Data.NHibernate.Tests.Utils;
using NHibernate;
using NHibernate.Bytecode;
using NHibernate.Exceptions;
using NHibernate.Linq;
using NUnit.Framework;
using SharpTestsEx;
using Environment = NHibernate.Cfg.Environment;

namespace Motherlode.Data.NHibernate.Tests.Tests
{
    [TestFixture]
    public class NHibernateInterceptionCompletenessAndInpcImplementationTestFixture
    {
        #region Public Methods and Operators

        [Test]
        public void PropertyChanged_should_be_raised_for_readonly_properties_depending_on_the_changed_one()
        {
            ISessionFactory sessionFactory = Helper.CreateHiLoWpfSessionFactory();

            using (ISession session = sessionFactory.OpenSession())
            {
                using (session.BeginTransaction())
                {
                    var employee = session.Get<Employee>(1);

                    int fullNameCounter = 0;
                    int firstNameCounter = 0;
                    (employee as INotifyPropertyChanged).PropertyChanged += (sender, args) =>
                    {
                        if (args.PropertyName == "FullName")
                        {
                            fullNameCounter++;
                        }
                        else if (args.PropertyName == "FirstName")
                        {
                            firstNameCounter++;
                        }
                    };

                    employee.FirstName = "test";

                    fullNameCounter.Should().Be.EqualTo(1);
                    firstNameCounter.Should().Be.EqualTo(1);
                }
            }

            sessionFactory.Close();
        }

        [Test]
        public void PropertyChanged_should_not_be_raised_if_assigned_value_is_equal_to_the_current_one()
        {
            ISessionFactory sessionFactory = Helper.CreateHiLoWpfSessionFactory(lazyLoading: false);

            using (ISession session = sessionFactory.OpenSession())
            {
                using (global::NHibernate.ITransaction tx = session.BeginTransaction())
                {
                    var playlist = session.Get<Playlist>(1);

                    (playlist is INotifyPropertyChanged).Should().Be.True();

                    int callCount = 0;
                    (playlist as INotifyPropertyChanged).PropertyChanged += (sender, args) =>
                    {
                        callCount++;
                        sender.Should().Be.SameInstanceAs(playlist);
                    };

                    playlist.Name = "same_value";
                    playlist.Name = "same_value";

                    callCount.Should().Be.EqualTo(1);

                    tx.Commit();
                }
            }

            sessionFactory.Close();
        }

        [Test]
        public void interceptor_should_throw_if_DependsOn_attribute_is_incorrect()
        {
            ISessionFactory sessionFactory = Helper.CreateHiLoWpfSessionFactory();

            using (ISession session = sessionFactory.OpenSession())
            {
                using (session.BeginTransaction())
                {
                    var ex = Assert.Throws<GenericADOException>(() => session.Query<IncorrectGenre>().First());
                    ex.InnerException.Should().Be.InstanceOf<FormatException>();
                }
            }

            sessionFactory.Close();
        }

        [Test]
        public void objects_loaded_with_Get_and_disabled_lazy_loading_implement_INotifyPropertyChanged()
        {
            ISessionFactory sessionFactory = Helper.CreateHiLoWpfSessionFactory(lazyLoading: false);

            using (ISession session = sessionFactory.OpenSession())
            {
                using (global::NHibernate.ITransaction tx = session.BeginTransaction())
                {
                    var playlist = session.Get<Playlist>(1);

                    (playlist is INotifyPropertyChanged).Should().Be.True();

                    int callCount = 0;
                    (playlist as INotifyPropertyChanged).PropertyChanged += (sender, args) =>
                    {
                        callCount++;
                        sender.Should().Be.SameInstanceAs(playlist);
                    };

                    playlist.Name = "getl";

                    callCount.Should().Be.EqualTo(1);

                    tx.Commit();
                }
            }

            sessionFactory.Close();
        }

        [Test]
        public void objects_loaded_with_Get_implement_INotifyPropertyChanged()
        {
            ISessionFactory sessionFactory = Helper.CreateHiLoWpfSessionFactory();

            using (ISession session = sessionFactory.OpenSession())
            {
                using (global::NHibernate.ITransaction tx = session.BeginTransaction())
                {
                    var playlist = session.Get<Playlist>(1);

                    (playlist is INotifyPropertyChanged).Should().Be.True();

                    int callCount = 0;
                    (playlist as INotifyPropertyChanged).PropertyChanged += (sender, args) =>
                    {
                        callCount++;
                        sender.Should().Be.SameInstanceAs(playlist);
                    };

                    playlist.Name = "get";

                    callCount.Should().Be.EqualTo(1);

                    tx.Commit();
                }
            }

            sessionFactory.Close();
        }

        [Test]
        public void objects_loaded_with_Load_and_disabled_lazy_loading_implement_INotifyPropertyChanged()
        {
            ISessionFactory sessionFactory = Helper.CreateHiLoWpfSessionFactory(lazyLoading: false);

            using (ISession session = sessionFactory.OpenSession())
            {
                using (global::NHibernate.ITransaction tx = session.BeginTransaction())
                {
                    var playlist = session.Load<Playlist>(1);

                    (playlist is INotifyPropertyChanged).Should().Be.True();

                    int callCount = 0;
                    (playlist as INotifyPropertyChanged).PropertyChanged += (sender, args) =>
                    {
                        callCount++;
                        sender.Should().Be.SameInstanceAs(playlist);
                    };

                    playlist.Name = "loadl";

                    callCount.Should().Be.EqualTo(1);

                    tx.Commit();
                }
            }

            sessionFactory.Close();
        }

        [Test]
        public void objects_loaded_with_Load_and_lazy_loading_should_not_implement_INotifyPropertyChanged()
        {
            ISessionFactory sessionFactory = Helper.CreateHiLoWpfSessionFactory(notifiableProxyFactory: true);

            using (ISession session = sessionFactory.OpenSession())
            {
                using (session.BeginTransaction())
                {
                    var playlist = session.Load<Playlist>(1);

                    (playlist is INotifyPropertyChanged).Should().Be.True();
                    (playlist is IEditableObject).Should().Be.False();
                }
            }

            sessionFactory.Close();
        }

        [Test]
        public void objects_loaded_with_Load_do_not_implement_INotifyPropertyChanged_despite_interceptor()
        {
            // var currentProxyFactoryFactory = Environment.BytecodeProvider.ProxyFactoryFactory;
            ((IInjectableProxyFactoryFactory)Environment.BytecodeProvider).SetProxyFactoryFactory(
                typeof(DefaultProxyFactoryFactory).FullName);
            ISessionFactory sessionFactory = Helper.CreateHiLoWpfSessionFactory();

            using (ISession session = sessionFactory.OpenSession())
            {
                using (session.BeginTransaction())
                {
                    var playlist = session.Load<Playlist>(1);

                    (playlist is INotifyPropertyChanged).Should().Be.False();
                }
            }

            sessionFactory.Close();
        }

        [Test]
        public void objects_loaded_with_Load_implement_INotifyPropertyChanged_using_custom_factory()
        {
            ISessionFactory sessionFactory = Helper.CreateHiLoWpfSessionFactory(notifiableProxyFactory: true);

            using (ISession session = sessionFactory.OpenSession())
            {
                using (session.BeginTransaction())
                {
                    var playlist = session.Load<Playlist>(1);

                    (playlist is INotifyPropertyChanged).Should().Be.True();

                    int callCount = 0;
                    (playlist as INotifyPropertyChanged).PropertyChanged += (sender, args) =>
                    {
                        callCount++;
                        sender.Should().Be.SameInstanceAs(playlist);
                    };

                    playlist.Name = "load";
                    playlist.Name = "load2";

                    callCount.Should().Be.EqualTo(2);
                }
            }

            sessionFactory.Close();
        }

        [Test]
        public void objects_loaded_with_Load_implement_INotifyPropertyChanged_using_custom_factory_even_if_loaded_from_cache()
        {
            ISessionFactory sessionFactory = Helper.CreateHiLoWpfSessionFactory(notifiableProxyFactory: true);

            using (ISession session = sessionFactory.OpenSession())
            {
                using (session.BeginTransaction())
                {
                    var playlist_tmp = session.Load<Playlist>(1);
                    var playlist = session.Load<Playlist>(1);

                    (playlist is INotifyPropertyChanged).Should().Be.True();

                    int callCount = 0;
                    (playlist as INotifyPropertyChanged).PropertyChanged += (sender, args) =>
                    {
                        callCount++;
                        sender.Should().Be.SameInstanceAs(playlist);
                    };

                    playlist.Name = "load";
                    playlist.Name = "load2";

                    callCount.Should().Be.EqualTo(2);
                }
            }

            sessionFactory.Close();
        }

        [Test]
        public void objects_loaded_with_Query_and_disabled_lazy_loading_implement_INotifyPropertyChanged()
        {
            ISessionFactory sessionFactory = Helper.CreateHiLoWpfSessionFactory(lazyLoading: false);

            using (ISession session = sessionFactory.OpenSession())
            {
                using (global::NHibernate.ITransaction tx = session.BeginTransaction())
                {
                    List<Playlist> playlists = session.Query<Playlist>().Take(4).ToList();

                    foreach (Playlist playlist in playlists)
                    {
                        (playlist is INotifyPropertyChanged).Should().Be.True();

                        int callCount = 0;
                        (playlist as INotifyPropertyChanged).PropertyChanged += (sender, args) =>
                        {
                            callCount++;
                            sender.Should().Be.SameInstanceAs(playlist);
                        };

                        playlist.Name = "queryl";

                        callCount.Should().Be.EqualTo(1);
                    }

                    tx.Commit();
                }
            }

            sessionFactory.Close();
        }

        [Test]
        public void objects_loaded_with_Query_implement_INotifyPropertyChanged()
        {
            ISessionFactory sessionFactory = Helper.CreateHiLoWpfSessionFactory();

            using (ISession session = sessionFactory.OpenSession())
            {
                using (global::NHibernate.ITransaction tx = session.BeginTransaction())
                {
                    List<Playlist> playlists = session.Query<Playlist>().Take(4).ToList();

                    foreach (Playlist playlist in playlists)
                    {
                        (playlist is INotifyPropertyChanged).Should().Be.True();

                        int callCount = 0;
                        (playlist as INotifyPropertyChanged).PropertyChanged += (sender, args) =>
                        {
                            callCount++;
                            sender.Should().Be.SameInstanceAs(playlist);
                        };

                        playlist.Name = "query";

                        callCount.Should().Be.EqualTo(1);
                    }

                    tx.Commit();
                }
            }

            sessionFactory.Close();
        }

        #endregion
    }
}
