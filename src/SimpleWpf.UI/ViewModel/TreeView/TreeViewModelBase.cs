using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

using SimpleWpf.Extensions.Collection;
using SimpleWpf.Extensions.ObservableCollection;

namespace SimpleWpf.UI.ViewModel.TreeView
{
    /// <summary>
    /// Simple Tree View:  This is the base class for a recursive-friendly model that handles: selection; bubble-up selection;
    ///                    lazy loading; and item property changed events (+ bubble-up handling); and, also, grouped property
    ///                    events.
    /// </summary>
    public abstract class TreeViewModelBase : ViewModelBase, IEnumerable, IDisposable
    {
        // Parent Node
        TreeViewModelBase? _parent;

        // Primary collection
        KeyedObservableCollection<int, TreeViewModelBase> _children;

        // Basic Properties
        bool _isLoaded;
        bool _isExpanded;
        bool _isSelected;

        // Tree Ordering:  Tree item order may be applied to the tree to
        //                 make multiple selection simple and more efficient.
        //
        //                 The method to do this should be called after 
        //                 initializing the tree; or after inserting new
        //                 nodes.
        //
        //                 This may be turned off
        //
        static int _TREE_ITEM_COUNTER = 0;

        /// <summary>
        /// ID set when numbering the tree
        /// </summary>
        public int ItemId { get; private set; }

        /// <summary>
        /// Check to see if the tree is numbered. This should be cascade-set starting from the root.
        /// </summary>
        public bool IsNumbered { get; private set; }

        public TreeViewModelBase? Parent
        {
            get { return _parent; }
            set { this.RaiseAndSetIfChanged(ref _parent, value); }
        }
        public IReadOnlyCollection<TreeViewModelBase> Children
        {
            get { return _children; }
        }
        public bool IsLoaded
        {
            get { return _isLoaded; }
            set { this.RaiseAndSetIfChanged(ref _isLoaded, value); }
        }
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { this.RaiseAndSetIfChanged(ref _isExpanded, value); }
        }
        public bool IsSelected
        {
            get { return _isSelected; }
            set { this.RaiseAndSetIfChanged(ref _isSelected, value); }
        }

        // Begin / End Update (pattern)
        bool _updating;

        public TreeViewModelBase(TreeViewModelBase? parent)
        {
            _children = new KeyedObservableCollection<int, TreeViewModelBase>();
            _parent = parent;

            _updating = false;

            // The ItemId is set when calling "Add". The IsNumbered flag is set
            // once the user calls "SetTreeNumbering" from the root.
            this.ItemId = 0;
            this.IsNumbered = false;
        }

        #region IEnumerable Methods
        public IEnumerator GetEnumerator()
        {
            return new TreeViewEnumerator(this);
        }
        #endregion

        // Method used for recursive members (includes current node for action)
        private void Recurse(Action<TreeViewModelBase> action, bool leafFirst = false, bool childrenOnly = false)
        {
            if (!leafFirst && !childrenOnly)
                action(this);

            // Recursive Iterator
            foreach (TreeViewModelBase item in _children)
            {
                item.Recurse(action);
            }

            if (leafFirst && !childrenOnly)
                action(this);
        }

        private void Recurse<T>(Action<T> action, bool leafFirst = false, bool childrenOnly = false) where T : TreeViewModelBase
        {
            if (this is not T)
                throw new ArgumentException("Invalid cast of tree node");

            if (!leafFirst && !childrenOnly)
                action(this as T);

            // Recursive Iterator
            foreach (TreeViewModelBase item in _children)
            {
                item.Recurse(action);
            }

            if (leafFirst && !childrenOnly)
                action(this as T);
        }

        #region IList Methods

        /// <summary>
        /// Recursively iterates the collection. This method must not overlap with IEnumerable due to framework
        /// usage. e.g. is the HierarchicalDataTemplate - which will then treat the tree as a flat list.
        /// </summary>
        public void RecurseForEach(Action<TreeViewModelBase> action)
        {
            Recurse(action);
        }

        /// <summary>
        /// (Casted) Recursively iterates the collection. This method must not overlap with IEnumerable due to framework
        /// usage. e.g. is the HierarchicalDataTemplate - which will then treat the tree as a flat list.
        /// </summary>
        public void RecurseForEach<T>(Action<T> action) where T : TreeViewModelBase
        {
            Recurse<T>(action);
        }

        public int RecursiveCount()
        {
            var count = 0;
            Recurse(x => count++);
            return count;
        }
        public int RecursiveCount(Func<TreeViewModelBase, bool> predicate)
        {
            var count = 0;
            Recurse(x =>
            {
                if (predicate(x))
                    count++;
            });
            return count;
        }
        public int RecursiveCount<T>(Func<T, bool> predicate) where T : TreeViewModelBase
        {
            var count = 0;
            Recurse<T>(x =>
            {
                if (predicate(x))
                    count++;
            });
            return count;
        }
        public IEnumerable<TreeViewModelBase> RecursiveWhere(Func<TreeViewModelBase, bool> predicate)
        {
            var result = new List<TreeViewModelBase>();

            Recurse(x =>
            {
                if (predicate(x))
                    result.Add(x);
            });

            return result;
        }
        public IEnumerable<T> RecursiveWhere<T>(Func<T, bool> predicate) where T : TreeViewModelBase
        {
            var result = new List<T>();

            Recurse<T>(x =>
            {
                if (predicate(x))
                    result.Add(x);
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
        public TreeViewModelBase Add(TreeViewModelBase item)
        {
            if (item == null)
                throw new NullReferenceException("Trying to insert null value into recursive tree view model");

            //if (!this.CanHaveChildren)
            //    throw new Exception("Trying to add a node to a sub-tree that has not set the proper CanHaveChildren value on its nodes");

            //item.ItemPropertyChanged += OnItemPropertyChanged;
            //item.PropertyChanged += OnNodeValuePropertyChanged;

            item.ItemId = _TREE_ITEM_COUNTER++;

            _children.Add(item.ItemId, item);

            return item;
        }

        /// <summary>
        /// Re-numbers tree and sets flag used during multi-select
        /// </summary>
        public void SetTreeNumbering()
        {
            if (_parent != null)
                throw new Exception("Must call SetTreeNumbering at the root of the tree only");

            // Procedure:  The numbering should be depth-first down the tree
            //
            // 1) Reset the counter
            // 2) Clear child items
            // 3) -> Recurse Downward
            //       - Set ItemId
            //       - Set IsNumbered = true
            //

            // RESET COUNTER
            _TREE_ITEM_COUNTER = 0;

            SetTreeNumberingRecurse(this);

            this.IsNumbered = true;
            this.ItemId = 0;
        }

        private void SetTreeNumberingRecurse(TreeViewModelBase treeNode)
        {
            // Save Items
            var items = _children.Values.Actualize();

            // Clear Children
            _children.Clear();

            foreach (var item in items)
            {
                // Re-Number
                item.ItemId = ++_TREE_ITEM_COUNTER;
                item.IsNumbered = true;

                // Children -> Add
                _children.Add(item.ItemId, item);

                // -> Recurse (Depth First)
                item.SetTreeNumberingRecurse(item);
            }
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
                //node.ItemPropertyChanged -= OnItemPropertyChanged;
                //node.PropertyChanged -= OnNodeValuePropertyChanged;
            }

            _children.Clear();
        }

        /// <summary>
        /// (Recursive Method) Checks tree (from this depth downward) for the item
        /// </summary>
        public bool Contains(TreeViewModelBase item)
        {
            var contains = false;

            Recurse(x =>
            {
                if (x == item)
                    contains = true;
            });

            return contains;
        }

        /// <summary>
        /// Removes item (FROM THIS DEPTH ONLY!) This is a non-recursive method.
        /// </summary>
        public bool Remove(TreeViewModelBase item)
        {
            if (!_children.ContainsKey(item.ItemId))
                throw new ArgumentException("Item not contained within tree view children");

            return _children.Remove(item.ItemId);
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

            //// (There may be listeners at this level)
            //if (this.CollectionChangedTreeEvent != null)
            //    this.CollectionChangedTreeEvent(treeSender, sender, e);

            //// -> Bubble Up
            ////
            //if (this.Parent != null)
            //    this.Parent.OnTreeItemCollectionChanged(treeSender, sender, e);
        }

        // Tree Item Events
        private void OnTreeItemPropertyChanged(TreeViewModelBase treeSender, object item, PropertyChangedEventArgs e)
        {
            if (_updating)
                return;

            //// (There may be listeners at this level)
            //if (this.ItemPropertyChangedTreeEvent != null)
            //    this.ItemPropertyChangedTreeEvent(treeSender, item, e);

            //// -> Bubble Up
            ////
            //if (this.Parent != null)
            //    this.Parent.OnTreeItemPropertyChanged(treeSender, item, e);
        }

        // Item Events
        private void OnItemPropertyChanged(object item, PropertyChangedEventArgs propertyArgs)
        {
            if (_updating)
                return;

            //if (this.ItemPropertyChanged != null)
            //    this.ItemPropertyChanged(item, propertyArgs);

            //// -> Bubble Up
            ////
            //OnTreeItemPropertyChanged(this, item, propertyArgs);
        }

        // Item Events
        private void OnNodeValuePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_updating)
                return;

            //if (this.ItemPropertyChanged != null)
            //    this.ItemPropertyChanged(sender as TreeViewModelBase, e);

            //// -> Bubble Up
            ////
            //OnTreeItemPropertyChanged(this, sender as TreeViewModelBase, e);
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
                _children = null;
            }
        }
    }
}
