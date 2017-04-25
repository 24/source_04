using pb.Data.Xml;
using pb.Web.Http;

namespace pb.Web.Data.Ocr.Test
{
    public static class Test_OcrWebService
    {
        private static string _configFile = @"c:\pib\drive\google\dev_data\exe\runsource\ocr\config\ocrWebService.config.local.xml";
        private static string _cacheDirectory = @"c:\pib\dev_data\exe\runsource\ocr\cache";

        public static OcrWebService CreateOcrWebService()
        {
            XmlConfig config = new XmlConfig(_configFile);
            OcrWebService ocrWebService = new OcrWebService(config.GetExplicit("UserName"), config.GetExplicit("LicenseCode"));
            //ocrWebService.UserName = config.GetExplicit("UserName");
            //ocrWebService.LicenseCode = config.GetExplicit("LicenseCode");
            if (_cacheDirectory != null)
            {
                UrlCache urlCache = new UrlCache(_cacheDirectory);
                urlCache.UrlFileNameType = UrlFileNameType.Host | UrlFileNameType.Path;
                urlCache.IndexedFile = true;
                ocrWebService.HttpManager.SetCacheManager(urlCache);
            }
            return ocrWebService;
        }
    }
}
