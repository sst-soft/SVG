// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.Drawing;
using System.Drawing.Drawing2D;
using Svg.ExtensionMethods;

namespace Svg
{
    public abstract class SvgMarkerElement : SvgPathBasedElement
    {
        [SvgAttribute("marker-end")]
        public Uri MarkerEnd
        {
            get => GetAttribute<Uri>("marker-end", false).ReplaceWithNullIfNone();
            set => Attributes["marker-end"] = value;
        }

        [SvgAttribute("marker-mid")]
        public Uri MarkerMid
        {
            get => GetAttribute<Uri>("marker-mid", false).ReplaceWithNullIfNone();
            set => Attributes["marker-mid"] = value;
        }

        [SvgAttribute("marker-start")]
        public Uri MarkerStart
        {
            get => GetAttribute<Uri>("marker-start", false).ReplaceWithNullIfNone();
            set => Attributes["marker-start"] = value;
        }

        protected internal override bool RenderStroke(ISvgRenderer renderer)
        {
            var flag = base.RenderStroke(renderer);
            GraphicsPath graphicsPath = Path(renderer);
            var length = graphicsPath.PathPoints.Length;
            if (MarkerStart != null)
            {
                PointF pathPoint1 = graphicsPath.PathPoints[0];
                var index = 1;
                while (index < length && graphicsPath.PathPoints[index] == pathPoint1)
                {
                    ++index;
                }

                PointF pathPoint2 = graphicsPath.PathPoints[index];
                OwnerDocument.GetElementById<SvgMarker>(MarkerStart.ToString()).RenderMarker(renderer, this, pathPoint1, pathPoint1, pathPoint2, true);
            }
            if (MarkerMid != null)
            {
                SvgMarker elementById = OwnerDocument.GetElementById<SvgMarker>(MarkerMid.ToString());
                var num = -1;
                for (var index = 1; index <= graphicsPath.PathPoints.Length - 2; ++index)
                {
                    num = (graphicsPath.PathTypes[index] & 7) != 3 ? -1 : (num + 1) % 3;
                    if (num == -1 || num == 2)
                    {
                        elementById.RenderMarker(renderer, this, graphicsPath.PathPoints[index], graphicsPath.PathPoints[index - 1], graphicsPath.PathPoints[index], graphicsPath.PathPoints[index + 1]);
                    }
                }
            }
            if (MarkerEnd != null)
            {
                var index1 = length - 1;
                PointF pathPoint3 = graphicsPath.PathPoints[index1];
                var index2 = index1 - 1;
                while (index2 > 0 && graphicsPath.PathPoints[index2] == pathPoint3)
                {
                    --index2;
                }

                PointF pathPoint4 = graphicsPath.PathPoints[index2];
                OwnerDocument.GetElementById<SvgMarker>(MarkerEnd.ToString()).RenderMarker(renderer, this, pathPoint3, pathPoint4, graphicsPath.PathPoints[graphicsPath.PathPoints.Length - 1], false);
            }
            return flag;
        }
    }
}
