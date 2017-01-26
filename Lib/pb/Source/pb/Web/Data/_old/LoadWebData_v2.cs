using pb.Web.old;
using pb.Data.old;

namespace pb.Web.Data.v1
{
    // pb.Web.v1.RequestFromWeb  pb.Web.v1.LoadDataFromWeb<T>
    public class WebDataRequest_v2<T> : RequestFromWeb_v2, IDocumentRequest_v1<T>
    {
        private bool _refreshDocumentStore = false;
        private bool _documentLoaded = false;
        private object _key = null;
        private T _document;

        public WebDataRequest_v2(string url, HttpRequestParameters_v1 requestParameters = null, bool reload = false, bool loadImage = false)
            : base(url, requestParameters, reload, loadImage)
        {
        }

        public WebDataRequest_v2(RequestFromWeb_v2 request)
            : base(request.Url, request.RequestParameters, request.ReloadFromWeb, request.LoadImage)
        {
        }

        public bool RefreshDocumentStore { get { return _refreshDocumentStore; } set { _refreshDocumentStore = value; } }

        public T Document { get { return _document; } set { _document = value; } }

        public bool DocumentLoaded { get { return _documentLoaded; } set { _documentLoaded = value; } }

        public object Key { get { return _key; } set { _key = value; } }
    }

    public class LoadWebData_v2<T>
    {
        private LoadDataFromWeb_v2<T> _loadDataFromWeb = null;
        private IDocumentStore_v1<T> _documentStore = null;
        private bool _desactivateDocumentStore = false;

        static LoadWebData_v2()
        {
            //if (!BsonClassMap.IsClassMapRegistered(typeof(ImageHtml)))
            //{
            //    BsonClassMap.RegisterClassMap<ImageHtml>(cm => { cm.AutoMap(); cm.UnmapProperty(c => c.Image); });
            //}
        }

        public LoadWebData_v2(LoadDataFromWeb_v2<T> loadDataFromWeb, IDocumentStore_v1<T> documentStore = null)
        {
            _loadDataFromWeb = loadDataFromWeb;
            _documentStore = documentStore;
        }

        public bool DesactivateDocumentStore { get { return _desactivateDocumentStore; } set { _desactivateDocumentStore = value; } }

        public T Load(RequestFromWeb_v2 request, object key = null, bool refreshDocumentStore = false)
        {
            WebDataRequest_v2<T> dataRequest = new WebDataRequest_v2<T>(request);
            dataRequest.RefreshDocumentStore = refreshDocumentStore;
            dataRequest.Key = key;
            if (dataRequest.ReloadFromWeb || dataRequest.RefreshDocumentStore || !DocumentExists(dataRequest))
            {
                _LoadDocumentFromWeb(dataRequest);
                SaveDocument(dataRequest);
            }
            LoadDocument(dataRequest);
            return dataRequest.Document;
        }

        private bool DocumentExists(WebDataRequest_v2<T> dataRequest)
        {
            if (_documentStore != null && !_desactivateDocumentStore)
                return _documentStore.DocumentExists(dataRequest);
            else
                return false;
        }

        private void LoadDocument(WebDataRequest_v2<T> dataRequest)
        {
            if (_documentStore != null && !_desactivateDocumentStore)
            {
                Trace.WriteLine("IDocumentStore.LoadDocument(\"{0}\")", dataRequest.Url);
                _documentStore.LoadDocument(dataRequest);
            }
            else
                _LoadDocumentFromWeb(dataRequest);
        }

        private void SaveDocument(WebDataRequest_v2<T> dataRequest)
        {
            if (_documentStore != null && !_desactivateDocumentStore)
                _documentStore.SaveDocument(dataRequest);
        }

        private void _LoadDocumentFromWeb(WebDataRequest_v2<T> dataRequest)
        {
            if (!dataRequest.DocumentLoaded)
            {
                //dataRequest.SetDocument(_loadDataFromWeb.Load(dataRequest));
                dataRequest.Document = _loadDataFromWeb.Load(dataRequest);
                dataRequest.DocumentLoaded = true;
            }
        }

        //protected string GetImageFile()
        //{
        //    throw new NotImplementedException();
        //    //if (_imageFile == null)
        //    //    _imageFile = Path.Combine(_imageCacheDirectory, Path.GetFileNameWithoutExtension(GetUrlCachePath()));
        //    //return _imageFile;
        //}
    }
}
