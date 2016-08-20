using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using pb;
using pb.Web.Data;
using pb.Data;
using pb.Web;
using pb.Web.Data.Mongo;

// doc
//   PostHeaderDetailMongoManagerBase :
//     public class PostHeaderDetailMongoManagerBase<THeaderData, TDetailData> : WebHeaderDetailMongoManagerBase<THeaderData, TDetailData>
//     CreateServerManager() :
//       ServerManager CreateServerManager(string name, Action loadNewPost = null, Func<DateTime, IEnumerable<IPostToDownload>> getPostList = null, Func<string, string, IEnumerable<IPostToDownload>> findPost = null)
//     LoadNewDocuments() (not implemented)
//       protected virtual void LoadNewDocuments()
//     Find(DateTime date) (not implemented)
//       protected virtual IEnumerable<IPostToDownload> Find(DateTime date)
//     Find(string query, string sort) (not implemented)
//       protected virtual IEnumerable<IPostToDownload> Find(string query, string sort)
//     LoadDocument(BsonValue id) (not implemented)
//       protected virtual IPostToDownload LoadDocument(BsonValue id)

namespace Download.Print
{
    public class PostHeader : IHeaderData
    {
        public string SourceUrl;
        public DateTime? LoadFromWebDate;
        public string Title;
        public string UrlDetail;

        public WebImage[] Images;

        public HttpRequest GetHttpRequestDetail()
        {
            return new HttpRequest { Url = UrlDetail };
        }
    }

    public class PostHeaderDataPage<TDataPage> : IEnumDataPages<TDataPage>, IKeyData where TDataPage : IHeaderData
    {
        public int Id;
        public string SourceUrl;
        public DateTime LoadFromWebDate;

        public TDataPage[] Headers;
        public string UrlNextPage;

        // IKeyData
        public BsonValue GetKey()
        {
            return Id;
        }

        //public IEnumerable<IHeaderData> GetDataList()
        public IEnumerable<TDataPage> GetDataList()
        {
            return Headers;
        }

        // IEnumDataPages<TData>
        public HttpRequest GetHttpRequestNextPage()
        {
            if (UrlNextPage != null)
                return new HttpRequest { Url = UrlNextPage };
            else
                return null;
        }
    }

    // IPostToDownload => IHttpRequestData, IKeyData, ILoadImages
    public abstract class PostDetailBase : IPostToDownload
    {
        public BsonValue Id;
        public string SourceUrl;
        public DateTime LoadFromWebDate;
        public string Title;
        public PrintType PrintType;
        public WebImage[] Images;

        // IPostToDownload
        public abstract string GetServer();

        // IPostToDownload:IKeyData
        public virtual BsonValue GetKey()
        {
            return Id;
        }

        // IPostToDownload:IHttpRequestData
        public virtual HttpRequest GetDataHttpRequest()
        {
            return new HttpRequest { Url = SourceUrl };
        }

        // IPostToDownload
        public string GetTitle()
        {
            return Title;
        }

        // IPostToDownload
        public virtual PrintType GetPrintType()
        {
            return PrintType;
        }

        public virtual void LoadImages()
        {
            Images = DownloadPrint.LoadImages(Images).ToArray();
        }

        //ILoadImages
        public void LoadImages(WebImageRequest imageRequest)
        {
            WebImageMongoManager.Current.LoadImages(Images, imageRequest);
        }

        // IPostToDownload
        //public string[] GetDownloadLinks()
        //{
        //    return DownloadLinks;
        //}

        public abstract PostDownloadLinks GetDownloadLinks();
    }

    public abstract class PostDetailSimpleLinks : PostDetailBase
    {
        public string[] DownloadLinks;

        // IPostToDownload
        public override PostDownloadLinks GetDownloadLinks()
        {
            return PostDownloadLinks.Create(DownloadLinks);
        }
    }

    public abstract class PostDetailMultiLinks : PostDetailBase
    {
        public PostDownloadLinks DownloadLinks;

        public PostDetailMultiLinks()
        {
            DownloadLinks = new PostDownloadLinks();
        }

        // IPostToDownload
        public override PostDownloadLinks GetDownloadLinks()
        {
            return DownloadLinks;
        }
    }

    public class PostHeaderDetailMongoManagerBase<THeaderData, TDetailData> : WebHeaderDetailMongoManagerBase<THeaderData, TDetailData>, IServerManager
    {
        private bool _enableLoadNewDocument = false;
        private bool _enableSearchDocumentToDownload = false;
        private string _downloadDirectory = null;

        // IServerManager properties
        public virtual string Name { get { throw new PBException("Name not implemented"); } }
        //public virtual bool EnableLoadNewPost { get { throw new PBException("EnableLoadNewPost not implemented"); } set { throw new PBException("EnableLoadNewPost not implemented"); } }
        //public virtual bool EnableSearchPostToDownload { get { throw new PBException("EnableSearchPostToDownload not implemented"); } set { throw new PBException("EnableSearchPostToDownload not implemented"); } }
        //public virtual string DownloadDirectory { get { throw new PBException("DownloadDirectory not implemented"); } set { throw new PBException("DownloadDirectory not implemented"); } }
        public virtual bool EnableLoadNewDocument { get { return _enableLoadNewDocument; } set { _enableLoadNewDocument = value; } }
        public virtual bool EnableSearchDocumentToDownload { get { return _enableSearchDocumentToDownload; } set { _enableSearchDocumentToDownload = value; } }
        public virtual string DownloadDirectory { get { return _downloadDirectory; } set { _downloadDirectory = value; } }

        //protected virtual void Create(XElement xe)
        //protected virtual void CreateDataManager(XElement xe)
        //{
        //    CreateHeaderWebDataPageManager(xe.zXPathElement("Header"));
        //    CreateDetailWebDataManager(xe.zXPathElement("Detail"));
        //    CreateWebHeaderDetailManager();
        //}

        //protected ServerManager CreateServerManager(string name, Action loadNewPost = null, Func<DateTime, IEnumerable<IPostToDownload>> getPostList = null, Func<string, string, int, IEnumerable<IPostToDownload>> findPost = null)
        //{
        //    if (loadNewPost == null)
        //        loadNewPost = LoadNewDocuments;
        //    if (getPostList == null)
        //        getPostList = Find;
        //    if (findPost == null)
        //        findPost = Find;
        //    return new ServerManager
        //    {
        //        Name = name,
        //        EnableLoadNewPost = false,
        //        EnableSearchPostToDownload = false,
        //        DownloadDirectory = null,
        //        LoadNewPost = loadNewPost,
        //        GetPostList = getPostList,
        //        FindPost = findPost,
        //        LoadPost = LoadDocument,
        //        Backup = dir => Backup(dir)
        //    };
        //}

        // IServerManager method
        public virtual void LoadNewDocuments()
        {
            throw new PBException("LoadNewDocuments() not implemented");
        }

        // IServerManager method
        public virtual LoadNewDocumentsResult LoadNewDocuments(int maxNbDocumentsLoadedFromStore = 5, int startPage = 1, int maxPage = 20, bool loadImageFromWeb = true)
        {
            //throw new PBException("LoadNewDocuments(int maxNbDocumentsLoadedFromStore = 5, int startPage = 1, int maxPage = 20, bool loadImage = true) not implemented");
            return _headerDetailManager.LoadNewDocuments(maxNbDocumentsLoadedFromStore: maxNbDocumentsLoadedFromStore, startPage: startPage, maxPage: maxPage, loadImageFromWeb: loadImageFromWeb);
        }

        // getPostList
        // IServerManager method
        public virtual IEnumerable<IPostToDownload> FindFromDateTime(DateTime dateTime)
        {
            throw new PBException("FindFromDateTime(DateTime dateTime) not implemented");
        }

        // IServerManager method
        public virtual IEnumerable<IPostToDownload> Find(string query = null, string sort = null, int limit = 0, bool loadImage = false)
        {
            throw new PBException("Find(string query = null, string sort = null, int limit = 0, bool loadImage = false) not implemented");
        }

        public virtual IPostToDownload Load(BsonValue id)
        {
            throw new PBException("Load(BsonValue id) not implemented");
        }

        //protected virtual IEnumerable<IPostToDownload> Find(DateTime date)
        //{
        //    throw new PBException("Find(DateTime date) not implemented");
        //}

        //protected virtual IEnumerable<IPostToDownload> Find(string query, string sort, int limit = 0)
        //{
        //    throw new PBException("Find(string query, string sort, int limit = 0) not implemented");
        //}

        //protected virtual IPostToDownload LoadDocument(BsonValue id)
        //{
        //    throw new PBException("LoadDocument() not implemented");
        //}
    }
}
