using System.Runtime.InteropServices;

namespace SimpleWpf.Native.WinAPI.Data.CDPlayerDevice
{
    [StructLayout(LayoutKind.Sequential)]
    public class RAW_READ_INFO
    {
        public long DiskOffset = 0;
        public uint SectorCount = 0;
        public TRACK_MODE_TYPE TrackMode = TRACK_MODE_TYPE.CDDA;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class PREVENT_MEDIA_REMOVAL
    {
        public byte PreventMediaRemoval = 0;
    }

    public struct DEV_BROADCAST_HDR
    {
        public uint dbch_size;
        public DeviceType dbch_devicetype;
        uint dbch_reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DEV_BROADCAST_VOLUME
    {
        public uint dbcv_size;
        public DeviceType dbcv_devicetype;
        uint dbcv_reserved;
        uint dbcv_unitmask;
        public char[] Drives
        {
            get
            {
                var drvs = "";
                for (var c = 'A'; c <= 'Z'; c++)
                {
                    if ((dbcv_unitmask & (1 << (c - 'A'))) != 0)
                    {
                        drvs += c;
                    }
                }
                return drvs.ToCharArray();
            }
        }
        public VolumeChangeFlags dbcv_flags;
    }
}
