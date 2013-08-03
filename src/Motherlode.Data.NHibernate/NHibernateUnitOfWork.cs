using System;
using System.Data;
using Motherlode.Common;
using NHibernate;

namespace Motherlode.Data.NHibernate
{
    public class NHibernateUnitOfWork : IUnitOfWork
    {
        #region Constants and Fields

        private NHibernateTransaction _transaction;

        #endregion

        #region Constructors and Destructors

        public NHibernateUnitOfWork(ISession session)
        {
            this.Session = session;
        }

        #endregion

        #region Public Properties

        public bool IsDirty
        {
            get
            {
                return this.Session.IsDirty();
            }
        }

        public bool IsInActiveTransaction
        {
            get
            {
                return this._transaction != null && this._transaction.IsActive;
            }
        }

        public ISession Session { get; private set; }

        #endregion

        #region Public Methods and Operators

        public void Abort()
        {
            this.checkIfSessionIsOpen();

            this.Session.Close();
        }

        public ITransaction BeginTransaction()
        {
            this.checkIfSessionIsOpen();
            this.checkIfThereIsAnActiveTransaction();

            this._transaction = new NHibernateTransaction(this.Session.BeginTransaction());
            return this._transaction;
        }

        public ITransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            Guard.IsEnumMember(() => isolationLevel);

            this.checkIfSessionIsOpen();
            this.checkIfThereIsAnActiveTransaction();

            this._transaction = new NHibernateTransaction(this.Session.BeginTransaction(isolationLevel));
            return this._transaction;
        }

        public void Dispose()
        {
            if (this.Session.IsOpen)
            {
                this.Abort();
            }
        }

        public void End()
        {
            this.checkIfSessionIsOpen();

            this.Session.Flush();
            this.Session.Close();
        }

        public void Flush()
        {
            this.checkIfSessionIsOpen();

            this.Session.Flush();
        }

        #endregion

        #region Methods

        private void checkIfSessionIsOpen()
        {
            if (!this.Session.IsOpen)
            {
                throw new InvalidOperationException("The unit of work has already been completed.");
            }
        }

        private void checkIfThereIsAnActiveTransaction()
        {
            if (this.IsInActiveTransaction)
            {
                throw new InvalidOperationException("Parallel transactions are not supported.");
            }
        }

        #endregion
    }
}
