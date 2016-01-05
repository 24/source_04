using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using pb.Web;

namespace Download.Print.MagazinesGratuits.old
{
    public static class MagazinesGratuits_v1
    {
        //private static char[] _trimChars = new char[] { ' ', '\t', '\n', '\r', ',', '»', '&', '+', '/', '|', '*', '=', '»', '.', ':', '_' };
        //private static Func<string, string> __trimFunc1 = text => text.Trim(_trimChars);
        //private static Func<string, string> __replaceChars = text => text.Replace('\u2013', '-');

        //public static Func<string, string> TrimFunc1 { get { return __trimFunc1; } }
        //public static Func<string, string> ReplaceChars { get { return __replaceChars; } }

        static MagazinesGratuits_v1()
        {
            ServerManagers.Add("MagazinesGratuits", CreateServerManager());
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
                loadNewPost = () => MagazinesGratuits_DetailManager.WebHeaderDetailManager.LoadNewDocuments(maxNbDocumentsLoadedFromStore: 10, startPage: 1, maxPage: 10);
            if (getPostList == null)
            {
                getPostList =
                    lastRunDateTime =>
                    {
                        string query = string.Format("{{ 'download.PostCreationDate': {{ $gt: ISODate('{0}') }} }}", lastRunDateTime.ToUniversalTime().ToString("o"));
                        string sort = "{ 'download.PostCreationDate': -1 }";
                        // useCursorCache: true
                        return MagazinesGratuits_DetailManager.DetailWebDataManager.FindDocuments(query, sort: sort, loadImage: false);
                    };
            }
            Func<int, IPostToDownload_v1> loadPost = id => MagazinesGratuits_DetailManager.DetailWebDataManager.FindDocuments(string.Format("{{ _id: {0} }}", id)).FirstOrDefault();
            return new ServerManager_v1
            {
                Name = "magazines-gratuits.org",
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
