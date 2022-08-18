// todo: add license

using System.Drawing;
using System.Drawing.Drawing2D;

namespace Svg
{
    [SvgElement("rect")]
    public class SvgRectangle : SvgPathBasedElement
    {
        private SvgUnit _x = (SvgUnit)0.0f;
        private SvgUnit _y = (SvgUnit)0.0f;
        private SvgUnit _width = (SvgUnit)0.0f;
        private SvgUnit _height = (SvgUnit)0.0f;
        private SvgUnit _cornerRadiusX = (SvgUnit)0.0f;
        private SvgUnit _cornerRadiusY = (SvgUnit)0.0f;
        private GraphicsPath _path;

        public SvgPoint Location => new SvgPoint(X, Y);

        [SvgAttribute("x")]
        public SvgUnit X
        {
            get => _x;
            set
            {
                _x = value;
                Attributes["x"] = value;
                IsPathDirty = true;
            }
        }

        [SvgAttribute("y")]
        public SvgUnit Y
        {
            get => _y;
            set
            {
                _y = value;
                Attributes["y"] = value;
                IsPathDirty = true;
            }
        }

        [SvgAttribute("width")]
        public SvgUnit Width
        {
            get => _width;
            set
            {
                _width = value;
                Attributes["width"] = value;
                IsPathDirty = true;
            }
        }

        [SvgAttribute("height")]
        public SvgUnit Height
        {
            get => _height;
            set
            {
                _height = value;
                Attributes["height"] = value;
                IsPathDirty = true;
            }
        }

        [SvgAttribute("rx")]
        public SvgUnit CornerRadiusX
        {
            get => (double)_cornerRadiusX.Value != 0.0 || (double)_cornerRadiusY.Value <= 0.0 ? _cornerRadiusX : _cornerRadiusY;
            set
            {
                _cornerRadiusX = value;
                Attributes["rx"] = value;
                IsPathDirty = true;
            }
        }

        [SvgAttribute("ry")]
        public SvgUnit CornerRadiusY
        {
            get => (double)_cornerRadiusY.Value != 0.0 || (double)_cornerRadiusX.Value <= 0.0 ? _cornerRadiusY : _cornerRadiusX;
            set
            {
                _cornerRadiusY = value;
                Attributes["ry"] = value;
                IsPathDirty = true;
            }
        }

        protected override bool RequiresSmoothRendering
        {
            get
            {
                if (!base.RequiresSmoothRendering)
                {
                    return false;
                }

                return (double)CornerRadiusX.Value > 0.0 || (double)CornerRadiusY.Value > 0.0;
            }
        }

        public override GraphicsPath Path(ISvgRenderer renderer)
        {
            if (_path == null || IsPathDirty)
            {
                var num1 = (float)StrokeWidth / 2f;
                if (renderer != null)
                {
                    num1 = 0.0f;
                    IsPathDirty = false;
                }
                if (renderer == null || (double)CornerRadiusX.Value == 0.0 && (double)CornerRadiusY.Value == 0.0)
                {
                    var deviceValue = Location.Y.ToDeviceValue(renderer, UnitRenderingType.Vertical, this);
                    SvgUnit svgUnit = Location.X;
                    SvgPoint svgPoint = new SvgPoint((SvgUnit)(svgUnit.ToDeviceValue(renderer, UnitRenderingType.Horizontal, this) - num1), (SvgUnit)(deviceValue - num1));
                    svgUnit = Width;
                    var width = svgUnit.ToDeviceValue(renderer, UnitRenderingType.Horizontal, this) + num1 * 2f;
                    svgUnit = Height;
                    var height = svgUnit.ToDeviceValue(renderer, UnitRenderingType.Vertical, this) + num1 * 2f;
                    RectangleF rectangleF = new RectangleF(svgPoint.ToDeviceValue(renderer, this), new SizeF(width, height));
                    _path = new GraphicsPath();
                    _path.StartFigure();
                    _path.AddRectangle(rectangleF);
                    _path.CloseFigure();
                }
                else
                {
                    _path = new GraphicsPath();
                    RectangleF rectangleF = new RectangleF();
                    PointF pointF1 = new PointF();
                    PointF pointF2 = new PointF();
                    var deviceValue1 = Width.ToDeviceValue(renderer, UnitRenderingType.Horizontal, this);
                    var deviceValue2 = Height.ToDeviceValue(renderer, UnitRenderingType.Vertical, this);
                    SvgUnit svgUnit = CornerRadiusX;
                    var num2 = Math.Min(svgUnit.ToDeviceValue(renderer, UnitRenderingType.Horizontal, this) * 2f, deviceValue1);
                    svgUnit = CornerRadiusY;
                    var num3 = Math.Min(svgUnit.ToDeviceValue(renderer, UnitRenderingType.Vertical, this) * 2f, deviceValue2);
                    PointF deviceValue3 = Location.ToDeviceValue(renderer, this);
                    _path.StartFigure();
                    rectangleF.Location = deviceValue3;
                    rectangleF.Width = num2;
                    rectangleF.Height = num3;
                    _path.AddArc(rectangleF, 180f, 90f);
                    pointF1.X = Math.Min(deviceValue3.X + num2, deviceValue3.X + deviceValue1 * 0.5f);
                    pointF1.Y = deviceValue3.Y;
                    pointF2.X = Math.Max(deviceValue3.X + deviceValue1 - num2, deviceValue3.X + deviceValue1 * 0.5f);
                    pointF2.Y = pointF1.Y;
                    _path.AddLine(pointF1, pointF2);
                    rectangleF.Location = new PointF(deviceValue3.X + deviceValue1 - num2, deviceValue3.Y);
                    _path.AddArc(rectangleF, 270f, 90f);
                    pointF1.X = deviceValue3.X + deviceValue1;
                    pointF1.Y = Math.Min(deviceValue3.Y + num3, deviceValue3.Y + deviceValue2 * 0.5f);
                    pointF2.X = pointF1.X;
                    pointF2.Y = Math.Max(deviceValue3.Y + deviceValue2 - num3, deviceValue3.Y + deviceValue2 * 0.5f);
                    _path.AddLine(pointF1, pointF2);
                    rectangleF.Location = new PointF(deviceValue3.X + deviceValue1 - num2, deviceValue3.Y + deviceValue2 - num3);
                    _path.AddArc(rectangleF, 0.0f, 90f);
                    pointF1.X = Math.Max(deviceValue3.X + deviceValue1 - num2, deviceValue3.X + deviceValue1 * 0.5f);
                    pointF1.Y = deviceValue3.Y + deviceValue2;
                    pointF2.X = Math.Min(deviceValue3.X + num2, deviceValue3.X + deviceValue1 * 0.5f);
                    pointF2.Y = pointF1.Y;
                    _path.AddLine(pointF1, pointF2);
                    rectangleF.Location = new PointF(deviceValue3.X, deviceValue3.Y + deviceValue2 - num3);
                    _path.AddArc(rectangleF, 90f, 90f);
                    pointF1.X = deviceValue3.X;
                    pointF1.Y = Math.Max(deviceValue3.Y + deviceValue2 - num3, deviceValue3.Y + deviceValue2 * 0.5f);
                    pointF2.X = pointF1.X;
                    pointF2.Y = Math.Min(deviceValue3.Y + num3, deviceValue3.Y + deviceValue2 * 0.5f);
                    _path.AddLine(pointF1, pointF2);
                    _path.CloseFigure();
                }
            }
            return _path;
        }

        protected override void Render(ISvgRenderer renderer)
        {
            if ((double)Width.Value <= 0.0 || (double)Height.Value <= 0.0)
            {
                return;
            }

            base.Render(renderer);
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgRectangle>();
        }

        public override SvgElement DeepCopy<T>()
        {
            SvgRectangle svgRectangle = base.DeepCopy<T>() as SvgRectangle;
            svgRectangle.CornerRadiusX = CornerRadiusX;
            svgRectangle.CornerRadiusY = CornerRadiusY;
            svgRectangle.Height = Height;
            svgRectangle.Width = Width;
            svgRectangle.X = X;
            svgRectangle.Y = Y;
            return svgRectangle;
        }
    }
}
