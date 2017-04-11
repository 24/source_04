// HttpContent :
//   ByteArrayContent : HttpContent
//   StringContent : ByteArrayContent
//   FormUrlEncodedContent : ByteArrayContent
//   MultipartContent : HttpContent, IEnumerable<HttpContent>
//   MultipartFormDataContent : MultipartContent
//   StreamContent : HttpContent

using pb.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace pb.Web.Http
{
    public abstract class HttpContentValue : IDisposable
    {
        public abstract System.Net.Http.HttpContent GetHttpContent();

        public virtual void Dispose()
        {
        }
    }

    public class UrlEncodedContent : HttpContentValue
    {
        public IEnumerable<KeyValuePair<string, string>> Values;

        public UrlEncodedContent(IEnumerable<KeyValuePair<string, string>> values)
        {
            Values = values;
        }

        public UrlEncodedContent(params string[] values)
        {
            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
            string key = null;
            foreach (string value in values)
            {
                if (key == null)
                    key = value;
                else
                {
                    list.Add(new KeyValuePair<string, string>(key, value));
                    key = null;
                }
            }
            if (key != null)
                list.Add(new KeyValuePair<string, string>(key, null));
            Values = list;
        }

        public override HttpContent GetHttpContent()
        {
            return new FormUrlEncodedContent(Values);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            bool first = true;
            foreach (KeyValuePair<string, string> value in Values)
            {
                if (!first)
                    sb.Append(",");
                sb.Append($" \"{value.Key}\": \"{value.Value}\"");
                first = false;
            }
            sb.Append("}");
            return sb.ToString();
        }
    }

    public abstract class MultipartContent : HttpContentValue
    {
        public string Name = null;
        public string Filename = null;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            if (Name != null)
            {
                sb.Append($"Name: \"{Name}\"");
                first = false;
            }
            if (Filename != null)
            {
                if (!first)
                    sb.Append(", ");
                sb.Append($"Filename: \"{Filename}\"");
            }
            return sb.ToString();
        }
    }

    public class TextContent : MultipartContent // HttpContentValue
    {
        public string Content = null;
        public Encoding Encoding = null;
        public string MediaType = "text/plain";

        public TextContent(string content, Encoding encoding = null, string mediaType = "text/plain")
        {
            Content = content;
            Encoding = encoding;
            MediaType = mediaType;
        }

        public override HttpContent GetHttpContent()
        {
            return new StringContent(Content, Encoding ?? Encoding.UTF8, MediaType);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{ ");
            string s = base.ToString();
            if (s != "")
            {
                sb.Append(s);
                sb.Append(", ");
            }
            sb.Append($"MediaType: \"{MediaType}\", Charset: \"{(Encoding ?? Encoding.UTF8).EncodingName}\", Content: \"{Content}\" }}");
            return sb.ToString();
        }
    }

    public class FileContent : MultipartContent
    {
        private FileStream _fs = null;

        public string File;

        public FileContent(string file)
        {
            File = file;
            Filename = zPath.GetFileName(file);
        }

        public override void Dispose()
        {
            if (_fs != null)
            {
                _fs.Close();
                _fs = null;
            }
        }

        public override HttpContent GetHttpContent()
        {
            _fs = zFile.Open(File, FileMode.Open, FileAccess.Read, FileShare.Read);
            return new StreamContent(_fs);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{ ");
            string s = base.ToString();
            if (s != "")
            {
                sb.Append(s);
                sb.Append(", ");
            }
            sb.Append($"File: \"{File}\" }}");
            return sb.ToString();
        }
    }

    public class MultipartContents : HttpContentValue
    {
        public string Boundary;
        public IEnumerable<MultipartContent> Contents;

        public MultipartContents(string boundary, params MultipartContent[] contents)
        {
            Boundary = boundary;
            Contents = contents;
        }

        public override void Dispose()
        {
            foreach (MultipartContent content in Contents)
                content.Dispose();
        }

        public override HttpContent GetHttpContent()
        {
            MultipartFormDataContent multiContent;
            if (Boundary != null)
                multiContent = new MultipartFormDataContent(Boundary);
            else
                multiContent = new MultipartFormDataContent();
            foreach (MultipartContent content in Contents)
            {
                HttpContent httpContent = content.GetHttpContent();
                if (content.Name != null)
                {
                    if (content.Filename != null)
                        multiContent.Add(httpContent, content.Name, content.Filename);
                    else
                        multiContent.Add(httpContent, content.Name);
                }
                else
                    multiContent.Add(httpContent);
            }
            return multiContent;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            bool first = true;
            foreach (MultipartContent content in Contents)
            {
                if (!first)
                    sb.Append(",");
                sb.Append(" ");
                sb.Append(content.ToString());
                first = false;
            }
            if (!first)
                sb.Append(" ");
            sb.Append("]");
            return sb.ToString();
        }
    }
}
