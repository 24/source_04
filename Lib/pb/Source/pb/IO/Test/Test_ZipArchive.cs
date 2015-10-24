using System;
using System.IO;
using System.IO.Compression;

namespace pb.IO.Test
{
    public static class Test_ZipArchive
    {
        // CompressionLevel : 
        //   Optimal       : compress export_TelechargerMagazine_Detail.txt size 4 671 843 compressed size 1 081 398 ratio 4.3
        //   Fastest       : compress export_TelechargerMagazine_Detail.txt size 4 671 843 compressed size 1 262 392 ratio 3.7
        //   NoCompression : compress export_TelechargerMagazine_Detail.txt size 4 671 843 compressed size 4 672 730 ratio 0.9998

        public static void Test_ZipArchive_01()
        {
            Trace.WriteLine("Test_ZipArchive_01");
        }

        public static void Test_ZipArchive_AddEntry_01(string zipFile, string entryName, CompressionLevel compressionLevel = CompressionLevel.Optimal)
        {
            // The first example shows how to create a new entry and write to it by using a stream.
            // FileMode.Open
            using (FileStream fileStream = new FileStream(zipFile, FileMode.OpenOrCreate))
            {
                using (System.IO.Compression.ZipArchive archive = new System.IO.Compression.ZipArchive(fileStream, ZipArchiveMode.Update))
                {
                    ZipArchiveEntry readmeEntry = archive.CreateEntry(entryName, compressionLevel);
                    using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
                    {
                        writer.WriteLine("Information about this package.");
                        writer.WriteLine("========================");
                    }
                }
            }
        }

        public static void Test_ZipArchive_AddFile_01(string zipFile, string file, string entryName, CompressionLevel compressionLevel = CompressionLevel.Optimal,
            FileMode fileMode = FileMode.OpenOrCreate)
        {
            using (FileStream fileStream = new FileStream(zipFile, fileMode))
            {
                using (System.IO.Compression.ZipArchive archive = new System.IO.Compression.ZipArchive(fileStream, ZipArchiveMode.Update))
                {
                    archive.CreateEntryFromFile(file, entryName, compressionLevel);
                }
            }
        }
    }
}
