// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using ExCSS;
using Fizzler;

namespace Svg.Css
{
    internal static class CssQuery
    {
        public static IEnumerable<SvgElement> QuerySelectorAll(
          this SvgElement elem,
          string selector,
          SvgElementFactory elementFactory)
        {
            SelectorGenerator<SvgElement> generator = new SelectorGenerator<SvgElement>((IElementOps<SvgElement>)new SvgElementOps(elementFactory));
            Fizzler.Parser.Parse<SelectorGenerator<SvgElement>>(selector, generator);
            return generator.Selector(Enumerable.Repeat<SvgElement>(elem, 1));
        }

        public static int GetSpecificity(this BaseSelector selector)
        {
            switch (selector)
            {
                case SimpleSelector _:
                    var lowerInvariant = selector.ToString().ToLowerInvariant();
                    if (lowerInvariant.StartsWith(":not("))
                    {
                        return new SimpleSelector(lowerInvariant.Substring(5, lowerInvariant.Length - 6)).GetSpecificity();
                    }

                    if (lowerInvariant.StartsWith("#"))
                    {
                        return 4096;
                    }

                    if (lowerInvariant.StartsWith("::") || lowerInvariant == ":after" || lowerInvariant == ":before" || lowerInvariant == ":first-letter" || lowerInvariant == ":first-line" || lowerInvariant == ":selection")
                    {
                        return 16;
                    }

                    if (lowerInvariant.StartsWith(".") || lowerInvariant.StartsWith(":") || lowerInvariant.StartsWith("["))
                    {
                        return 256;
                    }

                    return lowerInvariant.Equals("*") ? 0 : 16;
                case IEnumerable<BaseSelector> source1:
                    return source1.Select<BaseSelector, int>(s => s.GetSpecificity()).Aggregate<int>((p, c) => p + c);
                case IEnumerable<CombinatorSelector> source2:
                    return source2.Select<CombinatorSelector, int>(s => s.Selector.GetSpecificity()).Aggregate<int>((p, c) => p + c);
                default:
                    return 0;
            }
        }
    }
}
