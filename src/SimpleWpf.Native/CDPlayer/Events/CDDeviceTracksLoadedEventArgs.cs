namespace SimpleWpf.Native.CDPlayer.Events
{
    public class CDDeviceTracksLoadedEventArgs
    {
        public char Drive { get; private set; }
        public int TrackCount { get; private set; }
        public bool CDDeviceReady { get; private set; }

        public CDDeviceTracksLoadedEventArgs(char drive, int trackCount, bool deviceReady)
        {
            this.Drive = drive;
            this.TrackCount = trackCount;
            this.CDDeviceReady = deviceReady;
        }
    }
}
