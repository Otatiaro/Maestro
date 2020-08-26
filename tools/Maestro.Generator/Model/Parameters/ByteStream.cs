namespace Maestro.Generator.Model.Parameters
{
    public class ByteStream : ParameterBase
    {
        public ByteStream() : base()
        {

        }

        public ByteStream(ByteStream byteStream) : base(byteStream)
        {
            canRead = byteStream.canRead;
            canWrite = byteStream.canWrite;
        }

        private bool canRead;
        private bool canWrite;

        public bool CanRead { get => canRead; set => Set(ref canRead, value); }
        public bool CanWrite { get => canWrite; set => Set(ref canWrite, value); }
    }
}
