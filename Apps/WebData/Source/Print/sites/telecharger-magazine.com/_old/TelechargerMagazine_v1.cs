using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using pb.Web;
using Download.Print.old;

namespace Download.Print.TelechargerMagazine.old
{
    public static class TelechargerMagazine_v1
    {
        static TelechargerMagazine_v1()
        {
            ServerManagers_v1.Add("TelechargerMagazine", CreateServerManager());
        }

        public static void FakeInit()
        {
        }

        public static string GetServer()
        {
            return "telecharger-magazine.com";
        }

        public static HttpRequestParameters GetHttpRequestParameters()
        {
            return new HttpRequestParameters { Encoding = Encoding.UTF8 };
        }

        public static ServerManager_v1 CreateServerManager(Action loadNewPost = null, Func<DateTime, IEnumerable<IPostToDownload_v1>> getPostList = null)
        {
            if (loadNewPost == null)
                loadNewPost = () => TelechargerMagazine_DetailManager_v1.WebHeaderDetailManager.LoadNewDocuments(maxNbDocumentsLoadedFromStore: 25, startPage: 1, maxPage: 10);
            if (getPostList == null)
            {
                getPostList =
                    lastRunDateTime =>
                    {
                        //string query = string.Format("{{ 'download.PostCreationDate': {{ $gt: ISODate('{0}') }} }}", lastRunDateTime.ToUniversalTime().ToString("o"));
                        string query = string.Format("{{ 'download.LoadFromWebDate': {{ $gt: ISODate('{0}') }} }}", lastRunDateTime.ToUniversalTime().ToString("o"));
                        //string sort = "{ 'download.PostCreationDate': -1 }";
                        string sort = "{ _id: -1 }";
                        // useCursorCache: true
                        return TelechargerMagazine_DetailManager_v1.DetailWebDataManager.FindDocuments(query, sort: sort, loadImage: false);
                    };
            }
            Func<int, IPostToDownload_v1> loadPost = id => TelechargerMagazine_DetailManager_v1.DetailWebDataManager.FindDocuments(string.Format("{{ _id: {0} }}", id)).FirstOrDefault();

            //GetPostInfoListDelegate getPostInfoList = null;
            //MongoDocumentStore<int, TelechargerMagazine_PostDetail> mongoDocumentStore = TelechargerMagazine_DetailManager.DetailWebDataManager.DocumentStore as MongoDocumentStore<int, TelechargerMagazine_PostDetail>;
            //if (mongoDocumentStore != null)
            //{
            //    MongoCollection mongoCollection = mongoDocumentStore.GetCollection();
            //    getPostInfoList = (query, sort, limit) => DownloadPrint.GetPostInfoList(mongoCollection, GetServer(), query, sort, limit);
            //}

            return new ServerManager_v1
            {
                Name = GetServer(),
                EnableLoadNewPost = false,
                EnableSearchPostToDownload = false,
                DownloadDirectory = null,
                LoadNewPost = loadNewPost,
                GetPostList = getPostList,
                //GetPostInfoList = getPostInfoList,
                LoadPost = loadPost
                //Backup = DownloadPrint.CreateMongoBackup(TelechargerMagazine_DetailManager_v1.DetailMongoDocumentStore.GetCollection()).Backup
            };
        }

    }
}
