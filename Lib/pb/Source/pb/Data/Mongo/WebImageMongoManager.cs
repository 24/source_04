using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Linq;

namespace pb.Web.Data.Mongo
{
    public class WebImageMongoManager
    {
        private static WebImageMongoManager __current = null;
        private WebImageMongoCacheManager _imageCacheManager = null;
        ////////private Predicate<ImageMongoCache> _imageFilter = null;

        public static WebImageMongoManager Current { get { return __current; } }

        //public void InitImage(XElement xe)
        //{
        //    if (xe.zXPathValue("UseUrlCache").zTryParseAs(false))
        //    {
        //        _imageCacheManager = new WebImageMongoCacheManager(xe.zXPathValue("MongoServer"), xe.zXPathValue("MongoDatabase"), xe.zXPathValue("MongoCollection"), xe.zXPathValue("CacheDirectory"));
        //    }
        //    //_imageFilterMinHeight = xe.zXPathValue("ImageFilterMinHeight").zParseAs<int>();
        //}

        public WebImageMongoManager(WebImageMongoCacheManager imageCacheManager = null, Predicate<ImageMongoCache> imageFilter = null)
        {
            _imageCacheManager = imageCacheManager;
            //_imageFilter = imageFilter;
        }

        //public IEnumerable<WebImage> LoadImages(IEnumerable<WebImage> images, ImageRequest imageRequest, HttpRequestParameters requestParameters = null)
        //{
        //    foreach (WebImage image in images)
        //    {
        //        if (LoadImage(image, imageRequest, requestParameters))
        //            yield return image;
        //    }
        //}

        public void LoadImages(IEnumerable<WebImage> images, WebImageRequest imageRequest, HttpRequestParameters requestParameters = null)
        {
            foreach (WebImage image in images)
            {
                LoadImage(image, imageRequest, requestParameters);
            }
        }

        //Predicate<ImageMongoCache> filter = null
        public bool LoadImage(WebImage webImage, WebImageRequest imageRequest, HttpRequestParameters requestParameters = null)
        {
            //Trace.WriteLine($"WebImageMongo.LoadImage() : \"{webImage.Url}\"    _imageCacheManager {_imageCacheManager}");
            if (webImage.Url != null && webImage.Image == null)
            {
                try
                {
                    Image image = null;
                    if (_imageCacheManager != null)
                    {
                        ImageMongoCache imageCache = (ImageMongoCache)_imageCacheManager.GetImageCache(webImage.Url, requestParameters);
                        //if (_imageFilter != null && !_imageFilter(imageCache))
                        //    return false;
                        image = imageCache.Image;
                    }
                    else if (imageRequest.LoadImageFromWeb)
                        image = HttpManager.CurrentHttpManager.LoadImage(new HttpRequest { Url = webImage.Url }, requestParameters);
                    if (imageRequest.LoadImageToData)
                        webImage.Image = image;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("error loading image \"{0}\"", webImage.Url);
                    Trace.WriteLine(ex.Message);
                    //return false;
                }
            }
            return true;
        }

        public static void Create(XElement xe)
        {
            __current = new WebImageMongoManager(WebImageMongoCacheManager.Create(xe));
        }
    }
}
