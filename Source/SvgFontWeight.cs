// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.ComponentModel;

namespace Svg
{
    [TypeConverter(typeof(SvgFontWeightConverter))]
    [Flags]
    public enum SvgFontWeight
    {
        All = 511, // 0x000001FF
        Inherit = 0,
        Normal = 8,
        Bold = 64, // 0x00000040
        Bolder = 512, // 0x00000200
        Lighter = 1024, // 0x00000400
        W100 = 1,
        W200 = 2,
        W300 = 4,
        W400 = Normal, // 0x00000008
        W500 = 16, // 0x00000010
        W600 = 32, // 0x00000020
        W700 = Bold, // 0x00000040
        W800 = 128, // 0x00000080
        W900 = 256, // 0x00000100
    }
}
