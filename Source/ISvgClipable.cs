// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

namespace Svg
{
    public interface ISvgClipable
    {
        Uri ClipPath { get; set; }

        SvgClipRule ClipRule { get; set; }

        void SetClip(ISvgRenderer renderer);

        void ResetClip(ISvgRenderer renderer);
    }
}
