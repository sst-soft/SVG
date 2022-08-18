// todo: add license

using System.Drawing;
using System.Drawing.Drawing2D;

namespace Svg
{
    [SvgElement("linearGradient")]
    public sealed class SvgLinearGradientServer : SvgGradientServer
    {
        [SvgAttribute("x1")]
        public SvgUnit X1
        {
            get => GetAttribute<SvgUnit>("x1", false, new SvgUnit(SvgUnitType.Percentage, 0.0f));
            set => Attributes["x1"] = value;
        }

        [SvgAttribute("y1")]
        public SvgUnit Y1
        {
            get => GetAttribute<SvgUnit>("y1", false, new SvgUnit(SvgUnitType.Percentage, 0.0f));
            set => Attributes["y1"] = value;
        }

        [SvgAttribute("x2")]
        public SvgUnit X2
        {
            get => GetAttribute<SvgUnit>("x2", false, new SvgUnit(SvgUnitType.Percentage, 100f));
            set => Attributes["x2"] = value;
        }

        [SvgAttribute("y2")]
        public SvgUnit Y2
        {
            get => GetAttribute<SvgUnit>("y2", false, new SvgUnit(SvgUnitType.Percentage, 0.0f));
            set => Attributes["y2"] = value;
        }

        private bool IsInvalid => Stops.Count < 2;

        public override Brush GetBrush(
          SvgVisualElement renderingElement,
          ISvgRenderer renderer,
          float opacity,
          bool forStroke = false)
        {

            LoadStops(renderingElement);
            if (Stops.Count < 1)
            {
                return null;
            }

            if (Stops.Count == 1)
            {
                Color color = Stops[0].GetColor(renderingElement);
                return new SolidBrush(System.Drawing.Color.FromArgb((int)Math.Round((double)opacity * (color.A / (double)byte.MaxValue) * byte.MaxValue), color));
            }
            try
            {
                if (GradientUnits == SvgCoordinateUnits.ObjectBoundingBox)
                {
                    renderer.SetBoundable(renderingElement);
                }

                PointF[] pointFArray = new PointF[2]
                {
          SvgUnit.GetDevicePoint(NormalizeUnit(X1), NormalizeUnit(Y1), renderer,  this),
          SvgUnit.GetDevicePoint(NormalizeUnit(X2), NormalizeUnit(Y2), renderer,  this)
                };
                RectangleF bounds = renderer.GetBoundable().Bounds;
                if ((double)bounds.Width <= 0.0 || (double)bounds.Height <= 0.0 || (double)pointFArray[0].X == (double)pointFArray[1].X && (double)pointFArray[0].Y == (double)pointFArray[1].Y)
                {
                    return GetCallback != null ? GetCallback().GetBrush(renderingElement, renderer, opacity, forStroke) : null;
                }

                using (Matrix gradientTransform = EffectiveGradientTransform)
                {
                    PointF pointF = new PointF((float)(((double)pointFArray[0].X + (double)pointFArray[1].X) / 2.0), (float)(((double)pointFArray[0].Y + (double)pointFArray[1].Y) / 2.0));
                    gradientTransform.Translate(bounds.X, bounds.Y, 0);
                    if (GradientUnits == SvgCoordinateUnits.ObjectBoundingBox)
                    {
                        gradientTransform.Scale(bounds.Width, bounds.Height, 0);
                        gradientTransform.RotateAt(-90f, pointF, 0);
                    }
                    gradientTransform.TransformPoints(pointFArray);
                }
                if (GradientUnits == SvgCoordinateUnits.ObjectBoundingBox)
                {
                    PointF pointF = new PointF((float)(((double)pointFArray[0].X + (double)pointFArray[1].X) / 2.0), (float)(((double)pointFArray[0].Y + (double)pointFArray[1].Y) / 2.0));
                    var x1 = pointFArray[1].Y - pointFArray[0].Y;
                    var x2 = pointFArray[1].X - pointFArray[0].X;
                    var x3 = pointFArray[0].X;
                    var y1 = pointFArray[1].Y;
                    if (Math.Round((double)x2, 4) == 0.0)
                    {
                        pointFArray[0] = new PointF(pointF.X + x1 / 2f * bounds.Width / bounds.Height, pointF.Y);
                        pointFArray[1] = new PointF(pointF.X - x1 / 2f * bounds.Width / bounds.Height, pointF.Y);
                    }
                    else if (Math.Round((double)x1, 4) == 0.0)
                    {
                        pointFArray[0] = new PointF(pointF.X, pointF.Y - x2 / 2f * bounds.Height / bounds.Width);
                        pointFArray[1] = new PointF(pointF.X, pointF.Y + x2 / 2f * bounds.Height / bounds.Width);
                    }
                    else
                    {
                        var x4 = (float)(((double)x1 * (double)x2 * ((double)pointF.Y - (double)y1) + Math.Pow((double)x2, 2.0) * (double)pointF.X + Math.Pow((double)x1, 2.0) * (double)x3) / (Math.Pow((double)x2, 2.0) + Math.Pow((double)x1, 2.0)));
                        var y2 = x1 * (x4 - x3) / x2 + y1;
                        pointFArray[0] = new PointF(x4, y2);
                        pointFArray[1] = new PointF(pointF.X + (pointF.X - x4), pointF.Y + (pointF.Y - y2));
                    }
                }
                PointF startPoint = pointFArray[0];
                PointF endPoint = pointFArray[1];
                if (PointsToMove(renderingElement, pointFArray[0], pointFArray[1]) > SvgLinearGradientServer.LinePoints.None)
                {
                    SvgLinearGradientServer.GradientPoints gradientPoints = ExpandGradient(renderingElement, pointFArray[0], pointFArray[1]);
                    startPoint = gradientPoints.StartPoint;
                    endPoint = gradientPoints.EndPoint;
                }
                return new LinearGradientBrush(startPoint, endPoint, System.Drawing.Color.Transparent, System.Drawing.Color.Transparent)
                {
                    InterpolationColors = CalculateColorBlend(renderer, opacity, pointFArray[0], startPoint, pointFArray[1], endPoint),
                    WrapMode = (WrapMode)1
                };
            }
            finally
            {
                if (GradientUnits == SvgCoordinateUnits.ObjectBoundingBox)
                {
                    renderer.PopBoundable();
                }
            }
        }

        private SvgUnit NormalizeUnit(SvgUnit orig)
        {
            return orig.Type != SvgUnitType.Percentage || GradientUnits != SvgCoordinateUnits.ObjectBoundingBox ? orig : new SvgUnit(SvgUnitType.User, orig.Value / 100f);
        }

        private SvgLinearGradientServer.LinePoints PointsToMove(
          ISvgBoundable boundable,
          PointF specifiedStart,
          PointF specifiedEnd)
        {
            RectangleF bounds1 = boundable.Bounds;
            if ((double)specifiedStart.X == (double)specifiedEnd.X)
            {
                return (SvgLinearGradientServer.LinePoints)(((double)bounds1.Top >= (double)specifiedStart.Y || (double)specifiedStart.Y >= (double)bounds1.Bottom ? 0 : 1) | ((double)bounds1.Top >= (double)specifiedEnd.Y || (double)specifiedEnd.Y >= (double)bounds1.Bottom ? 0 : 2));
            }

            if ((double)specifiedStart.Y == (double)specifiedEnd.Y)
            {
                return (SvgLinearGradientServer.LinePoints)(((double)bounds1.Left >= (double)specifiedStart.X || (double)specifiedStart.X >= (double)bounds1.Right ? 0 : 1) | ((double)bounds1.Left >= (double)specifiedEnd.X || (double)specifiedEnd.X >= (double)bounds1.Right ? 0 : 2));
            }

            RectangleF bounds2 = boundable.Bounds;
            var num1 = bounds2.Contains(specifiedStart) ? 1 : 0;
            bounds2 = boundable.Bounds;
            var num2 = bounds2.Contains(specifiedEnd) ? 2 : 0;
            return (SvgLinearGradientServer.LinePoints)(num1 | num2);
        }

        private SvgLinearGradientServer.GradientPoints ExpandGradient(
          ISvgBoundable boundable,
          PointF specifiedStart,
          PointF specifiedEnd)
        {
            SvgLinearGradientServer.LinePoints move = PointsToMove(boundable, specifiedStart, specifiedEnd);
            if (move == SvgLinearGradientServer.LinePoints.None)
            {
                return new SvgLinearGradientServer.GradientPoints(specifiedStart, specifiedEnd);
            }

            RectangleF bounds = boundable.Bounds;
            PointF pointF1 = specifiedStart;
            PointF pointF2 = specifiedEnd;
            IList<PointF> source = CandidateIntersections(bounds, specifiedStart, specifiedEnd);
            if (Math.Sign(source[1].X - source[0].X) != Math.Sign(specifiedEnd.X - specifiedStart.X) || Math.Sign(source[1].Y - source[0].Y) != Math.Sign(specifiedEnd.Y - specifiedStart.Y))
            {
                source = source.Reverse<PointF>().ToList<PointF>();
            }

            if ((move & SvgLinearGradientServer.LinePoints.Start) > SvgLinearGradientServer.LinePoints.None)
            {
                pointF1 = source[0];
            }

            if ((move & SvgLinearGradientServer.LinePoints.End) > SvgLinearGradientServer.LinePoints.None)
            {
                pointF2 = source[1];
            }

            switch (SpreadMethod)
            {
                case SvgGradientSpreadMethod.Reflect:
                case SvgGradientSpreadMethod.Repeat:
                    var distance1 = SvgGradientServer.CalculateDistance(specifiedStart, specifiedEnd);
                    PointF unitVector1 = new PointF((specifiedEnd.X - specifiedStart.X) / (float)distance1, (specifiedEnd.Y - specifiedStart.Y) / (float)distance1);
                    PointF unitVector2 = new PointF(-unitVector1.X, -unitVector1.Y);
                    var distance2 = (float)(Math.Ceiling(SvgGradientServer.CalculateDistance(pointF1, specifiedStart) / distance1) * distance1);
                    pointF1 = SvgLinearGradientServer.MovePointAlongVector(specifiedStart, unitVector2, distance2);
                    var distance3 = (float)(Math.Ceiling(SvgGradientServer.CalculateDistance(pointF2, specifiedEnd) / distance1) * distance1);
                    pointF2 = SvgLinearGradientServer.MovePointAlongVector(specifiedEnd, unitVector1, distance3);
                    break;
            }
            return new SvgLinearGradientServer.GradientPoints(pointF1, pointF2);
        }

        private IList<PointF> CandidateIntersections(
          RectangleF bounds,
          PointF p1,
          PointF p2)
        {
            List<PointF> pointFList = new List<PointF>();
            if (Math.Round((double)Math.Abs(p1.Y - p2.Y), 4) == 0.0)
            {
                pointFList.Add(new PointF(bounds.Left, p1.Y));
                pointFList.Add(new PointF(bounds.Right, p1.Y));
            }
            else if (Math.Round((double)Math.Abs(p1.X - p2.X), 4) == 0.0)
            {
                pointFList.Add(new PointF(p1.X, bounds.Top));
                pointFList.Add(new PointF(p1.X, bounds.Bottom));
            }
            else
            {
                PointF pointF;
                if (((double)p1.X == (double)bounds.Left || (double)p1.X == (double)bounds.Right) && ((double)p1.Y == (double)bounds.Top || (double)p1.Y == (double)bounds.Bottom))
                {
                    pointFList.Add(p1);
                }
                else
                {
                    pointF = new PointF(bounds.Left, (float)(((double)p2.Y - (double)p1.Y) / ((double)p2.X - (double)p1.X) * ((double)bounds.Left - (double)p1.X)) + p1.Y);
                    if ((double)bounds.Top <= (double)pointF.Y && (double)pointF.Y <= (double)bounds.Bottom)
                    {
                        pointFList.Add(pointF);
                    }

                    pointF = new PointF(bounds.Right, (float)(((double)p2.Y - (double)p1.Y) / ((double)p2.X - (double)p1.X) * ((double)bounds.Right - (double)p1.X)) + p1.Y);
                    if ((double)bounds.Top <= (double)pointF.Y && (double)pointF.Y <= (double)bounds.Bottom)
                    {
                        pointFList.Add(pointF);
                    }
                }
                if (((double)p2.X == (double)bounds.Left || (double)p2.X == (double)bounds.Right) && ((double)p2.Y == (double)bounds.Top || (double)p2.Y == (double)bounds.Bottom))
                {
                    pointFList.Add(p2);
                }
                else
                {
                    pointF = new PointF((float)(((double)bounds.Top - (double)p1.Y) / ((double)p2.Y - (double)p1.Y) * ((double)p2.X - (double)p1.X)) + p1.X, bounds.Top);
                    if ((double)bounds.Left <= (double)pointF.X && (double)pointF.X <= (double)bounds.Right)
                    {
                        pointFList.Add(pointF);
                    }

                    pointF = new PointF((float)(((double)bounds.Bottom - (double)p1.Y) / ((double)p2.Y - (double)p1.Y) * ((double)p2.X - (double)p1.X)) + p1.X, bounds.Bottom);
                    if ((double)bounds.Left <= (double)pointF.X && (double)pointF.X <= (double)bounds.Right)
                    {
                        pointFList.Add(pointF);
                    }
                }
            }
            return pointFList;
        }

        private ColorBlend CalculateColorBlend(
          ISvgRenderer renderer,
          float opacity,
          PointF specifiedStart,
          PointF effectiveStart,
          PointF specifiedEnd,
          PointF effectiveEnd)
        {
            ColorBlend colorBlend = GetColorBlend(renderer, opacity, false);
            var distance1 = SvgGradientServer.CalculateDistance(specifiedStart, effectiveStart);
            var distance2 = SvgGradientServer.CalculateDistance(specifiedEnd, effectiveEnd);
            if (distance1 <= 0.0 && distance2 <= 0.0)
            {
                return colorBlend;
            }

            var distance3 = SvgGradientServer.CalculateDistance(specifiedStart, specifiedEnd);
            PointF unitVector = new PointF((specifiedEnd.X - specifiedStart.X) / (float)distance3, (specifiedEnd.Y - specifiedStart.Y) / (float)distance3);
            var distance4 = SvgGradientServer.CalculateDistance(effectiveStart, effectiveEnd);
            float startExtend;
            float endExtend;
            switch (SpreadMethod)
            {
                case SvgGradientSpreadMethod.Reflect:
                    startExtend = (float)Math.Ceiling(SvgGradientServer.CalculateDistance(effectiveStart, specifiedStart) / distance3);
                    endExtend = (float)Math.Ceiling(SvgGradientServer.CalculateDistance(effectiveEnd, specifiedEnd) / distance3);
                    List<Color> list1 = colorBlend.Colors.ToList<Color>();
                    List<float> list2 = colorBlend.Positions.Select<float, float>(p => p + startExtend).ToList<float>();
                    for (var index1 = 0; index1 < (double)startExtend; ++index1)
                    {
                        if (index1 % 2 == 0)
                        {
                            for (var index2 = 1; index2 < colorBlend.Positions.Length; ++index2)
                            {
                                list2.Insert(0, (float)((double)startExtend - 1.0 - index1 + 1.0) - colorBlend.Positions[index2]);
                                list1.Insert(0, colorBlend.Colors[index2]);
                            }
                        }
                        else
                        {
                            for (var index3 = 0; index3 < colorBlend.Positions.Length - 1; ++index3)
                            {
                                list2.Insert(index3, startExtend - 1f - index1 + colorBlend.Positions[index3]);
                                list1.Insert(index3, colorBlend.Colors[index3]);
                            }
                        }
                    }
                    for (var index4 = 0; index4 < (double)endExtend; ++index4)
                    {
                        if (index4 % 2 == 0)
                        {
                            var count = list2.Count;
                            for (var index5 = 0; index5 < colorBlend.Positions.Length - 1; ++index5)
                            {
                                list2.Insert(count, (float)((double)startExtend + 1.0 + index4 + 1.0) - colorBlend.Positions[index5]);
                                list1.Insert(count, colorBlend.Colors[index5]);
                            }
                        }
                        else
                        {
                            for (var index6 = 1; index6 < colorBlend.Positions.Length; ++index6)
                            {
                                list2.Add(startExtend + 1f + index4 + colorBlend.Positions[index6]);
                                list1.Add(colorBlend.Colors[index6]);
                            }
                        }
                    }
                    colorBlend.Colors = list1.ToArray();
                    colorBlend.Positions = list2.Select<float, float>(p => p / (startExtend + 1f + endExtend)).ToArray<float>();
                    break;
                case SvgGradientSpreadMethod.Repeat:
                    startExtend = (float)Math.Ceiling(SvgGradientServer.CalculateDistance(effectiveStart, specifiedStart) / distance3);
                    endExtend = (float)Math.Ceiling(SvgGradientServer.CalculateDistance(effectiveEnd, specifiedEnd) / distance3);
                    List<Color> colorList = new List<Color>();
                    List<float> floatList = new List<float>();
                    for (var index7 = 0; index7 < (double)startExtend + (double)endExtend + 1.0; ++index7)
                    {
                        for (var index8 = 0; index8 < colorBlend.Positions.Length; ++index8)
                        {
                            floatList.Add((float)((index7 + colorBlend.Positions[index8] * 0.99989998340606689) / ((double)startExtend + (double)endExtend + 1.0)));
                            colorList.Add(colorBlend.Colors[index8]);
                        }
                    }
                    floatList[floatList.Count - 1] = 1f;
                    colorBlend.Colors = colorList.ToArray();
                    colorBlend.Positions = floatList.ToArray();
                    break;
                default:
                    for (var index = 0; index < colorBlend.Positions.Length; ++index)
                    {
                        PointF second = SvgLinearGradientServer.MovePointAlongVector(specifiedStart, unitVector, (float)distance3 * colorBlend.Positions[index]);
                        var distance5 = SvgGradientServer.CalculateDistance(effectiveStart, second);
                        colorBlend.Positions[index] = (float)Math.Round(Math.Max(0.0, Math.Min(distance5 / distance4, 1.0)), 5);
                    }
                    if (distance1 > 0.0)
                    {
                        colorBlend.Positions = (new float[1]).Concat<float>(colorBlend.Positions).ToArray<float>();
                        colorBlend.Colors = (new Color[1]
                        {
               colorBlend.Colors.First<Color>()
                        }).Concat<Color>(colorBlend.Colors).ToArray<Color>();
                    }
                    if (distance2 > 0.0)
                    {
                        colorBlend.Positions = colorBlend.Positions.Concat<float>(new float[1]
                        {
              1f
                        }).ToArray<float>();
                        colorBlend.Colors = colorBlend.Colors.Concat<Color>(new Color[1]
                        {
               colorBlend.Colors.Last<Color>()
                        }).ToArray<Color>();
                        break;
                    }
                    break;
            }
            return colorBlend;
        }

        private static PointF CalculateClosestIntersectionPoint(
          PointF sourcePoint,
          IList<PointF> targetPoints)
        {
            return SvgGradientServer.CalculateDistance(sourcePoint, targetPoints[0]) >= SvgGradientServer.CalculateDistance(sourcePoint, targetPoints[1]) ? targetPoints[1] : targetPoints[0];
        }

        private static PointF MovePointAlongVector(
          PointF start,
          PointF unitVector,
          float distance)
        {
            return start + new SizeF(unitVector.X * distance, unitVector.Y * distance);
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgLinearGradientServer>();
        }

        public override SvgElement DeepCopy<T>()
        {
            SvgLinearGradientServer linearGradientServer = base.DeepCopy<T>() as SvgLinearGradientServer;
            linearGradientServer.X1 = X1;
            linearGradientServer.Y1 = Y1;
            linearGradientServer.X2 = X2;
            linearGradientServer.Y2 = Y2;
            return linearGradientServer;
        }

        [Flags]
        private enum LinePoints
        {
            None = 0,
            Start = 1,
            End = 2,
        }

        public struct GradientPoints
        {
            public PointF StartPoint;
            public PointF EndPoint;

            public GradientPoints(PointF startPoint, PointF endPoint)
            {
                StartPoint = startPoint;
                EndPoint = endPoint;
            }
        }

        private sealed class LineF
        {
            private float X1 { get; set; }

            private float Y1 { get; set; }

            private float X2 { get; set; }

            private float Y2 { get; set; }

            public LineF(float x1, float y1, float x2, float y2)
            {
                X1 = x1;
                Y1 = y1;
                X2 = x2;
                Y2 = y2;
            }

            public List<PointF> Intersection(RectangleF rectangle)
            {
                List<PointF> result = new List<PointF>();
                SvgLinearGradientServer.LineF.AddIfIntersect(this, new SvgLinearGradientServer.LineF(rectangle.X, rectangle.Y, rectangle.Right, rectangle.Y), result);
                SvgLinearGradientServer.LineF.AddIfIntersect(this, new SvgLinearGradientServer.LineF(rectangle.Right, rectangle.Y, rectangle.Right, rectangle.Bottom), result);
                SvgLinearGradientServer.LineF.AddIfIntersect(this, new SvgLinearGradientServer.LineF(rectangle.Right, rectangle.Bottom, rectangle.X, rectangle.Bottom), result);
                SvgLinearGradientServer.LineF.AddIfIntersect(this, new SvgLinearGradientServer.LineF(rectangle.X, rectangle.Bottom, rectangle.X, rectangle.Y), result);
                return result;
            }

            private PointF? Intersection(SvgLinearGradientServer.LineF other)
            {
                var num1 = (double)Y2 - (double)Y1;
                var num2 = (double)X1 - (double)X2;
                var num3 = num1 * (double)X1 + num2 * (double)Y1;
                var num4 = (double)other.Y2 - (double)other.Y1;
                var num5 = (double)other.X1 - (double)other.X2;
                var num6 = num4 * (double)other.X1 + num5 * (double)other.Y1;
                var num7 = num1 * num5 - num4 * num2;
                if (num7 == 0.0)
                {
                    return new PointF?();
                }

                var x = (num5 * num3 - num2 * num6) / num7;
                var y = (num1 * num6 - num4 * num3) / num7;
                return Math.Round((double)Math.Min(X1, X2), 8) <= Math.Round(x, 8) && Math.Round(x, 8) <= Math.Round((double)Math.Max(X1, X2), 8) && Math.Round((double)Math.Min(Y1, Y2), 8) <= Math.Round(y, 8) && Math.Round(y, 8) <= Math.Round((double)Math.Max(Y1, Y2), 8) && Math.Round((double)Math.Min(other.X1, other.X2), 8) <= Math.Round(x, 8) && Math.Round(x, 8) <= Math.Round((double)Math.Max(other.X1, other.X2), 8) && Math.Round((double)Math.Min(other.Y1, other.Y2), 8) <= Math.Round(y, 8) && Math.Round(y, 8) <= Math.Round((double)Math.Max(other.Y1, other.Y2), 8) ? new PointF?(new PointF((float)x, (float)y)) : new PointF?();
            }

            private static void AddIfIntersect(
              SvgLinearGradientServer.LineF first,
              SvgLinearGradientServer.LineF second,
              ICollection<PointF> result)
            {
                PointF? nullable = first.Intersection(second);
                if (!nullable.HasValue)
                {
                    return;
                }

                result.Add(nullable.Value);
            }
        }
    }
}
