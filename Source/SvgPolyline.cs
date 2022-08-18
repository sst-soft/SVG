// todo: add license

using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Svg
{
    [SvgElement("polyline")]
    public class SvgPolyline : SvgPolygon
    {
        private GraphicsPath _Path;

        public override GraphicsPath Path(ISvgRenderer renderer)
        {
            if ((_Path == null || IsPathDirty) && (double)(float)StrokeWidth > 0.0)
            {
                _Path = new GraphicsPath();
                try
                {
                    for (var index = 0; index + 1 < Points.Count; index += 2)
                    {
                        PointF pointF = new PointF(Points[index].ToDeviceValue(renderer, UnitRenderingType.Horizontal, this), Points[index + 1].ToDeviceValue(renderer, UnitRenderingType.Vertical, this));
                        if (renderer == null)
                        {
                            var num = (float)StrokeWidth / 2f;
                            _Path.AddEllipse(pointF.X - num, pointF.Y - num, 2f * num, 2f * num);
                        }
                        else if (_Path.PointCount == 0)
                        {
                            _Path.AddLine(pointF, pointF);
                        }
                        else
                        {
                            _Path.AddLine(_Path.GetLastPoint(), pointF);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Error rendering points: " + ex.Message);
                }
                if (renderer != null)
                {
                    IsPathDirty = false;
                }
            }
            return _Path;
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgPolyline>();
        }
    }
}
