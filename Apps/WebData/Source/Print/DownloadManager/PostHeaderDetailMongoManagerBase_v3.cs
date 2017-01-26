using CenterCLR;
using MongoDB.Bson;
using pb;
using pb.Data.Xml;
using pb.Web.Data;
using pb.Web.Data.Mongo;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Download.Print
{
    public class PostHeaderDetailMongoManagerBase_v3<THeaderData, TDetailData> : WebHeaderDetailMongoManagerBase_v2<THeaderData, TDetailData>, IServerManager where TDetailData : IPostToDownload
    {
        private bool _enableLoadNewDocument = false;
        private bool _enableSearchDocumentToDownload = false;
        private string _downloadDirectory = null;
        private int? _loadNewDocumentsMaxNbDocumentsLoadedFromStore = null;
        private int? _loadNewDocumentsStartPage = null;
        private int? _loadNewDocumentsMaxPage = null;
        private bool? _loadNewDocumentsLoadImageFromWeb = null;
        private string _findFromDateTimeQuery = null;
        private string _findFromDateTimeSort = null;

        // IServerManager properties
        public virtual string Name { get { throw new PBException("Name not implemented"); } }
        public virtual bool EnableLoadNewDocument { get { return _enableLoadNewDocument; } set { _enableLoadNewDocument = value; } }
        public virtual bool EnableSearchDocumentToDownload { get { return _enableSearchDocumentToDownload; } set { _enableSearchDocumentToDownload = value; } }
        public virtual string DownloadDirectory { get { return _downloadDirectory; } set { _downloadDirectory = value; } }

        protected override void Create(XElement xe)
        {
            base.Create(xe);
            XElement xe2 = xe.zXPathElement("LoadNewDocuments");
            if (xe2 != null)
            {
                _loadNewDocumentsMaxNbDocumentsLoadedFromStore = xe2.zXPathValue("MaxDocumentsLoadedFromStore").zParseAs<int?>();
                _loadNewDocumentsStartPage = xe2.zXPathValue("StartPage").zParseAs<int?>();
                _loadNewDocumentsMaxPage = xe2.zXPathValue("MaxPage").zParseAs<int?>();
                _loadNewDocumentsLoadImageFromWeb = xe2.zXPathValue("LoadImageFromWeb").zParseAs<bool?>();
            }
            //_findFromDateTimeQuery
            xe2 = xe.zXPathElement("FindFromDateTime");
            if (xe2 != null)
            {
                _findFromDateTimeQuery = xe2.zXPathValue("Query");
                _findFromDateTimeSort = xe2.zXPathValue("Sort");
            }
        }

        // IServerManager method
        public virtual LoadNewDocumentsResult LoadNewDocuments()
        {
            //throw new PBException("LoadNewDocuments() not implemented");
            if (_loadNewDocumentsMaxNbDocumentsLoadedFromStore == null || _loadNewDocumentsStartPage == null || _loadNewDocumentsMaxPage == null || _loadNewDocumentsLoadImageFromWeb == null)
                throw new PBException("load new documents missing parameter(s)");
            return _headerDetailManager.LoadNewDocuments(maxDocumentsLoadedFromStore: (int)_loadNewDocumentsMaxNbDocumentsLoadedFromStore, startPage: (int)_loadNewDocumentsStartPage, maxPage: (int)_loadNewDocumentsMaxPage,
                webImageRequest: new WebImageRequest { LoadImageFromWeb = (bool)_loadNewDocumentsLoadImageFromWeb });
        }

        // IServerManager method
        //public virtual LoadNewDocumentsResult LoadNewDocuments(int maxNbDocumentsLoadedFromStore = 5, int startPage = 1, int maxPage = 20, WebImageRequest webImageRequest = null)
        //{
        //    return _headerDetailManager.LoadNewDocuments(maxDocumentsLoadedFromStore: maxNbDocumentsLoadedFromStore, startPage: startPage, maxPage: maxPage, webImageRequest: webImageRequest);
        //}

        // IServerManager method
        public virtual IEnumerable<IPostToDownload> FindFromDateTime(DateTime datetime)
        {
            //throw new PBException("FindFromDateTime(DateTime dateTime) not implemented");
            if (_findFromDateTimeQuery == null)
                throw new PBException("FindFromDateTime query undefined");
            string query = Named.Format(_findFromDateTimeQuery, new Dictionary<string, object> { { "date", datetime.ToUniversalTime().ToString("o") } });
            string sort = _findFromDateTimeSort;
            if (sort == null)
                sort = _detailDataManager.DataStore.DefaultSort;
            return (IEnumerable<IPostToDownload>)_detailDataManager.Find(query, sort: sort, loadImage: false);
        }

        // IServerManager method
        public virtual IEnumerable<IPostToDownload> Find(string query = null, string sort = null, int limit = 0, bool loadImage = false)
        {
            //throw new PBException("Find(string query = null, string sort = null, int limit = 0, bool loadImage = false) not implemented");
            if (sort == null)
                sort = _detailDataManager.DataStore.DefaultSort;
            return (IEnumerable<IPostToDownload>)_detailDataManager.Find(query, sort: sort, limit: limit, loadImage: loadImage);
        }

        // IServerManager method
        public virtual IPostToDownload Load(BsonValue id)
        {
            //throw new PBException("Load(BsonValue id) not implemented");
            return _detailDataManager.LoadFromId(id);
        }
    }
}
