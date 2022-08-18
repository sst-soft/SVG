// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.Drawing;
using System.Drawing.Drawing2D;

namespace Svg
{
    [SvgElement("radialGradient")]
    public sealed class SvgRadialGradientServer : SvgGradientServer
    {
        private readonly object _lockObj = new object();

        [SvgAttribute("cx")]
        public SvgUnit CenterX
        {
            get => GetAttribute<SvgUnit>("cx", false, new SvgUnit(SvgUnitType.Percentage, 50f));
            set => Attributes["cx"] = value;
        }

        [SvgAttribute("cy")]
        public SvgUnit CenterY
        {
            get => GetAttribute<SvgUnit>("cy", false, new SvgUnit(SvgUnitType.Percentage, 50f));
            set => Attributes["cy"] = value;
        }

        [SvgAttribute("r")]
        public SvgUnit Radius
        {
            get => GetAttribute<SvgUnit>("r", false, new SvgUnit(SvgUnitType.Percentage, 50f));
            set => Attributes["r"] = value;
        }

        [SvgAttribute("fx")]
        public SvgUnit FocalX
        {
            get
            {
                SvgUnit focalX = GetAttribute<SvgUnit>("fx", false, SvgUnit.None);
                if (focalX.IsEmpty || focalX.IsNone)
                {
                    focalX = CenterX;
                }

                return focalX;
            }
            set => Attributes["fx"] = value;
        }

        [SvgAttribute("fy")]
        public SvgUnit FocalY
        {
            get
            {
                SvgUnit focalY = GetAttribute<SvgUnit>("fy", false, SvgUnit.None);
                if (focalY.IsEmpty || focalY.IsNone)
                {
                    focalY = CenterY;
                }

                return focalY;
            }
            set => Attributes["fy"] = value;
        }

        private SvgUnit NormalizeUnit(SvgUnit orig)
        {
            return orig.Type != SvgUnitType.Percentage || GradientUnits != SvgCoordinateUnits.ObjectBoundingBox ? orig : new SvgUnit(SvgUnitType.User, orig.Value / 100f);
        }

        public override Brush GetBrush(SvgVisualElement renderingElement, ISvgRenderer renderer, float opacity, bool forStroke = false)
        {
            LoadStops(renderingElement);
            try
            {
                if (base.GradientUnits == SvgCoordinateUnits.ObjectBoundingBox)
                {
                    renderer.SetBoundable(renderingElement);
                }
                PointF pointF = new PointF(NormalizeUnit(CenterX).ToDeviceValue(renderer, UnitRenderingType.Horizontal, this), NormalizeUnit(CenterY).ToDeviceValue(renderer, UnitRenderingType.Vertical, this));
                PointF[] array = new PointF[1]
                {
            new PointF(NormalizeUnit(FocalX).ToDeviceValue(renderer, UnitRenderingType.Horizontal, this), NormalizeUnit(FocalY).ToDeviceValue(renderer, UnitRenderingType.Vertical, this))
                };
                var num = NormalizeUnit(Radius).ToDeviceValue(renderer, UnitRenderingType.Other, this);
                GraphicsPath graphicsPath = new GraphicsPath();
                graphicsPath.AddEllipse(pointF.X - num, pointF.Y - num, num * 2f, num * 2f);
                using (Matrix matrix = base.EffectiveGradientTransform)
                {
                    RectangleF bounds = renderer.GetBoundable().Bounds;
                    matrix.Translate(bounds.X, bounds.Y, MatrixOrder.Prepend);
                    if (base.GradientUnits == SvgCoordinateUnits.ObjectBoundingBox)
                    {
                        matrix.Scale(bounds.Width, bounds.Height, MatrixOrder.Prepend);
                    }
                    graphicsPath.Transform(matrix);
                    matrix.TransformPoints(array);
                }
                RectangleF bounds2 = RectangleF.Inflate(renderingElement.Bounds, renderingElement.StrokeWidth, renderingElement.StrokeWidth);
                var outScale = CalcScale(bounds2, graphicsPath);
                if (outScale > 1f && base.SpreadMethod == SvgGradientSpreadMethod.Pad)
                {
                    SvgGradientStop svgGradientStop = base.Stops.Last();
                    Color color = svgGradientStop.GetColor(renderingElement);
                    Color color2 = System.Drawing.Color.FromArgb((int)Math.Round(opacity * svgGradientStop.StopOpacity * 255f), color);
                    Region clip = renderer.GetClip();
                    try
                    {
                        using SolidBrush brush = new SolidBrush(color2);
                        Region region = clip.Clone();
                        region.Exclude(graphicsPath);
                        renderer.SetClip(region);
                        GraphicsPath path = renderingElement.Path(renderer);
                        if (forStroke)
                        {
                            using Pen pen = new Pen(brush, renderingElement.StrokeWidth.ToDeviceValue(renderer, UnitRenderingType.Other, renderingElement));
                            renderer.DrawPath(pen, path);
                        }
                        else
                        {
                            renderer.FillPath(brush, path);
                        }
                    }
                    finally
                    {
                        renderer.SetClip(clip);
                    }
                }
                ColorBlend interpolationColors = CalculateColorBlend(renderer, opacity, outScale, out outScale);
                RectangleF bounds3 = graphicsPath.GetBounds();
                PointF pointF2 = new PointF(bounds3.Left + bounds3.Width / 2f, bounds3.Top + bounds3.Height / 2f);
                using (Matrix matrix2 = new Matrix())
                {
                    matrix2.Translate(-1f * pointF2.X, -1f * pointF2.Y, MatrixOrder.Append);
                    matrix2.Scale(outScale, outScale, MatrixOrder.Append);
                    matrix2.Translate(pointF2.X, pointF2.Y, MatrixOrder.Append);
                    graphicsPath.Transform(matrix2);
                }
                return new PathGradientBrush(graphicsPath)
                {
                    CenterPoint = array[0],
                    InterpolationColors = interpolationColors
                };
            }
            finally
            {
                if (base.GradientUnits == SvgCoordinateUnits.ObjectBoundingBox)
                {
                    renderer.PopBoundable();
                }
            }
        }
        //PIX: 
        //public override Brush GetBrush(
        //  SvgVisualElement renderingElement,
        //  ISvgRenderer renderer,
        //  float opacity,
        //  bool forStroke = false)
        //{
        //    this.LoadStops(renderingElement);
        //    try
        //    {
        //        if (this.GradientUnits == SvgCoordinateUnits.ObjectBoundingBox)
        //            renderer.SetBoundable((ISvgBoundable)renderingElement);
        //        PointF pointF1;
        //        ref PointF local = ref pointF1;
        //        SvgUnit svgUnit = this.NormalizeUnit(this.CenterX);
        //        double deviceValue1 = (double)svgUnit.ToDeviceValue(renderer, UnitRenderingType.Horizontal, (SvgElement)this);
        //        svgUnit = this.NormalizeUnit(this.CenterY);
        //        double deviceValue2 = (double)svgUnit.ToDeviceValue(renderer, UnitRenderingType.Vertical, (SvgElement)this);
        //        local = new PointF((float)deviceValue1, (float)deviceValue2);
        //        PointF[] pointFArray1 = new PointF[1];
        //        svgUnit = this.NormalizeUnit(this.FocalX);
        //        double deviceValue3 = (double)svgUnit.ToDeviceValue(renderer, UnitRenderingType.Horizontal, (SvgElement)this);
        //        svgUnit = this.NormalizeUnit(this.FocalY);
        //        double deviceValue4 = (double)svgUnit.ToDeviceValue(renderer, UnitRenderingType.Vertical, (SvgElement)this);
        //        pointFArray1[0] = new PointF((float)deviceValue3, (float)deviceValue4);
        //        PointF[] pointFArray2 = pointFArray1;
        //        svgUnit = this.NormalizeUnit(this.Radius);
        //        float deviceValue5 = svgUnit.ToDeviceValue(renderer, UnitRenderingType.Other, (SvgElement)this);
        //        GraphicsPath path1 = new GraphicsPath();
        //        path1.AddEllipse(pointF1.X - deviceValue5, pointF1.Y - deviceValue5, deviceValue5 * 2f, deviceValue5 * 2f);
        //        using (Matrix gradientTransform = this.EffectiveGradientTransform)
        //        {
        //            RectangleF bounds = renderer.GetBoundable().Bounds;
        //            gradientTransform.Translate(bounds.X, bounds.Y, (MatrixOrder)0);
        //            if (this.GradientUnits == SvgCoordinateUnits.ObjectBoundingBox)
        //                gradientTransform.Scale(bounds.Width, bounds.Height, (MatrixOrder)0);
        //            path1.Transform(gradientTransform);
        //            gradientTransform.TransformPoints(pointFArray2);
        //        }
        //        float outScale = this.CalcScale(RectangleF.Inflate(renderingElement.Bounds, (float)renderingElement.StrokeWidth, (float)renderingElement.StrokeWidth), path1);
        //        if ((double)outScale > 1.0 && this.SpreadMethod == SvgGradientSpreadMethod.Pad)
        //        {
        //            SvgGradientStop svgGradientStop = this.Stops.Last<SvgGradientStop>();
        //            Color color1 = svgGradientStop.GetColor((SvgElement)renderingElement);
        //            Color color2 = System.Drawing.Color.FromArgb((int)Math.Round((double)opacity * (double)svgGradientStop.StopOpacity * (double)byte.MaxValue), color1);
        //            Region clip = renderer.GetClip();
        //            try
        //            {
        //                using (SolidBrush solidBrush = new SolidBrush(color2))
        //                {
        //                    Region region = clip.Clone();
        //                    region.Exclude(path1);
        //                    renderer.SetClip(region);
        //                    GraphicsPath path2 = renderingElement.Path(renderer);
        //                    if (forStroke)
        //                    {
        //                        using (Pen pen = new Pen((Brush)solidBrush, renderingElement.StrokeWidth.ToDeviceValue(renderer, UnitRenderingType.Other, (SvgElement)renderingElement)))
        //                            renderer.DrawPath(pen, path2);
        //                    }
        //                    else
        //                        renderer.FillPath((Brush)solidBrush, path2);
        //                }
        //            }
        //            finally
        //            {
        //                renderer.SetClip(clip);
        //            }
        //        }
        //        ColorBlend colorBlend = this.CalculateColorBlend(renderer, opacity, outScale, out outScale);
        //        RectangleF bounds1 = path1.GetBounds();
        //        PointF pointF2 = new PointF(bounds1.Left + bounds1.Width / 2f, bounds1.Top + bounds1.Height / 2f);
        //        using (Matrix matrix = new Matrix())
        //        {
        //            matrix.Translate(-1f * pointF2.X, -1f * pointF2.Y, (MatrixOrder)1);
        //            matrix.Scale(outScale, outScale, (MatrixOrder)1);
        //            matrix.Translate(pointF2.X, pointF2.Y, (MatrixOrder)1);
        //            path1.Transform(matrix);
        //        }
        //        return (Brush)new PathGradientBrush(path1)
        //        {
        //            CenterPoint = pointFArray2[0],
        //            InterpolationColors = colorBlend
        //        };
        //    }
        //    finally
        //    {
        //        if (this.GradientUnits == SvgCoordinateUnits.ObjectBoundingBox)
        //            renderer.PopBoundable();
        //    }
        //}

        private float CalcScale(RectangleF bounds, GraphicsPath path, Graphics graphics = null)
        {
            PointF[] second = new PointF[4]
            {
        new PointF(bounds.Left, bounds.Top),
        new PointF(bounds.Right, bounds.Top),
        new PointF(bounds.Right, bounds.Bottom),
        new PointF(bounds.Left, bounds.Bottom)
            };
            RectangleF bounds1 = path.GetBounds();
            PointF pointF = new PointF(bounds1.X + bounds1.Width / 2f, bounds1.Y + bounds1.Height / 2f);
            using (Matrix matrix = new Matrix())
            {
                matrix.Translate(-1f * pointF.X, -1f * pointF.Y, (MatrixOrder)1);
                matrix.Scale(0.95f, 0.95f, (MatrixOrder)1);
                matrix.Translate(pointF.X, pointF.Y, (MatrixOrder)1);
                PointF[] first;
                do
                {
                    if (path.IsVisible(second[0]) && path.IsVisible(second[1]) && path.IsVisible(second[2]))
                    {
                        goto label_4;
                    }

                label_2:
                    first = new PointF[4]
                    {
            new PointF(second[0].X, second[0].Y),
            new PointF(second[1].X, second[1].Y),
            new PointF(second[2].X, second[2].Y),
            new PointF(second[3].X, second[3].Y)
                    };
                    matrix.TransformPoints(second);
                    continue;
                label_4:
                    if (!path.IsVisible(second[3]))
                    {
                        goto label_2;
                    }
                    else
                    {
                        break;
                    }
                }
                while (!first.SequenceEqual<PointF>(second));
            }
            return bounds.Height / (second[2].Y - second[1].Y);
        }

        private static IEnumerable<GraphicsPath> GetDifference(RectangleF subject, GraphicsPath clip)
        {
            GraphicsPath obj = (GraphicsPath)clip.Clone();
            obj.Flatten();
            RectangleF bounds = obj.GetBounds();
            RectangleF rectangleF = RectangleF.Union(subject, bounds);
            rectangleF.Inflate(rectangleF.Width * 0.3f, rectangleF.Height * 0.3f);
            PointF pointF = new PointF((bounds.Left + bounds.Right) / 2f, (bounds.Top + bounds.Bottom) / 2f);
            List<PointF> list = new List<PointF>();
            List<PointF> rightPoints = new List<PointF>();
            PointF[] pathPoints = obj.PathPoints;
            for (var i = 0; i < pathPoints.Length; i++)
            {
                PointF item = pathPoints[i];
                if (item.X <= pointF.X)
                {
                    list.Add(item);
                }
                else
                {
                    rightPoints.Add(item);
                }
            }
            list.Sort((PointF p, PointF q) => p.Y.CompareTo(q.Y));
            rightPoints.Sort((PointF p, PointF q) => p.Y.CompareTo(q.Y));
            PointF item2 = new PointF((list.Last().X + rightPoints.Last().X) / 2f, (list.Last().Y + rightPoints.Last().Y) / 2f);
            list.Add(item2);
            rightPoints.Add(item2);
            item2 = new PointF(item2.X, rectangleF.Bottom);
            list.Add(item2);
            rightPoints.Add(item2);
            list.Add(new PointF(rectangleF.Left, rectangleF.Bottom));
            list.Add(new PointF(rectangleF.Left, rectangleF.Top));
            rightPoints.Add(new PointF(rectangleF.Right, rectangleF.Bottom));
            rightPoints.Add(new PointF(rectangleF.Right, rectangleF.Top));
            item2 = new PointF((list.First().X + rightPoints.First().X) / 2f, rectangleF.Top);
            list.Add(item2);
            rightPoints.Add(item2);
            item2 = new PointF(item2.X, (list.First().Y + rightPoints.First().Y) / 2f);
            list.Add(item2);
            rightPoints.Add(item2);
            GraphicsPath path = new GraphicsPath(FillMode.Winding);
            path.AddPolygon(list.ToArray());
            yield return path;
            path.Reset();
            path.AddPolygon(rightPoints.ToArray());
            yield return path;
        }
        //private static IEnumerable<GraphicsPath> GetDifference(
        //  RectangleF subject,
        //  GraphicsPath clip)
        //{
        //PIX:
        //GraphicsPath graphicsPath;
        //GraphicsPath graphicsPath1 = (GraphicsPath)graphicsPath.Clone();
        //graphicsPath1.Flatten();
        //RectangleF bounds = graphicsPath1.GetBounds();
        //RectangleF a;
        //RectangleF rectangleF = RectangleF.Union(a, bounds);
        //rectangleF.Inflate(rectangleF.Width * 0.3f, rectangleF.Height * 0.3f);
        //PointF pointF1 = new PointF((float)(((double)bounds.Left + (double)bounds.Right) / 2.0), (float)(((double)bounds.Top + (double)bounds.Bottom) / 2.0));
        //List<PointF> source = new List<PointF>();
        //List<PointF> rightPoints = new List<PointF>();
        //foreach (PointF pathPoint in graphicsPath1.PathPoints)
        //{
        //    if ((double)pathPoint.X <= (double)pointF1.X)
        //        source.Add(pathPoint);
        //    else
        //        rightPoints.Add(pathPoint);
        //}
        //source.Sort((Comparison<PointF>)((p, q) => p.Y.CompareTo(q.Y)));
        //rightPoints.Sort((Comparison<PointF>)((p, q) => p.Y.CompareTo(q.Y)));
        //PointF pointF2;
        //ref PointF local1 = ref pointF2;
        //double x1 = (double)source.Last<PointF>().X;
        //PointF pointF3 = rightPoints.Last<PointF>();
        //double x2 = (double)pointF3.X;
        //double x3 = (x1 + x2) / 2.0;
        //pointF3 = source.Last<PointF>();
        //double y1 = (double)pointF3.Y;
        //pointF3 = rightPoints.Last<PointF>();
        //double y2 = (double)pointF3.Y;
        //double y3 = (y1 + y2) / 2.0;
        //local1 = new PointF((float)x3, (float)y3);
        //source.Add(pointF2);
        //rightPoints.Add(pointF2);
        //pointF2 = new PointF(pointF2.X, rectangleF.Bottom);
        //source.Add(pointF2);
        //rightPoints.Add(pointF2);
        //source.Add(new PointF(rectangleF.Left, rectangleF.Bottom));
        //source.Add(new PointF(rectangleF.Left, rectangleF.Top));
        //rightPoints.Add(new PointF(rectangleF.Right, rectangleF.Bottom));
        //rightPoints.Add(new PointF(rectangleF.Right, rectangleF.Top));
        //ref PointF local2 = ref pointF2;
        //PointF pointF4 = source.First<PointF>();
        //double x4 = (double)pointF4.X;
        //pointF4 = rightPoints.First<PointF>();
        //double x5 = (double)pointF4.X;
        //double x6 = (x4 + x5) / 2.0;
        //double top = (double)rectangleF.Top;
        //local2 = new PointF((float)x6, (float)top);
        //source.Add(pointF2);
        //rightPoints.Add(pointF2);
        //ref PointF local3 = ref pointF2;
        //double x7 = (double)pointF2.X;
        //pointF4 = source.First<PointF>();
        //double y4 = (double)pointF4.Y;
        //pointF4 = rightPoints.First<PointF>();
        //double y5 = (double)pointF4.Y;
        //double y6 = (y4 + y5) / 2.0;
        //local3 = new PointF((float)x7, (float)y6);
        //source.Add(pointF2);
        //rightPoints.Add(pointF2);
        //GraphicsPath path = new GraphicsPath((FillMode)1);
        //path.AddPolygon(source.ToArray());
        //yield return path;
        //path.Reset();
        //path.AddPolygon(rightPoints.ToArray());
        //yield return path;
        //}

        private static GraphicsPath CreateGraphicsPath(
          PointF origin,
          PointF centerPoint,
          float effectiveRadius)
        {
            GraphicsPath graphicsPath = new GraphicsPath();
            graphicsPath.AddEllipse(origin.X + centerPoint.X - effectiveRadius, origin.Y + centerPoint.Y - effectiveRadius, effectiveRadius * 2f, effectiveRadius * 2f);
            return graphicsPath;
        }

        private ColorBlend CalculateColorBlend(
          ISvgRenderer renderer,
          float opacity,
          float scale,
          out float outScale)
        {
            ColorBlend colorBlend = GetColorBlend(renderer, opacity, true);
            outScale = scale;
            float newScale;
            if ((double)scale > 1.0)
            {
                switch (SpreadMethod)
                {
                    case SvgGradientSpreadMethod.Reflect:
                        newScale = (float)Math.Ceiling((double)scale);
                        List<float> list1 = colorBlend.Positions.Select<float, float>(p => (float)(1.0 + ((double)p - 1.0) / (double)newScale)).ToList<float>();
                        List<Color> list2 = colorBlend.Colors.ToList<Color>();
                        for (var index1 = 1; index1 < (double)newScale; ++index1)
                        {
                            if (index1 % 2 == 1)
                            {
                                for (var index2 = 1; index2 < colorBlend.Positions.Length; ++index2)
                                {
                                    list1.Insert(0, (float)(((double)newScale - index1 - 1.0) / (double)newScale + 1.0) - colorBlend.Positions[index2]);
                                    list2.Insert(0, colorBlend.Colors[index2]);
                                }
                            }
                            else
                            {
                                for (var index3 = 0; index3 < colorBlend.Positions.Length - 1; ++index3)
                                {
                                    list1.Insert(index3, (float)((double)newScale - index1 - 1.0) / newScale + colorBlend.Positions[index3]);
                                    list2.Insert(index3, colorBlend.Colors[index3]);
                                }
                            }
                        }
                        colorBlend.Positions = list1.ToArray();
                        colorBlend.Colors = list2.ToArray();
                        outScale = newScale;
                        break;
                    case SvgGradientSpreadMethod.Repeat:
                        newScale = (float)Math.Ceiling((double)scale);
                        List<float> list3 = colorBlend.Positions.Select<float, float>(p => p / newScale).ToList<float>();
                        List<Color> list4 = colorBlend.Colors.ToList<Color>();
                        for (var i = 1; i < (double)newScale; i++)
                        {
                            list3.AddRange(colorBlend.Positions.Select<float, float>(p => (i + ((double)p <= 0.0 ? 1f / 1000f : p)) / newScale));
                            list4.AddRange(colorBlend.Colors);
                        }
                        colorBlend.Positions = list3.ToArray();
                        colorBlend.Colors = list4.ToArray();
                        outScale = newScale;
                        break;
                    default:
                        outScale = 1f;
                        break;
                }
            }
            return colorBlend;
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgRadialGradientServer>();
        }

        public override SvgElement DeepCopy<T>()
        {
            SvgRadialGradientServer radialGradientServer = base.DeepCopy<T>() as SvgRadialGradientServer;
            radialGradientServer.CenterX = CenterX;
            radialGradientServer.CenterY = CenterY;
            radialGradientServer.Radius = Radius;
            radialGradientServer.FocalX = FocalX;
            radialGradientServer.FocalY = FocalY;
            return radialGradientServer;
        }
    }
}
