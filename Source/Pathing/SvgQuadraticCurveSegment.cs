// todo: add license

using System.Drawing;
using System.Drawing.Drawing2D;

namespace Svg.Pathing
{
    public sealed class SvgQuadraticCurveSegment : SvgPathSegment
    {
        public PointF ControlPoint { get; set; }

        private PointF FirstControlPoint
        {
            get
            {
                PointF pointF = Start;
                var x1 = (double)pointF.X;
                pointF = ControlPoint;
                var x2 = (double)pointF.X;
                pointF = Start;
                var x3 = (double)pointF.X;
                var num1 = (x2 - x3) * 2.0 / 3.0;
                var x4 = x1 + num1;
                pointF = Start;
                var y1 = (double)pointF.Y;
                pointF = ControlPoint;
                var y2 = (double)pointF.Y;
                pointF = Start;
                var y3 = (double)pointF.Y;
                var num2 = (y2 - y3) * 2.0 / 3.0;
                var y4 = y1 + num2;
                return new PointF((float)x4, (float)y4);
            }
        }

        private PointF SecondControlPoint
        {
            get
            {
                PointF pointF = ControlPoint;
                var x1 = (double)pointF.X;
                pointF = End;
                var x2 = (double)pointF.X;
                pointF = ControlPoint;
                var x3 = (double)pointF.X;
                var num1 = (x2 - x3) / 3.0;
                var x4 = x1 + num1;
                pointF = ControlPoint;
                var y1 = (double)pointF.Y;
                pointF = End;
                var y2 = (double)pointF.Y;
                pointF = ControlPoint;
                var y3 = (double)pointF.Y;
                var num2 = (y2 - y3) / 3.0;
                var y4 = y1 + num2;
                return new PointF((float)x4, (float)y4);
            }
        }

        public SvgQuadraticCurveSegment(PointF start, PointF controlPoint, PointF end)
        {
            Start = start;
            ControlPoint = controlPoint;
            End = end;
        }

        public override void AddToPath(GraphicsPath graphicsPath)
        {
            graphicsPath.AddBezier(Start, FirstControlPoint, SecondControlPoint, End);
        }

        public override string ToString()
        {
            return "Q" + ControlPoint.ToSvgString() + " " + End.ToSvgString();
        }
    }
}
