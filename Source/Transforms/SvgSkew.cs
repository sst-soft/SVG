// todo: add license

using System.Drawing.Drawing2D;
using System.Globalization;

namespace Svg.Transforms
{
    public sealed class SvgSkew : SvgTransform
    {
        public float AngleX { get; set; }

        public float AngleY { get; set; }

        public override Matrix Matrix
        {
            get
            {
                Matrix matrix = new Matrix();
                matrix.Shear((float)Math.Tan((double)AngleX / 180.0 * Math.PI), (float)Math.Tan((double)AngleY / 180.0 * Math.PI));
                return matrix;
            }
        }

        public override string WriteToString()
        {
            return (double)AngleY == 0.0 ? string.Format(CultureInfo.InvariantCulture, "skewX({0})", AngleX) : string.Format(CultureInfo.InvariantCulture, "skewY({0})", AngleY);
        }

        public SvgSkew(float x, float y)
        {
            AngleX = x;
            AngleY = y;
        }

        public override object Clone()
        {
            return new SvgSkew(AngleX, AngleY);
        }
    }
}
