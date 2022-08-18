// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

namespace Svg
{
    public static class Enums
    {
        public static bool TryParse<TEnum>(string value, out TEnum result) where TEnum : struct, IConvertible
        {
            try
            {
                result = (TEnum)Enum.Parse(typeof(TEnum), value, true);
                return true;
            }
            catch
            {
                result = default(TEnum);
                return false;
            }
        }
    }
}
