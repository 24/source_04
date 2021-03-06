﻿using pb.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;

// - filter
// - http request param, use function _getHttpRequestParameters from WebLoadManager
// - set width and height

namespace pb.Web.Data
{
    public interface IGetWebImages
    {
        //void LoadImages(bool refreshImage = false);
        IEnumerable<WebImage> GetWebImages();
    }

    //public partial class WebDataManager<TData>
    //{
    //    public bool LoadImagesFromWeb(WebData<TData> webData)
    //    {
    //    }

    //    public void LoadImagesToData(TData data)
    //    {
    //    }
    //}

    // WebLoadImageManager<TData> load image from web using a cache
    //   - TData must have interface IGetWebImages to get WebImage list from TData
    //   - ImageFilter is optional
    //   - use WebImageCacheManager_v3 to load image
    // todo :
    //   - replace _getImageSubDirectory with an optional interface
    // used by :
    //   - WebData<TData>.Load_v1()
    //   - WebData<TData>.Load_v2()
    //   - WebDataManager<TData>
    //   - WebDataManager_v4<TData>
    public class WebLoadImageManager_v2<TData>
    //public class WebDataImageManager<TData>
    {
        //protected WebImageCacheManager_v2 _webImageCacheManager = null;
        protected WebImageCacheManager_v3 _webImageCacheManager = null;
        protected Predicate<WebImage> _imageFilter = null;
        //protected Func<WebData<TData>, string> _getImageSubDirectory = null;

        public WebImageCacheManager_v3 WebImageCacheManager { get { return _webImageCacheManager; } set { _webImageCacheManager = value; } }
        public Predicate<WebImage> ImageFilter { get { return _imageFilter; } set { _imageFilter = value; } }
        //public Func<WebData<TData>, string> GetImageSubDirectory { get { return _getImageSubDirectory; } set { _getImageSubDirectory = value; } }

        //public bool LoadImagesFromWeb(WebData<TData> webData)
        //{
        //    WebImageRequest imageRequest = webData.Request.ImageRequest;
        //    if (!imageRequest.LoadImageFromWeb && !imageRequest.LoadImageToData && !imageRequest.RefreshImage)
        //        return false;

        //    if (!(webData.Data is IGetWebImages))
        //        //throw new PBException($"{typeof(TData).zGetTypeName()} is not IGetWebImages");
        //        return false;
        //    IEnumerable<WebImage> images = ((IGetWebImages)webData.Data).GetWebImages();
        //    if (_imageFilter != null)
        //        images = images.Where(image => _imageFilter(image));
        //    string subDirectory = null;
        //    if (_getImageSubDirectory != null)
        //        subDirectory = _getImageSubDirectory(webData);
        //    return _webImageCacheManager.LoadImagesFromWeb(images, imageRequest, subDirectory);
        //}

        public bool LoadImagesFromWeb(WebImageRequest imageRequest, IEnumerable<WebImage> images, string subDirectory = null)
        {
            if (!imageRequest.LoadImageFromWeb && !imageRequest.LoadImageToData && !imageRequest.RefreshImage)
                return false;
            if (_imageFilter != null)
                images = images.Where(image => _imageFilter(image));
            //string subDirectory = null;
            //if (_getImageSubDirectory != null)
            //    subDirectory = _getImageSubDirectory(webData);
            return _webImageCacheManager.LoadImagesFromWeb(images, imageRequest, subDirectory);
        }

        public void LoadImagesToData(TData data)
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
