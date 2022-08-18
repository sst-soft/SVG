// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.ComponentModel;
using System.Globalization;

namespace Svg
{
    internal class SvgStrokeDashArrayConverter : SvgUnitCollectionConverter
    {
        public override object ConvertFrom(
          ITypeDescriptorContext context,
          CultureInfo culture,
          object value)
        {
            if (value is string)
            {
                var str = ((string)value).Trim();
                if (str.Equals(SvgUnitCollection.None, StringComparison.OrdinalIgnoreCase))
                {
                    return new SvgUnitCollection()
                    {
                        StringForEmptyValue = SvgUnitCollection.None
                    };
                }

                if (str.Equals(SvgUnitCollection.Inherit, StringComparison.OrdinalIgnoreCase))
                {
                    return new SvgUnitCollection()
                    {
                        StringForEmptyValue = SvgUnitCollection.Inherit
                    };
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}
