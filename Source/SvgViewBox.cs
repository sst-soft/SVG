// todo: add license

using System.ComponentModel;
using System.Drawing;

namespace Svg
{
    [TypeConverter(typeof(SvgViewBoxConverter))]
    public struct SvgViewBox
    {
        public static readonly SvgViewBox Empty;

        public float MinX { get; set; }

        public float MinY { get; set; }

        public float Width { get; set; }

        public float Height { get; set; }

        public static implicit operator RectangleF(SvgViewBox value) => new RectangleF(value.MinX, value.MinY, value.Width, value.Height);

        public static implicit operator SvgViewBox(RectangleF value) => new SvgViewBox(value.X, value.Y, value.Width, value.Height);

        public SvgViewBox(float minX, float minY, float width, float height)
        {
            MinX = minX;
            MinY = minY;
            Width = width;
            Height = height;
        }

        public override bool Equals(object obj)
        {
            return obj is SvgViewBox other && Equals(other);
        }

        public bool Equals(SvgViewBox other)
        {
            return (double)MinX == (double)other.MinX && (double)MinY == (double)other.MinY && (double)Width == (double)other.Width && (double)Height == (double)other.Height;
        }

        public override int GetHashCode()
        {
            return 0 + 1000000007 * MinX.GetHashCode() + 1000000009 * MinY.GetHashCode() + 1000000021 * Width.GetHashCode() + 1000000033 * Height.GetHashCode();
        }

        public static bool operator ==(SvgViewBox lhs, SvgViewBox rhs) => lhs.Equals(rhs);

        public static bool operator !=(SvgViewBox lhs, SvgViewBox rhs) => !(lhs == rhs);

        public void AddViewBoxTransform(
      SvgAspectRatio aspectRatio,
      ISvgRenderer renderer,
      SvgFragment frag)
        {
            var dx1 = frag == null ? 0.0f : frag.X.ToDeviceValue(renderer, UnitRenderingType.Horizontal, frag);
            var dy1 = frag == null ? 0.0f : frag.Y.ToDeviceValue(renderer, UnitRenderingType.Vertical, frag);
            if (Equals(SvgViewBox.Empty))
            {
                renderer.TranslateTransform(dx1, dy1, 0);
            }
            else
            {
                var num1 = frag == null ? Width : frag.Width.ToDeviceValue(renderer, UnitRenderingType.Horizontal, frag);
                var num2 = frag == null ? Height : frag.Height.ToDeviceValue(renderer, UnitRenderingType.Vertical, frag);
                var num3 = num1 / Width;
                var num4 = num2 / Height;
                var dx2 = -MinX * num3;
                var dy2 = -MinY * num4;
                aspectRatio = aspectRatio ?? new SvgAspectRatio(SvgPreserveAspectRatio.xMidYMid);
                if (aspectRatio.Align != SvgPreserveAspectRatio.none)
                {
                    if (aspectRatio.Slice)
                    {
                        num3 = Math.Max(num3, num4);
                        num4 = Math.Max(num3, num4);
                    }
                    else
                    {
                        num3 = Math.Min(num3, num4);
                        num4 = Math.Min(num3, num4);
                    }
                    var num5 = Width / 2f * num3;
                    var num6 = Height / 2f * num4;
                    var num7 = num1 / 2f;
                    var num8 = num2 / 2f;
                    dx2 = -MinX * num3;
                    dy2 = -MinY * num4;
                    switch (aspectRatio.Align)
                    {
                        case SvgPreserveAspectRatio.xMidYMid:
                            dx2 += num7 - num5;
                            dy2 += num8 - num6;
                            break;
                        case SvgPreserveAspectRatio.xMidYMin:
                            dx2 += num7 - num5;
                            break;
                        case SvgPreserveAspectRatio.xMaxYMin:
                            dx2 += num1 - Width * num3;
                            break;
                        case SvgPreserveAspectRatio.xMinYMid:
                            dy2 += num8 - num6;
                            break;
                        case SvgPreserveAspectRatio.xMaxYMid:
                            dx2 += num1 - Width * num3;
                            dy2 += num8 - num6;
                            break;
                        case SvgPreserveAspectRatio.xMinYMax:
                            dy2 += num2 - Height * num4;
                            break;
                        case SvgPreserveAspectRatio.xMidYMax:
                            dx2 += num7 - num5;
                            dy2 += num2 - Height * num4;
                            break;
                        case SvgPreserveAspectRatio.xMaxYMax:
                            dx2 += num1 - Width * num3;
                            dy2 += num2 - Height * num4;
                            break;
                    }
                }
                renderer.TranslateTransform(dx1, dy1, 0);
                renderer.TranslateTransform(dx2, dy2, 0);
                renderer.ScaleTransform(num3, num4, 0);
            }
        }
    }
}
