﻿// todo: add license

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
