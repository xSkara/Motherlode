using System;

namespace Motherlode.Data.NHibernate
{
    internal static class LockModeMapper
    {
        #region Public Methods and Operators

        public static global::NHibernate.LockMode Map(LockMode lockMode)
        {
            switch (lockMode)
            {
            case LockMode.None:
                return global::NHibernate.LockMode.None;
            case LockMode.Read:
                return global::NHibernate.LockMode.Read;
            case LockMode.Upgrade:
                return global::NHibernate.LockMode.Upgrade;
            case LockMode.UpgradeNoWait:
                return global::NHibernate.LockMode.UpgradeNoWait;
            case LockMode.Write:
                return global::NHibernate.LockMode.Write;
            case LockMode.Force:
                return global::NHibernate.LockMode.Force;
            default:
                throw new ArgumentOutOfRangeException("lockMode");
            }
        }

        #endregion
    }
}
