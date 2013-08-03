using System;
using Motherlode.Data.NHibernate.Tests.Cfg.Providers;
using Motherlode.Data.NHibernate.Wpf.Interceptor;
using NHibernate;

namespace Motherlode.Data.NHibernate.Tests.Utils
{
    public static class Helper
    {
        #region Public Methods and Operators

        public static ISessionFactory CreateHiLoWpfSessionFactory(
            Func<ObjectFactoryInterceptor> interceptorGenerator = null,
            bool lazyLoading = true,
            bool notifiableProxyFactory = false,
            bool editableNotifiableProxyFactory = false)
        {
            var configurationProvider = new HiLoWpfConfigurationProvider(
                lazyLoading: lazyLoading,
                notifiableProxyFactory: notifiableProxyFactory,
                editableNotifiableProxyFactory: editableNotifiableProxyFactory);

            if (interceptorGenerator != null)
            {
                configurationProvider.ObjectsFactoryInterceptorGenerator = interceptorGenerator;
            }

            return configurationProvider.Create().BuildSessionFactory();
        }

        #endregion
    }
}
