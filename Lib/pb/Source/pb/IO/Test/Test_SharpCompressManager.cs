using System;
using System.Collections.Generic;
using System.IO;
using SharpCompress.Common;
using SharpCompress.Compressor.Deflate;

namespace pb.IO.Test
{
    public static class Test_SharpCompressManager
    {
        public static void Test_Compress_01(string compressFile, IEnumerable<string> files, ArchiveType archiveType = ArchiveType.Zip, string baseDirectory = null, CompressionLevel compressionLevel = CompressionLevel.Default)
        {
            Trace.WriteLine("Test_Compress_01 :");
            Trace.WriteLine("  compress file : \"{0}\"", compressFile);
            Trace.WriteLine("  archive type : {0}", archiveType);
            Trace.WriteLine("  compression level : {0}", compressionLevel);
            foreach (string file in files)
                Trace.WriteLine("  file : \"{0}\"", baseDirectory != null ? file.Substring(baseDirectory.Length + 1) : file);
            SharpCompressManager sharpCompressManager = new SharpCompressManager();
            sharpCompressManager.Compress(compressFile, files, archiveType, baseDirectory, compressionLevel);
            var fileInfo = zFile.CreateFileInfo(compressFile);
            Trace.WriteLine("  compressed file size : {0}", fileInfo.Length);
        }
    }
}
