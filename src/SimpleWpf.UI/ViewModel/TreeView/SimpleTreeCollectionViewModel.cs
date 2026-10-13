using System.Collections;
using System.Collections.Specialized;

using SimpleWpf.Extensions.ObservableCollection;
using SimpleWpf.UI.ViewModel.TreeView.Interface;

namespace SimpleWpf.UI.ViewModel.TreeView
{
    public class SimpleTreeCollectionViewModel : ViewModelBase, ISimpleTreeCollection, INotifyCollectionChanged
    {
        // Recursion depth for the root nodes
        const int ROOT_DEPTH = 0;

        // Nodes by Recursion Depth
        KeyedObservableCollection<int, KeyedObservableCollection<object, TreeViewNodeModelBase>> _depthNodes;

        int _minRecursionDepth;
        int _maxRecursionDepth;

        /// <summary>
        /// Collection changed event for the root nodes (tree nodes are automatically bound to the tree); and
        /// there are tree-wide item changed events that bubble up to the roots.
        /// </summary>
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public int Count
        {
            get { return _count; }
            private set { this.RaiseAndSetIfChanged(ref _count, value); }
        }
        public int MaxDepth
        {
            get { return _maxRecursionDepth; }
        }

        /// <summary>
        /// Returns node collection at specified depth:  NOTE*** THESE ARE FOR READ ONLY PURPOSES!
        /// </summary>
        /// <param name="depthIndex">Depth of recursion</param>
        /// <returns>A keyed observable collection for readonly purposes</returns>
        protected KeyedObservableCollection<object, TreeViewNodeModelBase> GetDepthNodes(int depthIndex)
        {
            return _depthNodes[depthIndex];
        }

        // Branch Strategy
        SimpleTreeViewBranchingStrategy _branchStrategy;

        // Begin / End Update Pattern
        bool _updating;
        bool _invalid;
        int _count;

        public SimpleTreeCollectionViewModel(SimpleTreeViewBranchingStrategy branchStrategy)
        {
            _branchStrategy = branchStrategy;
            _minRecursionDepth = 0;
            _maxRecursionDepth = 0;
            _depthNodes = new KeyedObservableCollection<int, KeyedObservableCollection<object, TreeViewNodeModelBase>>();
        }

        public void BeginUpdate()
        {
            // Must be allowed to call this to add nodes

            _updating = true;
        }
        public void EndUpdate()
        {
            if (!_updating)
                throw new Exception("Trying to end update before calling BeginUpdate");

            var treeCounter = 0;

            // Set Numbering (multi-selection)
            foreach (var rootNode in GetSafeRootCollection())
                rootNode.SetTreeNumbering(ref treeCounter);    // Set node numbers (ItemNumber)

            // Set Invalid Flag (this is for tree multi-selection)
            _invalid = SimpleTreeAny<TreeViewNodeModelBase>(x => !x.IsNumbered);

            // Set Count
            _count = treeCounter;

            // Complete update
            _updating = false;

            OnPropertyChanged(nameof(Count));
            OnCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        public bool IsUpdating()
        {
            return _updating;
        }
        public bool IsInvalid()
        {
            return _invalid;
        }

        /// <summary>
        /// Returns branch of the tree by tracing upward starting from the provided node. The
        /// nodes are ordered forwards - starting from the root.
        /// </summary>
        public IEnumerable<TreeViewNodeModelBase> GetBranch(TreeViewNodeModelBase node, bool includeDescendants = false)
        {
            if (node == null)
                throw new ArgumentNullException("Node not set to an instance of the node class");

            var stack = new Stack<TreeViewNodeModelBase>();
            var result = new List<TreeViewNodeModelBase>();
            var descendants = new List<TreeViewNodeModelBase>();
            var currentNode = node;

            // Include Descendants
            if (includeDescendants)
            {
                currentNode.RecurseForEach(descendant =>
                {
                    if (descendant == currentNode)
                        return;

                    descendants.Add(descendant);
                });
            }

            do
            {
                // Stack these up from the leaf-er node
                stack.Push(currentNode);

                // Trace upwards towards the root
                currentNode = currentNode.Parent;

            } while (currentNode != null);

            // Arrange these starting with the root
            while (stack.Any())
            {
                result.Add(stack.Pop());
            }

            // Descendants
            if (includeDescendants)
                result.AddRange(descendants);

            return result;
        }

        public void SimpleTreeAdd(TreeViewNodeModelBase node)
        {
            if (!_updating)
                throw new Exception("Must first call BeginUpdate before adding tree nodes");

            if (SimpleTreeContains(node))
                throw new Exception("Node already contained in the tree");

            if (node.Parent != null && !SimpleTreeContains(node.Parent))
                throw new Exception("Parent node not contained in the tree");

            // Procedure:  Add
            // 
            // 0) Check node integrity with the tree
            // 1) Check Parent
            //      -> If (Null), Add to root nodes
            //      -> If (Not in Tree), throw exception
            //      -> If (Child not in collection), throw exception
            //
            // 2) Add to depth node collection
            // 3) Set Invalid
            //

            // Check integrity of the parent:  (options) 1) throw exception, 2) Hook up the parent for the user (??) (option??)
            //
            if (node.Parent == null)
            {
                // Root (nothing to do)
            }
            else if (!node.Parent.Children.Contains(node))
            {
                node.Parent.Add(node);  // optional ??
            }
            else
            {
                // Nothing to do 
            }

            // Finally, keep our internal depth collection (performance!)
            if (!_depthNodes.ContainsKey(node.RecursionDepth))
                _depthNodes.Add(node.RecursionDepth, new KeyedObservableCollection<object, TreeViewNodeModelBase>());

            _depthNodes[node.RecursionDepth].Add(node.Key, node);

            // Update Recursion Depth
            _minRecursionDepth = _depthNodes.Keys.Min();
            _maxRecursionDepth = _depthNodes.Keys.Max();

            _invalid = true;
        }

        public T? SimpleTreeGetNode<T>(object key) where T : TreeViewNodeModelBase
        {
            for (int depth = _minRecursionDepth; depth <= _maxRecursionDepth; depth++)
            {
                if (_depthNodes.ContainsKey(depth) &&
                    _depthNodes[depth].ContainsKey(key))
                {
                    return (T)_depthNodes[depth][key];
                }
            }

            return null;
        }
        public T? SimpleTreeGetNode<T>(int recursionDepth, object key) where T : TreeViewNodeModelBase
        {
            if (_depthNodes.ContainsKey(recursionDepth) &&
                _depthNodes[recursionDepth].ContainsKey(key))
            {
                return (T)_depthNodes[recursionDepth][key];
            }

            return null;
        }

        /// <summary>
        /// Removes the tree view node from the tree. Must be done during an update!
        /// </summary>
        public void SimpleTreeRemove<T>(T node) where T : TreeViewNodeModelBase
        {
            if (!_updating)
                throw new Exception("Must first call BeginUpdate before removing tree nodes");

            // ~ O(1)
            else if (!SimpleTreeContains(node))
                throw new Exception("Node not contained in the tree");

            else
            {
                // Procedure:  Remove
                //
                // 0) Check Contains
                // 1) Detach Parent
                // 2) Detach Children (recursive)
                // 3) Maintain Depth Collection
                // 4) Set Invalid
                //

                if (node.Parent != null && SimpleTreeContains(node.Parent))
                {
                    // Remove this node from the children
                    node.Parent.Remove(node);
                }

                // -> (Recursive) Unhook events and clear children
                node.Clear();

                // Remove from depth collection
                _depthNodes[node.RecursionDepth].Remove(node.Key);

                // Update Recursion Depth
                _minRecursionDepth = _depthNodes.Keys.Min();
                _maxRecursionDepth = _depthNodes.Keys.Max();

                // This gets reset once the nodes are added
                _invalid = true;
                _count = 0;
            }
        }
        public IEnumerator GetEnumerator()
        {
            return GetSafeRootCollection().GetEnumerator();
        }
        public bool SimpleTreeContains<T>(T node) where T : TreeViewNodeModelBase
        {
            // Recursion Depth
            if (!_depthNodes.ContainsKey(node.RecursionDepth))
                return false;

            var nodesAtDepth = _depthNodes[node.RecursionDepth];

            return nodesAtDepth.ContainsKey(node.Key);
        }
        public bool SimpleTreeAll<T>(Func<T, bool> predicate) where T : TreeViewNodeModelBase
        {
            foreach (var rootNode in GetSafeRootCollection())
            {
                if (rootNode.RecursiveAll(predicate))
                    return true;
            }

            return false;
        }
        public bool SimpleTreeAny<T>(Func<T, bool> predicate) where T : TreeViewNodeModelBase
        {
            foreach (var rootNode in GetSafeRootCollection())
            {
                if (rootNode.RecursiveAny(predicate))
                    return true;
            }

            return false;
        }
        public int SimpleTreeCount<T>(Func<T, bool> predicate) where T : TreeViewNodeModelBase
        {
            var result = 0;

            foreach (var rootNode in GetSafeRootCollection())
            {
                result += rootNode.RecursiveCount(predicate);
            }

            return result;
        }
        public T? SimpleTreeFirst<T>(Func<T, bool> predicate) where T : TreeViewNodeModelBase
        {
            T? result = null;

            foreach (var rootNode in GetSafeRootCollection())
            {
                result = rootNode.RecursiveFirst<T>(predicate);

                if (result != null)
                    break;
            }

            return result;
        }
        public IEnumerable<T> SimpleTreeWhere<T>(Func<T, bool> predicate) where T : TreeViewNodeModelBase
        {
            var result = new List<T>();

            foreach (var rootNode in GetSafeRootCollection())
            {
                result.AddRange(rootNode.RecursiveWhere(predicate));
            }

            return result;
        }
        public void SimpleTreeForEach<T>(Action<T> action) where T : TreeViewNodeModelBase
        {
            foreach (var rootNode in GetSafeRootCollection())
            {
                rootNode.RecurseForEach(action);
            }
        }
        public void SimpleTreeClear()
        {
            foreach (var rootNode in GetSafeRootCollection())
            {
                // Unhook nodes and clear children
                rootNode.Clear();
            }
        }
        private IEnumerable<TreeViewNodeModelBase> GetSafeRootCollection()
        {
            return !_depthNodes.ContainsKey(ROOT_DEPTH) ? Enumerable.Empty<TreeViewNodeModelBase>()
                                                        : _depthNodes[ROOT_DEPTH].Values;
        }
        private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // Forward this event
            if (this.CollectionChanged != null)
                this.CollectionChanged(sender, e);
        }
    }
}
