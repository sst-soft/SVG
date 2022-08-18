// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

namespace Svg.ExtensionMethods
{
    public static class UriExtensions
    {
        public static Uri ReplaceWithNullIfNone(this Uri uri)
        {
            if (uri == null)
            {
                return null;
            }

            return !string.Equals(uri.ToString(), "none", StringComparison.OrdinalIgnoreCase) ? uri : null;
        }
    }
}
