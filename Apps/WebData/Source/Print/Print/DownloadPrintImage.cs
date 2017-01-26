using pb;
using pb.Data.Xml;
using pb.Web;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Linq;

namespace Download.Print
{
    public static class DownloadPrintImage
    {
        private static WebImageMongoCacheManager _imageCacheManager = null;
        private static int _imageFilterMinHeight = 0;   // 70
        private static Predicate<ImageMongoCache> _imageFilter = imageCache => (_imageFilterMinHeight == 0 || imageCache.Height > _imageFilterMinHeight) && imageCache.MongoImage.Category != "layout";

        static DownloadPrintImage()
        {
            InitImage(XmlConfig.CurrentConfig.GetElement("Image"));
        }

        private static void InitImage(XElement xe)
        {
            UrlCache urlCache = UrlCache.Create(xe);
            //if (xe.zXPathValue("UseUrlCache").zTryParseAs(false))
            if (urlCache != null)
            {
                // xe.zXPathValue("CacheDirectory")
                _imageCacheManager = new WebImageMongoCacheManager(xe.zXPathValue("MongoServer"), xe.zXPathValue("MongoDatabase"), xe.zXPathValue("MongoCollection"), urlCache);
            }
            _imageFilterMinHeight = xe.zXPathValue("ImageFilterMinHeight").zParseAs<int>();
        }

        public static IEnumerable<WebImage> LoadImages(IEnumerable<WebImage> images, HttpRequestParameters requestParameters = null)
        {
            foreach (WebImage image in images)
            {
                if (LoadImage(image, requestParameters, _imageFilter))
                    yield return image;
            }
            //return imageList.ToArray();
        }

        public static bool LoadImage(WebImage image, HttpRequestParameters requestParameters = null, Predicate<ImageMongoCache> filter = null)
        {
            if (image.Url != null && image.Image == null)
            {
                try
                {
                    if (_imageCacheManager != null)
                    {
                        ImageMongoCache imageCache = (ImageMongoCache)_imageCacheManager.GetImageCache(image.Url, requestParameters);
                        if (filter != null && !filter(imageCache))
                            return false;
                        image.Image = imageCache.Image;
                    }
                    else
                        image.Image = HttpManager.CurrentHttpManager.LoadImage(new HttpRequest { Url = image.Url }, requestParameters);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("error loading image \"{0}\"", image.Url);
                    Trace.WriteLine(ex.Message);
                    //return false;
                }
            }
            return true;
        }

        public static Image LoadImage(string url, HttpRequestParameters requestParameters = null)
        {
            try
            {
                if (_imageCacheManager != null)
                {
                    ImageCache imageCache = _imageCacheManager.GetImageCache(url, requestParameters);
                    return imageCache.Image;
                }
                else
                    return HttpManager.CurrentHttpManager.LoadImage(new HttpRequest { Url = url }, requestParameters);
            }
            catch (Exception ex)
            {
                pb.Trace.WriteLine("error RapideDdl loading image : {0}", ex.Message);
                return null;
            }
        }
    }
}
