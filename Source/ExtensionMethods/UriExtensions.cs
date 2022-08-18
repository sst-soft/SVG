// todo: add license

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
