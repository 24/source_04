using System;
using System.Collections.Generic;
using System.IO;
using SharpCompress.Common;
using SharpCompress.Reader;

namespace pb.IO
{
    // use ReaderFactory from SharpCompress.Reader
    public class SharpCompressManager : CompressBaseManager
    {
        public override string[] Uncompress(string file, string directory, UncompressBaseOptions options = UncompressBaseOptions.None)
        {
            bool extractFullPath = (options & UncompressBaseOptions.ExtractFullPath) == UncompressBaseOptions.ExtractFullPath;
            bool overrideExistingFile = (options & UncompressBaseOptions.OverrideExistingFile) == UncompressBaseOptions.OverrideExistingFile;
            bool renameExistingFile = (options & UncompressBaseOptions.RenameExistingFile) == UncompressBaseOptions.RenameExistingFile;
            List<string> files = new List<string>();
            using (Stream stream = File.OpenRead(file))
            {
                var reader = ReaderFactory.Open(stream);
                while (reader.MoveToNextEntry())
                {
                    //Trace.WriteLine("  \"{0}\"", reader.Entry.FilePath);
                    if (!reader.Entry.IsDirectory)
                    {
                        //reader.WriteEntryToDirectory(directory, options);
                        string uncompressFile;
                        if (extractFullPath)
                            uncompressFile = reader.Entry.FilePath;
                        else
                            Path.GetFileName(reader.Entry.FilePath);
                        uncompressFile = Path.Combine(directory, reader.Entry.FilePath);
                        if (File.Exists(uncompressFile))
                        {
                            if (overrideExistingFile)
                                File.Delete(uncompressFile);
                            else if (renameExistingFile)
                                uncompressFile = zfile.GetNewFilename(uncompressFile);
                            else
                                throw new PBException("error file already exist can't uncompress \"{0}\"", uncompressFile);
                        }
                        zfile.CreateFileDirectory(uncompressFile);
                        reader.WriteEntryToFile(uncompressFile, ExtractOptions.None);
                        files.Add(uncompressFile);
                    }
                }
            }
            return files.ToArray();
        }
    }
}
