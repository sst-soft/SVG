// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.Net;
using System.Text.RegularExpressions;

namespace Svg
{
    public class SvgElementIdManager
    {
        private readonly SvgDocument _document;
        private readonly Dictionary<string, SvgElement> _idValueMap;
        private static readonly Regex regex = new Regex("#\\d+$");

        public virtual SvgElement GetElementById(string id)
        {
            id = Utility.GetUrlString(id);
            if (id.StartsWith("#"))
            {
                id = id.Substring(1);
            }

            _idValueMap.TryGetValue(id, out SvgElement elementById);
            return elementById;
        }

        public virtual SvgElement GetElementById(Uri uri)
        {
            var urlString = Utility.GetUrlString(uri.ToString());
            if (!urlString.StartsWith("#"))
            {
                var startIndex = urlString.LastIndexOf('#');
                var id = urlString.Substring(startIndex);
                uri = new Uri(urlString.Remove(startIndex, id.Length), UriKind.RelativeOrAbsolute);
                if (!uri.IsAbsoluteUri && _document.BaseUri != null)
                {
                    uri = new Uri(_document.BaseUri, uri);
                }

                if (uri.IsAbsoluteUri)
                {
                    if (uri.IsFile)
                    {
                        return SvgDocument.Open<SvgDocument>(uri.LocalPath).IdManager.GetElementById(id);
                    }

                    if (!(uri.Scheme == Uri.UriSchemeHttp) && !(uri.Scheme == Uri.UriSchemeHttps))
                    {
                        throw new NotSupportedException();
                    }

                    using (WebResponse response = WebRequest.Create(uri).GetResponse())
                    {
                        return SvgDocument.Open<SvgDocument>(response.GetResponseStream()).IdManager.GetElementById(id);
                    }
                }
            }
            return GetElementById(urlString);
        }

        public virtual void Add(SvgElement element)
        {
            AddAndForceUniqueID(element, null, false);
        }

        public virtual bool AddAndForceUniqueID(
      SvgElement element,
      SvgElement sibling,
      bool autoForceUniqueID = true,
      Action<SvgElement, string, string> logElementOldIDNewID = null)
        {
            var flag = false;
            if (!string.IsNullOrEmpty(element.ID))
            {
                var newID = EnsureValidId(element.ID, autoForceUniqueID);
                if (autoForceUniqueID && newID != element.ID)
                {
                    if (logElementOldIDNewID != null)
                    {
                        logElementOldIDNewID(element, element.ID, newID);
                    }

                    element.ForceUniqueID(newID);
                    flag = true;
                }
                _idValueMap.Add(element.ID, element);
            }
            OnAdded(element);
            return flag;
        }

        public virtual void Remove(SvgElement element)
        {
            if (!string.IsNullOrEmpty(element.ID))
            {
                _idValueMap.Remove(element.ID);
            }

            OnRemoved(element);
        }

        public string EnsureValidId(string id, bool autoForceUniqueID = false)
        {
            if (string.IsNullOrEmpty(id) || !_idValueMap.ContainsKey(id))
            {
                return id;
            }

            if (!autoForceUniqueID)
            {
                throw new SvgIDExistsException("An element with the same ID already exists: '" + id + "'.");
            }

            Match match = SvgElementIdManager.regex.Match(id);
            id = !match.Success || !int.TryParse(match.Value.Substring(1), out var result) ? id + "#1" : SvgElementIdManager.regex.Replace(id, "#" + (result + 1).ToString());
            return EnsureValidId(id, true);
        }

        public SvgElementIdManager(SvgDocument document)
        {
            _document = document;
            _idValueMap = new Dictionary<string, SvgElement>();
        }

        public event EventHandler<SvgElementEventArgs> ElementAdded;

        public event EventHandler<SvgElementEventArgs> ElementRemoved;

        protected void OnAdded(SvgElement element)
        {
            EventHandler<SvgElementEventArgs> elementAdded = ElementAdded;
            if (elementAdded == null)
            {
                return;
            }

            elementAdded(_document, new SvgElementEventArgs()
            {
                Element = element
            });
        }

        protected void OnRemoved(SvgElement element)
        {
            EventHandler<SvgElementEventArgs> elementRemoved = ElementRemoved;
            if (elementRemoved == null)
            {
                return;
            }

            elementRemoved(_document, new SvgElementEventArgs()
            {
                Element = element
            });
        }
    }
}
