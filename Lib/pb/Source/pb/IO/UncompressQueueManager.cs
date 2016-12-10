using System;
using System.Collections.Generic;
using System.IO;
using pb;
using pb.IO;
using System.Linq;

namespace Download.Print
{
    public class UncompressAction
    {
        //public string file;
        public string File;
        //public Action<string[]> onEndUncompress;
        public Action<IEnumerable<string>> OnEndUncompress;
    }

    //public class UncompressManager : AsyncManager
    public class UncompressQueueManager : AsyncManager
    {
        private bool _archiveCompress = true;
        private string _archiveCompressDirectory = null;
        private string _archiveErrorCompressDirectory = null;
        private CompressManager _compressManager = null;
        private Queue<UncompressAction> _uncompressQueue = new Queue<UncompressAction>();

        //public UncompressQueueManager()
        public UncompressQueueManager(CompressBaseManager compressManager)
        {
            //_compressManager = new CompressManager();
            _compressManager = new CompressManager(compressManager);
            Start();
        }

        public bool ArchiveCompress { get { return _archiveCompress; } set { _archiveCompress = value; } }
        public string ArchiveCompressDirectory { get { return _archiveCompressDirectory; } set { _archiveCompressDirectory = value; } }
        public string ArchiveErrorCompressDirectory { get { return _archiveErrorCompressDirectory; } set { _archiveErrorCompressDirectory = value; } }
        public CompressManager CompressManager { get { return _compressManager; } }

        //public void AsyncUncompress(string file, Action<string[]> onEndUncompress = null)
        public void AsyncUncompress(string file, Action<IEnumerable<string>> onEndUncompress = null)
        {
            _uncompressQueue.Enqueue(new UncompressAction { File = file, OnEndUncompress = onEndUncompress });
        }

        protected override void ThreadExecute()
        {
            //if (_uncompressQueue.Count == 0)
            //    return;
            while (_uncompressQueue.Count != 0)
            {
                UncompressAction uncompressAction = _uncompressQueue.Dequeue();
                //string[] uncompressFiles = Uncompress(uncompressAction.File);
                IEnumerable<string> uncompressFiles = Uncompress(uncompressAction.File);
                if (uncompressAction.OnEndUncompress != null)
                    uncompressAction.OnEndUncompress(uncompressFiles);
            }
        }

        public void UncompressDirectoryFiles(string directory, Action<string> uncompress = null)
        {
            //foreach (FileInfo fileInfo in new DirectoryInfo(directory).EnumerateFiles("*.*", SearchOption.AllDirectories))
            foreach (var fileInfo in zDirectory.CreateDirectoryInfo(directory).EnumerateFiles("*.*", SearchOption.AllDirectories))
            {
                //if (CompressManager.IsCompressFile(fileInfo.Name))
                if (_compressManager.IsCompressFile(fileInfo.Name))
                {
                    if (uncompress != null)
                        uncompress(fileInfo.FullName);
                    Uncompress(fileInfo.FullName);
                }
            }
        }

        //public string[] Uncompress(string file)
        public IEnumerable<string> Uncompress(string file)
        {
            string dir = zpath.PathSetExtension(file, "");
            if (zDirectory.Exists(dir))
            {
                dir = zdir.GetNewDirectory(dir);
            }
            zDirectory.CreateDirectory(dir);

            //UncompressMultipleResult uncompressResult = _compressManager.Uncompress(file, dir, UncompressMultipleOptions.ExtractFullPath | UncompressMultipleOptions.RenameExistingFile
            //    | UncompressMultipleOptions.UncompressNestedCompressFiles | UncompressMultipleOptions.DeleteNestedCompressFiles);
            UncompressMultipleResult uncompressResult = _compressManager.UncompressMultiple(file, dir, uncompressOptions: UncompressOptions.ExtractFullPath | UncompressOptions.RenameExistingFile,
               multipleOptions: UncompressMultipleOptions.UncompressNestedCompressFiles | UncompressMultipleOptions.DeleteNestedCompressFiles);

            // archive compress file
            //if (_archiveCompress && _archiveCompressDirectory != null)
            if (_archiveCompress)
            {
                //zdir.CreateDirectory(_archiveCompressDirectory);
                //ArchiveCompressFile(file);
                //foreach (string compressFile in uncompressResult.CompressFiles)
                //    ArchiveCompressFile(compressFile);

                if (_archiveCompressDirectory != null)
                    zdir.CreateDirectory(_archiveCompressDirectory);
                if (_archiveErrorCompressDirectory != null)
                    zdir.CreateDirectory(_archiveErrorCompressDirectory);
                foreach (UncompressFileResult uncompressFileResult in uncompressResult.UncompressFileResults)
                {
                    string compressFile = uncompressFileResult.File;
                    string archiveDirectory;
                    if (!uncompressFileResult.Error)
                        archiveDirectory = _archiveCompressDirectory;
                    else
                        archiveDirectory = _archiveErrorCompressDirectory;
                    if (archiveDirectory != null)
                        zFile.Move(compressFile, zfile.GetNewFilename(zPath.Combine(archiveDirectory, zPath.GetFileName(compressFile))));
                }
            }

            // si un seul fichier
            //if (uncompressResult.UncompressFiles.Length == 1)
            if (uncompressResult.UncompressFiles.Count() == 1)
            {
                //string uncompressFile = uncompressResult.UncompressFiles[0];
                string uncompressFile = uncompressResult.UncompressFiles.First();
                string newFile = zPath.Combine(zPath.GetDirectoryName(file), zPath.GetFileNameWithoutExtension(file)) + zPath.GetExtension(uncompressFile);
                newFile = zfile.GetNewFilename(newFile);
                zFile.Move(zPath.Combine(dir, uncompressFile), newFile);
                zDirectory.Delete(dir);
                return new string[] { newFile };
            }
            return uncompressResult.UncompressFiles;
        }

        private void ArchiveCompressFile(string file)
        {
            zFile.Move(file, zfile.GetNewFilename(zPath.Combine(_archiveCompressDirectory, zPath.GetFileName(file))));
        }
    }
}
