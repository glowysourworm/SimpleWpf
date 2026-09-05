using SimpleWpf.UI.Command;
using SimpleWpf.UI.ViewModel.TreeView.Interface;

namespace SimpleWpf.UI.ViewModel.TreeView
{
    public abstract class TreeViewNodeModelBase : ViewModelBase, ITreeViewNode
    {
        bool _isLoaded;
        bool _isExpanded;
        bool _isSelected;

        bool _canHaveChildren;

        int _recursionDepth;

        string _displayName;

        SimpleCommand _toggleExpansionCommand;
        SimpleCommand _toggleSelectionCommand;

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
        public bool CanHaveChildren
        {
            get { return _canHaveChildren; }
            set { this.RaiseAndSetIfChanged(ref _canHaveChildren, value); }
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
        public SimpleCommand ToggleSelectionCommand
        {
            get { return _toggleSelectionCommand; }
            set { this.RaiseAndSetIfChanged(ref _toggleSelectionCommand, value); }
        }


        public TreeViewNodeModelBase(string displayName, int recursionDepth)
        {
            this.DisplayName = displayName;
            this.RecursionDepth = recursionDepth;
            this.CanHaveChildren = true;

            this.ToggleExpansionCommand = new SimpleCommand(() =>
            {
                this.IsExpanded = !this.IsExpanded;
            });
            this.ToggleSelectionCommand = new SimpleCommand(() =>
            {
                this.IsSelected = !this.IsSelected;
            });
        }
    }
}
