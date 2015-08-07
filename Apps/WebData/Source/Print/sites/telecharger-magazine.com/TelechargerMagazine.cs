using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using pb.Data.Mongo;
using pb.Web;

namespace Download.Print.TelechargerMagazine
{
    public static class TelechargerMagazine
    {
        static TelechargerMagazine()
        {
            ServerManagers.Add("TelechargerMagazine", CreateServerManager());
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

        public static ServerManager CreateServerManager(Action loadNewPost = null, Func<DateTime, IEnumerable<IPostToDownload>> getPostList = null)
        {
            if (loadNewPost == null)
                loadNewPost = () => TelechargerMagazine_DetailManager.WebHeaderDetailManager.LoadNewDocuments(maxNbDocumentsLoadedFromStore: 25, startPage: 1, maxPage: 10);
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
                        return TelechargerMagazine_DetailManager.DetailWebDataManager.FindDocuments(query, sort: sort, loadImage: false);
                    };
            }
            Func<int, IPostToDownload> loadPost = id => TelechargerMagazine_DetailManager.DetailWebDataManager.FindDocuments(string.Format("{{ _id: {0} }}", id)).FirstOrDefault();

            //GetPostInfoListDelegate getPostInfoList = null;
            //MongoDocumentStore<int, TelechargerMagazine_PostDetail> mongoDocumentStore = TelechargerMagazine_DetailManager.DetailWebDataManager.DocumentStore as MongoDocumentStore<int, TelechargerMagazine_PostDetail>;
            //if (mongoDocumentStore != null)
            //{
            //    MongoCollection mongoCollection = mongoDocumentStore.GetCollection();
            //    getPostInfoList = (query, sort, limit) => DownloadPrint.GetPostInfoList(mongoCollection, GetServer(), query, sort, limit);
            //}

            return new ServerManager
            {
                Name = GetServer(),
                EnableLoadNewPost = false,
                EnableSearchPostToDownload = false,
                DownloadDirectory = null,
                LoadNewPost = loadNewPost,
                GetPostList = getPostList,
                //GetPostInfoList = getPostInfoList,
                LoadPost = loadPost
            };
        }

    }
}
