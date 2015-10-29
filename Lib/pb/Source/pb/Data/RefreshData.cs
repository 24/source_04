using System;

namespace pb.Source.pb.Data
{
    public interface IRefreshable
    {
        bool Refresh();
    }

    // Error 123 Cannot derive from 'T' because it is a type parameter
    //public class RefreshData<T> : T where T : IRefreshable
    //{
    //}
}
