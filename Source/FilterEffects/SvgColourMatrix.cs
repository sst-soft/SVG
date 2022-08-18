// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;

namespace Svg.FilterEffects
{
    [SvgElement("feColorMatrix")]
    public class SvgColourMatrix : SvgFilterPrimitive
    {
        private SvgColourMatrixType _type;
        private string _values;

        [SvgAttribute("type")]
        public SvgColourMatrixType Type
        {
            get => _type;
            set
            {
                _type = value;
                Attributes["type"] = value;
            }
        }

        [SvgAttribute("values")]
        public string Values
        {
            get => _values;
            set
            {
                _values = value;
                Attributes["values"] = value;
            }
        }

        public override void Process(ImageBuffer buffer)
        {
            Bitmap bitmap1 = buffer[Input];
            if (bitmap1 == null)
            {
                return;
            }

            float[][] numArray;
            switch (Type)
            {
                case SvgColourMatrixType.Saturate:
                    var num1 = string.IsNullOrEmpty(Values) ? 1f : float.Parse(Values, NumberStyles.Any, CultureInfo.InvariantCulture);
                    numArray = new float[5][]
                    {
            new float[5]
            {
              (float) (0.213 + 0.787 * (double) num1),
              (float) (0.715 - 0.715 * (double) num1),
              (float) (0.072 - 0.072 * (double) num1),
              0.0f,
              0.0f
            },
            new float[5]
            {
              (float) (0.213 - 0.213 * (double) num1),
              (float) (0.715 + 0.285 * (double) num1),
              (float) (0.072 - 0.072 * (double) num1),
              0.0f,
              0.0f
            },
            new float[5]
            {
              (float) (0.213 - 0.213 * (double) num1),
              (float) (0.715 - 0.715 * (double) num1),
              (float) (0.072 + 0.928 * (double) num1),
              0.0f,
              0.0f
            },
            new float[5]{ 0.0f, 0.0f, 0.0f, 1f, 0.0f },
            new float[5]{ 0.0f, 0.0f, 0.0f, 0.0f, 1f }
                    };
                    break;
                case SvgColourMatrixType.HueRotate:
                    var num2 = string.IsNullOrEmpty(Values) ? 0.0f : float.Parse(Values, NumberStyles.Any, CultureInfo.InvariantCulture);
                    numArray = new float[5][]
                    {
            new float[5]
            {
              (float) (0.213 + Math.Cos((double) num2) * 0.787 + Math.Sin((double) num2) * -0.213),
              (float) (0.715 + Math.Cos((double) num2) * -0.715 + Math.Sin((double) num2) * -0.715),
              (float) (0.072 + Math.Cos((double) num2) * -0.072 + Math.Sin((double) num2) * 0.928),
              0.0f,
              0.0f
            },
            new float[5]
            {
              (float) (0.213 + Math.Cos((double) num2) * -0.213 + Math.Sin((double) num2) * 0.143),
              (float) (0.715 + Math.Cos((double) num2) * 0.285 + Math.Sin((double) num2) * 0.14),
              (float) (0.072 + Math.Cos((double) num2) * -0.072 + Math.Sin((double) num2) * -0.283),
              0.0f,
              0.0f
            },
            new float[5]
            {
              (float) (0.213 + Math.Cos((double) num2) * -0.213 + Math.Sin((double) num2) * -0.787),
              (float) (0.715 + Math.Cos((double) num2) * -0.715 + Math.Sin((double) num2) * 0.715),
              (float) (0.072 + Math.Cos((double) num2) * 0.928 + Math.Sin((double) num2) * 0.072),
              0.0f,
              0.0f
            },
            new float[5]{ 0.0f, 0.0f, 0.0f, 1f, 0.0f },
            new float[5]{ 0.0f, 0.0f, 0.0f, 0.0f, 1f }
                    };
                    break;
                case SvgColourMatrixType.LuminanceToAlpha:
                    numArray = new float[5][]
                    {
            new float[5],
            new float[5],
            new float[5],
            new float[5]{ 0.2125f, 0.7154f, 0.0721f, 0.0f, 0.0f },
            new float[5]{ 0.0f, 0.0f, 0.0f, 0.0f, 1f }
                    };
                    break;
                default:
                    var source = Values.Split(new char[5]
                    {
            ' ',
            '\t',
            '\n',
            '\r',
            ','
                    }, StringSplitOptions.RemoveEmptyEntries);
                    numArray = new float[5][];
                    for (var index = 0; index < 4; ++index)
                    {
                        numArray[index] = source.Skip<string>(index * 5).Take<string>(5).Select<string, float>(v => float.Parse(v, NumberStyles.Any, CultureInfo.InvariantCulture)).ToArray<float>();
                    }

                    numArray[4] = new float[5]
          {
            0.0f,
            0.0f,
            0.0f,
            0.0f,
            1f
          };
                    break;
            }
            ColorMatrix colorMatrix = new ColorMatrix(numArray);
            using (ImageAttributes imageAttributes = new ImageAttributes())
            {
                imageAttributes.SetColorMatrix(colorMatrix, 0, (ColorAdjustType)1);
                Bitmap bitmap2 = new Bitmap(bitmap1.Width, bitmap1.Height);
                using (Graphics graphics = Graphics.FromImage(bitmap2))
                {
                    graphics.DrawImage(bitmap1, new Rectangle(0, 0, bitmap1.Width, bitmap1.Height), 0, 0, bitmap1.Width, bitmap1.Height, (GraphicsUnit)2, imageAttributes);
                    graphics.Flush();
                }
                buffer[Result] = bitmap2;
            }
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgColourMatrix>();
        }

        public override SvgElement DeepCopy<T>()
        {
            SvgColourMatrix svgColourMatrix = base.DeepCopy<T>() as SvgColourMatrix;
            svgColourMatrix.Type = Type;
            svgColourMatrix.Values = Values;
            return svgColourMatrix;
        }
    }
}
