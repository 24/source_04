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

        public static void Init()
        {
            //Trace.WriteLine("Download.Print._RunCode.Init()");
            //RunSource rs = RunSource.CurrentRunSource;
            //rs.InitConfig("download");
            XmlConfig.CurrentConfig = new XmlConfig(RunSource.CurrentRunSource.GetFilePath("download.config.xml"));
            XmlConfig config = XmlConfig.CurrentConfig;

            //string log = config.Get("Log").zRootPath(zapp.GetEntryAssemblyDirectory());
            //if (log != null)
            //    Trace.CurrentTrace.SetLogFile(log, LogOptions.IndexedFile);

            Trace.CurrentTrace.SetWriter(config.Get("Log").zRootPath(zapp.GetAppDirectory()), config.Get("Log/@option").zTextDeserialize(FileOption.None));

            __traceListener = new PBTraceListener();
            //__traceListener.Filter = new System.Diagnostics.EventTypeFilter(System.Diagnostics.SourceLevels.Warning);
            System.Diagnostics.Trace.Listeners.Add(__traceListener);
            string dataDir = config.GetExplicit("DataDir");
            //RunSource.CurrentRunSource.DataDir = dataDir;
            AppData.DataDirectory = dataDir;

            //string trace = config.Get("Trace").zRootPath(zapp.GetEntryAssemblyDirectory());
            //if (trace != null && trace != "")
            //    Trace.CurrentTrace.SetTraceDirectory(trace);
            HttpManager.CurrentHttpManager.ExportDirectory = config.Get("HttpExportDirectory").zRootPath(zapp.GetEntryAssemblyDirectory());

            //string mongoLog = config.Get("MongoLog").zRootPath(zapp.GetEntryAssemblyDirectory());
            //if (mongoLog != null)
            //{
            //    // set mongo log file
            //    MongoLog.CurrentMongoLog.SetLogFile(mongoLog, LogOptions.IndexedFile);
            //    // send mongo log file to current trace
            //    //MongoLog.CurrentMongoLog.Log.Writed += Trace.CurrentTrace.Write;
            //}
            MongoLog.CurrentMongoLog.SetLogFile(config.Get("MongoLog").zRootPath(zapp.GetEntryAssemblyDirectory()), config.Get("MongoLog/@option").zTextDeserialize(FileOption.None));

            string mongoCache = config.Get("MongoCache").zRootPath(zapp.GetEntryAssemblyDirectory());
            if (mongoCache != null)
                MongoCursorCache.CacheFile = mongoCache;
            //pb.Data.Mongo.TraceMongoCommand.ResultToText = true;
            //pb.Data.Mongo.TraceMongoCommand.ResultToGrid = true;
            DefaultMongoSerialization.SetDefaultMongoSerializationOptions();
            DefaultMongoSerialization.RegisterDefaultMongoSerializer();
            //MailSender.Trace = true;
            //Download.Print.RapideDdl.RapideDdl.Trace = true;
            //Download.Print.RapideDdl.RapideDdl_LoadPostDetailFromWebManager.Trace = true;
            //Download.Print.GoldenDdl.GoldenDdl_LoadPostDetailFromWebManager.Trace = true;
            //Download.Print.Ebookdz.Ebookdz_LoadPostDetail.Trace = true;
            //global::Print.PrintTextValuesManager.Trace = true;

            // export data of PrintTextValuesManager
            bool exportData = config.Get("TextInfos/ExportData").zParseAs<bool>();
            if (exportData)
            {
                string exportDataFile = config.Get("TextInfos/ExportDataFile");
                if (exportDataFile != null)
                    DownloadPrint.PrintTextValuesManager.SetExportDataFile(exportDataFile);
            }

            HtmlRun.SetResult = dt => RunSource.CurrentRunSource.SetResult(dt);
            //FindPrintManager.TraceWarning = true;
            //DebriderAlldebrid.Trace = true;
        }

        public static void End()
        {
            //Trace.WriteLine("Download.Print._RunCode.End()");
            //Download.Print.RapideDdl.RapideDdl.UnregisterMongoSerializer();
            DefaultMongoSerialization.RemoveDefaultMongoSerializationOptions();
            DefaultMongoSerialization.UnregisterDefaultMongoSerializer();
            //MongoLog.CurrentMongoLog.Log.Writed -= Trace.CurrentTrace.Write;
            System.Diagnostics.Trace.Listeners.Remove(__traceListener);
            DownloadPrint.PrintTextValuesManager.CloseExportDataFile();
            HtmlRun.SetResult = null;
        }
    }
}
