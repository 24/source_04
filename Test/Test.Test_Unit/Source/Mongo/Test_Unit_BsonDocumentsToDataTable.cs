using System;
using System.Data;
using System.IO;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using pb.Data;
using pb.Data.Mongo;

namespace Test.Test_Unit.Mongo
{
    public static class Test_Unit_BsonDocumentsToDataTable
    {
        public static void Test()
        {
            Trace.WriteLine("Test_Unit_BsonDocumentsToDataTable");
            string dir = Path.Combine(RunSource.CurrentRunSource.Config.GetExplicit("TestUnitDirectory"), @"Mongo\BsonDocumentsToDataTable");
            Test_01(Path.Combine(dir, "BsonDocumentsToDataTable_01.txt"));
            Test_01(Path.Combine(dir, "BsonDocumentsToDataTable_02.txt"));
            Test_01(Path.Combine(dir, "BsonDocumentsToDataTable_03.txt"));
        }

        public static void Test_01(string file)
        {
            string traceFile = zpath.PathSetFileName(file, Path.GetFileNameWithoutExtension(file) + "_out");
            Trace.CurrentTrace.AddTraceFile(traceFile, LogOptions.RazLogFile);
            try
            {
                Trace.WriteLine("read file \"{0}\"", file);
                int i = 1;
                foreach (BsonDocument document in zmongo.BsonReader<BsonDocument>(file))
                {
                    Trace.WriteLine();
                    Trace.WriteLine("read document no {0}", i++);
                    Trace.WriteLine(document.zToJson());
                    Trace.WriteLine();
                    //Trace.WriteLine("generate DataTable");
                    DataTable dt = document.zToDataTable2();
                    dt.TableName = "table";
                    //Trace.WriteLine(dt.zToJson());

                    // from http://www.codeproject.com/Questions/326223/How-to-convert-datatable-to-xml
                    //StringWriter writer = new StringWriter();
                    //dt.WriteXml(writer, XmlWriteMode.IgnoreSchema, false);
                    //Trace.WriteLine(writer.ToString());
                    //Trace.WriteLine();
                    //TraceDataTable(dt);
                    Trace.WriteLine("table :");
                    Trace.WriteLine(dt.zToString());


                    Trace.WriteLine();
                    //RunSource.CurrentRunSource.SetResult(dt);
                }
            }
            finally
            {
                Trace.CurrentTrace.RemoveTraceFile(traceFile);
            }
        }

        //public static void TraceDataTable(DataTable dt)
        //{
        //    Trace.WriteLine("table :");
        //    //Trace.WriteLine();
        //    int rowNumber = 1;
        //    foreach (DataRow row in dt.Rows)
        //    {
        //        Trace.WriteLine("  row {0}", rowNumber++);
        //        foreach (DataColumn column in dt.Columns)
        //        {
        //            Trace.WriteLine("    {0} : {1}", column.ColumnName, row[column.ColumnName]);
        //        }
        //        //Trace.WriteLine();
        //    }
        //}
    }
}
