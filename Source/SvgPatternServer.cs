// todo: add license

using System.Drawing;
using System.Drawing.Drawing2D;
using Svg.Transforms;

namespace Svg
{
    [SvgElement("pattern")]
    public sealed class SvgPatternServer : SvgPaintServer, ISvgViewPort
    {
        private SvgUnit _x = SvgUnit.None;
        private SvgUnit _y = SvgUnit.None;
        private SvgUnit _width = SvgUnit.None;
        private SvgUnit _height = SvgUnit.None;
        private SvgCoordinateUnits _patternUnits;
        private SvgCoordinateUnits _patternContentUnits;
        private SvgViewBox _viewBox;

        [SvgAttribute("x")]
        public SvgUnit X
        {
            get => _x;
            set
            {
                _x = value;
                Attributes["x"] = value;
            }
        }

        [SvgAttribute("y")]
        public SvgUnit Y
        {
            get => _y;
            set
            {
                _y = value;
                Attributes["y"] = value;
            }
        }

        [SvgAttribute("width")]
        public SvgUnit Width
        {
            get => _width;
            set
            {
                _width = value;
                Attributes["width"] = value;
            }
        }

        [SvgAttribute("height")]
        public SvgUnit Height
        {
            get => _height;
            set
            {
                _height = value;
                Attributes["height"] = value;
            }
        }

        [SvgAttribute("patternUnits")]
        public SvgCoordinateUnits PatternUnits
        {
            get => _patternUnits;
            set
            {
                _patternUnits = value;
                Attributes["patternUnits"] = value;
            }
        }

        [SvgAttribute("patternContentUnits")]
        public SvgCoordinateUnits PatternContentUnits
        {
            get => _patternContentUnits;
            set
            {
                _patternContentUnits = value;
                Attributes["patternContentUnits"] = value;
            }
        }

        [SvgAttribute("viewBox")]
        public SvgViewBox ViewBox
        {
            get => _viewBox;
            set
            {
                _viewBox = value;
                Attributes["viewBox"] = value;
            }
        }

        [SvgAttribute("href", "http://www.w3.org/1999/xlink")]
        public SvgDeferredPaintServer InheritGradient
        {
            get => GetAttribute<SvgDeferredPaintServer>("href", false);
            set => Attributes["href"] = value;
        }

        [SvgAttribute("overflow")]
        public SvgOverflow Overflow
        {
            get => GetAttribute<SvgOverflow>("overflow", false);
            set => Attributes["overflow"] = value;
        }

        [SvgAttribute("preserveAspectRatio")]
        public SvgAspectRatio AspectRatio
        {
            get => GetAttribute<SvgAspectRatio>("preserveAspectRatio", false, new SvgAspectRatio(SvgPreserveAspectRatio.xMidYMid));
            set => Attributes["preserveAspectRatio"] = value;
        }

        [SvgAttribute("patternTransform")]
        public SvgTransformCollection PatternTransform
        {
            get => GetAttribute<SvgTransformCollection>("patternTransform", false);
            set => Attributes["patternTransform"] = value;
        }

        private Matrix EffectivePatternTransform
        {
            get
            {
                Matrix patternTransform = new Matrix();
                if (PatternTransform != null)
                {
                    using (Matrix matrix = PatternTransform.GetMatrix())
                    {
                        patternTransform.Multiply(matrix);
                    }
                }
                return patternTransform;
            }
        }

        public override Brush GetBrush(
          SvgVisualElement renderingElement,
          ISvgRenderer renderer,
          float opacity,
          bool forStroke = false)
        {
            List<SvgPatternServer> source = new List<SvgPatternServer>();
            SvgPatternServer svgPatternServer1 = this;
            do
            {
                source.Add(svgPatternServer1);
                svgPatternServer1 = SvgDeferredPaintServer.TryGet<SvgPatternServer>(svgPatternServer1.InheritGradient, renderingElement);
            }
            while (svgPatternServer1 != null);
            SvgPatternServer svgPatternServer2 = source.Where<SvgPatternServer>(p => p.Children.Count > 0).FirstOrDefault<SvgPatternServer>();
            if (svgPatternServer2 == null)
            {
                return null;
            }

            SvgPatternServer svgPatternServer3 = source.Where<SvgPatternServer>(p =>
      {
          SvgUnit x = p.X;
          return p.X != SvgUnit.None;
      }).FirstOrDefault<SvgPatternServer>();
            SvgPatternServer svgPatternServer4 = source.Where<SvgPatternServer>(p =>
            {
                SvgUnit y = p.Y;
                return p.Y != SvgUnit.None;
            }).FirstOrDefault<SvgPatternServer>();
            SvgPatternServer svgPatternServer5 = source.Where<SvgPatternServer>(p =>
            {
                SvgUnit width = p.Width;
                return p.Width != SvgUnit.None;
            }).FirstOrDefault<SvgPatternServer>();
            SvgPatternServer svgPatternServer6 = source.Where<SvgPatternServer>(p =>
            {
                SvgUnit height = p.Height;
                return p.Height != SvgUnit.None;
            }).FirstOrDefault<SvgPatternServer>();
            if (svgPatternServer5 == null || svgPatternServer6 == null)
            {
                return null;
            }

            SvgPatternServer svgPatternServer7 = source.Where<SvgPatternServer>(p => p.PatternUnits != 0).FirstOrDefault<SvgPatternServer>();
            SvgPatternServer svgPatternServer8 = source.Where<SvgPatternServer>(p => p.PatternContentUnits != 0).FirstOrDefault<SvgPatternServer>();
            SvgPatternServer svgPatternServer9 = source.Where<SvgPatternServer>(p =>
            {
                SvgViewBox viewBox = p.ViewBox;
                return p.ViewBox != SvgViewBox.Empty;
            }).FirstOrDefault<SvgPatternServer>();
            SvgUnit svgUnit1 = svgPatternServer3 == null ? new SvgUnit(0.0f) : svgPatternServer3.X;
            SvgUnit svgUnit2 = svgPatternServer4 == null ? new SvgUnit(0.0f) : svgPatternServer4.Y;
            SvgUnit width1 = svgPatternServer5.Width;
            SvgUnit height1 = svgPatternServer6.Height;
            var num = svgPatternServer7 == null ? 1 : (int)svgPatternServer7.PatternUnits;
            SvgCoordinateUnits svgCoordinateUnits = svgPatternServer8 == null ? SvgCoordinateUnits.UserSpaceOnUse : svgPatternServer8.PatternContentUnits;
            SvgViewBox svgViewBox = svgPatternServer9 == null ? SvgViewBox.Empty : svgPatternServer9.ViewBox;
            var flag = num == 1;
            try
            {
                if (flag)
                {
                    renderer.SetBoundable(renderingElement);
                }

                var deviceValue1 = svgUnit1.ToDeviceValue(renderer, UnitRenderingType.Horizontal, this);
                var deviceValue2 = svgUnit2.ToDeviceValue(renderer, UnitRenderingType.Vertical, this);
                var deviceValue3 = width1.ToDeviceValue(renderer, UnitRenderingType.Horizontal, this);
                var deviceValue4 = height1.ToDeviceValue(renderer, UnitRenderingType.Vertical, this);
                if (flag)
                {
                    RectangleF bounds = renderer.GetBoundable().Bounds;
                    if (svgUnit1.Type != SvgUnitType.Percentage)
                    {
                        deviceValue1 *= bounds.Width;
                    }

                    if (svgUnit2.Type != SvgUnitType.Percentage)
                    {
                        deviceValue2 *= bounds.Height;
                    }

                    if (width1.Type != SvgUnitType.Percentage)
                    {
                        deviceValue3 *= bounds.Width;
                    }

                    if (height1.Type != SvgUnitType.Percentage)
                    {
                        deviceValue4 *= bounds.Height;
                    }

                    deviceValue1 += bounds.X;
                    deviceValue2 += bounds.Y;
                }
                Bitmap bitmap = new Bitmap((int)Math.Ceiling((double)deviceValue3), (int)Math.Ceiling((double)deviceValue4));
                using (ISvgRenderer renderer1 = SvgRenderer.FromImage(bitmap))
                {
                    renderer1.SetBoundable(renderingElement);
                    if (svgViewBox != SvgViewBox.Empty)
                    {
                        RectangleF bounds = renderer1.GetBoundable().Bounds;
                        renderer1.ScaleTransform(deviceValue3 / svgViewBox.Width, deviceValue4 / svgViewBox.Height);
                    }
                    else if (svgCoordinateUnits == SvgCoordinateUnits.ObjectBoundingBox)
                    {
                        RectangleF bounds = renderer1.GetBoundable().Bounds;
                        renderer1.ScaleTransform(bounds.Width, bounds.Height);
                    }
                    foreach (SvgElement child in svgPatternServer2.Children)
                    {
                        child.RenderElement(renderer1);
                    }
                }
                using (Matrix patternTransform = EffectivePatternTransform)
                {
                    TextureBrush brush = new TextureBrush(bitmap, new RectangleF(0.0f, 0.0f, deviceValue3, deviceValue4))
                    {
                        Transform = patternTransform
                    };
                    brush.TranslateTransform(deviceValue1, deviceValue2);
                    return brush;
                }
            }
            finally
            {
                if (flag)
                {
                    renderer.PopBoundable();
                }
            }
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgPatternServer>();
        }

        public override SvgElement DeepCopy<T>()
        {
            SvgPatternServer svgPatternServer = base.DeepCopy<T>() as SvgPatternServer;
            svgPatternServer.X = X;
            svgPatternServer.Y = Y;
            svgPatternServer.Width = Width;
            svgPatternServer.Height = Height;
            svgPatternServer.ViewBox = ViewBox;
            svgPatternServer.Overflow = Overflow;
            svgPatternServer.AspectRatio = AspectRatio;
            return svgPatternServer;
        }
    }
}
