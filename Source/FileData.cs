// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

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
