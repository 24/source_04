using System;
using System.IO;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using pb.Data.Mongo;
using pb.IO;

namespace Test.Test_Unit.Mongo
{
    public static class Test_Unit_BsonReader
    {
        public static void Test()
        {
            Trace.WriteLine("Test_Unit_BsonReader");
            string dir = zPath.Combine(RunSource.CurrentRunSource.Config.GetExplicit("TestUnitDirectory"), @"Mongo\BsonReader");
            //Test_01(zPath.Combine(dir, "BsonReader_01.txt"));
            Test_01(zPath.Combine(dir, "BsonReader_02.txt"));
            //Test_BsonWriter_01(zPath.Combine(dir, "BsonWriter_01.txt"));
        }

        public static void Test_01(string file)
        {
            string traceFile = zpath.PathSetFileName(file, zPath.GetFileNameWithoutExtension(file) + "_out");
            Trace.CurrentTrace.AddTraceFile(traceFile, LogOptions.RazLogFile);
            try
            {
                Trace.WriteLine("read file \"{0}\"", file);
                Trace.WriteLine();
                int i = 1;
                foreach (BsonValue bsonValue in zmongo.BsonReader<BsonValue>(file))
                {
                    Trace.WriteLine("read no {0} type {1}", i++, bsonValue.BsonType.ToString());
                    Trace.WriteLine(bsonValue.zToJson());
                    Trace.WriteLine();
                }
            }
            finally
            {
                Trace.CurrentTrace.RemoveTraceFile(traceFile);
            }
        }

        //public static void Test_CloneReader_01(string file)
        //{
        //    string traceFile = zpath.PathSetFile(file, zPath.GetFileNameWithoutExtension(file) + "_out");
        //    Trace.CurrentTrace.AddTraceFile(traceFile, LogOptions.RazLogFile);
        //    FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
        //    try
        //    {
        //        Trace.WriteLine("read file \"{0}\"", file);
        //        Trace.WriteLine();
        //        BsonReader reader = MongoDB.Bson.IO.BsonReader.Create(new StreamReader(fileStream, Encoding.UTF8));
        //        int i = 1;
        //        foreach (BsonValue bsonValue in zmongo.BsonReader<BsonValue>(file))
        //        {
        //            Trace.WriteLine("read no {0} type {1}", i++, bsonValue.BsonType.ToString());
        //            Trace.WriteLine(bsonValue.zToJson());
        //            Trace.WriteLine();
        //        }

        //        try
        //        {
        //            while (reader.ReadBsonType() != BsonType.EndOfDocument)
        //            {
        //                yield return BsonSerializer.Deserialize<T>(reader);
        //            }
        //        }
        //        finally
        //        {
        //        }





        //    }
        //    finally
        //    {
        //        Trace.CurrentTrace.RemoveTraceFile(traceFile);
        //        fileStream.Close();
        //    }
        //}

        public static void Test_BsonWriter_01(string file)
        {
            // MongoDB Extended JSON http://docs.mongodb.org/manual/reference/mongodb-extended-json/
            Trace.CurrentTrace.AddTraceFile(file, LogOptions.RazLogFile);
            try
            {
                Trace.WriteLine("Test_BsonWriter_01");
                StringWriter stringWriter = new StringWriter();
                JsonWriterSettings jsonWriterSettings = new JsonWriterSettings();
                jsonWriterSettings.Indent = true;
                //jsonWriterSettings.OutputMode = JsonOutputMode.
                BsonWriter writer = BsonWriter.Create(stringWriter, jsonWriterSettings);
                //writer.WriteInt32("Int32", 1234);
                //writer.WriteInt32("Int32", 1234);
                //writer.WriteInt64(123456789);
                //writer.WriteDouble(123.456);
                //writer.WriteBoolean(true);
                //writer.WriteBoolean(false);
                //writer.WriteBytes(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09 });
                ////BsonDateTime dt = ;
                //writer.WriteDateTime(new BsonDateTime(DateTime.Now).ToInt64());
                writer.WriteStartDocument();
                writer.WriteString("String", "toto tata tutu");
                writer.WriteInt32("Int32", 1234);
                writer.WriteInt64("Int64", 123456789);
                writer.WriteDouble("Double", 123.456);
                writer.WriteBoolean("Boolean", true);
                writer.WriteBoolean("Boolean", false);
                writer.WriteBytes("bytes", new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09 });
                writer.WriteDateTime("DateTime", new BsonDateTime(DateTime.Now).MillisecondsSinceEpoch);
                writer.WriteJavaScript("JavaScript", "code JavaScript");
                //writer.WriteJavaScriptWithScope("JavaScriptWithScope", "code JavaScript");
                writer.WriteMinKey("MinKey");
                writer.WriteMaxKey("MaxKey");
                writer.WriteNull("Null");
                writer.WriteObjectId("ObjectId", new ObjectId());
                writer.WriteRegularExpression("RegularExpression", new BsonRegularExpression("pattern", "options"));
                writer.WriteSymbol("Symbol", "symbol");
                writer.WriteTimestamp("Timestamp", 1);
                writer.WriteUndefined("Undefined");
                writer.WriteEndDocument();
                Trace.WriteLine(stringWriter.ToString());
            }
            finally
            {
                Trace.CurrentTrace.RemoveTraceFile(file);
            }
        }

        //public static void Test_01_old(string file)
        //{
        //    string traceFile = zpath.PathSetFile(file, zPath.GetFileNameWithoutExtension(file) + "_out");
        //    Trace.CurrentTrace.AddTraceFile(traceFile, LogOptions.RazLogFile);
        //    try
        //    {
        //        Trace.WriteLine("read file \"{0}\"", file);
        //        Trace.WriteLine();
        //        using (StreamReader streamReader = new StreamReader(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.UTF8))
        //        {
        //            BsonReader reader = BsonReader.Create(streamReader);
        //            int i = 1;
        //            while (true)
        //            //while (reader.State != BsonReaderState.Done)
        //            {
        //                BsonType bsonType = reader.ReadBsonType();
        //                if (bsonType == BsonType.EndOfDocument)
        //                    break;
        //                Trace.WriteLine("read no {0} type {1}", i++, bsonType.ToString());
        //                //reader.r
        //                //if (bsonType != BsonType.Document)
        //                //    break;
        //                //Trace.WriteLine("document no {0}", i++);
        //                //BsonDocument document = BsonSerializer.Deserialize<BsonDocument>(reader);
        //                //Trace.WriteLine(document.zToJson());
        //                BsonValue bsonValue = BsonSerializer.Deserialize<BsonValue>(reader);
        //                Trace.WriteLine(bsonValue.zToJson());
        //                Trace.WriteLine();
        //            }
        //        }
        //    }
        //    finally
        //    {
        //        Trace.CurrentTrace.RemoveTraceFile(traceFile);
        //    }
        //}
    }
}
