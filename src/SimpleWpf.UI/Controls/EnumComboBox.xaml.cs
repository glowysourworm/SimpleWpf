using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Controls;

using SimpleWpf.Extensions;
using SimpleWpf.ViewModel;

namespace SimpleWpf.UI.Controls
{
    public partial class EnumComboBox : UserControl
    {
        public static readonly DependencyProperty EnumTypeProperty = DependencyProperty.Register(
            "EnumType",
            typeof(Type),
            typeof(EnumComboBox),
            new PropertyMetadata(new PropertyChangedCallback(OnEnumTypeChanged)));

        public static readonly DependencyProperty EnumValueProperty = DependencyProperty.Register(
            "EnumValue",
            typeof(object),
            typeof(EnumComboBox),
            new PropertyMetadata(new PropertyChangedCallback(OnEnumValueChanged)));

        public static readonly RoutedEvent EnumValueChangedEvent = EventManager.RegisterRoutedEvent(
            "EnumValueChanged",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(EnumComboBox));

        public Type EnumType
        {
            get { return (Type)GetValue(EnumTypeProperty); }
            set { SetValue(EnumTypeProperty, value); }
        }
        public object EnumValue
        {
            get { return GetValue(EnumValueProperty); }
            set { SetValue(EnumValueProperty, value); }
        }
        public event RoutedEventHandler EnumValueChanged
        {
            add { AddHandler(EnumValueChangedEvent, value); }
            remove { RemoveHandler(EnumValueChangedEvent, value); }
        }

        public class EnumItem : ViewModelBase
        {
            object _value;
            string _name;
            string _displayName;
            string _description;

            public object Value
            {
                get { return _value; }
                set { this.RaiseAndSetIfChanged(ref _value, value); }
            }
            public string Name
            {
                get { return _name; }
                set { this.RaiseAndSetIfChanged(ref _name, value); }
            }
            public string DisplayName
            {
                get { return _displayName; }
                set { this.RaiseAndSetIfChanged(ref _displayName, value); }
            }
            public string Description
            {
                get { return _description; }
                set { this.RaiseAndSetIfChanged(ref _description, value); }
            }

            public EnumItem()
            {
                this.Value = null;
                this.Name = string.Empty;
                this.DisplayName = string.Empty;
                this.Description = string.Empty;
            }
        }

        public EnumComboBox()
        {
            InitializeComponent();

            RaiseEvent(new RoutedEventArgs(EnumValueChangedEvent, this));
        }

        protected virtual void SetItemSource()
        {
            if (this.EnumType != null &&
                this.EnumType.IsEnum)
            {
                var itemSource = new ObservableCollection<EnumItem>();

                foreach (Enum enumValue in Enum.GetValues(this.EnumType))
                {
                    var item = new EnumItem();
                    var displayAttribute = enumValue.GetAttribute<DisplayAttribute>();

                    item.Value = enumValue;
                    item.Name = Enum.GetName(this.EnumType, enumValue) ?? string.Empty;
                    item.DisplayName = displayAttribute?.Name ?? item.Name ?? string.Empty;
                    item.Description = displayAttribute?.Description ?? string.Empty;

                    itemSource.Add(item);
                }

                // Sets initial selected item
                this.TheComboBox.ItemsSource = itemSource;
            }
        }

        // Binding -> ComboBox
        private static void OnEnumTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as EnumComboBox;

            if (instance != null)
            {
                instance.SetItemSource();
            }
        }

        // Binding -> ComboBox
        private static void OnEnumValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as EnumComboBox;

            if (instance != null &&
                e.NewValue != null &&
                instance.EnumType != null)
            {
                // Binding -> Enum Name
                var enumName = Enum.GetName(instance.EnumType, e.NewValue);

                var itemsSource = instance.TheComboBox.ItemsSource as IEnumerable<EnumItem>;

                if (itemsSource != null)
                {
                    // UI -> Set Selected Item
                    instance.TheComboBox.SelectedItem = itemsSource.FirstOrDefault(x => x.Name == enumName);

                    // Listeners
                    instance.RaiseEvent(new RoutedEventArgs(EnumValueChangedEvent, instance));
                }
            }
        }

        // ComboBox -> Binding
        private void TheComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var enumItem = this.TheComboBox.SelectedItem as EnumItem;

            if (enumItem != null)
            {
                // -> OnEnumValueChanged
                this.EnumValue = enumItem.Value;
            }
        }
    }
}

