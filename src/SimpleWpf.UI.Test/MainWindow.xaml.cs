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
        public TreeViewModel TreeViewModel;

        public MainWindow()
        {
            InitializeComponent();

            this.TreeViewModel = new TreeViewModel(new TreeViewNodeModel("Root", 0));

            for (int index = 0; index < 10; index++)
            {
                var item = this.TreeViewModel.Add(new TreeViewNodeModel("Item " + index, 1));

                for (int childIndex = 0; childIndex < 10; childIndex++)
                {
                    var child = item.Add(new TreeViewNodeModel("Child " + childIndex, 2));

                    for (int grandChildIndex = 0; grandChildIndex < 10; grandChildIndex++)
                    {
                        var grandChild = child.Add(new TreeViewNodeModel("Grand Child (CanHaveChildren = false) " + grandChildIndex, 3));

                        var grandChildTree = child.Add(new TreeViewNodeModel("Grand Child (CanHaveChildren = true) " + grandChildIndex, 3)
                        {
                            CanHaveChildren = true
                        });
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

            this.TheTreeView.ItemsSource = this.TreeViewModel;
        }

        private void TheTreeView_SelectedItemsChanged(SimpleTreeView sender, IEnumerable<TreeViewModelBase> selectedItems)
        {
            _selectedItems.Clear();
            _selectedItems.AddRange(selectedItems);
        }
    }
}