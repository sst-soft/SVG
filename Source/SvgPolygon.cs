// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Svg
{
    [SvgElement("polygon")]
    public class SvgPolygon : SvgMarkerElement
    {
        private GraphicsPath _path;

        [SvgAttribute("points")]
        public SvgPointCollection Points
        {
            get => GetAttribute<SvgPointCollection>("points", false);
            set
            {
                Attributes["points"] = value;
                IsPathDirty = true;
            }
        }

        public override GraphicsPath Path(ISvgRenderer renderer)
        {
            if (_path == null || IsPathDirty)
            {
                _path = new GraphicsPath();
                _path.StartFigure();
                try
                {
                    SvgPointCollection points = Points;
                    for (var index = 2; index + 1 < points.Count; index += 2)
                    {
                        PointF devicePoint = SvgUnit.GetDevicePoint(points[index], points[index + 1], renderer, this);
                        if (renderer == null)
                        {
                            var num = (float)StrokeWidth * 2f;
                            _path.AddEllipse(devicePoint.X - num, devicePoint.Y - num, 2f * num, 2f * num);
                        }
                        else if (_path.PointCount == 0)
                        {
                            _path.AddLine(SvgUnit.GetDevicePoint(points[index - 2], points[index - 1], renderer, this), devicePoint);
                        }
                        else
                        {
                            _path.AddLine(_path.GetLastPoint(), devicePoint);
                        }
                    }
                }
                catch
                {
                    Trace.TraceError("Error parsing points");
                }
                _path.CloseFigure();
                if (renderer != null)
                {
                    IsPathDirty = false;
                }
            }
            return _path;
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgPolygon>();
        }

        public override SvgElement DeepCopy<T>()
        {
            SvgPolygon svgPolygon = base.DeepCopy<T>() as SvgPolygon;
            if (Points != null)
            {
                svgPolygon.Points = new SvgPointCollection();
                foreach (SvgUnit point in (List<SvgUnit>)Points)
                {
                    svgPolygon.Points.Add(point);
                }
            }
            return svgPolygon;
        }
    }
}
