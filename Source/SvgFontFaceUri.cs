// todo: add license

namespace Svg
{
    [SvgElement("font-face-uri")]
    public class SvgFontFaceUri : SvgElement
    {
        [SvgAttribute("href", "http://www.w3.org/1999/xlink")]
        public virtual Uri ReferencedElement
        {
            get => GetAttribute<Uri>("href", false);
            set => Attributes["href"] = value;
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgFontFaceUri>();
        }

        public override SvgElement DeepCopy<T>()
        {
            SvgFontFaceUri svgFontFaceUri = base.DeepCopy<T>() as SvgFontFaceUri;
            svgFontFaceUri.ReferencedElement = ReferencedElement;
            return svgFontFaceUri;
        }
    }
}
