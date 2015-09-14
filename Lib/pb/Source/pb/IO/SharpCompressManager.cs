using System;
using System.Collections.Generic;
using System.IO;
using SharpCompress.Archive.Zip;
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

        // File : Create new, Append to existing, Raz existing
        // ArchiveType : Rar = 0, Zip = 1, Tar = 2, SevenZip = 3, GZip = 4
        // CompressionType : None = 0, GZip = 1, BZip2 = 2, PPMd = 3, Deflate = 4, Rar = 5, LZMA = 6, BCJ = 7, BCJ2 = 8, Unknown = 9,
        // Zip compression type : BZip2
        // GZip compression type : GZip
        // example from https://github.com/adamhathcock/sharpcompress/wiki/API-Examples
        public void Test_Compress_01(string compressFile, IEnumerable<string> files, string baseDirectory = null, ArchiveType archiveType = ArchiveType.Zip,
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
                    writer.Write(entryPath, file);
                }
            }
        }

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
