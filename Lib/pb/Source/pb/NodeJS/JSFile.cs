using pb.IO;
using System.Threading.Tasks;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
//#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

namespace pb.NodeJS
{
    public class JSFile
    {
        public async Task<object> GetFileNumber(object file)
        {
            return FileNumber.GetFileNumber((string)file);
        }
    }
}
