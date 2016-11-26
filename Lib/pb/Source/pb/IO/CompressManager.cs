using System.Collections.Generic;
using System.IO;

namespace pb.IO
{
    //CompressManagerMultiple
    //CompressManagerSharp
    //CompressManagerZip
    public partial class CompressManager
    {
        // error when uncompress some zip file (bonus6.zip)
        // System.IO.DirectoryNotFoundException occurred Could not find a part of the path 'c:\...\bonus6\Encyclopedie Junior-Dot Com-8 pdf\'.
        //private CompressBaseManager _zipManager = null;
        //private CompressBaseManager _sharpCompressManager = null;
        private CompressBaseManager _compressManager = null;

        //public CompressManager()
        //{
        //    //_zipManager = new ZipManager();
        //    //_sharpCompressManager = new SharpCompressManager();
        //    _compressManager = new SharpCompressManager();
        //}

        public CompressManager(CompressBaseManager compressManager)
        {
            _compressManager = compressManager;
        }

        public void Compress(string compressFile, IEnumerable<string> files, FileMode fileMode = FileMode.Create, CompressOptions compressOptions = CompressOptions.None, string rootDirectory = null)
        {
            bool entryAsFilename = false;
            if ((compressOptions & CompressOptions.StorePartialPath) == CompressOptions.StorePartialPath)
            {
                if (rootDirectory == null)
                    throw new PBException("need root directory to store partial path in zip file");
            }
            else
            {
                rootDirectory = null;
                if ((compressOptions & CompressOptions.StorePath) != CompressOptions.StorePath)
                    entryAsFilename = true;
            }

            _compressManager.Compress(compressFile, GetCompressFiles(files, rootDirectory, entryAsFilename), fileMode);

            if ((compressOptions & CompressOptions.DeleteSourceFiles) == CompressOptions.DeleteSourceFiles)
            {
                foreach (string file in files)
                    zFile.Delete(file);
            }
        }

        public void Compress(string compressFile, IEnumerable<CompressFile> files, FileMode fileMode = FileMode.Create, CompressOptions compressOptions = CompressOptions.None)
        {
            _compressManager.Compress(compressFile, files, fileMode);

            if ((compressOptions & CompressOptions.DeleteSourceFiles) == CompressOptions.DeleteSourceFiles)
            {
                foreach (CompressFile file in files)
                    zFile.Delete(file.File);
            }
        }

        // Action<string> doAfterUncompress = null
        public IEnumerable<string> Uncompress(string compressFile, string directory = null, IEnumerable<string> selectedFiles = null, UncompressOptions uncompressOptions = UncompressOptions.None)
        {
            if (directory == null)
                directory = zPath.GetDirectoryName(compressFile);
            return _compressManager.Uncompress(compressFile, directory, selectedFiles, uncompressOptions);
        }

        // Uncompress and rename files : extract CompressFile.CompressedFile and rename to CompressFile.File, only filename of CompressFile.File is used no path
        public IEnumerable<string> Uncompress(string compressFile, string directory, IEnumerable<CompressFile> selectedFiles, UncompressOptions uncompressOptions = UncompressOptions.None)
        {
            if (directory == null)
                directory = zPath.GetDirectoryName(compressFile);
            return _compressManager.Uncompress(compressFile, directory, selectedFiles, uncompressOptions);
        }

        public bool IsCompressFile(string file)
        {
            return _compressManager.IsCompressFile(file);
        }

        private static IEnumerable<CompressFile> GetCompressFiles(IEnumerable<string> files, string rootDirectory, bool entryAsFilename)
        {
            if (!rootDirectory.EndsWith("\\"))
                rootDirectory += "\\";
            int l = rootDirectory.Length;
            foreach (string file in files)
            {
                string compressedFile;
                if (entryAsFilename)
                    compressedFile = zPath.GetFileName(file);
                else if (rootDirectory != null && file.StartsWith(rootDirectory))
                    compressedFile = file.Substring(l);
                else
                    compressedFile = file;
                yield return new CompressFile { File = file, CompressedFile = compressedFile };
            }
        }

        //private static IEnumerable<string> GetFiles(IEnumerable<CompressFile> files)
        //{
        //    foreach (CompressFile file in files)
        //    {
        //        yield return file.CompressedFile;
        //    }
        //}

    }
}
