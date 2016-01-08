using MongoDB.Driver;
using pb.IO;

namespace pb.Data.Mongo
{
    public class MongoBackup
    {
        //private string _backupDirectory = null;
        //private string _tempBackupDirectory = null;
        //private string _zipFilename = null;
        //private List<MongoCollection> _collections = new List<MongoCollection>();

        //public string BackupDirectory { get { return _backupDirectory; } set { _backupDirectory = value; } }
        //public string TempBackupDirectory { get { return _tempBackupDirectory; } set { _tempBackupDirectory = value; } }

        //public void AddCollection(MongoCollection collection)
        //{
        //    _collections.Add(collection);
        //}

        //public void AddCollections(IEnumerable<MongoCollection> collections)
        //{
        //    foreach (MongoCollection collection in collections)
        //        _collections.Add(collection);
        //}

        //public void _Backup()
        //{
        //    List<string> files = new List<string>();
        //    foreach (MongoCollection collection in _collections)
        //    {
        //        string file = _Backup(collection);
        //        files.Add(file);
        //    }
        //    string zipFile = Zip(files);
        //}

        //private string _Backup(MongoCollection collection)
        //{
        //    string name = collection.Database.Name + "." + collection.Name;
        //    string file = zPath.Combine(_tempBackupDirectory, name + ".txt");
        //    Trace.WriteLine("export mongo collection \"{0}\"", name);
        //    MongoCommand.Export(collection, file);
        //    return file;
        //}

        //private string Zip(IEnumerable<string> files)
        //{
        //    if (_zipFilename == null)
        //        throw new PBException("zip file is not defined");
        //    if (_tempBackupDirectory == null)
        //        throw new PBException("temp backup directory is not defined");
        //    string zipFile = zPath.Combine(_tempBackupDirectory, _zipFilename + ".zip");
        //    Trace.WriteLine("zip files to \"{0}\"", zipFile);
        //    ZipArchive.Zip(zipFile, files, options: ZipArchiveOptions.DeleteSourceFiles);
        //    return zipFile;
        //}

        //private void MoveFilesToBackupDirectory(string file)
        //{
        //    if (_backupDirectory == null)
        //        throw new PBException("backup directory is not defined");
        //    Trace.WriteLine("move zip file to \"{0}\"", _backupDirectory);
        //    zfile.MoveFile(file, _backupDirectory, overwrite: true);
        //}

        public static string Backup(MongoCollection collection, string directory)
        {
            if (directory == null)
                throw new PBException("directory is not defined");
            string name = collection.Database.Name + "." + collection.Name;
            string file = zPath.Combine(directory, name + ".txt");
            Trace.WriteLine("export mongo collection \"{0}\"", name);
            MongoCommand.Export(collection, file);
            return file;
        }
    }
}
