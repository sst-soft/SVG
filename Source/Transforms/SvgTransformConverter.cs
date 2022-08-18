// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.ComponentModel;
using System.Globalization;

namespace Svg.Transforms
{
    public class SvgTransformConverter : TypeConverter
    {
        private static IEnumerable<string> SplitTransforms(string transforms)
        {
            transforms = transforms.Replace(',', ' ');
            var startIndex = 0;
            for (var i = 0; i < transforms.Length; ++i)
            {
                if (transforms[i] == ')')
                {
                    yield return transforms.Substring(startIndex, i + 1 - startIndex).TrimStart();
                    startIndex = i + 1;
                }
            }
        }

        public override object ConvertFrom(
          ITypeDescriptorContext context,
          CultureInfo culture,
          object value)
        {
            if (!(value is string))
            {
                return base.ConvertFrom(context, culture, value);
            }

            SvgTransformCollection transformCollection = new SvgTransformCollection();
            foreach (var splitTransform in SvgTransformConverter.SplitTransforms((string)value))
            {
                if (!string.IsNullOrEmpty(splitTransform))
                {
                    var strArray1 = splitTransform.Split('(', ')');
                    var str1 = strArray1[0].TrimEnd();
                    var s = strArray1[1].Trim();
                    switch (str1)
                    {
                        case "matrix":
                            var strArray2 = s.Split(new char[1]
                            {
                ' '
                            }, StringSplitOptions.RemoveEmptyEntries);
                            if (strArray2.Length != 6)
                            {
                                throw new FormatException("Matrix transforms must be in the format 'matrix(m11 m12 m21 m22 dx dy)'");
                            }

                            List<float> m = new List<float>(6);
                            foreach (var str2 in strArray2)
                            {
                                m.Add(float.Parse(str2.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture));
                            }

                            transformCollection.Add(new SvgMatrix(m));
                            continue;
                        case "rotate":
                            var strArray3 = s.Split(new char[1]
                            {
                ' '
                            }, StringSplitOptions.RemoveEmptyEntries);
                            var angle = strArray3.Length == 1 || strArray3.Length == 3 ? float.Parse(strArray3[0], NumberStyles.Float, CultureInfo.InvariantCulture) : throw new FormatException("Rotate transforms must be in the format 'rotate(angle [cx cy])'");
                            if (strArray3.Length == 1)
                            {
                                transformCollection.Add(new SvgRotate(angle));
                                continue;
                            }
                            var centerX = float.Parse(strArray3[1], NumberStyles.Float, CultureInfo.InvariantCulture);
                            var centerY = float.Parse(strArray3[2], NumberStyles.Float, CultureInfo.InvariantCulture);
                            transformCollection.Add(new SvgRotate(angle, centerX, centerY));
                            continue;
                        case "scale":
                            var strArray4 = s.Split(new char[1]
                            {
                ' '
                            }, StringSplitOptions.RemoveEmptyEntries);
                            var x1 = strArray4.Length != 0 && strArray4.Length <= 2 ? float.Parse(strArray4[0].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture) : throw new FormatException("Scale transforms must be in the format 'scale(x [y])'");
                            if (strArray4.Length > 1)
                            {
                                var y = float.Parse(strArray4[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture);
                                transformCollection.Add(new SvgScale(x1, y));
                                continue;
                            }
                            transformCollection.Add(new SvgScale(x1));
                            continue;
                        case "shear":
                            var strArray5 = s.Split(new char[1]
                            {
                ' '
                            }, StringSplitOptions.RemoveEmptyEntries);
                            var x2 = strArray5.Length != 0 && strArray5.Length <= 2 ? float.Parse(strArray5[0].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture) : throw new FormatException("Shear transforms must be in the format 'shear(x [y])'");
                            if (strArray5.Length > 1)
                            {
                                var y = float.Parse(strArray5[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture);
                                transformCollection.Add(new SvgShear(x2, y));
                                continue;
                            }
                            transformCollection.Add(new SvgShear(x2));
                            continue;
                        case "skewX":
                            var x3 = float.Parse(s, NumberStyles.Float, CultureInfo.InvariantCulture);
                            transformCollection.Add(new SvgSkew(x3, 0.0f));
                            continue;
                        case "skewY":
                            var y1 = float.Parse(s, NumberStyles.Float, CultureInfo.InvariantCulture);
                            transformCollection.Add(new SvgSkew(0.0f, y1));
                            continue;
                        case "translate":
                            var strArray6 = s.Split(new char[1]
                            {
                ' '
                            }, StringSplitOptions.RemoveEmptyEntries);
                            var x4 = strArray6.Length != 0 && strArray6.Length <= 2 ? float.Parse(strArray6[0].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture) : throw new FormatException("Translate transforms must be in the format 'translate(x [y])'");
                            if (strArray6.Length > 1)
                            {
                                var y2 = float.Parse(strArray6[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture);
                                transformCollection.Add(new SvgTranslate(x4, y2));
                                continue;
                            }
                            transformCollection.Add(new SvgTranslate(x4));
                            continue;
                        default:
                            continue;
                    }
                }
            }
            return transformCollection;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value,
      Type destinationType)
        {
            return destinationType == typeof(string) && value is SvgTransformCollection ? string.Join(" ", ((IEnumerable<SvgTransform>)value).Select<SvgTransform, string>(t => t.WriteToString()).ToArray<string>()) : base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
