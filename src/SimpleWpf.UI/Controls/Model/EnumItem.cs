using SimpleWpf.Extensions;

namespace SimpleWpf.UI.Controls.Model
{
    public class EnumItem : ViewModelBase
    {
        string _name;
        string _displayName;
        string _description;
        object _value;
        bool _isChecked;

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
        public object Value
        {
            get { return _value; }
            set { this.RaiseAndSetIfChanged(ref _value, value); }
        }
        public bool IsChecked
        {
            get { return _isChecked; }
            set { this.RaiseAndSetIfChanged(ref _isChecked, value); }
        }

        public EnumItem()
        {
            this.Name = string.Empty;
            this.DisplayName = string.Empty;
            this.Description = string.Empty;
            this.Value = null;
        }
    }
}
