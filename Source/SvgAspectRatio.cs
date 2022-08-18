// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.ComponentModel;
using Svg.DataTypes;

namespace Svg
{
    [TypeConverter(typeof(SvgPreserveAspectRatioConverter))]
    public class SvgAspectRatio
    {
        public SvgAspectRatio()
          : this(SvgPreserveAspectRatio.none)
        {
        }

        public SvgAspectRatio(SvgPreserveAspectRatio align)
          : this(align, false)
        {
        }

        public SvgAspectRatio(SvgPreserveAspectRatio align, bool slice)
          : this(align, slice, false)
        {
        }

        public SvgAspectRatio(SvgPreserveAspectRatio align, bool slice, bool defer)
        {
            Align = align;
            Slice = slice;
            Defer = defer;
        }

        public SvgPreserveAspectRatio Align { get; set; }

        public bool Slice { get; set; }

        public bool Defer { get; set; }

        public override string ToString()
        {
            return TypeDescriptor.GetConverter(typeof(SvgPreserveAspectRatio)).ConvertToString(Align) + (Slice ? " slice" : "");
        }
    }
}
