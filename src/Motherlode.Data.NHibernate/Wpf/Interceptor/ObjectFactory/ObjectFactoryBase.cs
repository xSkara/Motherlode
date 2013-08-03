using System;
using System.Linq;
using Motherlode.Common;
using NHibernate.Proxy.DynamicProxy;

namespace Motherlode.Data.NHibernate.Wpf.Interceptor.ObjectFactory
{
    public abstract class ObjectFactoryBase : IObjectFactory
    {
        #region Constants and Fields

        private static readonly ProxyFactory _proxyFactory = new ProxyFactory();
        private readonly Type[] _interfacesToProxy;

        #endregion

        #region Constructors and Destructors

        protected ObjectFactoryBase(Type[] interfacesToProxy)
        {
            Guard.IsNotNull(() => interfacesToProxy);

            if (!interfacesToProxy.Contains(typeof(ITypeInfo)))
            {
                interfacesToProxy = new[] { typeof(ITypeInfo) }.Concat(interfacesToProxy).ToArray();
            }

            this._interfacesToProxy = interfacesToProxy;
        }

        #endregion

        #region Interfaces

        public interface ITypeInfo
        {
            #region Public Properties

            string TypeName { get; }

            #endregion
        }

        #endregion

        #region Public Methods and Operators

        public T Create<T>()
        {
            return (T)this.Create(typeof(T));
        }

        public object Create(Type type)
        {
            return _proxyFactory.CreateProxy(type, this.createInterceptor(type), this._interfacesToProxy);
        }

        #endregion

        #region Methods

        protected abstract IInterceptor createInterceptor(Type type);

        #endregion
    }
}
