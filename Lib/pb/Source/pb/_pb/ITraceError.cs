using System;

namespace pb
{
    public partial interface ITrace
    {
        void WriteError(Exception ex);
    }
}
