// todo: add license

using System.Drawing;
using System.Drawing.Drawing2D;
using Svg.Pathing;

namespace Svg
{
    [SvgElement("path")]
    public class SvgPath : SvgMarkerElement, ISvgPathElement
    {
        private GraphicsPath _path;

        [SvgAttribute("d")]
        public SvgPathSegmentList PathData
        {
            get => GetAttribute<SvgPathSegmentList>("d", false);
            set
            {
                SvgPathSegmentList pathData = PathData;
                if (pathData != null)
                {
                    pathData.Owner = null;
                }

                Attributes["d"] = value;
                value.Owner = this;
                IsPathDirty = true;
            }
        }

        [SvgAttribute("pathLength")]
        public float PathLength
        {
            get => GetAttribute<float>("pathLength", false);
            set => Attributes["pathLength"] = value;
        }

        public override GraphicsPath Path(ISvgRenderer renderer)
        {
            if (_path == null || IsPathDirty)
            {
                _path = new GraphicsPath();
                if (PathData != null)
                {
                    foreach (SvgPathSegment svgPathSegment in PathData)
                    {
                        svgPathSegment.AddToPath(_path);
                    }

                    if (_path.PointCount == 0)
                    {
                        if (PathData.Count > 0)
                        {
                            SvgPathSegment last = PathData.Last;
                            _path.AddLine(last.End, last.End);
                            Fill = SvgPaintServer.None;
                        }
                        else
                        {
                            _path = null;
                        }
                    }
                    else if (renderer == null)
                    {
                        var num = (float)StrokeWidth * 2f;
                        RectangleF bounds = _path.GetBounds();
                        _path.AddEllipse(bounds.Left - num, bounds.Top - num, 2f * num, 2f * num);
                        _path.AddEllipse(bounds.Right - num, bounds.Bottom - num, 2f * num, 2f * num);
                    }
                }
                if (renderer != null)
                {
                    IsPathDirty = false;
                }
            }
            return _path;
        }

        public void OnPathUpdated()
        {
            IsPathDirty = true;
            OnAttributeChanged(new AttributeEventArgs()
            {
                Attribute = "d",
                Value = Attributes.GetAttribute<SvgPathSegmentList>("d")
            });
        }

        public override RectangleF Bounds => Path(null).GetBounds();

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgPath>();
        }

        public override SvgElement DeepCopy<T>()
        {
            SvgPath svgPath = base.DeepCopy<T>() as SvgPath;
            if (PathData != null)
            {
                SvgPathSegmentList svgPathSegmentList = new SvgPathSegmentList();
                foreach (SvgPathSegment svgPathSegment in PathData)
                {
                    svgPathSegmentList.Add(svgPathSegment.Clone());
                }

                svgPath.PathData = svgPathSegmentList;
            }
            svgPath.PathLength = PathLength;
            svgPath.MarkerStart = MarkerStart;
            svgPath.MarkerEnd = MarkerEnd;
            return svgPath;
        }
    }
}
