using System;
using System.Drawing;
using pb.Web.old;

namespace pb.Web
{
    public abstract class WebImageCacheManager_v1
    {
        public WebImageCacheManager_v1()
        {
        }

        public virtual ImageCache_v1 GetImageCache(string url, HttpRequestParameters_v1 requestParameters = null)
        {
            return new ImageCache_v1(this, url, requestParameters);
        }

        public abstract Image LoadImage(string url, HttpRequestParameters_v1 requestParameters = null);
    }

    public class ImageCache_v1
    {
        protected WebImageCacheManager_v1 _imageUrlCacheManager = null;
        protected string _url;
        protected HttpRequestParameters_v1 _requestParameters = null;
        protected Image _image = null;

        public ImageCache_v1(WebImageCacheManager_v1 imageUrlCacheManager, string url, HttpRequestParameters_v1 requestParameters = null)
        {
            _imageUrlCacheManager = imageUrlCacheManager;
            _url = url;
            _requestParameters = requestParameters;
        }

        public virtual Image Image
        {
            get
            {
                if (_image == null)
                    _image = _imageUrlCacheManager.LoadImage(_url, _requestParameters);
                return _image;
            }
        }
        public virtual int Width { get { return Image.Width; } }
        public virtual int Height { get { return Image.Height; } }
    }
}
