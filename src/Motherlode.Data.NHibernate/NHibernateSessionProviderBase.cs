using NHibernate;

namespace Motherlode.Data.NHibernate
{
    public abstract class NHibernateSessionProviderBase : INHibernateSessionProvider
    {
        #region Constructors and Destructors

        public NHibernateSessionProviderBase(ISessionFactory sessionFactory)
        {
            this.SessionFactory = sessionFactory;
        }

        #endregion

        #region Public Properties

        public ISessionFactory SessionFactory { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public virtual void Dispose()
        {
            this.SessionFactory.Dispose();
        }

        public abstract ISession GetSession();

        public virtual IStatelessSession GetStatelessSession()
        {
            return this.SessionFactory.OpenStatelessSession();
        }

        #endregion
    }
}
