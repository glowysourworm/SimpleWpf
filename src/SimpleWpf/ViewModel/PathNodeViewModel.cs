namespace SimpleWpf.ViewModel
{
    public class PathNodeViewModel : RecursiveNodeViewModel<PathViewModel>
    {
        private readonly string _searchPattern;

        public PathNodeViewModel(string searchPattern,
                                      PathViewModel nodeValue,
                                      RecursiveNodeViewModel<PathViewModel> parent = null)
            : base(nodeValue, parent)
        {
            _searchPattern = searchPattern;
        }

        protected override RecursiveNodeViewModel<PathViewModel> Construct(PathViewModel nodeValue)
        {
            return new PathNodeViewModel(_searchPattern, nodeValue, this);
        }
    }
}
