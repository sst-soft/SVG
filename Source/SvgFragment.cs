// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;

namespace Svg
{
    [SvgElement("svg")]
    public class SvgFragment : SvgElement, ISvgViewPort, ISvgBoundable
    {
        public static readonly Uri Namespace = new Uri("http://www.w3.org/2000/svg");
        private SvgUnit _x = (SvgUnit)0.0f;
        private SvgUnit _y = (SvgUnit)0.0f;

        PointF ISvgBoundable.Location => PointF.Empty;

        SizeF ISvgBoundable.Size => GetDimensions();

        RectangleF ISvgBoundable.Bounds => new RectangleF(((ISvgBoundable)this).Location, ((ISvgBoundable)this).Size);

        [SvgAttribute("x")]
        public SvgUnit X
        {
            get => _x;
            set
            {
                if (_x != value)
                {
                    _x = value;
                }

                Attributes["x"] = value;
            }
        }

        [SvgAttribute("y")]
        public SvgUnit Y
        {
            get => _y;
            set
            {
                if (_y != value)
                {
                    _y = value;
                }

                Attributes["y"] = value;
            }
        }

        [SvgAttribute("width")]
        public SvgUnit Width
        {
            get => GetAttribute<SvgUnit>("width", false, new SvgUnit(SvgUnitType.Percentage, 100f));
            set => Attributes["width"] = value;
        }

        [SvgAttribute("height")]
        public SvgUnit Height
        {
            get => GetAttribute<SvgUnit>("height", false, new SvgUnit(SvgUnitType.Percentage, 100f));
            set => Attributes["height"] = value;
        }

        [SvgAttribute("overflow")]
        public virtual SvgOverflow Overflow
        {
            get => GetAttribute<SvgOverflow>("overflow", false);
            set => Attributes["overflow"] = value;
        }

        [SvgAttribute("viewBox")]
        public SvgViewBox ViewBox
        {
            get => GetAttribute<SvgViewBox>("viewBox", false, SvgViewBox.Empty);
            set => Attributes["viewBox"] = value;
        }

        [SvgAttribute("preserveAspectRatio")]
        public SvgAspectRatio AspectRatio
        {
            get => GetAttribute<SvgAspectRatio>("preserveAspectRatio", false, new SvgAspectRatio(SvgPreserveAspectRatio.xMidYMid));
            set => Attributes["preserveAspectRatio"] = value;
        }

        [SvgAttribute("font-size")]
        public override SvgUnit FontSize
        {
            get => GetAttribute<SvgUnit>("font-size", true, SvgUnit.Empty);
            set => Attributes["font-size"] = value;
        }

        [SvgAttribute("font-family")]
        public override string FontFamily
        {
            get => GetAttribute<string>("font-family", true);
            set => Attributes["font-family"] = value;
        }

        public override XmlSpaceHandling SpaceHandling
        {
            get => GetAttribute<XmlSpaceHandling>("space", true);
            set
            {
                base.SpaceHandling = value;
                IsPathDirty = true;
            }
        }

        protected internal override bool PushTransforms(ISvgRenderer renderer)
        {
            if (!base.PushTransforms(renderer))
            {
                return false;
            }

            ViewBox.AddViewBoxTransform(AspectRatio, renderer, this);
            return true;
        }

        protected override void Render(ISvgRenderer renderer)
        {
            switch (Overflow)
            {
                case SvgOverflow.Inherit:
                case SvgOverflow.Auto:
                case SvgOverflow.Visible:
                    base.Render(renderer);
                    break;
                default:
                    Region clip = renderer.GetClip();
                    try
                    {
                        SizeF sizeF = Parent == null ? renderer.GetBoundable().Bounds.Size : GetDimensions();
                        RectangleF rectangleF = new RectangleF(X.ToDeviceValue(renderer, UnitRenderingType.Horizontal, this), Y.ToDeviceValue(renderer, UnitRenderingType.Vertical, this), sizeF.Width, sizeF.Height);
                        renderer.SetClip(new Region(rectangleF), (CombineMode)1);
                        base.Render(renderer);
                        break;
                    }
                    finally
                    {
                        renderer.SetClip(clip);
                    }
            }
        }

        public GraphicsPath Path
        {
            get
            {
                GraphicsPath path = new GraphicsPath();
                AddPaths(this, path);
                return path;
            }
        }

        public RectangleF Bounds
        {
            get
            {
                RectangleF rectangleF = new RectangleF();
                foreach (SvgElement child in Children)
                {
                    RectangleF b = new RectangleF();
                    if (child is SvgFragment)
                    {
                        b = ((SvgFragment)child).Bounds;
                        b.Offset((float)((SvgFragment)child).X, (float)((SvgFragment)child).Y);
                    }
                    else if (child is SvgVisualElement)
                    {
                        b = ((SvgVisualElement)child).Bounds;
                    }

                    if (!b.IsEmpty)
                    {
                        rectangleF = !rectangleF.IsEmpty ? RectangleF.Union(rectangleF, b) : b;
                    }
                }
                return TransformedBounds(rectangleF);
            }
        }

        public SizeF GetDimensions()
        {
            SvgUnit svgUnit = Width;
            var num1 = svgUnit.Type == SvgUnitType.Percentage ? 1 : 0;
            svgUnit = Height;
            var flag = svgUnit.Type == SvgUnitType.Percentage;
            RectangleF rectangleF = new RectangleF();
            if ((num1 | (flag ? 1 : 0)) != 0)
            {
                SvgViewBox viewBox = ViewBox;
                if ((double)viewBox.Width > 0.0)
                {
                    viewBox = ViewBox;
                    if ((double)viewBox.Height > 0.0)
                    {
                        ref RectangleF local = ref rectangleF;
                        viewBox = ViewBox;
                        var minX = (double)viewBox.MinX;
                        viewBox = ViewBox;
                        var minY = (double)viewBox.MinY;
                        viewBox = ViewBox;
                        var width = (double)viewBox.Width;
                        viewBox = ViewBox;
                        var height = (double)viewBox.Height;
                        local = new RectangleF((float)minX, (float)minY, (float)width, (float)height);
                        goto label_5;
                    }
                }
                rectangleF = Bounds;
            }
        label_5:
            float width1;
            if (num1 != 0)
            {
                var num2 = (double)rectangleF.Width + (double)rectangleF.X;
                svgUnit = Width;
                var num3 = (double)svgUnit.Value * 0.0099999997764825821;
                width1 = (float)(num2 * num3);
            }
            else
            {
                svgUnit = Width;
                width1 = svgUnit.ToDeviceValue(null, UnitRenderingType.Horizontal, this);
            }
            float height1;
            if (flag)
            {
                var num4 = (double)rectangleF.Height + (double)rectangleF.Y;
                svgUnit = Height;
                var num5 = (double)svgUnit.Value * 0.0099999997764825821;
                height1 = (float)(num4 * num5);
            }
            else
            {
                svgUnit = Height;
                height1 = svgUnit.ToDeviceValue(null, UnitRenderingType.Vertical, this);
            }
            return new SizeF(width1, height1);
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgFragment>();
        }

        public override SvgElement DeepCopy<T>()
        {
            SvgFragment svgFragment = base.DeepCopy<T>() as SvgFragment;
            svgFragment.Height = Height;
            svgFragment.Width = Width;
            svgFragment.Overflow = Overflow;
            svgFragment.ViewBox = ViewBox;
            svgFragment.AspectRatio = AspectRatio;
            return svgFragment;
        }

        protected override void WriteStartElement(XmlTextWriter writer)
        {
            base.WriteStartElement(writer);
            foreach (KeyValuePair<string, string> keyValuePair in SvgAttributeAttribute.Namespaces)
            {
                if (string.IsNullOrEmpty(keyValuePair.Key))
                {
                    writer.WriteAttributeString("xmlns", keyValuePair.Value);
                }
                else
                {
                    writer.WriteAttributeString("xmlns:" + keyValuePair.Key, keyValuePair.Value);
                }
            }
            writer.WriteAttributeString("version", "1.1");
        }
    }
}
