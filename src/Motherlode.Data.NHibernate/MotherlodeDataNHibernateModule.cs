using NHibernate;
using Ninject.Modules;

namespace Motherlode.Data.NHibernate
{
    public class MotherlodeDataNHibernateModule : NinjectModule
    {
        #region Constants and Fields

        private readonly INHibernateSessionProvider _sessionProvider;

        #endregion

        #region Constructors and Destructors

        public MotherlodeDataNHibernateModule(INHibernateSessionProvider sessionProvider)
        {
            this._sessionProvider = sessionProvider;
        }

        #endregion

        #region Public Methods and Operators

        public override void Load()
        {
            this.Bind<ISession>().ToMethod(context => this._sessionProvider.GetSession());
            this.Bind<IStatelessSession>().ToMethod(context => this._sessionProvider.GetStatelessSession());
            this.Bind<NHibernateUnitOfWork>().ToSelf();
            this.Bind<NHibernateStatelessUnitOfWork>().ToSelf();
            this.Bind<IDaoFactory>().To<NHibernateDaoFactory>();
        }

        #endregion
    }
}
