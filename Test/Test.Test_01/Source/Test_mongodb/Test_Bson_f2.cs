using System;
using System.Collections.Generic;
using System.Data;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
using pb;
using pb.Compiler;
using pb.Data;
using pb.Data.Mongo;
using pb.IO;

namespace Test.Test_Bson
{
    public enum Test_Enum_01
    {
        Value1 = 1,
        Value2,
        Value3,
        Value4,
        Value5
    }

    public class Test_Class_Enum_01
    {
        public Test_Enum_01 enum_01;
    }

    public class Test_Bson_Class_01
    {
        public string title = null;
        public List<string> description = new List<string>();
        public int? nbPages = null;
        public NamedValues<ZValue> infos1NamedValuesZValue = new NamedValues<ZValue>();
        public Dictionary<string, ZValue> infos2DictionaryStringZValue = new Dictionary<string, ZValue>();
        public Dictionary<string, BsonValue> infos3DictionaryStringBsonValue = new Dictionary<string, BsonValue>();
    }

    public class Test_Bson_Class_02
    {
        public Dictionary<string, BsonValue> infos1DictionaryStringBsonValue = new Dictionary<string, BsonValue>();
        public Dictionary<string, BsonValue> infos2DictionaryStringBsonValue = new Dictionary<string, BsonValue>();
        public Dictionary<string, BsonValue> infos3DictionaryStringBsonValue = new Dictionary<string, BsonValue>();
    }

    public class Test_DateTime_Class_01
    {
        public DateTime datetime;
    }

    public class Test_StringArray_Class_01
    {
        public string[] stringArray;
    }

    public class Test_ZValue_Class_01
    {
        public ZValue value1;
        public ZValue value2;
        public ZValue value3;
    }

    public class Test_ZValue_Class_02
    {
        public ZString value1;
        public ZStringArray value2;
        public ZInt value3;
    }

    public static class Test_Bson_f2
    {
        public static Test_Bson_Class_01 Create_Test_Bson_Class_01()
        {
            return new Test_Bson_Class_01
            {
                title = "toto",
                description = { "description1", "description2" },
                nbPages = null,
                infos1NamedValuesZValue = { { "infos1_string_key", "infos1_string_value" }, { "infos1_int_key", 1234 } },
                infos2DictionaryStringZValue = { { "infos2_string_key", "infos2_string_value" }, { "infos2_int_key", 1234 } },
                infos3DictionaryStringBsonValue = { { "infos3_string_key", "infos3_string_value" }, { "infos3_int_key", 1234 }, { "infos3_datetime_key", new BsonDateTime(DateTime.Now) } }
            };
        }

        public static Test_Bson_Class_02 Create_Test_Bson_Class_02()
        {
            return new Test_Bson_Class_02
            {
                infos1DictionaryStringBsonValue = { { "infos1_string_key", "infos1_string_value" }, { "infos1_int_key", 1234 }, { "infos1_datetime_key", new BsonDateTime(DateTime.Now) } },
                infos2DictionaryStringBsonValue = { { "infos2_string_key", "infos2_string_value" }, { "infos2_int_key", 1234 }, { "infos2_datetime_key", new BsonDateTime(DateTime.Now) } },
                infos3DictionaryStringBsonValue = { { "infos3_string_key", "infos3_string_value" }, { "infos3_int_key", 1234 }, { "infos3_datetime_key", new BsonDateTime(DateTime.Now) } }
            };
            //System.Diagnostics.Trace.Listeners.Add();
        }

        public static NamedValues<ZValue> Create_NamedValues_ZValue_01()
        {
            return new NamedValues<ZValue> { { "infos1_string_key", "infos1_string_value" }, { "infos1_int_key", 1234 } };
        }

        public static Dictionary<string, ZValue> Create_Dictionary_string_ZValue_01()
        {
            return new Dictionary<string, ZValue> { { "infos2_string_key", "infos2_string_value" }, { "infos2_int_key", 1234 } };
        }

        public static Dictionary<string, BsonValue> Create_Dictionary_string_BsonValue_01()
        {
            return new Dictionary<string, BsonValue> { { "infos3_string_key", "infos3_string_value" }, { "infos3_int_key", 1234 }, { "infos3_datetime_key", new BsonDateTime(DateTime.Now) } };
        }

        //public static Download.Print.RapideDdl.RapideDdl_PostDetail Create_RapideDdl_PostDetail_01()
        //{
        //    return new Download.Print.RapideDdl.RapideDdl_PostDetail
        //    {
        //        sourceUrl = "http://zozo.com",
        //        loadFromWebDate = DateTime.Now,
        //        creationDate = DateTime.Now,
        //        postAuthor = "toto",
        //        title = "tutu",
        //        category = "tata",
        //        language = "titi",
        //        size = "zaza",
        //        nbPages = 5,
        //        description = new string[] { "nana", "nono", "nini" },
        //        infos = new NamedValues<ZValue> { { "gaga", "gogo" }, { "gigi", "gugu" } },
        //        //images = new List<ImageHtml> { new ImageHtml { Source = "http://baba.com/1234" }, new ImageHtml { Source = "http://baba.com/5678" } },
        //        images = new UrlImage[] { new UrlImage("http://baba.com/1234"), new UrlImage("http://baba.com/5678") },
        //        downloadLinks = new string[] { "http://zozo.com/1234", "http://zozo.com/5678" }
        //    };
        //}

        //public static Download.Print.RapideDdl.RapideDdl_PostDetail Create_RapideDdl_PostDetail_02()
        //{
        //    return new Download.Print.RapideDdl.RapideDdl_PostDetail
        //    {
        //        sourceUrl = "http://zozo.com",
        //        loadFromWebDate = DateTime.Now,
        //        creationDate = DateTime.Now,
        //        postAuthor = "toto",
        //        title = "tutu",
        //        category = "tata",
        //        language = "titi",
        //        size = "zaza",
        //        nbPages = 5,
        //        description = new string[] { "nana", "nono", "nini" },
        //        infos = new NamedValues<ZValue> { { "gaga.", "gogo" }, { "gigi", "gugu" } },
        //        //images = new List<ImageHtml> { new ImageHtml { Source = "http://baba.com/1234" }, new ImageHtml { Source = "http://baba.com/5678" } },
        //        images = new UrlImage[] { new UrlImage("http://baba.com/1234"), new UrlImage("http://baba.com/5678") },
        //        downloadLinks = new string[] { "http://zozo.com/1234", "http://zozo.com/5678" }
        //    };
        //}

        public static void RegisterClassMap_Test_Bson_Class_01_01(DictionarySerializationOptions dictionarySerializationOptions)
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(Test_Bson_Class_01)))
            {
                //BsonClassMap map = BsonClassMap.LookupClassMap(typeof(Test_Bson_Class_01));
                //Trace.WriteLine("change existing class map");
                //BsonMemberMap memberMap = map.GetMemberMap("infos1NamedValuesZValue");
                //memberMap.SetSerializationOptions(DictionarySerializationOptions.ArrayOfDocuments);
                Trace.WriteLine("error class map Test_Bson_Class_01 is already registered");
            }
            else
            {
                Trace.WriteLine("register class map Test_Bson_Class_01 : dictionarySerializationOptions {0}", dictionarySerializationOptions);
                BsonClassMap.RegisterClassMap<Test_Bson_Class_01>(cm =>
                {
                    cm.AutoMap();
                    //cm.GetMemberMap(c => c.infos3DictionaryStringBsonValue).SetSerializationOptions(DictionarySerializationOptions.ArrayOfArrays);
                    cm.GetMemberMap(c => c.infos1NamedValuesZValue).SetSerializationOptions(dictionarySerializationOptions);
                    cm.GetMemberMap(c => c.infos2DictionaryStringZValue).SetSerializationOptions(dictionarySerializationOptions);
                    cm.GetMemberMap(c => c.infos3DictionaryStringBsonValue).SetSerializationOptions(dictionarySerializationOptions);
                });
            }
            Trace.WriteLine();
        }

        public static void Test_Serialize_01(DictionarySerializationOptions dictionarySerializationOptions)
        {
            //DictionarySerializationOptions.Document
            //DictionarySerializationOptions.ArrayOfDocuments
            //DictionarySerializationOptions.ArrayOfArrays

            Trace.WriteLine("BsonPBSerializationProvider.RegisterProvider()");
            BsonPBSerializationProvider.RegisterProvider();
            try
            {
                RegisterClassMap_Test_Bson_Class_01_01(dictionarySerializationOptions);
                // DictionarySerializationOptions.Defaults : Create and register a DictionarySerializer with the desired options instead.
                //BsonSerializer.RegisterGenericSerializerDefinition();
                //Dictionary<string, string> dic = new Dictionary<string,string>();
                //new Dictionary<string, string>().GetType().AssemblyQualifiedName;
                //DictionarySerializer
                //DictionarySerializationOptions.Defaults = DictionarySerializationOptions.Document;
                Test_Bson_Class_01 test = Create_Test_Bson_Class_01();
                //BsonSerializer.Serialize<BsonDocument>(test);
                BsonDocument document = test.zToBsonDocument();
                //Trace.WriteLine(test.zToJson());
                Trace.WriteLine(document.zToJson());
                //DataTable dt = document.zToDataTable2();
                //Trace.WriteLine();
                //Trace.WriteLine("table :");
                //Trace.WriteLine(dt.zToString());
                //RunSource.CurrentRunSource.SetResult(dt);


                //BsonDocument document = null;
                BsonValue value = document["toto"];
                //value.IsBsonArray
            }
            finally
            {
                //UnregisterBsonPBSerializationProvider();
                Trace.WriteLine("BsonPBSerializationProvider.UnregisterProvider()");
                BsonPBSerializationProvider.UnregisterProvider();
            }
        }

        public static void Test_Serialize_02(DictionaryRepresentation dictionaryRepresentation)
        {
            Trace.WriteLine("BsonPBSerializationProvider.RegisterProvider()");
            BsonPBSerializationProvider.RegisterProvider();
            PBDictionarySerializer.RegisterGenericDictionarySerializer();
            try
            {
                Trace.WriteLine("set DefaultDictionaryRepresentation to {0}", dictionaryRepresentation);
                PBDictionarySerializer.DefaultDictionaryRepresentation = dictionaryRepresentation;
                Test_Bson_Class_01 test = Create_Test_Bson_Class_01();
                BsonDocument document = test.zToBsonDocument();
                Trace.WriteLine(document.zToJson());
            }
            finally
            {
                Trace.WriteLine("BsonPBSerializationProvider.UnregisterProvider()");
                BsonPBSerializationProvider.UnregisterProvider();
                PBDictionarySerializer.UnregisterGenericDictionarySerializer();
            }
        }

        public static void Test_Serialize_03(DictionaryRepresentation dictionaryRepresentation)
        {
            Trace.WriteLine("BsonPBSerializationProvider.RegisterProvider()");
            BsonPBSerializationProvider.RegisterProvider();
            PBDictionarySerializer.RegisterGenericDictionarySerializer();
            try
            {
                PBDictionarySerializer.DefaultDictionaryRepresentation = dictionaryRepresentation;
                Test_Bson_Class_02 test = Create_Test_Bson_Class_02();
                BsonDocument document = test.zToBsonDocument();
                Trace.WriteLine(document.zToJson());
            }
            finally
            {
                Trace.WriteLine("BsonPBSerializationProvider.UnregisterProvider()");
                BsonPBSerializationProvider.UnregisterProvider();
                PBDictionarySerializer.UnregisterGenericDictionarySerializer();
            }
        }

        public static void Test_Serialize_04(DictionaryRepresentation dictionaryRepresentation)
        {
            Trace.WriteLine("BsonPBSerializationProvider.RegisterProvider()");
            BsonPBSerializationProvider.RegisterProvider();
            PBDictionarySerializer.RegisterGenericDictionarySerializer();
            try
            {
                //PBDictionarySerializer.DefaultDictionaryRepresentation = dictionaryRepresentation;
                DictionarySerializationOptions options = new DictionarySerializationOptions(dictionaryRepresentation);
                Dictionary<string, ZValue> infos2 = Create_Dictionary_string_ZValue_01();
                //BsonDocument document = infos2.zToBsonDocument(options);
                //Trace.WriteLine(document.zToJson());
                Trace.WriteLine("dictionaryRepresentation : {0}", dictionaryRepresentation);
                string json = infos2.ToJson(options);
                Trace.WriteLine(json);
            }
            finally
            {
                Trace.WriteLine("BsonPBSerializationProvider.UnregisterProvider()");
                BsonPBSerializationProvider.UnregisterProvider();
                PBDictionarySerializer.UnregisterGenericDictionarySerializer();
            }
        }

        public static void Test_Serialize_05()
        {
            Test_Serialize_02(DictionaryRepresentation.ArrayOfArrays);
            Test_Serialize_02(DictionaryRepresentation.ArrayOfDocuments);
        }

        public static void Test_Serialize_06(object value)
        {
            //BsonSerializer.Trace = true;
            //BsonSerializer.Trace = false;
            //Trace.WriteLine();
            Trace.WriteLine("Test_Serialize_06 : object {0}", value.GetType().zGetName());
            //Trace.WriteLine();
            Trace.WriteLine("1) value.zToBsonDocument() then document.zToJson()");
            //Download.Print.RapideDdl.RapideDdl_PostDetail post = Create_RapideDdl_PostDetail_01();
            BsonDocument document = value.zToBsonDocument();
            //BsonDocument document = value.zToBsonDocument(value.GetType());
            Trace.WriteLine(document.zToJson());
            Trace.WriteLine();
            Trace.WriteLine("2) value.zToJson()");
            Trace.WriteLine(value.zToJson());
        }

        public static void Test_Serialize_07(object value)
        {
            BsonDocument document = value.ToBsonDocument(value.GetType());
            Trace.WriteLine(document.zToJson());
        }

        public static void Test_Deserialize_01(object value)
        {
            Trace.WriteLine("Serialize {0}", value.GetType().zGetName());
            BsonDocument document = value.ToBsonDocument(value.GetType());
            string json = document.zToJson();
            Trace.WriteLine(json);
            Trace.WriteLine("Deserialize to {0}", value.GetType().zGetName());
            object value2 = BsonSerializer.Deserialize(json, value.GetType());
            document = value.ToBsonDocument(value.GetType());
            Trace.WriteLine(document.zToJson());
        }

        public static void Test_DeserializeFromFile_01(string file, Type type)
        {
            string json = zfile.ReadAllText(file);
            Trace.WriteLine("json");
            Trace.WriteLine(json);
            Trace.WriteLine("Deserialize to {0}", type.zGetName());
            object value = BsonSerializer.Deserialize(json, type);
            BsonDocument document = value.ToBsonDocument(type);
            Trace.WriteLine(document.zToJson());
        }

        public static void Test_BsonReader_01()
        {
            BsonPBSerializationProvider.RegisterProvider();
            try
            {
                Test_Bson_Class_01 test = Create_Test_Bson_Class_01();
                BsonDocument document = test.zToBsonDocument();
                Trace.WriteLine(document.zToJson());
                Trace.WriteLine("start reader");
                //BsonReader reader = BsonReader.Create(document);
                //BsonReader reader = BsonReader.Create(document.zToJson());
                //PBBsonReader reader = new PBBsonReader(document);
                PBBsonReader reader = new PBBsonReader(BsonReader.Create(document));
                while (reader.Read())
                {
                    if (RunSource.CurrentRunSource.IsExecutionAborted())
                        break;
                    if (reader.Type == PBBsonReaderType.Value)
                    {
                        if (reader.Name != null)
                            Trace.Write("{0}: ", reader.Name);
                        Trace.WriteLine("{0} ({1})", reader.Value, reader.Value.BsonType);
                    }
                    else
                        Trace.WriteLine("{0}", reader.Type);
                }
            }
            finally
            {
                //UnregisterBsonPBSerializationProvider();
                BsonPBSerializationProvider.UnregisterProvider();
            }
        }

        public static void Test_PBBsonEnumerateValues_01()
        {
            BsonPBSerializationProvider.RegisterProvider();
            try
            {
                Test_Bson_Class_01 test = Create_Test_Bson_Class_01();
                BsonDocument document = test.zToBsonDocument();
                Trace.WriteLine(document.zToJson());
                Trace.WriteLine("start reader");
                foreach (PBBsonNamedValue value in new PBBsonEnumerateValues(new PBBsonReader(BsonReader.Create(document))))
                {
                    Trace.WriteLine("{0}: {1} ({2})", value.Name, value.Value, value.Value.BsonType);
                }
            }
            finally
            {
                //UnregisterBsonPBSerializationProvider();
                BsonPBSerializationProvider.UnregisterProvider();
            }
        }

        public static void Test_DateTime_01()
        {
            //DateTimeSerializer dateTimeSerializer = new DateTimeSerializer(DateTimeSerializationOptions.LocalInstance);
            //BsonSerializer.RegisterSerializer(typeof(DateTime), dateTimeSerializer);

            //DateTime dt = new DateTime(2014, 8, 16, 22, 0, 0);
            Test_DateTime_Class_01 datetime = new Test_DateTime_Class_01 { datetime = new DateTime(2014, 8, 16, 22, 0, 0) };
            Trace.WriteLine("DateTime {0:dd-MM-yyyy HH:mm:ss K zz}", datetime.datetime);
            BsonDocument document = datetime.ToBsonDocument();
            Trace.WriteLine("DateTime.ToBsonDocument {0}", document.ToJson());
            Test_DateTime_Class_01 datetime2 = BsonSerializer.Deserialize<Test_DateTime_Class_01>(document);
            Trace.WriteLine("DateTime2 {0:dd-MM-yyyy HH:mm:ss K zz}", datetime2.datetime);
            Trace.WriteLine("DateTime == DateTime2 {0}", datetime.datetime == datetime2.datetime);
        }

        public static void Test_StringArray_01()
        {
            Test_StringArray_Class_01 stringArray = new Test_StringArray_Class_01 { stringArray = new string[] { "toto", "tata", "tutu", "titi" } };
            Trace.WriteLine("stringArray {0}", stringArray.stringArray.zToStringValues());
            BsonDocument document = stringArray.ToBsonDocument();
            Trace.WriteLine("stringArray.ToBsonDocument {0}", document.ToJson());
            Test_StringArray_Class_01 stringArray2 = BsonSerializer.Deserialize<Test_StringArray_Class_01>(document);
            Trace.WriteLine("stringArray2 {0}", stringArray2.stringArray.zToStringValues());
        }

        public static void Test_ZValue_01()
        {
            Test_ZValue_Class_01 zvalue = new Test_ZValue_Class_01 { value1 = new ZString("toto"), value2 = new ZStringArray(new string[] { "toto", "tata", "tutu", "titi" }), value3 = new ZInt(123) };
            Trace.WriteLine("zvalue value1 {0} value2 {1} value3 {2}", zvalue.value1, zvalue.value2, zvalue.value3);
            BsonDocument document = zvalue.ToBsonDocument();
            Trace.WriteLine("zvalue.ToBsonDocument {0}", document.ToJson());
            Test_ZValue_Class_01 zvalue2 = BsonSerializer.Deserialize<Test_ZValue_Class_01>(document);
            Trace.WriteLine("zvalue2 value1 {0} value2 {1} value3 {2}", zvalue2.value1, zvalue2.value2, zvalue2.value3);
        }

        public static void Test_ZValue_02()
        {
            Test_ZValue_Class_02 zvalue = new Test_ZValue_Class_02 { value1 = new ZString("toto"), value2 = new ZStringArray(new string[] { "toto", "tata", "tutu", "titi" }), value3 = new ZInt(123) };
            Trace.WriteLine("zvalue value1 {0} value2 {1} value3 {2}", zvalue.value1, zvalue.value2, zvalue.value3);
            BsonDocument document = zvalue.ToBsonDocument();
            Trace.WriteLine("zvalue.ToBsonDocument {0}", document.ToJson());
            Test_ZValue_Class_02 zvalue2 = BsonSerializer.Deserialize<Test_ZValue_Class_02>(document);
            Trace.WriteLine("zvalue2 value1 {0} value2 {1} value3 {2}", zvalue2.value1, zvalue2.value2, zvalue2.value3);
        }

        //public static void Test_RapideDdl_PostDetail_01()
        //{
        //    Download.Print.RapideDdl.RapideDdl_PostDetail post = Create_RapideDdl_PostDetail_02();
        //    Trace.WriteLine("post :");
        //    Trace.WriteLine(post.zToJson());
        //    BsonDocument document = post.ToBsonDocument();
        //    Trace.WriteLine("post.ToBsonDocument {0}", document.ToJson());
        //    Download.Print.RapideDdl.RapideDdl_PostDetail post2 = BsonSerializer.Deserialize<Download.Print.RapideDdl.RapideDdl_PostDetail>(document);
        //    Trace.WriteLine("post2 :");
        //    Trace.WriteLine(post2.zToJson());
        //}

        public static void Test_BsonElement_01()
        {
            //BsonElement[] elements = new BsonElement[] { { "", "" } };
            BsonDocument document = new BsonDocument { { "name", "value" } };
        }

        public static void Test_EnumAsString_01(bool setEnumConvention = false)
        {
            if (setEnumConvention)
            {
                ConventionPack conventions = new ConventionPack();
                conventions.Add(new EnumRepresentationConvention(BsonType.String));
                ConventionRegistry.Register("EnumRepresentationConvention", conventions, t => true);
            }
            try
            {
                Test_Class_Enum_01 enum_01 = new Test_Class_Enum_01 { enum_01 = Test_Enum_01.Value3 };
                Trace.WriteLine("enum_01 enum_01 {0}", enum_01.enum_01);
                BsonDocument document = enum_01.ToBsonDocument();
                Trace.WriteLine("enum_01.ToBsonDocument {0}", document.ToJson());
                Test_Class_Enum_01 enum_02 = BsonSerializer.Deserialize<Test_Class_Enum_01>(document);
                Trace.WriteLine("enum_02 enum_01 {0}", enum_02.enum_01);
            }
            finally
            {
                if (setEnumConvention)
                    ConventionRegistry.Remove("EnumRepresentationConvention");
            }
        }

        public static void Test_EnumDerialization_01(string json)
        {
            Trace.WriteLine("json \"{0}\"", json);
            Test_Class_Enum_01 enum_01 = BsonSerializer.Deserialize<Test_Class_Enum_01>(json);
            Trace.WriteLine("enum_01 enum_01 {0}", enum_01.enum_01);
        }
    }
}
