using System;
using System.Collections.Generic;
using System.Xml.Linq;
using pb.Data;
using pb.IO;
using pb.Web.Data;
using System.Drawing;

// todo :
//   replace string url by HttpRequest

namespace pb.Web
{
    public class WebImageCacheManager_v2
    {
        //protected IDocumentStore<WebImage> _documentStore = null;
        protected UrlCache _urlCache = null;

        public WebImageCacheManager_v2(UrlCache urlCache)
        {
            _urlCache = urlCache;
        }

        public virtual bool LoadImagesFromWeb(IEnumerable<WebImage> images, WebImageRequest imageRequest, HttpRequestParameters requestParameters = null, string subDirectory = null)
        {
            if (!imageRequest.LoadImageFromWeb && !imageRequest.RefreshImage)
                return false;
            bool ret = false;
            foreach (WebImage image in images)
            {
                if (LoadImageFromWeb(image, imageRequest, requestParameters, subDirectory))
                    ret = true;
            }
            return ret;
        }

        protected bool LoadImageFromWeb(WebImage webImage, WebImageRequest imageRequest, HttpRequestParameters requestParameters = null, string subDirectory = null)
        {
            if (!imageRequest.LoadImageFromWeb || (webImage.File != null && !imageRequest.RefreshImage))
                return false;
            HttpRequest httpRequest = new HttpRequest { Url = webImage.Url };
            string file = _urlCache.GetUrlSubPath(httpRequest);
            if (subDirectory != null)
                file = zPath.Combine(subDirectory, file);
            string path = zPath.Combine(_urlCache.CacheDirectory, file);
            if (imageRequest.RefreshImage || !zFile.Exists(path))
                HttpManager.CurrentHttpManager.LoadToFile(httpRequest, path, _urlCache.SaveRequest, requestParameters);
            webImage.File = file;

            if (zFile.Exists(path))
            {
                Image image = LoadImageFromFile(path);
                if (image != null)
                {
                    webImage.Width = image.Width;
                    webImage.Height = image.Height;
                    if (imageRequest.LoadImageToData)
                        webImage.Image = image;
                }
            }
            return true;
        }

        public virtual void LoadImagesToData(IEnumerable<WebImage> images)
        {
            //if (!imageRequest.LoadImageToData)
            //    return;
            foreach (WebImage image in images)
            {
                LoadImageToData(image);
            }
        }

        protected void LoadImageToData(WebImage webImage)
        {
            if (webImage.File == null || webImage.Image != null)
                return;
            string path = zPath.Combine(_urlCache.CacheDirectory, webImage.File);
            if (path != null && zFile.Exists(path))
            {
                //try
                //{
                //    webImage.Image = zimg.LoadFromFile(path);
                //}
                //catch (Exception exception)
                //{
                //    Trace.WriteLine("error unable to load image url \"{0}\" from file \"{1}\"", webImage.Url, path);
                //    Trace.Write("error : ");
                //    Trace.WriteLine(exception.Message);
                //}
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

        //public virtual bool LoadImage(WebImage webImage, ImageRequest imageRequest, HttpRequestParameters requestParameters = null)
        //{
        //    string file = null;
        //    if (!imageRequest.RefreshImage)
        //    {
        //        WebImage webImage2 = _documentStore.LoadFromId(webImage.Url);
        //        if (webImage2 != null)
        //        {
        //            webImage.Width = webImage2.Width;
        //            webImage.Heigth = webImage2.Heigth;
        //            file = webImage2.File;
        //        }
        //    }

        //    string path = null;
        //    if (imageRequest.LoadImageFromWeb && file == null)
        //    {
        //        HttpRequest httpRequest = new HttpRequest { Url = webImage.Url };
        //        file = _urlCache.GetUrlSubPath(httpRequest);
        //        path = zPath.Combine(_urlCache.CacheDirectory, file);
        //        if (!zFile.Exists(path))
        //            HttpManager.CurrentHttpManager.LoadToFile(httpRequest, path, _urlCache.SaveRequest, requestParameters);
        //    }

        //    if (imageRequest.LoadImageToData)
        //    {
        //        Image image = null;
        //        if (path == null && file != null)
        //            path = zPath.Combine(_urlCache.CacheDirectory, file);
        //        if (path != null && zFile.Exists(path))
        //        {
        //            try
        //            {
        //                image = zimg.LoadFromFile(path);
        //            }
        //            catch (Exception exception)
        //            {
        //                Trace.WriteLine("error unable to load image url \"{0}\" to \"{1}\" (WebImageMongoCacheManager.CreateMongoImage())", webImage.Url, path);
        //                Trace.Write("error : ");
        //                Trace.WriteLine(exception.Message);
        //            }
        //        }
        //        else
        //        {
        //            Trace.WriteLine("error unable to load image url \"{0}\" to \"{1}\" (WebImageMongoCacheManager.CreateMongoImage())", webImage.Url, path);
        //        }
        //    }

        //    MongoImage mongoImage = new MongoImage();
        //    mongoImage.Url = url;
        //    mongoImage.File = file;
        //    mongoImage.Width = image != null ? image.Width : 0;
        //    mongoImage.Height = image != null ? image.Height : 0;
        //    mongoImage.Image = image;

        //    GetCollection().zUpdate(new QueryDocument { { "_id", BsonValue.Create(url) } }, new UpdateDocument { { "$set", mongoImage.ToBsonDocument() } }, UpdateFlags.Upsert);

        //    if (webImage.Url != null && webImage.Image == null)
        //    {
        //        try
        //        {
        //            Image image = null;
        //            if (_imageCacheManager != null)
        //            {
        //                ImageMongoCache imageCache = (ImageMongoCache)_imageCacheManager.GetImageCache(webImage.Url, requestParameters);
        //                //if (_imageFilter != null && !_imageFilter(imageCache))
        //                //    return false;
        //                image = imageCache.Image;
        //            }
        //            else if (imageRequest.LoadImageFromWeb)
        //                image = HttpManager.CurrentHttpManager.LoadImage(new HttpRequest { Url = webImage.Url }, requestParameters);
        //            if (imageRequest.LoadImageToData)
        //                webImage.Image = image;
        //        }
        //        catch (Exception ex)
        //        {
        //            Trace.WriteLine("error loading image \"{0}\"", webImage.Url);
        //            Trace.WriteLine(ex.Message);
        //            //return false;
        //        }
        //    }
        //    return true;
        //}

        protected virtual string GetUrlSubDirectory(HttpRequest httpRequest)
        {
            return null;
        }

        protected void Create(XElement xe = null)
        {
            _urlCache = UrlCache.Create(xe);
            if (_urlCache != null)
                //_urlCache.GetUrlSubDirectory = httpRequest => zurl.GetDomain(httpRequest.Url);
                _urlCache.GetUrlSubDirectory = GetUrlSubDirectory;
        }
    }
}
