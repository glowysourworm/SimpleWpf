using System.Windows.Navigation;

namespace SimpleWpf.ViewModel
{
    public class PathNodeViewModel : RecursiveDispatcherViewModel<PathViewModel>
    {
        private readonly string _searchPattern;

        public PathNodeViewModel(string searchPattern,
                                      PathViewModel nodeValue,
                                      RecursiveDispatcherViewModel<PathViewModel> parent = null)
            : base(nodeValue, parent)
        {
            _searchPattern = searchPattern;
        }

        protected override RecursiveDispatcherViewModel<PathViewModel> Construct(PathViewModel nodeValue)
        {
            return new PathNodeViewModel(_searchPattern, nodeValue, this);
        }

        public override string ToString()
        {
            return this.NodeValue.ToString();
        }
    }
}
