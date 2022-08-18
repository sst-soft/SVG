// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.Drawing;
using System.Drawing.Drawing2D;

namespace Svg.Pathing
{
    public class SvgMoveToSegment : SvgPathSegment
    {
        public SvgMoveToSegment(PointF moveTo)
        {
            Start = moveTo;
            End = moveTo;
        }

        public override void AddToPath(GraphicsPath graphicsPath)
        {
            graphicsPath.StartFigure();
        }

        public override string ToString()
        {
            return "M" + Start.ToSvgString();
        }
    }
}
