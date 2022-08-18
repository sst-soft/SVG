// todo: add license

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
