using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

using SimpleWpf.Extensions.Collection;
using SimpleWpf.Extensions.Event;
using SimpleWpf.SimpleCollections.Collection;
using SimpleWpf.SimpleCollections.Extension;

namespace SimpleWpf.Extensions.ObservableCollection
{
    public class KeyedObservableCollection<K, V> : IEnumerable<V>, INotifyCollectionChanged where V : INotifyPropertyChanged
    {
        // Hash support for performance
        SimpleDictionary<K, V> _dictionary;

        public int Count { get { return _dictionary.Count; } }

        // INotifyCollectionChanged
        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        public event CollectionItemChangedHandler<V> ItemPropertyChanged;

        // BeginUpdate / EndUpdate
        //
        // This is an attempt at performance improvement from the UI side. During a large
        // collection change there will be a lot of extra binding events. So, there will
        // be a blocker to see how it helps; but it will need to be enacted from the user
        // code.
        //
        bool _updating;

        public KeyedObservableCollection()
        {
            _dictionary = new SimpleDictionary<K, V>();
            _updating = false;

            OnCollectionReset();
        }

        public void BeginUpdate()
        {
            _updating = true;
        }
        public void EndUpdate(bool notifyObservers = false)
        {
            _updating = false;

            if (notifyObservers)
                OnCollectionReset();
        }

        public V this[K key]
        {
            get { return _dictionary[key]; }
            set
            {
                if (ContainsKey(key))
                {
                    var oldItem = _dictionary[key];
                    var newItem = value;

                    _dictionary[key].PropertyChanged -= OnItemPropertyChanged;
                    _dictionary[key] = value;

                    value.PropertyChanged += OnItemPropertyChanged;

                    OnCollectionReplace(oldItem, newItem);
                }
                else
                    throw new Exception("Key not found in the dictionary");
            }
        }

        public bool ContainsKey(K key)
        {
            return _dictionary.ContainsKey(key);
        }

        public bool Remove(K key)
        {
            var value = _dictionary[key];

            value.PropertyChanged -= OnItemPropertyChanged;

            var index = _dictionary.IndexOf(pair => pair.Key.Equals(key));

            var returnValue = _dictionary.Remove(key);

            OnCollectionRemove(value, index);

            return returnValue;
        }

        /// <summary>
        /// Removes items where they match the predicate
        /// </summary>
        public bool Remove(Func<V, bool> predicate)
        {
            var removed = _dictionary.Filter(pair => predicate(pair.Value));

            foreach (var pair in removed)
            {
                pair.Value.PropertyChanged -= OnItemPropertyChanged;
            }

            return true;
        }

        public void Add(K key, V value)
        {
            if (_dictionary.ContainsKey(key))
                throw new ArgumentException("Dictionary already contains element with the same key");

            value.PropertyChanged += OnItemPropertyChanged;

            _dictionary.Add(key, value);

            OnCollectionAdd(value);
        }

        public void Add(KeyValuePair<K, V> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            foreach (var pair in _dictionary)
            {
                pair.Value.PropertyChanged -= OnItemPropertyChanged;
            }

            _dictionary.Clear();

            OnCollectionReset();
        }

        public bool Contains(KeyValuePair<K, V> item)
        {
            return ContainsKey(item.Key);
        }

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<K, V> item)
        {
            return Remove(item.Key);
        }

        public IEnumerator GetEnumerator()
        {
            return _dictionary.Values.GetEnumerator();
        }
        IEnumerator<V> IEnumerable<V>.GetEnumerator()
        {
            return _dictionary.Values.GetEnumerator();
        }

        private void OnCollectionAdd(V item)
        {
            if (_updating)
                return;

            if (this.CollectionChanged != null)
                this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        private void OnCollectionRemove(V item, int index)
        {
            if (_updating)
                return;

            if (this.CollectionChanged != null)
                this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
        }

        private void OnCollectionReplace(V oldItem, V newItem)
        {
            if (_updating)
                return;

            if (this.CollectionChanged != null)
                this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, newItem, oldItem));
        }

        private void OnCollectionReset()
        {
            if (_updating)
                return;

            if (this.CollectionChanged != null)
                this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_updating)
                return;

            if (this.ItemPropertyChanged != null)
                this.ItemPropertyChanged((V)sender, e);
        }
    }
}
