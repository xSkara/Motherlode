using System;
using System.Linq;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.Proxy;

namespace Motherlode.Data.NHibernate.Extensions
{
    public static class UnitOfWorkExtensions
    {
        #region Public Methods and Operators

        public static object GetOriginalEntityProperty(this IUnitOfWork unitOfWork, object entity, string propertyName)
        {
            ISession session = getSession(unitOfWork);
            ISessionImplementor sessionImpl = session.GetSessionImplementation();
            IPersistenceContext persistenceContext = sessionImpl.PersistenceContext;
            EntityEntry oldEntry = persistenceContext.GetEntry(entity);

            if (oldEntry == null && entity is INHibernateProxy)
            {
                var proxy = entity as INHibernateProxy;
                object obj = sessionImpl.PersistenceContext.Unproxy(proxy);
                oldEntry = sessionImpl.PersistenceContext.GetEntry(obj);
            }

            if (oldEntry == null)
            {
                return null;
            }

            IEntityPersister persister = oldEntry.Persister;

            object[] oldState = oldEntry.LoadedState;
            object[] currentState = persister.GetPropertyValues(entity, sessionImpl.EntityMode);
            int[] dirtyProps = persister.FindDirty(currentState, oldState, entity, sessionImpl);
            int index = Array.IndexOf(persister.PropertyNames, propertyName);

            bool isDirty = dirtyProps != null && Array.IndexOf(dirtyProps, index) != -1;
            return isDirty ? oldState[index] : currentState[index];
        }

        public static bool IsDirtyEntity(this IUnitOfWork unitOfWork, object entity)
        {
            ISession session = getSession(unitOfWork);
            ISessionImplementor sessionImpl = session.GetSessionImplementation();
            IPersistenceContext persistenceContext = sessionImpl.PersistenceContext;
            EntityEntry oldEntry = persistenceContext.GetEntry(entity);

            if (oldEntry == null && entity is INHibernateProxy)
            {
                object obj = sessionImpl.PersistenceContext.Unproxy(entity);
                oldEntry = sessionImpl.PersistenceContext.GetEntry(obj);
            }

            if (oldEntry == null)
            {
                return true;
            }

            IEntityPersister persister = oldEntry.Persister;

            object[] oldState = oldEntry.LoadedState;
            object[] currentState = persister.GetPropertyValues(entity, sessionImpl.EntityMode);

            bool dirtyPropertiesExists = oldState.Zip(
                currentState,
                (o1, o2) => o1 == null
                                ? o2 != null
                                : !o1.Equals(o2)).Any(i => i);
            return dirtyPropertiesExists;
        }

        public static bool IsDirtyProperty(this IUnitOfWork unitOfWork, object entity, string propertyName)
        {
            ISession session = getSession(unitOfWork);
            ISessionImplementor sessionImpl = session.GetSessionImplementation();
            IPersistenceContext persistenceContext = sessionImpl.PersistenceContext;
            EntityEntry oldEntry = persistenceContext.GetEntry(entity);

            if (oldEntry == null)
            {
                return true;
            }

            IEntityPersister persister = oldEntry.Persister;

            object[] oldState = oldEntry.LoadedState;
            object[] currentState = persister.GetPropertyValues(entity, sessionImpl.EntityMode);

            int[] dirtyProps = persister.FindDirty(currentState, oldState, entity, sessionImpl);
            int index = Array.IndexOf(persister.PropertyNames, propertyName);
            bool isDirty = dirtyProps != null && Array.IndexOf(dirtyProps, index) != -1;

            return isDirty;
        }

        #endregion

        #region Methods

        private static ISession getSession(IUnitOfWork unitOfWork)
        {
            if (!(unitOfWork is NHibernateUnitOfWork))
            {
                throw new InvalidOperationException("The unit of work is not an instance of the NHibernateUnitOfWork class.");
            }

            ISession session = (unitOfWork as NHibernateUnitOfWork).Session;
            return session;
        }

        #endregion
    }
}
