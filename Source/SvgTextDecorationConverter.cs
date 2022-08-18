// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.ComponentModel;
using System.Globalization;

namespace Svg
{
    public sealed class SvgTextDecorationConverter : EnumBaseConverter<SvgTextDecoration>
    {
        public SvgTextDecorationConverter()
          : base(SvgTextDecoration.None)
        {
        }

        public override object ConvertFrom(
          ITypeDescriptorContext context,
          CultureInfo culture,
          object value)
        {
            return value.ToString() == "line-through" ? SvgTextDecoration.LineThrough : base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(
          ITypeDescriptorContext context,
          CultureInfo culture,
          object value,
          Type destinationType)
        {
            return destinationType == typeof(string) && value is SvgTextDecoration svgTextDecoration && svgTextDecoration == SvgTextDecoration.LineThrough ? "line-through" : base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
