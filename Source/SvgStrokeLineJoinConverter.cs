﻿// todo: add license

namespace Svg
{
    public sealed class SvgStrokeLineJoinConverter : EnumBaseConverter<SvgStrokeLineJoin>
    {
        public SvgStrokeLineJoinConverter()
          : base(SvgStrokeLineJoin.Miter)
        {
        }
    }
}
