using System;

namespace pb.IO
{
    [Flags]
    public enum CompressOptions
    {
        None                    = 0x0000,
        StorePath               = 0x0001,
        StorePartialPath        = 0x0002,
        DeleteSourceFiles       = 0x0004
    }

    [Flags]
    //public enum UncompressBaseOptions
    public enum UncompressOptions
    {
        None                    = 0x0000,
        ExtractFullPath         = 0x0001,
        OverrideExistingFile    = 0x0002,
        RenameExistingFile      = 0x0004
    }
}
