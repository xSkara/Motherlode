using System;
using Motherlode.Data.NHibernate.Wpf.Interceptor.ObjectFactory;
using NHibernate;
using NHibernate.Metadata;

namespace Motherlode.Data.NHibernate.Wpf.Interceptor
{
    public class ObjectFactoryInterceptor : EmptyInterceptor
    {
        #region Constants and Fields

        private readonly IObjectFactory _dataBindingFactory;

        private ISessionFactory _sessionFactory;

        #endregion

        #region Constructors and Destructors

        public ObjectFactoryInterceptor(IObjectFactory objectFactory)
        {
            this._dataBindingFactory = objectFactory;
        }

        #endregion

        #region Public Methods and Operators

        public override string GetEntityName(object entity)
        {
            var markerInterface = entity as ObjectFactoryBase.ITypeInfo;
            if (markerInterface != null)
            {
                return markerInterface.TypeName;
            }

            return base.GetEntityName(entity);
        }

        public override object Instantiate(string clazz, EntityMode entityMode, object id)
        {
            if (entityMode == EntityMode.Poco)
            {
                IClassMetadata classMetadata = this._sessionFactory.GetClassMetadata(clazz);
                Type type = classMetadata.GetMappedClass(entityMode);
                object instance = this._dataBindingFactory.Create(type);
                classMetadata.SetIdentifier(instance, id, entityMode);
                return instance;
            }

            return base.Instantiate(clazz, entityMode, id);
        }

        public override void SetSession(ISession session)
        {
            base.SetSession(session);
            this._sessionFactory = session.SessionFactory;
        }

        #endregion
    }
}
