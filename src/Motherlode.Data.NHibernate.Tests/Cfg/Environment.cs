using System.Configuration;

namespace Motherlode.Data.NHibernate.Tests.Cfg
{
    internal class Environment
    {
        #region Public Properties

        public static string TargetDirectory
        {
            get
            {
                return ConfigurationManager.AppSettings["targetDir"];
            }
        }

        #endregion
    }
}
