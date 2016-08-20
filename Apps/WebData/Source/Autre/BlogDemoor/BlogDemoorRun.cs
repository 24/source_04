using pb;
using pb.Compiler;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.IO;
using pb.Web;

namespace WebData.BlogDemoor
{
    public static class BlogDemoorRun
    {
        private static PBTraceListener __traceListener = null;

        public static void Init()
        {
            __traceListener = new PBTraceListener();
            //__traceListener.Filter = new System.Diagnostics.EventTypeFilter(System.Diagnostics.SourceLevels.Warning);
            System.Diagnostics.Trace.Listeners.Add(__traceListener);

            //DefaultMongoSerialization.SetDefaultMongoSerializationOptions();
            //DefaultMongoSerialization.RegisterDefaultMongoSerializer();

            HtmlRun.SetResult = dt => RunSource.CurrentRunSource.SetResult(dt);
        }

        public static void End()
        {
            //DefaultMongoSerialization.RemoveDefaultMongoSerializationOptions();
            //DefaultMongoSerialization.UnregisterDefaultMongoSerializer();
            System.Diagnostics.Trace.Listeners.Remove(__traceListener);
            HtmlRun.SetResult = null;
        }

        public static void InitAlways()
        {
            XmlConfig.CurrentConfig = new XmlConfig(RunSource.CurrentRunSource.GetFilePath("BlogDemoor.config.xml"));
            XmlConfig config = XmlConfig.CurrentConfig;

            Trace.CurrentTrace.SetWriter(config.Get("Log").zRootPath(zapp.GetAppDirectory()), config.Get("Log/@option").zTextDeserialize(FileOption.None));

            HttpManager.CurrentHttpManager.ExportDirectory = config.Get("HttpExportDirectory").zRootPath(zapp.GetEntryAssemblyDirectory());

            MongoLog.CurrentMongoLog.SetLogFile(config.Get("MongoLog").zRootPath(zapp.GetEntryAssemblyDirectory()), config.Get("MongoLog/@option").zTextDeserialize(FileOption.None));
            string mongoCache = config.Get("MongoCache").zRootPath(zapp.GetEntryAssemblyDirectory());
            if (mongoCache != null)
                MongoCursorCache.CacheFile = mongoCache;
        }

        public static void EndAlways()
        {
        }
    }
}
