using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;

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

    //[Flags]
    //public enum ZipArchiveOptions
    //{
    //    None = 0x0000,
    //    StorePath = 0x0001,
    //    StorePartialPath = 0x0002,
    //    DeleteSourceFiles = 0x0004
    //}

    public class ZipArchive : IDisposable
    {
        //private string _file = null;
        //private FileStream _fileStream = null;
        private System.IO.Compression.ZipArchive _zipArchive = null;

        // zip variables
        //private string _rootDirectory = null;     // used to generate entryName
        //private bool _entryAsFilename = false;
        private CompressionLevel _compressionLevel = CompressionLevel.Optimal;
        private bool _setSlashInEntryName = true;  // needed for google doc

        // unzip variables
        private bool _extractFullPath = false;
        private bool _overrideExistingFile = false;
        private bool _renameExistingFile = false;
        // _selectedFiles values contains filename to use when extracting file
        private Dictionary<string, string> _selectedFiles = null;

        //public ZipArchive(string file, FileMode fileMode)
        public ZipArchive(FileStream fs, ZipArchiveMode mode, Encoding entryNameEncoding = null)
        {
            //_file = file;
            //Open(fileMode);
            if (entryNameEncoding == null)
                entryNameEncoding = Encoding.UTF8;
            _zipArchive = new System.IO.Compression.ZipArchive(fs, mode, false, entryNameEncoding);
        }

        public void Dispose()
        {
            //Close();
            if (_zipArchive != null)
            {
                _zipArchive.Dispose();
                _zipArchive = null;
            }
        }

        //public Encoding EntryNameEncoding { get { return _entryNameEncoding; } set { _entryNameEncoding = value; } }
        //public string RootDirectory { get { return _rootDirectory; } set { _rootDirectory = value; if (_rootDirectory != null && !_rootDirectory.EndsWith("\\")) _rootDirectory = _rootDirectory + "\\"; } }
        //public bool EntryAsFilename { get { return _entryAsFilename; } set { _entryAsFilename = value; } }
        public CompressionLevel CompressionLevel { get { return _compressionLevel; } set { _compressionLevel = value; } }
        public bool SetSlashInEntryName { get { return _setSlashInEntryName; } set { _setSlashInEntryName = value; } }

        //private void Open(FileMode fileMode)
        //{
        //    _fileStream = new FileStream(_file, fileMode);
        //    _zipArchive = new System.IO.Compression.ZipArchive(_fileStream, ZipArchiveMode.Update);
        //}

        //public void Close()
        //{
        //    if (_zipArchive != null)
        //    {
        //        _zipArchive.Dispose();
        //        _zipArchive = null;
        //    }
        //    if (_fileStream != null)
        //    {
        //        _fileStream.Close();
        //        _fileStream = null;
        //    }
        //}

        public void Zip(IEnumerable<CompressFile> files)
        {
            foreach (CompressFile file in files)
            {
                AddFile(file.File, file.CompressedFile);
            }
        }

        //public void Zip(IEnumerable<string> files)
        //{
        //    foreach (string file in files)
        //    {
        //        AddFile(file);
        //    }
        //}

        // bool entryAsFilename = false
        //public void AddFile(string file, string entryName = null)
        public void AddFile(string file, string entryName)
        {
            //if (entryName == null)
            //{
            //    if (_entryAsFilename)
            //        entryName = zPath.GetFileName(file);
            //    else if (_rootDirectory != null && file.StartsWith(_rootDirectory))
            //        entryName = file.Substring(_rootDirectory.Length);
            //    else
            //        entryName = file;
            //}
            if (entryName == null)
                throw new PBException("entryName is null");
            if (_setSlashInEntryName)
                entryName = entryName.Replace('\\', '/');
            ZipArchiveEntry entry = _zipArchive.GetEntry(entryName);
            if (entry != null)
                entry.Delete();
            _zipArchive.CreateEntryFromFile(file, entryName, _compressionLevel);
        }

        private void SetUnzipOptions(UncompressOptions options)
        {
            _extractFullPath = (options & UncompressOptions.ExtractFullPath) == UncompressOptions.ExtractFullPath;
            _overrideExistingFile = (options & UncompressOptions.OverrideExistingFile) == UncompressOptions.OverrideExistingFile;
            _renameExistingFile = (options & UncompressOptions.RenameExistingFile) == UncompressOptions.RenameExistingFile;
        }

        private void SetUnzipSelectedFiles(IEnumerable<string> selectedFiles = null)
        {
            if (selectedFiles != null)
            {
                _selectedFiles = new Dictionary<string, string>();
                foreach (string selectedFile in selectedFiles)
                    _selectedFiles.Add(selectedFile, null);
            }
        }

        private void SetUnzipSelectedFiles(IEnumerable<CompressFile> selectedFiles = null)
        {
            if (selectedFiles != null)
            {
                _selectedFiles = new Dictionary<string, string>();
                foreach (CompressFile selectedFile in selectedFiles)
                    _selectedFiles.Add(selectedFile.CompressedFile, selectedFile.File);
            }
        }

        //private IEnumerable<string> Unzip(string directory, IEnumerable<string> selectedFiles = null, UncompressOptions options = UncompressOptions.None)
        private IEnumerable<string> Unzip(string directory)
        {
            List<string> files = new List<string>();

            foreach (ZipArchiveEntry entry in _zipArchive.Entries)
            {
                string compressedFile = entry.FullName;

                string uncompressFile = null;

                //if (_selectedFiles != null && !_selectedFiles.ContainsKey(compressedFile))
                //    continue;
                if (_selectedFiles != null)
                {
                    //_selectedFiles.ContainsKey(compressedFile)
                    // _selectedFiles values contains filename to use when extracting file
                    if (!_selectedFiles.TryGetValue(compressedFile, out uncompressFile))
                        continue;
                }

                if (uncompressFile == null)
                {
                    if (_extractFullPath)
                        uncompressFile = compressedFile;
                    else
                        uncompressFile = zPath.GetFileName(compressedFile);
                }
                uncompressFile = zPath.Combine(directory, uncompressFile);

                if (zFile.Exists(uncompressFile))
                {
                    if (_overrideExistingFile)
                        zFile.Delete(uncompressFile);
                    else if (_renameExistingFile)
                        uncompressFile = zfile.GetNewFilename(uncompressFile);
                    else
                        throw new PBException("error file already exist can't uncompress \"{0}\"", uncompressFile);
                }

                zfile.CreateFileDirectory(uncompressFile);
                entry.ExtractToFile(uncompressFile);

                files.Add(uncompressFile);
            }

            return files;
        }

        public static void Zip(string zipFile, IEnumerable<CompressFile> files, FileMode fileMode = FileMode.Create)
        {
            using (FileStream fs = new FileStream(zipFile, fileMode))
            using (ZipArchive zipArchive = new ZipArchive(fs, ZipArchiveMode.Update))
            {
                //Trace.WriteLine("CompressionLevel.Fastest");
                zipArchive.CompressionLevel = CompressionLevel.Fastest;
                zipArchive.Zip(files);
            }
        }

        //public static void Zip(string zipFile, IEnumerable<string> files, FileMode fileMode = FileMode.Create, CompressOptions compressOptions = CompressOptions.None, string rootDirectory = null)
        //{
        //    bool entryAsFilename = false;
        //    if ((compressOptions & CompressOptions.StorePartialPath) == CompressOptions.StorePartialPath)
        //    {
        //        if (rootDirectory == null)
        //            throw new PBException("need root directory to store partial path in zip file");
        //    }
        //    else
        //    {
        //        rootDirectory = null;
        //        if ((compressOptions & CompressOptions.StorePath) != CompressOptions.StorePath)
        //            entryAsFilename = true;
        //    }
        //    //using (ZipArchive zipArchive = new ZipArchive(zipFile, fileMode))
        //    using (FileStream fs = new FileStream(zipFile, fileMode))
        //    using (ZipArchive zipArchive = new ZipArchive(fs, ZipArchiveMode.Update))
        //    {
        //        zipArchive.RootDirectory = rootDirectory;
        //        zipArchive.EntryAsFilename = entryAsFilename;
        //        //foreach (string file in files)
        //        //{
        //        //    zipArchive.AddFile(file, entryAsFilename: entryAsFilename);
        //        //}
        //        zipArchive.Zip(files);
        //    }
        //    if ((compressOptions & CompressOptions.DeleteSourceFiles) == CompressOptions.DeleteSourceFiles)
        //    {
        //        foreach (string file in files)
        //            zFile.Delete(file);
        //    }
        //}

        public static IEnumerable<string> Unzip(string zipFile, string directory, IEnumerable<string> selectedFiles = null, UncompressOptions uncompressOptions = UncompressOptions.None)
        {
            using (FileStream fs = new FileStream(zipFile, FileMode.Open))
            using (ZipArchive zipArchive = new ZipArchive(fs, ZipArchiveMode.Read))
            {
                zipArchive.SetUnzipOptions(uncompressOptions);
                zipArchive.SetUnzipSelectedFiles(selectedFiles);
                return zipArchive.Unzip(directory);
            }
        }

        public static IEnumerable<string> Unzip(string zipFile, string directory, IEnumerable<CompressFile> selectedFiles, UncompressOptions uncompressOptions = UncompressOptions.None)
        {
            using (FileStream fs = new FileStream(zipFile, FileMode.Open))
            using (ZipArchive zipArchive = new ZipArchive(fs, ZipArchiveMode.Read))
            {
                zipArchive.SetUnzipOptions(uncompressOptions);
                zipArchive.SetUnzipSelectedFiles(selectedFiles);
                return zipArchive.Unzip(directory);
            }
        }

        public static bool IsCompressFile(string file)
        {
            switch (zPath.GetExtension(file).ToLower())
            {
                case ".zip":
                    return true;
                default:
                    return false;
            }
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
