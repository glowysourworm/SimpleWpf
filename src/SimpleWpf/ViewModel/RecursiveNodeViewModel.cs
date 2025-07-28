using System.Collections;
using System.Collections.Specialized;

using SimpleWpf.Extensions.Event;

using SimpleWpf.Extensions.ObservableCollection;

namespace SimpleWpf.ViewModel
{
    /// <summary>
    /// Base class for a recursive view model which handles recursive iteration using IList (IEnumerable).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class RecursiveNodeViewModel<T> : ViewModelBase, IDisposable, INotifyCollectionChanged, IEnumerable<T> where T : RecursiveViewModelBase
    {
        /// <summary>
        /// (RECURSIVE!) Event that fires when collection has changed. This should be
        ///              set on the ROOT of the tree only!
        /// </summary>
        public event NotifyCollectionChangedEventHandler? CollectionChanged
        {
            add { Recurse(x => x.CollectionChanged += value); }
            remove { Recurse(x => x.CollectionChanged -= value); }
        }

        /// <summary>
        /// (RECURSIVE!) Event that fires when collection's item property has changed. This should be
        ///              set on the ROOT of the tree only!
        /// </summary>
        public event CollectionItemChangedHandler<T> ItemPropertyChanged
        {
            add { Recurse(x => x.ItemPropertyChanged += value); }
            remove { Recurse(x => x.ItemPropertyChanged -= value); }
        }

        // Parent Node
        RecursiveNodeViewModel<T> _parent;

        // Primary collection
        NotifyingObservableCollection<RecursiveNodeViewModel<T>> _children;

        // Current node's value
        T _nodeValue;

        public RecursiveNodeViewModel<T> Parent
        {
            get { return _parent; }
            set { this.RaiseAndSetIfChanged(ref _parent, value); }
        }
        public NotifyingObservableCollection<RecursiveNodeViewModel<T>> Children
        {
            get { return _children; }
        }
        public T NodeValue
        {
            get { return _nodeValue; }
        }

        public RecursiveNodeViewModel(T nodeValue, RecursiveNodeViewModel<T> parent = null)
        {
            _children = new NotifyingObservableCollection<RecursiveNodeViewModel<T>>();
            _nodeValue = nodeValue;
            _parent = parent;
            _nodeValue = nodeValue;
        }


        /// <summary>
        /// Constructs instance of the tree's node for the child collection
        /// </summary>
        protected abstract RecursiveNodeViewModel<T> Construct(T nodeValue);

        // Hooks all events to top-level handlers. Then, the public events are bubbled up.
        //
        private void Recurse(Action<RecursiveNodeViewModel<T>> action)
        {
            action(this);

            // Recursive Iterator
            foreach (var item in _children)
            {
                action(item);
            }
        }

        #region IList Methods

        public IEnumerator<T> GetEnumerator()
        {
            return new RecursiveEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// (Non-Recursive Method!) Adds an item to CURRENT DEPTH of the tree ONLY. Returns the new node.
        /// </summary>
        /// <exception cref="ArgumentException">Depths do not match for inserted item</exception>
        public RecursiveNodeViewModel<T> Add(T item)
        {
            if (item == null)
                throw new NullReferenceException("Trying to insert null value into recursive tree view model");

            var node = Construct(item);

            _children.Add(node);

            return node;
        }

        /// <summary>
        /// (Recursive Method) Clears tree starting at this depth
        /// </summary>
        public void Clear()
        {
            Recurse(x => x.Clear());

            _children.Clear();
        }

        /// <summary>
        /// (Recursive Method) Checks tree (from this depth downward) for the item
        /// </summary>
        public bool Contains(T item)
        {
            var contains = false;

            Recurse(x =>
            {
                if (x.NodeValue == item || x.Children.Any(z => z.NodeValue == item))
                    contains = true;
            });

            return contains;
        }

        /// <summary>
        /// Removes item (FROM THIS DEPTH ONLY!) This is a non-recursive method.
        /// </summary>
        public bool Remove(T item)
        {
            // NON-RECURSIVE
            for (int index = _children.Count - 1; index >= 0; index--)
            {
                if (_children[index].NodeValue == item)
                {
                    _children.RemoveAt(index);
                    return true;
                }
            }

            // Collection must contain the item
            throw new Exception("Application Error: Item not found the tree (starting at this depth!):  RecursiveNodeViewModel.Remove");
        }

        #endregion

        public void Dispose()
        {
            if (_children != null)
            {
                Recurse(x => x.Dispose());

                _children.Clear();
                _children = null;
            }
        }
    }
}
