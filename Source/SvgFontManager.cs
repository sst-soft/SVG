// todo: add license

using System.Drawing;
using System.Drawing.Text;

namespace Svg
{
    public static class SvgFontManager
    {
        private static readonly Dictionary<string, FontFamily> SystemFonts = FontFamily.Families.GroupBy<FontFamily, string>(ff => ff.Name.ToLower()).ToDictionary<IGrouping<string, FontFamily>, string, FontFamily>(x => x.Key, x => x.First<FontFamily>());
        public static Func<string, FontFamily> FontLoaderCallback;

        public static FontFamily LoadFontFamily(string path)
        {
            PrivateFontCollection privateFontCollection = new PrivateFontCollection();
            var fullPath = Path.GetFullPath(path);
            privateFontCollection.AddFontFile(fullPath);
            return privateFontCollection.Families.Length != 0 ? privateFontCollection.Families[0] : null;
        }

        public static FontFamily FindFont(string name)
        {
            if (name == null)
            {
                return null;
            }

            if (SvgFontManager.SystemFonts.TryGetValue(name.ToLower(), out FontFamily font1))
            {
                return font1;
            }

            if (SvgFontManager.FontLoaderCallback == null)
            {
                return null;
            }

            FontFamily font2 = SvgFontManager.FontLoaderCallback(name);
            if (font2 != null)
            {
                SvgFontManager.SystemFonts.Add(name.ToLower(), font2);
            }

            return font2;
        }
    }
}
