using MongoDB.Driver;

namespace pb.Data.Mongo
{
    public static class zMongoDB
    {
        public static MongoCollection GetCollection(string collectionName, string databaseName, string serverName = null)
        {
            if (collectionName == null)
                throw new PBException("cant generate id, mongo collection is'nt defined");
            if (databaseName == null)
                throw new PBException("cant generate id, mongo database is'nt defined");
            if (serverName == null)
                serverName = "mongodb://localhost";
            return new MongoClient(serverName).GetServer().GetDatabase(databaseName).GetCollection(collectionName);
        }
    }

    public static partial class MongoDBExtension
    {
        public static string zGetFullName(this MongoDatabase database)
        {
            return database.Server.Instance.Address + "." + database.Name;
        }

        public static string zGetFullName(this MongoCollection collection)
        {
            return collection.Database.zGetFullName() + "." + collection.Name;
        }
    }
}
