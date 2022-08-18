// todo: add license

namespace Svg
{
    [SvgElement("missing-glyph")]
    public class SvgMissingGlyph : SvgGlyph
    {
        [SvgAttribute("glyph-name")]
        public override string GlyphName
        {
            get => GetAttribute<string>("glyph-name", true, "__MISSING_GLYPH__");
            set => Attributes["glyph-name"] = value;
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgMissingGlyph>();
        }
    }
}
