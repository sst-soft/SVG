// todo: add license

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace Svg.Pathing
{
    public sealed class SvgArcSegment : SvgPathSegment
    {
        private const double RadiansPerDegree = 0.017453292519943295;
        private const double DoublePI = 6.2831853071795862;

        public float RadiusX { get; set; }

        public float RadiusY { get; set; }

        public float Angle { get; set; }

        public SvgArcSweep Sweep { get; set; }

        public SvgArcSize Size { get; set; }

        public SvgArcSegment(
          PointF start,
          float radiusX,
          float radiusY,
          float angle,
          SvgArcSize size,
          SvgArcSweep sweep,
          PointF end)
          : base(start, end)
        {
            RadiusX = Math.Abs(radiusX);
            RadiusY = Math.Abs(radiusY);
            Angle = angle;
            Sweep = sweep;
            Size = size;
        }

        private static double CalculateVectorAngle(double ux, double uy, double vx, double vy)
        {
            var num1 = Math.Atan2(uy, ux);
            var num2 = Math.Atan2(vy, vx);
            return num2 >= num1 ? num2 - num1 : 2.0 * Math.PI - (num1 - num2);
        }

        public override void AddToPath(GraphicsPath graphicsPath)
        {
            if (Start == End)
            {
                return;
            }

            if ((double)RadiusX == 0.0 && (double)RadiusY == 0.0)
            {
                graphicsPath.AddLine(Start, End);
            }
            else
            {
                var num1 = Math.Sin((double)Angle * (Math.PI / 180.0));
                var num2 = Math.Cos((double)Angle * (Math.PI / 180.0));
                var num3 = num2 * ((double)Start.X - (double)End.X) / 2.0;
                var num4 = num1;
                var y1 = (double)Start.Y;
                PointF pointF = End;
                var y2 = (double)pointF.Y;
                var num5 = y1 - y2;
                var num6 = num4 * num5 / 2.0;
                var num7 = num3 + num6;
                var num8 = -num1;
                pointF = Start;
                var x1 = (double)pointF.X;
                pointF = End;
                var x2 = (double)pointF.X;
                var num9 = x1 - x2;
                var num10 = num8 * num9 / 2.0;
                var num11 = num2;
                pointF = Start;
                var y3 = (double)pointF.Y;
                pointF = End;
                var y4 = (double)pointF.Y;
                var num12 = y3 - y4;
                var num13 = num11 * num12 / 2.0;
                var num14 = num10 + num13;
                var num15 = (double)RadiusX * (double)RadiusX * (double)RadiusY * (double)RadiusY - (double)RadiusX * (double)RadiusX * num14 * num14 - (double)RadiusY * (double)RadiusY * num7 * num7;
                var radiusX = RadiusX;
                var radiusY = RadiusY;
                double num16;
                if (num15 < 0.0)
                {
                    var num17 = (float)Math.Sqrt(1.0 - num15 / ((double)RadiusX * (double)RadiusX * (double)RadiusY * (double)RadiusY));
                    radiusX *= num17;
                    radiusY *= num17;
                    num16 = 0.0;
                }
                else
                {
                    num16 = (Size == SvgArcSize.Large && Sweep == SvgArcSweep.Positive || Size == SvgArcSize.Small && Sweep == SvgArcSweep.Negative ? -1.0 : 1.0) * Math.Sqrt(num15 / ((double)RadiusX * (double)RadiusX * num14 * num14 + (double)RadiusY * (double)RadiusY * num7 * num7));
                }

                var num18 = num16 * (double)radiusX * num14 / (double)radiusY;
                var num19 = -num16 * (double)radiusY * num7 / (double)radiusX;
                var num20 = num2 * num18 - num1 * num19;
                pointF = Start;
                var x3 = (double)pointF.X;
                pointF = End;
                var x4 = (double)pointF.X;
                var num21 = (x3 + x4) / 2.0;
                var num22 = num20 + num21;
                var num23 = num1 * num18 + num2 * num19;
                pointF = Start;
                var y5 = (double)pointF.Y;
                pointF = End;
                var y6 = (double)pointF.Y;
                var num24 = (y5 + y6) / 2.0;
                var num25 = num23 + num24;
                var num26 = SvgArcSegment.CalculateVectorAngle(1.0, 0.0, (num7 - num18) / (double)radiusX, (num14 - num19) / (double)radiusY);
                var vectorAngle = SvgArcSegment.CalculateVectorAngle((num7 - num18) / (double)radiusX, (num14 - num19) / (double)radiusY, (-num7 - num18) / (double)radiusX, (-num14 - num19) / (double)radiusY);
                if (Sweep == SvgArcSweep.Negative && vectorAngle > 0.0)
                {
                    vectorAngle -= 2.0 * Math.PI;
                }
                else if (Sweep == SvgArcSweep.Positive && vectorAngle < 0.0)
                {
                    vectorAngle += 2.0 * Math.PI;
                }

                var num27 = (int)Math.Ceiling(Math.Abs(vectorAngle / (Math.PI / 2.0)));
                var num28 = vectorAngle / num27;
                var num29 = 8.0 / 3.0 * Math.Sin(num28 / 4.0) * Math.Sin(num28 / 4.0) / Math.Sin(num28 / 2.0);
                pointF = Start;
                var num30 = pointF.X;
                pointF = Start;
                var num31 = pointF.Y;
                for (var index = 0; index < num27; ++index)
                {
                    var num32 = Math.Cos(num26);
                    var num33 = Math.Sin(num26);
                    var num34 = num26 + num28;
                    var num35 = Math.Cos(num34);
                    var num36 = Math.Sin(num34);
                    var num37 = num2 * (double)radiusX * num35 - num1 * (double)radiusY * num36 + num22;
                    var num38 = num1 * (double)radiusX * num35 + num2 * (double)radiusY * num36 + num25;
                    var num39 = num29 * (-num2 * (double)radiusX * num33 - num1 * (double)radiusY * num32);
                    var num40 = num29 * (-num1 * (double)radiusX * num33 + num2 * (double)radiusY * num32);
                    var num41 = num29 * (num2 * (double)radiusX * num36 + num1 * (double)radiusY * num35);
                    var num42 = num29 * (num1 * (double)radiusX * num36 - num2 * (double)radiusY * num35);
                    graphicsPath.AddBezier(num30, num31, num30 + (float)num39, num31 + (float)num40, (float)(num37 + num41), (float)(num38 + num42), (float)num37, (float)num38);
                    num26 = num34;
                    num30 = (float)num37;
                    num31 = (float)num38;
                }
            }
        }

        public override string ToString()
        {
            var str1 = Size == SvgArcSize.Large ? "1" : "0";
            var str2 = Sweep == SvgArcSweep.Positive ? "1" : "0";
            var strArray = new string[12];
            strArray[0] = "A";
            var num = RadiusX;
            strArray[1] = num.ToString(CultureInfo.InvariantCulture);
            strArray[2] = " ";
            num = RadiusY;
            strArray[3] = num.ToString(CultureInfo.InvariantCulture);
            strArray[4] = " ";
            num = Angle;
            strArray[5] = num.ToString(CultureInfo.InvariantCulture);
            strArray[6] = " ";
            strArray[7] = str1;
            strArray[8] = " ";
            strArray[9] = str2;
            strArray[10] = " ";
            strArray[11] = End.ToSvgString();
            return string.Concat(strArray);
        }
    }
}
