using System.Collections.Generic;

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

        protected override IEnumerable<(string, object)> Errors()
        {
            foreach (var e in base.Errors())
                yield return e;

            if(!canRead && !canWrite)
            {
                yield return (nameof(CanRead), "Byte stream is not readable nor writable");
                yield return (nameof(CanWrite), "Byte stream is not readable nor writable");
            }
        }
    }
}
