using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using pb;
using pb.Data.Mongo;

namespace Download.Print
{
    public class DownloadAutomate
    {
        public int id;
        public DateTime? lastRunDateTime;
        public TimeSpan timeBetweenRun;
    }

    public class MongoDownloadAutomateManager
    {
        private static TimeSpan __defaultTimeBetweenRun = TimeSpan.FromHours(1);
        private string _server = null;
        private string _database = null;
        private string _collectionName = null;
        private MongoCollection _collection = null;
        private int _id;
        private DownloadAutomate _downloadAutomate = null;

        public MongoDownloadAutomateManager(string server, string database, string collectionName, int id = 1)
        {
            _server = server;
            _database = database;
            _collectionName = collectionName;
            _id = id;
            LoadData();
        }

        public DateTime? GetLastRunDateTime()
        {
            return _downloadAutomate.lastRunDateTime;
        }

        public void SetLastRunDateTime(DateTime lastRunDateTime)
        {
            _downloadAutomate.lastRunDateTime = lastRunDateTime;
            SaveData();
        }

        public void SetTimeBetweenRun(TimeSpan timeBetweenRun)
        {
            _downloadAutomate.timeBetweenRun = timeBetweenRun;
            SaveData();
        }

        public DateTime GetNextRunDateTime()
        {
            if (_downloadAutomate.lastRunDateTime != null)
                return (DateTime)_downloadAutomate.lastRunDateTime + _downloadAutomate.timeBetweenRun;
            else
                return DateTime.Now;
        }

        private DownloadAutomate CreateData()
        {
            return new DownloadAutomate { id = _id, lastRunDateTime = null, timeBetweenRun = __defaultTimeBetweenRun };
        }

        private void LoadData()
        {
            BsonDocument document = GetCollection().zFindOneById<BsonDocument>(BsonValue.Create(_id));
            if (document != null)
                _downloadAutomate = BsonSerializer.Deserialize<DownloadAutomate>(document);
            else
                _downloadAutomate = CreateData();
        }

        private void SaveData()
        {
            BsonDocument document = _downloadAutomate.zToBsonDocument();
            GetCollection().zUpdate(new QueryDocument { { "_id", BsonValue.Create(_id) } }, new UpdateDocument { { "$set", document } }, UpdateFlags.Upsert);
        }

        public MongoCollection GetCollection()
        {
            if (_collection == null)
            {
                if (_server == null)
                    throw new PBException("error mongo server is'nt defined");
                if (_database == null)
                    throw new PBException("error mongo database is'nt defined");
                if (_collectionName == null)
                    throw new PBException("error mongo collection is'nt defined");
                _collection = new MongoClient(_server).GetServer().GetDatabase(_database).GetCollection(_collectionName);
            }
            return _collection;
        }
    }
}
