using MongoDB.Bson;
using pb;
using pb.Data;
using pb.Web;
using pb.Web.Data;
using System;
using System.Collections.Generic;

namespace WebData.BlogDemoor
{
    public class BlogDemoorHeaderData : IHeaderData
    {
        public string SourceUrl;
        public DateTime? LoadFromWebDate;
        public string Title;
        public string Date;
        public string UrlDetail;

        public HttpRequest GetHttpRequestDetail()
        {
            return new HttpRequest { Url = UrlDetail };
        }
    }

    public class BlogDemoorHeaderDataPages : DataPages<BlogDemoorHeaderData>, IKeyData
    {
        public int Id;

        public BsonValue GetKey()
        {
            return Id;
        }
    }

    public class BlogDemoorDetailData : IKeyData, IGetWebImages //, ILoadImages
    {
        public string SourceUrl;
        public DateTime? LoadFromWebDate;
        public int Id;
        public string Title;
        public Date? Date;
        public string Content;
        public WebImage[] Images;

        public BsonValue GetKey()
        {
            return Id;
        }

        //public void LoadImages(WebImageRequest imageRequest)
        //{
        //    WebImageMongoManager.Current.LoadImages(Images, imageRequest);
        //}

        public IEnumerable<WebImage> GetWebImages()
        {
            return Images;
        }
    }
}
