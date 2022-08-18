// todo: add license

using System.Text;

namespace Svg
{
    internal class FileData
    {
        public FileData(byte[] dataBytes, string mimeType, Encoding charset)
        {
            Charset = charset;
            MimeType = mimeType;
            DataBytes = dataBytes;
        }

        public Encoding Charset { get; }

        public string MimeType { get; }

        public byte[] DataBytes { get; }
    }
}
