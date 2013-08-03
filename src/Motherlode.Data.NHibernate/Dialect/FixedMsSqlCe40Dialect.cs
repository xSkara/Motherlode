using NHibernate.Dialect;

namespace Motherlode.Data.NHibernate.Dialect
{
    public class FixedMsSqlCe40Dialect : MsSqlCe40Dialect
    {
        #region Public Properties

        public override bool SupportsVariableLimit
        {
            get
            {
                return true;
            }
        }

        #endregion
    }
}
