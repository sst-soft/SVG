// todo: add license

using System.ComponentModel;
using System.Globalization;

namespace Svg
{
    public class SvgUnitConverter : TypeConverter
    {
        public override object ConvertFrom(
          ITypeDescriptorContext context,
          CultureInfo culture,
          object value)
        {
            if (value == null)
            {
                return new SvgUnit(SvgUnitType.User, 0.0f);
            }

            var str = value is string ? (string)value : throw new ArgumentOutOfRangeException("value must be a string.");
            var num = -1;
            if (str == "none")
            {
                return SvgUnit.None;
            }

            if (str == "medium")
            {
                str = "1em";
            }
            else if (str == "small")
            {
                str = "0.8em";
            }
            else if (str == "x-small")
            {
                str = "0.7em";
            }
            else if (str == "xx-small")
            {
                str = "0.6em";
            }
            else if (str == "large")
            {
                str = "1.2em";
            }
            else if (str == "x-large")
            {
                str = "1.4em";
            }
            else if (str == "xx-large")
            {
                str = "1.7em";
            }

            for (var index = 0; index < str.Length; ++index)
            {
                if (str[index] == '%' || char.IsLetter(str[index]) && (str[index] != 'e' && str[index] != 'E' || index >= str.Length - 1 || char.IsLetter(str[index + 1])))
                {
                    num = index;
                    break;
                }
            }
            float.TryParse(num > -1 ? str.Substring(0, num) : str, NumberStyles.Float, CultureInfo.InvariantCulture, out var result);
            if (num == -1)
            {
                return new SvgUnit(result);
            }

            switch (str.Substring(num).Trim().ToLower())
            {
                case "%":
                    return new SvgUnit(SvgUnitType.Percentage, result);
                case "cm":
                    return new SvgUnit(SvgUnitType.Centimeter, result);
                case "em":
                    return new SvgUnit(SvgUnitType.Em, result);
                case "ex":
                    return new SvgUnit(SvgUnitType.Ex, result);
                case "in":
                    return new SvgUnit(SvgUnitType.Inch, result);
                case "mm":
                    return new SvgUnit(SvgUnitType.Millimeter, result);
                case "pc":
                    return new SvgUnit(SvgUnitType.Pica, result);
                case "pt":
                    return new SvgUnit(SvgUnitType.Point, result);
                case "px":
                    return new SvgUnit(SvgUnitType.Pixel, result);
                default:
                    throw new FormatException("Unit is in an invalid format '" + str + "'.");
            }
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value,
      Type destinationType)
        {
            return destinationType == typeof(string) ? ((SvgUnit)value).ToString() : base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
