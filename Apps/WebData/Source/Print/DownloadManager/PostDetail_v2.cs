using MongoDB.Bson;
using pb.Data;
using pb.Web.Data;
using pb.Web.Http;
using System;
using System.Collections.Generic;

namespace Download.Print
{
    //public class PostHeader_v2 : IHeaderData
    //{
    //    public string SourceUrl;
    //    public DateTime? LoadFromWebDate;
    //    public string Title;
    //    public string Date;
    //    public string UrlDetail;

    //    public WebImage[] Images;

    //    public HttpRequest GetHttpRequestDetail()
    //    {
    //        return new HttpRequest { Url = UrlDetail };
    //    }
    //}

    public class PostHeaderHeaderDataPages_v2 : DataPages<PostHeader>, IKeyData
    {
        public int Id;

        public BsonValue GetKey()
        {
            return Id;
        }
    }

    public abstract class PostDetailSimpleLinks_v2 : PostDetailBase_v2
    {
        public string[] DownloadLinks;

        // IPostToDownload
        public override PostDownloadLinks GetDownloadLinks()
        {
            return PostDownloadLinks.Create(DownloadLinks);
        }
    }

    public abstract class PostDetailMultiLinks_v2 : PostDetailBase_v2
    {
        public PostDownloadLinks DownloadLinks;

        public PostDetailMultiLinks_v2()
        {
            DownloadLinks = new PostDownloadLinks();
        }

        // IPostToDownload
        public override PostDownloadLinks GetDownloadLinks()
        {
            return DownloadLinks;
        }
    }

    // IPostToDownload => IHttpRequestData, IKeyData
    // ILoadImages
    public abstract class PostDetailBase_v2 : IPostToDownload, IGetWebImages
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

        // IGetWebImages
        public IEnumerable<WebImage> GetWebImages()
        {
            return Images;
        }

        //public virtual void LoadImages()
        //{
        //    Images = DownloadPrint.LoadImages(Images).ToArray();
        //}

        //ILoadImages
        //public void LoadImages(WebImageRequest imageRequest)
        //{
        //    WebImageMongoManager.Current.LoadImages(Images, imageRequest);
        //}

        public abstract PostDownloadLinks GetDownloadLinks();
    }
}
