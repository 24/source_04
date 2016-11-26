using System;
using System.Collections.Generic;

namespace pb.IO
{
    public class Backup
    {
        private string _tempBackupDirectory = null;
        private string _backupDirectory = null;
        private string _zipFilename = null;
        private List<Func<string, IEnumerable<string>>> _backupActions = new List<Func<string, IEnumerable<string>>>();
        private CompressManager _compressManager = new CompressManager(new ZipManager());

        public string TempBackupDirectory { get { return _tempBackupDirectory; } set { _tempBackupDirectory = value; } }
        public string BackupDirectory { get { return _backupDirectory; } set { _backupDirectory = value; } }
        public string ZipFilename { get { return _zipFilename; } set { _zipFilename = value; } }

        public void Add(Func<string, IEnumerable<string>> backupAction)
        {
            _backupActions.Add(backupAction);
        }

        public void Add(Func<string, string> backupAction)
        {
            _backupActions.Add(dir => new string[] { backupAction(dir) });
        }

        public void DoBackup()
        {
            if (_tempBackupDirectory == null)
                throw new PBException("temp backup directory is not defined");
            List<string> files = new List<string>();
            foreach (Func<string, IEnumerable<string>> backupAction in _backupActions)
            {
                files.AddRange(backupAction(_tempBackupDirectory));
            }
            string zipFile = Zip(files);
            MoveFilesToBackupDirectory(zipFile);
        }

        private string Zip(IEnumerable<string> files)
        {
            if (_zipFilename == null)
                throw new PBException("zip file is not defined");
            if (_tempBackupDirectory == null)
                throw new PBException("temp backup directory is not defined");
            string zipFile = zPath.Combine(_tempBackupDirectory, _zipFilename + ".zip");
            Trace.WriteLine("zip files to \"{0}\"", zipFile);
            //ZipArchive.Zip(zipFile, files, compressOptions: CompressOptions.DeleteSourceFiles);
            _compressManager.Compress(zipFile, files, compressOptions: CompressOptions.DeleteSourceFiles);
            return zipFile;
        }

        private void MoveFilesToBackupDirectory(string file)
        {
            if (_backupDirectory == null)
                throw new PBException("backup directory is not defined");
            Trace.WriteLine("move zip file to \"{0}\"", _backupDirectory);
            zfile.MoveFile(file, _backupDirectory, overwrite: true);
        }
    }
}
