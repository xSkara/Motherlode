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
    public class NHibernateEditableObjectInterceptorTestFixture
    {
        #region Public Methods and Operators

        [Test]
        public void canceling_editing_should_lead_to_reverting_the_changes()
        {
            ISessionFactory sessionFactory =
                Helper.CreateHiLoWpfSessionFactory(() => new ObjectFactoryInterceptor(new EditableObjectsFactory()));

            using (ISession session = sessionFactory.OpenSession())
            {
                var playlist = session.Get<Playlist>(1);

                var editablePlaylist = (IEditableObject)playlist;

                string oldValue = playlist.Name;
                editablePlaylist.BeginEdit();
                playlist.Name = oldValue + "_modified";
                editablePlaylist.CancelEdit();
                playlist.Name.Should().Be.EqualTo(oldValue);
            }

            sessionFactory.Close();
        }

        [Test]
        public void ending_editing_should_commit_the_changes()
        {
            ISessionFactory sessionFactory =
                Helper.CreateHiLoWpfSessionFactory(() => new ObjectFactoryInterceptor(new EditableObjectsFactory()));

            using (ISession session = sessionFactory.OpenSession())
            {
                var playlist = session.Get<Playlist>(1);

                var editablePlaylist = (IEditableObject)playlist;

                string newValue = playlist.Name + "_modified";
                editablePlaylist.BeginEdit();
                playlist.Name = newValue;
                editablePlaylist.EndEdit();
                playlist.Name.Should().Be.EqualTo(newValue);
            }

            sessionFactory.Close();
        }

        [Test]
        public void interceptor_implements_IEditableObject_interface()
        {
            ISessionFactory sessionFactory =
                Helper.CreateHiLoWpfSessionFactory(() => new ObjectFactoryInterceptor(new EditableObjectsFactory()));

            using (ISession session = sessionFactory.OpenSession())
            {
                var playlist = session.Get<Playlist>(1);

                (playlist is IEditableObject).Should().Be.True();
            }

            sessionFactory.Close();
        }

        [Test]
        public void object_should_return_modified_values_while_editing_is_active()
        {
            ISessionFactory sessionFactory =
                Helper.CreateHiLoWpfSessionFactory(() => new ObjectFactoryInterceptor(new EditableObjectsFactory()));

            using (ISession session = sessionFactory.OpenSession())
            {
                var playlist = session.Get<Playlist>(1);

                var editablePlaylist = (IEditableObject)playlist;

                string newValue = playlist.Name + "_modified";
                editablePlaylist.BeginEdit();
                playlist.Name = newValue;
                playlist.Name.Should().Be.EqualTo(newValue);
                editablePlaylist.CancelEdit();
            }

            sessionFactory.Close();
        }

        [Test]
        public void secondary_call_of_the_BeginEdit_method_should_not_affect_modified_values()
        {
            ISessionFactory sessionFactory =
                Helper.CreateHiLoWpfSessionFactory(() => new ObjectFactoryInterceptor(new EditableObjectsFactory()));

            using (ISession session = sessionFactory.OpenSession())
            {
                var playlist = session.Get<Playlist>(1);

                var editablePlaylist = (IEditableObject)playlist;

                string newValue = playlist.Name + "_modified";
                editablePlaylist.BeginEdit();
                playlist.Name = newValue;
                editablePlaylist.BeginEdit();
                playlist.Name.Should().Be.EqualTo(newValue);
                editablePlaylist.CancelEdit();
            }

            sessionFactory.Close();
        }

        [Test]
        public void sequential_transaction_should_not_affect_each_other()
        {
            ISessionFactory sessionFactory =
                Helper.CreateHiLoWpfSessionFactory(() => new ObjectFactoryInterceptor(new EditableObjectsFactory()));

            using (ISession session = sessionFactory.OpenSession())
            {
                var playlist = session.Get<Playlist>(1);

                var editablePlaylist = (IEditableObject)playlist;

                string newValue = playlist.Name + "_modified";
                string newValue2 = playlist.Name + "_modified2";
                editablePlaylist.BeginEdit();
                playlist.Name = newValue;
                editablePlaylist.EndEdit();

                editablePlaylist.BeginEdit();
                playlist.Name = newValue2;
                editablePlaylist.CancelEdit();

                playlist.Name.Should().Be.EqualTo(newValue);
            }

            sessionFactory.Close();
        }

        #endregion
    }
}
