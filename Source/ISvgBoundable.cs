// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.Drawing;

namespace Svg
{
    public interface ISvgBoundable
    {
        PointF Location { get; }

        SizeF Size { get; }

        RectangleF Bounds { get; }
    }
}
