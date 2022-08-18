// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

namespace Svg
{
    public class MouseArg : SVGArg
    {
        public float x;
        public float y;
        public int Button;
        public int ClickCount = -1;
        public bool AltKey;
        public bool ShiftKey;
        public bool CtrlKey;
    }
}
