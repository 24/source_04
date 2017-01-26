using System;

namespace pb.old
{
    public partial interface ITrace
    {
        void WriteError(Exception ex);
    }
}
