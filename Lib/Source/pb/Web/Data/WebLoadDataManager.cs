using System;

namespace pb.Web.Data
{
    public class WebLoadDataManager<TData> : WebLoadManager
    {
        protected Func<WebResult, TData> _getData = null;

        public Func<WebResult, TData> GetData { get { return _getData; } set { _getData = value; } }

        public TData LoadData(WebRequest webRequest)
        {
            WebResult loadDataFromWeb = Load(webRequest);
            if (loadDataFromWeb.LoadResult)
                return _getData(loadDataFromWeb);
            else
                return default(TData);
        }
    }
}
