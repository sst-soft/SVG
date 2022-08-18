﻿// todo: add license

using System.ComponentModel;

namespace Svg
{
    [TypeConverter(typeof(SvgTextTransformationConverter))]
    [Flags]
    public enum SvgTextTransformation
    {
        Inherit = 0,
        None = 1,
        Capitalize = 2,
        Uppercase = 4,
        Lowercase = 8,
    }
}
