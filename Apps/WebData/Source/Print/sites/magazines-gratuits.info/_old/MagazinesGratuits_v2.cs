using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MongoDB.Bson;
using pb;
using pb.Data.Xml;
using pb.Web;

namespace Download.Print.MagazinesGratuits.old
{
    public static class MagazinesGratuits_v2
    {
        public static string Name = "magazines-gratuits.info";

        static MagazinesGratuits_v2()
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
                xe = XmlConfig.CurrentConfig.GetElement("MagazinesGratuits");
            else
            {
                Trace.WriteLine("MagazinesGratuits init for test");
                xe = XmlConfig.CurrentConfig.GetElement("MagazinesGratuits_Test");
            }
            MagazinesGratuits_HeaderManager_v2.Init(xe);
            MagazinesGratuits_DetailManager_v2.Init(xe);
            ServerManagers.Add("MagazinesGratuits", CreateServerManager());
        }

        public static HttpRequestParameters GetHttpRequestParameters()
        {
            return new HttpRequestParameters { Encoding = Encoding.UTF8 };
        }

        public static ServerManager CreateServerManager(Action loadNewPost = null, Func<DateTime, IEnumerable<IPostToDownload>> getPostList = null)
        {
            if (loadNewPost == null)
                loadNewPost = () => MagazinesGratuits_DetailManager_v2.WebHeaderDetailManager.LoadNewDocuments(maxNbDocumentsLoadedFromStore: 10, startPage: 1, maxPage: 10);
            if (getPostList == null)
            {
                getPostList =
                    lastRunDateTime =>
                    {
                        string query = string.Format("{{ 'download.PostCreationDate': {{ $gt: ISODate('{0}') }} }}", lastRunDateTime.ToUniversalTime().ToString("o"));
                        string sort = "{ 'download.PostCreationDate': -1 }";
                        // useCursorCache: true
                        return MagazinesGratuits_DetailManager_v2.DetailWebDataManager.Find(query, sort: sort, loadImage: false);
                    };
            }
            Func<BsonValue, IPostToDownload> loadPost = id => MagazinesGratuits_DetailManager_v2.DetailWebDataManager.Find(string.Format("{{ _id: \"{0}\" }}", id)).FirstOrDefault();
            return new ServerManager
            {
                //Name = "magazines-gratuits.info",
                Name = Name,
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
