using Motherlode.Data.NHibernate.Wpf.CollectionsTypeFactory.Impl.Types;
using NHibernate.Type;

namespace Motherlode.Data.NHibernate.Wpf.CollectionsTypeFactory
{
    public class WpfCollectionTypeFactory : DefaultCollectionTypeFactory
    {
        #region Public Methods and Operators

        public override CollectionType Bag<T>(string role, string propertyRef, bool embedded)
        {
            return new ObservableBagType<T>(role, propertyRef, embedded);
        }

        public override CollectionType List<T>(string role, string propertyRef, bool embedded)
        {
            return new ObservableListType<T>(role, propertyRef, embedded);
        }

        public override CollectionType Set<T>(string role, string propertyRef, bool embedded)
        {
            return new ObservableSetType<T>(role, propertyRef, embedded);
        }

        #endregion
    }
}
