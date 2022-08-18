// todo: add license

using System.ComponentModel;
using System.Drawing;

namespace Svg
{
    public struct SvgPoint
    {
        private SvgUnit x;
        private SvgUnit y;

        public SvgUnit X
        {
            get => x;
            set => x = value;
        }

        public SvgUnit Y
        {
            get => y;
            set => y = value;
        }

        public PointF ToDeviceValue(ISvgRenderer renderer, SvgElement owner)
        {
            return SvgUnit.GetDevicePoint(X, Y, renderer, owner);
        }

        public bool IsEmpty()
        {
            return (double)X.Value == 0.0 && (double)Y.Value == 0.0;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj.GetType() == typeof(SvgPoint)))
            {
                return false;
            }

            SvgPoint svgPoint = (SvgPoint)obj;
            return svgPoint.X.Equals(X) && svgPoint.Y.Equals(Y);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public SvgPoint(string x, string y)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(SvgUnit));
            this.x = (SvgUnit)converter.ConvertFrom(x);
            this.y = (SvgUnit)converter.ConvertFrom(y);
        }

        public SvgPoint(SvgUnit x, SvgUnit y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
