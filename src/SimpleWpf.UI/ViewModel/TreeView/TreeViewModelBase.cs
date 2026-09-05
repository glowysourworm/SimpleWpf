using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

using SimpleWpf.Extensions.Event;

using SimpleWpf.Extensions.ObservableCollection;
using SimpleWpf.UI.ViewModel.TreeView.Interface;

namespace SimpleWpf.UI.ViewModel.TreeView
{
    /// <summary>
    /// Base class for a recursive view model which handles recursive iteration using IList (IEnumerable).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TreeViewModelBase : ViewModelBase, IDisposable, IEnumerable
    {
        /// <summary>
        /// Event that fires when collection's item property has changed. This event fires only at this level of the tree
        /// </summary>
        public event CollectionItemChangedHandler<ITreeViewNode> ItemPropertyChanged;

        /// <summary>
        /// (Bubble Up Event) Event that fires when collection has changed. This bubbles
        ///                   up the tree. So, setting this at the root will forward all tree collection events.
        /// </summary>
        public event TreeViewDelegates.CollectionChangedTreeEventHandler CollectionChangedTreeEvent;

        /// <summary>
        /// (Bubble Up Event) Event that fires when collection's item property has changed. This bubbles
        ///                   up the tree. So, setting this at the root will forward all tree item events.
        /// </summary>
        public event TreeViewDelegates.ItemPropertyChangedTreeEventHandler ItemPropertyChangedTreeEvent;

        // Parent Node
        TreeViewModelBase _parent;

        // Primary collection
        NotifyingObservableCollection<TreeViewModelBase> _children;

        // Current node's value
        ITreeViewNode _nodeValue;

        public TreeViewModelBase Parent
        {
            get { return _parent; }
            set { this.RaiseAndSetIfChanged(ref _parent, value); }
        }
        public NotifyingObservableCollection<TreeViewModelBase> Children
        {
            get { return _children; }
        }
        public ITreeViewNode NodeValue
        {
            get { return _nodeValue; }
        }
        public bool CanHaveChildren
        {
            get { return _nodeValue.CanHaveChildren; }
        }

        // Begin / End Update (pattern)
        bool _updating;

        public TreeViewModelBase(ITreeViewNode nodeValue, TreeViewModelBase parent = null)
        {
            _children = new NotifyingObservableCollection<TreeViewModelBase>();
            _nodeValue = nodeValue;
            _parent = parent;
            _nodeValue = nodeValue;

            _updating = false;

            _children.ItemPropertyChanged += OnItemPropertyChanged;
            _nodeValue.PropertyChanged += OnNodeValuePropertyChanged;
        }

        /// <summary>
        /// Constructs instance of the tree's node for the child collection
        /// </summary>
        protected abstract TreeViewModelBase Construct(ITreeViewNode nodeValue);

        // Method used for recursive members (includes current node for action)
        private void Recurse(Action<TreeViewModelBase> action, bool leafFirst = false, bool childrenOnly = false)
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

        #region IEnumerable Methods
        public IEnumerator GetEnumerator()
        {
            return new TreeViewEnumerator(this);
        }
        #endregion

        #region IList Methods

        /// <summary>
        /// Recursively iterates the collection. This method must not overlap with IEnumerable due to framework
        /// usage. e.g. is the HierarchicalDataTemplate - which will then treat the tree as a flat list.
        /// </summary>
        public void RecurseForEach(Action<TreeViewModelBase> action)
        {
            Recurse(action);
        }
        public int RecursiveCount()
        {
            var count = 0;
            Recurse(x => count++);
            return count;
        }
        public int RecursiveCount(Func<ITreeViewNode, bool> predicate)
        {
            var count = 0;
            Recurse(x =>
            {
                if (predicate(x.NodeValue))
                    count++;
            });
            return count;
        }
        public IEnumerable<ITreeViewNode> RecursiveWhere(Func<ITreeViewNode, bool> predicate)
        {
            var result = new List<ITreeViewNode>();

            Recurse(x =>
            {
                if (predicate(x.NodeValue))
                    result.Add(x.NodeValue);
            });

            return result;
        }

        public bool HasDirectAncestor(TreeViewModelBase subTree)
        {
            if (subTree == this)
                return true;

            if (this.Parent != null)
                return this.Parent.HasDirectAncestor(subTree);

            return false;
        }

        /// <summary>
        /// (Non-Recursive Method!) Adds an item to CURRENT DEPTH of the tree ONLY. Returns the new node.
        /// </summary>
        /// <exception cref="ArgumentException">Depths do not match for inserted item</exception>
        public TreeViewModelBase Add(ITreeViewNode item)
        {
            if (item == null)
                throw new NullReferenceException("Trying to insert null value into recursive tree view model");

            if (!this.CanHaveChildren)
                throw new Exception("Trying to add a node to a sub-tree that has not set the proper CanHaveChildren value on its nodes");

            // NEW NODE:  Use this opportunity to hook tree events
            var node = Construct(item);

            node.ItemPropertyChanged += OnItemPropertyChanged;
            node.NodeValue.PropertyChanged += OnNodeValuePropertyChanged;

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
            }

            _children.Clear();
        }

        /// <summary>
        /// (Recursive Method) Checks tree (from this depth downward) for the item
        /// </summary>
        public bool Contains(ITreeViewNode item)
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
        public bool Remove(ITreeViewNode item)
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

                    _children.RemoveAt(index);
                    return true;
                }
            }

            // Collection must contain the item
            throw new Exception("Application Error: Item not found the tree (starting at this depth!):  RecursiveNodeViewModel.Remove");
        }

        #endregion

        // Begin / End Update:  Blocking events is needed for handling selection. These methods are invoked by the user code
        //                      to prevent selection from bogging down recursion loops.
        //
        public void BeginUpdate()
        {
            if (_updating)
                throw new Exception("Update already in progress for the TreeViewModelBase");

            _updating = true;
        }

        public void EndUpdate()
        {
            if (!_updating)
                throw new Exception("Update not in progress for the TreeViewModelBase");

            _updating = false;
        }

        // Tree Collection Events
        private void OnTreeItemCollectionChanged(TreeViewModelBase treeSender, object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (_updating)
                return;

            // (There may be listeners at this level)
            if (this.CollectionChangedTreeEvent != null)
                this.CollectionChangedTreeEvent(treeSender, sender, e);

            // -> Bubble Up
            //
            if (this.Parent != null)
                this.Parent.OnTreeItemCollectionChanged(treeSender, sender, e);
        }

        // Tree Item Events
        private void OnTreeItemPropertyChanged(TreeViewModelBase treeSender, ITreeViewNode item, PropertyChangedEventArgs e)
        {
            if (_updating)
                return;

            // (There may be listeners at this level)
            if (this.ItemPropertyChangedTreeEvent != null)
                this.ItemPropertyChangedTreeEvent(treeSender, item, e);

            // -> Bubble Up
            //
            if (this.Parent != null)
                this.Parent.OnTreeItemPropertyChanged(treeSender, item, e);
        }

        // Item Events
        private void OnItemPropertyChanged(ITreeViewNode item, PropertyChangedEventArgs propertyArgs)
        {
            if (_updating)
                return;

            if (this.ItemPropertyChanged != null)
                this.ItemPropertyChanged(item, propertyArgs);

            // -> Bubble Up
            //
            OnTreeItemPropertyChanged(this, item, propertyArgs);
        }

        // Item Events
        private void OnItemPropertyChanged(TreeViewModelBase item, PropertyChangedEventArgs propertyArgs)
        {
            if (_updating)
                return;

            if (this.ItemPropertyChanged != null)
                this.ItemPropertyChanged(item.NodeValue, propertyArgs);

            // -> Bubble Up
            //
            OnTreeItemPropertyChanged(this, item.NodeValue, propertyArgs);
        }

        // Item Events
        private void OnNodeValuePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_updating)
                return;

            if (this.ItemPropertyChanged != null)
                this.ItemPropertyChanged(sender as ITreeViewNode, e);

            // -> Bubble Up
            //
            OnTreeItemPropertyChanged(this, sender as ITreeViewNode, e);
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
                _nodeValue.PropertyChanged -= OnNodeValuePropertyChanged;
                _children = null;
            }
        }
    }
}
