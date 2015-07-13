using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using pb;
using pb.Data;
using pb.Data.Mongo;

namespace Test.Test_Bson
{
    public static class Test_BsonWriter_f
    {
        public static void Test_BsonWriter_01()
        {
            string bsonFile = @"c:\pib\dev_data\exe\runsource\test\bson.txt";
            StreamWriter sw = null;
            BsonWriter bsonWriter = null;
            try
            {
                sw = File.CreateText(bsonFile);
                JsonWriterSettings jsonSettings = new JsonWriterSettings();
                jsonSettings.Indent = true;
                //jsonSettings.NewLineChars = "\r\n";
                //jsonSettings.OutputMode = JsonOutputMode.Shell;
                bsonWriter = JsonWriter.Create(sw, jsonSettings);
                //bsonWriter.WriteString("test");

                bsonWriter.WriteStartDocument();
                bsonWriter.WriteString("test", "test");
                bsonWriter.WriteEndDocument();
                sw.WriteLine();

                bsonWriter.WriteName("test");    // ??? pour éviter l'erreur : WriteString can only be called when State is Value or Initial, not when State is Name (System.InvalidOperationException)
                bsonWriter.WriteStartDocument();
                bsonWriter.WriteString("test", "test");
                bsonWriter.WriteEndDocument();
                sw.WriteLine();

                bsonWriter.WriteName("test");
                bsonWriter.WriteString("test");
                sw.WriteLine();

                bsonWriter.WriteName("test");
                bsonWriter.WriteNull();
                sw.WriteLine();
            }
            finally
            {
                if (bsonWriter != null)
                    bsonWriter.Close();
                if (sw != null)
                    sw.Close();
            }
        }

        public static void Test_BsonWriter_02(string file)
        {
            //zmongo.BsonReader<BsonDocument>(file).Select(doc => doc.zGet("PrintTextValues").zDeserialize<PrintTextValues>()).zForEach(printTextValues => Trace.WriteLine(printTextValues.zToJson()));
            //zmongo.BsonReader<BsonDocument>(file).Select(doc => doc.zGet("PrintTextValues.infos").zDeserialize<NamedValues<ZValue>>()).zForEach(printTextValues => Trace.WriteLine(printTextValues.zToJson()));
            //zmongo.BsonReader<BsonDocument>(file).Select(doc => doc.zGet("PrintTextValues.infos").zDeserialize<Dictionary<string, string>>()).zForEach(printTextValues => Trace.WriteLine(printTextValues.zToJson()));
            //zmongo.BsonReader<BsonDocument>(file).Select(doc => doc.zGet("PrintTextValues").zDeserialize<TestInfos01>()).zForEach(printTextValues => Trace.WriteLine(printTextValues.zToJson()));
            //zmongo.BsonReader<BsonDocument>(file).Select(doc => doc.zGet("PrintTextValues").zDeserialize<TestInfos01>()).zForEach(printTextValues => Trace.WriteLine(printTextValues.infos.zToJson()));
            //zmongo.BsonReader<BsonDocument>(file).Select(doc => doc.zGet("PrintTextValues").zDeserialize<TestInfos02>()).zForEach(printTextValues => Trace.WriteLine(printTextValues.infos.zToJson()));
            zmongo.BsonReader<BsonDocument>(file).Select(doc => doc.zDeserialize<TestInfos02>()).zForEach(printTextValues => Trace.WriteLine(printTextValues.infos.zToJson()));
        }
    }

    [BsonIgnoreExtraElements]
    public class TestInfos01
    {
        public NamedValues<ZValue> infos = new NamedValues<ZValue>();
    }

    [BsonIgnoreExtraElements]
    public class TestInfos02
    {
        public Dictionary<string, string> infos = new Dictionary<string, string>();
    }
}
