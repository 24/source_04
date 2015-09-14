using System;

namespace pb.IO
{
    // used in : pb.IO.WriteToFile.Create()
    public enum FileOption
    {
        None = 0,
        IndexedFile = 1,
        RazFile = 2
    }

    // System.IO.FileMode : CreateNew = 1, Create = 2, Open = 3, OpenOrCreate = 4, Truncate = 5, Append = 6
    // not used
    public enum zFileMode
    {
        /// <summary>create a new file (FileMode.CreateNew)</summary>
        CreateNew = 1,
        /// <summary>create a new file or open existing file and truncate it (FileMode.Create)</summary>
        Create,
        /// <summary>open existing file (FileMode.Open)</summary>
        Open,
        /// <summary>create a new file or open existing file (FileMode.OpenOrCreate)</summary>
        OpenOrCreate,
        /// <summary>open existing file and truncate it (FileMode.Truncate)</summary>
        Truncate,
        /// <summary>open existing file and seeks to the end of the file to append data or create a new file (FileMode.Append)</summary>
        Append,
        /// <summary>create new indexed file</summary>
        CreateNewIndexedFile
    }
}
