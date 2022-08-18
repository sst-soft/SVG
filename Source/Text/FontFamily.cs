// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.Text;

namespace Svg.Text
{
    public class FontFamily
    {
        private string _fontName = string.Empty;
        private string _fontSubFamily = string.Empty;
        private string _fontPath = string.Empty;

        private FontFamily(string fontName, string fontSubFamily, string fontPath)
        {
            _fontName = fontName;
            _fontSubFamily = fontSubFamily;
            _fontPath = fontPath;
        }

        public string FontName
        {
            get => _fontName;
            set => _fontName = value;
        }

        public string FontSubFamily
        {
            get => _fontSubFamily;
            set => _fontSubFamily = value;
        }

        public string FontPath
        {
            get => _fontPath;
            set => _fontPath = value;
        }

        public static FontFamily FromPath(string fontFilePath)
        {
            var fontName = string.Empty;
            var fontSubFamily = string.Empty;
            var str1 = "UTF-8";
            var empty = string.Empty;
            using (FileStream fs = new FileStream(fontFilePath, (FileMode)3, (FileAccess)1))
            {
                FontFamily.TT_OFFSET_TABLE ttOffsetTable = new FontFamily.TT_OFFSET_TABLE()
                {
                    uMajorVersion = FontFamily.ReadUShort(fs),
                    uMinorVersion = FontFamily.ReadUShort(fs),
                    uNumOfTables = FontFamily.ReadUShort(fs),
                    uSearchRange = FontFamily.ReadUShort(fs),
                    uEntrySelector = FontFamily.ReadUShort(fs),
                    uRangeShift = FontFamily.ReadUShort(fs)
                };
                FontFamily.TT_TABLE_DIRECTORY ttTableDirectory = new FontFamily.TT_TABLE_DIRECTORY();
                var flag = false;
                for (var index = 0; index <= ttOffsetTable.uNumOfTables; ++index)
                {
                    ttTableDirectory = new FontFamily.TT_TABLE_DIRECTORY();
                    ttTableDirectory.Initialize();
                    fs.Read(ttTableDirectory.szTag, 0, ttTableDirectory.szTag.Length);
                    ttTableDirectory.uCheckSum = FontFamily.ReadULong(fs);
                    ttTableDirectory.uOffset = FontFamily.ReadULong(fs);
                    ttTableDirectory.uLength = FontFamily.ReadULong(fs);
                    if (Encoding.GetEncoding(str1).GetString(ttTableDirectory.szTag).CompareTo("name") == 0)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    return null;
                }
                fs.Seek(ttTableDirectory.uOffset, 0);
                FontFamily.TT_NAME_TABLE_HEADER ttNameTableHeader = new FontFamily.TT_NAME_TABLE_HEADER()
                {
                    uFSelector = FontFamily.ReadUShort(fs),
                    uNRCount = FontFamily.ReadUShort(fs),
                    uStorageOffset = FontFamily.ReadUShort(fs)
                };
                //PIX: FontFamily.TT_NAME_RECORD ttNameRecord1 = new FontFamily.TT_NAME_RECORD();
                for (var index = 0; index <= ttNameTableHeader.uNRCount; ++index)
                {
                    FontFamily.TT_NAME_RECORD ttNameRecord2 = new FontFamily.TT_NAME_RECORD()
                    {
                        uPlatformID = FontFamily.ReadUShort(fs),
                        uEncodingID = FontFamily.ReadUShort(fs),
                        uLanguageID = FontFamily.ReadUShort(fs),
                        uNameID = FontFamily.ReadUShort(fs),
                        uStringLength = FontFamily.ReadUShort(fs),
                        uStringOffset = FontFamily.ReadUShort(fs)
                    };
                    if (ttNameRecord2.uNameID <= 2)
                    {
                        var position = fs.Position;
                        fs.Seek(ttTableDirectory.uOffset + ttNameRecord2.uStringOffset + ttNameTableHeader.uStorageOffset, 0);
                        var numArray = new byte[ttNameRecord2.uStringLength];
                        fs.Read(numArray, 0, ttNameRecord2.uStringLength);
                        var str2 = (ttNameRecord2.uEncodingID == 3 || ttNameRecord2.uEncodingID == 1 ? Encoding.BigEndianUnicode : Encoding.UTF8).GetString(numArray);
                        if (ttNameRecord2.uNameID == 1)
                        {
                            fontName = str2;
                        }

                        if (ttNameRecord2.uNameID == 2)
                        {
                            fontSubFamily = str2;
                        }
                        fs.Seek(position, 0);
                    }
                    else
                    {
                        break;
                    }
                }
                return new FontFamily(fontName, fontSubFamily, fontFilePath);
            }
        }

        private static ushort ReadChar(FileStream fs, int characters)
        {
            var numArray = new byte[(Convert.ToByte(new string[characters].Length))];
            return BitConverter.ToUInt16(FontFamily.ReadAndSwap(fs, numArray.Length), 0);
        }

        private static ushort ReadByte(FileStream fs)
        {
            var numArray = new byte[11];
            return BitConverter.ToUInt16(FontFamily.ReadAndSwap(fs, numArray.Length), 0);
        }

        private static ushort ReadUShort(FileStream fs)
        {
            var numArray = new byte[2];
            return BitConverter.ToUInt16(FontFamily.ReadAndSwap(fs, numArray.Length), 0);
        }

        private static uint ReadULong(FileStream fs)
        {
            var numArray = new byte[4];
            return BitConverter.ToUInt32(FontFamily.ReadAndSwap(fs, numArray.Length), 0);
        }

        private static byte[] ReadAndSwap(FileStream fs, int size)
        {
            var numArray = new byte[size];
            fs.Read(numArray, 0, numArray.Length);
            Array.Reverse<byte>(numArray);
            return numArray;
        }

        public struct TT_OFFSET_TABLE
        {
            public ushort uMajorVersion;
            public ushort uMinorVersion;
            public ushort uNumOfTables;
            public ushort uSearchRange;
            public ushort uEntrySelector;
            public ushort uRangeShift;
        }

        public struct TT_TABLE_DIRECTORY
        {
            public byte[] szTag;
            public uint uCheckSum;
            public uint uOffset;
            public uint uLength;

            public void Initialize()
            {
                szTag = new byte[4];
            }
        }

        public struct TT_NAME_TABLE_HEADER
        {
            public ushort uFSelector;
            public ushort uNRCount;
            public ushort uStorageOffset;
        }

        public struct TT_NAME_RECORD
        {
            public ushort uPlatformID;
            public ushort uEncodingID;
            public ushort uLanguageID;
            public ushort uNameID;
            public ushort uStringLength;
            public ushort uStringOffset;
        }
    }
}
