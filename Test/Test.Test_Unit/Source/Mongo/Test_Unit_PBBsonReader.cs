using System;
using System.IO;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using pb.Data.Mongo;

namespace Test.Test_Unit.Mongo
{
    public static class Test_Unit_PBBsonReader
    {
        public static void Test_TextReader()
        {
            string dir = Path.Combine(RunSource.CurrentRunSource.Config.GetExplicit("TestUnitDirectory"), @"Mongo\PBBsonReader");
            string file = Path.Combine(dir, "PBBsonReader_01.txt");
            Test_TextReader_01(file, traceLevel: 0);
            Test_TextReader_01(file, traceLevel: 1);
            Test_TextReader_01(file, traceLevel: 2);
        }

        public static void Test_CloneTextReader(string dir)
        {
            Test_CloneTextReader_01(Path.Combine(dir, @"PBBsonReader\PBBsonReader_01.txt"));
        }

        public static void Test_BookmarkTextReader(string dir)
        {
            Test_BookmarkTextReader_01(Path.Combine(dir, @"PBBsonReader\PBBsonReader_01.txt"));
        }

        public static void Test_BinaryReader(string dir)
        {
            Test_BinaryReader_01(Path.Combine(dir, @"PBBsonReader\PBBsonReader_01.txt"));
        }

        public static void Test_CloneBinaryReader(string dir)
        {
            Test_CloneBinaryReader_01(Path.Combine(dir, @"PBBsonReader\PBBsonReader_01.txt"));
        }

        public static void Test_BookmarkBinaryReader(string dir)
        {
            Test_BookmarkBinaryReader_01(Path.Combine(dir, @"PBBsonReader\PBBsonReader_01.txt"));
        }

        public static void Test_BinaryReader_01(string file)
        {
            // test example using BsonBinaryReader : MongoDB.BsonUnitTests.Jira.CSharp71Tests.Test20KDocument()

            string traceFile = zpath.PathSetFileName(file, Path.GetFileNameWithoutExtension(file) + "_out_bin");
            Trace.WriteLine("Test.Test_Unit.Test_Unit_PBBsonReader.Test_BinaryReader_01()");
            Trace.WriteLine("trace to file \"{0}\"", traceFile);
            Trace.WriteLine();
            Trace.CurrentTrace.DisableBaseLog();
            Trace.CurrentTrace.AddTraceFile(traceFile, LogOptions.RazLogFile);
            try
            {
                Trace.WriteLine("Test.Test_Unit.Test_Unit_PBBsonReader.Test_BinaryReader_01()");
                Trace.WriteLine();
                Trace.WriteLine("read file \"{0}\"", file);
                Trace.WriteLine();
                PBBsonReader.TraceLevel = 0;

                int i = 1;
                foreach (BsonValue bsonValue in zmongo.BsonReader<BsonValue>(file))
                {
                    Trace.WriteLine("read no {0} type {1}", i++, bsonValue.BsonType.ToString());
                    Trace.WriteLine();
                    byte[] bson = bsonValue.ToBson();

                    PBBsonReader reader = new PBBsonReader(BsonReader.Create(new MemoryStream(bson)));
                    int pos = 1;
                    while (reader.Read())
                    {
                        TracePBBsonReader(reader, pos++);
                    }

                    Trace.WriteLine();
                }
            }
            finally
            {
                Trace.CurrentTrace.EnableBaseLog();
                Trace.CurrentTrace.RemoveTraceFile(traceFile);
            }
        }

        public static void Test_CloneBinaryReader_01(string file)
        {
            string traceFile = zpath.PathSetFileName(file, Path.GetFileNameWithoutExtension(file) + "_out_bin_clone");
            Trace.WriteLine("Test.Test_Unit.Test_Unit_PBBsonReader.Test_CloneBinaryReader_01()");
            Trace.WriteLine("trace to file \"{0}\"", traceFile);
            Trace.WriteLine();
            Trace.CurrentTrace.DisableBaseLog();
            Trace.CurrentTrace.AddTraceFile(traceFile, LogOptions.RazLogFile);
            try
            {
                Trace.WriteLine("Test.Test_Unit.Test_Unit_PBBsonReader.Test_CloneBinaryReader_01()");
                Trace.WriteLine();
                Trace.WriteLine("read file \"{0}\"", file);
                Trace.WriteLine();
                PBBsonReader.TraceLevel = 0;

                int i = 1;
                foreach (BsonValue bsonValue in zmongo.BsonReader<BsonValue>(file))
                {
                    Trace.WriteLine("read no {0} type {1}", i++, bsonValue.BsonType.ToString());
                    Trace.WriteLine();
                    byte[] bson = bsonValue.ToBson();

                    PBBsonReader reader = new PBBsonReader(BsonReader.Create(new MemoryStream(bson)));
                    PBBsonReader readerClone1 = null;
                    int pos1 = 0;
                    int pos = 1;
                    while (reader.Read())
                    {
                        TracePBBsonReader(reader, pos++);

                        if (pos == 5)
                        {
                            Trace.WriteLine("**** create clone 1 at {0}", pos);
                            readerClone1 = reader.Clone();
                            pos1 = pos;
                        }
                    }

                    Trace.WriteLine();
                    Trace.WriteLine("read clone 1");
                    reader = readerClone1;
                    pos = pos1;
                    // Error : Not enough input bytes available. Needed 1, but only 0 are available (at position 278). (System.IO.EndOfStreamException)
                    // at MongoDB.Bson.IO.ByteArrayBuffer.EnsureDataAvailable(Int32 needed) in c:\pib\prog\dev\mongodb\driver\CSharpDriver-1.9.2\source\MongoDB.Bson\IO\ByteArrayBuffer.cs:line 522
                    while (reader.Read())
                    {
                        TracePBBsonReader(reader, pos++);
                    }

                    Trace.WriteLine();
                }
            }
            finally
            {
                Trace.CurrentTrace.EnableBaseLog();
                Trace.CurrentTrace.RemoveTraceFile(traceFile);
            }
        }

        public static void Test_BookmarkBinaryReader_01(string file)
        {
            string traceFile = zpath.PathSetFileName(file, Path.GetFileNameWithoutExtension(file) + "_out_bin_bookmark");
            Trace.WriteLine("Test.Test_Unit.Test_Unit_PBBsonReader.Test_BookmarkBinaryReader_01()");
            Trace.WriteLine("trace to file \"{0}\"", traceFile);
            Trace.WriteLine();
            Trace.CurrentTrace.DisableBaseLog();
            Trace.CurrentTrace.AddTraceFile(traceFile, LogOptions.RazLogFile);
            try
            {
                Trace.WriteLine("Test.Test_Unit.Test_Unit_PBBsonReader.Test_BookmarkBinaryReader_01()");
                Trace.WriteLine();
                Trace.WriteLine("read file \"{0}\"", file);
                Trace.WriteLine();
                PBBsonReader.TraceLevel = 0;

                int i = 1;
                foreach (BsonValue bsonValue in zmongo.BsonReader<BsonValue>(file))
                {
                    Trace.WriteLine("read no {0} type {1}", i++, bsonValue.BsonType.ToString());
                    Trace.WriteLine();
                    byte[] bson = bsonValue.ToBson();

                    PBBsonReader reader = new PBBsonReader(BsonReader.Create(new MemoryStream(bson)));
                    PBBsonReaderBookmark bookmark = null;
                    int pos1 = 0;
                    int pos = 1;
                    while (reader.Read())
                    {
                        TracePBBsonReader(reader, pos++);

                        if (pos == 5)
                        {
                            Trace.WriteLine("**** create bookmark 1 at {0}", pos);
                            bookmark = reader.GetBookmark();
                            pos1 = pos;
                        }
                    }

                    Trace.WriteLine();
                    Trace.WriteLine("read bookmark 1");
                    reader.ReturnToBookmark(bookmark);
                    pos = pos1;
                    while (reader.Read())
                    {
                        TracePBBsonReader(reader, pos++);
                    }

                    Trace.WriteLine();
                }
            }
            finally
            {
                Trace.CurrentTrace.EnableBaseLog();
                Trace.CurrentTrace.RemoveTraceFile(traceFile);
            }
        }

        public static void Test_TextReader_01(string file, int traceLevel = 0)
        {
            FileStream fileStream = null;

            string traceFile = zpath.PathSetFileName(file, Path.GetFileNameWithoutExtension(file) + "_out_text_trace_" + traceLevel.ToString());
            Trace.WriteLine("Test.Test_Unit.Test_Unit_PBBsonReader.Test_TextReader_01()");
            Trace.WriteLine("trace to file \"{0}\"", traceFile);
            Trace.WriteLine();
            Trace.CurrentTrace.DisableBaseLog();
            Trace.CurrentTrace.AddTraceFile(traceFile, LogOptions.RazLogFile);
            try
            {
                Trace.WriteLine("Test.Test_Unit.Test_Unit_PBBsonReader.Test_TextReader_01()");
                Trace.WriteLine();
                Trace.WriteLine("read file \"{0}\"", file);
                Trace.WriteLine("trace level {0}", traceLevel);
                Trace.WriteLine();

                PBBsonReader.TraceLevel = traceLevel;

                PBBsonReader reader = new PBBsonReader(BsonReader.Create(new StreamReader(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.UTF8)));
                int pos = 1;
                while (reader.Read())
                {
                    if (traceLevel == 0)
                    {
                        TracePBBsonReader(reader, pos++);
                    }
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

        public static void Test_CloneTextReader_01(string file)
        {
            FileStream fileStream = null;

            string traceFile = zpath.PathSetFileName(file, Path.GetFileNameWithoutExtension(file) + "_out_text_clone");
            Trace.WriteLine("Test.Test_Unit.Test_Unit_PBBsonReader.Test_CloneTextReader_01()");
            Trace.WriteLine("trace to file \"{0}\"", traceFile);
            Trace.WriteLine();
            Trace.CurrentTrace.DisableBaseLog();
            Trace.CurrentTrace.AddTraceFile(traceFile, LogOptions.RazLogFile);
            try
            {
                Trace.WriteLine("Test.Test_Unit.Test_Unit_PBBsonReader.Test_CloneTextReader_01()");
                Trace.WriteLine();
                Trace.WriteLine("read file \"{0}\"", file);
                Trace.WriteLine();

                PBBsonReader.TraceLevel = 0;

                PBBsonReader reader = new PBBsonReader(BsonReader.Create(new StreamReader(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.UTF8)));
                PBBsonReader readerClone1 = null;
                int pos1 = 0;
                int pos = 1;
                while (reader.Read())
                {
                    TracePBBsonReader(reader, pos++);

                    if (pos == 5)
                    {
                        Trace.WriteLine("**** create clone 1 at {0}", pos);
                        readerClone1 = reader.Clone();
                        pos1 = pos;
                    }
                }

                Trace.WriteLine();
                Trace.WriteLine("read clone 1");
                reader = readerClone1;
                pos = pos1;
                while (reader.Read())
                {
                    TracePBBsonReader(reader, pos++);
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

        public static void Test_BookmarkTextReader_01(string file)
        {
            FileStream fileStream = null;

            string traceFile = zpath.PathSetFileName(file, Path.GetFileNameWithoutExtension(file) + "_out_text_bookmark");
            Trace.WriteLine("Test.Test_Unit.Test_Unit_PBBsonReader.Test_BookmarkTextReader_01()");
            Trace.WriteLine("trace to file \"{0}\"", traceFile);
            Trace.WriteLine();
            Trace.CurrentTrace.DisableBaseLog();
            Trace.CurrentTrace.AddTraceFile(traceFile, LogOptions.RazLogFile);
            try
            {
                Trace.WriteLine("Test.Test_Unit.Test_Unit_PBBsonReader.Test_BookmarkTextReader_01()");
                Trace.WriteLine();
                Trace.WriteLine("read file \"{0}\"", file);
                Trace.WriteLine();

                PBBsonReader.TraceLevel = 0;

                PBBsonReader reader = new PBBsonReader(BsonReader.Create(new StreamReader(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.UTF8)));
                PBBsonReaderBookmark bookmark = null;
                int pos1 = 0;
                int pos = 1;
                while (reader.Read())
                {
                    TracePBBsonReader(reader, pos++);
                    if (pos == 5)
                    {
                        Trace.WriteLine("**** create bookmark 1 at {0}", pos);
                        bookmark = reader.GetBookmark();
                        pos1 = pos;
                    }
                }

                Trace.WriteLine();
                Trace.WriteLine("read bookmark 1");
                reader.ReturnToBookmark(bookmark);
                pos = pos1;
                while (reader.Read())
                {
                    TracePBBsonReader(reader, pos++);
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

        public static void TracePBBsonReader(PBBsonReader reader, int pos)
        {
            Trace.Write("{0} - ", pos);
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
}
