using System;
using System.ComponentModel;
using System.Linq;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Proxy;
using NHibernate.Proxy.DynamicProxy;

namespace Motherlode.Data.NHibernate.Wpf.ProxyFactoryFactory
{
    public class EditableNotifiableProxyFactory : DefaultProxyFactory
    {
        #region Constants and Fields

        private readonly ProxyFactory _factory = new ProxyFactory();

        #endregion

        #region Public Methods and Operators

        public override INHibernateProxy GetProxy(object id, ISessionImplementor session)
        {
            if (!this.IsClassProxy)
            {
                return base.GetProxy(id, session);
            }

            try
            {
                var initializer = new WpfLazyInitializer(
                    this.EntityName,
                    this.PersistentClass,
                    id,
                    this.GetIdentifierMethod,
                    this.SetIdentifierMethod,
                    this.ComponentIdType,
                    session);

                Type[] interfaces =
                    this.Interfaces.Concat(new[] { typeof(INotifyPropertyChanged), typeof(IEditableObject) }).ToArray();

                object proxyInstance = this._factory.CreateProxy(this.PersistentClass, initializer, interfaces);
                initializer.ProxyInstance = proxyInstance;

                return (INHibernateProxy)proxyInstance;
            }
            catch (Exception ex)
            {
                log.Error("Creating a proxy instance failed", ex);
                throw new HibernateException("Creating a proxy instance failed", ex);
            }
        }

        #endregion
    }
}
