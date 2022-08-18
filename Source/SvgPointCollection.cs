// todo: add license

using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace Svg
{
    [TypeConverter(typeof(SvgPointCollectionConverter))]
    public class SvgPointCollection : List<SvgUnit>
    {
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (var index = 0; index < Count; index += 2)
            {
                if (index + 1 < Count)
                {
                    if (index > 1)
                    {
                        stringBuilder.Append(" ");
                    }

                    stringBuilder.Append(this[index].Value.ToString(CultureInfo.InvariantCulture));
                    stringBuilder.Append(",");
                    stringBuilder.Append(this[index + 1].Value.ToString(CultureInfo.InvariantCulture));
                }
            }
            return stringBuilder.ToString();
        }
    }
}
