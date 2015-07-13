using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization;
using pb;
using pb.Compiler;
using pb.Data;
using pb.Data.Xml;
using pb.IO;

namespace Print.download_test
{
    static partial class w
    {
        //private static RunSource _rs = RunSource.CurrentRunSource;

        public static void Init()
        {
            //_rs.InitConfig("download");
            XmlConfig.CurrentConfig = new XmlConfig(RunSource.CurrentRunSource.GetFilePath("download.config.xml"));

            //string log = XmlConfig.CurrentConfig.Get("Log").zRootPath(zapp.GetEntryAssemblyDirectory());
            //if (log != null)
            //    Trace.CurrentTrace.SetLogFile(log, LogOptions.IndexedFile);
            Trace.CurrentTrace.SetWriter(XmlConfig.CurrentConfig.Get("Log").zRootPath(zapp.GetAppDirectory()), XmlConfig.CurrentConfig.Get("Log/@option").zTextDeserialize(FileOption.None));
        }

        public static void End()
        {
        }

        public static void Test_Download_01()
        {
            Trace.WriteLine("Test_Download_01");
        }

        public static void Test_ClassMap_01()
        {
            Trace.WriteLine("Test_ClassMap_01");
            Trace.WriteLine();
            //Test_LookupClassMap_01(typeof(RapideDdl_Base));
            //Test_LookupClassMap_01(typeof(RapideDdl_Base2222));
            Test_IsClassMapRegistered_01(typeof(RapideDdl_Base));
            Test_IsClassMapRegistered_01(typeof(RapideDdl_Base2222));
        }

        public static void Test_LookupClassMap_01(Type type)
        {
            BsonClassMap map = BsonClassMap.LookupClassMap(type);
            if (map != null)
                Trace.WriteLine("lookup class map \"{0}\" exist", type);
            else
                Trace.WriteLine("lookup class map \"{0}\" dont exist", type);
        }

        public static void Test_IsClassMapRegistered_01(Type type)
        {
            if (BsonClassMap.IsClassMapRegistered(type))
                Trace.WriteLine("class map \"{0}\" is registered", type);
            else
                Trace.WriteLine("class map \"{0}\" is not registered", type);
        }

        public static void Test_RegisterClassMap_RapideDdl_Base_01()
        {
            Trace.WriteLine("register class map RapideDdl_Base");
            BsonClassMap.RegisterClassMap<RapideDdl_Base>(cm =>
            {
                cm.AutoMap();
                //cm.GetMemberMap(c => c.infos).SetSerializationOptions(DictionarySerializationOptions.ArrayOfDocuments);
            });
        }

        public static void Test_RegisterClassMap_RapideDdl_Base2222_01()
        {
            Trace.WriteLine("register class map RapideDdl_Base2222");
            BsonClassMap.RegisterClassMap<RapideDdl_Base2222>(cm =>
            {
                cm.AutoMap();
                //cm.GetMemberMap(c => c.infos).SetSerializationOptions(DictionarySerializationOptions.ArrayOfDocuments);
            });
        }
    }

    public class RapideDdl_Base2222
    {
    }

    public class RapideDdl_Base
    {
        public string title = null;
        public List<string> description = new List<string>();
        public string language = null;
        public string size = null;
        public int? nbPages = null;
        public NamedValues<ZValue> infos = new NamedValues<ZValue>();
    }
}
