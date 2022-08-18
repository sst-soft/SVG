// todo: add license

using System.Drawing;
using System.Drawing.Drawing2D;

namespace Svg
{
    [SvgElement("foreignObject")]
    public class SvgForeignObject : SvgVisualElement
    {
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

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgForeignObject>();
        }

        public override SvgElement DeepCopy<T>()
        {
            SvgForeignObject svgForeignObject = base.DeepCopy<T>() as SvgForeignObject;
            if (Fill != null)
            {
                svgForeignObject.Fill = Fill.DeepCopy() as SvgPaintServer;
            }

            return svgForeignObject;
        }
    }
}
