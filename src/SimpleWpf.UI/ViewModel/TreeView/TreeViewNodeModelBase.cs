using SimpleWpf.UI.Command;

namespace SimpleWpf.UI.ViewModel.TreeView
{
    public abstract class TreeViewNodeModelBase : ViewModelBase
    {
        bool _isLoaded;
        bool _isExpanded;
        bool _isSelected;

        int _recursionDepth;

        string _displayName;

        SimpleCommand _toggleExpansionCommand;

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
        public string DisplayName
        {
            get { return _displayName; }
            set { this.RaiseAndSetIfChanged(ref _displayName, value); }
        }

        public SimpleCommand ToggleExpansionCommand
        {
            get { return _toggleExpansionCommand; }
            set { this.RaiseAndSetIfChanged(ref _toggleExpansionCommand, value); }
        }


        public TreeViewNodeModelBase(int recursionDepth)
        {
            this.DisplayName = string.Empty;
            this.RecursionDepth = recursionDepth;

            this.ToggleExpansionCommand = new SimpleCommand(() =>
            {
                this.IsExpanded = !this.IsExpanded;
            });
        }
    }
}
