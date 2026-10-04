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
        public TreeViewModel TreeView;

        public MainWindow()
        {
            InitializeComponent();

            this.TreeView = new TreeViewModel("Root", 0, null);

            for (int index = 0; index < 10; index++)
            {
                var item = new TreeViewModel("Item " + index, 1, this.TreeView);
                this.TreeView.Add(item);

                for (int childIndex = 0; childIndex < 10; childIndex++)
                {
                    var child = new TreeViewModel("Child " + childIndex, 2, item);
                    item.Add(child);

                    for (int grandChildIndex = 0; grandChildIndex < 10; grandChildIndex++)
                    {
                        var grandChild = new TreeViewModel("Grand Child (CanHaveChildren = false) " + grandChildIndex, 3, child);
                        child.Add(grandChild);


                        var grandChildTree = new TreeViewModel("Grand Child (CanHaveChildren = true) " + grandChildIndex, 3, child)
                        {
                            CanHaveChildren = true
                        };

                        child.Add(grandChildTree);
                    }
                }
            }

            _selectedItems = new ObservableCollection<TreeViewModelBase>();

            this.SelectedItemsLB.ItemsSource = _selectedItems;
            this.TheTreeView.SelectedItemsChanged += TheTreeView_SelectedItemsChanged;

            this.EnumTest = new EnumTestViewModel();
            this.SimpleEnumCB.DataContext = this.EnumTest;
            this.SimpleEnumFC.DataContext = this.EnumTest;
            this.SimpleEnumRB.DataContext = this.EnumTest;

            this.TheTreeView.ItemsSource = this.TreeView;

            // Multi-Selection
            this.TreeView.SetTreeNumbering();
        }

        private void TheTreeView_SelectedItemsChanged(SimpleTreeView sender, IEnumerable<TreeViewModelBase> selectedItems)
        {
            _selectedItems.Clear();
            _selectedItems.AddRange(selectedItems);
        }
    }
}