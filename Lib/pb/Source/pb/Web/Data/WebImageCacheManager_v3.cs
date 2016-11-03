using pb.Data;
using pb.IO;
using pb.Web.Data;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace pb.Web
{
    public class WebImageCacheManager_v3 : HttpManager_v2
    {
        public WebImageCacheManager_v3(UrlCache urlCache)
        {
            UrlCache = urlCache;
        }

        // HttpRequestParameters requestParameters = null
        public virtual bool LoadImagesFromWeb(IEnumerable<WebImage> images, WebImageRequest imageRequest, string subDirectory = null)
        {
            if (!imageRequest.LoadImageFromWeb && !imageRequest.RefreshImage)
                return false;
            bool ret = false;
            foreach (WebImage image in images)
            {
                if (LoadImageFromWeb(image, imageRequest, subDirectory))
                    ret = true;
            }
            return ret;
        }

        // HttpRequestParameters requestParameters = null
        protected bool LoadImageFromWeb(WebImage webImage, WebImageRequest imageRequest, string subDirectory = null)
        {
            if (!imageRequest.LoadImageFromWeb || (webImage.File != null && !imageRequest.RefreshImage))
                return false;
            HttpRequest httpRequest = new HttpRequest { Url = webImage.Url, ReloadFromWeb = imageRequest.RefreshImage };

            //string file = _urlCache.GetUrlSubPath(httpRequest);
            //if (subDirectory != null)
            //    file = zPath.Combine(subDirectory, file);
            //string path = zPath.Combine(_urlCache.CacheDirectory, file);

            //if (imageRequest.RefreshImage || !zFile.Exists(path))
            //    HttpManager.CurrentHttpManager.LoadToFile(httpRequest, path, _urlCache.SaveRequest, requestParameters);
            //webImage.File = file;

            //UrlCachePathResult urlCachePath = LoadHttpToCache(httpRequest, subDirectory);
            //webImage.File = urlCachePath.SubPath;
            //string path = urlCachePath.Path;

            //if (zFile.Exists(path))
            //{
            //    Image image = LoadImageFromFile(path);
            //    if (image != null)
            //    {
            //        webImage.Width = image.Width;
            //        webImage.Height = image.Height;
            //        if (imageRequest.LoadImageToData)
            //            webImage.Image = image;
            //    }
            //}

            HttpResult<Image> httpResult = LoadImage(httpRequest, subDirectory);
            if (httpResult.Success)
            {
                Image image = httpResult.Data;
                webImage.File = httpResult.Http.HttpRequest.UrlCachePath?.SubPath;
                webImage.Width = image.Width;
                webImage.Height = image.Height;
                if (imageRequest.LoadImageToData)
                    webImage.Image = image;
                return true;
            }
            else
                return false;
        }

        public virtual void LoadImagesToData(IEnumerable<WebImage> images)
        {
            foreach (WebImage image in images)
            {
                LoadImageToData(image);
            }
        }

        protected void LoadImageToData(WebImage webImage)
        {
            if (webImage.File == null || webImage.Image != null)
                return;
            // _urlCache.CacheDirectory
            string path = zPath.Combine(GetCacheDirectory(), webImage.File);
            if (path != null && zFile.Exists(path))
            {
                webImage.Image = LoadImageFromFile(path);
            }
            else
            {
                Trace.WriteLine("error unable to load image url \"{0}\" from file \"{1}\"", webImage.Url, path);
            }
        }

        protected Image LoadImageFromFile(string path)
        {
            try
            {
                return zimg.LoadBitmapFromFile(path);
            }
            catch (Exception exception)
            {
                Trace.WriteLine("error unable to load image from file \"{0}\"", path);
                Trace.Write("error : ");
                Trace.WriteLine(exception.Message);
                return null;
            }
        }
    }
}
