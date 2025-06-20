using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using SimpleWpf.Extensions;
using SimpleWpf.UI.Controls.Model;

namespace SimpleWpf.UI.Collection
{
    /// <summary>
    /// Produces an observable collection of type EnumItem - which is bindable to the UI. This collection is read-only. The point of
    /// this collection is to allow the binding of an enum property to a collection that is displayable like a list; and to provide
    /// two-way binding using a converter.
    /// </summary>
    public class EnumObservableCollection<TEnum> : IList<EnumItem>, IList, INotifyPropertyChanged, INotifyCollectionChanged where TEnum : Enum
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        // List that represents the enum class-object
        List<EnumItem> _list;

        // Value of the bound enum
        object _enumValue;

        /// <summary>
        /// Value of the bound enum
        /// </summary>
        public object EnumValue
        {
            get { return _enumValue; }
            set
            {
                _enumValue = value;

                if (this.PropertyChanged != null)
                    this.PropertyChanged(this, new PropertyChangedEventArgs("EnumValue"));

                // Re-calculated checked flags
                foreach (var item in _list)
                {
                    item.IsChecked = GetIsChecked((TEnum)item.Value);
                }
            }
        }

        public EnumItem this[int index]
        {
            get { return _list[index]; }
            set { this.Insert(index, value); }
        }
        object? IList.this[int index] 
        {
            get { return _list[index]; }
            set { this.Insert(index, value); }
        }

        public int Count { get { return _list.Count; } }
        public bool IsReadOnly { get { return true; } }

        public bool IsFixedSize { get; }
        public bool IsSynchronized { get; }
        public object SyncRoot { get; }

        public EnumObservableCollection()
        {
            Initialize();
        }
        public EnumObservableCollection(object enumValue)
        {
            this.EnumValue = enumValue;

            Initialize();
        }

        private void Initialize()
        {
            _list = new List<EnumItem>();

            foreach (Enum enumValue in Enum.GetValues(typeof(TEnum)))
            {
                var enumName = Enum.GetName(typeof(TEnum), enumValue);

                _list.Add(new EnumItem()
                {
                    Name = enumName,
                    Value = enumValue,
                    Description = enumValue.GetAttribute<DisplayAttribute>()?.Description ?? "",
                    DisplayName = enumValue.GetAttribute<DisplayAttribute>()?.Name ?? enumName,
                    IsChecked = GetIsChecked((TEnum)enumValue)
                });
            }

            if (this.CollectionChanged != null)
                this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private bool GetIsChecked(TEnum enumValue)
        {
            // Enum:  If it's a [Flags] enum, then must check the Has<> method. Otherwise,
            //        we can use value Equals.

            // Compare values for the enum
            var enumFlags = typeof(TEnum).GetAttribute<FlagsAttribute>() != null;
            
            if (enumFlags)
            {
                return this.EnumValue != null ? ((TEnum)this.EnumValue).Has<TEnum>(enumValue) : false;
            }
            else
                return this.EnumValue != null ? this.EnumValue.Equals(enumValue) : false;
        }

        public void Add(EnumItem item)
        {
            throw new NotSupportedException("Add method is not supported for EnumObservableCollection");
        }

        public void Clear()
        {
            throw new NotSupportedException("Clear method is not supported for EnumObservableCollection");
        }

        public bool Contains(EnumItem item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(EnumItem[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<EnumItem> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public int IndexOf(EnumItem item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, EnumItem item)
        {
            throw new NotSupportedException("Insert method is not supported for EnumObservableCollection");
        }

        public bool Remove(EnumItem item)
        {
            throw new NotSupportedException("Remove method is not supported for EnumObservableCollection");
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException("RemoveAt method is not supported for EnumObservableCollection");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public int Add(object? value)
        {
            throw new NotSupportedException("Add method is not supported for EnumObservableCollection");
        }

        public bool Contains(object? value)
        {
            return _list.Contains(value);
        }

        public int IndexOf(object? value)
        {
            return _list.IndexOf((EnumItem)value);
        }

        public void Insert(int index, object? value)
        {
            throw new NotSupportedException("Insert method is not supported for EnumObservableCollection");
        }

        public void Remove(object? value)
        {
            throw new NotSupportedException("Remove method is not supported for EnumObservableCollection");
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotSupportedException("CopyTo(Array array, int index) method is not supported for EnumObservableCollection");
        }
    }
}
