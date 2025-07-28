using SimpleWpf.Extensions.Event;
using SimpleWpf.Native.CDPlayer.Events;

namespace SimpleWpf.Native.CDPlayer.Interface
{
    public interface ICDDeviceNotifier
    {
        event SimpleEventHandler<CDDeviceChangeEventArgs> DeviceChange;
    }
}
