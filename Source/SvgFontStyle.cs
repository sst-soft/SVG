// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.ComponentModel;

namespace Svg
{
    [TypeConverter(typeof(SvgFontStyleConverter))]
    [Flags]
    public enum SvgFontStyle
    {
        All = 7,
        Normal = 1,
        Oblique = 2,
        Italic = 4,
    }
}
