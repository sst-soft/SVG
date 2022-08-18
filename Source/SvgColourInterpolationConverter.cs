// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using Svg.DataTypes;

namespace Svg
{
    public sealed class SvgColourInterpolationConverter : EnumBaseConverter<SvgColourInterpolation>
    {
        public SvgColourInterpolationConverter()
          : base(SvgColourInterpolation.SRGB)
        {
        }
    }
}
