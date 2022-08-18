// todo: add license

using System.Drawing;

namespace Svg.FilterEffects
{
    [SvgElement("feOffset")]
    public class SvgOffset : SvgFilterPrimitive
    {
        private SvgUnit _dx = (SvgUnit)0.0f;
        private SvgUnit _dy = (SvgUnit)0.0f;

        [SvgAttribute("dx")]
        public SvgUnit Dx
        {
            get => _dx;
            set
            {
                _dx = value;
                Attributes["dx"] = value;
            }
        }

        [SvgAttribute("dy")]
        public SvgUnit Dy
        {
            get => _dy;
            set
            {
                _dy = value;
                Attributes["dy"] = value;
            }
        }

        public override void Process(ImageBuffer buffer)
        {
            Bitmap bitmap1 = buffer[Input];
            Bitmap bitmap2 = new Bitmap(bitmap1.Width, bitmap1.Height);
            PointF[] pointFArray1 = new PointF[1];
            SvgUnit svgUnit = Dx;
            var deviceValue1 = (double)svgUnit.ToDeviceValue(null, UnitRenderingType.Horizontal, null);
            svgUnit = Dy;
            var deviceValue2 = (double)svgUnit.ToDeviceValue(null, UnitRenderingType.Vertical, null);
            pointFArray1[0] = new PointF((float)deviceValue1, (float)deviceValue2);
            PointF[] pointFArray2 = pointFArray1;
            buffer.Transform.TransformVectors(pointFArray2);
            using (Graphics graphics = Graphics.FromImage(bitmap2))
            {
                graphics.DrawImage(bitmap1, new Rectangle((int)pointFArray2[0].X, (int)pointFArray2[0].Y, bitmap1.Width, bitmap1.Height), 0, 0, bitmap1.Width, bitmap1.Height, (GraphicsUnit)2);
                graphics.Flush();
            }
            buffer[Result] = bitmap2;
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgOffset>();
        }

        public override SvgElement DeepCopy<T>()
        {
            SvgOffset svgOffset = base.DeepCopy<T>() as SvgOffset;
            svgOffset.Dx = Dx;
            svgOffset.Dy = Dy;
            return svgOffset;
        }
    }
}
