// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.Drawing;
using System.Drawing.Drawing2D;

namespace Svg
{
    public abstract class SvgPathBasedElement : SvgVisualElement
    {
        public override RectangleF Bounds
        {
            get
            {
                GraphicsPath graphicsPath1 = Path(null);
                if (graphicsPath1 == null)
                {
                    return new RectangleF();
                }

                if (Transforms == null || Transforms.Count == 0)
                {
                    return graphicsPath1.GetBounds();
                }

                GraphicsPath graphicsPath2;
                using (graphicsPath2 = (GraphicsPath)graphicsPath1.Clone())
                {
                    using (Matrix matrix = Transforms.GetMatrix())
                    {
                        graphicsPath2.Transform(matrix);
                        return graphicsPath2.GetBounds();
                    }
                }
            }
        }
    }
}
