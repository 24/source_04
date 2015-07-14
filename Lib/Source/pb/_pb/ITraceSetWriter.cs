using System;
using System.Text;
using pb.IO;

namespace pb
{
    public partial interface ITrace
    {
        void SetWriter(string file, FileOption option, Encoding encoding = null);
    }
}
