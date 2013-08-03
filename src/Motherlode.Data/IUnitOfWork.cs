using System;
using System.Data;

namespace Motherlode.Data
{
    public interface IUnitOfWork : IDisposable
    {
        #region Public Properties

        /// <summary>Gets a value indicating whether this instance is dirty i.e. contains any changes to be flushed.</summary>
        /// <value>
        ///     <see langword="true" /> if this instance is dirty, <see langword="false" /> if not.
        /// </value>
        bool IsDirty { get; }

        bool IsInActiveTransaction { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Aborts this unit of work rolling back all the changes made.</summary>
        void Abort();

        ITransaction BeginTransaction();

        ITransaction BeginTransaction(IsolationLevel isolationLevel);

        /// <summary>Flushes the changes and finishes this unit of work.</summary>
        void End();

        /// <summary>Flushes the changes have been made since the last flush keeping the unit of work alive.</summary>
        void Flush();

        #endregion
    }
}
