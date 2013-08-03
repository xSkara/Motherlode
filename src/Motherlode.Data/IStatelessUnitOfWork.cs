using System;
using System.Data;

namespace Motherlode.Data
{
    public interface IStatelessUnitOfWork : IDisposable
    {
        #region Public Properties

        bool IsInActiveTransaction { get; }

        #endregion

        #region Public Methods and Operators

        ITransaction BeginTransaction();

        ITransaction BeginTransaction(IsolationLevel isolationLevel);

        /// <summary>Finishes this unit of work.</summary>
        void End();

        #endregion
    }
}
