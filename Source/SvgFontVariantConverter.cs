// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.ComponentModel;
using System.Globalization;

namespace Svg
{
    public sealed class SvgFontVariantConverter : EnumBaseConverter<SvgFontVariant>
    {
        public SvgFontVariantConverter()
          : base(SvgFontVariant.Normal)
        {
        }

        public override object ConvertFrom(
          ITypeDescriptorContext context,
          CultureInfo culture,
          object value)
        {
            return value.ToString() == "small-caps" ? SvgFontVariant.Smallcaps : base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(
          ITypeDescriptorContext context,
          CultureInfo culture,
          object value,
          Type destinationType)
        {
            return destinationType == typeof(string) && value is SvgFontVariant svgFontVariant && svgFontVariant == SvgFontVariant.Smallcaps ? "small-caps" : base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
