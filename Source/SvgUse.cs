// todo: add license

using System.Drawing;
using System.Drawing.Drawing2D;

namespace Svg
{
    [SvgElement("use")]
    public class SvgUse : SvgVisualElement
    {
        [SvgAttribute("href", "http://www.w3.org/1999/xlink")]
        public virtual Uri ReferencedElement
        {
            get => GetAttribute<Uri>("href", false);
            set => Attributes["href"] = value;
        }

        private bool ElementReferencesUri(SvgElement element, List<Uri> elementUris)
        {
            switch (element)
            {
                case SvgUse svgUse:
                    if (elementUris.Contains(svgUse.ReferencedElement))
                    {
                        return true;
                    }

                    if (OwnerDocument.IdManager.GetElementById(svgUse.ReferencedElement) is SvgUse)
                    {
                        elementUris.Add(svgUse.ReferencedElement);
                    }

                    return svgUse.ReferencedElementReferencesUri(elementUris);
                case SvgGroup svgGroup:
                    using (IEnumerator<SvgElement> enumerator = svgGroup.Children.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            if (ElementReferencesUri(enumerator.Current, elementUris))
                            {
                                return true;
                            }
                        }
                        break;
                    }
            }
            return false;
        }

        private bool ReferencedElementReferencesUri(List<Uri> elementUris)
        {
            return ElementReferencesUri(OwnerDocument.IdManager.GetElementById(ReferencedElement), elementUris);
        }

        private bool HasRecursiveReference()
        {
            return ElementReferencesUri(OwnerDocument.IdManager.GetElementById(ReferencedElement), new List<Uri>()
    {
      ReferencedElement
    });
        }

        [SvgAttribute("x")]
        public virtual SvgUnit X
        {
            get => GetAttribute<SvgUnit>("x", false, (SvgUnit)0.0f);
            set => Attributes["x"] = value;
        }

        [SvgAttribute("y")]
        public virtual SvgUnit Y
        {
            get => GetAttribute<SvgUnit>("y", false, (SvgUnit)0.0f);
            set => Attributes["y"] = value;
        }

        [SvgAttribute("width")]
        public virtual SvgUnit Width
        {
            get => GetAttribute<SvgUnit>("width", false, (SvgUnit)0.0f);
            set => Attributes["width"] = value;
        }

        [SvgAttribute("height")]
        public virtual SvgUnit Height
        {
            get => GetAttribute<SvgUnit>("height", false, (SvgUnit)0.0f);
            set => Attributes["height"] = value;
        }

        protected internal override bool PushTransforms(ISvgRenderer renderer)
        {
            if (!base.PushTransforms(renderer))
            {
                return false;
            }

            renderer.TranslateTransform(X.ToDeviceValue(renderer, UnitRenderingType.Horizontal, this), Y.ToDeviceValue(renderer, UnitRenderingType.Vertical, this), 0);
            return true;
        }

        public override GraphicsPath Path(ISvgRenderer renderer)
        {
            SvgVisualElement elementById = (SvgVisualElement)OwnerDocument.IdManager.GetElementById(ReferencedElement);
            return elementById == null || HasRecursiveReference() ? null : elementById.Path(renderer);
        }

        public SvgPoint Location => new SvgPoint(X, Y);

        public override RectangleF Bounds
        {
            get
            {
                var deviceValue1 = Width.ToDeviceValue(null, UnitRenderingType.Horizontal, this);
                var deviceValue2 = Height.ToDeviceValue(null, UnitRenderingType.Vertical, this);
                if ((double)deviceValue1 > 0.0 && (double)deviceValue2 > 0.0)
                {
                    return TransformedBounds(new RectangleF(Location.ToDeviceValue(null, this), new SizeF(deviceValue1, deviceValue2)));
                }

                return OwnerDocument.IdManager.GetElementById(ReferencedElement) is SvgVisualElement elementById ? elementById.Bounds : new RectangleF();
            }
        }

        protected override bool Renderable => false;

        protected override void Render(ISvgRenderer renderer)
        {
            if (!Visible || !Displayable || !(ReferencedElement != null))
            {
                return;
            }

            if (HasRecursiveReference())
            {
                return;
            }

            try
            {
                if (!PushTransforms(renderer))
                {
                    return;
                }

                SetClip(renderer);
                if (OwnerDocument.IdManager.GetElementById(ReferencedElement) is SvgVisualElement elementById)
                {
                    var deviceValue1 = Width.ToDeviceValue(renderer, UnitRenderingType.Horizontal, this);
                    var deviceValue2 = Height.ToDeviceValue(renderer, UnitRenderingType.Vertical, this);
                    if ((double)deviceValue1 > 0.0 && (double)deviceValue2 > 0.0)
                    {
                        SvgViewBox attribute = elementById.Attributes.GetAttribute<SvgViewBox>("viewBox");
                        if (attribute != SvgViewBox.Empty && (double)Math.Abs(deviceValue1 - attribute.Width) > 1.4012984643248171E-45 && (double)Math.Abs(deviceValue2 - attribute.Height) > 1.4012984643248171E-45)
                        {
                            var sx = deviceValue1 / attribute.Width;
                            var sy = deviceValue2 / attribute.Height;
                            renderer.ScaleTransform(sx, sy, 0);
                        }
                    }
                    SvgElement parent = elementById.Parent;
                    elementById._parent = this;
                    elementById.InvalidateChildPaths();
                    elementById.RenderElement(renderer);
                    elementById._parent = parent;
                }
                ResetClip(renderer);
            }
            finally
            {
                PopTransforms(renderer);
            }
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgUse>();
        }

        public override SvgElement DeepCopy<T>()
        {
            SvgUse svgUse = base.DeepCopy<T>() as SvgUse;
            svgUse.ReferencedElement = ReferencedElement;
            svgUse.X = X;
            svgUse.Y = Y;
            return svgUse;
        }
    }
}
