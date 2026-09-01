using System.Collections;

namespace SimpleWpf.UI.ViewModel.TreeView
{
    internal class TreeViewEnumerator : IEnumerator
    {
        private object _treeView;

        public object Current
        {
            get { return _treeView; }
        }

        // There is only one child in the "Tree". Each of the child items will be 
        // part of enumerating the child list. So, this should be an internal class.
        int _currentItem;

        internal TreeViewEnumerator(object treeView)
        {
            _treeView = treeView;
        }

        public bool MoveNext()
        {
            return _currentItem++ < 1;
        }

        public void Reset()
        {
            _currentItem = 0;
        }
    }
}
