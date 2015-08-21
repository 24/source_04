using System;
using System.Collections.Generic;
using MongoDB.Driver;
using pb.IO;

namespace pb.Data.Mongo
{
    public class MongoBackup
    {
        private string _backupDirectory = null;
        private string _tmpBackupDirectory = null;
        private List<MongoCollection> _collections = new List<MongoCollection>();

        public string BackupDirectory { get { return _backupDirectory; } set { _backupDirectory = value; } }
        public string TmpBackupDirectory { get { return _tmpBackupDirectory; } set { _tmpBackupDirectory = value; } }

        public void AddCollection(MongoCollection collection)
        {
            _collections.Add(collection);
        }

        public void AddCollections(IEnumerable<MongoCollection> collections)
        {
            foreach (MongoCollection collection in collections)
                _collections.Add(collection);
        }

        public void Backup()
        {
            foreach (MongoCollection collection in _collections)
            {

            }
        }

        private void _Backup(MongoCollection collection)
        {
            string name = collection.Name;
            string file = zPath.Combine(_tmpBackupDirectory, name + ".txt");
            MongoCommand.Export(collection, file);
        }
    }
}
