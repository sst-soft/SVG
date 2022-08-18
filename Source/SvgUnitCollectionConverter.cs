// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.ComponentModel;
using System.Globalization;

namespace Svg
{
    internal class SvgUnitCollectionConverter : TypeConverter
    {
        private static readonly SvgUnitConverter _unitConverter = new SvgUnitConverter();

        public override object ConvertFrom(
          ITypeDescriptorContext context,
          CultureInfo culture,
          object value)
        {
            if (!(value is string))
            {
                return base.ConvertFrom(context, culture, value);
            }

            var strArray = ((string)value).Trim().Split(new char[5]
      {
        ',',
        ' ',
        '\r',
        '\n',
        '\t'
      }, StringSplitOptions.RemoveEmptyEntries);
            SvgUnitCollection svgUnitCollection = new SvgUnitCollection();
            foreach (var str in strArray)
            {
                SvgUnit svgUnit = (SvgUnit)SvgUnitCollectionConverter._unitConverter.ConvertFrom(str.Trim());
                if (!svgUnit.IsNone)
                {
                    svgUnitCollection.Add(svgUnit);
                }
            }
            return svgUnitCollection;
        }
    }
}
