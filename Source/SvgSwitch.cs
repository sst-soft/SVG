// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.Drawing;
using System.Drawing.Drawing2D;

namespace Svg
{
    [SvgElement("switch")]
    public class SvgSwitch : SvgVisualElement
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

        protected override void Render(ISvgRenderer renderer)
        {
            if (!Visible)
            {
                return;
            }

            if (!Displayable)
            {
                return;
            }

            try
            {
                if (!PushTransforms(renderer))
                {
                    return;
                }

                SetClip(renderer);
                RenderChildren(renderer);
                ResetClip(renderer);
            }
            finally
            {
                PopTransforms(renderer);
            }
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgSwitch>();
        }

        public override SvgElement DeepCopy<T>()
        {
            SvgSwitch svgSwitch = base.DeepCopy<T>() as SvgSwitch;
            if (Fill != null)
            {
                svgSwitch.Fill = Fill.DeepCopy() as SvgPaintServer;
            }

            return svgSwitch;
        }
    }
}
