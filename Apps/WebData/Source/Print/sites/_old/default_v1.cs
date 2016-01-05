using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MongoDB.Bson;
using pb.Data;
using pb.Data.Xml;
using pb.Web;
using pb.Web.Data;
using Download.Print.old;

namespace Download.Print.Test
{
    public class Test_PostDetail_v1 : PostDetailSimpleLinks
    {
        public Test_PostDetail_v1()
        {
            Infos = new NamedValues<ZValue>(useLowercaseKey: true);
        }

        public string OriginalTitle;
        public string PostAuthor;
        public DateTime? PostCreationDate;
        public string Category;
        public string[] Description;
        public NamedValues<ZValue> Infos;

        // IPostToDownload
        public override string GetServer()
        {
            return Test_v1.ServerName;
        }
    }

    public class Test_v1 : PostHeaderDetailMongoManagerBase_v1<Test_PostDetail_v1>
    {
        private static bool __trace = false;
        private static string __serverName = "test.com";
        private static string __configName = "Test";
        private static Test_v1 __current = null;
        //private static string __urlMainPage = "http://test.com/";

        static Test_v1()
        {
            Init(test: DownloadPrint.Test);
        }

        public static void FakeInit()
        {
        }

        public static void Init(bool test = false)
        {
            XElement xe;
            if (!test)
                xe = XmlConfig.CurrentConfig.GetElement(__configName);
            else
            {
                pb.Trace.WriteLine("{0} init for test", __configName);
                xe = XmlConfig.CurrentConfig.GetElement(__configName + "_Test");
            }
            __current = new Test_v1();
            __current.CreateManagers(xe);
            ServerManagers.Add(__serverName, __current.CreateServerManager(__serverName));
        }

        public static bool Trace { get { return __trace; } set { __trace = value; } }
        public static string ServerName { get { return __serverName; } }
        public static Test_v1 Current { get { return __current; } }

        // header get data
        protected override IEnumDataPages<IHeaderData> GetHeaderPageData(WebResult webResult)
        {
            return null;
        }

        private static int GetPageKey(HttpRequest httpRequest)
        {
            return 0;
        }

        // header get url page
        protected override HttpRequest GetHttpRequestPage(int page)
        {
            return null;
        }

        // used by detail cache
        protected override string GetDetailCacheUrlSubDirectory(HttpRequest httpRequest)
        {
            return null;
        }

        // detail get data
        protected override Test_PostDetail_v1 GetDetailData(WebResult webResult)
        {
            return null;
        }

        protected override BsonValue GetDetailKey(HttpRequest httpRequest)
        {
            return _GetDetailKey(httpRequest);
        }

        private static int _GetDetailKey(HttpRequest httpRequest)
        {
            return 0;
        }

        protected override void LoadDetailImages(Test_PostDetail_v1 data)
        {
            data.LoadImages();
        }

        protected override void LoadNewDocuments()
        {
            //_webHeaderDetailManager.LoadNewDocuments(maxNbDocumentsLoadedFromStore: 10, startPage: 1, maxPage: 10);
        }

        protected override IEnumerable<IPostToDownload> Find(DateTime date)
        {
            string query = string.Format("{{ 'download.PostCreationDate': {{ $gt: ISODate('{0}') }} }}", date.ToUniversalTime().ToString("o"));
            string sort = "{ 'download.PostCreationDate': -1 }";
            // useCursorCache: true
            return _detailWebDataManager.Find(query, sort: sort, loadImage: false);
        }

        protected override IPostToDownload LoadDocument(BsonValue id)
        {
            return _detailWebDataManager.DocumentStore.LoadFromId(id);
        }
    }
}
