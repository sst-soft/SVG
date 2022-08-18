// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Svg
{
    [TypeConverter(typeof(SvgUnitCollectionConverter))]
    public class SvgUnitCollection : ObservableCollection<SvgUnit>
    {
        public static string None = "none";
        public static string Inherit = "inherit";

        public string StringForEmptyValue { get; set; }

        public void AddRange(IEnumerable<SvgUnit> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (collection == this)
            {
                SvgUnitCollection svgUnitCollection = new SvgUnitCollection();
                foreach (SvgUnit svgUnit in collection)
                {
                    svgUnitCollection.Add(svgUnit);
                }

                collection = svgUnitCollection;
            }
            foreach (SvgUnit svgUnit in collection)
            {
                Add(svgUnit);
            }
        }

        public override string ToString()
        {
            return Count <= 0 && !string.IsNullOrEmpty(StringForEmptyValue) ? StringForEmptyValue : string.Join(" ", this.Select<SvgUnit, string>(u => u.ToString()).ToArray<string>());
        }

        public static bool IsNullOrEmpty(SvgUnitCollection collection)
        {
            if (collection == null || collection.Count < 1)
            {
                return true;
            }

            if (collection.Count != 1)
            {
                return false;
            }

            return collection[0] == SvgUnit.Empty || collection[0] == SvgUnit.None;
        }
    }
}
