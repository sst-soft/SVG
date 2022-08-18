// todo: add license

using System.Drawing;

namespace Svg.FilterEffects
{
    [SvgElement("feGaussianBlur")]
    public class SvgGaussianBlur : SvgFilterPrimitive
    {
        private float _stdDeviation;
        private BlurType _blurType;
        private int[] _kernel;
        private int _kernelSum;
        private int[,] _multable;

        public SvgGaussianBlur()
          : this(1f, BlurType.Both)
        {
        }

        public SvgGaussianBlur(float stdDeviation)
          : this(stdDeviation, BlurType.Both)
        {
        }

        public SvgGaussianBlur(float stdDeviation, BlurType blurType)
        {
            _stdDeviation = stdDeviation;
            _blurType = blurType;
            PreCalculate();
        }

        private void PreCalculate()
        {
            var length = (int)(_stdDeviation * 2.0 + 1.0);
            _kernel = new int[length];
            _multable = new int[length, 256];
            for (var index1 = 1; index1 <= (double)_stdDeviation; ++index1)
            {
                var index2 = (int)(_stdDeviation - (double)index1);
                var index3 = (int)(_stdDeviation + (double)index1);
                _kernel[index3] = _kernel[index2] = (index2 + 1) * (index2 + 1);
                _kernelSum += _kernel[index3] + _kernel[index2];
                for (var index4 = 0; index4 < 256; ++index4)
                {
                    _multable[index3, index4] = _multable[index2, index4] = _kernel[index3] * index4;
                }
            }
            _kernel[(int)_stdDeviation] = (int)((_stdDeviation + 1.0) * (_stdDeviation + 1.0));
            _kernelSum += _kernel[(int)_stdDeviation];
            for (var index = 0; index < 256; ++index)
            {
                _multable[(int)_stdDeviation, index] = _kernel[(int)_stdDeviation] * index;
            }
        }

        public Bitmap Apply(Image inputImage)
        {
            if (!(inputImage is Bitmap originBitmap))
            {
                originBitmap = new Bitmap(inputImage);
            }

            using (RawBitmap rawBitmap1 = new RawBitmap(originBitmap))
            {
                using (RawBitmap rawBitmap2 = new RawBitmap(new Bitmap(inputImage.Width, inputImage.Height)))
                {
                    var length = rawBitmap1.Width * rawBitmap1.Height;
                    var numArray1 = new int[length];
                    var numArray2 = new int[length];
                    var numArray3 = new int[length];
                    var numArray4 = new int[length];
                    var numArray5 = new int[length];
                    var numArray6 = new int[length];
                    var numArray7 = new int[length];
                    var numArray8 = new int[length];
                    var index1 = 0;
                    for (var index2 = 0; index2 < length; ++index2)
                    {
                        numArray1[index2] = rawBitmap1.ArgbValues[index1];
                        int num1;
                        numArray2[index2] = rawBitmap1.ArgbValues[num1 = index1 + 1];
                        int num2;
                        numArray3[index2] = rawBitmap1.ArgbValues[num2 = num1 + 1];
                        int num3;
                        numArray4[index2] = rawBitmap1.ArgbValues[num3 = num2 + 1];
                        index1 = num3 + 1;
                    }
                    var num4 = 0;
                    var index3 = 0;
                    if (_blurType != BlurType.VerticalOnly)
                    {
                        for (var index4 = 0; index4 < length; ++index4)
                        {
                            int num5;
                            var num6 = num5 = 0;
                            var num7 = num5;
                            var num8 = num5;
                            var num9 = num5;
                            var num10 = (int)(index4 - (double)_stdDeviation);
                            for (var index5 = 0; index5 < _kernel.Length; ++index5)
                            {
                                var index6 = num10 >= num4 ? (num10 <= num4 + rawBitmap1.Width - 1 ? num10 : num4 + rawBitmap1.Width - 1) : num4;
                                num9 += _multable[index5, numArray1[index6]];
                                num8 += _multable[index5, numArray2[index6]];
                                num7 += _multable[index5, numArray3[index6]];
                                num6 += _multable[index5, numArray4[index6]];
                                ++num10;
                            }
                            numArray5[index4] = num9 / _kernelSum;
                            numArray6[index4] = num8 / _kernelSum;
                            numArray7[index4] = num7 / _kernelSum;
                            numArray8[index4] = num6 / _kernelSum;
                            if (_blurType == BlurType.HorizontalOnly)
                            {
                                rawBitmap2.ArgbValues[index3] = (byte)(num9 / _kernelSum);
                                int num11;
                                rawBitmap2.ArgbValues[num11 = index3 + 1] = (byte)(num8 / _kernelSum);
                                int num12;
                                rawBitmap2.ArgbValues[num12 = num11 + 1] = (byte)(num7 / _kernelSum);
                                int num13;
                                rawBitmap2.ArgbValues[num13 = num12 + 1] = (byte)(num6 / _kernelSum);
                                index3 = num13 + 1;
                            }
                            if (index4 > 0 && index4 % rawBitmap1.Width == 0)
                            {
                                num4 += rawBitmap1.Width;
                            }
                        }
                    }
                    if (_blurType == BlurType.HorizontalOnly)
                    {
                        return rawBitmap2.Bitmap;
                    }

                    var index7 = 0;
                    for (var index8 = 0; index8 < rawBitmap1.Height; ++index8)
                    {
                        var num14 = (int)(index8 - (double)_stdDeviation);
                        var num15 = num14 * rawBitmap1.Width;
                        for (var index9 = 0; index9 < rawBitmap1.Width; ++index9)
                        {
                            int num16;
                            var num17 = num16 = 0;
                            var num18 = num16;
                            var num19 = num16;
                            var num20 = num16;
                            var num21 = num15 + index9;
                            var num22 = num14;
                            for (var index10 = 0; index10 < _kernel.Length; ++index10)
                            {
                                if (_blurType == BlurType.VerticalOnly)
                                {
                                    var index11 = num22 >= 0 ? (num22 <= rawBitmap1.Height - 1 ? num21 : length - (rawBitmap1.Width - index9)) : index9;
                                    num20 += _multable[index10, numArray1[index11]];
                                    num19 += _multable[index10, numArray2[index11]];
                                    num18 += _multable[index10, numArray3[index11]];
                                    num17 += _multable[index10, numArray4[index11]];
                                }
                                else
                                {
                                    var index12 = num22 >= 0 ? (num22 <= rawBitmap1.Height - 1 ? num21 : length - (rawBitmap1.Width - index9)) : index9;
                                    num20 += _multable[index10, numArray5[index12]];
                                    num19 += _multable[index10, numArray6[index12]];
                                    num18 += _multable[index10, numArray7[index12]];
                                    num17 += _multable[index10, numArray8[index12]];
                                }
                                num21 += rawBitmap1.Width;
                                ++num22;
                            }
                            rawBitmap2.ArgbValues[index7] = (byte)(num20 / _kernelSum);
                            int num23;
                            rawBitmap2.ArgbValues[num23 = index7 + 1] = (byte)(num19 / _kernelSum);
                            int num24;
                            rawBitmap2.ArgbValues[num24 = num23 + 1] = (byte)(num18 / _kernelSum);
                            int num25;
                            rawBitmap2.ArgbValues[num25 = num24 + 1] = (byte)(num17 / _kernelSum);
                            index7 = num25 + 1;
                        }
                    }
                    return rawBitmap2.Bitmap;
                }
            }
        }

        [SvgAttribute("stdDeviation")]
        public float StdDeviation
        {
            get => _stdDeviation;
            set
            {
                _stdDeviation = (double)value > 0.0 ? value : throw new InvalidOperationException("Radius must be greater then 0");
                Attributes["stdDeviation"] = value;
                PreCalculate();
            }
        }

        public BlurType BlurType
        {
            get => _blurType;
            set => _blurType = value;
        }

        public override void Process(ImageBuffer buffer)
        {
            Bitmap bitmap = Apply(buffer[Input]);
            buffer[Result] = bitmap;
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgGaussianBlur>();
        }

        public override SvgElement DeepCopy<T>()
        {
            SvgGaussianBlur svgGaussianBlur = base.DeepCopy<T>() as SvgGaussianBlur;
            svgGaussianBlur.StdDeviation = StdDeviation;
            svgGaussianBlur.BlurType = BlurType;
            return svgGaussianBlur;
        }
    }
}
