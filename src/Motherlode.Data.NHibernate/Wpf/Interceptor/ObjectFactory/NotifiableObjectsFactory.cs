using System;
using System.ComponentModel;
using Motherlode.Data.NHibernate.Wpf.Interceptor.DynamicProxy;
using NHibernate.Proxy.DynamicProxy;

namespace Motherlode.Data.NHibernate.Wpf.Interceptor.ObjectFactory
{
    public class NotifiableObjectsFactory : ObjectFactoryBase
    {
        #region Constructors and Destructors

        public NotifiableObjectsFactory() : base(new[] { typeof(INotifyPropertyChanged) })
        {
        }

        #endregion

        #region Methods

        protected override IInterceptor createInterceptor(Type type)
        {
            if (typeof(INotifyPropertyChanged).IsAssignableFrom(type))
            {
                throw new InvalidOperationException(
                    "The objects being created with notifiable factory should not implement INPC interface.");
            }

            return new NotifyPropertyChangedInterceptor(type);
        }

        #endregion
    }
}
