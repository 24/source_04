using pb;
using pb.Compiler;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.IO;
using pb.Web.Http;

namespace hts
{
    class Run
    {
        public static void InitAlways()
        {
            XmlConfig.CurrentConfig = new XmlConfig(RunSourceCommand.GetFilePath("hts.config.xml"));
            XmlConfig config = XmlConfig.CurrentConfig;

            string logFile = config.Get("Log").zRootPath(zapp.GetAppDirectory());
            //Trace.WriteLine($"set log file \"{logFile}\"");
            //RunSourceCommand.SetWriter(logFile, config.Get("Log/@option").zTextDeserialize(FileOption.None));
            RunSourceCommand.TraceSetWriter(WriteToFile.Create(logFile, config.Get("Log/@option").zTextDeserialize(FileOption.None)));

            AppData.DataDirectory = config.GetExplicit("DataDir");
            //string httpExportDirectory = config.Get("HttpExportDirectory").zRootPath(zapp.GetEntryAssemblyDirectory());
            //if (httpExportDirectory != null)
            //{
            //    //Trace.WriteLine($"http export directory \"{httpExportDirectory}\"");
            //    HttpRun.HttpManager.UrlCache = new UrlCache(httpExportDirectory);
            //    HttpRun.HttpManager.UrlCache.UrlFileNameType = UrlFileNameType.Host | UrlFileNameType.Path | UrlFileNameType.Ext | UrlFileNameType.Query;
            //    HttpRun.HttpManager.UrlCache.IndexedFile = true;
            //    HttpRun.HttpManager.UrlCache.SaveRequest = true;
            //}
            HttpRun.HttpManager.UrlCache = UrlCache.CreateIndexedCache(config.Get("HttpExportDirectory").zRootPath(zapp.GetEntryAssemblyDirectory()));
            MongoLog.CurrentMongoLog.SetLogFile(config.Get("MongoLog").zRootPath(zapp.GetEntryAssemblyDirectory()), config.Get("MongoLog/@option").zTextDeserialize(FileOption.None));
            string mongoCache = config.Get("MongoCache").zRootPath(zapp.GetEntryAssemblyDirectory());
            if (mongoCache != null)
                MongoCursorCache.CacheFile = mongoCache;
            //TraceMongoCommand.ResultToGrid = true;
            //DefaultMongoSerialization.SetDefaultMongoSerializationOptions();
            //DefaultMongoSerialization.RegisterDefaultMongoSerializer();
            //HtmlRun.SetResult = dt => RunSource.CurrentRunSource.SetResult(dt);

            //pb.Web.Html.HtmlToXmlManager.Current.ExportXml = true;
        }

        public static void EndAlways()
        {
            //DefaultMongoSerialization.RemoveDefaultMongoSerializationOptions();
            //DefaultMongoSerialization.UnregisterDefaultMongoSerializer();
            //HtmlRun.SetResult = null;
            RunSourceCommand.TraceRemoveWriter();
        }
    }
}
