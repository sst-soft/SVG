// todo: add license

using System.Drawing;
using System.Drawing.Drawing2D;

namespace Svg
{
    public class PathStatistics
    {
        private const double GqBreak_TwoPoint = 0.57735026918962573;
        private const double GqBreak_ThreePoint = 0.7745966692414834;
        private const double GqBreak_FourPoint_01 = 0.33998104358485631;
        private const double GqBreak_FourPoint_02 = 0.86113631159405257;
        private const double GqWeight_FourPoint_01 = 0.65214515486254621;
        private const double GqWeight_FourPoint_02 = 0.34785484513745385;
        private readonly PathData _data;
        private readonly double _totalLength;
        private readonly List<PathStatistics.ISegment> _segments = new List<PathStatistics.ISegment>();

        public double TotalLength => _totalLength;

        public PathStatistics(PathData data)
        {
            _data = data;
            var index = 1;
            _totalLength = 0.0;
            while (index < _data.Points.Length)
            {
                PathStatistics.ISegment segment;
                switch (_data.Types[index])
                {
                    case 1:
                        segment = new PathStatistics.LineSegment(_data.Points[index - 1], _data.Points[index]);
                        ++index;
                        break;
                    case 3:
                        segment = new PathStatistics.CubicBezierSegment(_data.Points[index - 1], _data.Points[index], _data.Points[index + 1], _data.Points[index + 2]);
                        index += 3;
                        break;
                    default:
                        throw new NotSupportedException();
                }
                segment.StartOffset = _totalLength;
                _segments.Add(segment);
                _totalLength += segment.Length;
            }
        }

        public void LocationAngleAtOffset(double offset, out PointF point, out float angle)
        {
            _segments[BinarySearchForSegment(offset, 0, _segments.Count - 1)].LocationAngleAtOffset(offset, out point, out angle);
        }

        public bool OffsetOnPath(double offset)
        {
            PathStatistics.ISegment segment = _segments[BinarySearchForSegment(offset, 0, _segments.Count - 1)];
            offset -= segment.StartOffset;
            return offset >= 0.0 && offset <= segment.Length;
        }

        private int BinarySearchForSegment(double offset, int first, int last)
        {
            if (last == first)
            {
                return first;
            }

            if (last - first == 1)
            {
                return offset < _segments[last].StartOffset ? first : last;
            }

            var num = (last + first) / 2;
            return offset < _segments[num].StartOffset ? BinarySearchForSegment(offset, first, num) : BinarySearchForSegment(offset, num, last);
        }

        private interface ISegment
        {
            double StartOffset { get; set; }

            double Length { get; }

            void LocationAngleAtOffset(double offset, out PointF point, out float rotation);
        }

        private class LineSegment : PathStatistics.ISegment
        {
            private readonly double _length;
            private readonly double _rotation;
            private PointF _start;
            private PointF _end;

            public double StartOffset { get; set; }

            public double Length => _length;

            public LineSegment(PointF start, PointF end)
            {
                _start = start;
                _end = end;
                _length = Math.Sqrt(Math.Pow((double)end.X - (double)start.X, 2.0) + Math.Pow((double)end.Y - (double)start.Y, 2.0));
                _rotation = Math.Atan2((double)end.Y - (double)start.Y, (double)end.X - (double)start.X) * 180.0 / Math.PI;
            }

            public void LocationAngleAtOffset(double offset, out PointF point, out float rotation)
            {
                offset -= StartOffset;
                if (offset < 0.0 || offset > _length)
                {
                    throw new ArgumentOutOfRangeException();
                }

                point = new PointF(_start.X + (float)(offset / _length * ((double)_end.X - (double)_start.X)), _start.Y + (float)(offset / _length * ((double)_end.Y - (double)_start.Y)));
                rotation = (float)_rotation;
            }
        }

        private class CubicBezierSegment : PathStatistics.ISegment
        {
            private PointF _p0;
            private PointF _p1;
            private PointF _p2;
            private PointF _p3;
            private readonly double _length;
            private readonly Func<double, double> _integral;
            private readonly SortedList<double, double> _lengths = new SortedList<double, double>();

            public double StartOffset { get; set; }

            public double Length => _length;

            public CubicBezierSegment(PointF p0, PointF p1, PointF p2, PointF p3)
            {
                _p0 = p0;
                _p1 = p1;
                _p2 = p2;
                _p3 = p3;
                _integral = t => CubicBezierArcLengthIntegrand(_p0, _p1, _p2, _p3, t);
                _length = GetLength(0.0, 1.0, 9.99999993922529E-09);
                _lengths.Add(0.0, 0.0);
                _lengths.Add(_length, 1.0);
            }

            private double GetLength(double left, double right, double epsilon)
            {
                var fullInt = PathStatistics.CubicBezierSegment.GaussianQuadrature(_integral, left, right, 4);
                return Subdivide(left, right, fullInt, 0.0, epsilon);
            }

            private double Subdivide(
              double left,
              double right,
              double fullInt,
              double totalLength,
              double epsilon)
            {
                var num1 = (left + right) / 2.0;
                var fullInt1 = PathStatistics.CubicBezierSegment.GaussianQuadrature(_integral, left, num1, 4);
                var fullInt2 = PathStatistics.CubicBezierSegment.GaussianQuadrature(_integral, num1, right, 4);
                if (Math.Abs(fullInt - (fullInt1 + fullInt2)) <= epsilon)
                {
                    return fullInt1 + fullInt2;
                }

                var num2 = Subdivide(left, num1, fullInt1, totalLength, epsilon / 2.0);
                totalLength += num2;
                AddElementToTable(num1, totalLength);
                return Subdivide(num1, right, fullInt2, totalLength, epsilon / 2.0) + num2;
            }

            private void AddElementToTable(double position, double totalLength)
            {
                _lengths.Add(totalLength, position);
            }

            public void LocationAngleAtOffset(double offset, out PointF point, out float rotation)
            {
                offset -= StartOffset;
                if (offset < 0.0 || offset > _length)
                {
                    throw new ArgumentOutOfRangeException();
                }

                var t = BinarySearchForParam(offset, 0, _lengths.Count - 1);
                point = CubicBezierCurve(_p0, _p1, _p2, _p3, t);
                PointF pointF = CubicBezierDerivative(_p0, _p1, _p2, _p3, t);
                rotation = (float)(Math.Atan2((double)pointF.Y, (double)pointF.X) * 180.0 / Math.PI);
            }

            private double BinarySearchForParam(double length, int first, int last)
            {
                if (last == first)
                {
                    return _lengths.Values[last];
                }

                if (last - first == 1)
                {
                    return _lengths.Values[first] + (_lengths.Values[last] - _lengths.Values[first]) * (length - _lengths.Keys[first]) / (_lengths.Keys[last] - _lengths.Keys[first]);
                }

                var num = (last + first) / 2;
                return length < _lengths.Keys[num] ? BinarySearchForParam(length, first, num) : BinarySearchForParam(length, num, last);
            }

            public static double GaussianQuadrature(
              Func<double, double> func,
              double a,
              double b,
              int points)
            {
                switch (points)
                {
                    case 1:
                        return (b - a) * func((a + b) / 2.0);
                    case 2:
                        return (b - a) / 2.0 * (func((b - a) / 2.0 * -1.0 * 0.57735026918962573 + (a + b) / 2.0) + func((b - a) / 2.0 * 0.57735026918962573 + (a + b) / 2.0));
                    case 3:
                        return (b - a) / 2.0 * (5.0 / 9.0 * func((b - a) / 2.0 * -1.0 * 0.7745966692414834 + (a + b) / 2.0) + 8.0 / 9.0 * func((a + b) / 2.0) + 5.0 / 9.0 * func((b - a) / 2.0 * 0.7745966692414834 + (a + b) / 2.0));
                    case 4:
                        return (b - a) / 2.0 * (0.65214515486254621 * func((b - a) / 2.0 * -1.0 * 0.33998104358485631 + (a + b) / 2.0) + 0.65214515486254621 * func((b - a) / 2.0 * 0.33998104358485631 + (a + b) / 2.0) + 0.34785484513745385 * func((b - a) / 2.0 * -1.0 * 0.86113631159405257 + (a + b) / 2.0) + 0.34785484513745385 * func((b - a) / 2.0 * 0.86113631159405257 + (a + b) / 2.0));
                    default:
                        throw new NotSupportedException();
                }
            }

            private PointF CubicBezierCurve(PointF p0, PointF p1, PointF p2, PointF p3, double t)
            {
                return new PointF((float)(Math.Pow(1.0 - t, 3.0) * (double)p0.X + 3.0 * Math.Pow(1.0 - t, 2.0) * t * (double)p1.X + 3.0 * (1.0 - t) * Math.Pow(t, 2.0) * (double)p2.X + Math.Pow(t, 3.0) * (double)p3.X), (float)(Math.Pow(1.0 - t, 3.0) * (double)p0.Y + 3.0 * Math.Pow(1.0 - t, 2.0) * t * (double)p1.Y + 3.0 * (1.0 - t) * Math.Pow(t, 2.0) * (double)p2.Y + Math.Pow(t, 3.0) * (double)p3.Y));
            }

            private PointF CubicBezierDerivative(
        PointF p0,
        PointF p1,
        PointF p2,
        PointF p3,
        double t)
            {
                return new PointF((float)(3.0 * Math.Pow(1.0 - t, 2.0) * ((double)p1.X - (double)p0.X) + 6.0 * (1.0 - t) * t * ((double)p2.X - (double)p1.X) + 3.0 * Math.Pow(t, 2.0) * ((double)p3.X - (double)p2.X)), (float)(3.0 * Math.Pow(1.0 - t, 2.0) * ((double)p1.Y - (double)p0.Y) + 6.0 * (1.0 - t) * t * ((double)p2.Y - (double)p1.Y) + 3.0 * Math.Pow(t, 2.0) * ((double)p3.Y - (double)p2.Y)));
            }

            private double CubicBezierArcLengthIntegrand(
              PointF p0,
              PointF p1,
              PointF p2,
              PointF p3,
              double t)
            {
                return Math.Sqrt(Math.Pow(3.0 * Math.Pow(1.0 - t, 2.0) * ((double)p1.X - (double)p0.X) + 6.0 * (1.0 - t) * t * ((double)p2.X - (double)p1.X) + 3.0 * Math.Pow(t, 2.0) * ((double)p3.X - (double)p2.X), 2.0) + Math.Pow(3.0 * Math.Pow(1.0 - t, 2.0) * ((double)p1.Y - (double)p0.Y) + 6.0 * (1.0 - t) * t * ((double)p2.Y - (double)p1.Y) + 3.0 * Math.Pow(t, 2.0) * ((double)p3.Y - (double)p2.Y), 2.0));
            }
        }
    }
}
