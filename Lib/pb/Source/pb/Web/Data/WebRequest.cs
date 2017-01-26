using pb.Web.Http;

namespace pb.Web.Data
{
    // used by :
    //   - WebLoadManager
    //   - WebLoadDataManager<TData>
    //   - WebLoadDataManager_v2<TData>
    public class WebRequest
    {
        private HttpRequest _httpRequest = null;
        private bool _reloadFromWeb = false;
        private bool _refreshDocumentStore = false;
        private WebImageRequest _imageRequest = null;

        public HttpRequest HttpRequest { get { return _httpRequest; } set { _httpRequest = value; } }
        public bool ReloadFromWeb { get { return _reloadFromWeb; } set { _reloadFromWeb = value; } }
        public WebImageRequest ImageRequest { get { return _imageRequest; } set { _imageRequest = value; } }
        public bool RefreshDocumentStore { get { return _refreshDocumentStore; } set { _refreshDocumentStore = value; } }
    }

    // used by :
    //   - WebRequest
    //   - WebImageMongoManager
    //   - WebData<TData>.Load_v1()
    //   - WebData<TData>.Load_v2()
    //   - WebHeaderDetailManager<THeaderData, TDetailData>
    //   - WebHeaderDetailManager_v4<THeaderData, TDetailData>
    //   - WebDataPageManager_v4<TData>
    //   - WebLoadImageManager_v1
    //   - WebLoadImageManager_v2<TData>
    //   - WebImageCacheManager_v2
    //   - WebImageCacheManager_v3
    public class WebImageRequest
    {
        public bool LoadImageFromWeb;
        public bool LoadMissingImageFromWeb;
        public bool LoadImageToData;
        public bool RefreshImage;
    }
}
