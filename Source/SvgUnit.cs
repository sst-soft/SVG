// todo: add license

using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace Svg
{
    [TypeConverter(typeof(SvgUnitConverter))]
    public struct SvgUnit
    {
        private readonly SvgUnitType _type;
        private readonly float _value;
        private bool _isEmpty;
        private float? _deviceValue;
        public static readonly SvgUnit Empty = new SvgUnit(SvgUnitType.User, 0.0f)
        {
            _isEmpty = true
        };
        public static readonly SvgUnit None = new SvgUnit(SvgUnitType.None, 0.0f);

        public bool IsEmpty => _isEmpty;

        public bool IsNone => _type == SvgUnitType.None;

        public float Value => _value;

        public SvgUnitType Type => _type;

        public float ToDeviceValue(
          ISvgRenderer renderer,
          UnitRenderingType renderType,
          SvgElement owner)
        {
            if (_deviceValue.HasValue)
            {
                return _deviceValue.Value;
            }

            if (_value == 0.0)
            {
                _deviceValue = new float?(0.0f);
                return _deviceValue.Value;
            }
            var pointsPerInch = SvgDocument.PointsPerInch;
            SvgUnitType type = Type;
            var num = Value;
            switch (type)
            {
                case SvgUnitType.Pixel:
                    _deviceValue = new float?(num);
                    break;
                case SvgUnitType.Em:
                    using (IFontDefn font = GetFont(renderer, owner))
                    {
                        _deviceValue = font != null ? new float?(num * (font.SizeInPoints / 72f) * pointsPerInch) : new float?(num * 9f / 72f * pointsPerInch);
                        break;
                    }
                case SvgUnitType.Ex:
                    using (IFontDefn font = GetFont(renderer, owner))
                    {
                        _deviceValue = font != null ? new float?((float)((double)num * 0.5 * ((double)font.SizeInPoints / 72.0)) * pointsPerInch) : new float?((float)((double)(num * 9f) * 0.5 / 72.0) * pointsPerInch);
                        break;
                    }
                case SvgUnitType.Percentage:
                    ISvgBoundable svgBoundable = renderer == null ? (owner == null ? null : (ISvgBoundable)owner.OwnerDocument) : renderer.GetBoundable();
                    if (svgBoundable == null)
                    {
                        _deviceValue = new float?(num);
                        break;
                    }
                    SizeF size = svgBoundable.Bounds.Size;
                    switch (renderType)
                    {
                        case UnitRenderingType.Other:
                            if (owner.OwnerDocument != null)
                            {
                                SvgViewBox viewBox = owner.OwnerDocument.ViewBox;
                                if ((double)owner.OwnerDocument.ViewBox.Width != 0.0 && (double)owner.OwnerDocument.ViewBox.Height != 0.0)
                                {
                                    _deviceValue = new float?((float)(Math.Sqrt(Math.Pow((double)owner.OwnerDocument.ViewBox.Width, 2.0) + Math.Pow((double)owner.OwnerDocument.ViewBox.Height, 2.0)) / Math.Sqrt(2.0) * (double)num / 100.0));
                                    break;
                                }
                            }
                            _deviceValue = new float?((float)(Math.Sqrt(Math.Pow((double)size.Width, 2.0) + Math.Pow((double)size.Height, 2.0)) / Math.Sqrt(2.0) * (double)num / 100.0));
                            break;
                        case UnitRenderingType.Horizontal:
                            _deviceValue = new float?(size.Width / 100f * num);
                            break;
                        case UnitRenderingType.HorizontalOffset:
                            _deviceValue = new float?(size.Width / 100f * num + svgBoundable.Location.X);
                            break;
                        case UnitRenderingType.Vertical:
                            _deviceValue = new float?(size.Height / 100f * num);
                            break;
                        case UnitRenderingType.VerticalOffset:
                            _deviceValue = new float?(size.Height / 100f * num + svgBoundable.Location.Y);
                            break;
                    }
                    break;
                case SvgUnitType.User:
                    _deviceValue = new float?(num);
                    break;
                case SvgUnitType.Inch:
                    _deviceValue = new float?(num * pointsPerInch);
                    break;
                case SvgUnitType.Centimeter:
                    _deviceValue = new float?(num / 2.54f * pointsPerInch);
                    break;
                case SvgUnitType.Millimeter:
                    _deviceValue = new float?((float)((double)num / 10.0 / 2.5399999618530273) * pointsPerInch);
                    break;
                case SvgUnitType.Pica:
                    _deviceValue = new float?((float)((double)num * 12.0 / 72.0) * pointsPerInch);
                    break;
                case SvgUnitType.Point:
                    _deviceValue = new float?(num / 72f * pointsPerInch);
                    break;
                default:
                    _deviceValue = new float?(num);
                    break;
            }
            return _deviceValue.Value;
        }

        private IFontDefn GetFont(ISvgRenderer renderer, SvgElement owner)
        {
            if (owner == null)
            {
                return null;
            }

            return owner.Parents.OfType<SvgVisualElement>().FirstOrDefault<SvgVisualElement>()?.GetFont(renderer);
        }

        public SvgUnit ToPercentage()
        {
            switch (Type)
            {
                case SvgUnitType.Percentage:
                    return this;
                case SvgUnitType.User:
                    return new SvgUnit(SvgUnitType.Percentage, Value * 100f);
                default:
                    throw new NotImplementedException();
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj.GetType() == typeof(SvgUnit)))
            {
                return false;
            }

            SvgUnit svgUnit = (SvgUnit)obj;
            return (double)svgUnit.Value == (double)Value && svgUnit.Type == Type;
        }

        public bool Equals(SvgUnit other)
        {
            return _type == other._type && _value == (double)other._value;
        }

        public override int GetHashCode()
        {
            return 0 + 1000000007 * _type.GetHashCode() + 1000000009 * _value.GetHashCode() + 1000000021 * _isEmpty.GetHashCode() + 1000000033 * _deviceValue.GetHashCode();
        }

        public static bool operator ==(SvgUnit lhs, SvgUnit rhs) => lhs.Equals(rhs);

        public static bool operator !=(SvgUnit lhs, SvgUnit rhs) => !(lhs == rhs);

        public override string ToString()
        {
            var str = string.Empty;
            switch (Type)
            {
                case SvgUnitType.None:
                    return "none";
                case SvgUnitType.Pixel:
                    str = "px";
                    break;
                case SvgUnitType.Em:
                    str = "em";
                    break;
                case SvgUnitType.Percentage:
                    str = "%";
                    break;
                case SvgUnitType.Inch:
                    str = "in";
                    break;
                case SvgUnitType.Centimeter:
                    str = "cm";
                    break;
                case SvgUnitType.Millimeter:
                    str = "mm";
                    break;
                case SvgUnitType.Point:
                    str = "pt";
                    break;
            }
            return Value.ToString(CultureInfo.InvariantCulture) + str;
        }

        public static implicit operator float(SvgUnit value) => value.ToDeviceValue(null, UnitRenderingType.Other, null);

        public static implicit operator SvgUnit(float value) => new SvgUnit(value);

        public SvgUnit(SvgUnitType type, float value)
        {
            _isEmpty = false;
            _type = type;
            _value = value;
            _deviceValue = new float?();
        }

        public SvgUnit(float value)
        {
            _isEmpty = false;
            _value = value;
            _type = SvgUnitType.User;
            _deviceValue = new float?();
        }

        public static PointF GetDevicePoint(
          SvgUnit x,
          SvgUnit y,
          ISvgRenderer renderer,
          SvgElement owner)
        {
            return new PointF(x.ToDeviceValue(renderer, UnitRenderingType.Horizontal, owner), y.ToDeviceValue(renderer, UnitRenderingType.Vertical, owner));
        }

        public static PointF GetDevicePointOffset(
          SvgUnit x,
          SvgUnit y,
          ISvgRenderer renderer,
          SvgElement owner)
        {
            return new PointF(x.ToDeviceValue(renderer, UnitRenderingType.HorizontalOffset, owner), y.ToDeviceValue(renderer, UnitRenderingType.VerticalOffset, owner));
        }

        public static SizeF GetDeviceSize(
          SvgUnit width,
          SvgUnit height,
          ISvgRenderer renderer,
          SvgElement owner)
        {
            return new SizeF(width.ToDeviceValue(renderer, UnitRenderingType.HorizontalOffset, owner), height.ToDeviceValue(renderer, UnitRenderingType.VerticalOffset, owner));
        }
    }
}
