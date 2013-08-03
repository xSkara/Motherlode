using Motherlode.Common;

namespace Motherlode.Data.NHibernate
{
    public class NHibernateTransaction : ITransaction
    {
        #region Constructors and Destructors

        public NHibernateTransaction(global::NHibernate.ITransaction transaction)
        {
            Guard.IsNotNull(() => transaction);

            this.Transaction = transaction;
        }

        #endregion

        #region Public Properties

        public bool IsActive
        {
            get
            {
                return this.Transaction.IsActive;
            }
        }

        public global::NHibernate.ITransaction Transaction { get; private set; }

        #endregion

        #region Public Methods and Operators

        public void Commit()
        {
            this.Transaction.Commit();
        }

        public void Dispose()
        {
            this.Transaction.Dispose();
        }

        public void Rollback()
        {
            this.Transaction.Rollback();
        }

        #endregion
    }
}
