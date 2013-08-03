using System;
using Castle.DynamicProxy;
using System.Linq;
using Motherlode.Common;

namespace NHibernateConcepts.Wpf.Interceptor
{
    public abstract class ObjectFactoryBase : IObjectsFactory
    {
        private static readonly ProxyGenerator _proxyGenerator = new ProxyGenerator();
        private readonly Type[] _interfacesToProxy;

        protected ObjectFactoryBase(Type[] interfacesToProxy)
        {
            Guard.IsNotNull(() => interfacesToProxy);

            if (!interfacesToProxy.Contains(typeof(ITypeInfo)))
            {
                interfacesToProxy = new[] { typeof(ITypeInfo) }.Concat(interfacesToProxy).ToArray();
            }

            _interfacesToProxy = interfacesToProxy;
        }

        public T Create<T>()
        {
            return (T)Create(typeof(T));
        }

        public object Create(Type type)
        {
            return _proxyGenerator.CreateClassProxy(type, _interfacesToProxy, createInterceptors(type));
        }

        protected abstract IInterceptor[] createInterceptors(Type type);

        public interface ITypeInfo
        {
            string TypeName { get; }
        }
    }
}