namespace pb.Web.Data
{
    // used by :
    //   WebHeaderDetailManager<THeaderData, TDetailData>
    //   WebHeaderDetailManager_v4<THeaderData, TDetailData>
    public interface IHeaderData
    {
        HttpRequest GetHttpRequestDetail();
    }
}
