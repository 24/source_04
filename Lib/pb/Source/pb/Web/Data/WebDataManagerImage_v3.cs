using pb.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pb.Web.Data
{
    public partial class WebDataManager_v3<TData>
    {
        protected WebImageCacheManager_v2 _webImageCacheManager = null;
        protected Predicate<WebImage> _imageFilter = null;
        //protected Func<TData, string> _getImageSubDirectory = null;
        protected Func<WebData<TData>, string> _getImageSubDirectory = null;

        public WebImageCacheManager_v2 WebImageCacheManager { get { return _webImageCacheManager; } set { _webImageCacheManager = value; } }
        public Predicate<WebImage> ImageFilter { get { return _imageFilter; } set { _imageFilter = value; } }
        public Func<WebData<TData>, string> GetImageSubDirectory { get { return _getImageSubDirectory; } set { _getImageSubDirectory = value; } }

        protected bool LoadImagesFromWeb(WebData<TData> webData)
        {
            WebImageRequest imageRequest = webData.Request.ImageRequest;
            if (!imageRequest.LoadImageFromWeb && !imageRequest.LoadImageToData && !imageRequest.RefreshImage)
                return false;

            //if (imageRequest.LoadImageFromWeb || imageRequest.LoadImageToData || imageRequest.RefreshImage)
            //{
            if (!(webData.Document is IGetWebImages))
                throw new PBException($"{typeof(TData).zGetTypeName()} is not IGetWebImages");
            IEnumerable<WebImage> images = ((IGetWebImages)webData.Document).GetWebImages();
            if (_imageFilter != null)
                images = images.Where(image => _imageFilter(image));
            string subDirectory = null;
            if (_getImageSubDirectory != null)
                subDirectory = _getImageSubDirectory(webData);
            return _webImageCacheManager.LoadImagesFromWeb(images, imageRequest, subDirectory: subDirectory);
            //}
        }

        protected void LoadImagesToData(TData data)
        {
            if (!(data is IGetWebImages))
                throw new PBException($"{typeof(TData).zGetTypeName()} is not IGetWebImages");
            IEnumerable<WebImage> images = ((IGetWebImages)data).GetWebImages();
            if (_imageFilter != null)
                images = images.Where(image => _imageFilter(image));
            _webImageCacheManager.LoadImagesToData(images);
        }

        //protected void _LoadImageFromWebCache(WebImage webImage, WebImageRequest imageRequest, HttpRequestParameters requestParameters = null)
        //{
        //    if (!imageRequest.LoadImageFromWeb || (webImage.File != null && !imageRequest.RefreshImage))
        //        return;
        //    HttpRequest httpRequest = new HttpRequest { Url = webImage.Url };
        //    string file = _urlCache.GetUrlSubPath(httpRequest);
        //    path = zPath.Combine(_urlCache.CacheDirectory, file);
        //    if (!zFile.Exists(path))
        //        HttpManager.CurrentHttpManager.LoadToFile(httpRequest, path, _urlCache.SaveRequest, requestParameters);
        //}

        //protected void _LoadImageFromWeb(WebImage webImage, WebImageRequest imageRequest, HttpRequestParameters requestParameters = null)
        //{
        //    if (!imageRequest.LoadImageFromWeb || (webImage.File != null && !imageRequest.RefreshImage))
        //        return;
        //}
    }
}
