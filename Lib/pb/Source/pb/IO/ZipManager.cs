using System.Collections.Generic;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace pb.IO
{
    public class ZipManager : CompressBaseManager
    {
        // use ZipArchive from System.IO.Compression
        public override string[] Uncompress(string file, string directory, UncompressBaseOptions options = UncompressBaseOptions.None)
        {
            bool extractFullPath = (options & UncompressBaseOptions.ExtractFullPath) == UncompressBaseOptions.ExtractFullPath;
            bool overrideExistingFile = (options & UncompressBaseOptions.OverrideExistingFile) == UncompressBaseOptions.OverrideExistingFile;
            bool renameExistingFile = (options & UncompressBaseOptions.RenameExistingFile) == UncompressBaseOptions.RenameExistingFile;
            List<string> files = new List<string>();

            using (System.IO.Compression.ZipArchive archive = ZipFile.OpenRead(file))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    string path = zPath.Combine(directory, entry.FullName);
                    zfile.CreateFileDirectory(path);
                    entry.ExtractToFile(path);
                    files.Add(path);
                }
            } 

            return files.ToArray();
        }

        private static Regex __zipFilePartName = new Regex(@"(\.part[0-9]+)\.rar$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        // used by DownloadManager<TKey>.DebridLink()  (DownloadManager.cs)
        public static string GetZipFilePartName(string filename)
        {
            // ex : file.part01.rar return ".part01"
            Match match = __zipFilePartName.Match(filename);
            if (match.Success)
                return match.Groups[1].Value;
            else
                return null;
        }
    }
}
