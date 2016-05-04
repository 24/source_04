using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MongoDB.Bson;
using pb;
using pb.Data.Xml;
using pb.Web.Data;
using pb.Data;
using pb.Web;

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

        //public IHeaderData[] Headers;
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

    // IPostToDownload => IHttpRequestData, IKeyData
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

    public class PostHeaderDetailMongoManagerBase<THeaderData, TDetailData> : WebHeaderDetailMongoManagerBase<THeaderData, TDetailData>
    {
        protected virtual void Create(XElement xe)
        {
            CreateHeaderWebDataPageManager(xe.zXPathElement("Header"));
            CreateDetailWebDataManager(xe.zXPathElement("Detail"));
            CreateWebHeaderDetailManager();
        }

        protected ServerManager CreateServerManager(string name, Action loadNewPost = null, Func<DateTime, IEnumerable<IPostToDownload>> getPostList = null)
        {
            if (loadNewPost == null)
                loadNewPost = LoadNewDocuments;
            if (getPostList == null)
                getPostList = Find;
            return new ServerManager
            {
                Name = name,
                EnableLoadNewPost = false,
                EnableSearchPostToDownload = false,
                DownloadDirectory = null,
                LoadNewPost = loadNewPost,
                GetPostList = getPostList,
                LoadPost = LoadDocument,
                Backup = dir => Backup(dir)
            };
        }

        protected virtual void LoadNewDocuments()
        {
            throw new PBException("LoadNewDocuments() not implemented");
        }

        protected virtual IEnumerable<IPostToDownload> Find(DateTime date)
        {
            throw new PBException("Find() not implemented");
        }

        protected virtual IPostToDownload LoadDocument(BsonValue id)
        {
            throw new PBException("LoadDocument() not implemented");
        }
    }
}
