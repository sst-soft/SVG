// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.Drawing.Drawing2D;
using System.Globalization;

namespace Svg.Transforms
{
    public sealed class SvgScale : SvgTransform
    {
        public float X { get; set; }

        public float Y { get; set; }

        public override Matrix Matrix
        {
            get
            {
                Matrix matrix = new Matrix();
                matrix.Scale(X, Y);
                return matrix;
            }
        }

        public override string WriteToString()
        {
            return (double)X == (double)Y ? string.Format(CultureInfo.InvariantCulture, "scale({0})", X) : string.Format(CultureInfo.InvariantCulture, "scale({0}, {1})", X, Y);
        }

        public SvgScale(float x)
      : this(x, x)
        {
        }

        public SvgScale(float x, float y)
        {
            X = x;
            Y = y;
        }

        public override object Clone()
        {
            return new SvgScale(X, Y);
        }
    }
}
