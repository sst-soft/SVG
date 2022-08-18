// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.Drawing;
using System.Drawing.Drawing2D;

namespace Svg.Document_Structure
{
    [SvgElement("symbol")]
    public class SvgSymbol : SvgVisualElement
    {
        [SvgAttribute("viewBox")]
        public SvgViewBox ViewBox
        {
            get => GetAttribute<SvgViewBox>("viewBox", false);
            set => Attributes["viewBox"] = value;
        }

        [SvgAttribute("preserveAspectRatio")]
        public SvgAspectRatio AspectRatio
        {
            get => GetAttribute<SvgAspectRatio>("preserveAspectRatio", false);
            set => Attributes["preserveAspectRatio"] = value;
        }

        public override GraphicsPath Path(ISvgRenderer renderer)
        {
            return GetPaths(this, renderer);
        }

        public override RectangleF Bounds
        {
            get
            {
                RectangleF rectangleF = new RectangleF();
                foreach (SvgElement child in Children)
                {
                    if (child is SvgVisualElement)
                    {
                        if (rectangleF.IsEmpty)
                        {
                            rectangleF = ((SvgVisualElement)child).Bounds;
                        }
                        else
                        {
                            RectangleF bounds = ((SvgVisualElement)child).Bounds;
                            if (!bounds.IsEmpty)
                            {
                                rectangleF = RectangleF.Union(rectangleF, bounds);
                            }
                        }
                    }
                }
                return TransformedBounds(rectangleF);
            }
        }

        protected override bool Renderable => false;

        protected internal override bool PushTransforms(ISvgRenderer renderer)
        {
            if (!base.PushTransforms(renderer))
            {
                return false;
            }

            ViewBox.AddViewBoxTransform(AspectRatio, renderer, null);
            return true;
        }

        protected override void Render(ISvgRenderer renderer)
        {
            if (!(_parent is SvgUse))
            {
                return;
            }

            base.Render(renderer);
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgSymbol>();
        }

        public override SvgElement DeepCopy<T>()
        {
            SvgSymbol svgSymbol = base.DeepCopy<T>() as SvgSymbol;
            if (Fill != null)
            {
                svgSymbol.Fill = Fill.DeepCopy() as SvgPaintServer;
            }

            return svgSymbol;
        }
    }
}
