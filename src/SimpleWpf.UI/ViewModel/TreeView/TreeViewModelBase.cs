using System.Collections;
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
        /// <summary>
        /// Notifies to the iterator code what to do with actions and predicates for
        /// the caller
        /// </summary>
        protected enum IteratorContinuation
        {
            Continue = 0,
            BreakAndReturn
        }

        /// <summary>
        /// (Bubble Up Event) This is a tree-wide event for property changed events
        /// </summary>
        public event TreeViewDelegates.ItemPropertyChangedTreeEventHandler ItemPropertyChanged;

        // Parent Node
        TreeViewModelBase? _parent;

        // Primary collection
        KeyedObservableCollection<int, TreeViewModelBase> _children;

        // Basic Properties
        bool _isLoaded;
        bool _isExpanded;
        bool _isSelected;
        int _recursionDepth;

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
        public int RecursionDepth
        {
            get { return _recursionDepth; }
            set { this.RaiseAndSetIfChanged(ref _recursionDepth, value); }
        }

        // Begin / End Update (pattern)
        bool _updating;

        public TreeViewModelBase(int recursionDepth, TreeViewModelBase? parent)
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
        private void Recurse(Func<TreeViewModelBase, IteratorContinuation> userFunc, bool leafFirst = false, bool childrenOnly = false)
        {
            if (!leafFirst && !childrenOnly)
            {
                if (userFunc(this) == IteratorContinuation.BreakAndReturn)
                    return;
            }

            // Recursive Iterator
            foreach (TreeViewModelBase item in _children)
            {
                item.Recurse(userFunc);
            }

            if (leafFirst && !childrenOnly)
            {
                if (userFunc(this) == IteratorContinuation.BreakAndReturn)
                    return;
            }
        }

        private void Recurse<T>(Func<T, IteratorContinuation> userFunc, bool leafFirst = false, bool childrenOnly = false) where T : TreeViewModelBase
        {
            if (this is not T)
                throw new ArgumentException("Invalid cast of tree node");

            if (!leafFirst && !childrenOnly)
            {
                if (userFunc(this as T) == IteratorContinuation.BreakAndReturn)
                    return;
            }

            // Recursive Iterator
            foreach (TreeViewModelBase item in _children)
            {
                item.Recurse(userFunc);
            }

            if (leafFirst && !childrenOnly)
            {
                if (userFunc(this as T) == IteratorContinuation.BreakAndReturn)
                    return;
            }
        }

        #region IList Methods

        /// <summary>
        /// Recursively iterates the collection. This method must not overlap with IEnumerable due to framework
        /// usage. e.g. is the HierarchicalDataTemplate - which will then treat the tree as a flat list.
        /// </summary>
        public void RecurseForEach(Action<TreeViewModelBase> action)
        {
            this.RecurseForEach<TreeViewModelBase>(action);
        }

        /// <summary>
        /// (Casted) Recursively iterates the collection. This method must not overlap with IEnumerable due to framework
        /// usage. e.g. is the HierarchicalDataTemplate - which will then treat the tree as a flat list.
        /// </summary>
        public void RecurseForEach<T>(Action<T> action) where T : TreeViewModelBase
        {
            Recurse<T>((node) =>
            {
                action(node);
                return IteratorContinuation.Continue;
            });
        }

        public int RecursiveCount()
        {
            return this.RecursiveCount(x => true);
        }
        public int RecursiveCount(Func<TreeViewModelBase, bool> predicate)
        {
            return this.RecursiveCount<TreeViewModelBase>(predicate);
        }
        public int RecursiveCount<T>(Func<T, bool> predicate) where T : TreeViewModelBase
        {
            var count = 0;
            Recurse<T>(x =>
            {
                if (predicate(x))
                    count++;

                return IteratorContinuation.Continue;
            });
            return count;
        }
        public IEnumerable<TreeViewModelBase> RecursiveWhere(Func<TreeViewModelBase, bool> predicate)
        {
            return this.RecursiveWhere<TreeViewModelBase>(predicate);
        }
        public IEnumerable<T> RecursiveWhere<T>(Func<T, bool> predicate) where T : TreeViewModelBase
        {
            var result = new List<T>();

            Recurse<T>(x =>
            {
                if (predicate(x))
                    result.Add(x);

                return IteratorContinuation.Continue;
            });

            return result;
        }

        public bool RecursiveAll(Func<TreeViewModelBase, bool> predicate)
        {
            return this.RecursiveAll<TreeViewModelBase>(predicate);
        }
        public bool RecursiveAll<T>(Func<T, bool> predicate) where T : TreeViewModelBase
        {
            var result = true;

            Recurse<T>(node =>
            {
                if (!predicate(node))
                {
                    result = false;
                    return IteratorContinuation.BreakAndReturn;
                }

                return IteratorContinuation.Continue;
            });

            return result;
        }
        public bool RecursiveAny(Func<TreeViewModelBase, bool> predicate)
        {
            return this.RecursiveAny<TreeViewModelBase>(predicate);
        }
        public bool RecursiveAny<T>(Func<T, bool> predicate) where T : TreeViewModelBase
        {
            var result = false;

            Recurse<T>(node =>
            {
                if (predicate(node))
                {
                    result = true;
                    return IteratorContinuation.BreakAndReturn;
                }

                return IteratorContinuation.Continue;
            });

            return result;
        }
        public TreeViewModelBase? RecursiveFirst(Func<TreeViewModelBase, bool> predicate)
        {
            return this.RecursiveFirst<TreeViewModelBase>(predicate);
        }
        public T? RecursiveFirst<T>(Func<T, bool> predicate) where T : TreeViewModelBase
        {
            T? result = null;

            Recurse<T>(node =>
            {
                if (predicate(node))
                {
                    result = node;
                    return IteratorContinuation.BreakAndReturn;
                }

                return IteratorContinuation.Continue;
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

            item.PropertyChanged += OnItemPropertyChanged;

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

        private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // This should only be subscribed at the root
            if (this.ItemPropertyChanged != null)
                this.ItemPropertyChanged(this, sender, e);

            // Bubble Up!
            else if (this.Parent != null)
                this.Parent.OnItemPropertyChanged(sender, e);
        }

        /// <summary>
        /// (Recursive Method) Clears tree starting at this depth
        /// </summary>
        public void Clear()
        {
            // Leaf First:  Runs the delegate after iterating the children (recursively)
            Recurse(x =>
            {
                x.ClearImpl();
                return IteratorContinuation.Continue;

            }, true);
        }

        private void ClearImpl()
        {
            // Unhook Events
            foreach (TreeViewModelBase node in _children)
            {
                node.PropertyChanged -= OnItemPropertyChanged;
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
                {
                    contains = true;
                    return IteratorContinuation.BreakAndReturn;
                }

                return IteratorContinuation.Continue;
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

        public void Dispose()
        {
            Clear();
        }
    }
}
