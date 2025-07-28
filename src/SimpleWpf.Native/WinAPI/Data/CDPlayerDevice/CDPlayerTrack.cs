using System.Runtime.InteropServices;

namespace SimpleWpf.Native.WinAPI.Data.CDPlayerDevice
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CDPlayerTrack
    {
        // NOTE*** I HAD TO COUNT BYTES PHYSICALLY! THE TOTAL LENGTH OF A TABLE OF CONTENTS (TOC) ENTRY
        //         APPEARS TO BE 8 BYTES. 
        //  
        //         So, I'm going to remove the Control byte to get the right alignment.
        //

        /*  https://www.codeproject.com/Articles/15725/Tutorial-on-reading-Audio-CDs

            UCHAR Reserved;
            UCHAR Control : 4;      // Removing
            UCHAR Adr : 4;
            UCHAR TrackNumber;
            UCHAR Reserved1;
            UCHAR Address[4];

        */

        public byte Reserved;
        private byte BitMapped;     // Not serialized
        /*public byte Control
        {
            get
            {
                return (byte)(BitMapped & 0x0F);
            }
            set
            {
                BitMapped = (byte)((BitMapped & 0xF0) |
                   (value & (byte)0x0F));
            }
        }*/
        public byte Adr
        {
            get
            {
                return (byte)((BitMapped & 0xF0) >> 4);
            }
            set
            {
                BitMapped = (byte)(BitMapped & 0x0F |
                   value << 4);
            }
        }
        public byte TrackNumber;
        public byte Reserved1;

        public byte Address0;
        public byte Address1;
        public byte Address2;
        public byte Address3;

        public CDPlayerTrack()
        { }
    }

    [StructLayout(LayoutKind.Sequential)]
    public class CDPlayerTrackList
    {
        public const int MAXIMUM_NUMBER_TRACKS = 100;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAXIMUM_NUMBER_TRACKS * 8)]
        public byte[] Data;

        /// <summary>
        /// No way to know what track count was here. This method should be located in the CDPlayerData struct.
        /// </summary>
        public CDPlayerTrack GetTrack(int index)
        {
            var result = new CDPlayerTrack();

            // We have the data loaded - just have to deserialize it
            //
            var baseAddress = index * Marshal.SizeOf<CDPlayerTrack>();

            result.Reserved = Data[baseAddress++];

            //result.Control = this.Data[baseAddress++];
            result.Adr = Data[baseAddress++];

            result.TrackNumber = Data[baseAddress++];
            result.Reserved1 = Data[baseAddress++];

            result.Address0 = Data[baseAddress++];
            result.Address1 = Data[baseAddress++];
            result.Address2 = Data[baseAddress++];
            result.Address3 = Data[baseAddress++];

            return result;
        }
    }
}
