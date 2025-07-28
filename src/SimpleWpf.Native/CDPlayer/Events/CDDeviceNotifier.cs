using SimpleWpf.Extensions.Event;
using SimpleWpf.Native.CDPlayer.Interface;

namespace SimpleWpf.Native.CDPlayer.Events
{
    public class CDDeviceNotifier : ICDDeviceNotifier
    {
        public event SimpleEventHandler<CDDeviceChangeEventArgs> DeviceChange;
    }
}
