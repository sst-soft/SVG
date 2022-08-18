// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

namespace Svg
{
    [Obsolete("ISvgSupportsCoordinateUnits will be removed.")]
    internal interface ISvgSupportsCoordinateUnits
    {
        SvgCoordinateUnits GetUnits();
    }
}
