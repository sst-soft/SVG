﻿// todo: add license

namespace Svg
{
    public interface ISvgNode
    {
        string Content { get; }

        ISvgNode DeepCopy();
    }
}
