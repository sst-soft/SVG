// todo: add license

using System.Drawing.Drawing2D;

namespace Svg
{
    [SvgElement("textPath")]
    public class SvgTextPath : SvgTextBase
    {
        public override SvgUnitCollection Dx
        {
            get => null;
            set
            {
            }
        }

        [SvgAttribute("startOffset")]
        public virtual SvgUnit StartOffset
        {
            get => base.Dx.Count >= 1 ? base.Dx[0] : SvgUnit.None;
            set
            {
                if (base.Dx.Count < 1)
                {
                    base.Dx.Add(value);
                }
                else
                {
                    base.Dx[0] = value;
                }

                Attributes["startOffset"] = value;
            }
        }

        [SvgAttribute("method")]
        public virtual SvgTextPathMethod Method
        {
            get => GetAttribute<SvgTextPathMethod>("method", true);
            set => Attributes["method"] = value;
        }

        [SvgAttribute("spacing")]
        public virtual SvgTextPathSpacing Spacing
        {
            get => GetAttribute<SvgTextPathSpacing>("spacing", true);
            set => Attributes["spacing"] = value;
        }

        [SvgAttribute("href", "http://www.w3.org/1999/xlink")]
        public virtual Uri ReferencedPath
        {
            get => GetAttribute<Uri>("href", false);
            set => Attributes["href"] = value;
        }

        protected override GraphicsPath GetBaselinePath(ISvgRenderer renderer)
        {
            if (!(OwnerDocument.IdManager.GetElementById(ReferencedPath) is SvgVisualElement elementById))
            {
                return null;
            }

            GraphicsPath baselinePath = (GraphicsPath)elementById.Path(renderer).Clone();
            if (elementById.Transforms != null && elementById.Transforms.Count > 0)
            {
                using (Matrix matrix = elementById.Transforms.GetMatrix())
                {
                    baselinePath.Transform(matrix);
                }
            }
            return baselinePath;
        }

        protected override float GetAuthorPathLength()
        {
            return !(OwnerDocument.IdManager.GetElementById(ReferencedPath) is SvgPath elementById) ? 0.0f : elementById.PathLength;
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgTextPath>();
        }
    }
}
