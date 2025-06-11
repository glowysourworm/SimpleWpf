using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

using SimpleWpf.RecursiveSerializer.Component.Interface;
using SimpleWpf.RecursiveSerializer.Interface;
using SimpleWpf.SimpleCollections.Collection;

namespace SimpleWpf.Extensions.ObservableCollection
{
    /// <summary>
    /// A simple ordered list implementation - sorts items when inserted and removed. NOTE*** The binding views seemed to "want"
    /// the IList implementation (!!!?) It must've been required for bindings to operate.
    /// </summary>
    public class SortedObservableCollection<T> : IList<T>, INotifyPropertyChanged, INotifyCollectionChanged, IRecursiveSerializable
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Item hash table used for fast reference checking. Example:  "Contains", or "IndexOf" must 
        /// lookup the item using the provided comparer. This comparer will assume that the item will be
        /// used during the BinarySearch procedure; but it may not be a part of the collection. So, for
        /// reference we need to know if the user's item is a part of our collection or not. Then, it
        /// we will need a hash table to look it up; or else the point of this collection would be lost
        /// to linear searches.
        /// </summary>
        protected SimpleDictionary<T, T> ItemHash { get; private set; }

        /// <summary>
        /// Protected list of items
        /// </summary>
        protected List<T> ItemList { get; private set; }

        /// <summary>
        /// Equality comparer for sorting
        /// </summary>
        protected Comparer<T> ItemComparer { get; private set; }

        const int UNSUCCESSFUL_SEARCH = -1;

        public SortedObservableCollection()
        {
            this.ItemList = new List<T>();
            this.ItemComparer = Comparer<T>.Default;
            this.ItemHash = new SimpleDictionary<T, T>();

            OnPropertyChanged("Count");
        }

        public SortedObservableCollection(Comparer<T> comparer)
        {
            this.ItemList = new List<T>();
            this.ItemComparer = comparer;
            this.ItemHash = new SimpleDictionary<T, T>();

            OnPropertyChanged("Count");
        }

        public SortedObservableCollection(IEnumerable<T> items)
        {
            this.ItemList = new List<T>(items);
            this.ItemComparer = Comparer<T>.Default;
            this.ItemHash = new SimpleDictionary<T, T>();

            foreach (var item in items)
                this.ItemHash.Add(item, item);
        }

        public SortedObservableCollection(IEnumerable<T> items, Comparer<T> itemComparer)
        {
            this.ItemList = new List<T>(items);
            this.ItemComparer = itemComparer;
            this.ItemHash = new SimpleDictionary<T, T>();

            foreach (var item in items)
                this.ItemHash.Add(item, item);
        }

        public SortedObservableCollection(IPropertyReader reader)
        {
            this.ItemList = reader.Read<List<T>>("List");
            this.ItemComparer = reader.Read<Comparer<T>>("Comparer");
            this.ItemHash = new SimpleDictionary<T, T>();

            foreach (var item in this.ItemList)
                this.ItemHash.Add(item, item);
        }

        public void GetProperties(IPropertyWriter writer)
        {
            writer.Write("List", this.ItemList);
            writer.Write("Comparer", this.ItemComparer);
        }

        public void AddRange(IEnumerable<T> items)
        {
            foreach (T item in items)
                Add(item);              // Notify
        }

        // Functions Watched:  Add, Remove, RemoveAt, Clear
        private void OnCollectionChanged_Add(T item, int index)
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            }

            // Also -> Property Changed (Count)
            OnPropertyChanged("Count");
        }
        private void OnCollectionChanged_Remove(T item, int index)
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
            }

            // Also -> Property Changed (Count)
            OnPropertyChanged("Count");
        }
        private void OnCollectionChanged_Clear()
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

            // Also -> Property Changed (Count)
            OnPropertyChanged("Count");
        }

        // Functions Watched:  Count (also, follows collection changes)
        private void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #region (public) IList<T> IList

        public T this[int index]
        {
            get { return this.ItemList[index]; }
            set { throw new Exception("Manual insert not supported for SimpleOrderedList<>"); }
        }

        public int Count
        {
            get { return this.ItemList.Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return this; }
        }

        // O(log n)
        public void Add(T item)
        {
            var index = GetInsertIndex(item);

            this.ItemList.Insert(index, item);
            this.ItemHash.Add(item, item);

            OnCollectionChanged_Add(item, index);
        }

        public void Clear()
        {
            this.ItemList.Clear();
            this.ItemHash.Clear();

            OnCollectionChanged_Clear();
        }

        // O(1)
        public bool Contains(T item)
        {
            return this.ItemHash.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.ItemList.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.ItemList.GetEnumerator();
        }

        // O(log n)
        public int IndexOf(T item)
        {
            if (!this.Contains(item))
                return -1;

            return GetInsertIndex(item);
        }

        public void Insert(int index, T item)
        {
            throw new NotSupportedException("Manual insertion not allowed for SimpleOrderedList<>");
        }

        // O(log n)
        public bool Remove(T item)
        {
            if (!this.Contains(item))
                throw new Exception("Item not found in collection SimpleOrderedList.cs");

            // Still need index to remove from the list
            var index = GetInsertIndex(item);

            if (index == UNSUCCESSFUL_SEARCH)
                throw new Exception("Item not found in collection SimpleOrderedList.cs");

            this.ItemList.RemoveAt(index);
            this.ItemHash.Remove(item);

            OnCollectionChanged_Remove(item, index);

            return true;
        }

        public void RemoveAt(int index)
        {
            var item = this.ItemList[index];

            this.ItemList.RemoveAt(index);
            this.ItemHash.Remove(item);

            OnCollectionChanged_Remove(item, index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.ItemList.GetEnumerator();
        }

        public void Insert(int index, object value)
        {
            throw new NotSupportedException("Manual insertion not allowed for SimpleOrderedList<>");
        }


        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region (private) Binary Search Implementation

        private int BinarySearch(T searchItem, out int insertIndex)
        {
            /*
                function binary_search(A, n, T) is
                    L := 0
                    R := n − 1
                    while L ≤ R do
                        m := floor((L + R) / 2)
                        if A[m] < T then
                            L := m + 1
                        else if A[m] > T then
                            R := m − 1
                        else:
                            return m
                    return unsuccessful
             */

            var leftIndex = 0;
            var rightIndex = this.ItemList.Count - 1;

            // Initialize insert index to be the left index
            insertIndex = leftIndex;

            while (leftIndex <= rightIndex)
            {
                var middleIndex = (int)Math.Floor((leftIndex + rightIndex) / 2.0D);
                var item = this.ItemList[middleIndex];

                // Set insert index
                insertIndex = middleIndex;

                // Item's value is LESS THAN search value
                if (this.ItemComparer.Compare(item, searchItem) < 0)
                {
                    leftIndex = middleIndex + 1;

                    // Set insert index for catching final iteration
                    insertIndex = leftIndex;
                }

                // GREATER THAN
                else if (this.ItemComparer.Compare(item, searchItem) > 0)
                    rightIndex = middleIndex - 1;

                else
                    return middleIndex;
            }

            return UNSUCCESSFUL_SEARCH;
        }

        private int GetInsertIndex(T item)
        {
            var insertIndex = UNSUCCESSFUL_SEARCH;
            var searchIndex = BinarySearch(item, out insertIndex);

            // NOT FOUND
            if (searchIndex == UNSUCCESSFUL_SEARCH)
            {
                return insertIndex;
            }
            else
            {
                return searchIndex;
            }
        }
        #endregion
    }
}
