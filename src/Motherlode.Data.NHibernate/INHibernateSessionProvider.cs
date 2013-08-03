using System;
using NHibernate;

namespace Motherlode.Data.NHibernate
{
    public interface INHibernateSessionProvider : IDisposable
    {
        #region Public Methods and Operators

        ISession GetSession();

        IStatelessSession GetStatelessSession();

        #endregion
    }
}
