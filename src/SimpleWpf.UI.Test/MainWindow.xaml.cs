using System.Collections.ObjectModel;
using System.Windows;

using SimpleWpf.Extensions.ObservableCollection;
using SimpleWpf.UI.Controls.TreeViewUI;
using SimpleWpf.UI.ViewModel.TreeView;

namespace SimpleWpf.UI.Test
{
    public partial class MainWindow : Window
    {
        ObservableCollection<TreeViewModel> _selectedItems;

        public MainWindow()
        {
            InitializeComponent();

            var treeView = new TreeViewModel(new TreeViewNodeModel(0)
            {
                DisplayName = "Root"
            });

            for (int index = 0; index < 10; index++)
            {
                var item = treeView.Add(new TreeViewNodeModel(1)
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

            this.DataContext = treeView;
        }

        private void TheTreeView_SelectedItemsChanged(SimpleTreeView treeView, IEnumerable<TreeViewModel> selectedItems)
        {
            _selectedItems.Clear();
            _selectedItems.AddRange(selectedItems);
        }
    }
}