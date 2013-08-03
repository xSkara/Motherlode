using System;
using NHibernate.Bytecode;
using NHibernate.Proxy;

namespace Motherlode.Data.NHibernate.Wpf.ProxyFactoryFactory
{
    public class NotifableProxyFactoryFactory : IProxyFactoryFactory
    {
        #region Public Properties

        public IProxyValidator ProxyValidator
        {
            get
            {
                return new DynProxyTypeValidator();
            }
        }

        #endregion

        #region Public Methods and Operators

        public IProxyFactory BuildProxyFactory()
        {
            return new NotifiableProxyFactory();
        }

        public bool IsInstrumented(Type entityClass)
        {
            return true;
        }

        public bool IsProxy(object entity)
        {
            return entity is INHibernateProxy;
        }

        #endregion
    }
}
