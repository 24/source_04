using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using pb.Web;

namespace Download.Print.Vosbooks
{
    public static class Vosbooks
    {
        //private static char[] _trimChars = new char[] { ' ', '\t', '\n', '\r', ',', '»', '&', '+', '/', '|', '*', '=', '»', '.', ':', '_' };
        //private static Func<string, string> __trimFunc1 = text => text.Trim(_trimChars);
        //private static Func<string, string> __replaceChars = text => text.Replace('\u2013', '-');

        //public static Func<string, string> TrimFunc1 { get { return __trimFunc1; } }
        //public static Func<string, string> ReplaceChars { get { return __replaceChars; } }

        static Vosbooks()
        {
            //Trace.WriteLine("Vosbooks.Vosbooks()");
            ServerManagers.Add("Vosbooks", CreateServerManager());
            //ServerManagers.Add("Vosbooks_test", CreateServerManager());
        }

        public static void FakeInit()
        {
        }

        public static HttpRequestParameters GetHttpRequestParameters()
        {
            return new HttpRequestParameters { Encoding = Encoding.UTF8 };
        }

        //public static ServerManager CreateServerManager(bool enableLoadNewPost = true, bool enableSearchPostToDownload = true, string downloadDirectory = null)
        public static ServerManager CreateServerManager(Action loadNewPost = null, Func<DateTime, IEnumerable<IPostToDownload>> getPostList = null)
        {
            if (loadNewPost == null)
                loadNewPost = () => Vosbooks_DetailManager.WebHeaderDetailManager.LoadNewDocuments(maxNbDocumentsLoadedFromStore: 10, startPage: 1, maxPage: 10);
            if (getPostList == null)
            {
                getPostList =
                    lastRunDateTime =>
                    {
                        string query = string.Format("{{ 'download.PostCreationDate': {{ $gt: ISODate('{0}') }} }}", lastRunDateTime.ToUniversalTime().ToString("o"));
                        string sort = "{ 'download.PostCreationDate': -1 }";
                        // useCursorCache: true
                        return Vosbooks_DetailManager.DetailWebDataManager.FindDocuments(query, sort: sort, loadImage: false);
                    };
            }
            Func<int, IPostToDownload> loadPost = id => Vosbooks_DetailManager.DetailWebDataManager.FindDocuments(string.Format("{{ _id: {0} }}", id)).FirstOrDefault();
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
