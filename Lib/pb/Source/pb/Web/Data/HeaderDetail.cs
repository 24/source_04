namespace pb.Web.Data
{
    // used by :
    //   WebHeaderDetailManager<THeaderData, TDetailData>
    //   WebHeaderDetailManager_v4<THeaderData, TDetailData>
    public class HeaderDetail<THeaderData, TDetailData>
    {
        public THeaderData Header;
        public TDetailData Detail;
    }
}
