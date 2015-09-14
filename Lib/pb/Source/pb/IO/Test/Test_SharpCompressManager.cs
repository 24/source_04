using System;
using System.Collections.Generic;
using System.IO;
using SharpCompress.Common;
using SharpCompress.Compressor.Deflate;

namespace pb.IO.Test
{
    public static class Test_SharpCompressManager
    {
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
            sharpCompressManager.Test_Compress_01(compressFile, files, baseDirectory, archiveType, compressionType, compressionLevel);
            var fileInfo = zFile.CreateFileInfo(compressFile);
            Trace.WriteLine("  compressed file size : {0}", fileInfo.Length);
        }
    }
}
