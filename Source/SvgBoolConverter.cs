// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.ComponentModel;
using System.Globalization;

namespace Svg
{
    public class SvgBoolConverter : BaseConverter
    {
        public override object ConvertFrom(
          ITypeDescriptorContext context,
          CultureInfo culture,
          object value)
        {
            if (value == null)
            {
                return true;
            }

            var str = value is string ? (string)value : throw new ArgumentOutOfRangeException("value must be a string.");
            return str == "hidden" || str == "collapse" ? false : (object)true;
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

            return !(bool)value ? "hidden" : (object)"visible";
        }
    }
}
