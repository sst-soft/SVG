// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.Drawing;

namespace Svg.FilterEffects
{
    [SvgElement("feMerge")]
    public class SvgMerge : SvgFilterPrimitive
    {
        public override void Process(ImageBuffer buffer)
        {
            List<SvgMergeNode> list = Children.OfType<SvgMergeNode>().ToList<SvgMergeNode>();
            Bitmap bitmap1 = buffer[list.First<SvgMergeNode>().Input];
            Bitmap bitmap2 = new Bitmap(bitmap1.Width, bitmap1.Height);
            using (Graphics graphics = Graphics.FromImage(bitmap2))
            {
                foreach (SvgMergeNode svgMergeNode in list)
                {
                    graphics.DrawImage(buffer[svgMergeNode.Input], new Rectangle(0, 0, bitmap1.Width, bitmap1.Height), 0, 0, bitmap1.Width, bitmap1.Height, (GraphicsUnit)2);
                }

                graphics.Flush();
            }
            buffer[Result] = bitmap2;
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgMerge>();
        }
    }
}
