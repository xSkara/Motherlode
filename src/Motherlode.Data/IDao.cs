using System.Collections.Generic;
using System.Linq;

namespace Motherlode.Data
{
    public interface IDao<T> : IQueryable<T> where T : class
    {
        #region Public Properties

        IUnitOfWork UnitOfWork { get; }

        #endregion

        #region Public Methods and Operators

        T Get(object id);

        T Get(object id, LockMode lockMode);

        IList<T> GetAll();

        T Load(object id);

        T Load(object id, LockMode lockMode);

        void Lock(T entity, LockMode lockMode);

        void MakePersistent(T entity);

        void MakeTransient(T entity);

        #endregion
    }
}
