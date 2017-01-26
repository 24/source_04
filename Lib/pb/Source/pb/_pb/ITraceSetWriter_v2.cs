using System.Text;
using pb.IO;

namespace pb.old
{
    public partial interface ITrace
    {
        void SetWriter(string file, FileOption option, Encoding encoding = null);
    }
}
