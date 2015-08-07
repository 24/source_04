using System.Collections.Generic;
using System.IO;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using pb.Data.Mongo;
using pb.IO;
using pb.Web;
using pb.Web.old;

namespace Test.Test_Unit.Web
{
    public class Test_UrlToFileName
    {
        public string url;
        public HttpRequestParameters_v1 httpRequestParameters;
        public UrlFileNameType fileNameType;
        public string ext;
        public string filename;
    }

    public static class Test_Unit_UrlToFileName
    {
        public static void Test()
        {
            Trace.WriteLine("Test_Unit_UrlToFileName");
            string dir = zPath.Combine(RunSource.CurrentRunSource.Config.GetExplicit("TestUnitDirectory"), @"Web\UrlToFileName");
            Test_01(zPath.Combine(dir, "UrlToFileName_01.txt"));
        }

        public static void SetBsonSerializationConventionEnumToString()
        {
            ConventionPack conventions = new ConventionPack();
            conventions.Add(new EnumRepresentationConvention(BsonType.String));
            ConventionRegistry.Register("EnumRepresentationConvention", conventions, t => true);
        }

        //public static List<Test_UrlToFileName> ReadFile(string file)
        //{
        //    SetBsonSerializationConventionEnumToString();
        //    BsonPBSerializationProvider.RegisterProvider();
        //    try
        //    {
        //        return BsonSerializer.Deserialize<List<Test_UrlToFileName>>(zfile.ReadFile(file));
        //    }
        //    finally
        //    {
        //        BsonPBSerializationProvider.UnregisterProvider();
        //    }
        //}

        public static void Test_01(string file)
        {
            bool ok = true;
            Trace.WriteLine("file \"{0}\"", file);
            //foreach (Test_UrlToFileName urlToFileName in ReadFile(file))
            foreach (Test_UrlToFileName urlToFileName in zmongo.BsonReader<Test_UrlToFileName>(file))
            {
                if (urlToFileName.url == "")
                    continue;
                string filename = zurl.UrlToFileName(urlToFileName.url, urlToFileName.fileNameType, urlToFileName.ext, urlToFileName.httpRequestParameters.content);
                bool error = filename != urlToFileName.filename;
                if (error)
                {
                    Trace.Write("error");
                    ok = false;
                }
                else
                    Trace.Write("ok   ");
                Trace.Write(" url \"{0}\"", urlToFileName.url);
                if (urlToFileName.httpRequestParameters != null && urlToFileName.httpRequestParameters.content != null)
                    Trace.Write(" content \"{0}\"", urlToFileName.httpRequestParameters.content);
                Trace.Write("  type \"{0}\"", urlToFileName.fileNameType);
                if (urlToFileName.ext != null)
                    Trace.Write("  ext \"{0}\"", urlToFileName.ext);
                Trace.Write("  filename \"{0}\"", filename);
                if (error)
                    Trace.Write(" should be \"{0}\"", urlToFileName.filename);
                Trace.WriteLine();

                //if (filename != urlToFileName.filename)
                //{
                //    Trace.WriteLine("error url \"{0}\" type \"{1}\" filename \"{2}\" should be \"{3}\"", urlToFileName.url, urlToFileName.fileNameType, filename, urlToFileName.filename);
                //    ok = false;
                //}
                //else
                //    Trace.WriteLine("ok    url \"{0}\" type \"{1}\" filename \"{2}\"", urlToFileName.url, urlToFileName.fileNameType, filename);
            }
            if (ok)
                Trace.WriteLine("test ok");
            else
                Trace.WriteLine("test error");
        }
    }
}
