using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Iesi.Collections.Generic;

namespace Motherlode.Data.NHibernate.Wpf.CollectionsTypeFactory.Impl.Collections
{
    /// <summary>
    ///     ISet that implements INotifyCollectionChanged
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableSet<T> : HashedSet<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        #region Constants and Fields

        private const string COUNT_PROPERTY_NAME = "Count";
        private const string ISEMPTY_PROPERTY_NAME = "IsEmpty";

        private bool _isInXAllOperation;

        #endregion

        #region Public Events

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds the specified element to this set if it is not already present.
        /// </summary>
        /// <param name="o">
        ///     The <typeparamref name="T" /> to add to the set.
        /// </param>
        /// <returns>
        ///     <see langword="true" /> is the object was added, <see langword="false" /> if it was already present.
        /// </returns>
        public override bool Add(T o)
        {
            bool add = base.Add(o);
            if (add && !this._isInXAllOperation)
            {
                this.OnPropertyChanged(ISEMPTY_PROPERTY_NAME);
                this.OnPropertyChanged(COUNT_PROPERTY_NAME);
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, o));
            }
            return add;
        }

        /// <summary>
        ///     Adds all the elements in the specified collection to the set if they are not already present.
        /// </summary>
        /// <param name="c">A collection of objects to add to the set.</param>
        /// <returns>
        ///     <see langword="true" /> is the set changed as a result of this operation, <see langword="false" /> if not.
        /// </returns>
        public override bool AddAll(ICollection<T> c)
        {
            bool flag = false;
            this._isInXAllOperation = true;
            var itemsAdded = new List<T>();
            foreach (T item in c)
            {
                bool operationResult = this.Add(item);
                flag |= operationResult;
                if (operationResult)
                {
                    itemsAdded.Add(item);
                }
            }
            if (flag)
            {
                this.OnPropertyChanged(ISEMPTY_PROPERTY_NAME);
                this.OnPropertyChanged(COUNT_PROPERTY_NAME);
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, itemsAdded));
            }
            this._isInXAllOperation = false;
            return flag;
        }

        /// <summary>
        ///     Removes all objects from the set.
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            this.OnPropertyChanged(ISEMPTY_PROPERTY_NAME);
            this.OnPropertyChanged(COUNT_PROPERTY_NAME);
            this.OnCollectionReset();
        }

        /// <summary>
        ///     Removes the specified element from the set.
        /// </summary>
        /// <param name="o">The element to be removed.</param>
        /// <returns>
        ///     <see langword="true" /> if the set contained the specified element, <see langword="false" /> otherwise.
        /// </returns>
        public override bool Remove(T o)
        {
            int itemIndex = 0;
            foreach (T obj in this)
            {
                if (obj.Equals(o))
                {
                    break;
                }
                itemIndex++;
            }

            bool result = base.Remove(o);
            if (result && !this._isInXAllOperation)
            {
                this.OnPropertyChanged(ISEMPTY_PROPERTY_NAME);
                this.OnPropertyChanged(COUNT_PROPERTY_NAME);
                this.OnCollectionChanged(
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, o, itemIndex));
            }

            return result;
        }

        /// <summary>
        ///     Remove all the specified elements from this set, if they exist in this set.
        /// </summary>
        /// <param name="c">A collection of elements to remove.</param>
        /// <returns>
        ///     <see langword="true" /> if the set was modified as a result of this operation.
        /// </returns>
        public override bool RemoveAll(ICollection<T> c)
        {
            bool flag = false;
            this._isInXAllOperation = true;
            var itemsRemoved = new List<T>();
            foreach (T item in c)
            {
                bool operationResult = this.Remove(item);
                flag |= operationResult;
                if (operationResult)
                {
                    itemsRemoved.Add(item);
                }
            }
            if (flag)
            {
                this.OnPropertyChanged(ISEMPTY_PROPERTY_NAME);
                this.OnPropertyChanged(COUNT_PROPERTY_NAME);
                this.OnCollectionChanged(
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, itemsRemoved));
            }
            this._isInXAllOperation = false;

            return flag;
        }

        /// <summary>
        ///     Retains only the elements in this set that are contained in the specified collection.
        /// </summary>
        /// <param name="c">Collection that defines the set of elements to be retained.</param>
        /// <returns>
        ///     <see langword="true" /> if this set changed as a result of this operation.
        /// </returns>
        public override bool RetainAll(ICollection<T> c)
        {
            bool flag = false;
            this._isInXAllOperation = true;
            var itemsRemoved = new List<T>();
            foreach (T item in (IEnumerable)this.Clone())
            {
                if (c.Contains(item))
                {
                    continue;
                }

                bool operationResult = this.Remove(item);
                flag |= operationResult;
                if (operationResult)
                {
                    itemsRemoved.Add(item);
                }
            }
            if (flag)
            {
                this.OnPropertyChanged(ISEMPTY_PROPERTY_NAME);
                this.OnPropertyChanged(COUNT_PROPERTY_NAME);
                this.OnCollectionChanged(
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, itemsRemoved));
            }
            this._isInXAllOperation = false;

            return flag;
        }

        #endregion

        #region Methods

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler changed = this.CollectionChanged;
            if (changed != null)
            {
                changed(this, e);
            }
        }

        private void OnCollectionReset()
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler changed = this.PropertyChanged;
            if (changed != null)
            {
                changed(this, e);
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
