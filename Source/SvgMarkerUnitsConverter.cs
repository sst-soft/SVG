// todo: add license

using Svg.DataTypes;

namespace Svg
{
    public sealed class SvgMarkerUnitsConverter : EnumBaseConverter<SvgMarkerUnits>
    {
        public SvgMarkerUnitsConverter()
          : base(SvgMarkerUnits.StrokeWidth)
        {
        }
    }
}
