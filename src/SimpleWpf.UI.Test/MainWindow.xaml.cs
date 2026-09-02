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
        ObservableCollection<TreeViewModel> _selectedItems;

        public EnumTestViewModel EnumTest;
        public TreeViewModel TreeViewModel;

        public MainWindow()
        {
            InitializeComponent();

            this.TreeViewModel = new TreeViewModel(new TreeViewNodeModel(0)
            {
                DisplayName = "Root"
            });

            for (int index = 0; index < 10; index++)
            {
                var item = this.TreeViewModel.Add(new TreeViewNodeModel(1)
                {
                    DisplayName = "Item " + index
                });

                for (int childIndex = 0; childIndex < 10; childIndex++)
                {
                    var child = item.Add(new TreeViewNodeModel(2)
                    {
                        DisplayName = "Child " + childIndex
                    });

                    for (int grandChildIndex = 0; grandChildIndex < 10; grandChildIndex++)
                    {
                        var grandChild = child.Add(new TreeViewNodeModel(3)
                        {
                            DisplayName = "Grand Child (CanHaveChildren = false) " + grandChildIndex,
                            CanHaveChildren = false
                        });

                        var grandChildTree = child.Add(new TreeViewNodeModel(3)
                        {
                            DisplayName = "Grand Child (CanHaveChildren = true) " + grandChildIndex,
                            CanHaveChildren = true
                        });
                    }
                }
            }

            _selectedItems = new ObservableCollection<TreeViewModel>();

            this.SelectedItemsLB.ItemsSource = _selectedItems;
            this.TheTreeView.SelectedItemsChanged += TheTreeView_SelectedItemsChanged;

            this.EnumTest = new EnumTestViewModel();
            this.SimpleEnumCB.DataContext = this.EnumTest;
            this.SimpleEnumFC.DataContext = this.EnumTest;
            this.SimpleEnumRB.DataContext = this.EnumTest;

            this.TheTreeView.ItemsSource = this.TreeViewModel;
        }

        private void TheTreeView_SelectedItemsChanged(SimpleTreeView treeView, IEnumerable<TreeViewModel> selectedItems)
        {
            _selectedItems.Clear();
            _selectedItems.AddRange(selectedItems);
        }
    }
}