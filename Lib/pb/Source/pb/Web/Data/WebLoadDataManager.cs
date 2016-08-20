using System;

namespace pb.Web.Data
{
    public class WebDataResult<TData>
    {
        public WebRequest Request;
        public WebResult Result;
        public TData Data;
    }

    public class WebLoadDataManager<TData> : WebLoadManager
    {
        protected Func<WebResult, TData> _getData = null;

        public Func<WebResult, TData> GetData { get { return _getData; } set { _getData = value; } }

        //public TData LoadData(WebRequest webRequest)
        public WebDataResult<TData> LoadData(WebRequest webRequest)
        {
            WebResult loadDataFromWeb = Load(webRequest);
            TData data;
            if (loadDataFromWeb.LoadResult)
            {
                //return _getData(loadDataFromWeb);
                data = _getData(loadDataFromWeb);
                //if (webRequest.LoadImageFromWeb || webRequest.LoadImageToData || webRequest.RefreshImage)
                //{
                //    if (!(data is ILoadImages))
                //        throw new PBException($"{typeof(TData).zGetTypeName()} is not ILoadImages");
                //    ((ILoadImages)data).LoadImages(WebImageRequest.FromWebRequest(webRequest));
                //}
                //return data;
            }
            else
                //return default(TData);
                data = default(TData);
            return new WebDataResult<TData> { Request = webRequest, Result = loadDataFromWeb, Data = data };
        }
    }
}
