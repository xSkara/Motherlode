using System.Collections.Generic;
using System.Linq;

namespace Motherlode.Data
{
    public interface IStatelessDao<T> : IQueryable<T> where T : class
    {
        #region Public Properties

        IStatelessUnitOfWork UnitOfWork { get; }

        #endregion

        #region Public Methods and Operators

        void Delete(T entity);

        T Get(object id);

        T Get(object id, LockMode lockMode);

        IList<T> GetAll();

        void Insert(T entity);

        void Update(T entity);

        #endregion
    }
}
