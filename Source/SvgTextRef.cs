// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

namespace Svg
{
    [SvgElement("tref")]
    public class SvgTextRef : SvgTextBase
    {
        [SvgAttribute("href", "http://www.w3.org/1999/xlink")]
        public virtual Uri ReferencedElement
        {
            get => GetAttribute<Uri>("href", false);
            set => Attributes["href"] = value;
        }

        internal override IEnumerable<ISvgNode> GetContentNodes()
        {
            SvgTextBase elementById = OwnerDocument.IdManager.GetElementById(ReferencedElement) as SvgTextBase;
            return (elementById != null ? elementById.GetContentNodes() : base.GetContentNodes()).Where<ISvgNode>(o => !(o is ISvgDescriptiveElement));
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgTextRef>();
        }

        public override SvgElement DeepCopy<T>()
        {
            SvgTextRef svgTextRef = base.DeepCopy<T>() as SvgTextRef;
            svgTextRef.X = X;
            svgTextRef.Y = Y;
            svgTextRef.Dx = Dx;
            svgTextRef.Dy = Dy;
            svgTextRef.Text = Text;
            svgTextRef.ReferencedElement = ReferencedElement;
            return svgTextRef;
        }
    }
}
