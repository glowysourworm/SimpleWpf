using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;

using SimpleWpf.Extensions.ObservableCollection;

namespace SimpleWpf.Extensions.Event
{
    public delegate Task SimpleAsyncEventHandler(object sender);
    public delegate Task SimpleAsyncEventHandler<T>(T sender);
    public delegate void SimpleEventHandler();
    public delegate void SimpleEventHandler<T>(T sender);
    public delegate void SimpleEventHandler<T1, T2>(T1 item1, T2 item2);
    public delegate void SimpleEventHandler<T1, T2, T3>(T1 item1, T2 item2, T3 item3);
    public delegate void SimpleEventHandler<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4);

    /// <summary>
    /// Delegate for collection item changed for the NotifyingObservableCollection
    /// </summary>
    public delegate void CollectionItemChangedHandler<T>(T item, PropertyChangedEventArgs propertyArgs) where T : INotifyPropertyChanged;
}
