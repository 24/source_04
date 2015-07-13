using System;
using System.Collections.Generic;
using System.IO;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using pb.Web;
//using fastJSON;
using System.Collections.Specialized;
using pb.Data.Mongo;
using pb.Web.old;
using Test.Test_Unit.Web;

namespace Test_Unit
{
    static partial class w
    {
        public static RunSource _rs = RunSource.CurrentRunSource;
        public static string _dataDir = null;

        public static void Init()
        {
            _rs.InitConfig("test_unit");
            //string log = _rs.Config.GetRootSubPath("Log", null);
            string log = _rs.Config.Get("Log").zSetRootDirectory();
            if (log != null)
                Trace.CurrentTrace.SetLogFile(log, LogOptions.IndexedFile);
            _dataDir = _rs.Config.GetExplicit("DataDir");
        }

        public static void End()
        {
        }

        public static void Test_01()
        {
            Trace.WriteLine("toto");
            
        }

        //Test_Unit_UrlToFileName

        public static List<Test_UrlToFileName> Test_GetUrlToFileNameList()
        {
            List<Test_UrlToFileName> urlToFileNameList = new List<Test_UrlToFileName>();
            urlToFileNameList.Add(new Test_UrlToFileName { url = "http://www.reseau-gesat.com/Gesat/", fileNameType = UrlFileNameType.Path, filename = "Gesat.html" });
            urlToFileNameList.Add(new Test_UrlToFileName { url = "http://www.reseau-gesat.com/Gesat/EtablissementList-10-10.html", fileNameType = UrlFileNameType.Path, filename = "Gesat_EtablissementList-10-10.html" });
            urlToFileNameList.Add(new Test_UrlToFileName { url = "http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/annuaire.asp",
                    httpRequestParameters = new HttpRequestParameters_v1 {
                        method = HttpRequestMethod.Post,
                        referer = "http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/annuaire.asp?txtRecherche=Entrez%20le%20nom%20d'une%20entreprise&hiddenValider=true",
                        content = "hiddenValider=true&txtRecherche=&txtRecherche1=&txtRecherche2=1&txtRecherche3=&txtRecherche4="
                    },
                    fileNameType = UrlFileNameType.FileName | UrlFileNameType.Content, filename = "Gesat.html"
                });
            return urlToFileNameList;
        }

        public static void Test_ViewUrlToFileNameList_01()
        {
            Test_GetUrlToFileNameList().zView();
        }

        public static void Test_MongoToJson_01()
        {
            Trace.WriteLine("Test_MongoToJson_01");
            BsonPBSerializationProvider.RegisterProvider();
            try
            {
                string file = Path.Combine(_dataDir, @"Test_Mongo_UrlToFileName_01.txt");
                Test_Unit_UrlToFileName.SetBsonSerializationConventionEnumToString();
                List<Test_UrlToFileName> urlToFileNameList = Test_GetUrlToFileNameList();
                zfile.WriteFile(file, urlToFileNameList.ToJson());
            }
            finally
            {
                BsonPBSerializationProvider.UnregisterProvider();
            }
        }

        public static void Test_MongoDeserialize_01()
        {
            Trace.WriteLine("Test_MongoDeserialize_01");
            BsonPBSerializationProvider.RegisterProvider();
            try
            {
                string file = Path.Combine(_dataDir, @"Test_Mongo_UrlToFileName_01.txt");
                string json = zfile.ReadAllText(file);
                Test_Unit_UrlToFileName.SetBsonSerializationConventionEnumToString();
                List<Test_UrlToFileName> urlToFileNameList = BsonSerializer.Deserialize<List<Test_UrlToFileName>>(json);
                urlToFileNameList.zView();
            }
            finally
            {
                BsonPBSerializationProvider.UnregisterProvider();
            }
        }
    }
}
