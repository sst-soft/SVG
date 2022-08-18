// todo: add license

using System.ComponentModel;

namespace Svg
{
    [TypeConverter(typeof(SvgStrokeLineJoinConverter))]
    public enum SvgStrokeLineJoin
    {
        Inherit,
        Miter,
        Round,
        Bevel,
    }
}
