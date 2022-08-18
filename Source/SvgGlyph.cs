// todo: add license

using System.Drawing.Drawing2D;
using Svg.Pathing;

namespace Svg
{
    [SvgElement("glyph")]
    public class SvgGlyph : SvgPathBasedElement, ISvgPathElement
    {
        private GraphicsPath _path;

        [SvgAttribute("d")]
        public SvgPathSegmentList PathData
        {
            get => GetAttribute<SvgPathSegmentList>("d", false);
            set
            {
                SvgPathSegmentList pathData = PathData;
                if (pathData != null)
                {
                    pathData.Owner = null;
                }

                Attributes["d"] = value;
                value.Owner = this;
            }
        }

        [SvgAttribute("glyph-name")]
        public virtual string GlyphName
        {
            get => GetAttribute<string>("glyph-name", true);
            set => Attributes["glyph-name"] = value;
        }

        [SvgAttribute("horiz-adv-x")]
        public float HorizAdvX
        {
            get => GetAttribute<float>("horiz-adv-x", true, Parents.OfType<SvgFont>().First<SvgFont>().HorizAdvX);
            set => Attributes["horiz-adv-x"] = value;
        }

        [SvgAttribute("unicode")]
        public string Unicode
        {
            get => GetAttribute<string>("unicode", true);
            set => Attributes["unicode"] = value;
        }

        [SvgAttribute("vert-adv-y")]
        public float VertAdvY
        {
            get => GetAttribute<float>("vert-adv-y", true, Parents.OfType<SvgFont>().First<SvgFont>().VertAdvY);
            set => Attributes["vert-adv-y"] = value;
        }

        [SvgAttribute("vert-origin-x")]
        public float VertOriginX
        {
            get => GetAttribute<float>("vert-origin-x", true, Parents.OfType<SvgFont>().First<SvgFont>().VertOriginX);
            set => Attributes["vert-origin-x"] = value;
        }

        [SvgAttribute("vert-origin-y")]
        public float VertOriginY
        {
            get => GetAttribute<float>("vert-origin-y", true, Parents.OfType<SvgFont>().First<SvgFont>().VertOriginY);
            set => Attributes["vert-origin-y"] = value;
        }

        public override GraphicsPath Path(ISvgRenderer renderer)
        {
            if (_path == null || IsPathDirty)
            {
                _path = new GraphicsPath();
                if (PathData != null)
                {
                    foreach (SvgPathSegment svgPathSegment in PathData)
                    {
                        svgPathSegment.AddToPath(_path);
                    }
                }
                IsPathDirty = false;
            }
            return _path;
        }

        public void OnPathUpdated()
        {
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgGlyph>();
        }

        public override SvgElement DeepCopy<T>()
        {
            SvgGlyph svgGlyph = base.DeepCopy<T>() as SvgGlyph;
            if (PathData != null)
            {
                SvgPathSegmentList svgPathSegmentList = new SvgPathSegmentList();
                foreach (SvgPathSegment svgPathSegment in PathData)
                {
                    svgPathSegmentList.Add(svgPathSegment.Clone());
                }

                svgGlyph.PathData = svgPathSegmentList;
            }
            return svgGlyph;
        }
    }
}
