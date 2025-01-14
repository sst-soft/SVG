﻿// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.Drawing;
using System.Drawing.Drawing2D;

namespace Svg.Pathing
{
    public sealed class SvgLineSegment : SvgPathSegment
    {
        public SvgLineSegment(PointF start, PointF end)
        {
            Start = start;
            End = end;
        }

        public override void AddToPath(GraphicsPath graphicsPath)
        {
            graphicsPath.AddLine(Start, End);
        }

        public override string ToString()
        {
            return "L" + End.ToSvgString();
        }
    }
}
