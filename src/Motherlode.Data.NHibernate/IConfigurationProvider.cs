using NHibernate.Cfg;

namespace Motherlode.Data.NHibernate
{
    public interface IConfigurationProvider
    {
        #region Public Methods and Operators

        Configuration Create();

        #endregion
    }
}
