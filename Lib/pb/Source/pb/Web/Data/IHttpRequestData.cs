using pb.Web.Http;

namespace pb.Web.Data
{
    public interface IHttpRequestData
    {
        // get HttpRequest from data to refresh data and to get url
        // used in WebDataManager_v2<TData>.Refresh() and WebDataManager<TKey, TData>.RefreshDocumentsStore()
        // used in DownloadAutomateManager_v2.GetPostMessage()
        HttpRequest GetDataHttpRequest();  // used in WebDataManager<TKey, TData>.RefreshDocumentsStore() and IPostToDownload
    }
}
