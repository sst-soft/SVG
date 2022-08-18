// todo: add license

using System.Drawing;
using System.Drawing.Drawing2D;

namespace Svg
{
    [SvgElement("clipPath")]
    public sealed class SvgClipPath : SvgElement
    {
        private GraphicsPath _path;

        [SvgAttribute("clipPathUnits")]
        public SvgCoordinateUnits ClipPathUnits
        {
            get => GetAttribute<SvgCoordinateUnits>("clipPathUnits", false, SvgCoordinateUnits.UserSpaceOnUse);
            set => Attributes["clipPathUnits"] = value;
        }

        public Region GetClipRegion(SvgVisualElement owner, ISvgRenderer renderer)
        {
            if (_path == null || IsPathDirty)
            {
                _path = new GraphicsPath();
                foreach (SvgElement child in Children)
                {
                    CombinePaths(_path, child, renderer);
                }

                IsPathDirty = false;
            }
            GraphicsPath graphicsPath = _path;
            if (ClipPathUnits == SvgCoordinateUnits.ObjectBoundingBox)
            {
                graphicsPath = (GraphicsPath)_path.Clone();
                using (Matrix matrix = new Matrix())
                {
                    RectangleF bounds = owner.Bounds;
                    matrix.Scale(bounds.Width, bounds.Height, (MatrixOrder)1);
                    matrix.Translate(bounds.Left, bounds.Top, (MatrixOrder)1);
                    graphicsPath.Transform(matrix);
                }
            }
            return new Region(graphicsPath);
        }

        private void CombinePaths(GraphicsPath path, SvgElement element, ISvgRenderer renderer)
        {
            if (element is SvgVisualElement svgVisualElement)
            {
                GraphicsPath graphicsPath = svgVisualElement.Path(renderer);
                if (graphicsPath != null)
                {
                    path.FillMode = svgVisualElement.ClipRule == SvgClipRule.NonZero ? (FillMode)1 : 0;
                    if (svgVisualElement.Transforms != null)
                    {
                        using (Matrix matrix = svgVisualElement.Transforms.GetMatrix())
                        {
                            graphicsPath.Transform(matrix);
                        }
                    }
                    if (graphicsPath.PointCount > 0)
                    {
                        path.AddPath(graphicsPath, false);
                    }
                }
            }
            foreach (SvgElement child in element.Children)
            {
                CombinePaths(path, child, renderer);
            }
        }

        protected override void AddElement(SvgElement child, int index)
        {
            base.AddElement(child, index);
            IsPathDirty = true;
        }

        protected override void RemoveElement(SvgElement child)
        {
            base.RemoveElement(child);
            IsPathDirty = true;
        }

        protected override void Render(ISvgRenderer renderer)
        {
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgClipPath>();
        }

        public override SvgElement DeepCopy<T>()
        {
            SvgClipPath svgClipPath = base.DeepCopy<T>() as SvgClipPath;
            svgClipPath.ClipPathUnits = ClipPathUnits;
            return svgClipPath;
        }
    }
}
