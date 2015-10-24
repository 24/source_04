using System;
using System.IO;
using System.IO.Compression;

namespace pb.IO
{
    public class ZipArchive : IDisposable
    {
        private string _file = null;
        private FileStream _fileStream = null;
        private System.IO.Compression.ZipArchive _zipArchive = null;
        private string _rootPath = null;     // used to generate entryName
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

        public string RootPath { get { return _rootPath; } set { _rootPath = value; if (!_rootPath.EndsWith("\\")) _rootPath = _rootPath + "\\"; } }
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

        public void AddFile(string file, string entryName = null, bool noDirectory = false)
        {
            if (entryName == null)
            {
                if (noDirectory)
                    entryName = zPath.GetFileName(file);
                else if (_rootPath != null && file.StartsWith(_rootPath))
                    entryName = file.Substring(_rootPath.Length);
                else
                    entryName = file;
            }
            _zipArchive.CreateEntryFromFile(file, entryName, _compressionLevel);
        }

        //public static ZipArchive Open(string file, FileMode fileMode)
        //{
        //    return new ZipArchive(file, fileMode);
        //}
    }
}
