// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.Drawing.Drawing2D;

namespace Svg.Transforms
{
    public abstract class SvgTransform : ICloneable
    {
        public abstract Matrix Matrix { get; }

        public abstract string WriteToString();

        public abstract object Clone();

        public override bool Equals(object obj)
        {
            SvgTransform svgTransform = obj as SvgTransform;
            return !(svgTransform == null) && Matrix.Equals(svgTransform.Matrix);
        }

        public override int GetHashCode()
        {
            return Matrix.GetHashCode();
        }

        public static bool operator ==(SvgTransform lhs, SvgTransform rhs)
        {
            if (lhs == (object)rhs)
            {
                return true;
            }

            return (object)lhs != null && (object)rhs != null && lhs.Equals(rhs);
        }

        public static bool operator !=(SvgTransform lhs, SvgTransform rhs) => !(lhs == rhs);

        public override string ToString()
        {
            return WriteToString();
        }
    }
}
