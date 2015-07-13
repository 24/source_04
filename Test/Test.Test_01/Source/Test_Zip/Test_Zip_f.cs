using System;
using System.IO;
using System.IO.Compression;
using pb;
using pb.Data.Mongo;
using pb.IO;
using SharpCompress.Common;
using SharpCompress.Reader;
using SharpCompress.Reader.Zip;
//using SharpCompress.Archive.Zip;

namespace Test.Test_Zip
{
    public static class Test_Zip_f
    {
        public static void Test_Uncompress_01(string file, bool overwrite = false)
        {
            string dir = zpath.PathSetExtension(file, "");
            if (Directory.Exists(dir))
            {
                Trace.WriteLine("error directory already exist \"{0}\"", dir);
                return;
            }
            zdir.CreateDirectory(dir);
            Trace.WriteLine("uncompress file \"{0}\" in \"{1}\"", file, dir);
            ExtractOptions options = ExtractOptions.ExtractFullPath;
            if (overwrite)
                options |= ExtractOptions.Overwrite;
            using (Stream stream = File.OpenRead(file))
            {
                var reader = ReaderFactory.Open(stream);
                while (reader.MoveToNextEntry())
                {
                    Trace.WriteLine("  \"{0}\"", reader.Entry.FilePath);
                    if (!reader.Entry.IsDirectory)
                    {
                        reader.WriteEntryToDirectory(dir, options);
                    }
                }
            }
        }

        public static void Test_Uncompress_02(string file)
        {
            string directory = zpath.PathSetExtension(file, "");
            if (Directory.Exists(directory))
            {
                Trace.WriteLine("error directory already exist \"{0}\"", directory);
                return;
            }
            zdir.CreateDirectory(directory);
            Trace.WriteLine("uncompress file \"{0}\" in \"{1}\"", file, directory);
            using (ZipArchive archive = ZipFile.OpenRead(file))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    string path = Path.Combine(directory, entry.FullName);
                    zfile.CreateFileDirectory(path);
                    entry.ExtractToFile(path);
                }
            } 
        }

        public static void Test_ViewCompressFile_01(string file)
        {
            //System.IO.Compression.ZipFile
            Trace.WriteLine("view compress file \"{0}\"", file);
            using (Stream stream = File.OpenRead(file))
            {
                var reader = ReaderFactory.Open(stream);
                Trace.WriteLine("reader  {0}", reader);
                Trace.WriteLine("  ArchiveType  : {0}", reader.ArchiveType);
                ZipReader zipReader = reader as ZipReader;
                if (zipReader != null)
                {
                    Trace.WriteLine("  ArchiveType  : {0}", zipReader.ArchiveType);
                }
                while (reader.MoveToNextEntry())
                {
                    Trace.WriteLine("  FilePath                 : \"{0}\"", reader.Entry.FilePath);
                    Trace.WriteLine("    ArchivedTime           : {0}", reader.Entry.ArchivedTime);
                    Trace.WriteLine("    CompressedSize         : {0}", reader.Entry.CompressedSize);
                    Trace.WriteLine("    CompressionType        : {0}", reader.Entry.CompressionType);
                    Trace.WriteLine("    CreatedTime            : {0}", reader.Entry.CreatedTime);
                    Trace.WriteLine("    IsDirectory            : {0}", reader.Entry.IsDirectory);
                    Trace.WriteLine("    IsEncrypted            : {0}", reader.Entry.IsEncrypted);
                    Trace.WriteLine("    Size                   : {0}", reader.Entry.Size);
                }
            }
        }

        public static void Test_CompressManager_01(string file, string directory = null, UncompressOptions options = UncompressOptions.None)
        {
            Trace.WriteLine("Uncompress file \"{0}\" in \"{1}\"", file, directory);
            Trace.WriteLine("options {0}", options);
            CompressManager compressManager = new CompressManager();
            UncompressResult result = compressManager.Uncompress(file, directory, options);
            //Trace.WriteLine("Uncompress files :");
            //foreach (string file2 in result.UncompressFiles)
            //    Trace.WriteLine("  \"{0}\"", file2);
            //Trace.WriteLine("Compress files :");
            //foreach (string file2 in result.CompressFiles)
            //    Trace.WriteLine("  \"{0}\"", file2);
            Trace.WriteLine("result :");
            Trace.WriteLine(result.zToJson());
        }
    }
}
