using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
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

        /// <summary>
        /// Raised INotifyPropertyChanged event if there's a change to the property. Returns true if there was
        /// a change
        /// </summary>
        protected virtual bool SetValueOverride<T>(ref T field, T value, [CallerMemberName] string memberName = "")
        {
            var changed = false;
            if (field == null)
                changed = value != null;
            else
                changed = !field.Equals(value);

            if (changed)
            {
                field = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(memberName));
            }

            return changed;
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
