// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.ComponentModel;
using Svg.DataTypes;

namespace Svg
{
    [TypeConverter(typeof(SvgPreserveAspectRatioConverter))]
    public enum SvgPreserveAspectRatio
    {
        xMidYMid,
        none,
        xMinYMin,
        xMidYMin,
        xMaxYMin,
        xMinYMid,
        xMaxYMid,
        xMinYMax,
        xMidYMax,
        xMaxYMax,
    }
}
