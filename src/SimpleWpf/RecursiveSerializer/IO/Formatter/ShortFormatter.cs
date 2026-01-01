using System.IO;

namespace RecursiveSerializer.Formatter
{
    public class ShortFormatter : BaseFormatter<short>
    {
        readonly byte[] _buffer;

        public ShortFormatter()
        {
            _buffer = new byte[sizeof(short)];
        }

        protected override short ReadImpl(Stream stream)
        {
            stream.Read(_buffer, 0, _buffer.Length);

            return BitConverter.ToInt16(_buffer, 0);
        }

        protected override void WriteImpl(Stream stream, short theObject)
        {
            var buffer = BitConverter.GetBytes(theObject);

            stream.Write(buffer, 0, buffer.Length);
        }
    }
}
