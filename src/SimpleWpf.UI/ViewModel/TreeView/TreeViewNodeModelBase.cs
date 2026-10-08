using System.Collections;
using System.ComponentModel;

using SimpleWpf.Extensions.ObservableCollection;

namespace SimpleWpf.UI.ViewModel.TreeView
{
    /// <summary>
    /// Simple Tree View:  This is the base class for a recursive-friendly model that handles: selection; bubble-up selection;
    ///                    lazy loading; and item property changed events (+ bubble-up handling); and, also, grouped property
    ///                    events.
    /// </summary>
    public abstract class TreeViewNodeModelBase : ViewModelBase
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
        TreeViewNodeModelBase? _parent;

        // Primary collection
        KeyedObservableCollection<object, TreeViewNodeModelBase> _children;

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

        /// <summary>
        /// ID set when numbering the tree
        /// </summary>
        public int ItemNumber { get; private set; }

        /// <summary>
        /// Check to see if the tree is numbered. This should be cascade-set starting from the root.
        /// </summary>
        public bool IsNumbered { get; private set; }

        /// <summary>
        /// Key for the object (provided by inherited classes)
        /// </summary>
        public abstract object Key { get; }

        public TreeViewNodeModelBase? Parent
        {
            get { return _parent; }
            set { this.RaiseAndSetIfChanged(ref _parent, value); }
        }
        public IEnumerator Enumerator
        {
            get { return new TreeViewEnumerator(this); }
        }
        public IReadOnlyCollection<TreeViewNodeModelBase> Children
        {
            get { return _children; }
        }
        public int ChildCount
        {
            get { return _children.Count; }
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

        public TreeViewNodeModelBase(int recursionDepth, TreeViewNodeModelBase? parent)
        {
            _children = new KeyedObservableCollection<object, TreeViewNodeModelBase>();
            _parent = parent;

            _updating = false;

            // The ItemNumber is set when calling "SetTreeNumbering". The IsNumbered flag is set
            // once the user calls "SetTreeNumbering" from the root.
            this.ItemNumber = 0;
            this.IsNumbered = false;

            OnPropertyChanged(nameof(ChildCount));
        }

        // Method used for recursive members (includes current node for action)
        private void Recurse(Func<TreeViewNodeModelBase, IteratorContinuation> userFunc, bool leafFirst = false, bool childrenOnly = false)
        {
            if (!leafFirst && !childrenOnly)
            {
                if (userFunc(this) == IteratorContinuation.BreakAndReturn)
                    return;
            }

            // Recursive Iterator
            foreach (TreeViewNodeModelBase item in _children)
            {
                item.Recurse(userFunc);
            }

            if (leafFirst && !childrenOnly)
            {
                if (userFunc(this) == IteratorContinuation.BreakAndReturn)
                    return;
            }
        }
        private void Recurse<T>(Func<T, IteratorContinuation> userFunc, bool leafFirst = false, bool childrenOnly = false) where T : TreeViewNodeModelBase
        {
            if (this is not T)
                throw new ArgumentException("Invalid cast of tree node");

            if (!leafFirst && !childrenOnly)
            {
                if (userFunc(this as T) == IteratorContinuation.BreakAndReturn)
                    return;
            }

            // Recursive Iterator
            foreach (TreeViewNodeModelBase item in _children)
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
        public void RecurseForEach(Action<TreeViewNodeModelBase> action)
        {
            this.RecurseForEach<TreeViewNodeModelBase>(action);
        }

        /// <summary>
        /// (Casted) Recursively iterates the collection. This method must not overlap with IEnumerable due to framework
        /// usage. e.g. is the HierarchicalDataTemplate - which will then treat the tree as a flat list.
        /// </summary>
        public void RecurseForEach<T>(Action<T> action) where T : TreeViewNodeModelBase
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
        public int RecursiveCount(Func<TreeViewNodeModelBase, bool> predicate)
        {
            return this.RecursiveCount<TreeViewNodeModelBase>(predicate);
        }
        public int RecursiveCount<T>(Func<T, bool> predicate) where T : TreeViewNodeModelBase
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
        public IEnumerable<TreeViewNodeModelBase> RecursiveWhere(Func<TreeViewNodeModelBase, bool> predicate)
        {
            return this.RecursiveWhere<TreeViewNodeModelBase>(predicate);
        }
        public IEnumerable<T> RecursiveWhere<T>(Func<T, bool> predicate) where T : TreeViewNodeModelBase
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

        public bool RecursiveAll(Func<TreeViewNodeModelBase, bool> predicate)
        {
            return this.RecursiveAll<TreeViewNodeModelBase>(predicate);
        }
        public bool RecursiveAll<T>(Func<T, bool> predicate) where T : TreeViewNodeModelBase
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
        public bool RecursiveAny(Func<TreeViewNodeModelBase, bool> predicate)
        {
            return this.RecursiveAny<TreeViewNodeModelBase>(predicate);
        }
        public bool RecursiveAny<T>(Func<T, bool> predicate) where T : TreeViewNodeModelBase
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
        public TreeViewNodeModelBase? RecursiveFirst(Func<TreeViewNodeModelBase, bool> predicate)
        {
            return this.RecursiveFirst<TreeViewNodeModelBase>(predicate);
        }
        public T? RecursiveFirst<T>(Func<T, bool> predicate) where T : TreeViewNodeModelBase
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

        public bool RecursiveContains(TreeViewNodeModelBase item)
        {
            return RecursiveAny(node =>
            {
                // Performance:  1) Check recursion depth, 2) Use key comparison
                //
                if (node._children.ContainsKey(item.Key))
                    return true;

                else
                    return false;
            });
        }
        public bool HasDirectAncestor(TreeViewNodeModelBase subTree)
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
        public TreeViewNodeModelBase Add(TreeViewNodeModelBase item)
        {
            if (item == null)
                throw new NullReferenceException("Trying to insert null value into recursive tree view model");

            item.PropertyChanged += OnItemPropertyChanged;

            _children.Add(item.Key, item);

            OnPropertyChanged(nameof(ChildCount));

            return item;
        }

        /// <summary>
        /// Re-numbers tree and sets flag used during multi-select. The counter is used to handle multiple branches.
        /// </summary>
        public void SetTreeNumbering(ref int counter)
        {
            if (_parent != null)
                throw new Exception("Must call SetTreeNumbering at the root of the tree only");

            // Procedure:  The numbering should be depth-first down the tree
            //
            // 1) Reset the counter (ONLY FOR THE CONTAINER)
            // 2) -> Recurse Downward
            //       - Set ItemNumber
            //       - Set IsNumbered = true
            //

            this.IsNumbered = true;
            this.ItemNumber = ++counter;

            SetTreeNumberingRecurse(this, ref counter);
        }

        private void SetTreeNumberingRecurse(TreeViewNodeModelBase treeNode, ref int counter)
        {
            foreach (var item in _children.Values)
            {
                // Re-Number
                item.ItemNumber = ++counter;
                item.IsNumbered = true;

                // -> Recurse (Depth First)
                item.SetTreeNumberingRecurse(item, ref counter);
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
            foreach (TreeViewNodeModelBase node in _children)
            {
                node.PropertyChanged -= OnItemPropertyChanged;
            }

            _children.Clear();

            OnPropertyChanged(nameof(ChildCount));
        }

        /// <summary>
        /// (Recursive Method) Checks tree (from this depth downward) for the item
        /// </summary>
        public bool Contains(TreeViewNodeModelBase item)
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
        public bool Remove(TreeViewNodeModelBase item)
        {
            if (!_children.Contains(item))
                throw new ArgumentException("Item not contained within tree view children");

            var result = _children.Remove(item);

            OnPropertyChanged(nameof(ChildCount));

            return result;
        }

        #endregion
    }
}
