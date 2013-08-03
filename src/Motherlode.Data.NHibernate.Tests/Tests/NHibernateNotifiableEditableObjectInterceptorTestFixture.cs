using System.ComponentModel;
using Motherlode.Data.NHibernate.Tests.Domain.Model;
using Motherlode.Data.NHibernate.Tests.Utils;
using Motherlode.Data.NHibernate.Wpf.Interceptor;
using Motherlode.Data.NHibernate.Wpf.Interceptor.ObjectFactory;
using NHibernate;
using NUnit.Framework;
using SharpTestsEx;

namespace Motherlode.Data.NHibernate.Tests.Tests
{
    [TestFixture]
    public class NHibernateNotifiableEditableObjectInterceptorTestFixture
    {
        #region Public Methods and Operators

        [Test]
        public void INPC_should_fire_even_if_IEO_transaction_is_in_active()
        {
            ISessionFactory sessionFactory = Helper.CreateHiLoWpfSessionFactory(
                () => new ObjectFactoryInterceptor(new NotifiableEditableObjectsFactory()));

            using (ISession session = sessionFactory.OpenSession())
            {
                var playlist = session.Get<Playlist>(1);

                var ieditable = (IEditableObject)playlist;
                var inpc = (INotifyPropertyChanged)playlist;

                int count = 0;
                inpc.PropertyChanged += (sender, args) => count++;

                string oldValue = playlist.Name;
                ieditable.BeginEdit();
                playlist.Name = playlist.Name + "_modified";
                ieditable.CancelEdit();
                playlist.Name.Should().Be.EqualTo(oldValue);

                playlist.Name = playlist.Name + "_modified2";

                count.Should().Be.EqualTo(2);
            }

            sessionFactory.Close();
        }

        [Test]
        public void objects_should_implement_INPC_and_IEditableObject()
        {
            ISessionFactory sessionFactory = Helper.CreateHiLoWpfSessionFactory(
                () => new ObjectFactoryInterceptor(new NotifiableEditableObjectsFactory()));

            using (ISession session = sessionFactory.OpenSession())
            {
                var playlist = session.Get<Playlist>(1);

                (playlist is IEditableObject).Should().Be.True();
                (playlist is INotifyPropertyChanged).Should().Be.True();
            }

            sessionFactory.Close();
        }

        [Test]
        public void objects_should_implement_INPC_and_IEditableObject_if_its_loaded_with_Load_method_and_lazyLoading()
        {
            ISessionFactory sessionFactory = Helper.CreateHiLoWpfSessionFactory(
                () => new ObjectFactoryInterceptor(new NotifiableEditableObjectsFactory()),
                false,
                false,
                true);

            using (ISession session = sessionFactory.OpenSession())
            {
                var playlist = session.Load<Playlist>(1);

                (playlist is IEditableObject).Should().Be.True();
                (playlist is INotifyPropertyChanged).Should().Be.True();

                var ieditable = (IEditableObject)playlist;
                var inpc = (INotifyPropertyChanged)playlist;

                int count = 0;
                inpc.PropertyChanged += (sender, args) => count++;

                string oldValue = playlist.Name;
                ieditable.BeginEdit();
                playlist.Name = playlist.Name + "_modified";
                ieditable.CancelEdit();
                playlist.Name.Should().Be.EqualTo(oldValue);

                playlist.Name = playlist.Name + "_modified2";

                count.Should().Be.EqualTo(2);
            }

            sessionFactory.Close();
        }

        #endregion
    }
}
