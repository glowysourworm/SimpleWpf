using System.Collections.Specialized;
using System.ComponentModel;

using SimpleWpf.Extensions.Event;

using SimpleWpf.Extensions.ObservableCollection;

namespace SimpleWpf.ViewModel
{
    /// <summary>
    /// Base class for a recursive view model which handles recursive iteration using IList (IEnumerable).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class RecursiveDispatcherViewModel<T> : ViewModelBase, IDisposable, INotifyCollectionChanged where T : DispatcherViewModelBase
    {
        /// <summary>
        /// (Bubble Up Event) Event that fires when collection has changed. This bubbles
        ///                   up the tree. So, setting this at the root will forward all tree collection events.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// (Bubble Up Event) Event that fires when collection's item property has changed. This bubbles
        ///                   up the tree. So, setting this at the root will forward all tree item events.
        /// </summary>
        public event CollectionItemChangedHandler<T> ItemPropertyChanged;

        // Parent Node
        RecursiveDispatcherViewModel<T> _parent;

        // Primary collection
        NotifyingObservableCollection<RecursiveDispatcherViewModel<T>> _children;

        // Current node's value
        T _nodeValue;

        public RecursiveDispatcherViewModel<T> Parent
        {
            get { return _parent; }
            set { this.RaiseAndSetIfChanged(ref _parent, value); }
        }
        public NotifyingObservableCollection<RecursiveDispatcherViewModel<T>> Children
        {
            get { return _children; }
        }
        public T NodeValue
        {
            get { return _nodeValue; }
        }

        public RecursiveDispatcherViewModel(T nodeValue, RecursiveDispatcherViewModel<T> parent = null)
        {
            _children = new NotifyingObservableCollection<RecursiveDispatcherViewModel<T>>();
            _nodeValue = nodeValue;
            _parent = parent;
            _nodeValue = nodeValue;

            _children.ItemPropertyChanged += OnItemPropertyChanged;
            _children.CollectionChanged += OnItemCollectionChanged;
            _nodeValue.PropertyChanged += OnNodeValuePropertyChanged;
        }

        /// <summary>
        /// Constructs instance of the tree's node for the child collection
        /// </summary>
        protected abstract RecursiveDispatcherViewModel<T> Construct(T nodeValue);

        // Method used for recursive members (includes current node for action)
        private void Recurse(Action<RecursiveDispatcherViewModel<T>> action, bool leafFirst = false, bool childrenOnly = false)
        {
            if (!leafFirst && !childrenOnly)
                action(this);

            // Recursive Iterator
            foreach (var item in _children)
            {
                item.Recurse(action);
            }

            if (leafFirst && !childrenOnly)
                action(this);
        }

        #region IList Methods

        /// <summary>
        /// Recursively iterates the collection. This method must not overlap with IEnumerable due to framework
        /// usage. e.g. is the HierarchicalDataTemplate - which will then treat the tree as a flat list.
        /// </summary>
        public void RecurseForEach(Action<RecursiveDispatcherViewModel<T>> action)
        {
            Recurse(action);
        }
        public int RecursiveCount()
        {
            var count = 0;
            Recurse(x => count++);
            return count;
        }
        public int RecursiveCount(Func<T, bool> predicate)
        {
            var count = 0;
            Recurse(x =>
            {
                if (predicate(x.NodeValue))
                    count++;
            });
            return count;
        }
        public IEnumerable<T> RecursiveWhere(Func<T, bool> predicate)
        {
            var result = new List<T>();

            Recurse(x =>
            {
                if (predicate(x.NodeValue))
                    result.Add(x.NodeValue);
            });

            return result;
        }

        /// <summary>
        /// (Non-Recursive Method!) Adds an item to CURRENT DEPTH of the tree ONLY. Returns the new node.
        /// </summary>
        /// <exception cref="ArgumentException">Depths do not match for inserted item</exception>
        public RecursiveDispatcherViewModel<T> Add(T item)
        {
            if (item == null)
                throw new NullReferenceException("Trying to insert null value into recursive tree view model");

            // NEW NODE:  Use this opportunity to hook tree events
            var node = Construct(item);

            node.ItemPropertyChanged += OnItemPropertyChanged;
            node.NodeValue.PropertyChanged += OnNodeValuePropertyChanged;
            node.CollectionChanged += OnItemCollectionChanged;

            _children.Add(node);

            return node;
        }

        /// <summary>
        /// (Recursive Method) Clears tree starting at this depth
        /// </summary>
        public void Clear()
        {
            // Leaf First:  Runs the delegate after iterating the children (recursively)
            Recurse(x => x.ClearImpl(), true);
        }

        private void ClearImpl()
        {
            // Unhook Events
            foreach (var node in _children)
            {
                node.ItemPropertyChanged -= OnItemPropertyChanged;
                node.NodeValue.PropertyChanged -= OnNodeValuePropertyChanged;
                node.CollectionChanged -= OnItemCollectionChanged;
            }

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
                if (x.NodeValue == item)
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
                    var itemNode = _children[index];

                    // Unhook Events
                    itemNode.ItemPropertyChanged -= OnItemPropertyChanged;
                    itemNode.NodeValue.PropertyChanged -= OnNodeValuePropertyChanged;
                    itemNode.CollectionChanged -= OnItemCollectionChanged;

                    _children.RemoveAt(index);
                    return true;
                }
            }

            // Collection must contain the item
            throw new Exception("Application Error: Item not found the tree (starting at this depth!):  RecursiveNodeViewModel.Remove");
        }

        #endregion

        private void OnItemCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.CollectionChanged != null)
                this.CollectionChanged(sender, e);
        }

        private void OnItemPropertyChanged(T item, PropertyChangedEventArgs propertyArgs)
        {
            if (this.ItemPropertyChanged != null)
                this.ItemPropertyChanged(item, propertyArgs);
        }

        private void OnItemPropertyChanged(RecursiveDispatcherViewModel<T> item, PropertyChangedEventArgs propertyArgs)
        {
            if (this.ItemPropertyChanged != null)
                this.ItemPropertyChanged(item.NodeValue, propertyArgs);
        }

        private void OnNodeValuePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (this.ItemPropertyChanged != null)
                this.ItemPropertyChanged(sender as T, e);
        }

        public void Dispose()
        {
            if (_children != null)
            {
                Recurse(x => x.DisposeImpl(), true);
            }
        }
        private void DisposeImpl()
        {
            if (_children != null)
            {
                Clear();
                _children.ItemPropertyChanged -= OnItemPropertyChanged;
                _children.CollectionChanged -= OnItemCollectionChanged;
                _nodeValue.PropertyChanged -= OnNodeValuePropertyChanged;
                _children = null;
            }
        }
    }
}
