using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
using pb;
using pb.Data.Mongo;

namespace Test.Test_Bson
{
    public static class Test_Bson_f
    {
        public static void Test_Serialize_Dictionary_01()
        {
            // json Dictionary
            // { "toto1" : "tata1", "toto2" : "tata2" }
            Trace.WriteLine();
            Trace.WriteLine("Test_Serialize_Dictionary_01");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("toto1", "tata1");
            dictionary.Add("toto2", "tata2");
            Trace.WriteLine("Dictionary json :");
            string json = dictionary.ToJson();
            Trace.WriteLine(json);
        }

        public static void Test_Serialize_Dictionary_02(DictionaryRepresentation dictionaryRepresentation)
        {
            // json Dictionary
            // DictionaryRepresentation.Dynamic or DictionaryRepresentation.Document
            // { "toto1" : "tata1", "toto2" : "tata2" }
            // DictionaryRepresentation.ArrayOfArrays
            // [["toto1", "tata1"], ["toto2", "tata2"]]
            // DictionaryRepresentation.ArrayOfDocuments
            // [{ "k" : "toto1", "v" : "tata1" }, { "k" : "toto2", "v" : "tata2" }]
            Trace.WriteLine();
            Trace.WriteLine("Test_Serialize_Dictionary_02");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("toto1", "tata1");
            dictionary.Add("toto2", "tata2");
            //DictionaryRepresentation dictionaryRepresentation = DictionaryRepresentation.ArrayOfDocuments;
            Trace.WriteLine("DictionaryRepresentation : {0}", dictionaryRepresentation);
            Trace.WriteLine("Dictionary json :");
            string json = dictionary.ToJson(new DictionarySerializationOptions(dictionaryRepresentation));
            Trace.WriteLine(json);

            Trace.WriteLine("Deserialize json :");
            Dictionary<string, string> dictionary2 = BsonSerializer.Deserialize<Dictionary<string, string>>(json);
            string json2 = dictionary2.ToJson(new DictionarySerializationOptions(dictionaryRepresentation));
            Trace.WriteLine(json2);
            Trace.WriteLine("comparison of Dictionary json and Deserialize json : {0}", json == json2 ? "identical" : "different");
        }

        public static void Test_Serialize_NameValueCollection_01()
        {
            Trace.WriteLine();
            Trace.WriteLine("Test_Serialize_NameValueCollection_01");

            //if (BsonSerializer.zIsSerializerRegistered(typeof(NameValueCollection)))
            //{
            //    Trace.WriteLine("UnregisterSerializer(typeof(NameValueCollection)");
            //    BsonSerializer.zUnregisterSerializer(typeof(NameValueCollection));
            //}
            //UnregisterNameValueCollectionSerializer();

            NameValueCollection nameValues = new NameValueCollection();
            nameValues.Add("toto1", "tata1");
            nameValues.Add("toto2", "tata2");
            Trace.WriteLine("NameValueCollection json :");
            string json = nameValues.ToJson();
            Trace.WriteLine(json);
        }

        public static void Test_Serialize_NameValueCollection_02(DictionaryRepresentation dictionaryRepresentation)
        {
            Trace.WriteLine();
            Trace.WriteLine("Test_Serialize_NameValueCollection_02");

            //if (BsonSerializer.zIsSerializerRegistered(typeof(NameValueCollection)))
            //{
            //    Trace.WriteLine("UnregisterSerializer(typeof(NameValueCollection)");
            //    BsonSerializer.zUnregisterSerializer(typeof(NameValueCollection));
            //}
            //if (!BsonSerializer.zIsSerializerRegistered(typeof(NameValueCollection)))
            //{
            //    Trace.WriteLine("RegisterSerializer(typeof(NameValueCollection), new NameValueCollectionSerializer())");
            //    BsonSerializer.RegisterSerializer(typeof(NameValueCollection), new NameValueCollectionSerializer());
            //}

            //RegisterNameValueCollectionSerializer();
            //RegisterBsonPBSerializationProvider();
            BsonPBSerializationProvider.RegisterProvider();

            try
            {
                NameValueCollection nameValues = new NameValueCollection();
                nameValues.Add("toto1", "tata1");
                nameValues.Add("toto2", "tata2");
                Trace.WriteLine("DictionaryRepresentation : {0}", dictionaryRepresentation);
                Trace.WriteLine("NameValueCollection json :");
                string json = nameValues.ToJson(new DictionarySerializationOptions(dictionaryRepresentation));
                Trace.WriteLine(json);

                Trace.WriteLine("Deserialize json :");
                NameValueCollection nameValues2 = BsonSerializer.Deserialize<NameValueCollection>(json);
                string json2 = nameValues2.ToJson(new DictionarySerializationOptions(dictionaryRepresentation));
                Trace.WriteLine(json2);
                Trace.WriteLine("comparison of NameValueCollection json and Deserialize json : {0}", json == json2 ? "identical" : "different");
            }
            finally
            {
                //UnregisterBsonPBSerializationProvider();
                BsonPBSerializationProvider.UnregisterProvider();
            }
        }

        public static void Test_LookupSerializer_01(Type type)
        {
            //Test_LookupSerializer_01(typeof(NameValueCollection))
            Trace.WriteLine();
            Trace.WriteLine("Test_LookupSerializer_01");
            //UnregisterNameValueCollectionSerializer();
            //RegisterBsonPBSerializationProvider();
            BsonPBSerializationProvider.RegisterProvider();

            try
            {
                Trace.WriteLine("BsonSerializer.LookupSerializer(\"{0}\")", type.AssemblyQualifiedName);
                var serializer = BsonSerializer.LookupSerializer(type);
                if (serializer != null)
                    Trace.WriteLine("found serializer : \"{0}\"", serializer.GetType().AssemblyQualifiedName);
                else
                    Trace.WriteLine("serializer not found");
            }
            finally
            {
                //UnregisterBsonPBSerializationProvider();
                BsonPBSerializationProvider.UnregisterProvider();
            }
        }

        public static void Test_NameValueCollection_01()
        {
            Trace.WriteLine();
            Trace.WriteLine("Test_NameValueCollection_01");
            NameValueCollection nameValues = new NameValueCollection();
            nameValues.Add("toto1", "tata1");
            nameValues.Add("toto2", "tata2");
            foreach (var v in nameValues)
                Trace.WriteLine("{0}", v);
            for (int i = 0; i < nameValues.Count; i++)
                Trace.WriteLine("key {0} value {1}", nameValues.GetKey(i), nameValues.Get(i));
        }

        public static void Test_RegisterBsonPBSerializationProvider_01()
        {
            Trace.WriteLine();
            Trace.WriteLine("Test_RegisterBsonPBSerializationProvider_01");
            Test_ViewSerializationProviders_01();
            Trace.WriteLine("RegisterBsonPBSerializationProvider()");
            //RegisterBsonPBSerializationProvider();
            BsonPBSerializationProvider.RegisterProvider();
            Test_ViewSerializationProviders_01();
            Trace.WriteLine("UnregisterBsonPBSerializationProvider()");
            //UnregisterBsonPBSerializationProvider();
            BsonPBSerializationProvider.UnregisterProvider();
            Test_ViewSerializationProviders_01();
        }

        public static void Test_ViewSerializationProviders_01()
        {
            //Trace.WriteLine();
            //Trace.WriteLine("Test_ViewSerializationProviders_01");
            List<IBsonSerializationProvider> serializationProviders = BsonSerializer.GetSerializationProviders();
            Trace.WriteLine("{0} serialization providers", serializationProviders.Count);
            foreach (IBsonSerializationProvider serializationProvider in serializationProviders)
            {
                Trace.WriteLine("serialization provider \"{0}\"", serializationProvider.GetType().AssemblyQualifiedName);
            }
        }

        //public static void RegisterNameValueCollectionSerializer()
        //{
        //    UnregisterNameValueCollectionSerializer();
        //    if (!BsonSerializer.zIsSerializerRegistered(typeof(NameValueCollection)))
        //    {
        //        Trace.WriteLine("BsonSerializer.RegisterSerializer(typeof(NameValueCollection), new NameValueCollectionSerializer())");
        //        BsonSerializer.RegisterSerializer(typeof(NameValueCollection), new NameValueCollectionSerializer());
        //    }
        //}

        //public static void UnregisterNameValueCollectionSerializer()
        //{
        //    if (BsonSerializer.zIsSerializerRegistered(typeof(NameValueCollection)))
        //    {
        //        Trace.WriteLine("BsonSerializer.zUnregisterSerializer(typeof(NameValueCollection))");
        //        BsonSerializer.zUnregisterSerializer(typeof(NameValueCollection));
        //    }
        //}

        //public static void RegisterBsonPBSerializationProvider()
        //{
        //    //UnregisterBsonPBSerializationProvider();
        //    if (!BsonSerializer.zIsSerializationProviderRegistered(typeof(BsonPBSerializationProvider)))
        //    {
        //        Trace.WriteLine("BsonSerializer.RegisterSerializationProvider(new BsonPBSerializationProvider())");
        //        BsonSerializer.RegisterSerializationProvider(new BsonPBSerializationProvider());
        //    }
        //}

        //public static void UnregisterBsonPBSerializationProvider()
        //{
        //    if (BsonSerializer.zIsSerializationProviderRegistered(typeof(BsonPBSerializationProvider)))
        //    {
        //        Trace.WriteLine("BsonSerializer.zUnregisterSerializationProvider(typeof(BsonPBSerializationProvider)");
        //        BsonSerializer.zUnregisterSerializationProvider(typeof(BsonPBSerializationProvider));
        //    }
        //}

        public static void Test_BsonWriter_01()
        {
            Trace.WriteLine();
            Trace.WriteLine("Test_BsonWriter_01");
            //BsonReader reader;
            //BsonBuffer buffer = new BsonBuffer();
            //MemoryStream
            //TextReader
            //TextWriter textWriter;
            StringWriter stringWriter = new StringWriter();
            //StringBuilder stringBuilder = new StringBuilder();
            BsonWriter writer = BsonWriter.Create(stringWriter);
            writer.WriteStartDocument();
            writer.WriteString("toto", "toto value");
            writer.WriteEndDocument();
            Trace.WriteLine(stringWriter.ToString());
        }

        public static void Test_BsonReader_01(string file, Encoding encoding = null)
        {
            //Trace.WriteLine("Test_BsonReader_01");
            Trace.WriteLine("read file \"{0}\"", file);
            //BsonDocument doc = BsonDocument.ReadFrom(file);
            //BsonDocument doc = BsonSerializer.Deserialize<BsonDocument>(zfile.ReadFile(file, encoding));
            BsonDocument doc = zmongo.ReadFileAs<BsonDocument>(file, encoding);
            //var r = zmongo.BsonReader<BsonDocument>(file);
            //Trace.WriteLine("count {0}", r.Count());
            //BsonDocument doc = r.First();
            Trace.WriteLine("/log");
            doc = doc["log"].AsBsonDocument;
            foreach (var element in doc.Elements)
            {
                Trace.WriteLine("  {0}", element.Name);
            }
            Trace.WriteLine("/log/entries");
            BsonArray array = doc["entries"].AsBsonArray;
            int i = 1;
            foreach (BsonValue value in array)
            {
                Trace.WriteLine("  {0} - {1}", i++, value.BsonType);
                string name;
                doc = value.AsBsonDocument;
                name = "startedDateTime"; Trace.WriteLine("    \"{0}\": {1}", name, doc[name]);
                name = "time"; Trace.WriteLine("    \"{0}\": {1}", name, doc[name]);

                //name = "request"; Trace.WriteLine("    \"{0}\": {1}", name, doc[name]);
                BsonDocument doc2 = doc["request"].AsBsonDocument;
                name = "method"; Trace.WriteLine("    \"request.{0}\": {1}", name, doc2[name]);
                name = "url"; Trace.WriteLine("    \"request.{0}\": {1}", name, doc2[name]);
                name = "httpVersion"; Trace.WriteLine("    \"request.{0}\": {1}", name, doc2[name]);
                // request.headers
                // request.queryString
                // request.cookies
                name = "headersSize"; Trace.WriteLine("    \"request.{0}\": {1}", name, doc2[name]);
                name = "bodySize"; Trace.WriteLine("    \"request.{0}\": {1}", name, doc2[name]);

                //name = "response"; Trace.WriteLine("    \"{0}\": {1}", name, doc[name]);
                doc2 = doc["response"].AsBsonDocument;
                name = "status"; Trace.WriteLine("    \"response.{0}\": {1}", name, doc2[name]);
                name = "statusText"; Trace.WriteLine("    \"response.{0}\": {1}", name, doc2[name]);
                name = "httpVersion"; Trace.WriteLine("    \"response.{0}\": {1}", name, doc2[name]);
                // response.headers
                // response.cookies
                // response.content
                name = "redirectURL"; Trace.WriteLine("    \"response.{0}\": {1}", name, doc2[name]);
                name = "headersSize"; Trace.WriteLine("    \"response.{0}\": {1}", name, doc2[name]);
                name = "bodySize"; Trace.WriteLine("    \"response.{0}\": {1}", name, doc2[name]);
                // cache
                // timings
                name = "connection"; Trace.WriteLine("    \"{0}\": {1}", name, doc.GetValue(name, null));
                name = "pageref"; Trace.WriteLine("    \"{0}\": {1}", name, doc[name]);
                break;
            }
        }
    }

}
