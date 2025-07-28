using SimpleWpf.Extensions.Event;
using SimpleWpf.Native.CDPlayer.Events;

namespace SimpleWpf.Native.CDPlayer.Interface
{
    public interface ICDDrive
    {
        event SimpleEventHandler<CDDeviceTracksLoadedEventArgs> TracksLoadedEvent;

        void ReadTrack(int trackNumber, SimpleEventHandler<CDDataReadEventArgs> progressCallback);
        void SetDevice(char driveLetter, DeviceChangeEventType changeType);
    }
}
