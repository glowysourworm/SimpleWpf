using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using SimpleWpf.Extensions.Event;
using SimpleWpf.UI.ViewModel.TreeView;

using Xceed.Wpf.Toolkit.Core.Utilities;

namespace SimpleWpf.UI.Controls.TreeViewUI
{
    public partial class SimpleTreeView : UserControl
    {
        #region (public) Dependency Properties
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(SimpleTreeView), new PropertyMetadata(OnItemsSourceChanged));

        public static readonly DependencyProperty ItemExpanderClosedTemplateProperty =
            DependencyProperty.Register("ItemExpanderClosedTemplate", typeof(DataTemplate), typeof(SimpleTreeView));

        public static readonly DependencyProperty ItemExpanderOpenTemplateProperty =
            DependencyProperty.Register("ItemExpanderOpenTemplate", typeof(DataTemplate), typeof(SimpleTreeView));

        public static readonly DependencyProperty ItemHeaderContentTemplateProperty =
            DependencyProperty.Register("ItemHeaderContentTemplate", typeof(DataTemplate), typeof(SimpleTreeView));

        public static readonly DependencyProperty ItemIndentProperty =
            DependencyProperty.Register("ItemIndent", typeof(int), typeof(SimpleTreeView), new PropertyMetadata(0));

        public static readonly DependencyProperty ItemPaddingProperty =
            DependencyProperty.Register("ItemPadding", typeof(Thickness), typeof(SimpleTreeView));

        public static readonly DependencyProperty ItemBackgroundProperty =
            DependencyProperty.Register("ItemBackground", typeof(Brush), typeof(SimpleTreeView));

        public static readonly DependencyProperty ItemBackgroundAlternationProperty =
            DependencyProperty.Register("ItemBackgroundAlternation", typeof(Brush), typeof(SimpleTreeView));

        public static readonly DependencyProperty ItemHoverBrushProperty =
            DependencyProperty.Register("ItemHoverBrush", typeof(Brush), typeof(SimpleTreeView));

        public static readonly DependencyProperty ItemSelectionBrushProperty =
            DependencyProperty.Register("ItemSelectionBrush", typeof(Brush), typeof(SimpleTreeView));

        public static readonly DependencyProperty ItemBorderProperty =
            DependencyProperty.Register("ItemBorder", typeof(Brush), typeof(SimpleTreeView));

        public static readonly DependencyProperty ItemBorderThicknessProperty =
            DependencyProperty.Register("ItemBorderThickness", typeof(Thickness), typeof(SimpleTreeView));

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        public int ItemIndent
        {
            get { return (int)GetValue(ItemIndentProperty); }
            set { SetValue(ItemIndentProperty, value); }
        }
        public DataTemplate ItemExpanderClosedTemplate
        {
            get { return (DataTemplate)GetValue(ItemExpanderClosedTemplateProperty); }
            set { SetValue(ItemExpanderClosedTemplateProperty, value); }
        }
        public DataTemplate ItemExpanderOpenTemplate
        {
            get { return (DataTemplate)GetValue(ItemExpanderOpenTemplateProperty); }
            set { SetValue(ItemExpanderOpenTemplateProperty, value); }
        }
        public DataTemplate ItemHeaderContentTemplate
        {
            get { return (DataTemplate)GetValue(ItemHeaderContentTemplateProperty); }
            set { SetValue(ItemHeaderContentTemplateProperty, value); }
        }
        public Thickness ItemPadding
        {
            get { return (Thickness)GetValue(ItemPaddingProperty); }
            set { SetValue(ItemPaddingProperty, value); }
        }
        public Brush ItemBackground
        {
            get { return (Brush)GetValue(ItemBackgroundProperty); }
            set { SetValue(ItemBackgroundProperty, value); }
        }
        public Brush ItemBackgroundAlternation
        {
            get { return (Brush)GetValue(ItemBackgroundAlternationProperty); }
            set { SetValue(ItemBackgroundAlternationProperty, value); }
        }
        public Brush ItemHoverBrush
        {
            get { return (Brush)GetValue(ItemHoverBrushProperty); }
            set { SetValue(ItemHoverBrushProperty, value); }
        }
        public Brush ItemSelectionBrush
        {
            get { return (Brush)GetValue(ItemSelectionBrushProperty); }
            set { SetValue(ItemSelectionBrushProperty, value); }
        }
        public Brush ItemBorder
        {
            get { return (Brush)GetValue(ItemBorderProperty); }
            set { SetValue(ItemBorderProperty, value); }
        }
        public Thickness ItemBorderThickness
        {
            get { return (Thickness)GetValue(ItemBorderThicknessProperty); }
            set { SetValue(ItemBorderThicknessProperty, value); }
        }
        #endregion

        /// <summary>
        /// Event that occurs when a tree item is expanded or collapsed. The first argument is the sender (bound item). The
        /// second is the current expanded state.
        /// </summary>
        public event SimpleEventHandler<object, bool> ItemExpandedEvent;

        /// <summary>
        /// Event that occurs when the selection in the treeview has changed
        /// </summary>
        public event SimpleEventHandler<SimpleTreeView, IEnumerable<TreeViewNodeModelBase>> SelectedItemsChanged;

        // Private collections
        private Dictionary<TreeViewNodeModelBase, TreeViewNodeModelBase> _selectedItems;

        // Node Selection
        TreeViewNodeModelBase? _selectedNode;

        public SimpleTreeView()
        {
            InitializeComponent();

            _selectedItems = new Dictionary<TreeViewNodeModelBase, TreeViewNodeModelBase>();
        }

        // Occurs when a property on the UI (target) side changes
        private void HandleTreeSelection(SimpleTreeViewModel tree, TreeViewNodeModelBase nodeClicked)
        {
            var ctrl = Keyboard.Modifiers == ModifierKeys.Control;
            var shift = Keyboard.Modifiers == ModifierKeys.Shift;

            // Ordinary Selection:  (options) Single Select; Recursive Select (down the tree)
            if (!ctrl && !shift)
            {
                // Single Select
                //
                // 1) Tree becomes de-selected
                // 2) The selected node gets toggled
                //

                // Save Selection
                var isSelected = nodeClicked.IsSelected;

                // De-Select
                tree.RecursiveForEach(treeNode =>
                {
                    treeNode.IsSelected = false;

                    // Update Selection List
                    UpdateSelection(treeNode);
                });

                // Set Selection
                nodeClicked.IsSelected = !isSelected;

                // Set Follower
                _selectedNode = nodeClicked.IsSelected ? nodeClicked : null;

                // Update Selection List
                UpdateSelection(nodeClicked);
            }

            // Ctrl + Select:  User Single Select 
            else if (ctrl)
            {
                // User Single Select
                //
                // 1) Clicked node gets toggled
                //

                nodeClicked.IsSelected = !nodeClicked.IsSelected;

                // Set Follower
                _selectedNode = nodeClicked.IsSelected ? nodeClicked : null;

                // Update Selection List
                UpdateSelection(nodeClicked);
            }

            // Shift + Select:  Multiple Selection
            else if (shift)
            {
                // Multiple Selection:  We have a trick for handling this. The nodes are first
                //                      numbered going down the tree. Then, the item id's can
                //                      be used to do contiguous select.
                //

                // Preivious Selected Node
                if (_selectedNode != null)
                {
                    // First, check to see that tree has been numbered
                    if (!tree.IsNumberingSet())
                        throw new Exception("Must set tree numbering before using multi-selection");

                    // Get two item numbers
                    var numberLow = Math.Min(_selectedNode.ItemId, nodeClicked.ItemId);
                    var numberHigh = Math.Max(_selectedNode.ItemId, nodeClicked.ItemId);

                    tree.RecursiveForEach(treeNode =>
                    {
                        if (treeNode.ItemId >= numberLow &&
                            treeNode.ItemId <= numberHigh)
                        {
                            treeNode.IsSelected = true;
                        }
                        else
                            treeNode.IsSelected = false;

                        // Update Selection List
                        UpdateSelection(treeNode);
                    });
                }

                // Single Select
                else
                {
                    nodeClicked.IsSelected = !nodeClicked.IsSelected;

                    // Set Follower
                    _selectedNode = nodeClicked.IsSelected ? nodeClicked : null;

                    // Update Selection List
                    UpdateSelection(nodeClicked);
                }
            }

            // Selected Items Changed
            if (this.SelectedItemsChanged != null)
                this.SelectedItemsChanged(this, _selectedItems.Values);

            // UI-Direct:  This is another way to get the update through the view model
            //
            tree.SetSelection(_selectedItems.Values);
        }

        private void UpdateSelection(TreeViewNodeModelBase treeNode)
        {
            // Selection Changed
            if (treeNode.IsSelected && !_selectedItems.ContainsKey(treeNode))
                _selectedItems.Add(treeNode, treeNode);

            if (!treeNode.IsSelected && _selectedItems.ContainsKey(treeNode))
                _selectedItems.Remove(treeNode);
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e.OriginalSource is not DependencyObject)
                base.OnPreviewMouseLeftButtonDown(e);

            else
            {
                // Procedure: Prefer expander to selection first
                //
                // 1) Get TreeViewItem from the event data
                // 2) Search for the ContentSource="Header" (ContentPresenter)
                // 3) Check the header first for expander click
                //

                // Get topmost item
                var treeViewItem = VisualTreeHelperEx.FindAncestorByType<TreeViewItem>(e.OriginalSource as DependencyObject);

                if (treeViewItem == null)
                {
                    base.OnPreviewMouseLeftButtonDown(e);
                    return;
                }

                // Get our data context
                var viewModel = treeViewItem.DataContext as TreeViewNodeModelBase;

                if (viewModel == null)
                {
                    base.OnPreviewMouseLeftButtonDown(e);
                    return;
                }

                // Get input element
                var inputElement = this.InputHitTest(e.GetPosition(this));

                // Look up the tree to get the content presenter for the expander
                var header = VisualTreeHelperEx.FindAncestorByType<ContentPresenter>(inputElement as DependencyObject);

                // Templated Parent will be the ContentPresenter that holds the template for the expander
                var headerParent = header.TemplatedParent as ContentPresenter;

                // Expansion
                if (headerParent != null && headerParent.ContentSource == "Header")
                {
                    viewModel.IsExpanded = !viewModel.IsExpanded;
                    e.Handled = true;
                    return;
                }

                // Selection (finally)
                HandleTreeSelection(this.ItemsSource as SimpleTreeViewModel, viewModel);

                e.Handled = true;
            }
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SimpleTreeView;
            var source = e.NewValue as SimpleTreeViewModel;

            if (control != null &&
                source != null)
            {
                // This will circumvent binding issues:  it is simple to raise a property changed event
                // from the user code - which is what usually happens. So, any property changed event
                // that fires from the SimpleTreeViewModel class hierarchy will get caught here without
                // worrying about binding settings, templates, etc...
                //
                control.TheTreeView.ItemsSource = source;
            }
        }
    }
}
