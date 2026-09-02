using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

using SimpleWpf.Extensions.Collection;
using SimpleWpf.UI.ViewModel.EnumUI;

namespace SimpleWpf.UI.Controls.EnumUI
{
    public partial class SimpleEnumFlagsControl : UserControl
    {
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(SimpleEnumFlagsControl));

        public static readonly DependencyProperty HeaderFontSizeProperty =
            DependencyProperty.Register("HeaderFontSize", typeof(double), typeof(SimpleEnumFlagsControl), new PropertyMetadata(16.0D));

        public static readonly DependencyProperty EnumNameFontSizeProperty =
            DependencyProperty.Register("EnumNameFontSize", typeof(double), typeof(SimpleEnumFlagsControl), new PropertyMetadata(14.0D));

        public static readonly DependencyProperty EnumDescriptionFontSizeProperty =
            DependencyProperty.Register("EnumDescriptionFontSize", typeof(double), typeof(SimpleEnumFlagsControl), new PropertyMetadata(10.0D));

        public static readonly DependencyProperty ShowDescriptionsProperty =
            DependencyProperty.Register("ShowDescriptions", typeof(bool), typeof(SimpleEnumFlagsControl), new PropertyMetadata(true));

        public static readonly DependencyProperty EnumTypeProperty =
            DependencyProperty.Register("EnumType", typeof(Type), typeof(SimpleEnumFlagsControl));

        public static readonly DependencyProperty EnumValueProperty =
            DependencyProperty.Register("EnumValue", typeof(Enum), typeof(SimpleEnumFlagsControl), new PropertyMetadata(OnEnumValueChanged));

        public static readonly RoutedEvent EnumValueChangedEvent = EventManager.RegisterRoutedEvent(
            "EnumValueChanged",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(SimpleEnumFlagsControl));

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        public double HeaderFontSize
        {
            get { return (double)GetValue(HeaderFontSizeProperty); }
            set { SetValue(HeaderFontSizeProperty, value); }
        }
        public double EnumNameFontSize
        {
            get { return (double)GetValue(EnumNameFontSizeProperty); }
            set { SetValue(EnumNameFontSizeProperty, value); }
        }
        public double EnumDescriptionFontSize
        {
            get { return (double)GetValue(EnumDescriptionFontSizeProperty); }
            set { SetValue(EnumDescriptionFontSizeProperty, value); }
        }
        public bool ShowDescriptions
        {
            get { return (bool)GetValue(ShowDescriptionsProperty); }
            set { SetValue(ShowDescriptionsProperty, value); }
        }
        public Type EnumType
        {
            get { return (Type)GetValue(EnumTypeProperty); }
            set { SetValue(EnumTypeProperty, value); }
        }
        public object EnumValue
        {
            get { return (Enum)GetValue(EnumValueProperty); }
            set { SetValue(EnumValueProperty, value); }
        }
        public event RoutedEventHandler EnumValueChanged
        {
            add { AddHandler(EnumValueChangedEvent, value); }
            remove { RemoveHandler(EnumValueChangedEvent, value); }
        }

        bool _initializing = false;

        public SimpleEnumFlagsControl()
        {
            InitializeComponent();

            this.EnumItemsControl.Loaded += (sender, e) =>
            {
                UpdateItemsSource();
            };
        }

        protected void UpdateItemsSource()
        {
            _initializing = true;

            var collection = this.EnumItemsControl.ItemsSource as ObservableCollection<EnumItemViewModel>;

            if (collection != null)
            {
                // Enum Flags are set using the bitwise & operator
                foreach (var item in collection)
                {
                    Enum itemValue = item.Value as Enum;
                    item.IsChecked = itemValue.HasFlag(this.EnumValue as Enum);
                }
            }

            _initializing = false;
        }

        protected void UpdateValue()
        {
            var items = this.EnumItemsControl.ItemsSource as ObservableCollection<EnumItemViewModel>;

            if (items != null)
            {
                // EnumValue is set using the bitwise | operator
                int enumValue = 0;

                items.Where(item => item.IsChecked)
                     .ForEach(item =>
                     {
                         enumValue = (int)enumValue | (int)Enum.ToObject(this.EnumType, item.Value);
                     });

                this.EnumValue = Enum.ToObject(this.EnumType, enumValue);
            }
        }

        private static void OnEnumValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SimpleEnumFlagsControl;

            if (control != null)
            {
                // Selected Item (loops with selected index changed, should run twice)
                control.UpdateItemsSource();

                // Raise Event for listeners
                control.RaiseEvent(new RoutedEventArgs(EnumValueChangedEvent, control));
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateValue();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateValue();
        }
    }
}
