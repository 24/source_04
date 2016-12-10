using pb.IO;
using System.IO;
using System.IO.Compression;
using System.Text;
using DirectoryInfo = Pri.LongPath.DirectoryInfo;
using FileSystemInfo = Pri.LongPath.FileSystemInfo;
using ZipArchive = System.IO.Compression.ZipArchive;
using PBZipArchive = pb.IO.ZipArchive;

namespace pb.Data.OpenXml.Test
{
    public static class Test_OpenXml_Zip
    {
        public static void Test_OpenXml_Zip_01(string docxFile, string directory, bool useSlash, bool addDirectoryEntry)
        {
            // ok    useSlash = false, addDirectoryEntry = false
            // bad   useSlash = false, addDirectoryEntry = true               le fichier est corrompu
            // ok    useSlash = true,  addDirectoryEntry = true
            // ok    useSlash = true,  addDirectoryEntry = false
            if (zFile.Exists(docxFile))
                zFile.Delete(docxFile);
            int l = directory.Length;
            if (!directory.EndsWith("\\"))
                l++;
            using (FileStream fs = new FileStream(docxFile, FileMode.OpenOrCreate))
            using (ZipArchive zipArchive = new ZipArchive(fs, ZipArchiveMode.Update, false, Encoding.UTF8))
            {
                int fileCount = 0;
                int directoryCount = 0;
                foreach (FileSystemInfo file in new DirectoryInfo(directory).EnumerateFileSystemInfos("*.*", SearchOption.AllDirectories))
                {
                    string entryName = file.FullName.Substring(l);
                    if (useSlash)
                        entryName = entryName.Replace('\\', '/');
                    if ((file.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        if (useSlash)
                            entryName = entryName + "/";
                        else
                            entryName = entryName + "\\";
                        if (addDirectoryEntry)
                        {
                            Trace.WriteLine($"add directory \"{entryName}\"");
                            ZipArchiveEntry entry = zipArchive.CreateEntry(entryName);
                            directoryCount++;
                        }
                    }
                    else
                    {
                        Trace.WriteLine($"add file      \"{entryName}\"");
                        zipArchive.CreateEntryFromFile(file.FullName, entryName);
                        fileCount++;
                    }
                }
                Trace.WriteLine($"total {fileCount + directoryCount} entries {fileCount} files {directoryCount} directories");
            }
        }

        // bool addDirectoryEntry
        public static void Test_OpenXml_Zip_02(string docxFile, string directory, bool useSlash)
        {
            //
            if (zFile.Exists(docxFile))
                zFile.Delete(docxFile);
            int l = directory.Length;
            if (!directory.EndsWith("\\"))
                l++;
            using (FileStream fs = new FileStream(docxFile, FileMode.OpenOrCreate))
            // Encoding.UTF8
            using (PBZipArchive zipArchive = new PBZipArchive(fs, ZipArchiveMode.Update, Encoding.UTF8))
            {
                int fileCount = 0;
                int directoryCount = 0;
                foreach (FileSystemInfo file in new DirectoryInfo(directory).EnumerateFileSystemInfos("*.*", SearchOption.AllDirectories))
                {
                    string entryName = file.FullName.Substring(l);
                    if (useSlash)
                        entryName = entryName.Replace('\\', '/');
                    //if ((file.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                    //{
                    //    if (useSlash)
                    //        entryName = entryName + "/";
                    //    else
                    //        entryName = entryName + "\\";
                    //    if (addDirectoryEntry)
                    //    {
                    //        Trace.WriteLine($"add directory \"{entryName}\"");
                    //        ZipArchiveEntry entry = zipArchive.CreateEntry(entryName);
                    //        directoryCount++;
                    //    }
                    //}
                    //else
                    if ((file.Attributes & FileAttributes.Directory) != FileAttributes.Directory)
                    {
                        Trace.WriteLine($"add file      \"{entryName}\"");
                        zipArchive.AddFile(file.FullName, entryName);
                        fileCount++;
                    }
                }
                Trace.WriteLine($"total {fileCount + directoryCount} entries {fileCount} files {directoryCount} directories");
            }
        }
    }
}
