using System.Drawing;

namespace pb.Web
{
    public abstract class WebImageCacheManager
    {
        public virtual ImageCache GetImageCache(string url, HttpRequestParameters requestParameters = null)
        {
            return new ImageCache(this, url, requestParameters);
        }

        public abstract Image LoadImage(string url, HttpRequestParameters requestParameters = null);
    }

    public class ImageCache
    {
        protected WebImageCacheManager _webImageCacheManager = null;
        protected string _url;
        protected HttpRequestParameters _requestParameters = null;
        protected Image _image = null;

        public ImageCache(WebImageCacheManager webImageCacheManager, string url, HttpRequestParameters requestParameters = null)
        {
            _webImageCacheManager = webImageCacheManager;
            _url = url;
            _requestParameters = requestParameters;
        }

        public virtual Image Image
        {
            get
            {
                if (_image == null)
                    _image = _webImageCacheManager.LoadImage(_url, _requestParameters);
                return _image;
            }
        }
        public virtual int Width { get { return Image.Width; } }
        public virtual int Height { get { return Image.Height; } }
    }
}
