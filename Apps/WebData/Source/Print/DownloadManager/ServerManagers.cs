using System.Collections.Generic;
using pb;
using System;
using MongoDB.Bson;

namespace Download.Print
{
    public class ServerManager
    {
        public string Name;
        public bool EnableLoadNewPost;
        public bool EnableSearchPostToDownload;
        public string DownloadDirectory;
        public Action LoadNewPost = null;
        public Func<DateTime, IEnumerable<IPostToDownload>> GetPostList = null;
        public Func<BsonValue, IPostToDownload> LoadPost = null;
        public Func<string, IEnumerable<string>> Backup = null;
    }

    public static class ServerManagers
    {
        private static Dictionary<string, ServerManager> __servers = new Dictionary<string, ServerManager>();

        public static void Add(string name, ServerManager serverManager)
        {
            if (__servers.ContainsKey(name))
                throw new PBException("ServerManager \"{0}\" already in ServerManagers list", name);
            __servers.Add(name, serverManager);
        }

        public static ServerManager Get(string name)
        {
            if (!__servers.ContainsKey(name))
                throw new PBException("unknow ServerManager \"{0}\" in ServerManagers list", name);
            return __servers[name];
        }
    }
}
