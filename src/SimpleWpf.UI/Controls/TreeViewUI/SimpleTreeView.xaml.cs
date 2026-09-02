using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

using SimpleWpf.Extensions.Event;
using SimpleWpf.UI.ViewModel.TreeView;

namespace SimpleWpf.UI.Controls.TreeViewUI
{
    public partial class SimpleTreeView : UserControl
    {
        #region (public) Dependency Properties
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(SimpleTreeView));

        public static readonly DependencyProperty ItemExpanderClosedTemplateProperty =
            DependencyProperty.Register("ItemExpanderClosedTemplate", typeof(DataTemplate), typeof(SimpleTreeView));

        public static readonly DependencyProperty ItemExpanderOpenTemplateProperty =
            DependencyProperty.Register("ItemExpanderOpenTemplate", typeof(DataTemplate), typeof(SimpleTreeView));

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

        public SimpleTreeView()
        {
            InitializeComponent();

            this.DataContextChanged += SimpleTreeView_DataContextChanged;
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            // Calculate scroll extent
            var scrollAmount = Math.Clamp(this.TheScrollViewer.VerticalOffset - e.Delta, 0, this.TheScrollViewer.ScrollableHeight);

            // Handle scroll with the viewer
            this.TheScrollViewer.ScrollToVerticalOffset(scrollAmount);

            e.Handled = true;
        }

        // Occurs when a property on the UI (target) side changes
        private void OnItemSourceItemPropertyChanged(TreeViewModelBase<TreeViewNodeModel> treeSender, TreeViewNodeModel item, PropertyChangedEventArgs eventArgs)
        {
            var viewModel = this.DataContext as TreeViewModel;

            // Selection:  De-select anything not in this item's collection (if it is selected)
            //
            if (viewModel != null && item.IsSelected && eventArgs.PropertyName == "IsSelected")
            {
                viewModel.RecurseForEach(childItem =>
                {
                    if (childItem.NodeValue.RecursionDepth != item.RecursionDepth)
                        childItem.NodeValue.IsSelected = false;
                });
            }
        }

        private void SimpleTreeView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = e.NewValue as TreeViewModel;

            if (viewModel != null)
            {
                viewModel.ItemPropertyChangedTreeEvent += OnItemSourceItemPropertyChanged;
            }
        }

        private void InputFileExpanderButton_Checked(object sender, RoutedEventArgs e)
        {
            var toggleButton = sender as ToggleButton;

            // Have to force update of the template
            if (toggleButton != null)
            {
                var selector = toggleButton.ContentTemplateSelector;

                toggleButton.ContentTemplateSelector = null;
                toggleButton.ContentTemplateSelector = selector;
            }
        }

        private void InputFileExpanderButton_Unchecked(object sender, RoutedEventArgs e)
        {
            var toggleButton = sender as ToggleButton;

            // Have to force update of the template
            if (toggleButton != null)
            {
                var selector = toggleButton.ContentTemplateSelector;

                toggleButton.ContentTemplateSelector = null;
                toggleButton.ContentTemplateSelector = selector;
            }
        }
    }
}
