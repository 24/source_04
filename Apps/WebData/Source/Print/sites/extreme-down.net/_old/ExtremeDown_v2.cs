using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using pb.Web;
using Download.Print.old;

namespace Download.Print.ExtremeDown.old
{
    public static class ExtremeDown_v2
    {
        static ExtremeDown_v2()
        {
            ServerManagers_v1.Add("ExtremeDown", CreateServerManager());
        }

        public static void FakeInit()
        {
        }

        public static HttpRequestParameters GetHttpRequestParameters()
        {
            return new HttpRequestParameters { Encoding = Encoding.UTF8 };
        }

        public static ServerManager_v1 CreateServerManager(Action loadNewPost = null, Func<DateTime, IEnumerable<IPostToDownload_v1>> getPostList = null)
        {
            if (loadNewPost == null)
                loadNewPost = () => ExtremeDown_DetailManager_v2.WebHeaderDetailManager.LoadNewDocuments(maxNbDocumentsLoadedFromStore: 10, startPage: 1, maxPage: 10);
            if (getPostList == null)
            {
                getPostList =
                    lastRunDateTime =>
                    {
                        string query = string.Format("{{ 'download.PostCreationDate': {{ $gt: ISODate('{0}') }} }}", lastRunDateTime.ToUniversalTime().ToString("o"));
                        string sort = "{ 'download.PostCreationDate': -1 }";
                        // useCursorCache: true
                        return ExtremeDown_DetailManager_v2.DetailWebDataManager.FindDocuments(query, sort: sort, loadImage: false);
                    };
            }
            Func<int, IPostToDownload_v1> loadPost = id => ExtremeDown_DetailManager_v2.DetailWebDataManager.FindDocuments(string.Format("{{ _id: {0} }}", id)).FirstOrDefault();
            return new ServerManager_v1
            {
                Name = "extreme-down.net",
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
