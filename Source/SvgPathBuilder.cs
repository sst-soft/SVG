// todo: add license

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using Svg.Pathing;

namespace Svg
{
    public class SvgPathBuilder : TypeConverter
    {
        public static SvgPathSegmentList Parse(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            SvgPathSegmentList segments = new SvgPathSegmentList();
            try
            {
                foreach (var splitCommand in SvgPathBuilder.SplitCommands(path.TrimEnd(null)))
                {
                    int num = splitCommand[0];
                    var isRelative = char.IsLower((char)num);
                    SvgPathBuilder.CreatePathSegment((char)num, segments, new CoordinateParser(splitCommand.Trim()), isRelative);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error parsing path \"{0}\": {1}", path, ex.Message);
            }
            return segments;
        }

        private static void CreatePathSegment(
          char command,
          SvgPathSegmentList segments,
          CoordinateParser parser,
          bool isRelative)
        {
            var numArray = new float[6];
            switch (command)
            {
                case 'A':
                case 'a':
                    bool result1;
                    bool result2;
                    while (parser.TryGetFloat(out numArray[0]) && parser.TryGetFloat(out numArray[1]) && parser.TryGetFloat(out numArray[2]) && parser.TryGetBool(out result1) && parser.TryGetBool(out result2) && parser.TryGetFloat(out numArray[3]) && parser.TryGetFloat(out numArray[4]))
                    {
                        segments.Add(new SvgArcSegment(segments.Last.End, numArray[0], numArray[1], numArray[2], result1 ? SvgArcSize.Large : SvgArcSize.Small, result2 ? SvgArcSweep.Positive : SvgArcSweep.Negative, SvgPathBuilder.ToAbsolute(numArray[3], numArray[4], segments, isRelative)));
                    }

                    break;
                case 'C':
                case 'c':
                    while (parser.TryGetFloat(out numArray[0]) && parser.TryGetFloat(out numArray[1]) && parser.TryGetFloat(out numArray[2]) && parser.TryGetFloat(out numArray[3]) && parser.TryGetFloat(out numArray[4]) && parser.TryGetFloat(out numArray[5]))
                    {
                        segments.Add(new SvgCubicCurveSegment(segments.Last.End, SvgPathBuilder.ToAbsolute(numArray[0], numArray[1], segments, isRelative), SvgPathBuilder.ToAbsolute(numArray[2], numArray[3], segments, isRelative), SvgPathBuilder.ToAbsolute(numArray[4], numArray[5], segments, isRelative)));
                    }

                    break;
                case 'H':
                case 'h':
                    while (parser.TryGetFloat(out numArray[0]))
                    {
                        segments.Add(new SvgLineSegment(segments.Last.End, SvgPathBuilder.ToAbsolute(numArray[0], segments.Last.End.Y, segments, isRelative, false)));
                    }

                    break;
                case 'L':
                case 'l':
                    while (parser.TryGetFloat(out numArray[0]) && parser.TryGetFloat(out numArray[1]))
                    {
                        segments.Add(new SvgLineSegment(segments.Last.End, SvgPathBuilder.ToAbsolute(numArray[0], numArray[1], segments, isRelative)));
                    }

                    break;
                case 'M':
                case 'm':
                    if (parser.TryGetFloat(out numArray[0]) && parser.TryGetFloat(out numArray[1]))
                    {
                        segments.Add(new SvgMoveToSegment(SvgPathBuilder.ToAbsolute(numArray[0], numArray[1], segments, isRelative)));
                    }

                    while (parser.TryGetFloat(out numArray[0]) && parser.TryGetFloat(out numArray[1]))
                    {
                        segments.Add(new SvgLineSegment(segments.Last.End, SvgPathBuilder.ToAbsolute(numArray[0], numArray[1], segments, isRelative)));
                    }

                    break;
                case 'Q':
                case 'q':
                    while (parser.TryGetFloat(out numArray[0]) && parser.TryGetFloat(out numArray[1]) && parser.TryGetFloat(out numArray[2]) && parser.TryGetFloat(out numArray[3]))
                    {
                        segments.Add(new SvgQuadraticCurveSegment(segments.Last.End, SvgPathBuilder.ToAbsolute(numArray[0], numArray[1], segments, isRelative), SvgPathBuilder.ToAbsolute(numArray[2], numArray[3], segments, isRelative)));
                    }

                    break;
                case 'S':
                case 's':
                    while (parser.TryGetFloat(out numArray[0]) && parser.TryGetFloat(out numArray[1]) && parser.TryGetFloat(out numArray[2]) && parser.TryGetFloat(out numArray[3]))
                    {
                        PointF firstControlPoint = segments.Last is SvgCubicCurveSegment last ? SvgPathBuilder.Reflect(last.SecondControlPoint, segments.Last.End) : segments.Last.End;
                        segments.Add(new SvgCubicCurveSegment(segments.Last.End, firstControlPoint, SvgPathBuilder.ToAbsolute(numArray[0], numArray[1], segments, isRelative), SvgPathBuilder.ToAbsolute(numArray[2], numArray[3], segments, isRelative)));
                    }
                    break;
                case 'T':
                case 't':
                    while (parser.TryGetFloat(out numArray[0]) && parser.TryGetFloat(out numArray[1]))
                    {
                        PointF controlPoint = segments.Last is SvgQuadraticCurveSegment last ? SvgPathBuilder.Reflect(last.ControlPoint, segments.Last.End) : segments.Last.End;
                        segments.Add(new SvgQuadraticCurveSegment(segments.Last.End, controlPoint, SvgPathBuilder.ToAbsolute(numArray[0], numArray[1], segments, isRelative)));
                    }
                    break;
                case 'V':
                case 'v':
                    while (parser.TryGetFloat(out numArray[0]))
                    {
                        segments.Add(new SvgLineSegment(segments.Last.End, SvgPathBuilder.ToAbsolute(segments.Last.End.X, numArray[0], segments, false, isRelative)));
                    }

                    break;
                case 'Z':
                case 'z':
                    segments.Add(new SvgClosePathSegment());
                    break;
            }
        }

        private static PointF Reflect(PointF point, PointF mirror)
        {
            var num1 = Math.Abs(mirror.X - point.X);
            var num2 = Math.Abs(mirror.Y - point.Y);
            return new PointF(mirror.X + ((double)mirror.X >= (double)point.X ? num1 : -num1), mirror.Y + ((double)mirror.Y >= (double)point.Y ? num2 : -num2));
        }

        private static PointF ToAbsolute(
          float x,
          float y,
          SvgPathSegmentList segments,
          bool isRelativeBoth)
        {
            return SvgPathBuilder.ToAbsolute(x, y, segments, isRelativeBoth, isRelativeBoth);
        }

        private static PointF ToAbsolute(
          float x,
          float y,
          SvgPathSegmentList segments,
          bool isRelativeX,
          bool isRelativeY)
        {
            PointF absolute = new PointF(x, y);
            if (isRelativeX | isRelativeY && segments.Count > 0)
            {
                SvgPathSegment svgPathSegment = segments.Last;
                if (svgPathSegment is SvgClosePathSegment)
                {
                    svgPathSegment = segments.Reverse<SvgPathSegment>().OfType<SvgMoveToSegment>().First<SvgMoveToSegment>();
                }

                if (isRelativeX)
                {
                    absolute.X += svgPathSegment.End.X;
                }

                if (isRelativeY)
                {
                    absolute.Y += svgPathSegment.End.Y;
                }
            }
            return absolute;
        }

        private static IEnumerable<string> SplitCommands(string path)
        {
            var commandStart = 0;
            for (var i = 0; i < path.Length; ++i)
            {
                if (char.IsLetter(path[i]) && path[i] != 'e' && path[i] != 'E')
                {
                    var str = path.Substring(commandStart, i - commandStart).Trim();
                    commandStart = i;
                    if (!string.IsNullOrEmpty(str))
                    {
                        yield return str;
                    }

                    if (path.Length == i + 1)
                    {
                        yield return path[i].ToString();
                    }
                }
                else if (path.Length == i + 1)
                {
                    var str = path.Substring(commandStart, i - commandStart + 1).Trim();
                    if (!string.IsNullOrEmpty(str))
                    {
                        yield return str;
                    }
                }
            }
        }

        public override object ConvertFrom(
          ITypeDescriptorContext context,
          CultureInfo culture,
          object value)
        {
            return value is string ? SvgPathBuilder.Parse((string)value) : base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(
          ITypeDescriptorContext context,
          CultureInfo culture,
          object value,
          Type destinationType)
        {
            return destinationType == typeof(string) && value is SvgPathSegmentList source ? string.Join(" ", source.Select<SvgPathSegment, string>(p => p.ToString()).ToArray<string>()) : base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }
    }
}
