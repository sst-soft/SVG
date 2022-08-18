﻿// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

namespace Svg
{
    public sealed class SvgGradientSpreadMethodConverter : EnumBaseConverter<SvgGradientSpreadMethod>
    {
        public SvgGradientSpreadMethodConverter()
          : base(SvgGradientSpreadMethod.Pad)
        {
        }
    }
}