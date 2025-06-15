using System.Collections.ObjectModel;

using SimpleWpf.SimpleCollections.Collection;

namespace SimpleWpf.Extensions.ObservableCollection
{
    public class KeyedObservableCollection<K, V> : ObservableCollection<V>
    {
        SimpleDictionary<K, V> _dictionary;
        Func<V, K> _keySelector;

        public KeyedObservableCollection(Func<V, K> keySelector) : this(keySelector, Enumerable.Empty<V>())
        {

        }
        public KeyedObservableCollection(Func<V, K> keySelector, IEnumerable<V> collection) : base(collection)
        {
            _keySelector = keySelector;
            _dictionary = new SimpleDictionary<K, V>();

            // Overrides will take care of initialization
        }

        public V this[K key]
        {
            get { return _dictionary[key]; }
        }

        public bool ContainsKey(K key)
        {
            return _dictionary.ContainsKey(key);
        }

        public int IndexOfKey(K key)
        {
            var index = 0;

            foreach (var pair in _dictionary)
            {
                if (pair.Key.Equals(key))
                    return index;

                index++;
            }

            return -1;
        }

        public void RemoveByKey(K key)
        {
            if (!_dictionary.ContainsKey(key))
                throw new ArgumentException("Key not contained within the collection");

            var index = IndexOfKey(key);

            RemoveAt(index);
        }

        protected override void InsertItem(int index, V item)
        {
            base.InsertItem(index, item);

            _dictionary.Add(_keySelector(item), item);
        }

        protected override void ClearItems()
        {
            base.ClearItems();

            _dictionary.Clear();
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);

            var pair = _dictionary.ElementAt(index);

            _dictionary.Remove(pair.Key);
        }

        protected override void SetItem(int index, V item)
        {
            base.SetItem(index, item);

            var pair = _dictionary.ElementAt(index);

            _dictionary[pair.Key] = item;
        }
    }
}
