// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.ComponentModel;

namespace Svg
{
    [TypeConverter(typeof(SvgTextTransformationConverter))]
    [Flags]
    public enum SvgTextTransformation
    {
        Inherit = 0,
        None = 1,
        Capitalize = 2,
        Uppercase = 4,
        Lowercase = 8,
    }
}
