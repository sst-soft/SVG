﻿// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.Drawing;
using System.Drawing.Drawing2D;

namespace Svg.Pathing
{
    public sealed class SvgCubicCurveSegment : SvgPathSegment
    {
        public PointF FirstControlPoint { get; set; }

        public PointF SecondControlPoint { get; set; }

        public SvgCubicCurveSegment(
          PointF start,
          PointF firstControlPoint,
          PointF secondControlPoint,
          PointF end)
        {
            Start = start;
            End = end;
            FirstControlPoint = firstControlPoint;
            SecondControlPoint = secondControlPoint;
        }

        public override void AddToPath(GraphicsPath graphicsPath)
        {
            graphicsPath.AddBezier(Start, FirstControlPoint, SecondControlPoint, End);
        }

        public override string ToString()
        {
            return "C" + FirstControlPoint.ToSvgString() + " " + SecondControlPoint.ToSvgString() + " " + End.ToSvgString();
        }
    }
}
