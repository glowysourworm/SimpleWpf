using System.IO;

namespace RecursiveSerializer.Formatter
{
    public class UnsignedShortFormatter : BaseFormatter<ushort>
    {
        readonly byte[] _buffer;

        public UnsignedShortFormatter()
        {
            _buffer = new byte[sizeof(ushort)];
        }

        protected override ushort ReadImpl(Stream stream)
        {
            stream.Read(_buffer, 0, _buffer.Length);

            return BitConverter.ToUInt16(_buffer, 0);
        }

        protected override void WriteImpl(Stream stream, ushort theObject)
        {
            var buffer = BitConverter.GetBytes(theObject);

            stream.Write(buffer, 0, buffer.Length);
        }
    }
}
