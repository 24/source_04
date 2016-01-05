using System.Text;

namespace pb.IO
{
    public partial class WriteToFile : WriteToFileBase
    {
        public static WriteToFile Create(string file, FileOption option, Encoding encoding = null)
        {
            if (file != null)
            {
                file = file.zRootPath(zapp.GetAppDirectory());
                if (option == FileOption.IndexedFile)
                    return new WriteToFile(() => zfile.GetNewIndexedFileName(zPath.GetDirectoryName(file), zPath.GetFileName(file)), encoding);
                else
                    return new WriteToFile(file, encoding, appendToFile: option != FileOption.RazFile);
            }
            else
                return null;
        }
    }
}
