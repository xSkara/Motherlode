using NHibernate;

namespace Motherlode.Data.NHibernate
{
    public class SessionPerCallSessionProvider : NHibernateSessionProviderBase
    {
        #region Constructors and Destructors

        public SessionPerCallSessionProvider(ISessionFactory sessionFactory) : base(sessionFactory)
        {
        }

        #endregion

        #region Public Methods and Operators

        public override ISession GetSession()
        {
            return this.SessionFactory.OpenSession();
        }

        #endregion
    }
}
