// todo: add license

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Reflection;
using System.Xml;
using Svg.Document_Structure;
using Svg.Transforms;

namespace Svg
{
    public abstract class SvgElement : ISvgElement, ISvgTransformable, ICloneable, ISvgNode
    {
        internal const int StyleSpecificity_PresAttribute = 0;
        internal const int StyleSpecificity_InlineStyle = 65536;
        private readonly IEnumerable<SvgElement.PropertyAttributeTuple> _svgPropertyAttributes;
        private readonly IEnumerable<SvgElement.EventAttributeTuple> _svgEventAttributes;
        internal SvgElement _parent;
        private string _elementName;
        private SvgAttributeCollection _attributes;
        private readonly EventHandlerList _eventHandlers;
        private readonly SvgElementCollection _children;
        private static readonly object _loadEventKey = new object();
        private Matrix _graphicsTransform;
        private Region _graphicsClip;
        private readonly SvgCustomAttributeCollection _customAttributes;
        private readonly List<ISvgNode> _nodes = new List<ISvgNode>();
        private Dictionary<string, SortedDictionary<int, string>> _styles = new Dictionary<string, SortedDictionary<int, string>>();
        private string _content;
        public bool AutoPublishEvents = true;
        private bool _dirty;
        public static PrivateFontCollection PrivateFonts = new PrivateFontCollection();

        public void AddStyle(string name, string value, int specificity)
        {
            if (!_styles.TryGetValue(name, out SortedDictionary<int, string> sortedDictionary))
            {
                sortedDictionary = new SortedDictionary<int, string>();
                _styles[name] = sortedDictionary;
            }
            while (sortedDictionary.ContainsKey(specificity))
            {
                ++specificity;
            }

            sortedDictionary[specificity] = value;
        }

        public void FlushStyles(bool children = false)
        {
            FlushStyles();
            if (!children)
            {
                return;
            }

            foreach (SvgElement child in Children)
            {
                child.FlushStyles(children);
            }
        }

        private void FlushStyles()
        {
            if (!_styles.Any<KeyValuePair<string, SortedDictionary<int, string>>>())
            {
                return;
            }

            Dictionary<string, SortedDictionary<int, string>> dictionary = new Dictionary<string, SortedDictionary<int, string>>();
            foreach (KeyValuePair<string, SortedDictionary<int, string>> style in _styles)
            {
                if (!SvgElementFactory.SetPropertyValue(this, style.Key, style.Value.Last<KeyValuePair<int, string>>().Value, OwnerDocument, true))
                {
                    dictionary.Add(style.Key, style.Value);
                }
            }
            _styles = dictionary;
        }

        public bool ContainsAttribute(string name)
        {
            if (Attributes.ContainsKey(name) || CustomAttributes.ContainsKey(name))
            {
                return true;
            }

            if (!_styles.TryGetValue(name, out SortedDictionary<int, string> sortedDictionary))
            {
                return false;
            }

            return sortedDictionary.ContainsKey(65536) || sortedDictionary.ContainsKey(0);
        }

        public bool TryGetAttribute(string name, out string value)
        {
            if (Attributes.TryGetValue(name, out var obj))
            {
                value = obj.ToString();
                return true;
            }
            return CustomAttributes.TryGetValue(name, out value) || _styles.TryGetValue(name, out SortedDictionary<int, string> sortedDictionary) && (sortedDictionary.TryGetValue(65536, out value) || sortedDictionary.TryGetValue(0, out value));
        }

        protected internal string ElementName
        {
            get
            {
                if (string.IsNullOrEmpty(_elementName))
                {
                    SvgElementAttribute elementAttribute = TypeDescriptor.GetAttributes(this).OfType<SvgElementAttribute>().SingleOrDefault<SvgElementAttribute>();
                    if (elementAttribute != null)
                    {
                        _elementName = elementAttribute.ElementName;
                    }
                }
                return _elementName;
            }
            internal set => _elementName = value;
        }

        [SvgAttribute("color")]
        public virtual SvgPaintServer Color
        {
            get => GetAttribute<SvgPaintServer>("color", true, SvgPaintServer.NotSet);
            set => Attributes["color"] = value;
        }

        public virtual string Content
        {
            get => _content;
            set
            {
                if (_content != null)
                {
                    var content = _content;
                    _content = value;
                    if (!(_content != content))
                    {
                        return;
                    }

                    OnContentChanged(new ContentEventArgs()
                    {
                        Content = value
                    });
                }
                else
                {
                    _content = value;
                    OnContentChanged(new ContentEventArgs()
                    {
                        Content = value
                    });
                }
            }
        }

        protected virtual EventHandlerList Events => _eventHandlers;

        public event EventHandler Load
        {
            add => Events.AddHandler(SvgElement._loadEventKey, value);
            remove => Events.RemoveHandler(SvgElement._loadEventKey, value);
        }

        public virtual SvgElementCollection Children => _children;

        public IList<ISvgNode> Nodes => _nodes;

        public IEnumerable<SvgElement> Descendants()
        {
            return AsEnumerable().Descendants<SvgElement>();
        }

        private IEnumerable<SvgElement> AsEnumerable()
        {
            yield return this;
        }

        public virtual bool HasChildren()
        {
            return Children.Count > 0;
        }

        public virtual SvgElement Parent => _parent;

        public IEnumerable<SvgElement> Parents
        {
            get
            {
                SvgElement curr = this;
                while (curr.Parent != null)
                {
                    curr = curr.Parent;
                    yield return curr;
                }
            }
        }

        public IEnumerable<SvgElement> ParentsAndSelf
        {
            get
            {
                SvgElement curr = this;
                yield return curr;
                while (curr.Parent != null)
                {
                    curr = curr.Parent;
                    yield return curr;
                }
            }
        }

        public virtual SvgDocument OwnerDocument
        {
            get
            {
                if (this is SvgDocument)
                {
                    return this as SvgDocument;
                }

                return Parent != null ? Parent.OwnerDocument : null;
            }
        }

        protected internal virtual SvgAttributeCollection Attributes
        {
            get
            {
                if (_attributes == null)
                {
                    _attributes = new SvgAttributeCollection(this);
                }

                return _attributes;
            }
        }

        protected bool Writing { get; set; }

        protected internal TAttributeType GetAttribute<TAttributeType>(
          string attributeName,
          bool inherited,
          TAttributeType defaultValue = default(TAttributeType)) //PIX = null)
        {
            return Writing ? Attributes.GetAttribute<TAttributeType>(attributeName, defaultValue) : Attributes.GetInheritedAttribute<TAttributeType>(attributeName, inherited, defaultValue);
        }

        public SvgCustomAttributeCollection CustomAttributes => _customAttributes;

        protected internal virtual bool PushTransforms(ISvgRenderer renderer)
        {
            _graphicsTransform = renderer.Transform;
            _graphicsClip = renderer.GetClip();
            SvgTransformCollection transforms = Transforms;
            if (transforms == null || transforms.Count == 0)
            {
                return true;
            }

            using (Matrix matrix1 = transforms.GetMatrix())
            {
                using (Matrix matrix2 = new Matrix(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f))
                {
                    if (matrix2.Equals(matrix1))
                    {
                        return false;
                    }
                }
                using (Matrix matrix3 = _graphicsTransform.Clone())
                {
                    matrix3.Multiply(matrix1);
                    renderer.Transform = matrix3;
                }
            }
            return true;
        }

        protected internal virtual void PopTransforms(ISvgRenderer renderer)
        {
            renderer.Transform = _graphicsTransform;
            _graphicsTransform.Dispose();
            _graphicsTransform = null;
            renderer.SetClip(_graphicsClip);
            _graphicsClip = null;
        }

        void ISvgTransformable.PushTransforms(ISvgRenderer renderer)
        {
            PushTransforms(renderer);
        }

        void ISvgTransformable.PopTransforms(ISvgRenderer renderer)
        {
            PopTransforms(renderer);
        }

        [SvgAttribute("transform")]
        public SvgTransformCollection Transforms
        {
            get => GetAttribute<SvgTransformCollection>("transform", false);
            set
            {
                SvgTransformCollection transforms = Transforms;
                if (transforms != null)
                {
                    transforms.TransformChanged -= new EventHandler<AttributeEventArgs>(Attributes_AttributeChanged);
                }

                value.TransformChanged += new EventHandler<AttributeEventArgs>(Attributes_AttributeChanged);
                Attributes["transform"] = value;
            }
        }

        protected RectangleF TransformedBounds(RectangleF bounds)
        {
            if (Transforms == null || Transforms.Count <= 0)
            {
                return bounds;
            }

            using (GraphicsPath graphicsPath = new GraphicsPath())
            {
                using (Matrix matrix = Transforms.GetMatrix())
                {
                    graphicsPath.AddRectangle(bounds);
                    graphicsPath.Transform(matrix);
                    return graphicsPath.GetBounds();
                }
            }
        }

        [SvgAttribute("id")]
        public string ID
        {
            get => GetAttribute<string>("id", false);
            set => SetAndForceUniqueID(value, false);
        }

        [SvgAttribute("space", "http://www.w3.org/XML/1998/namespace")]
        public virtual XmlSpaceHandling SpaceHandling
        {
            get => GetAttribute<XmlSpaceHandling>("space", true, XmlSpaceHandling.inherit);
            set => Attributes["space"] = value;
        }

        public void SetAndForceUniqueID(
          string value,
          bool autoForceUniqueID = true,
          Action<SvgElement, string, string> logElementOldIDNewID = null)
        {
            if (string.Compare(ID, value) == 0)
            {
                return;
            }

            if (OwnerDocument != null)
            {
                OwnerDocument.IdManager.Remove(this);
            }

            Attributes["id"] = value;
            if (OwnerDocument == null)
            {
                return;
            }

            OwnerDocument.IdManager.AddAndForceUniqueID(this, null, autoForceUniqueID, logElementOldIDNewID);
        }

        internal void ForceUniqueID(string newID)
        {
            Attributes["id"] = newID;
        }

        protected virtual void AddElement(SvgElement child, int index)
        {
        }

        public event EventHandler<ChildAddedEventArgs> ChildAdded;

        internal void OnElementAdded(SvgElement child, int index)
        {
            AddElement(child, index);
            SvgElement svgElement = null;
            if (index < Children.Count - 1)
            {
                svgElement = Children[index + 1];
            }

            EventHandler<ChildAddedEventArgs> childAdded = ChildAdded;
            if (childAdded == null)
            {
                return;
            }

            childAdded(this, new ChildAddedEventArgs()
            {
                NewChild = child,
                BeforeSibling = svgElement
            });
        }

        protected virtual void RemoveElement(SvgElement child)
        {
        }

        internal void OnElementRemoved(SvgElement child)
        {
            RemoveElement(child);
        }

        public SvgElement()
        {
            _children = new SvgElementCollection(this);
            _eventHandlers = new EventHandlerList();
            _elementName = string.Empty;
            _customAttributes = new SvgCustomAttributeCollection(this);
            Attributes.AttributeChanged += new EventHandler<AttributeEventArgs>(Attributes_AttributeChanged);
            CustomAttributes.AttributeChanged += new EventHandler<AttributeEventArgs>(Attributes_AttributeChanged);
            _svgPropertyAttributes = TypeDescriptor.GetProperties(this).Cast<PropertyDescriptor>().Select(a => new
            {
                a = a,
                attribute = a.Attributes[typeof(SvgAttributeAttribute)] as SvgAttributeAttribute
            }).Where(_param1 => _param1.attribute != null).Select(_param1 => new SvgElement.PropertyAttributeTuple()
            {
                Property = _param1.a,
                Attribute = _param1.attribute
            });
            _svgEventAttributes = TypeDescriptor.GetEvents(this).Cast<EventDescriptor>().Select(a => new
            {
                a = a,
                attribute = a.Attributes[typeof(SvgAttributeAttribute)] as SvgAttributeAttribute
            }).Where(_param1 => _param1.attribute != null).Select(_param1 => new SvgElement.EventAttributeTuple()
            {
                Event = _param1.a.ComponentType.GetField(_param1.a.Name, (BindingFlags)36),
                Attribute = _param1.attribute
            });
        }

        private void Attributes_AttributeChanged(object sender, AttributeEventArgs e)
        {
            OnAttributeChanged(e);
        }

        public virtual void InitialiseFromXML(XmlTextReader reader, SvgDocument document)
        {
            throw new NotImplementedException();
        }

        public void RenderElement(ISvgRenderer renderer)
        {
            Render(renderer);
        }

        public virtual bool ShouldWriteElement()
        {
            return ElementName != string.Empty;
        }

        protected virtual void WriteStartElement(XmlTextWriter writer)
        {
            if (ElementName != string.Empty)
            {
                writer.WriteStartElement(ElementName);
            }

            WriteAttributes(writer);
        }

        protected virtual void WriteEndElement(XmlTextWriter writer)
        {
            if (!(ElementName != string.Empty))
            {
                return;
            }
            writer.WriteEndElement();
        }

        protected virtual void WriteAttributes(XmlTextWriter writer)
        {
            Dictionary<string, string> source = WritePropertyAttributes(writer);
            if (AutoPublishEvents)
            {
                foreach (SvgElement.EventAttributeTuple svgEventAttribute in _svgEventAttributes)
                {
                    if (svgEventAttribute.Event.GetValue(this) != null && !string.IsNullOrEmpty(ID))
                    {
                        writer.WriteAttributeString(svgEventAttribute.Attribute.Name, ID + "/" + svgEventAttribute.Attribute.Name);
                    }
                }
            }
            var empty = string.Empty;
            foreach (KeyValuePair<string, string> customAttribute in (Dictionary<string, string>)_customAttributes)
            {
                if (customAttribute.Key.Equals("style") && source.Any<KeyValuePair<string, string>>())
                {
                    empty = customAttribute.Value;
                }
                else
                {
                    writer.WriteAttributeString(customAttribute.Key, customAttribute.Value);
                }
            }
            if (!source.Any<KeyValuePair<string, string>>())
            {
                return;
            }
            writer.WriteAttributeString("style", source.Select<KeyValuePair<string, string>, string>(s =>
            {
                KeyValuePair<string, string> keyValuePair = s;
                var key = keyValuePair.Key;
                keyValuePair = s;
                var str = keyValuePair.Value;
                return key + ":" + str + ";";
            }).Aggregate<string>((p, c) => p + c) + empty);
        }

        private Dictionary<string, string> WritePropertyAttributes(XmlTextWriter writer)
        {
            Dictionary<string, string> dictionary1 = _styles.ToDictionary<KeyValuePair<string, SortedDictionary<int, string>>, string, string>(_styles => _styles.Key, _styles => _styles.Value.Last<KeyValuePair<int, string>>().Value);
            List<SvgElement.PropertyAttributeTuple> propertyAttributeTupleList = new List<SvgElement.PropertyAttributeTuple>();
            Dictionary<string, float> dictionary2 = new Dictionary<string, float>();
            try
            {
                Writing = true;
                foreach (SvgElement.PropertyAttributeTuple propertyAttribute in _svgPropertyAttributes)
                {
                    if (propertyAttribute.Property.Converter.CanConvertTo(typeof(string)))
                    {
                        if (propertyAttribute.Attribute.Name == "fill-opacity" || propertyAttribute.Attribute.Name == "stroke-opacity")
                        {
                            propertyAttributeTupleList.Add(propertyAttribute);
                        }
                        else if (Attributes.ContainsKey(propertyAttribute.Attribute.Name))
                        {
                            var obj = propertyAttribute.Property.GetValue(this);
                            var flag1 = false;
                            var flag2 = propertyAttribute.Attribute.Name == "fill" || propertyAttribute.Attribute.Name == "stroke";
                            if (Parent != null)
                            {
                                if (!flag2 || obj != SvgPaintServer.NotSet)
                                {
                                    if (TryResolveParentAttributeValue(propertyAttribute.Attribute.Name, out var parentAttributeValue))
                                    {
                                        if (parentAttributeValue == obj || parentAttributeValue != null && parentAttributeValue.Equals(obj))
                                        {
                                            if (flag2)
                                            {
                                                continue;
                                            }
                                        }
                                        else
                                        {
                                            flag1 = true;
                                        }
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            if (flag2 && obj is SvgColourServer && ((SvgColourServer)obj).Colour.A < byte.MaxValue)
                            {
                                var num = ((SvgColourServer)obj).Colour.A / (float)byte.MaxValue;
                                dictionary2.Add(propertyAttribute.Attribute.Name + "-opacity", num);
                            }
                            var str = obj == null ? null : (string)propertyAttribute.Property.Converter.ConvertTo(obj, typeof(string));
                            if (obj != null)
                            {
                                if (flag1 || !string.IsNullOrEmpty(str))
                                {
                                    if (flag2)
                                    {
                                        dictionary1[propertyAttribute.Attribute.Name] = str;
                                    }
                                    else
                                    {
                                        writer.WriteAttributeString(propertyAttribute.Attribute.NamespaceAndName, str);
                                    }
                                }
                            }
                            else if (propertyAttribute.Attribute.Name == "fill")
                            {
                                if (flag2)
                                {
                                    dictionary1[propertyAttribute.Attribute.Name] = str;
                                }
                                else
                                {
                                    writer.WriteAttributeString(propertyAttribute.Attribute.NamespaceAndName, str);
                                }
                            }
                        }
                    }
                }
                foreach (SvgElement.PropertyAttributeTuple propertyAttributeTuple in propertyAttributeTupleList)
                {
                    var num1 = 1f;
                    var flag = false;
                    var name = propertyAttributeTuple.Attribute.Name;
                    if (dictionary2.ContainsKey(name))
                    {
                        num1 = dictionary2[name];
                        flag = true;
                    }
                    if (Attributes.ContainsKey(name))
                    {
                        num1 *= (float)propertyAttributeTuple.Property.GetValue(this);
                        flag = true;
                    }
                    if (flag)
                    {
                        var num2 = (float)Math.Round((double)num1, 2, (MidpointRounding)1);
                        var str = (string)propertyAttributeTuple.Property.Converter.ConvertTo(num2, typeof(string));
                        if (!string.IsNullOrEmpty(str))
                        {
                            writer.WriteAttributeString(propertyAttributeTuple.Attribute.NamespaceAndName, str);
                        }
                    }
                }
            }
            finally
            {
                Writing = false;
            }
            return dictionary1;
        }

        private bool TryResolveParentAttributeValue(
          string attributeKey,
          out object parentAttributeValue)
        {
            parentAttributeValue = null;
            SvgElement parent = Parent;
            var flag = false;
            for (; parent != null; parent = parent.Parent)
            {
                if (parent.Attributes.ContainsKey(attributeKey))
                {
                    flag = true;
                    parentAttributeValue = parent.Attributes[attributeKey];
                    if (parentAttributeValue != null)
                    {
                        break;
                    }
                }
            }
            return flag;
        }

        public virtual void Write(XmlTextWriter writer)
        {
            if (!ShouldWriteElement())
            {
                return;
            }

            WriteStartElement(writer);
            WriteChildren(writer);
            WriteEndElement(writer);
        }

        protected virtual void WriteChildren(XmlTextWriter writer)
        {
            if (Nodes.Any<ISvgNode>())
            {
                foreach (ISvgNode node in (IEnumerable<ISvgNode>)Nodes)
                {
                    if (!(node is SvgContentNode svgContentNode))
                    {
                        ((SvgElement)node).Write(writer);
                    }
                    else if (!string.IsNullOrEmpty(svgContentNode.Content))
                    {
                        writer.WriteString(svgContentNode.Content);
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Content))
                {
                    writer.WriteString(Content);
                }

                foreach (SvgElement child in Children)
                {
                    child.Write(writer);
                }
            }
        }

        protected virtual void Render(ISvgRenderer renderer)
        {
            try
            {
                PushTransforms(renderer);
                RenderChildren(renderer);
            }
            finally
            {
                PopTransforms(renderer);
            }
        }

        protected virtual void RenderChildren(ISvgRenderer renderer)
        {
            foreach (SvgElement child in Children)
            {
                child.Render(renderer);
            }
        }

        void ISvgElement.Render(ISvgRenderer renderer)
        {
            Render(renderer);
        }

        protected void AddPaths(SvgElement elem, GraphicsPath path)
        {
            foreach (SvgElement child in elem.Children)
            {
                if (!(child is SvgSymbol))
                {
                    if (child is SvgVisualElement && !(child is SvgGroup))
                    {
                        GraphicsPath graphicsPath1 = ((SvgVisualElement)child).Path(null);
                        if (graphicsPath1 != null)
                        {
                            GraphicsPath graphicsPath2;
                            using (graphicsPath2 = (GraphicsPath)graphicsPath1.Clone())
                            {
                                if (child.Transforms != null)
                                {
                                    using (Matrix matrix = child.Transforms.GetMatrix())
                                    {
                                        graphicsPath2.Transform(matrix);
                                    }
                                }
                                if (graphicsPath2.PointCount > 0)
                                {
                                    path.AddPath(graphicsPath2, false);
                                }
                            }
                        }
                    }
                    if (!(child is SvgPaintServer))
                    {
                        AddPaths(child, path);
                    }
                }
            }
        }

        protected GraphicsPath GetPaths(SvgElement elem, ISvgRenderer renderer)
        {
            GraphicsPath paths1 = new GraphicsPath();
            foreach (SvgElement child in elem.Children)
            {
                if (child is SvgVisualElement)
                {
                    if (child is SvgGroup)
                    {
                        GraphicsPath paths2 = GetPaths(child, renderer);
                        if (paths2.PointCount > 0)
                        {
                            if (child.Transforms != null)
                            {
                                using (Matrix matrix = child.Transforms.GetMatrix())
                                {
                                    paths2.Transform(matrix);
                                }
                            }
                            paths1.AddPath(paths2, false);
                        }
                    }
                    else
                    {
                        GraphicsPath graphicsPath1 = ((SvgVisualElement)child).Path(renderer);
                        GraphicsPath graphicsPath2 = graphicsPath1 != null ? (GraphicsPath)graphicsPath1.Clone() : new GraphicsPath();
                        if (child.Children.Count > 0)
                        {
                            GraphicsPath paths3 = GetPaths(child, renderer);
                            if (paths3.PointCount > 0)
                            {
                                graphicsPath2.AddPath(paths3, false);
                            }
                        }
                        if (graphicsPath2.PointCount > 0)
                        {
                            if (child.Transforms != null)
                            {
                                using (Matrix matrix = child.Transforms.GetMatrix())
                                {
                                    graphicsPath2.Transform(matrix);
                                }
                            }
                            paths1.AddPath(graphicsPath2, false);
                        }
                    }
                }
            }
            return paths1;
        }

        public virtual object Clone()
        {
            return MemberwiseClone();
        }

        public abstract SvgElement DeepCopy();

        ISvgNode ISvgNode.DeepCopy()
        {
            return DeepCopy();
        }

        public virtual SvgElement DeepCopy<T>() where T : SvgElement, new()
        {
            T obj = new T
            {
                ID = ID,
                Content = Content,
                ElementName = ElementName
            };
            if (Transforms != null)
            {
                obj.Transforms = Transforms.Clone() as SvgTransformCollection;
            }

            foreach (SvgElement child in Children)
            {
                obj.Children.Add(child.DeepCopy());
            }

            foreach (SvgElement.EventAttributeTuple svgEventAttribute in _svgEventAttributes)
            {
                if (svgEventAttribute.Event.GetValue(this) != null)
                {
                    if (svgEventAttribute.Event.Name == "MouseDown")
                    {
                        obj.MouseDown += (_param1, _param2) => { };
                    }
                    else if (svgEventAttribute.Event.Name == "MouseUp")
                    {
                        obj.MouseUp += (_param1, _param2) => { };
                    }
                    else if (svgEventAttribute.Event.Name == "MouseOver")
                    {
                        obj.MouseOver += (_param1, _param2) => { };
                    }
                    else if (svgEventAttribute.Event.Name == "MouseOut")
                    {
                        obj.MouseOut += (_param1, _param2) => { };
                    }
                    else if (svgEventAttribute.Event.Name == "MouseMove")
                    {
                        obj.MouseMove += (_param1, _param2) => { };
                    }
                    else if (svgEventAttribute.Event.Name == "MouseScroll")
                    {
                        obj.MouseScroll += (_param1, _param2) => { };
                    }
                    else if (svgEventAttribute.Event.Name == "Click")
                    {
                        obj.Click += (_param1, _param2) => { };
                    }
                    else if (svgEventAttribute.Event.Name == "Change")
                    {
                        ((object)obj as SvgText).Change += (_param1, _param2) => { };
                    }
                }
            }
            if (_customAttributes.Count > 0)
            {
                foreach (KeyValuePair<string, string> customAttribute in (Dictionary<string, string>)_customAttributes)
                {
                    obj.CustomAttributes.Add(customAttribute.Key, customAttribute.Value);
                }
            }
            if (_nodes.Count > 0)
            {
                foreach (ISvgNode node in _nodes)
                {
                    obj.Nodes.Add(node.DeepCopy());
                }
            }
            return obj;
        }

        public event EventHandler<AttributeEventArgs> AttributeChanged;

        protected void OnAttributeChanged(AttributeEventArgs args)
        {
            EventHandler<AttributeEventArgs> attributeChanged = AttributeChanged;
            if (attributeChanged == null)
            {
                return;
            }

            attributeChanged(this, args);
        }

        public event EventHandler<ContentEventArgs> ContentChanged;

        protected void OnContentChanged(ContentEventArgs args)
        {
            EventHandler<ContentEventArgs> contentChanged = ContentChanged;
            if (contentChanged == null)
            {
                return;
            }

            contentChanged(this, args);
        }

        [SvgAttribute("onclick")]
        public event EventHandler<MouseArg> Click;

        [SvgAttribute("onmousedown")]
        public event EventHandler<MouseArg> MouseDown;

        [SvgAttribute("onmouseup")]
        public event EventHandler<MouseArg> MouseUp;

        [SvgAttribute("onmousemove")]
        public event EventHandler<MouseArg> MouseMove;

        [SvgAttribute("onmousescroll")]
        public event EventHandler<MouseScrollArg> MouseScroll;

        [SvgAttribute("onmouseover")]
        public event EventHandler<MouseArg> MouseOver;

        [SvgAttribute("onmouseout")]
        public event EventHandler<MouseArg> MouseOut;

        protected void RaiseMouseClick(object sender, MouseArg e)
        {
            EventHandler<MouseArg> click = Click;
            if (click == null)
            {
                return;
            }

            click(sender, e);
        }

        protected void RaiseMouseDown(object sender, MouseArg e)
        {
            EventHandler<MouseArg> mouseDown = MouseDown;
            if (mouseDown == null)
            {
                return;
            }

            mouseDown(sender, e);
        }

        protected void RaiseMouseUp(object sender, MouseArg e)
        {
            EventHandler<MouseArg> mouseUp = MouseUp;
            if (mouseUp == null)
            {
                return;
            }

            mouseUp(sender, e);
        }

        protected void RaiseMouseMove(object sender, MouseArg e)
        {
            EventHandler<MouseArg> mouseMove = MouseMove;
            if (mouseMove == null)
            {
                return;
            }

            mouseMove(sender, e);
        }

        protected void RaiseMouseOver(object sender, MouseArg args)
        {
            EventHandler<MouseArg> mouseOver = MouseOver;
            if (mouseOver == null)
            {
                return;
            }

            mouseOver(sender, args);
        }

        protected void RaiseMouseOut(object sender, MouseArg args)
        {
            EventHandler<MouseArg> mouseOut = MouseOut;
            if (mouseOut == null)
            {
                return;
            }

            mouseOut(sender, args);
        }

        protected void OnMouseScroll(
          int scroll,
          bool ctrlKey,
          bool shiftKey,
          bool altKey,
          string sessionID)
        {
            MouseScrollArg e = new MouseScrollArg
            {
                Scroll = scroll,
                AltKey = altKey,
                ShiftKey = shiftKey,
                CtrlKey = ctrlKey,
                SessionID = sessionID
            };
            RaiseMouseScroll(this, e);
        }

        protected void RaiseMouseScroll(object sender, MouseScrollArg e)
        {
            EventHandler<MouseScrollArg> mouseScroll = MouseScroll;
            if (mouseScroll == null)
            {
                return;
            }

            mouseScroll(sender, e);
        }

        protected virtual bool IsPathDirty
        {
            get => _dirty;
            set => _dirty = value;
        }

        public void InvalidateChildPaths()
        {
            IsPathDirty = true;
            foreach (SvgElement child in Children)
            {
                child.InvalidateChildPaths();
            }
        }

        protected static float FixOpacityValue(float value)
        {
            return Math.Min(Math.Max(value, 0.0f), 1f);
        }

        [SvgAttribute("fill")]
        public virtual SvgPaintServer Fill
        {
            get => GetAttribute<SvgPaintServer>("fill", true, SvgPaintServer.NotSet);
            set => Attributes["fill"] = value;
        }

        [SvgAttribute("stroke")]
        public virtual SvgPaintServer Stroke
        {
            get => GetAttribute<SvgPaintServer>("stroke", true);
            set => Attributes["stroke"] = value;
        }

        [SvgAttribute("fill-rule")]
        public virtual SvgFillRule FillRule
        {
            get => GetAttribute<SvgFillRule>("fill-rule", true);
            set => Attributes["fill-rule"] = value;
        }

        [SvgAttribute("fill-opacity")]
        public virtual float FillOpacity
        {
            get => GetAttribute<float>("fill-opacity", true, 1f);
            set => Attributes["fill-opacity"] = SvgElement.FixOpacityValue(value);
        }

        [SvgAttribute("stroke-width")]
        public virtual SvgUnit StrokeWidth
        {
            get => GetAttribute<SvgUnit>("stroke-width", true, (SvgUnit)1f);
            set => Attributes["stroke-width"] = value;
        }

        [SvgAttribute("stroke-linecap")]
        public virtual SvgStrokeLineCap StrokeLineCap
        {
            get => GetAttribute<SvgStrokeLineCap>("stroke-linecap", true, SvgStrokeLineCap.Butt);
            set => Attributes["stroke-linecap"] = value;
        }

        [SvgAttribute("stroke-linejoin")]
        public virtual SvgStrokeLineJoin StrokeLineJoin
        {
            get => GetAttribute<SvgStrokeLineJoin>("stroke-linejoin", true, SvgStrokeLineJoin.Miter);
            set => Attributes["stroke-linejoin"] = value;
        }

        [SvgAttribute("stroke-miterlimit")]
        public virtual float StrokeMiterLimit
        {
            get => GetAttribute<float>("stroke-miterlimit", true, 4f);
            set => Attributes["stroke-miterlimit"] = value;
        }

        [TypeConverter(typeof(SvgStrokeDashArrayConverter))]
        [SvgAttribute("stroke-dasharray")]
        public virtual SvgUnitCollection StrokeDashArray
        {
            get => GetAttribute<SvgUnitCollection>("stroke-dasharray", true);
            set => Attributes["stroke-dasharray"] = value;
        }

        [SvgAttribute("stroke-dashoffset")]
        public virtual SvgUnit StrokeDashOffset
        {
            get => GetAttribute<SvgUnit>("stroke-dashoffset", true, SvgUnit.Empty);
            set => Attributes["stroke-dashoffset"] = value;
        }

        [SvgAttribute("stroke-opacity")]
        public virtual float StrokeOpacity
        {
            get => GetAttribute<float>("stroke-opacity", true, 1f);
            set => Attributes["stroke-opacity"] = SvgElement.FixOpacityValue(value);
        }

        [SvgAttribute("opacity")]
        public virtual float Opacity
        {
            get => GetAttribute<float>("opacity", true, 1f);
            set => Attributes["opacity"] = SvgElement.FixOpacityValue(value);
        }

        [SvgAttribute("shape-rendering")]
        public virtual SvgShapeRendering ShapeRendering
        {
            get => GetAttribute<SvgShapeRendering>("shape-rendering", true);
            set => Attributes["shape-rendering"] = value;
        }

        [SvgAttribute("text-anchor")]
        public virtual SvgTextAnchor TextAnchor
        {
            get => GetAttribute<SvgTextAnchor>("text-anchor", true);
            set
            {
                Attributes["text-anchor"] = value;
                IsPathDirty = true;
            }
        }

        [SvgAttribute("baseline-shift")]
        public virtual string BaselineShift
        {
            get => GetAttribute<string>("baseline-shift", true);
            set
            {
                Attributes["baseline-shift"] = value;
                IsPathDirty = true;
            }
        }

        [SvgAttribute("font-family")]
        public virtual string FontFamily
        {
            get => GetAttribute<string>("font-family", true);
            set
            {
                Attributes["font-family"] = value;
                IsPathDirty = true;
            }
        }

        [SvgAttribute("font-size")]
        public virtual SvgUnit FontSize
        {
            get => GetAttribute<SvgUnit>("font-size", true, SvgUnit.Empty);
            set
            {
                Attributes["font-size"] = value;
                IsPathDirty = true;
            }
        }

        [SvgAttribute("font-style")]
        public virtual SvgFontStyle FontStyle
        {
            get => GetAttribute<SvgFontStyle>("font-style", true, SvgFontStyle.All);
            set
            {
                Attributes["font-style"] = value;
                IsPathDirty = true;
            }
        }

        [SvgAttribute("font-variant")]
        public virtual SvgFontVariant FontVariant
        {
            get => GetAttribute<SvgFontVariant>("font-variant", true, SvgFontVariant.Inherit);
            set
            {
                Attributes["font-variant"] = value;
                IsPathDirty = true;
            }
        }

        [SvgAttribute("text-decoration")]
        public virtual SvgTextDecoration TextDecoration
        {
            get => GetAttribute<SvgTextDecoration>("text-decoration", true);
            set
            {
                Attributes["text-decoration"] = value;
                IsPathDirty = true;
            }
        }

        [SvgAttribute("font-weight")]
        public virtual SvgFontWeight FontWeight
        {
            get => GetAttribute<SvgFontWeight>("font-weight", true);
            set
            {
                Attributes["font-weight"] = value;
                IsPathDirty = true;
            }
        }

        [SvgAttribute("text-transform")]
        public virtual SvgTextTransformation TextTransformation
        {
            get => GetAttribute<SvgTextTransformation>("text-transform", true);
            set
            {
                Attributes["text-transform"] = value;
                IsPathDirty = true;
            }
        }

        [SvgAttribute("font")]
        public virtual string Font
        {
            get => GetAttribute<string>("font", true, string.Empty);
            set
            {
                SvgElement.FontParseState fontParseState = SvgElement.FontParseState.fontStyle;
                var strArray1 = value.Split(' ', StringSplitOptions.None);
                for (var startIndex = 0; startIndex < strArray1.Length; ++startIndex)
                {
                    var str = strArray1[startIndex];
                    var flag = false;
                    while (!flag)
                    {
                        switch (fontParseState)
                        {
                            case SvgElement.FontParseState.fontStyle:
                                SvgFontStyle result1;
                                flag = Enums.TryParse<SvgFontStyle>(str, out result1);
                                if (flag)
                                {
                                    FontStyle = result1;
                                }

                                ++fontParseState;
                                continue;
                            case SvgElement.FontParseState.fontVariant:
                                SvgFontVariant result2;
                                flag = Enums.TryParse<SvgFontVariant>(str, out result2);
                                if (flag)
                                {
                                    FontVariant = result2;
                                }

                                ++fontParseState;
                                continue;
                            case SvgElement.FontParseState.fontWeight:
                                SvgFontWeight result3;
                                flag = Enums.TryParse<SvgFontWeight>(str, out result3);
                                if (flag)
                                {
                                    FontWeight = result3;
                                }

                                ++fontParseState;
                                continue;
                            case SvgElement.FontParseState.fontSize:
                                var strArray2 = str.Split('/', StringSplitOptions.None);
                                try
                                {
                                    SvgUnit svgUnit = (SvgUnit)new SvgUnitConverter().ConvertFromInvariantString(strArray2[0]);
                                    flag = true;
                                    FontSize = svgUnit;
                                }
                                catch
                                {
                                }
                                ++fontParseState;
                                continue;
                            case SvgElement.FontParseState.fontFamilyNext:
                                ++fontParseState;
                                flag = true;
                                continue;
                            default:
                                continue;
                        }
                    }
                    switch (fontParseState)
                    {
                        case SvgElement.FontParseState.fontFamilyNext:
                            FontFamily = string.Join(" ", strArray1, startIndex + 1, strArray1.Length - (startIndex + 1));
                            startIndex = 2147483645;
                            break;
                        case SvgElement.FontParseState.fontFamilyCurr:
                            FontFamily = string.Join(" ", strArray1, startIndex, strArray1.Length - startIndex);
                            startIndex = 2147483645;
                            break;
                    }
                }
                Attributes["font"] = value;
                IsPathDirty = true;
            }
        }

        internal IFontDefn GetFont(ISvgRenderer renderer)
        {
            SvgUnit fontSize = FontSize;
            var size = fontSize == SvgUnit.None || fontSize == SvgUnit.Empty ? (float)new SvgUnit(SvgUnitType.Em, 1f) : fontSize.ToDeviceValue(renderer, UnitRenderingType.Vertical, this);
            var obj = SvgElement.ValidateFontFamily(FontFamily, OwnerDocument);
            if (!(obj is IEnumerable<SvgFontFace> source))
            {
                System.Drawing.FontStyle fontStyle = 0;
                switch (FontWeight)
                {
                    case SvgFontWeight.W600:
                    case SvgFontWeight.Bold:
                    case SvgFontWeight.W800:
                    case SvgFontWeight.W900:
                    case SvgFontWeight.Bolder:
                        fontStyle = fontStyle | (System.Drawing.FontStyle)1;
                        break;
                }
                switch (FontStyle)
                {
                    case SvgFontStyle.Oblique:
                    case SvgFontStyle.Italic:
                        fontStyle = fontStyle | (System.Drawing.FontStyle)2;
                        break;
                }
                switch (TextDecoration)
                {
                    case SvgTextDecoration.Underline:
                        fontStyle = fontStyle | (System.Drawing.FontStyle)4;
                        break;
                    case SvgTextDecoration.LineThrough:
                        fontStyle = fontStyle | (System.Drawing.FontStyle)8;
                        break;
                }
                System.Drawing.FontFamily fontFamily = obj as System.Drawing.FontFamily;
                fontFamily.IsStyleAvailable(fontStyle);
                return new GdiFontDefn(new System.Drawing.Font(fontFamily, size, fontStyle, (GraphicsUnit)2));
            }
            if (!(source.First<SvgFontFace>().Parent is SvgFont font))
            {
                font = OwnerDocument.IdManager.GetElementById(source.First<SvgFontFace>().Descendants().OfType<SvgFontFaceUri>().First<SvgFontFaceUri>().ReferencedElement) as SvgFont;
            }

            return new SvgFontDefn(font, size, OwnerDocument.Ppi);
        }

        public static object ValidateFontFamily(string fontFamilyList, SvgDocument doc)
        {
            foreach (var str in (fontFamilyList ?? string.Empty).Split(',').Select<string, string>(fontName => fontName.Trim('"', ' ', '\'')))
            {
                var f = str;
                if (doc != null && doc.FontDefns().TryGetValue(f, out IEnumerable<SvgFontFace> svgFontFaces))
                {
                    return svgFontFaces;
                }

                System.Drawing.FontFamily fontFamily1;
                if (doc == null)
                {
                    fontFamily1 = null;
                }
                else
                {
                    System.Drawing.FontFamily[] source = doc.PrivateFontDefns();
                    fontFamily1 = source != null ? source.FirstOrDefault<System.Drawing.FontFamily>(ff => string.Equals(ff.Name, f, StringComparison.OrdinalIgnoreCase)) : null;
                }
                System.Drawing.FontFamily fontFamily2 = fontFamily1;
                if (fontFamily2 != null)
                {
                    return fontFamily2;
                }

                System.Drawing.FontFamily font = SvgFontManager.FindFont(f);
                if (font != null)
                {
                    return font;
                }

                System.Drawing.FontFamily fontFamily3 = PrivateFonts.Families.FirstOrDefault<System.Drawing.FontFamily>(ff => string.Equals(ff.Name, f, StringComparison.OrdinalIgnoreCase));
                if (fontFamily3 != null)
                {
                    return fontFamily3;
                }

                switch (f.ToLower())
                {
                    case "serif":
                        return System.Drawing.FontFamily.GenericSerif;
                    case "sans-serif":
                        return System.Drawing.FontFamily.GenericSansSerif;
                    case "monospace":
                        return System.Drawing.FontFamily.GenericMonospace;
                    default:
                        continue;
                }
            }
            return System.Drawing.FontFamily.GenericSansSerif;
        }

        protected class PropertyAttributeTuple
        {
            public PropertyDescriptor Property;
            public SvgAttributeAttribute Attribute;
        }

        protected class EventAttributeTuple
        {
            public FieldInfo Event;
            public SvgAttributeAttribute Attribute;
        }

        private enum FontParseState
        {
            fontStyle,
            fontVariant,
            fontWeight,
            fontSize,
            fontFamilyNext,
            fontFamilyCurr,
        }
    }
}
