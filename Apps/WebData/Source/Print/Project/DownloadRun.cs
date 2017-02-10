using pb;
using pb.Compiler;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.IO;
using pb.Web.Http;

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

            // called from Extension_01.dll.project.xml
            //RunSerializer.InitDefault(traceProvider, traceSerializer);
            //RunSerializer.InitZValue(traceSerializer);
            //RunSerializer.InitWebHeader(traceSerializer);
        }

        public static void End()
        {
            // called from Extension_01.dll.project.xml
            //RunSerializer.EndDefault();
            //RunSerializer.EndZValue();
            //RunSerializer.EndWebHeader();

            System.Diagnostics.Trace.Listeners.Remove(__traceListener);
        }

        public static void InitAlways()
        {
            XmlConfig.CurrentConfig = new XmlConfig(RunSourceCommand.GetFilePath("download.config.xml"));
            XmlConfig config = XmlConfig.CurrentConfig;

            //RunSourceCommand.SetWriter(config.Get("Log").zRootPath(zapp.GetAppDirectory()), config.Get("Log/@option").zTextDeserialize(FileOption.None));
            RunSourceCommand.TraceSetWriter(WriteToFile.Create(config.Get("Log").zRootPath(zapp.GetAppDirectory()), config.Get("Log/@option").zTextDeserialize(FileOption.None)));

            string dataDir = config.GetExplicit("DataDir");
            AppData.DataDirectory = dataDir;

            //HttpManager.CurrentHttpManager.ExportDirectory = config.Get("HttpExportDirectory").zRootPath(zapp.GetEntryAssemblyDirectory());
            HttpRun.HttpManager.UrlCache = UrlCache.CreateIndexedCache(config.Get("HttpExportDirectory").zRootPath(zapp.GetEntryAssemblyDirectory()));

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
            RunSourceCommand.TraceRemoveWriter();
            XmlConfig.CurrentConfig = null;
        }
    }
}
