using System.Runtime.InteropServices;

namespace SimpleWpf.Native.WinAPI.Data.CDPlayerDevice
{
    [StructLayout(LayoutKind.Sequential)]
    public class CDPlayerData
    {
        //public const int MAXIMUM_NUMBER_TRACKS = 100;

        public ushort Length;
        public byte FirstTrack = 0;
        public byte LastTrack = 0;
        public CDPlayerTrackList TrackData;

        public CDPlayerData()
        {
            // 100 tracks is some sort of windows maximum constant
            //
            TrackData = new CDPlayerTrackList();
            Length = (ushort)Marshal.SizeOf(this);
        }
    }
}
