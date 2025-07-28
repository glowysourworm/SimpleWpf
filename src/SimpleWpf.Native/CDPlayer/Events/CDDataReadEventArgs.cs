namespace SimpleWpf.Native.CDPlayer.Events
{
    public enum DeviceChangeEventType { DeviceInserted, DeviceRemoved };

    public class CDDataReadEventArgs : EventArgs
    {
        public byte[] Data { get; private set; }
        public uint DataSize { get; private set; }

        public uint TotalBytesToRead { get; private set; }
        public uint TotalBytesRead { get; private set; }

        public CDDataReadEventArgs(byte[] data, uint size, uint bytesToRead, uint bytesRead)
        {
            this.Data = data;
            this.DataSize = size;

            this.TotalBytesToRead = bytesToRead;
            this.TotalBytesRead = bytesRead;
        }
    }
}
