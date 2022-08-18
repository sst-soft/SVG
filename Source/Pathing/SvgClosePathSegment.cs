// todo: add license

using System.Drawing.Drawing2D;

namespace Svg.Pathing
{
    public sealed class SvgClosePathSegment : SvgPathSegment
    {
        public override void AddToPath(GraphicsPath graphicsPath)
        {
            PathData pathData = graphicsPath.PathData;
            if (pathData.Points.Length == 0)
            {
                return;
            }

            var index1 = pathData.Points.Length - 1;
            if (!pathData.Points[0].Equals(pathData.Points[index1]))
            {
                var index2 = index1;
                while (index2 > 0 && pathData.Types[index2] > 0)
                {
                    --index2;
                }

                graphicsPath.AddLine(pathData.Points[index1], pathData.Points[index2]);
            }
            graphicsPath.CloseFigure();
        }

        public override string ToString()
        {
            return "z";
        }
    }
}
