// todo: add license

using System.ComponentModel;
using System.Globalization;

namespace Svg
{
    internal class SvgPointCollectionConverter : TypeConverter
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

            CoordinateParser coordinateParser = new CoordinateParser(((string)value).Trim());
            var result = 0.0f;
            SvgPointCollection svgPointCollection = new SvgPointCollection();
            while (coordinateParser.TryGetFloat(out result))
            {
                svgPointCollection.Add(new SvgUnit(SvgUnitType.User, result));
            }

            return svgPointCollection;
        }
    }
}
