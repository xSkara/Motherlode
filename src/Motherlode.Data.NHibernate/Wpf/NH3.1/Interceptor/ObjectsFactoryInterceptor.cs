using System;
using System.ComponentModel;
using NHibernate;
using NHibernate.Proxy.DynamicProxy;

namespace NHibernateConcepts.Wpf.Interceptor
{
    public class ObjectsFactoryInterceptor : EmptyInterceptor
    {
        private readonly IObjectsFactory _dataBindingFactory;

        public ObjectsFactoryInterceptor(IObjectsFactory objectsFactory)
        {
            _dataBindingFactory = objectsFactory;
        }

        public ISessionFactory SessionFactory { get; set; }

        public override object Instantiate(string clazz, EntityMode entityMode, object id)
        {
            if (entityMode == EntityMode.Poco)
            {
                var type = Type.GetType(clazz);
                if (type != null)
                {
                    var instance = _dataBindingFactory.Create(type);
                    SessionFactory.GetClassMetadata(clazz).SetIdentifier(instance, id, entityMode);
                    return instance;
                }
            }

            return base.Instantiate(clazz, entityMode, id);
        }

        public override string GetEntityName(object entity)
        {
            var markerInterface = entity as ObjectFactoryBase.ITypeInfo;
            if (markerInterface != null)
            {
                return markerInterface.TypeName;
            }

            return base.GetEntityName(entity);
        }
    }
}
