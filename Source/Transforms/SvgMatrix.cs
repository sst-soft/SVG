// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.Drawing.Drawing2D;
using System.Globalization;

namespace Svg.Transforms
{
    public sealed class SvgMatrix : SvgTransform
    {
        public List<float> Points { get; set; }

        public override Matrix Matrix => new Matrix(Points[0], Points[1], Points[2], Points[3], Points[4], Points[5]);

        public override string WriteToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "matrix({0}, {1}, {2}, {3}, {4}, {5})", Points[0], Points[1], Points[2], Points[3], Points[4], Points[5]);
        }

        public SvgMatrix(List<float> m)
        {
            Points = m;
        }

        public override object Clone()
        {
            return new SvgMatrix(Points);
        }
    }
}
