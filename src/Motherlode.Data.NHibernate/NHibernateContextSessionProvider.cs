using NHibernate;
using NHibernate.Context;

namespace Motherlode.Data.NHibernate
{
    public class NHibernateContextSessionProvider : NHibernateSessionProviderBase
    {
        #region Constructors and Destructors

        public NHibernateContextSessionProvider(ISessionFactory sessionFactory) : base(sessionFactory)
        {
        }

        #endregion

        #region Public Methods and Operators

        public override void Dispose()
        {
            if (CurrentSessionContext.HasBind(this.SessionFactory))
            {
                ISession session = CurrentSessionContext.Unbind(this.SessionFactory);
                session.Dispose();
            }

            base.Dispose();
        }

        public override ISession GetSession()
        {
            if (!CurrentSessionContext.HasBind(this.SessionFactory))
            {
                CurrentSessionContext.Bind(this.SessionFactory.OpenSession());
            }

            return this.SessionFactory.GetCurrentSession();
        }

        #endregion
    }
}
