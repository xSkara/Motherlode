using System;
using System.Collections;
using System.Collections.Generic;
using Motherlode.Data.NHibernate.Wpf.CollectionsTypeFactory.Impl.Collections;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.Type;
using NHibernate.UserTypes;

namespace Motherlode.Data.NHibernate.Wpf.CollectionsTypeFactory.Impl.Types
{
    public class ObservableSetType<T> : CollectionType, IUserCollectionType
    {
        #region Constructors and Destructors

        public ObservableSetType(string role, string foreignKeyPropertyName, bool isEmbeddedInXML)
            : base(role, foreignKeyPropertyName, isEmbeddedInXML)
        {
        }

        public ObservableSetType()
            : base(string.Empty, string.Empty, false)
        {
        }

        #endregion

        #region Public Properties

        public override Type ReturnedClass
        {
            get
            {
                return typeof(PersistentObservableGenericSet<T>);
            }
        }

        #endregion

        #region Public Methods and Operators

        public bool Contains(object collection, object entity)
        {
            return ((ICollection<T>)collection).Contains((T)entity);
        }

        public IEnumerable GetElements(object collection)
        {
            return ((IEnumerable)collection);
        }

        public IPersistentCollection Instantiate(ISessionImplementor session, ICollectionPersister persister)
        {
            return new PersistentObservableGenericSet<T>(session);
        }


        public override object Instantiate(int anticipatedSize)
        {
            return new ObservableSet<T>();
        }

        public override IPersistentCollection Instantiate(
            ISessionImplementor session,
            ICollectionPersister persister,
            object key)
        {
            return new PersistentObservableGenericSet<T>(session);
        }

        public object ReplaceElements(
            object original,
            object target,
            ICollectionPersister persister,
            object owner,
            IDictionary copyCache,
            ISessionImplementor session)
        {
            var result = (ICollection<T>)target;
            result.Clear();
            foreach (object item in ((IEnumerable)original))
            {
                if (copyCache.Contains(item))
                {
                    result.Add((T)copyCache[item]);
                }
                else
                {
                    result.Add((T)item);
                }
            }
            return result;
        }

        public override IPersistentCollection Wrap(ISessionImplementor session, object collection)
        {
            return new PersistentObservableGenericSet<T>(session, (Iesi.Collections.Generic.ISet<T>)collection);
        }

        #endregion

        #region Methods

        protected override void Clear(object collection)
        {
            ((IList)collection).Clear();
        }

        #endregion
    }
}
