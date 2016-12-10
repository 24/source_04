using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using System;
using System.IO;

namespace pb.Data.Mongo.Test
{
    public static class Test_MongoSerializers
    {
        public static void Test_MongoSerializers_01()
        {
            Trace.WriteLine("Test_MongoSerializers");
            "toto".zTraceJson();
        }

        public static void Test_Serialize_01()
        {
            new ZString("toto").zTraceJson();
            DateTime.Now.zTraceJson();
            Date.Today.zTraceJson();
        }

        public static void Test_Serialize_02()
        {
            new Test_ZValue { value1 = "toto", value2 = 123 }.zTraceJson();
        }

        public static void Test_Serialize_03()
        {
            Test_ZValue value = new Test_ZValue { value1 = "toto", value2 = 123 };
            using (StringWriter sw = new StringWriter())
            {
                using (BsonWriter bsonWriter = BsonWriter.Create(sw, new JsonWriterSettings { Indent = true, NewLineChars = "\r\n" }))
                {
                    BsonSerializer.Serialize(bsonWriter, value);
                }
                Trace.WriteLine(sw.ToString());
            }
        }

        public static void Test_Deserialize_01()
        {
            //if (trace)
            //    SetTrace();

            Trace.WriteLine("Test_Deserialize_01 : Deserialize()");
            string json = "{ value1: \"toto\", value2: 123 }";
            Test_ZValue value = BsonSerializer.Deserialize<Test_ZValue>(json);
            Trace.WriteLine("Test_Deserialize_01 : Serialize()");
            value.zTraceJson();
        }

        //public static void SetTrace()
        //{
        //    //PBSerializationProvider_v1.Trace = true;
        //    //PBSerializationProvider_v2.Trace = true;

        //    DateSerializer.Trace = true;
        //    //NameValueCollectionSerializer.Trace = true;
        //    WebHeaderSerializer.Trace = true;

        //    ZValueSerializer.Trace = true;
        //    ZStringSerializer.Trace = true;
        //    ZStringArraySerializer.Trace = true;
        //    ZIntSerializer.Trace = true;

        //    //OXmlElementSerializer.Trace = true;
        //}
    }

    public class Test_ZValue
    {
        public ZValue value1;
        public ZValue value2;
    }
}
