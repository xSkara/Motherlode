using System;
using NHibernate.Bytecode;
using NHibernate.Proxy;

namespace NHibernateConcepts.Wpf.ProxyFactoryFactory
{
    public class WpfProxyFactoryFactory : IProxyFactoryFactory
    {
        public IProxyValidator ProxyValidator
        {
            get { return new DynProxyTypeValidator(); }
        }

        public IProxyFactory BuildProxyFactory()
        {
            return new WpfProxyFactory();
        }

        public bool IsInstrumented(Type entityClass)
        {
            return true;
        }

        public bool IsProxy(object entity)
        {
            return entity is INHibernateProxy;
        }
    }
}