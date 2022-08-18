﻿// todo: add license

namespace Svg
{
    internal interface ISvgElement
    {
        SvgElement Parent { get; }

        SvgElementCollection Children { get; }

        IList<ISvgNode> Nodes { get; }

        void Render(ISvgRenderer renderer);
    }
}
