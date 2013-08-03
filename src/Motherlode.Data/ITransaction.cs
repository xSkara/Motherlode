using System;

namespace Motherlode.Data
{
    public interface ITransaction : IDisposable
    {
        #region Public Properties

        bool IsActive { get; }

        #endregion

        #region Public Methods and Operators

        void Commit();

        void Rollback();

        #endregion
    }
}
