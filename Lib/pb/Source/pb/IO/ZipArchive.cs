using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace pb.IO
{
    //public static class TestZipArchive
    //{
    //    public static void Zip(System.Collections.Generic.IEnumerable<string> files)
    //    {
    //        using (ZipArchive zipArchive = new ZipArchive("zipfile.zip", FileMode.Create))
    //        {
    //            foreach (string file in files)
    //            {
    //                zipArchive.AddFile(file, entryName: zPath.GetFileName(file));
    //            }
    //        }
    //    }
    //}

    [Flags]
    public enum ZipArchiveOptions
    {
        None                = 0x0000,
        StorePath           = 0x0001,
        StorePartialPath    = 0x0002,
        DeleteSourceFiles   = 0x0004
    }

    public class ZipArchive : IDisposable
    {
        private string _file = null;
        private FileStream _fileStream = null;
        private System.IO.Compression.ZipArchive _zipArchive = null;
        private string _rootDirectory = null;     // used to generate entryName
        private CompressionLevel _compressionLevel = CompressionLevel.Optimal;

        public ZipArchive(string file, FileMode fileMode)
        {
            _file = file;
            Open(fileMode);
        }

        public void Dispose()
        {
            Close();
        }

        public string RootDirectory { get { return _rootDirectory; } set { _rootDirectory = value; if (_rootDirectory != null && !_rootDirectory.EndsWith("\\")) _rootDirectory = _rootDirectory + "\\"; } }
        public CompressionLevel CompressionLevel { get { return _compressionLevel; } set { _compressionLevel = value; } }

        private void Open(FileMode fileMode)
        {
            _fileStream = new FileStream(_file, fileMode);
            _zipArchive = new System.IO.Compression.ZipArchive(_fileStream, ZipArchiveMode.Update);
        }

        public void Close()
        {
            if (_zipArchive != null)
            {
                _zipArchive.Dispose();
                _zipArchive = null;
            }
            if (_fileStream != null)
            {
                _fileStream.Close();
                _fileStream = null;
            }
        }

        public void AddFile(string file, string entryName = null, bool entryAsFilename = false)
        {
            if (entryName == null)
            {
                if (entryAsFilename)
                    entryName = zPath.GetFileName(file);
                else if (_rootDirectory != null && file.StartsWith(_rootDirectory))
                    entryName = file.Substring(_rootDirectory.Length);
                else
                    entryName = file;
            }
            _zipArchive.CreateEntryFromFile(file, entryName, _compressionLevel);
        }

        //public static ZipArchive Open(string file, FileMode fileMode)
        //{
        //    return new ZipArchive(file, fileMode);
        //}

        public static void Zip(string zipFile, IEnumerable<string> files, FileMode fileMode = FileMode.Create, ZipArchiveOptions options = ZipArchiveOptions.None, string rootDirectory = null)
        {
            bool entryAsFilename = false;
            if ((options & ZipArchiveOptions.StorePartialPath) == ZipArchiveOptions.StorePartialPath)
            {
                if (rootDirectory == null)
                    throw new PBException("need root directory to store partial path in zip file");
            }
            else
            {
                rootDirectory = null;
                if ((options & ZipArchiveOptions.StorePath) != ZipArchiveOptions.StorePath)
                    entryAsFilename = true;
            }
            using (ZipArchive zipArchive = new ZipArchive(zipFile, fileMode))
            {
                zipArchive.RootDirectory = rootDirectory;
                foreach (string file in files)
                {
                    zipArchive.AddFile(file, entryAsFilename: entryAsFilename);
                }
            }
            if ((options & ZipArchiveOptions.DeleteSourceFiles) == ZipArchiveOptions.DeleteSourceFiles)
            {
                foreach (string file in files)
                {
                    zFile.Delete(file);
                }
            }
        }
    }
}
