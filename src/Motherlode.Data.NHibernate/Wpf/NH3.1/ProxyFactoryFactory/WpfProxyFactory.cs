using System;
using System.ComponentModel;
using System.Linq;
using NHibernate;
using NHibernate.ByteCode.Castle;
using NHibernate.Engine;
using NHibernate.Proxy;

namespace NHibernateConcepts.Wpf.ProxyFactoryFactory
{
    public class WpfProxyFactory : ProxyFactory
    {
        public override INHibernateProxy GetProxy(object id, ISessionImplementor session)
        {
            // If it is not a proxy for a class do what you usually did.
            if (!IsClassProxy)
            {
                return base.GetProxy(id, session);
            }

            try
            {
                var initializer = new WpfLazyInitializer(
                    EntityName,
                    PersistentClass,
                    id,
                    GetIdentifierMethod,
                    SetIdentifierMethod,
                    ComponentIdType,
                    session);

                // Add to the list of the interfaces that the proxy class will support the INotifyPropertyChanged interface.
                // This is only needed in the case when we need to cast our proxy object as INotifyPropertyChanged interface.
                var extraInterfaces = new[] { typeof(INotifyPropertyChanged) };

                var interfaces = Interfaces.Concat(extraInterfaces).ToArray();

                object generatedProxy = DefaultProxyGenerator.CreateClassProxy(PersistentClass, interfaces, initializer);

                initializer._constructed = true;
                initializer.ProxyInstance = generatedProxy;
                return (INHibernateProxy)generatedProxy;
            }
            catch (Exception e)
            {
                log.Error("Creating a proxy instance failed", e);
                throw new HibernateException("Creating a proxy instance failed", e);
            }
        }
    }
}