using pb;
using pb.Compiler;
using pb.Data.Mongo;
using pb.Data.Mongo.Serializers;
using pb.Data.Xml;
using pb.IO;
using pb.Web;

namespace Download.Print
{
    //public static partial class _RunCode
    public static class DownloadRun
    {
        private static PBTraceListener __traceListener = null;

        // load dll but to late to get Init method, dll is loaded only when code from dll is executed
        // used to load dll ebook.download.dll from project download.dll.project.xml
        //public static void Fake()
        //{
        //}

        public static void Init(bool traceProvider = false, bool traceSerializer = false)
        {
            __traceListener = new PBTraceListener();
            //__traceListener.Filter = new System.Diagnostics.EventTypeFilter(System.Diagnostics.SourceLevels.Warning);
            System.Diagnostics.Trace.Listeners.Add(__traceListener);

            // ATTENTION à ré-écrire
            //PBMongoSerialization.SetDefaultMongoSerializationOptions();
            //PBMongoSerialization.RegisterDefaultMongoSerializer();
            RunSerializer.InitDefault(traceProvider, traceSerializer);
            RunSerializer.InitZValue(traceSerializer);

            HtmlRun.SetResult = dt => RunSource.CurrentRunSource.SetResult(dt);
        }

        public static void End()
        {
            // ATTENTION à ré-écrire
            //PBMongoSerialization.RemoveDefaultMongoSerializationOptions();
            MongoSerializationManager.RemoveDefaultMongoSerializationOptions();
            //PBMongoSerialization.UnregisterDefaultMongoSerializer();

            System.Diagnostics.Trace.Listeners.Remove(__traceListener);
            HtmlRun.SetResult = null;
        }

        public static void InitAlways()
        {
            XmlConfig.CurrentConfig = new XmlConfig(RunSource.CurrentRunSource.GetFilePath("download.config.xml"));
            XmlConfig config = XmlConfig.CurrentConfig;

            Trace.CurrentTrace.SetWriter(config.Get("Log").zRootPath(zapp.GetAppDirectory()), config.Get("Log/@option").zTextDeserialize(FileOption.None));

            string dataDir = config.GetExplicit("DataDir");
            AppData.DataDirectory = dataDir;

            HttpManager.CurrentHttpManager.ExportDirectory = config.Get("HttpExportDirectory").zRootPath(zapp.GetEntryAssemblyDirectory());

            MongoLog.CurrentMongoLog.SetLogFile(config.Get("MongoLog").zRootPath(zapp.GetEntryAssemblyDirectory()), config.Get("MongoLog/@option").zTextDeserialize(FileOption.None));
            string mongoCache = config.Get("MongoCache").zRootPath(zapp.GetEntryAssemblyDirectory());
            if (mongoCache != null)
                MongoCursorCache.CacheFile = mongoCache;

            // export data of PrintTextValuesManager
            bool exportData = config.Get("TextInfos/ExportData").zParseAs<bool>();
            if (exportData)
            {
                string exportDataFile = config.Get("TextInfos/ExportDataFile");
                if (exportDataFile != null)
                    DownloadPrint.PrintTextValuesManager.SetExportDataFile(exportDataFile);
            }
        }

        public static void EndAlways()
        {
            DownloadPrint.PrintTextValuesManager.CloseExportDataFile();
        }
    }
}
