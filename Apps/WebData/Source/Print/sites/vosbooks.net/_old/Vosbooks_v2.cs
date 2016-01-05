using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MongoDB.Bson;
using pb;
using pb.Data.Xml;
using pb.Web;

namespace Download.Print.Vosbooks.old
{
    public static class Vosbooks_v2
    {
        static Vosbooks_v2()
        {
            //Trace.WriteLine("Vosbooks_v2 static constructor");
            Init(test: DownloadPrint.Test);
        }

        public static void FakeInit()
        {
        }

        public static void Init(bool test = false)
        {
            XElement xe;
            if (!test)
                xe = XmlConfig.CurrentConfig.GetElement("Vosbooks");
            else
            {
                Trace.WriteLine("Vosbooks init for test");
                xe = XmlConfig.CurrentConfig.GetElement("Vosbooks_Test");
            }
            Vosbooks_HeaderManager_v2.Init(xe);
            Vosbooks_DetailManager_v2.Init(xe);
            ServerManagers.Add("Vosbooks", CreateServerManager());
        }

        public static HttpRequestParameters GetHttpRequestParameters()
        {
            return new HttpRequestParameters { Encoding = Encoding.UTF8 };
        }

        public static ServerManager CreateServerManager(Action loadNewPost = null, Func<DateTime, IEnumerable<IPostToDownload>> getPostList = null)
        {
            if (loadNewPost == null)
                loadNewPost = () => Vosbooks_DetailManager_v2.WebHeaderDetailManager.LoadNewDocuments(maxNbDocumentsLoadedFromStore: 10, startPage: 1, maxPage: 10);
            if (getPostList == null)
            {
                getPostList =
                    lastRunDateTime =>
                    {
                        string query = string.Format("{{ 'download.PostCreationDate': {{ $gt: ISODate('{0}') }} }}", lastRunDateTime.ToUniversalTime().ToString("o"));
                        string sort = "{ 'download.PostCreationDate': -1 }";
                        // useCursorCache: true
                        return Vosbooks_DetailManager_v2.DetailWebDataManager.Find(query, sort: sort, loadImage: false);
                    };
            }
            Func<BsonValue, IPostToDownload> loadPost = id => Vosbooks_DetailManager_v2.DetailWebDataManager.Find(string.Format("{{ _id: {0} }}", id)).FirstOrDefault();
            return new ServerManager
            {
                Name = "vosbooks.net",
                EnableLoadNewPost = false,
                EnableSearchPostToDownload = false,
                DownloadDirectory = null,
                LoadNewPost = loadNewPost,
                GetPostList = getPostList,
                LoadPost = loadPost
            };
        }
    }
}
