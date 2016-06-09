using System.Text;
using pb.IO;

namespace pb
{
    public partial class TTrace : ITrace
    {
        public void SetWriter(string file, FileOption option, Encoding encoding = null)
        {
            SetWriter(WriteToFile.Create(file, option, encoding));
        }
    }
}
