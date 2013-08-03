using System;
using System.ComponentModel;
using Castle.DynamicProxy;

namespace NHibernateConcepts.Wpf.Interceptor
{
    public class NotifiableObjectsFactory : ObjectFactoryBase
    {
        public NotifiableObjectsFactory() : base(new[] { typeof(INotifyPropertyChanged) })
        {
        }

        protected override IInterceptor[] createInterceptors(Type type)
        {
            if (typeof(INotifyPropertyChanged).IsAssignableFrom(type))
            {
                throw new InvalidOperationException(
                    "The objects being created with notifiable factory should not implement INPC interface.");
            }

            return new IInterceptor[] { new NotifyPropertyChangedInterceptor(type.FullName) };
        }
    }
}