// todo: add license

using System.ComponentModel;
using System.Globalization;

namespace Svg
{
    internal class SvgViewBoxConverter : TypeConverter
    {
        public override object ConvertFrom(
          ITypeDescriptorContext context,
          CultureInfo culture,
          object value)
        {
            if (!(value is string))
            {
                return base.ConvertFrom(context, culture, value);
            }

            var strArray = ((string)value).Split(new char[2]
      {
        ',',
        ' '
      }, StringSplitOptions.RemoveEmptyEntries);
            return strArray.Length == 4 ? (object)new SvgViewBox(float.Parse(strArray[0], NumberStyles.Float, CultureInfo.InvariantCulture), float.Parse(strArray[1], NumberStyles.Float, CultureInfo.InvariantCulture), float.Parse(strArray[2], NumberStyles.Float, CultureInfo.InvariantCulture), float.Parse(strArray[3], NumberStyles.Float, CultureInfo.InvariantCulture)) : throw new SvgException("The 'viewBox' attribute must be in the format 'minX, minY, width, height'.");
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
            if (!(destinationType == typeof(string)))
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }

            SvgViewBox svgViewBox = (SvgViewBox)value;
            var objArray = new object[4];
            var num = svgViewBox.MinX;
            objArray[0] = num.ToString(CultureInfo.InvariantCulture);
            num = svgViewBox.MinY;
            objArray[1] = num.ToString(CultureInfo.InvariantCulture);
            num = svgViewBox.Width;
            objArray[2] = num.ToString(CultureInfo.InvariantCulture);
            num = svgViewBox.Height;
            objArray[3] = num.ToString(CultureInfo.InvariantCulture);
            return string.Format("{0}, {1}, {2}, {3}", objArray);
        }
    }
}
