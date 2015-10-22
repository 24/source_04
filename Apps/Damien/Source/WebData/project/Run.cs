using System;
using pb;
using pb.Compiler;
using pb.Data.Mongo;
using pb.Data.Mongo.Serializers;
using pb.Data.Xml;
using pb.IO;
using pb.Web;

namespace hts
{
    class Run
    {
        public static void Init()
        {
            XmlConfig.CurrentConfig = new XmlConfig(RunSource.CurrentRunSource.GetFilePath("hts.config.xml"));
            XmlConfig config = XmlConfig.CurrentConfig;
            Trace.CurrentTrace.SetWriter(config.Get("Log").zRootPath(zapp.GetAppDirectory()), config.Get("Log/@option").zTextDeserialize(FileOption.None));
            AppData.DataDirectory = config.GetExplicit("DataDir");
            HttpManager.CurrentHttpManager.ExportDirectory = config.Get("HttpExportDirectory").zRootPath(zapp.GetEntryAssemblyDirectory());
            MongoLog.CurrentMongoLog.SetLogFile(config.Get("MongoLog").zRootPath(zapp.GetEntryAssemblyDirectory()), config.Get("MongoLog/@option").zTextDeserialize(FileOption.None));
            string mongoCache = config.Get("MongoCache").zRootPath(zapp.GetEntryAssemblyDirectory());
            if (mongoCache != null)
                MongoCursorCache.CacheFile = mongoCache;
            pb.Data.Mongo.TraceMongoCommand.ResultToGrid = true;
            DefaultMongoSerialization.SetDefaultMongoSerializationOptions();
            DefaultMongoSerialization.RegisterDefaultMongoSerializer();
            HtmlRun.SetResult = dt => RunSource.CurrentRunSource.SetResult(dt);
        }

        public static void End()
        {
            DefaultMongoSerialization.RemoveDefaultMongoSerializationOptions();
            DefaultMongoSerialization.UnregisterDefaultMongoSerializer();
            HtmlRun.SetResult = null;
        }
    }
}
