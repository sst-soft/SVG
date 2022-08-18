// todo: add license

using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;

namespace Svg
{
    public class SvgColourConverter : ColorConverter
    {
        public override object ConvertFrom(
          ITypeDescriptorContext context,
          CultureInfo culture,
          object value)
        {
            if (value is string str1)
            {
                CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
                try
                {
                    Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                    var s1 = str1.Trim();
                    if (s1.StartsWith("rgb"))
                    {
                        try
                        {
                            var startIndex = s1.IndexOf("(") + 1;
                            var strArray = s1.Substring(startIndex, s1.IndexOf(")") - startIndex).Split(new char[2]
                            {
                ',',
                ' '
                            }, StringSplitOptions.RemoveEmptyEntries);
                            int num = byte.MaxValue;
                            if (strArray.Length > 3)
                            {
                                var s2 = strArray[3];
                                if (s2.StartsWith("."))
                                {
                                    s2 = "0" + s2;
                                }

                                var d = decimal.Parse(s2);
                                num = !(d <= 1M) ? (int)Math.Round(d) : (int)Math.Round(d * 255M);
                            }
                            return !strArray[0].Trim().EndsWith("%") ? Color.FromArgb(num, int.Parse(strArray[0]), int.Parse(strArray[1]), int.Parse(strArray[2])) : Color.FromArgb(num, (int)Math.Round(byte.MaxValue * (double)float.Parse(strArray[0].Trim().TrimEnd('%'), NumberStyles.Any, CultureInfo.InvariantCulture) / 100.0), (int)Math.Round(byte.MaxValue * (double)float.Parse(strArray[1].Trim().TrimEnd('%'), NumberStyles.Any, CultureInfo.InvariantCulture) / 100.0), (int)Math.Round(byte.MaxValue * (double)float.Parse(strArray[2].Trim().TrimEnd('%'), NumberStyles.Any, CultureInfo.InvariantCulture) / 100.0));
                        }
                        catch
                        {
                            throw new SvgException("Colour is in an invalid format: '" + s1 + "'");
                        }
                    }
                    else if (s1.StartsWith("hsl"))
                    {
                        try
                        {
                            var startIndex = s1.IndexOf("(") + 1;
                            var strArray = s1.Substring(startIndex, s1.IndexOf(")") - startIndex).Split(new char[2]
                            {
                ',',
                ' '
                            }, StringSplitOptions.RemoveEmptyEntries);
                            if (strArray[1].EndsWith("%"))
                            {
                                strArray[1] = strArray[1].TrimEnd('%');
                            }

                            if (strArray[2].EndsWith("%"))
                            {
                                strArray[2] = strArray[2].TrimEnd('%');
                            }

                            var h = double.Parse(strArray[0]) / 360.0;
                            var num1 = double.Parse(strArray[1]) / 100.0;
                            var num2 = double.Parse(strArray[2]) / 100.0;
                            var sl = num1;
                            var l = num2;
                            return SvgColourConverter.Hsl2Rgb(h, sl, l);
                        }
                        catch
                        {
                            throw new SvgException("Colour is in an invalid format: '" + s1 + "'");
                        }
                    }
                    else
                    {
                        if (s1.StartsWith("#") && s1.Length == 4)
                        {
                            var str = string.Format(culture, "#{0}{0}{1}{1}{2}{2}", s1[1], s1[2], s1[3]);
                            return base.ConvertFrom(context, culture, str);
                        }
                        switch (s1.ToLowerInvariant())
                        {
                            case "activeborder":
                                return SystemColors.ActiveBorder;
                            case "activecaption":
                                return SystemColors.ActiveCaption;
                            case "appworkspace":
                                return SystemColors.AppWorkspace;
                            case "background":
                                return SystemColors.Desktop;
                            case "buttonface":
                                return SystemColors.Control;
                            case "buttonhighlight":
                                return SystemColors.ControlLightLight;
                            case "buttonshadow":
                                return SystemColors.ControlDark;
                            case "buttontext":
                                return SystemColors.ControlText;
                            case "captiontext":
                                return SystemColors.ActiveCaptionText;
                            case "graytext":
                                return SystemColors.GrayText;
                            case "highlight":
                                return SystemColors.Highlight;
                            case "highlighttext":
                                return SystemColors.HighlightText;
                            case "inactiveborder":
                                return SystemColors.InactiveBorder;
                            case "inactivecaption":
                                return SystemColors.InactiveCaption;
                            case "inactivecaptiontext":
                                return SystemColors.InactiveCaptionText;
                            case "infobackground":
                                return SystemColors.Info;
                            case "infotext":
                                return SystemColors.InfoText;
                            case "menu":
                                return SystemColors.Menu;
                            case "menutext":
                                return SystemColors.MenuText;
                            case "scrollbar":
                                return SystemColors.ScrollBar;
                            case "threeddarkshadow":
                                return SystemColors.ControlDarkDark;
                            case "threedface":
                                return SystemColors.Control;
                            case "threedhighlight":
                                return SystemColors.ControlLight;
                            case "threedlightshadow":
                                return SystemColors.ControlLightLight;
                            case "window":
                                return SystemColors.Window;
                            case "windowframe":
                                return SystemColors.WindowFrame;
                            case "windowtext":
                                return SystemColors.WindowText;
                            default:
                                if (int.TryParse(s1, out var _))
                                {
                                    return SvgPaintServer.NotSet;
                                }

                                var startIndex = s1.LastIndexOf("grey", StringComparison.InvariantCultureIgnoreCase);
                                if (startIndex >= 0)
                                {
                                    if (startIndex + 4 == s1.Length)
                                    {
                                        value = new StringBuilder(s1).Replace("grey", "gray", startIndex, 4).Replace("Grey", "Gray", startIndex, 4).ToString();
                                        break;
                                    }
                                    break;
                                }
                                break;
                        }
                    }
                }
                finally
                {
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                }
            }
            return base.ConvertFrom(context, culture, value);
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
            if (!(destinationType == typeof(string)))
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }

            var str = ColorTranslator.ToHtml((Color)value).Replace("LightGrey", "LightGray");
            return !str.StartsWith("#") ? str.ToLower() : (object)str;
        }

        private static Color Hsl2Rgb(double h, double sl, double l)
        {
            var num1 = l;
            var num2 = l;
            var num3 = l;
            var num4 = l <= 0.5 ? l * (1.0 + sl) : l + sl - l * sl;
            if (num4 > 0.0)
            {
                var num5 = l + l - num4;
                var num6 = (num4 - num5) / num4;
                h *= 6.0;
                var num7 = (int)h;
                var num8 = h - num7;
                var num9 = num4 * num6 * num8;
                var num10 = num5 + num9;
                var num11 = num4 - num9;
                switch (num7)
                {
                    case 0:
                        num1 = num4;
                        num2 = num10;
                        num3 = num5;
                        break;
                    case 1:
                        num1 = num11;
                        num2 = num4;
                        num3 = num5;
                        break;
                    case 2:
                        num1 = num5;
                        num2 = num4;
                        num3 = num10;
                        break;
                    case 3:
                        num1 = num5;
                        num2 = num11;
                        num3 = num4;
                        break;
                    case 4:
                        num1 = num10;
                        num2 = num5;
                        num3 = num4;
                        break;
                    case 5:
                        num1 = num4;
                        num2 = num5;
                        num3 = num11;
                        break;
                }
            }
            return Color.FromArgb((int)Math.Round(num1 * byte.MaxValue), (int)Math.Round(num2 * byte.MaxValue), (int)Math.Round(num3 * byte.MaxValue));
        }
    }
}
