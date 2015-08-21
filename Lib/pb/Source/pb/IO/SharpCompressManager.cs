using System;
using System.Collections.Generic;
using System.IO;
using SharpCompress.Common;
using SharpCompress.Compressor.Deflate;
using SharpCompress.Reader;
using SharpCompress.Writer;

namespace pb.IO
{
    // use ReaderFactory from SharpCompress.Reader
    public class SharpCompressManager : CompressBaseManager
    {
        //private CompressionInfo _compressionInfo = new CompressionInfo();

        //public CompressionInfo CompressionInfo { get { return _compressionInfo; } }

        public void Compress(string compressFile, IEnumerable<string> files, ArchiveType archiveType = ArchiveType.Zip, string baseDirectory = null, CompressionLevel compressionLevel = CompressionLevel.Default)
        {
            if (baseDirectory != null && !baseDirectory.EndsWith("\\"))
                baseDirectory = baseDirectory + "\\";
            CompressionInfo compressionInfo = new CompressionInfo();
            compressionInfo.DeflateCompressionLevel = compressionLevel;
            using (FileStream stream = File.OpenWrite(compressFile))
            using (IWriter writer = WriterFactory.Open(stream, archiveType, compressionInfo))
            {
                foreach (string file in files)
                {
                    string entryPath;
                    if (baseDirectory != null && file.StartsWith(baseDirectory))
                        entryPath = file.Substring(baseDirectory.Length);
                    else
                        entryPath = zPath.GetFileName(file);
                    writer.Write(entryPath, file);
                }
            }
        }

        public override string[] Uncompress(string file, string directory, UncompressBaseOptions options = UncompressBaseOptions.None)
        {
            bool extractFullPath = (options & UncompressBaseOptions.ExtractFullPath) == UncompressBaseOptions.ExtractFullPath;
            bool overrideExistingFile = (options & UncompressBaseOptions.OverrideExistingFile) == UncompressBaseOptions.OverrideExistingFile;
            bool renameExistingFile = (options & UncompressBaseOptions.RenameExistingFile) == UncompressBaseOptions.RenameExistingFile;
            List<string> files = new List<string>();
            using (Stream stream = zFile.OpenRead(file))
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
                            zPath.GetFileName(reader.Entry.FilePath);
                        uncompressFile = zPath.Combine(directory, reader.Entry.FilePath);
                        if (zFile.Exists(uncompressFile))
                        {
                            if (overrideExistingFile)
                                zFile.Delete(uncompressFile);
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
