using System.Collections.Generic;
using SharpCompress.Common;
using SharpCompress.Compressor.Deflate;
using System.IO;
using SharpCompress.Writer;

namespace pb.IO.Test
{
    public static class Test_SharpCompressManager
    {
        public static void Test_SharpCompressManager_01()
        {
            Trace.WriteLine("Test_SharpCompressManager_01");
        }

        public static void Test_CompressZip_01(string compressFile, IEnumerable<string> files, string baseDirectory = null, CompressionLevel compressionLevel = CompressionLevel.Default)
        {
            Test_Compress_01(compressFile, files, baseDirectory, compressionLevel: compressionLevel, archiveType: ArchiveType.Zip, compressionType: CompressionType.BZip2);
        }

        public static void Test_Compress_01(string compressFile, IEnumerable<string> files, string baseDirectory = null, ArchiveType archiveType = ArchiveType.Zip,
            CompressionType compressionType = CompressionType.BZip2, CompressionLevel compressionLevel = CompressionLevel.Default)
        {
            Trace.WriteLine("Test_Compress_01 :");
            Trace.WriteLine("  compress file        : \"{0}\"", compressFile);
            Trace.WriteLine("  archive type         : {0}", archiveType);
            Trace.WriteLine("  compression type     : {0}", compressionType);
            Trace.WriteLine("  compression level    : {0}", compressionLevel);
            foreach (string file in files)
                Trace.WriteLine("  file                 : \"{0}\"", baseDirectory != null ? file.Substring(baseDirectory.Length + 1) : file);
            SharpCompressManager sharpCompressManager = new SharpCompressManager();
            //sharpCompressManager.Test_Compress_01(compressFile, files, baseDirectory, archiveType, compressionType, compressionLevel);
            Test_Compress(compressFile, files, baseDirectory, archiveType, compressionType, compressionLevel);
            var fileInfo = zFile.CreateFileInfo(compressFile);
            Trace.WriteLine("  compressed file size : {0}", fileInfo.Length);
        }

        // File : Create new, Append to existing, Raz existing
        // ArchiveType : Rar = 0, Zip = 1, Tar = 2, SevenZip = 3, GZip = 4
        // CompressionType : None = 0, GZip = 1, BZip2 = 2, PPMd = 3, Deflate = 4, Rar = 5, LZMA = 6, BCJ = 7, BCJ2 = 8, Unknown = 9,
        // Zip compression type : BZip2
        // GZip compression type : GZip
        // example from https://github.com/adamhathcock/sharpcompress/wiki/API-Examples
        // this example dont work to add file to an existing zip
        public static void Test_Compress(string compressFile, IEnumerable<string> files, string baseDirectory = null, ArchiveType archiveType = ArchiveType.Zip,
            CompressionType compressionType = CompressionType.BZip2, CompressionLevel compressionLevel = CompressionLevel.Default)
        {
            //FileOption
            if (baseDirectory != null && !baseDirectory.EndsWith("\\"))
                baseDirectory = baseDirectory + "\\";
            CompressionInfo compressionInfo = new CompressionInfo();
            compressionInfo.DeflateCompressionLevel = compressionLevel;
            compressionInfo.Type = compressionType;

            //Trace.WriteLine("SharpCompressManager : DeflateCompressionLevel {0}", compressionInfo.DeflateCompressionLevel);
            //Trace.WriteLine("SharpCompressManager : CompressionType {0}", compressionInfo.Type);

            Trace.WriteLine($"open compressed file \"{compressFile}\"");
            // File.OpenWrite ==> OpenOrCreate
            using (FileStream stream = File.OpenWrite(compressFile))
            using (IWriter writer = WriterFactory.Open(stream, archiveType, compressionInfo))
            //using (IWriter writer = WriterFactory.Open(stream, archiveType, CompressionType.BZip2))
            {
                foreach (string file in files)
                {
                    string entryPath;
                    if (baseDirectory != null && file.StartsWith(baseDirectory))
                        entryPath = file.Substring(baseDirectory.Length);
                    else
                        entryPath = zPath.GetFileName(file);
                    Trace.WriteLine($"add file \"{entryPath}\"  \"{file}\"");
                    writer.Write(entryPath, file);
                }
            }
        }
    }
}
