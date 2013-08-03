using System;
using System.ComponentModel;
using Motherlode.Data.NHibernate.Wpf.Interceptor.DynamicProxy;
using NHibernate.Proxy.DynamicProxy;

namespace Motherlode.Data.NHibernate.Wpf.Interceptor.ObjectFactory
{
    public class NotifiableEditableObjectsFactory : IObjectFactory
    {
        #region Constants and Fields

        private static readonly Type[] _interfacesForInnerProxy = new[] { typeof(IEditableObject) };

        private static readonly Type[] _interfacesForOuterProxy = new[]
            {
                typeof(IEditableObject),
                typeof(INotifyPropertyChanged),
                typeof(ObjectFactoryBase.ITypeInfo)
            };

        private static readonly ProxyFactory _proxyFactory = new ProxyFactory();

        #endregion

        #region Public Methods and Operators

        public T Create<T>()
        {
            return (T)this.Create(typeof(T));
        }

        public object Create(Type type)
        {
            if (typeof(INotifyPropertyChanged).IsAssignableFrom(type) ||
                typeof(IEditableObject).IsAssignableFrom(type))
            {
                throw new InvalidOperationException(
                    "The objects being created with notifiable factory should not implement INPC interface.");
            }

            object innerProxy = _proxyFactory.CreateProxy(type, new EditableObjectInterceptor(type), _interfacesForInnerProxy);
            return _proxyFactory.CreateProxy(type, new NotifyPropertyChangedInterceptor(innerProxy), _interfacesForOuterProxy);
        }

        #endregion
    }
}
