﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Controls;

using SimpleWpf.Extensions;
using SimpleWpf.Extensions.Collection;
using SimpleWpf.UI.Controls.Model;

namespace SimpleWpf.UI.Controls
{
    public partial class EnumFlagsControl : UserControl
    {
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(EnumFlagsControl), new PropertyMetadata("Header", new PropertyChangedCallback(OnHeaderChanged)));

        public static readonly DependencyProperty HeaderFontSizeProperty =
            DependencyProperty.Register("HeaderFontSize", typeof(double), typeof(EnumFlagsControl), new PropertyMetadata(16.0D));

        public static readonly DependencyProperty EnumNameFontSizeProperty =
            DependencyProperty.Register("EnumNameFontSize", typeof(double), typeof(EnumFlagsControl), new PropertyMetadata(14.0D));

        public static readonly DependencyProperty EnumDescriptionFontSizeProperty =
            DependencyProperty.Register("EnumDescriptionFontSize", typeof(double), typeof(EnumFlagsControl), new PropertyMetadata(10.0D));

        public static readonly DependencyProperty ShowDescriptionsProperty =
            DependencyProperty.Register("ShowDescriptions", typeof(bool), typeof(EnumFlagsControl), new PropertyMetadata(true));

        public static readonly DependencyProperty EnumTypeProperty =
            DependencyProperty.Register("EnumType", typeof(Type), typeof(EnumFlagsControl), new PropertyMetadata(new PropertyChangedCallback(OnTypeChanged)));

        public static readonly DependencyProperty EnumValueProperty =
            DependencyProperty.Register("EnumValue", typeof(Enum), typeof(EnumFlagsControl), new PropertyMetadata(new PropertyChangedCallback(OnValueChanged)));

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

        bool _initializing = false;

        public EnumFlagsControl()
        {
            InitializeComponent();
        }

        protected void CreateItemsSource()
        {
            _initializing = true;

            var enumItems = new ObservableCollection<EnumItem>();

            foreach (Enum enumValue in Enum.GetValues(this.EnumType))
            {
                var enumName = Enum.GetName(this.EnumType, enumValue);

                enumItems.Add(new EnumItem()
                {
                    Name = enumName,
                    Value = enumValue,
                    Description = enumValue.GetAttribute<DisplayAttribute>()?.Description ?? "",
                    DisplayName = enumValue.GetAttribute<DisplayAttribute>()?.Name ?? enumName,
                    IsChecked = this.EnumValue != null ? Enum.GetName(this.EnumType, this.EnumValue) == enumName : false
                });
            }

            this.EnumItemsControl.ItemsSource = enumItems;

            _initializing = false;
        }
        protected void UpdateItemsSource()
        {
            _initializing = true;

            var items = this.EnumItemsControl.ItemsSource as ObservableCollection<EnumItem>;

            // Enum Flags are set using the bitwise & operator
            items?.ForEach(item => item.IsChecked = ((item.Value as Enum).HasFlag(this.EnumValue as Enum)));

            _initializing = false;
        }
        protected void UpdateValue()
        {
            var items = this.EnumItemsControl.ItemsSource as ObservableCollection<EnumItem>;

            // EnumValue is set using the bitwise | operator
            int enumValue = 0;

            items?.Where(item => item.IsChecked)
                     .ForEach(item =>
                     {
                         enumValue = (int)enumValue | (int)Enum.ToObject(this.EnumType, item.Value);
                     });

            this.EnumValue = Enum.ToObject(this.EnumType, enumValue);
        }

        // Update the items source when value changed
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as EnumFlagsControl;
            if (control != null &&
                e.NewValue != null &&
                control.EnumValue != null)
                control.UpdateItemsSource();
        }

        private static void OnTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as EnumFlagsControl;
            if (control != null &&
                e.NewValue != null)
                control.CreateItemsSource();
        }

        private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as EnumFlagsControl;
            if (control != null &&
                e.NewValue != null)
                control.EnumGroupBox.Header = (string)e.NewValue;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!_initializing)
                UpdateValue();
        }
    }
}
