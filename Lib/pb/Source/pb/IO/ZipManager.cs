using System.Collections.Generic;
using System.IO;

namespace pb.IO
{
    public class ZipManager : CompressBaseManager
    {
        // use ZipArchive from System.IO.Compression
        //public override string[] Uncompress(string file, string directory, IEnumerable<string> selectedFiles = null, UncompressBaseOptions options = UncompressBaseOptions.None)
        //{
        //    bool extractFullPath = (options & UncompressBaseOptions.ExtractFullPath) == UncompressBaseOptions.ExtractFullPath;
        //    bool overrideExistingFile = (options & UncompressBaseOptions.OverrideExistingFile) == UncompressBaseOptions.OverrideExistingFile;
        //    bool renameExistingFile = (options & UncompressBaseOptions.RenameExistingFile) == UncompressBaseOptions.RenameExistingFile;
        //    List<string> files = new List<string>();

        //    Dictionary<string, string> dicCompressedFiles = null;
        //    if (selectedFiles != null)
        //    {
        //        dicCompressedFiles = new Dictionary<string, string>();
        //        foreach (string compressedFile in selectedFiles)
        //            dicCompressedFiles.Add(compressedFile, compressedFile);
        //    }

        //    using (System.IO.Compression.ZipArchive archive = ZipFile.OpenRead(file))
        //    {
        //        foreach (ZipArchiveEntry entry in archive.Entries)
        //        {
        //            if (dicCompressedFiles != null && !dicCompressedFiles.ContainsKey(entry.FullName))
        //                continue;

        //            string path = zPath.Combine(directory, entry.FullName);
        //            zfile.CreateFileDirectory(path);
        //            entry.ExtractToFile(path);
        //            files.Add(path);
        //        }
        //    } 

        //    return files.ToArray();
        //}

        public override void Compress(string compressFile, IEnumerable<CompressFile> files, FileMode fileMode = FileMode.Create)
        {
            ZipArchive.Zip(compressFile, files, fileMode);
        }

        public override IEnumerable<string> Uncompress(string compressFile, string directory, IEnumerable<string> selectedFiles = null, UncompressOptions uncompressOptions = UncompressOptions.None)
        {
            return ZipArchive.Unzip(compressFile, directory, selectedFiles, uncompressOptions);
        }

        public override IEnumerable<string> Uncompress(string compressFile, string directory, IEnumerable<CompressFile> selectedFiles, UncompressOptions uncompressOptions = UncompressOptions.None)
        {
            return ZipArchive.Unzip(compressFile, directory, selectedFiles, uncompressOptions);
        }

        public override bool IsCompressFile(string file)
        {
            return ZipArchive.IsCompressFile(file);
        }
    }
}
