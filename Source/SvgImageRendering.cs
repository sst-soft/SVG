﻿// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.ComponentModel;

namespace Svg
{
    [TypeConverter(typeof(SvgImageRenderingConverter))]
    public enum SvgImageRendering
    {
        Inherit,
        Auto,
        OptimizeSpeed,
        OptimizeQuality,
    }
}
