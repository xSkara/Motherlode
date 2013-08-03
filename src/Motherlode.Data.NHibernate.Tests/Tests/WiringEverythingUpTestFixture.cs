using Motherlode.Common;
using Motherlode.Data.NHibernate.Tests.Cfg.TestFixtureBases;
using Motherlode.Data.NHibernate.Tests.Domain.ViewModel;
using Ninject;
using NUnit.Framework;
using SharpTestsEx;

namespace Motherlode.Data.NHibernate.Tests.Tests
{
    [TestFixture]
    public class WiringEverythingUpTestFixture : HiLoFluentConfigurationTestFixtureBase
    {
        #region Public Methods and Operators

        [Test]
        public void context_session_provider_should_pass_same_session_for_all_factories()
        {
            var kernel = new StandardKernel(
                new MotherlodeDataNHibernateModule(new NHibernateContextSessionProvider(this.SessionFactory)),
                new MotherlodeCommonModule());

            var vm1 = kernel.Get<FakeViewModel>();
            var vm2 = kernel.Get<FakeViewModel>();

            vm1.DaoFactory.Should().Be.InstanceOf<NHibernateDaoFactory>();
            vm2.DaoFactory.Should().Be.InstanceOf<NHibernateDaoFactory>();
            var uow1 = (NHibernateUnitOfWork)vm1.DaoFactory.UnitOfWork;
            var uow2 = (NHibernateUnitOfWork)vm2.DaoFactory.UnitOfWork;
            uow1.Session.Should().Be.EqualTo(uow2.Session);
        }

        [Test]
        public void session_per_call_provider_should_create_a_new_session_each_call()
        {
            var kernel = new StandardKernel(
                new MotherlodeDataNHibernateModule(new SessionPerCallSessionProvider(this.SessionFactory)),
                new MotherlodeCommonModule());

            var vm1 = kernel.Get<FakeViewModel>();
            var vm2 = kernel.Get<FakeViewModel>();

            vm1.DaoFactory.Should().Be.InstanceOf<NHibernateDaoFactory>();
            vm2.DaoFactory.Should().Be.InstanceOf<NHibernateDaoFactory>();
            var uow1 = (NHibernateUnitOfWork)vm1.DaoFactory.UnitOfWork;
            var uow2 = (NHibernateUnitOfWork)vm2.DaoFactory.UnitOfWork;
            uow1.Session.Should().Not.Be.EqualTo(uow2.Session);
        }

        #endregion
    }
}
