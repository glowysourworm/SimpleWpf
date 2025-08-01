using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows;

namespace SimpleWpf.ViewModel
{
    public class DispatcherViewModelBase : DependencyObject, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Must use this method to inherit the INotifyPropertyChanged event properly
        /// </summary>
        protected virtual void SetValueOverride(DependencyProperty property, object value)
        {
            var changed = false;
            var field = GetValue(property);

            if (field == null)
                changed = value != null;
            else
                changed = !field.Equals(value);

            if (changed)
            {
                SetValue(property, value);

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(property.Name));
            }
        }

        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;

            if (memberExpression != null)
                OnPropertyChanged(memberExpression.Member.Name);

            else
                throw new Exception("Invalid Member Expression OnPropertyChanged<T>");
        }
    }
}
