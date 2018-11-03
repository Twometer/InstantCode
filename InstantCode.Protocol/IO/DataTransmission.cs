using System.IO;

namespace InstantCode.Protocol.IO
{
    public class DataTransmission
    {
        public Stream Stream { get; }
        public int Length { get;  }

        public DataTransmission(Stream stream, int length)
        {
            Stream = stream;
            Length = length;
        }

        public void Close()
        {
            Stream.Close();
        }

    }
}
