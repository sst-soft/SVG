// todo: add license

using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace Svg
{
    internal class SvgPaintServerFactory : TypeConverter
    {
        private static readonly SvgColourConverter _colourConverter = new SvgColourConverter();

        public static SvgPaintServer Create(string value, SvgDocument document)
        {
            if (value == null)
            {
                return SvgPaintServer.NotSet;
            }

            var str1 = value.Trim();
            if (string.IsNullOrEmpty(str1))
            {
                return SvgPaintServer.NotSet;
            }

            if (str1.Equals("none", StringComparison.OrdinalIgnoreCase))
            {
                return SvgPaintServer.None;
            }

            if (str1.Equals("currentColor", StringComparison.OrdinalIgnoreCase))
            {
                return new SvgDeferredPaintServer("currentColor");
            }

            if (str1.Equals("inherit", StringComparison.OrdinalIgnoreCase))
            {
                return SvgPaintServer.Inherit;
            }

            if (!str1.StartsWith("url(", StringComparison.OrdinalIgnoreCase))
            {
                return new SvgColourServer((Color)_colourConverter.ConvertFrom(str1));
            }

            var num = str1.IndexOf(')', 4) + 1;
            var id = str1.Substring(0, num);
            var str2 = str1.Substring(num).Trim();
            SvgPaintServer fallbackServer = string.IsNullOrEmpty(str2) ? null : SvgPaintServerFactory.Create(str2, document);
            return new SvgDeferredPaintServer(id, fallbackServer);
        }

        public override object ConvertFrom(
          ITypeDescriptorContext context,
          CultureInfo culture,
          object value)
        {
            return value is string ? SvgPaintServerFactory.Create((string)value, (SvgDocument)context) : base.ConvertFrom(context, culture, value);
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

            if (value == SvgPaintServer.None || value == SvgPaintServer.Inherit || value == SvgPaintServer.NotSet)
            {
                return value.ToString();
            }

            switch (value)
            {
                case SvgColourServer svgColourServer:
                    return new SvgColourConverter().ConvertTo(svgColourServer.Colour, typeof(string));
                case SvgDeferredPaintServer deferredPaintServer:
                    return deferredPaintServer.ToString();
                case null:
                    return "none";
                default:
                    return string.Format(CultureInfo.InvariantCulture, "url(#{0})", ((SvgElement)value).ID);
            }
        }
    }
}
