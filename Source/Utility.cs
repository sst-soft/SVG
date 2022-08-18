// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.Net;
using System.Text;

namespace Svg
{
    internal static class Utility
    {
        public static string GetUrlString(string url)
        {
            url = url.Trim();
            if (url.StartsWith("url(", StringComparison.OrdinalIgnoreCase) && url.EndsWith(")"))
            {
                url = new StringBuilder(url).Remove(url.Length - 1, 1).Remove(0, 4).ToString().Trim();
                if (url.StartsWith("\"") && url.EndsWith("\"") || url.StartsWith("'") && url.EndsWith("'"))
                {
                    url = new StringBuilder(url).Remove(url.Length - 1, 1).Remove(0, 1).ToString().Trim();
                }
            }
            return url;
        }

        public static FileData GetBytesFromUri(Uri uri)
        {
            if (uri.IsAbsoluteUri && uri.Scheme == "data")
            {
                return Utility.GetBytesFromDataUri(uri.ToString());
            }

            using (WebResponse response = WebRequest.Create(uri).GetResponse())
            {
                Stream responseStream = response.GetResponseStream();
                if (responseStream.CanSeek)
                {
                    responseStream.Position = 0L;
                }

                var numArray = new byte[102400];
                MemoryStream memoryStream = new MemoryStream();
                int num;
                do
                {
                    num = responseStream.Read(numArray, 0, numArray.Length);
                    memoryStream.Write(numArray, 0, num);
                }
                while (num > 0);
                return new FileData(memoryStream.GetBuffer(), response.ContentType, null);
            }
        }

        public static FileData GetBytesFromDataUri(string uriString)
        {
            var startIndex = 5;
            var num = uriString.IndexOf(",", startIndex);
            if (num < 0 || num + 1 >= uriString.Length)
            {
                throw new Exception("Invalid data URI");
            }

            var mimeType = "text/plain";
            Encoding charset = Encoding.ASCII;
            var flag = false;
            List<string> stringList = new List<string>(uriString.Substring(startIndex, num - startIndex).Split(';', StringSplitOptions.None));
            if (stringList[0].Contains("/"))
            {
                mimeType = stringList[0].Trim();
                stringList.RemoveAt(0);
            }
            if (stringList.Count > 0 && stringList[stringList.Count - 1].Trim().Equals("base64", StringComparison.InvariantCultureIgnoreCase))
            {
                flag = true;
                stringList.RemoveAt(stringList.Count - 1);
            }
            foreach (var str in stringList)
            {
                var strArray = str.Split('=', StringSplitOptions.None);
                if (strArray.Length >= 2 && strArray[0].Trim().Equals("charset", StringComparison.InvariantCultureIgnoreCase))
                {
                    charset = Encoding.GetEncoding(strArray[1].Trim());
                }
            }
            var s = uriString.Substring(num + 1);
            return new FileData(flag ? Convert.FromBase64String(s) : (charset ?? Encoding.UTF8).GetBytes(s), mimeType, charset);
        }
    }
}
