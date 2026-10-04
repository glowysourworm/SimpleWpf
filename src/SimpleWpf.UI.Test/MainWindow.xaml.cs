using System.Collections.ObjectModel;
using System.Windows;

using SimpleWpf.Extensions.ObservableCollection;
using SimpleWpf.UI.Controls.TreeViewUI;
using SimpleWpf.UI.Test.EnumUI;
using SimpleWpf.UI.ViewModel.TreeView;

namespace SimpleWpf.UI.Test
{
    public partial class MainWindow : Window
    {
        ObservableCollection<TreeViewModelBase> _selectedItems;

        public EnumTestViewModel EnumTest;
        public SimpleTreeViewModel TreeView;

        public MainWindow()
        {
            InitializeComponent();

            this.TreeView = new SimpleTreeViewModel();
            this.TreeView.BeginUpdate();

            // Use SimpleTreeViewModel to add nodes (or) can also just chain nodes together to the already added one(s)
            var root = this.TreeView.Add(new TreeViewModel("Root", 0, null));

            for (int index = 0; index < 10; index++)
            {
                // SimpleTreeViewModel.Add
                var item = this.TreeView.Add(new TreeViewModel("Item " + index, 1, root));

                for (int childIndex = 0; childIndex < 10; childIndex++)
                {
                    var child = new TreeViewModel("Child " + childIndex, 2, item);

                    // TreeViewModelBase.Add (node add)
                    item.Add(child);

                    for (int grandChildIndex = 0; grandChildIndex < 10; grandChildIndex++)
                    {
                        var grandChild = new TreeViewModel("Grand Child (CanHaveChildren = false) " + grandChildIndex, 3, child);
                        child.Add(grandChild);
                    }
                }
            }

            // Finalize Add (sets tree node numbering)
            this.TreeView.EndUpdate();

            _selectedItems = new ObservableCollection<TreeViewModelBase>();

            this.SelectedItemsLB.ItemsSource = _selectedItems;
            this.TheTreeView.SelectedItemsChanged += TheTreeView_SelectedItemsChanged;

            this.EnumTest = new EnumTestViewModel();
            this.SimpleEnumCB.DataContext = this.EnumTest;
            this.SimpleEnumFC.DataContext = this.EnumTest;
            this.SimpleEnumRB.DataContext = this.EnumTest;

            this.TheTreeView.ItemsSource = this.TreeView;
        }

        private void TheTreeView_SelectedItemsChanged(SimpleTreeView sender, IEnumerable<TreeViewModelBase> selectedItems)
        {
            _selectedItems.Clear();
            _selectedItems.AddRange(selectedItems);
        }
    }
}