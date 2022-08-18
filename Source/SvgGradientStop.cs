// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.ComponentModel;
using System.Drawing;

namespace Svg
{
    [SvgElement("stop")]
    public class SvgGradientStop : SvgElement
    {
        private SvgUnit _offset = (SvgUnit)0.0f;

        [SvgAttribute("offset")]
        public SvgUnit Offset
        {
            get => _offset;
            set
            {
                SvgUnit svgUnit = value;
                if (value.Type == SvgUnitType.Percentage)
                {
                    if ((double)value.Value > 100.0)
                    {
                        svgUnit = new SvgUnit(value.Type, 100f);
                    }
                    else if ((double)value.Value < 0.0)
                    {
                        svgUnit = new SvgUnit(value.Type, 0.0f);
                    }
                }
                else if (value.Type == SvgUnitType.User)
                {
                    if ((double)value.Value > 1.0)
                    {
                        svgUnit = new SvgUnit(value.Type, 1f);
                    }
                    else if ((double)value.Value < 0.0)
                    {
                        svgUnit = new SvgUnit(value.Type, 0.0f);
                    }
                }
                _offset = svgUnit.ToPercentage();
                Attributes["offset"] = svgUnit;
            }
        }

        [SvgAttribute("stop-color")]
        [TypeConverter(typeof(SvgPaintServerFactory))]
        public SvgPaintServer StopColor
        {
            get => GetAttribute<SvgPaintServer>("stop-color", true, new SvgColourServer(System.Drawing.Color.Black));
            set => Attributes["stop-color"] = value;
        }

        [SvgAttribute("stop-opacity")]
        public float StopOpacity
        {
            get => GetAttribute<float>("stop-opacity", true, 1f);
            set => Attributes["stop-opacity"] = SvgElement.FixOpacityValue(value);
        }

        public SvgGradientStop()
        {
            _offset = new SvgUnit(0.0f);
        }

        public SvgGradientStop(SvgUnit offset, Color colour)
        {
            _offset = offset;
        }

        public Color GetColor(SvgElement parent)
        {
            return (SvgDeferredPaintServer.TryGet<SvgColourServer>(StopColor, parent) ?? throw new InvalidOperationException("Invalid paint server for gradient stop detected.")).Colour;
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgGradientStop>();
        }

        public override SvgElement DeepCopy<T>()
        {
            SvgGradientStop svgGradientStop = base.DeepCopy<T>() as SvgGradientStop;
            svgGradientStop.Offset = Offset;
            return svgGradientStop;
        }
    }
}
