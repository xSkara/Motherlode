using System.Collections.Specialized;
using System.ComponentModel;
using Iesi.Collections.Generic;
using NHibernate.Collection.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Collection;

namespace Motherlode.Data.NHibernate.Wpf.CollectionsTypeFactory.Impl.Collections
{
    public class PersistentObservableGenericSet<T>
        : PersistentGenericSet<T>,
          INotifyCollectionChanged,
          INotifyPropertyChanged
    {
        #region Constants and Fields

        private NotifyCollectionChangedEventHandler _collectionChanged;
        private PropertyChangedEventHandler _propertyChanged;

        #endregion

        #region Constructors and Destructors

        public PersistentObservableGenericSet(ISessionImplementor sessionImplementor)
            : base(sessionImplementor)
        {
        }

        public PersistentObservableGenericSet(ISessionImplementor sessionImplementor, ISet<T> coll)
            : base(sessionImplementor, coll)
        {
            this.CaptureEventHandlers(coll);
        }

        public PersistentObservableGenericSet()
        {
        }

        #endregion

        #region Public Events

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                this.Initialize(false);
                this._collectionChanged += value;
            }
            remove
            {
                this._collectionChanged -= value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                this.Initialize(false);
                this._propertyChanged += value;
            }
            remove
            {
                this._propertyChanged += value;
            }
        }

        #endregion

        #region Public Methods and Operators

        public override void BeforeInitialize(ICollectionPersister persister, int anticipatedSize)
        {
            base.BeforeInitialize(persister, anticipatedSize);
            this.CaptureEventHandlers(this.set);
        }

        #endregion

        #region Methods

        private void CaptureEventHandlers(object coll)
        {
            var notificableCollection = coll as INotifyCollectionChanged;
            var propertyNotificableColl = coll as INotifyPropertyChanged;

            if (notificableCollection != null)
            {
                notificableCollection.CollectionChanged += this.OnCollectionChanged;
            }

            if (propertyNotificableColl != null)
            {
                propertyNotificableColl.PropertyChanged += this.OnPropertyChanged;
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler changed = this._collectionChanged;
            if (changed != null)
            {
                changed(this, e);
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler changed = this._propertyChanged;
            if (changed != null)
            {
                changed(this, e);
            }
        }

        #endregion
    }
}
