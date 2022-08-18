// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

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
