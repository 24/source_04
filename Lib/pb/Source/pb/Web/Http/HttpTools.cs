namespace pb.Web.Http
{
    public static class HttpTools
    {
        public static string GetFileExtensionFromContentType(string contentType)
        {
            contentType = contentType.ToLower();
            // text/html
            if (contentType.EndsWith("/html"))
                return ".html";
            // text/xml application/xml
            else if (contentType.EndsWith("/xml"))
                return ".xml";
            else
                return ".txt";
        }

        public static string GetContentTypeFromFileExtension(string ext)
        {
            //string contentType
            switch (ext.ToLower())
            {
                case ".xml":
                    return "text/xml";
                case ".htm":
                case ".html":
                case ".asp":
                case ".php":
                    return "text/html";
                case ".txt":
                    return "text/txt";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".gif":
                    return "image/gif";
                case ".png":
                    return "image/png";
                case ".tiff":
                    return "image/tiff";
                case ".bmp":
                    return "image/bmp";
                default:
                    return null;
                    //default:
                    //    if (ext.Length > 1)
                    //        _resultContentType = "/" + ext.Substring(1);
                    //    break;
            }
        }

        public static HttpRequestMethod GetHttpRequestMethod(string method)
        {
            switch (method.ToLower())
            {
                case "get":
                    return HttpRequestMethod.Get;
                case "post":
                    return HttpRequestMethod.Post;
                default:
                    throw new PBException($"unknow HttpRequestMethod \"{method}\"");
            }
        }
    }
}
