namespace SimpleWpf.Native.CDPlayer.Events
{
    public class CDDeviceChangeEventArgs : EventArgs
    {
        public char Drive { get; private set; }
        public DeviceChangeEventType ChangeType { get; private set; }

        public CDDeviceChangeEventArgs(char drive, DeviceChangeEventType type)
        {
            this.Drive = drive;
            this.ChangeType = type;
        }
    }
}
