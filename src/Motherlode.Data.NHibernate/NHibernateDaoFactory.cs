using System;
using Motherlode.Common;

namespace Motherlode.Data.NHibernate
{
    public class NHibernateDaoFactory : IDaoFactory
    {
        #region Constants and Fields

        private readonly Lazy<NHibernateStatelessUnitOfWork> _statelessUnitOfWork;
        private readonly Lazy<NHibernateUnitOfWork> _unitOfWork;

        #endregion

        #region Constructors and Destructors

        public NHibernateDaoFactory(
            Lazy<NHibernateUnitOfWork> unitOfWork,
            Lazy<NHibernateStatelessUnitOfWork> statelessUnitOfWork)
        {
            Guard.IsNotNull(() => unitOfWork);
            Guard.IsNotNull(() => statelessUnitOfWork);

            this._unitOfWork = unitOfWork;
            this._statelessUnitOfWork = statelessUnitOfWork;
        }

        #endregion

        #region Public Properties

        public IStatelessUnitOfWork StatelessUnitOfWork
        {
            get
            {
                return this._statelessUnitOfWork.Value;
            }
        }

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return this._unitOfWork.Value;
            }
        }

        #endregion

        #region Public Methods and Operators

        public IDao<T> CreateDao<T>() where T : class
        {
            return new NHibernateDao<T>(this._unitOfWork.Value);
        }

        public IStatelessDao<T> CreateStatelessDao<T>() where T : class
        {
            return new NHibernateStatelessDao<T>(this._statelessUnitOfWork.Value);
        }

        #endregion
    }
}
