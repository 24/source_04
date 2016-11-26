using System.Collections.Generic;
using MongoDB.Bson;
using pb.IO;

namespace pb.Data.Mongo
{
    public static class MongoCursorCache
    {
        private static string __cacheDirectory = "mongo_cache";
        private static string __cacheFile = "mongo_cache.txt";

        public static IEnumerable<BsonDocument> zCacheCursor(this IEnumerable<BsonDocument> cursor, bool deleteTempFile = true)
        {
            string cacheFile = null;
            try
            {
                cacheFile = zfile.GetNewIndexedFileName(__cacheDirectory) + "_" + __cacheFile;
                cursor.zSave(cacheFile);
                return zmongo.BsonRead<BsonDocument>(cacheFile);
            }
            finally
            {
                if (deleteTempFile && cacheFile != null)
                    zFile.Delete(cacheFile);
            }
        }

        public static string CacheFile
        {
            get { return __cacheFile; }
            set
            {
                __cacheDirectory = zPath.GetDirectoryName(value);
                __cacheFile = zPath.GetFileName(value);
            }
        }
    }
}
