// todo: add license

using System.ComponentModel;
using System.Globalization;

namespace Svg
{
    internal class SvgDeferredPaintServerFactory : SvgPaintServerFactory
    {
        public override object ConvertFrom(
          ITypeDescriptorContext context,
          CultureInfo culture,
          object value)
        {
            if (value is string)
            {
                var id = ((string)value).Trim();
                if (!string.IsNullOrEmpty(id))
                {
                    return new SvgDeferredPaintServer(id);
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}
