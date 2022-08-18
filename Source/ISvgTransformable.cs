// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using Svg.Transforms;

namespace Svg
{
    public interface ISvgTransformable
    {
        SvgTransformCollection Transforms { get; set; }

        void PushTransforms(ISvgRenderer renderer);

        void PopTransforms(ISvgRenderer renderer);
    }
}
