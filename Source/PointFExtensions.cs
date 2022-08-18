// todo: add license

using System.Drawing;
using System.Globalization;

namespace Svg
{
    public static class PointFExtensions
    {
        public static string ToSvgString(this PointF p)
        {
            var num = p.X;
            var str1 = num.ToString(CultureInfo.InvariantCulture);
            num = p.Y;
            var str2 = num.ToString(CultureInfo.InvariantCulture);
            return str1 + " " + str2;
        }
    }
}
