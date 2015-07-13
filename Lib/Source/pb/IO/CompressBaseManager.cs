using System;

namespace pb.IO
{
    [Flags]
    public enum UncompressBaseOptions
    {
        None                    = 0x0000,
        ExtractFullPath         = 0x0001,
        OverrideExistingFile    = 0x0002,
        RenameExistingFile      = 0x0004
    }

    public abstract class CompressBaseManager
    {
        public abstract string[] Uncompress(string file, string directory, UncompressBaseOptions options = UncompressBaseOptions.None);
    }
}
