﻿// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.Drawing;
using System.Drawing.Drawing2D;

namespace Svg.Pathing
{
    public abstract class SvgPathSegment
    {
        public PointF Start { get; set; }

        public PointF End { get; set; }

        protected SvgPathSegment()
        {
        }

        protected SvgPathSegment(PointF start, PointF end)
        {
            Start = start;
            End = end;
        }

        public abstract void AddToPath(GraphicsPath graphicsPath);

        public SvgPathSegment Clone()
        {
            return MemberwiseClone() as SvgPathSegment;
        }
    }
}
