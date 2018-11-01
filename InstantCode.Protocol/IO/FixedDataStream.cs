using System.IO;

namespace InstantCode.Protocol.IO
{
    public class FixedDataStream : Stream
    {
        private readonly Stream baseStream;

        public FixedDataStream(Stream baseStream)
        {
            this.baseStream = baseStream;
        }

        public override void Flush()
        {
            baseStream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return baseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            baseStream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var totalRead = 0;
            while (totalRead < count)
            {
                var read = baseStream.Read(buffer, offset + totalRead, count - totalRead);
                if (read < 0) 
                    return totalRead;
                totalRead += read;
            }
            return count;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            baseStream.Write(buffer, offset, count);
        }

        public override bool CanRead => baseStream.CanRead;
        public override bool CanSeek => baseStream.CanSeek;
        public override bool CanWrite => baseStream.CanWrite;
        public override long Length => baseStream.Length;
        public override long Position { get => baseStream.Position; set => baseStream.Position = value; }
    }
}
