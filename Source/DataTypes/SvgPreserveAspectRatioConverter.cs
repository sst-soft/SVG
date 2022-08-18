// todo: add license

using System.ComponentModel;
using System.Globalization;

namespace Svg.DataTypes
{
    public class SvgPreserveAspectRatioConverter : TypeConverter
    {
        public override object ConvertFrom(
          ITypeDescriptorContext context,
          CultureInfo culture,
          object value)
        {
            if (value == null)
            {
                return new SvgAspectRatio();
            }

            if (!(value is string))
            {
                throw new ArgumentOutOfRangeException("value must be a string.");
            }

            var defer = false;
            var slice = false;
            var strArray = (value as string).Split(new char[1]
            {
        ' '
            }, StringSplitOptions.RemoveEmptyEntries);
            var index1 = 0;
            if (strArray[0].Equals("defer"))
            {
                defer = true;
                ++index1;
                if (strArray.Length < 2)
                {
                    throw new ArgumentOutOfRangeException("value is not a member of SvgPreserveAspectRatio");
                }
            }
            SvgPreserveAspectRatio align = (SvgPreserveAspectRatio)Enum.Parse(typeof(SvgPreserveAspectRatio), strArray[index1]);
            var index2 = index1 + 1;
            if (strArray.Length > index2)
            {
                switch (strArray[index2])
                {
                    case "meet":
                        break;
                    case "slice":
                        slice = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("value is not a member of SvgPreserveAspectRatio");
                }
            }
            var num = index2 + 1;
            if (strArray.Length > num)
            {
                throw new ArgumentOutOfRangeException("value is not a member of SvgPreserveAspectRatio");
            }

            return new SvgAspectRatio(align, slice, defer);
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
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
