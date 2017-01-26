using System;
using pb.Data.Xml;
using pb.Text;

namespace Download.Print
{
    // DownloadPrint_LoadImage
    public static partial class DownloadPrint
    {
        //private static bool __trace = false;
        //private static bool __test = false;
        // add '-' 28/12/2014
        private static char[] __trimChars = new char[] { ' ', '\t', '\n', '\r', ',', '»', '&', '+', '/', '|', '*', '=', '»', '.', '_', '-', ':' };
        private static char[] __trimCharsWithoutColon = new char[] { ' ', '\t', '\n', '\r', ',', '»', '&', '+', '/', '|', '*', '=', '»', '.', '_', '-' };
        private static Func<string, string> __trim = text => text.Trim(__trimChars);
        private static Func<string, string> __trimWithoutColon = text => text.Trim(__trimCharsWithoutColon);
        private static Func<string, string> __replaceChars = text => text != null ? text.Replace('\u2013', '-') : null;
        private static PrintTextValuesManager __printTextValuesManager = null;
        //private static WebImageMongoCacheManager_v1 __imageCacheManager_v1 = null;

        static DownloadPrint()
        {
            Init();
        }

        //public static bool Test { get { return __test; } }

        public static void Init()
        {
            //__test = XmlConfig.CurrentConfig.Get("Test").zTryParseAs(false);
            __printTextValuesManager = new PrintTextValuesManager(new RegexValuesList(XmlConfig.CurrentConfig.GetElements("TextInfos/TextInfo")), __trim);
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

        //public static MongoBackup CreateMongoBackup(params MongoCollection[] collections)
        //{
        //    MongoBackup mongoBackup = new MongoBackup();
        //    mongoBackup.BackupDirectory = XmlConfig.CurrentConfig.GetExplicit("MongoBackupDirectory");
        //    mongoBackup.TempBackupDirectory = XmlConfig.CurrentConfig.GetExplicit("MongoBackupTmpDirectory");
        //    mongoBackup.AddCollections(collections);
        //    return mongoBackup;
        //}

        //public static IEnumerable<BsonDocument> GetPostInfoList(MongoCollection mongoCollection, string server, string query = null, string sort = null, int limit = 0)
        //{
        //    //IEnumerable<BsonDocument> cursor = mongoCollection.zFind<BsonDocument>(query.zToQueryDocument(), sort.zToSortByWrapper(), limit: limit);
        //    foreach (BsonDocument document in mongoCollection.zFind<BsonDocument>(query.zToQueryDocument(), sort.zToSortByWrapper(), limit: limit))
        //    {
        //        BsonDocument documentDownloaded = null;
        //        document.Add(new BsonElement("downloaded", documentDownloaded));
        //        yield return document;
        //    }
        //}
    }
}
