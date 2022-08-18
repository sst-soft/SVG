﻿// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.Drawing;

namespace Svg
{
    internal class GenericBoundable : ISvgBoundable
    {
        private RectangleF _rect;

        public GenericBoundable(RectangleF rect)
        {
            _rect = rect;
        }

        public GenericBoundable(float x, float y, float width, float height)
        {
            _rect = new RectangleF(x, y, width, height);
        }

        public PointF Location => _rect.Location;

        public SizeF Size => _rect.Size;

        public RectangleF Bounds => _rect;
    }
}
