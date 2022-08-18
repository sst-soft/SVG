// todo: add license

using System.Drawing;
using System.Drawing.Drawing2D;

namespace Svg
{
    [SvgElement("line")]
    public class SvgLine : SvgMarkerElement
    {
        private SvgUnit _startX = (SvgUnit)0.0f;
        private SvgUnit _startY = (SvgUnit)0.0f;
        private SvgUnit _endX = (SvgUnit)0.0f;
        private SvgUnit _endY = (SvgUnit)0.0f;
        private GraphicsPath _path;

        [SvgAttribute("x1")]
        public SvgUnit StartX
        {
            get => _startX;
            set
            {
                if (_startX != value)
                {
                    _startX = value;
                    IsPathDirty = true;
                }
                Attributes["x1"] = value;
            }
        }

        [SvgAttribute("y1")]
        public SvgUnit StartY
        {
            get => _startY;
            set
            {
                if (_startY != value)
                {
                    _startY = value;
                    IsPathDirty = true;
                }
                Attributes["y1"] = value;
            }
        }

        [SvgAttribute("x2")]
        public SvgUnit EndX
        {
            get => _endX;
            set
            {
                if (_endX != value)
                {
                    _endX = value;
                    IsPathDirty = true;
                }
                Attributes["x2"] = value;
            }
        }

        [SvgAttribute("y2")]
        public SvgUnit EndY
        {
            get => _endY;
            set
            {
                if (_endY != value)
                {
                    _endY = value;
                    IsPathDirty = true;
                }
                Attributes["y2"] = value;
            }
        }

        public override SvgPaintServer Fill
        {
            get => null;
            set
            {
            }
        }

        public override GraphicsPath Path(ISvgRenderer renderer)
        {
            if ((_path == null || IsPathDirty) && (double)(float)StrokeWidth > 0.0)
            {
                PointF pointF1 = new PointF(StartX.ToDeviceValue(renderer, UnitRenderingType.Horizontal, this), StartY.ToDeviceValue(renderer, UnitRenderingType.Vertical, this));
                PointF pointF2 = new PointF(EndX.ToDeviceValue(renderer, UnitRenderingType.Horizontal, this), EndY.ToDeviceValue(renderer, UnitRenderingType.Vertical, this));
                _path = new GraphicsPath();
                if (renderer != null)
                {
                    _path.AddLine(pointF1, pointF2);
                    IsPathDirty = false;
                }
                else
                {
                    _path.StartFigure();
                    var num = (float)StrokeWidth / 2f;
                    _path.AddEllipse(pointF1.X - num, pointF1.Y - num, 2f * num, 2f * num);
                    _path.AddEllipse(pointF2.X - num, pointF2.Y - num, 2f * num, 2f * num);
                    _path.CloseFigure();
                }
            }
            return _path;
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgLine>();
        }

        public override SvgElement DeepCopy<T>()
        {
            SvgLine svgLine = base.DeepCopy<T>() as SvgLine;
            svgLine.StartX = StartX;
            svgLine.EndX = EndX;
            svgLine.StartY = StartY;
            svgLine.EndY = EndY;
            if (Fill != null)
            {
                svgLine.Fill = Fill.DeepCopy() as SvgPaintServer;
            }

            return svgLine;
        }
    }
}
