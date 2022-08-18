// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

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
