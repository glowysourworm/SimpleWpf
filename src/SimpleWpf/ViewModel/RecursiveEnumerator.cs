using System.Collections;

namespace SimpleWpf.ViewModel
{
    internal class RecursiveEnumerator<T> : IEnumerator<T> where T : RecursiveViewModelBase
    {
        List<RecursiveNodeViewModel<T>> _flattenedList;

        int _currentNodeIndex;

        public T Current
        {
            get { return _flattenedList[_currentNodeIndex].NodeValue; }
        }
        object IEnumerator.Current
        {
            get { return _flattenedList[_currentNodeIndex].NodeValue; }
        }

        public RecursiveEnumerator(RecursiveNodeViewModel<T> root)
        {
            _currentNodeIndex = -1;
            _flattenedList = new List<RecursiveNodeViewModel<T>>();

            Recurse(root, x => _flattenedList.Add(x));
        }

        private void Recurse(RecursiveNodeViewModel<T> node, Action<RecursiveNodeViewModel<T>> action)
        {
            action(node);

            // IEnumerable
            foreach (var child in node.Children)
            {
                Recurse(child, action);
            }
        }

        public void Dispose()
        {
            _flattenedList.Clear();
            _flattenedList = null;
        }

        public bool MoveNext()
        {
            if (_flattenedList == null)
                throw new Exception("RecursiveEnumerator has been disposed.");

            if (_currentNodeIndex + 1 < _flattenedList.Count)
            {
                _currentNodeIndex++;
                return true;
            }

            return false;
        }

        public void Reset()
        {
            _currentNodeIndex = -1;
        }
    }
}
