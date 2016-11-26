using System.Collections.Generic;
using System.IO;
using SharpCompress.Common;
using SharpCompress.Reader;

namespace pb.IO
{
    // use ReaderFactory from SharpCompress.Reader
    public class SharpCompressManager : CompressBaseManager
    {
        //private CompressionInfo _compressionInfo = new CompressionInfo();

        //public CompressionInfo CompressionInfo { get { return _compressionInfo; } }

        //public void Test_ZipCompress_01(string compressFile, IEnumerable<string> files, string baseDirectory = null, ArchiveType archiveType = ArchiveType.Zip,
        //    CompressionType compressionType = CompressionType.BZip2, CompressionLevel compressionLevel = CompressionLevel.Default)
        //{
        //    using (var archive = ZipArchive.Create())
        //    {
        //        archive.AddEntry();
        //        archive.AddAllFromDirectoryEntry(@"C:\\source");
        //        archive.SaveTo("@C:\\new.zip");
        //    }
        //}

        public override void Compress(string compressFile, IEnumerable<CompressFile> files, FileMode fileMode = FileMode.Create)
        {
            throw new PBException("not implemented");
        }

        public override IEnumerable<string> Uncompress(string compressFile, string directory, IEnumerable<string> selectedFiles = null, UncompressOptions uncompressOptions = UncompressOptions.None)
        {
            bool extractFullPath = (uncompressOptions & UncompressOptions.ExtractFullPath) == UncompressOptions.ExtractFullPath;
            bool overrideExistingFile = (uncompressOptions & UncompressOptions.OverrideExistingFile) == UncompressOptions.OverrideExistingFile;
            bool renameExistingFile = (uncompressOptions & UncompressOptions.RenameExistingFile) == UncompressOptions.RenameExistingFile;

            List<string> files = new List<string>();

            Dictionary<string, string> dicCompressedFiles = null;
            if (selectedFiles != null)
            {
                dicCompressedFiles = new Dictionary<string, string>();
                foreach (string compressedFile in selectedFiles)
                    dicCompressedFiles.Add(compressedFile, compressedFile);
            }

            using (Stream stream = zFile.OpenRead(compressFile))
            {
                var reader = ReaderFactory.Open(stream);
                while (reader.MoveToNextEntry())
                {
                    if (!reader.Entry.IsDirectory)
                    {
                        string compressedFile = reader.Entry.FilePath;

                        if (dicCompressedFiles != null && !dicCompressedFiles.ContainsKey(compressedFile))
                            continue;

                        string uncompressFile;
                        if (extractFullPath)
                            uncompressFile = compressedFile;
                        else
                            uncompressFile = zPath.GetFileName(compressedFile);
                        uncompressFile = zPath.Combine(directory, uncompressFile);

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
            return files;
        }

        public override IEnumerable<string> Uncompress(string compressFile, string directory, IEnumerable<CompressFile> selectedFiles, UncompressOptions uncompressOptions = UncompressOptions.None)
        {
            throw new PBException("not implemented");
        }

        public override bool IsCompressFile(string file)
        {
            switch (zPath.GetExtension(file).ToLower())
            {
                case ".zip":
                case ".rar":
                case ".tar":
                case ".gz":
                case ".7z":
                    return true;
                default:
                    return false;
            }
        }

        // before use of GetCompressFiles()
        //public override string[] Uncompress(string file, string directory, UncompressBaseOptions options = UncompressBaseOptions.None)
        //{
        //    bool extractFullPath = (options & UncompressBaseOptions.ExtractFullPath) == UncompressBaseOptions.ExtractFullPath;
        //    bool overrideExistingFile = (options & UncompressBaseOptions.OverrideExistingFile) == UncompressBaseOptions.OverrideExistingFile;
        //    bool renameExistingFile = (options & UncompressBaseOptions.RenameExistingFile) == UncompressBaseOptions.RenameExistingFile;
        //    List<string> files = new List<string>();
        //    using (Stream stream = zFile.OpenRead(file))
        //    {
        //        var reader = ReaderFactory.Open(stream);
        //        while (reader.MoveToNextEntry())
        //        {
        //            //Trace.WriteLine("  \"{0}\"", reader.Entry.FilePath);
        //            if (!reader.Entry.IsDirectory)
        //            {
        //                //reader.WriteEntryToDirectory(directory, options);
        //                string uncompressFile;
        //                if (extractFullPath)
        //                    uncompressFile = reader.Entry.FilePath;
        //                else
        //                    zPath.GetFileName(reader.Entry.FilePath);
        //                uncompressFile = zPath.Combine(directory, reader.Entry.FilePath);
        //                if (zFile.Exists(uncompressFile))
        //                {
        //                    if (overrideExistingFile)
        //                        zFile.Delete(uncompressFile);
        //                    else if (renameExistingFile)
        //                        uncompressFile = zfile.GetNewFilename(uncompressFile);
        //                    else
        //                        throw new PBException("error file already exist can't uncompress \"{0}\"", uncompressFile);
        //                }
        //                zfile.CreateFileDirectory(uncompressFile);
        //                reader.WriteEntryToFile(uncompressFile, ExtractOptions.None);
        //                files.Add(uncompressFile);
        //            }
        //        }
        //    }
        //    return files.ToArray();
        //}

        //private static IEnumerable<string> GetCompressFiles(IReader reader)
        //{
        //    while (reader.MoveToNextEntry())
        //    {
        //        if (!reader.Entry.IsDirectory)
        //        {
        //            yield return reader.Entry.FilePath;
        //        }
        //    }
        //}
    }
}
