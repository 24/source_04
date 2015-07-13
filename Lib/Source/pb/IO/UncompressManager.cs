using System;
using System.Collections.Generic;
using System.IO;
using pb;
using pb.IO;
using SharpCompress.Common;
using SharpCompress.Reader;

namespace Download.Print
{
    public class UncompressAction
    {
        public string file;
        public Action<string[]> onEndUncompress;
    }

    public class UncompressManager : AsyncManager
    {
        private bool _archiveCompress = true;
        private string _archiveCompressDirectory = null;
        private string _archiveErrorCompressDirectory = null;
        private CompressManager _compressManager = null;
        private Queue<UncompressAction> _uncompressQueue = new Queue<UncompressAction>();

        public UncompressManager()
        {
            _compressManager = new CompressManager();
            Start();
        }

        public bool ArchiveCompress { get { return _archiveCompress; } set { _archiveCompress = value; } }
        public string ArchiveCompressDirectory { get { return _archiveCompressDirectory; } set { _archiveCompressDirectory = value; } }
        public string ArchiveErrorCompressDirectory { get { return _archiveErrorCompressDirectory; } set { _archiveErrorCompressDirectory = value; } }

        public void AsyncUncompress(string file, Action<string[]> onEndUncompress = null)
        {
            _uncompressQueue.Enqueue(new UncompressAction { file = file, onEndUncompress = onEndUncompress });
        }

        protected override void ThreadExecute()
        {
            //if (_uncompressQueue.Count == 0)
            //    return;
            while (_uncompressQueue.Count != 0)
            {
                UncompressAction uncompressAction = _uncompressQueue.Dequeue();
                string[] uncompressFiles = Uncompress(uncompressAction.file);
                if (uncompressAction.onEndUncompress != null)
                    uncompressAction.onEndUncompress(uncompressFiles);
            }
        }

        public string[] Uncompress(string file)
        {
            //try
            //{
                string dir = zpath.PathSetExtension(file, "");
                if (Directory.Exists(dir))
                {
                    dir = zdir.GetNewDirectory(dir);
                }
                Directory.CreateDirectory(dir);

                UncompressResult uncompressResult = _compressManager.Uncompress(file, dir, UncompressOptions.ExtractFullPath | UncompressOptions.RenameExistingFile
                    | UncompressOptions.UncompressNestedCompressFiles | UncompressOptions.DeleteNestedCompressFiles);

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
                            File.Move(compressFile, zfile.GetNewFilename(Path.Combine(archiveDirectory, Path.GetFileName(compressFile))));
                    }
                }

                // si un seul fichier
                if (uncompressResult.UncompressFiles.Length == 1)
                {
                    string uncompressFile = uncompressResult.UncompressFiles[0];
                    string newFile = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file)) + Path.GetExtension(uncompressFile);
                    newFile = zfile.GetNewFilename(newFile);
                    File.Move(Path.Combine(dir, uncompressFile), newFile);
                    Directory.Delete(dir);
                    return new string[] { newFile };
                }
                return uncompressResult.UncompressFiles;
            //}
            //catch (Exception ex)
            //{
            //    Trace.WriteLine("error uncompress file \"{0}\"", file);
            //    Trace.WriteLine(ex.Message);
            //}
            //return null;
        }

        private void ArchiveCompressFile(string file)
        {
            File.Move(file, zfile.GetNewFilename(Path.Combine(_archiveCompressDirectory, Path.GetFileName(file))));
        }
    }
}
