using System;
using System.IO;
using System.Text;
using MongoDB.Bson.IO;
using pb.Data.Mongo;
using pb.IO;

namespace Test.Test_Unit.Mongo
{
    public static class Test_Unit_PBBsonEnumerateValues
    {
        public static void Test()
        {
            Trace.WriteLine("Test_Unit_PBBsonEnumerateValues");
            string dir = zPath.Combine(RunSource.CurrentRunSource.Config.GetExplicit("TestUnitDirectory"), @"Mongo\PBBsonEnumerateValues");
            Test_01(zPath.Combine(dir, "PBBsonEnumerateValues_01.txt"));
        }

        public static void Test_01(string file)
        {
            FileStream fileStream = null;

            string traceFile = zpath.PathSetFileName(file, zPath.GetFileNameWithoutExtension(file) + "_out");
            Trace.WriteLine("Test.Test_Unit.Test_Unit_PBBsonEnumerateValues.Test_01()");
            Trace.WriteLine("trace to file \"{0}\"", traceFile);
            Trace.WriteLine();
            Trace.CurrentTrace.DisableBaseLog();
            Trace.CurrentTrace.AddTraceFile(traceFile, LogOptions.RazLogFile);
            try
            {
                Trace.WriteLine("Test.Test_Unit.Test_Unit_PBBsonEnumerateValues.Test_01()");
                Trace.WriteLine();
                Trace.WriteLine("read file \"{0}\"", file);
                Trace.WriteLine();
                foreach (PBBsonNamedValue value in new PBBsonEnumerateValues(new PBBsonReader(BsonReader.Create(new StreamReader(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.UTF8)))))
                {
                    Trace.WriteLine("{0}: {1} ({2})", value.Name, value.Value, value.Value.BsonType);
                }
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
                Trace.CurrentTrace.EnableBaseLog();
                Trace.CurrentTrace.RemoveTraceFile(traceFile);
            }
        }
    }
}
