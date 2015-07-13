using System;
using System.Collections.Generic;
using System.Xml.Linq;
using pb;
using pb.Data.Xml;
using pb.Text;
using pb.Web;
using pb.Web.old;
using Print;

namespace Download.Print.FreeTelechargement
{
    public static class FreeTelechargement
    {
        private static bool __trace = false;
        //private static char[] _trimChars = new char[] { ' ', '\t', '\n', '\r', ',', '»', '&', '+', '/', '|', '*', '=', '»', '.', ':', '_' };
        //private static Func<string, string> _trimFunc1 = text => text.Trim(_trimChars);
        private static PrintTextValuesManager _printTextValuesManager;
        private static WebImageMongoCacheManager_v1 __imageCacheManager = null;
        private static int __imageFilterMinHeight = 0;   // 70
        private static Predicate<ImageMongoCache_v1> __imageFilter = imageCache => (__imageFilterMinHeight == 0 || imageCache.Height > __imageFilterMinHeight) && imageCache.MongoImage.Category != "layout";

        static FreeTelechargement()
        {
            Init();
        }

        public static void Init()
        {
            InitImage(XmlConfig.CurrentConfig.GetElement("Image"));
            //_textInfoRegexList = new RegexValuesList(XmlConfig.CurrentConfig.GetElements("TextInfos/TextInfo"));
            //_printTextValuesManager = new PrintTextValuesManager(new RegexValuesList(XmlConfig.CurrentConfig.GetElements("TextInfos/TextInfo")), _trimFunc1);
            _printTextValuesManager = new PrintTextValuesManager(new RegexValuesList(XmlConfig.CurrentConfig.GetElements("TextInfos/TextInfo")), DownloadPrint.Trim);
        }

        public static void InitImage(XElement xe)
        {
            //if (xe.zXPathValueBool("UseUrlCache", false))
            if (xe.zXPathValue("UseUrlCache").zTryParseAs(false))
                __imageCacheManager = new WebImageMongoCacheManager_v1(xe.zXPathValue("MongoServer"), xe.zXPathValue("MongoDatabase"), xe.zXPathValue("MongoCollection"), xe.zXPathValue("CacheDirectory"));
            __imageFilterMinHeight = xe.zXPathValue("ImageFilterMinHeight").zParseAs<int>();
        }

        public static bool Trace { get { return __trace; } set { __trace = value; } }
        //public static Func<string, string> TrimFunc1 { get { return _trimFunc1; } }
        public static PrintTextValuesManager PrintTextValuesManager { get { return _printTextValuesManager; } }

        public static void LoadImages(List<WebImage> images, HttpRequestParameters_v1 requestParameters = null)
        {
            for (int i = 0; i < images.Count; )
            {
                if (!LoadImage(images[i], requestParameters, __imageFilter))
                    images.RemoveAt(i);
                else
                    i++;
            }
        }

        public static bool LoadImage(WebImage image, HttpRequestParameters_v1 requestParameters = null, Predicate<ImageMongoCache_v1> filter = null)
        {
            if (image.Url != null && image.Image == null)
            {
                if (__imageCacheManager != null)
                {
                    ImageMongoCache_v1 imageCache = (ImageMongoCache_v1)__imageCacheManager.GetImageCache(image.Url, requestParameters);
                    if (filter != null && !filter(imageCache))
                        return false;
                    image.Image = imageCache.Image;
                }
                else
                    image.Image = pb.old.Http_v2.LoadImageFromWeb(image.Url, requestParameters);
            }
            return true;
        }

    }
}
