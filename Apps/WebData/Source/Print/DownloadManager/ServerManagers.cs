using System.Collections.Generic;
using pb;
using System;
using MongoDB.Bson;
using pb.Web.Data;

namespace Download.Print
{
    public interface IServerManager
    {
        string Name { get; }
        bool EnableLoadNewDocument { get; set; }
        bool EnableSearchDocumentToDownload { get; set; }
        string DownloadDirectory { get; set; }
        void LoadNewDocuments();
        // bool loadImage = true
        LoadNewDocumentsResult LoadNewDocuments(int maxNbDocumentsLoadedFromStore = 5, int startPage = 1, int maxPage = 20, WebImageRequest webImageRequest = null);
        IEnumerable<IPostToDownload> FindFromDateTime(DateTime dateTime);
        IEnumerable<IPostToDownload> Find(string query = null, string sort = null, int limit = 0, bool loadImage = false);
        IPostToDownload Load(BsonValue id);
        IEnumerable<string> Backup(string directory);
    }

    //public class ServerManager
    //{
    //    public string Name;
    //    public bool EnableLoadNewPost;
    //    public bool EnableSearchPostToDownload;
    //    public string DownloadDirectory;
    //    public Action LoadNewPost = null;
    //    public Func<DateTime, IEnumerable<IPostToDownload>> GetPostList = null;
    //    public Func<string, string, int, IEnumerable<IPostToDownload>> FindPost = null;
    //    public Func<BsonValue, IPostToDownload> LoadPost = null;
    //    public Func<string, IEnumerable<string>> Backup = null;
    //}

    public static class ServerManagers_v2
    {
        private static Dictionary<string, IServerManager> __servers = new Dictionary<string, IServerManager>();

        public static void Add(IServerManager serverManager)
        {
            if (__servers.ContainsKey(serverManager.Name))
                throw new PBException("ServerManager \"{0}\" already in ServerManagers list", serverManager.Name);
            __servers.Add(serverManager.Name, serverManager);
        }

        public static IServerManager Get(string name)
        {
            if (!__servers.ContainsKey(name))
                throw new PBException("unknow ServerManager \"{0}\" in ServerManagers list", name);
            return __servers[name];
        }
    }

    //public static class ServerManagers
    //{
    //    private static Dictionary<string, ServerManager> __servers = new Dictionary<string, ServerManager>();

    //    public static void Add(string name, ServerManager serverManager)
    //    {
    //        if (__servers.ContainsKey(name))
    //            throw new PBException("ServerManager \"{0}\" already in ServerManagers list", name);
    //        __servers.Add(name, serverManager);
    //    }

    //    public static ServerManager Get(string name)
    //    {
    //        if (!__servers.ContainsKey(name))
    //            throw new PBException("unknow ServerManager \"{0}\" in ServerManagers list", name);
    //        return __servers[name];
    //    }
    //}
}
