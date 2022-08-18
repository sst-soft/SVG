// todo: add license

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using Svg.Transforms;

namespace Svg
{
    public abstract class SvgGradientServer : SvgPaintServer
    {
        internal SvgGradientServer()
        {
            Stops = new List<SvgGradientStop>();
        }

        protected override void AddElement(SvgElement child, int index)
        {
            if (child is SvgGradientStop)
            {
                Stops.Add((SvgGradientStop)child);
            }

            base.AddElement(child, index);
        }

        protected override void RemoveElement(SvgElement child)
        {
            if (child is SvgGradientStop)
            {
                Stops.Remove((SvgGradientStop)child);
            }

            base.RemoveElement(child);
        }

        public List<SvgGradientStop> Stops { get; private set; }

        [SvgAttribute("spreadMethod")]
        public SvgGradientSpreadMethod SpreadMethod
        {
            get => GetAttribute<SvgGradientSpreadMethod>("spreadMethod", false);
            set => Attributes["spreadMethod"] = value;
        }

        [SvgAttribute("gradientUnits")]
        public SvgCoordinateUnits GradientUnits
        {
            get => GetAttribute<SvgCoordinateUnits>("gradientUnits", false, SvgCoordinateUnits.ObjectBoundingBox);
            set => Attributes["gradientUnits"] = value;
        }

        [SvgAttribute("href", "http://www.w3.org/1999/xlink")]
        public SvgDeferredPaintServer InheritGradient
        {
            get => GetAttribute<SvgDeferredPaintServer>("href", false);
            set => Attributes["href"] = value;
        }

        [SvgAttribute("gradientTransform")]
        public SvgTransformCollection GradientTransform
        {
            get => GetAttribute<SvgTransformCollection>("gradientTransform", false);
            set => Attributes["gradientTransform"] = value;
        }

        [SvgAttribute("stop-color")]
        [TypeConverter(typeof(SvgPaintServerFactory))]
        public SvgPaintServer StopColor
        {
            get => GetAttribute<SvgPaintServer>("stop-color", false, new SvgColourServer(System.Drawing.Color.Black));
            set => Attributes["stop-color"] = value;
        }

        [SvgAttribute("stop-opacity")]
        public float StopOpacity
        {
            get => GetAttribute<float>("stop-opacity", false, 1f);
            set => Attributes["stop-opacity"] = SvgElement.FixOpacityValue(value);
        }

        protected Matrix EffectiveGradientTransform
        {
            get
            {
                Matrix gradientTransform = new Matrix();
                if (GradientTransform != null)
                {
                    using (Matrix matrix = GradientTransform.GetMatrix())
                    {
                        gradientTransform.Multiply(matrix);
                    }
                }
                return gradientTransform;
            }
        }

        protected ColorBlend GetColorBlend(ISvgRenderer renderer, float opacity, bool radial)
        {
            var count = Stops.Count;
            var flag1 = false;
            var flag2 = false;
            if ((double)Stops[0].Offset.Value > 0.0)
            {
                ++count;
                if (radial)
                {
                    flag2 = true;
                }
                else
                {
                    flag1 = true;
                }
            }
            var num1 = Stops[Stops.Count - 1].Offset.Value;
            if ((double)num1 < 100.0 || (double)num1 < 1.0)
            {
                ++count;
                if (radial)
                {
                    flag1 = true;
                }
                else
                {
                    flag2 = true;
                }
            }
            ColorBlend colorBlend = new ColorBlend(count);
            var num2 = 0;
            for (var index = 0; index < count; ++index)
            {
                SvgGradientStop stop = Stops[radial ? Stops.Count - 1 - num2 : num2];
                var width = renderer.GetBoundable().Bounds.Width;
                var num3 = (double)opacity * (double)stop.StopOpacity;
                SvgUnit offset;
                double num4;
                if (!radial)
                {
                    offset = stop.Offset;
                    num4 = (double)offset.ToDeviceValue(renderer, UnitRenderingType.Horizontal, this) / (double)width;
                }
                else
                {
                    offset = stop.Offset;
                    num4 = 1.0 - (double)offset.ToDeviceValue(renderer, UnitRenderingType.Horizontal, this) / (double)width;
                }
                var num5 = (float)Math.Round(num4, 1, (MidpointRounding)1);
                Color color = System.Drawing.Color.FromArgb((int)Math.Round(num3 * byte.MaxValue), stop.GetColor(this));
                ++num2;
                if (flag1 && index == 0)
                {
                    colorBlend.Positions[index] = 0.0f;
                    colorBlend.Colors[index] = color;
                    ++index;
                }
                colorBlend.Positions[index] = num5;
                colorBlend.Colors[index] = color;
                if (flag2 && index == count - 2)
                {
                    ++index;
                    colorBlend.Positions[index] = 1f;
                    colorBlend.Colors[index] = color;
                }
            }
            return colorBlend;
        }

        protected void LoadStops(SvgVisualElement parent)
        {
            SvgGradientServer svgGradientServer = SvgDeferredPaintServer.TryGet<SvgGradientServer>(InheritGradient, parent);
            if (Stops.Count != 0 || svgGradientServer == null)
            {
                return;
            }

            Stops.AddRange(svgGradientServer.Stops);
        }

        protected static double CalculateDistance(PointF first, PointF second)
        {
            return Math.Sqrt(Math.Pow((double)first.X - (double)second.X, 2.0) + Math.Pow((double)first.Y - (double)second.Y, 2.0));
        }

        protected static float CalculateLength(PointF vector)
        {
            return (float)Math.Sqrt(Math.Pow((double)vector.X, 2.0) + Math.Pow((double)vector.Y, 2.0));
        }

        public override SvgElement DeepCopy<T>()
        {
            SvgGradientServer svgGradientServer = base.DeepCopy<T>() as SvgGradientServer;
            svgGradientServer.SpreadMethod = SpreadMethod;
            svgGradientServer.GradientUnits = GradientUnits;
            svgGradientServer.InheritGradient = InheritGradient;
            svgGradientServer.GradientTransform = GradientTransform;
            return svgGradientServer;
        }
    }
}
