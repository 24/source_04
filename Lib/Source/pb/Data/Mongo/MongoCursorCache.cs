using System;
using System.Collections.Generic;
using System.IO;
using MongoDB.Bson;
using MongoDB.Driver;
using pb.IO;

namespace pb.Data.Mongo
{
    public static class MongoCursorCache
    {
        private static string __cacheDirectory = "mongo_cache";
        private static string __cacheFile = "mongo_cache.txt";

        public static IEnumerable<BsonDocument> zCacheCursor(this IEnumerable<BsonDocument> cursor, bool deleteTempFile = true)
        {
            //string cacheFile = zfile.GetNewIndexedFileName(__cacheFile);
            string cacheFile = zfile.GetNewIndexedFileName(__cacheDirectory) + "_" + __cacheFile;
            cursor.zSave(cacheFile);
            return zmongo.BsonReader<BsonDocument>(cacheFile, deleteFile: deleteTempFile);
        }

        public static string CacheFile
        {
            get { return __cacheFile; }
            set
            {
                __cacheDirectory = Path.GetDirectoryName(value);
                __cacheFile = Path.GetFileName(value);
            }
        }
    }
}
