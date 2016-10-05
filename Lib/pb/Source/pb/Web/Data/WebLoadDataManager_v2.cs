using System;

namespace pb.Web.Data
{
    public class WebDataResult_v2<TData>
    {
        public bool Success;
        public WebRequest Request;
        public HttpResult<string> Result;
        public TData Data;
    }

    // public class WebLoadDataManager<TData> : WebLoadManager
    public class WebLoadDataManager_v2<TData> : HttpManager_v2
    {
        protected Func<HttpResult<string>, TData> _getData = null;

        public Func<HttpResult<string>, TData> GetData { get { return _getData; } set { _getData = value; } }

        public WebDataResult_v2<TData> LoadData(WebRequest webRequest)
        {
            //WebResult loadDataFromWeb = Load(webRequest);
            HttpResult<string> httpResult = LoadText(webRequest.HttpRequest);
            TData data;
            if (httpResult.Success)
            {
                data = _getData(httpResult);
            }
            else
                data = default(TData);
            return new WebDataResult_v2<TData> { Success = httpResult.Success, Request = webRequest, Result = httpResult, Data = data };
        }
    }
}
