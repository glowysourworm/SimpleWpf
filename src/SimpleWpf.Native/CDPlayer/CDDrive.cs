using SimpleWpf.Extensions.Event;
using SimpleWpf.Native.CDPlayer.Events;
using SimpleWpf.Native.CDPlayer.Interface;

namespace SimpleWpf.Native.CDPlayer
{
    public class CDDrive : ICDDrive, IDisposable
    {
        public event SimpleEventHandler<CDDeviceTracksLoadedEventArgs> TracksLoadedEvent;

        protected const int NSECTORS = 13;
        protected const int CB_CDDASECTOR = 2368;
        protected const int CB_CDROMSECTOR = 2048;
        protected const int CB_QSUBCHANNEL = 16;
        protected const int CB_AUDIO = CB_CDDASECTOR - CB_QSUBCHANNEL;

        // This doesn't have to be exact - just greater than the last sector. The read will return 0 at the end, essentially.
        //
        protected const int MAX_SECTORS = 76 * 60 * 75;                     // 76 minutes * 60 seconds * 75 sectors (per second)

        private readonly CDDriveCore _device;

        public CDDrive()
        {
            _device = new CDDriveCore();
        }

        public void ReadTrack(int trackNumber, SimpleEventHandler<CDDataReadEventArgs> progressCallback)
        {
            if (!_device.GetReadyState().HasFlag(CDDriveCore.ReadyState.ReadReady))
                throw new Exception("CD-ROM device not initialized and not yet read");

            ReadTrackImpl(trackNumber, 0, 0, progressCallback);
        }
        public void SetDevice(char drive, DeviceChangeEventType changeType)
        {
            if (_device.Initialize(drive))
            {
                switch (changeType)
                {
                    case DeviceChangeEventType.DeviceInserted:
                    {
                        var loaded = _device.LoadMedia();
                        var ready = _device.GetReadyState().HasFlag(CDDriveCore.ReadyState.ReadReady);
                        var trackCount = _device.GetTrackCount();
                        OnTracksLoaded(drive, ready, trackCount);
                    }
                    break;
                    case DeviceChangeEventType.DeviceRemoved:
                    {
                        Dispose();
                        OnTracksLoaded(drive, false, 0);
                    }
                    break;
                }
            }
        }

        private int ReadTrackImpl(int trackNumber, uint startSecond, uint secondsRead, SimpleEventHandler<CDDataReadEventArgs> progressCallback)
        {
            if (!_device.GetReadyState().HasFlag(CDDriveCore.ReadyState.ReadReady))
                throw new Exception("CD-ROM device not initialized and not yet read");

            if ((trackNumber >= _device.GetFirstTrack()) &&
                (trackNumber <= _device.GetLastTrack()))
            {
                uint bytesToRead = 0;
                uint bytesRead = 0;
                int startSector = -1;
                int endSector = -1;

                // Last Track (Can't pre-compute the sector length)
                if (trackNumber == _device.GetLastTrack())
                {
                    startSector = _device.GetStartSector(trackNumber);
                    endSector = MAX_SECTORS;

                    bytesToRead = (uint)(endSector - startSector) * CB_AUDIO;
                    bytesRead = 0;
                }
                else
                {
                    startSector = _device.GetStartSector(trackNumber);
                    endSector = _device.GetStartSector(trackNumber + 1);

                    bytesToRead = (uint)(endSector - startSector) * CB_AUDIO;
                    bytesRead = 0;
                }

                byte[] data = new byte[CB_AUDIO * NSECTORS];
                bool readOK = true;

                // 0% Progress
                progressCallback(new CDDataReadEventArgs(Array.Empty<byte>(), 0, bytesToRead, 0));

                for (int sector = startSector; (sector < endSector) && readOK; sector += NSECTORS)
                {
                    //int sectorsToRead = ((sector + NSECTORS) < endSector) ? NSECTORS : (endSector - sector);
                    int sectorsToRead = NSECTORS;
                    int sectorsRead = 0;
                    uint actualBytesRead = 0;

                    readOK = _device.ReadSector(sector, data, sectorsToRead, ref actualBytesRead);

                    if (actualBytesRead % CB_AUDIO != 0)
                        throw new Exception("Sector read error:  CDDrive.cs");

                    if (readOK)
                    {
                        // Sectors Read
                        sectorsRead = (int)actualBytesRead / CB_AUDIO;

                        // Bytes Read
                        bytesRead += (uint)(CB_AUDIO * sectorsRead);

                        // % Progress
                        progressCallback(new CDDataReadEventArgs(data, (uint)(CB_AUDIO * sectorsRead), bytesToRead, bytesRead));
                    }

                    // FINAL SECTOR READ
                    if (sectorsRead < sectorsToRead)
                    {
                        return (int)bytesRead;
                    }

                }
                if (readOK)
                {
                    return (int)bytesRead;
                }
                else
                {
                    return -1;
                }

            }
            else
            {
                return -1;
            }
        }

        private void OnTracksLoaded(char driveLetter, bool isReady, int trackCount)
        {
            if (this.TracksLoadedEvent != null)
                this.TracksLoadedEvent(new CDDeviceTracksLoadedEventArgs(driveLetter, trackCount, isReady));
        }

        public void Dispose()
        {
            _device.Dispose();
        }
    }
}
