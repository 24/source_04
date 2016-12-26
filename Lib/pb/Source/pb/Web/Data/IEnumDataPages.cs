using System.Collections.Generic;

namespace pb.Web.Data
{
    // used by :
    //   - DataPages<TData>
    //   - WebDataPageManager<TData>
    //   - WebDataPageManager_v4<TData>
    public interface IEnumDataPages<TData>
    {
        IEnumerable<TData> GetDataList();
        HttpRequest GetHttpRequestNextPage();
    }
}
