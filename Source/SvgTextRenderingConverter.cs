// todo: add license

namespace Svg
{
    public sealed class SvgTextRenderingConverter : EnumBaseConverter<SvgTextRendering>
    {
        public SvgTextRenderingConverter()
          : base(SvgTextRendering.Inherit)
        {
        }
    }
}
