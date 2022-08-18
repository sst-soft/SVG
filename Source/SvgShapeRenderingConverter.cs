// todo: add license

namespace Svg
{
    public sealed class SvgShapeRenderingConverter : EnumBaseConverter<SvgShapeRendering>
    {
        public SvgShapeRenderingConverter()
          : base(SvgShapeRendering.Inherit)
        {
        }
    }
}
