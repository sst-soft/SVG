// todo: add license

using System.Drawing;
using System.Drawing.Drawing2D;

namespace Svg
{
    [SvgElement("g")]
    public class SvgGroup : SvgMarkerElement
    {
        private bool markersSet;

        private void AddMarkers()
        {
            if (markersSet)
            {
                return;
            }

            if (MarkerStart != null || MarkerMid != null || MarkerEnd != null)
            {
                foreach (SvgElement child in Children)
                {
                    if (child is SvgMarkerElement)
                    {
                        if (MarkerStart != null && ((SvgMarkerElement)child).MarkerStart == null)
                        {
                            ((SvgMarkerElement)child).MarkerStart = MarkerStart;
                        }

                        if (MarkerMid != null && ((SvgMarkerElement)child).MarkerMid == null)
                        {
                            ((SvgMarkerElement)child).MarkerMid = MarkerMid;
                        }

                        if (MarkerEnd != null && ((SvgMarkerElement)child).MarkerEnd == null)
                        {
                            ((SvgMarkerElement)child).MarkerEnd = MarkerEnd;
                        }
                    }
                }
            }
            markersSet = true;
        }

        protected override void Render(ISvgRenderer renderer)
        {
            AddMarkers();
            base.Render(renderer);
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

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgGroup>();
        }

        public override SvgElement DeepCopy<T>()
        {
            SvgGroup svgGroup = base.DeepCopy<T>() as SvgGroup;
            if (Fill != null)
            {
                svgGroup.Fill = Fill.DeepCopy() as SvgPaintServer;
            }

            return svgGroup;
        }
    }
}
