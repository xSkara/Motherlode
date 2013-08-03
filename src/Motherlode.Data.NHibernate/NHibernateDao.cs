using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Motherlode.Common;
using NHibernate;
using NHibernate.Linq;

namespace Motherlode.Data.NHibernate
{
    public class NHibernateDao<T> : IDao<T> where T : class
    {
        #region Constants and Fields

        private readonly ISession _session;

        #endregion

        #region Constructors and Destructors

        public NHibernateDao(NHibernateUnitOfWork unitOfWork)
        {
            Guard.IsNotNull(() => unitOfWork);

            this.UnitOfWork = unitOfWork;
            this._session = unitOfWork.Session;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the type of the element(s) that are returned when the expression tree associated with
        ///     this instance of <see cref="T:System.Linq.IQueryable" /> is executed.
        /// </summary>
        /// <value>
        ///     A <see cref="T:System.Type" /> that represents the type of the element(s) that are returned
        ///     when the expression tree associated with this object is executed.
        /// </value>
        public Type ElementType
        {
            get
            {
                return this._session.Query<T>().ElementType;
            }
        }

        /// <summary>
        ///     Gets the expression tree that is associated with the instance of
        ///     <see cref="T:System.Linq.IQueryable" />.
        /// </summary>
        /// <value>
        ///     The <see cref="T:System.Linq.Expressions.Expression" /> that is associated with this instance
        ///     of <see cref="T:System.Linq.IQueryable" />.
        /// </value>
        public Expression Expression
        {
            get
            {
                return this._session.Query<T>().Expression;
            }
        }

        /// <summary>Gets the query provider that is associated with this data source.</summary>
        /// <value>
        ///     The <see cref="T:System.Linq.IQueryProvider" /> that is associated with this data source.
        /// </value>
        public IQueryProvider Provider
        {
            get
            {
                return this._session.Query<T>().Provider;
            }
        }

        public IUnitOfWork UnitOfWork { get; private set; }

        #endregion

        #region Public Methods and Operators

        public T Get(object id)
        {
            Guard.IsNotNull(() => id);

            return this._session.Get<T>(id);
        }

        public T Get(object id, LockMode lockMode)
        {
            Guard.IsNotNull(() => id);
            Guard.IsEnumMember(() => lockMode);

            return this._session.Get<T>(id, LockModeMapper.Map(lockMode));
        }

        public IList<T> GetAll()
        {
            ICriteria criteria = this._session.CreateCriteria(typeof(T));
            return criteria.List<T>();
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>
        ///     A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate
        ///     through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this._session.Query<T>().GetEnumerator();
        }

        public T Load(object id)
        {
            Guard.IsNotNull(() => id);

            return this._session.Load<T>(id);
        }

        public T Load(object id, LockMode lockMode)
        {
            Guard.IsNotNull(() => id);
            Guard.IsEnumMember(() => lockMode);

            return this._session.Load<T>(id, LockModeMapper.Map(lockMode));
        }

        public void Lock(T entity, LockMode lockMode)
        {
            Guard.IsNotNull(() => entity);
            Guard.IsEnumMember(() => lockMode);

            this._session.Lock(entity, LockModeMapper.Map(lockMode));
        }

        public void MakePersistent(T entity)
        {
            Guard.IsNotNull(() => entity);

            this._session.SaveOrUpdate(entity);
        }

        public void MakeTransient(T entity)
        {
            Guard.IsNotNull(() => entity);

            this._session.Delete(entity);
        }

        #endregion

        #region Explicit Interface Methods

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through
        ///     the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._session.Query<T>().GetEnumerator();
        }

        #endregion
    }
}
