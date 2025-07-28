using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using SimpleWpf.Native.WinAPI;
using SimpleWpf.Native.WinAPI.Data;
using SimpleWpf.Native.WinAPI.Data.CDPlayerDevice;

namespace SimpleWpf.Native.CDPlayer
{
    public class CDDriveCore : IDisposable
    {
        [Flags]
        public enum ReadyState
        {
            None = 0,
            Initialized = 1,
            MediaAvailable = 2,
            ReadReady = 4,
            Locked = 8
        }

        protected const int NSECTORS = 13;
        protected const int CB_CDDASECTOR = 2368;
        protected const int CB_CDROMSECTOR = 2048;
        protected const int CB_QSUBCHANNEL = 16;
        protected const int CB_AUDIO = CB_CDDASECTOR - CB_QSUBCHANNEL;
        protected const int SECTOR_SIZE = 75;

        private IntPtr _handleCDDrive;
        private CDPlayerData _trackData = null;
        private ReadyState _readyState = ReadyState.None;

        public CDDriveCore()
        {
            _trackData = null;
            _handleCDDrive = IntPtr.Zero;
        }

        public ReadyState GetReadyState()
        {
            return _readyState;
        }
        private void CalculateReadyState()
        {
            ReadyState result = ReadyState.None;

            if ((int)_handleCDDrive != -1 && (int)_handleCDDrive != 0)
                result |= ReadyState.Initialized;

            if (_trackData != null)
                result |= ReadyState.MediaAvailable;

            if (result.HasFlag(ReadyState.Initialized) &&
                result.HasFlag(ReadyState.MediaAvailable) /*&&
                IsReadReady()*/)
                result |= ReadyState.ReadReady;

            // TODO: Need a way to know if it's locked / reading

            _readyState = result;
        }
        public int GetTrackCount()
        {
            if (!GetReadyState().HasFlag(ReadyState.ReadReady))
                throw new Exception("CD-ROM device not initialized and not yet read");

            return _trackData.LastTrack - _trackData.FirstTrack + 1;
        }
        public int GetFirstTrack()
        {
            if (!GetReadyState().HasFlag(ReadyState.ReadReady))
                throw new Exception("CD-ROM device not initialized and not yet read");

            return _trackData.FirstTrack;
        }
        public int GetLastTrack()
        {
            if (!GetReadyState().HasFlag(ReadyState.ReadReady))
                throw new Exception("CD-ROM device not initialized and not yet read");

            return _trackData.LastTrack;
        }
        public int GetStartSector(int track)
        {
            if (GetReadyState().HasFlag(ReadyState.ReadReady) &&
               (track >= _trackData.FirstTrack) &&
               (track <= _trackData.LastTrack))
            {
                return AddressToSector(_trackData.TrackData.GetTrack(track - 1));
            }
            else
            {
                return -1;
            }
        }
        public bool Initialize(char driveLetter)
        {
            if (!CDPlayerDeviceAPI.IsCDDrive(driveLetter))
                throw new ArgumentException("Provided drive is not a CD-ROM drive:  " + driveLetter);

            Dispose();

            _handleCDDrive = DeviceIO.CreateFile(@"\\.\" + driveLetter.ToString() + ":",
                                                     CDPlayerDeviceAPI.GENERIC_READ, 
                                                     CDPlayerDeviceAPI.FILE_SHARE_READ,
                                                     IntPtr.Zero, 
                                                     CDPlayerDeviceAPI.OPEN_EXISTING, 0,
                                                     IntPtr.Zero);

            CalculateReadyState();

            return _handleCDDrive > 0;
        }
        public bool LoadMedia()
        {
            if (!GetReadyState().HasFlag(ReadyState.Initialized))
                throw new Exception("CD-ROM device not initialized");

            return Load();
        }
        public bool ReadSector(int sector, byte[] buffer, int sectorsToRead, ref uint actualBytesRead)
        {
            if (GetReadyState().HasFlag(ReadyState.ReadReady) &&
               (buffer.Length >= CB_AUDIO * sectorsToRead))
            {
                var rawReadInfo = new RAW_READ_INFO();

                rawReadInfo.TrackMode = TRACK_MODE_TYPE.CDDA;
                rawReadInfo.SectorCount = (uint)sectorsToRead;
                rawReadInfo.DiskOffset = sector * CB_CDROMSECTOR;

                if (DeviceIO.DeviceIoControl(_handleCDDrive,
                                                 CDPlayerDeviceAPI.IOCTL_CDROM_RAW_READ, rawReadInfo,
                                                 (uint)Marshal.SizeOf(rawReadInfo), buffer, (uint)sectorsToRead *
                                                 CB_AUDIO, ref actualBytesRead, IntPtr.Zero) != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        private int AddressToSector(CDPlayerTrack track)
        {
            // Address[4]:  [ Hour, Minute, Second, Frame ]
            var sectors = (track.Address1 * 75 * 60) + (track.Address2 * 75) + track.Address3;

            return sectors - 150;       // 1/75 seconds per sample
        }

        #region (private) Device State Methods
        private bool Load()
        {
            if (!GetReadyState().HasFlag(ReadyState.Initialized))
                throw new Exception("Trying to load CD data before initializing the device handle");

            uint Dummy = 0;
            var resultLoad = DeviceIO.DeviceIoControl(_handleCDDrive,
                                                      CDPlayerDeviceAPI.IOCTL_STORAGE_LOAD_MEDIA, 
                                                      IntPtr.Zero, 0,
                                                      IntPtr.Zero, 0, ref Dummy, IntPtr.Zero) != 0;

            // Must have an instance pointer for the native call
            _trackData = new CDPlayerData();

            uint bytesRead = 0;
            var resultTOC = DeviceIO.DeviceIoControl(_handleCDDrive,
                                                        CDPlayerDeviceAPI.IOCTL_CDROM_READ_TOC, IntPtr.Zero, 0, _trackData,
                                                        (uint)Marshal.SizeOf(_trackData), ref bytesRead,
                                                        IntPtr.Zero) != 0;

            if (!resultTOC)
                _trackData = null;

            CalculateReadyState();

            return resultLoad && resultTOC;
        }
        private bool IsReadReady()
        {
            if (!_readyState.HasFlag(ReadyState.MediaAvailable))
                throw new Exception("Trying to access read ready state before checking media available state");

            uint Dummy = 0;

            var result = DeviceIO.DeviceIoControl(_handleCDDrive,
                                                       CDPlayerDeviceAPI.IOCTL_STORAGE_CHECK_VERIFY, IntPtr.Zero, 0,
                                                       IntPtr.Zero, 0, ref Dummy, IntPtr.Zero) != 0;

            if (!result)
            {
                Dispose();
            }

            CalculateReadyState();

            return result;
        }
        private bool Eject()
        {
            if (!_readyState.HasFlag(ReadyState.Initialized))
                throw new Exception("Trying to eject CD player before initializing the device handle");

            uint Dummy = 0;
            var result = DeviceIO.DeviceIoControl(_handleCDDrive,
                                                      CDPlayerDeviceAPI.IOCTL_STORAGE_EJECT_MEDIA, IntPtr.Zero, 0,
                                                      IntPtr.Zero, 0, ref Dummy, IntPtr.Zero) != 0;

            CalculateReadyState();

            return result;
        }
        private void Close()
        {
            if (!_readyState.HasFlag(ReadyState.Initialized))
                throw new Exception("Trying to close CD player before initializing the device handle");

            // Unlock device (should call close for the device tray if applicable)
            UnLock();

            // Free CD Drive Handle
            DeviceIO.CloseHandle(_handleCDDrive);

            _handleCDDrive = IntPtr.Zero;
            _trackData = null;

            CalculateReadyState();
        }
        private bool UnLock()
        {
            if (!_readyState.HasFlag(ReadyState.Initialized))
                throw new Exception("Trying to unlock CD player before initializing the device handle");

            uint Dummy = 0;
            var preventMediaRemoval = new PREVENT_MEDIA_REMOVAL();
            preventMediaRemoval.PreventMediaRemoval = 0;

            var result = DeviceIO.DeviceIoControl(_handleCDDrive,
                                                      DeviceIO.IOCTL_STORAGE_MEDIA_REMOVAL, preventMediaRemoval,
                                                      (uint)Marshal.SizeOf(preventMediaRemoval), IntPtr.Zero, 0,
                                                      ref Dummy, IntPtr.Zero) != 0;

            CalculateReadyState();

            return result;
        }
        #endregion

        public void Dispose()
        {
            if (_readyState.HasFlag(ReadyState.Initialized))
            {
                Close();
                GC.SuppressFinalize(this);
            }
        }
    }
}
