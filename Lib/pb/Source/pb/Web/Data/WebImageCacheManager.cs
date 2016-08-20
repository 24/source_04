using System.Drawing;

namespace pb.Web
{
    public abstract class WebImageCacheManager
    {
        public virtual ImageCache GetImageCache(string url, HttpRequestParameters requestParameters = null, bool refreshImage = false)
        {
            return new ImageCache(this, url, requestParameters, refreshImage);
        }

        public abstract Image LoadImage(string url, HttpRequestParameters requestParameters = null, bool refreshImage = false);
    }

    public class ImageCache
    {
        protected WebImageCacheManager _webImageCacheManager = null;
        protected string _url;
        protected HttpRequestParameters _requestParameters = null;
        protected bool _refreshImage = false;
        protected Image _image = null;

        public ImageCache(WebImageCacheManager webImageCacheManager, string url, HttpRequestParameters requestParameters = null, bool refreshImage = false)
        {
            _webImageCacheManager = webImageCacheManager;
            _url = url;
            _requestParameters = requestParameters;
            _refreshImage = refreshImage;
        }

        public virtual Image Image
        {
            get
            {
                if (_image == null)
                    _image = _webImageCacheManager.LoadImage(_url, _requestParameters, _refreshImage);
                return _image;
            }
        }
        public virtual int Width { get { return Image.Width; } }
        public virtual int Height { get { return Image.Height; } }
    }
}
