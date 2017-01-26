using pb.Web.Http;

namespace pb.Web.Data
{
    partial class WebData<TData>
    {
        private HttpResult<string> _result_v2;

        public HttpResult<string> Result_v2 { get { return _result_v2; } }
    }
}
