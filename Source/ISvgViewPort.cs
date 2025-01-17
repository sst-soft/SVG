﻿// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

namespace Svg
{
    public interface ISvgViewPort
    {
        SvgViewBox ViewBox { get; set; }

        SvgAspectRatio AspectRatio { get; set; }

        SvgOverflow Overflow { get; set; }
    }
}
