﻿// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.ComponentModel;

namespace Svg
{
    [TypeConverter(typeof(SvgTextDecorationConverter))]
    [Flags]
    public enum SvgTextDecoration
    {
        Inherit = 0,
        None = 1,
        Underline = 2,
        Overline = 4,
        LineThrough = 8,
        Blink = 16, // 0x00000010
    }
}
