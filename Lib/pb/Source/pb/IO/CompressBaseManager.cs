using System.Collections.Generic;
using System.IO;

namespace pb.IO
{
    public abstract class CompressBaseManager
    {
        public abstract void Compress(string compressFile, IEnumerable<CompressFile> files, FileMode fileMode = FileMode.Create);
        public abstract IEnumerable<string> Uncompress(string compressFile, string directory, IEnumerable<string> selectedFiles = null, UncompressOptions uncompressOptions = UncompressOptions.None);
        public abstract IEnumerable<string> Uncompress(string compressFile, string directory, IEnumerable<CompressFile> selectedFiles, UncompressOptions uncompressOptions = UncompressOptions.None);
        public abstract bool IsCompressFile(string file);
    }
}
