using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using pb;
using pb.Data.Xml;
using pb.Text;
using pb.Web;
using Print;

namespace Download.Print
{
    public static class DownloadPrint
    {
        //private static bool __trace = false;
        // add '-' 28/12/2014
        private static char[] __trimChars = new char[] { ' ', '\t', '\n', '\r', ',', '»', '&', '+', '/', '|', '*', '=', '»', '.', '_', '-', ':' };
        private static char[] __trimCharsWithoutColon = new char[] { ' ', '\t', '\n', '\r', ',', '»', '&', '+', '/', '|', '*', '=', '»', '.', '_', '-' };
        private static Func<string, string> __trim = text => text.Trim(__trimChars);
        private static Func<string, string> __trimWithoutColon = text => text.Trim(__trimCharsWithoutColon);
        private static Func<string, string> __replaceChars = text => text.Replace('\u2013', '-');
        private static PrintTextValuesManager __printTextValuesManager = null;
        //private static WebImageMongoCacheManager_v1 __imageCacheManager_v1 = null;
        private static WebImageMongoCacheManager __imageCacheManager = null;
        private static int __imageFilterMinHeight = 0;   // 70
        //private static Predicate<ImageMongoCache_v1> __imageFilter_v1 = imageCache => (__imageFilterMinHeight == 0 || imageCache.Height > __imageFilterMinHeight) && imageCache.MongoImage.Category != "layout";
        private static Predicate<ImageMongoCache> __imageFilter = imageCache => (__imageFilterMinHeight == 0 || imageCache.Height > __imageFilterMinHeight) && imageCache.MongoImage.Category != "layout";

        static DownloadPrint()
        {
            Init();
        }

        public static void Init()
        {
            InitImage(XmlConfig.CurrentConfig.GetElement("Image"));
            __printTextValuesManager = new PrintTextValuesManager(new RegexValuesList(XmlConfig.CurrentConfig.GetElements("TextInfos/TextInfo")), __trim);
        }

        public static void InitImage(XElement xe)
        {
            //if (xe.zXPathValueBool("UseUrlCache", false))
            if (xe.zXPathValue("UseUrlCache").zTryParseAs(false))
            {
                //__imageCacheManager_v1 = new WebImageMongoCacheManager_v1(xe.zXPathValue("MongoServer"), xe.zXPathValue("MongoDatabase"), xe.zXPathValue("MongoCollection"), xe.zXPathValue("CacheDirectory"));
                __imageCacheManager = new WebImageMongoCacheManager(xe.zXPathValue("MongoServer"), xe.zXPathValue("MongoDatabase"), xe.zXPathValue("MongoCollection"), xe.zXPathValue("CacheDirectory"));
            }
            __imageFilterMinHeight = xe.zXPathValue("ImageFilterMinHeight").zParseAs<int>();
        }

        //public static bool Trace { get { return __trace; } set { __trace = value; } }
        public static char[] TrimChars { get { return __trimChars; } }
        public static char[] TrimCharsWithoutColon { get { return __trimCharsWithoutColon; } }
        public static Func<string, string> Trim { get { return __trim; } }
        public static Func<string, string> TrimWithoutColon { get { return __trimWithoutColon; } }
        public static Func<string, string> ReplaceChars { get { return __replaceChars; } }
        public static PrintTextValuesManager PrintTextValuesManager { get { return __printTextValuesManager; } }

        //public static void LoadImages(IPost post)
        //{
        //    post.SetImages(LoadImages(post.GetImages()).ToArray());
        //}

        //public static UrlImage[] LoadImages(UrlImage[] images, HttpRequestParameters requestParameters = null)
        //{
        //    List<UrlImage> imageList = new List<UrlImage>(images);
        //    for (int i = 0; i < imageList.Count; )
        //    {
        //        if (!LoadImage(imageList[i], requestParameters, __imageFilter))
        //            imageList.RemoveAt(i);
        //        else
        //            i++;
        //    }
        //    return imageList.ToArray();
        //}

        //public static IEnumerable<UrlImage> LoadImages(IEnumerable<UrlImage> images, HttpRequestParameters requestParameters = null)
        //{
        //    //List<UrlImage> imageList = new List<UrlImage>(images);
        //    //for (int i = 0; i < imageList.Count; )
        //    foreach (UrlImage image in images)
        //    {
        //        //if (!LoadImage(imageList[i], requestParameters, __imageFilter))
        //        //    imageList.RemoveAt(i);
        //        //else
        //        //    i++;
        //        if (LoadImage(image, requestParameters, __imageFilter))
        //            yield return image;
        //    }
        //    //return imageList.ToArray();
        //}

        //public static bool LoadImage(UrlImage image, HttpRequestParameters requestParameters = null, Predicate<ImageMongoCache> filter = null)
        //{
        //    if (image.Url != null && image.Image == null)
        //    {
        //        if (__imageCacheManager != null)
        //        {
        //            ImageMongoCache imageCache = (ImageMongoCache)__imageCacheManager.GetImageCache(image.Url, requestParameters);
        //            if (filter != null && !filter(imageCache))
        //                return false;
        //            image.Image = imageCache.Image;
        //        }
        //        else
        //            image.Image = Http2.LoadImageFromWeb(image.Url, requestParameters);
        //    }
        //    return true;
        //}

        //public static Image LoadImage(string url, HttpRequestParameters requestParameters = null)
        //{
        //    try
        //    {
        //        if (__imageCacheManager != null)
        //        {
        //            ImageCache imageCache = __imageCacheManager.GetImageCache(url, requestParameters);
        //            return imageCache.Image;
        //        }
        //        else
        //            return Http2.LoadImageFromWeb(url, requestParameters);
        //    }
        //    catch (Exception ex)
        //    {
        //        pb.Trace.WriteLine("error RapideDdl loading image : {0}", ex.Message);
        //        return null;
        //    }
        //}

        public static void LoadImages(IPost post)
        {
            post.SetImages(LoadImages(post.GetImages()).ToArray());
        }

        //public static void LoadImages_new(List<UrlImage> images, HttpRequestParameters_new requestParameters = null)
        //{
        //    for (int i = 0; i < images.Count; )
        //    {
        //        if (!LoadImage_new(images[i], requestParameters, __imageFilter_new))
        //            images.RemoveAt(i);
        //        else
        //            i++;
        //    }
        //}

        public static IEnumerable<WebImage> LoadImages(IEnumerable<WebImage> images, HttpRequestParameters requestParameters = null)
        {
            foreach (WebImage image in images)
            {
                if (LoadImage(image, requestParameters, __imageFilter))
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
                    if (__imageCacheManager != null)
                    {
                        ImageMongoCache imageCache = (ImageMongoCache)__imageCacheManager.GetImageCache(image.Url, requestParameters);
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
                if (__imageCacheManager != null)
                {
                    ImageCache imageCache = __imageCacheManager.GetImageCache(url, requestParameters);
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
