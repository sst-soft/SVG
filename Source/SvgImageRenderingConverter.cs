// todo: add license

namespace Svg
{
    public sealed class SvgImageRenderingConverter : EnumBaseConverter<SvgImageRendering>
    {
        public SvgImageRenderingConverter()
          : base(SvgImageRendering.Inherit)
        {
        }
    }
}
