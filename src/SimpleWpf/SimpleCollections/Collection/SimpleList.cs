using System.Collections;

using SimpleWpf.Extensions.Collection;

namespace SimpleWpf.SimpleCollections.Collection
{
    public class SimpleList<T> : IList<T>, IEnumerable<T>
    {
        T[] _array;
        int _listCurrentLength;

        T IList<T>.this[int index]
        {
            get { return _array[index]; }
            set { _array[index] = (T)value; }
        }

        public bool IsFixedSize { get { return false; } }
        public bool IsReadOnly { get { return false; } }
        public int Count { get { return _listCurrentLength; } }
        public bool IsSynchronized { get { return false; } }
        public object SyncRoot
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Length with which to resize the underlying array. The default is 10. This may not be
        /// needed until there is a large collection to manage, in which case you may want to 
        /// use large increments.
        /// </summary>
        public int ListResizeAmount { get; private set; }

        public SimpleList()
        {
            _array = new T[0];

            this.ListResizeAmount = 10;
        }

        public SimpleList(IEnumerable<T> copy)
        {
            var count = copy.Count();

            _array = new T[count];
            this.ListResizeAmount = 10;
        }

        public SimpleList(IEnumerable<T> copy, int listResizeAmount)
        {
            var count = copy.Count();

            _array = new T[count];
            this.ListResizeAmount = listResizeAmount;
        }

        public void Add(T item)
        {
            Insert(_listCurrentLength, item);
        }

        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public void Clear()
        {
            _array = new T[0];
            _listCurrentLength = 0;
        }

        public bool Contains(T item)
        {
            return _array.Contains(item);
        }

        public void CopyTo(Array array, int index)
        {
            _array.CopyTo(array, index);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _array.CopyTo(array, arrayIndex);
        }

        public IEnumerator GetEnumerator()
        {
            return _array.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return _array.IndexOf(item);
        }

        public void Insert(int insertIndex, T item)
        {
            if (_array.Length == _listCurrentLength)
            {
                Expand();
            }

            if (insertIndex >= _array.Length || insertIndex < 0)
                throw new IndexOutOfRangeException("Index out of range:  SimpleList.Insert()");

            // Move elements back starting at end of the array and indexing backwards
            //
            for (int index = _listCurrentLength - 1; index >= insertIndex; index--)
            {
                _array[index + 1] = _array[index];
            }

            // Insert new element
            _array[insertIndex] = item;

            // Increment list length
            _listCurrentLength++;
        }

        public bool Remove(T item)
        {
            var removeIndex = this.IndexOf(item);

            RemoveAt(removeIndex);

            return true;
        }

        public void RemoveAt(int removeIndex)
        {
            if (removeIndex >= _listCurrentLength)
            {
                throw new IndexOutOfRangeException("Internal Error (SimpleWpf):  Index is out of range of the current list length!");
            }
            else if (removeIndex >= 0)
            {
                // Move elements towards the front starting at the removed index
                //
                for (int index = removeIndex; index < _listCurrentLength - 1; index++)
                {
                    _array[index] = _array[index + 1];
                }

                // Decrement list length
                _listCurrentLength--;
            }
            else
            {
                throw new ArgumentException("SimpleList item not found in underlying array");
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return (IEnumerator<T>)_array.GetEnumerator();
        }

        #region (private) List Methods
        private void Expand()
        {
            // Create array of current length
            var tempArray = new T[_listCurrentLength];

            // Copy elements from the current array (efficiency)
            Array.Copy(_array, tempArray, _listCurrentLength);

            // Resize Array
            _array = new T[_listCurrentLength + this.ListResizeAmount];

            // Copy elements back from the temporary array
            Array.Copy(tempArray, _array, _listCurrentLength);
        }
        #endregion
    }
}
