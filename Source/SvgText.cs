// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

namespace Svg
{
    [SvgElement("text")]
    public class SvgText : SvgTextBase
    {
        public SvgText()
        {
        }

        public SvgText(string text)
          : this()
        {
            Text = text;
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgText>();
        }

        public override SvgElement DeepCopy<T>()
        {
            SvgText svgText = base.DeepCopy<T>() as SvgText;
            svgText.TextAnchor = TextAnchor;
            svgText.WordSpacing = WordSpacing;
            svgText.LetterSpacing = LetterSpacing;
            svgText.Font = Font;
            svgText.FontFamily = FontFamily;
            svgText.FontSize = FontSize;
            svgText.FontWeight = FontWeight;
            svgText.X = X;
            svgText.Y = Y;
            return svgText;
        }
    }
}
