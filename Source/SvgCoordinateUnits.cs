// todo: add license

using System.ComponentModel;

namespace Svg
{
    [TypeConverter(typeof(SvgCoordinateUnitsConverter))]
    public enum SvgCoordinateUnits
    {
        Inherit,
        ObjectBoundingBox,
        UserSpaceOnUse,
    }
}
