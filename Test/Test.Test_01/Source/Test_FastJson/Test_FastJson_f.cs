using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using fastJSON;
using Test.Test_Unit.Web;
using Test_Unit;

namespace Test_FastJson
{
    public static class Test_FastJson
    {
        public static void Test_FastJsonToJsonToObject_01()
        {
            Test_FastJsonToJson_01();
            //Test_FastJsonToObject_01();
        }

        public static void Test_FastJsonToJson_01()
        {
            Trace.WriteLine("Test_FastJsonToJson_01");
            //string file = Path.Combine(w._dataDir, "Test_FastJson_UrlToFileName_01.txt");
            string file = Path.Combine(GetDirectory(), "Test_FastJson_UrlToFileName_01.txt");
            List<Test_UrlToFileName> urlToFileNameList = w.Test_GetUrlToFileNameList();
            zfile.WriteFile(file, JSON.ToJSON(urlToFileNameList));
        }

        public static void Test_FastJson_01()
        {
            Trace.WriteLine("Test_FastJson_01");
            //string file = Path.Combine(w._dataDir, "Test_FastJson_UrlToFileName_01.txt");
            string file = Path.Combine(GetDirectory(), "Test_FastJson_UrlToFileName_01.txt");
            string json = zfile.ReadAllText(file);
            List<Test_UrlToFileName> urlToFileNameList = JSON.ToObject<List<Test_UrlToFileName>>(json);
            urlToFileNameList.zView();
        }

        public static void Test_FastJson_02()
        {
            Trace.WriteLine("Test_FastJson_02");
            //string file = Path.Combine(w._dataDir, "Test_FastJson_02.txt");
            string file = Path.Combine(GetDirectory(), "Test_FastJson_02.txt");
            FastJsonToJson(file, new Test_01 { text = "toto", i = 1 });
            Test_01 test = FastJsonToObject<Test_01>(file);
            test.zView();
        }

        public static void Test_FastJson_Test_01_01()
        {
            // Test_FastJson_01 project : ok
            //   json : {"$types":{"Test_FastJson_01.Test_01, Test_FastJson_01, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null":"1"},"$type":"1","text":"toto","i":1}
            // runsource : error Cannot determine type (System.Exception)
            //   json : {"$types":{"Test_Unit.Test_01, RunSource_00003, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":"1"},"$type":"1","text":"toto","i":1}
            //   at at fastJSON.deserializer.ParseDictionary(Dictionary`2 d, Dictionary`2 globaltypes, Type type, Object input) in C:\pib\dropbox\pbeuz\Dropbox\dev\project\Tools\fastJSON\fastJSON_v2.1.1\fastJSON\JSON.cs:line 528
            //   bug Type.GetType("Test_FastJson.Test_01, RunSource_00011, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null") return null
            Trace.WriteLine("Test_FastJson_Test_01_01");
            Test_01 test1 = new Test_01 { text = "toto", i = 1 };
            Trace.WriteLine("JSON.ToJSON(test1)");
            string json = JSON.ToJSON(test1);
            Trace.WriteLine("json :");
            Trace.WriteLine(json);
            Trace.WriteLine("JSON.ToObject<Test_01>(json)");
            Test_01 test2 = JSON.ToObject<Test_01>(json);
            Trace.WriteLine("test2.text \"{0}\"", test2.text);
            Trace.WriteLine("test2.i    {0}", test2.i);
        }

        public static void Test_FastJson_Test_01_02()
        {
            // Test_FastJson_01 project : ok
            // runsource : ok
            Trace.WriteLine("Test_FastJson_Test_01_02");
            string json = "{\"text\":\"toto\",\"i\":1}";
            Trace.WriteLine("json :");
            Trace.WriteLine(json);
            Trace.WriteLine("JSON.ToObject<Test_01>(json)");
            Test_01 test = JSON.ToObject<Test_01>(json);
            Trace.WriteLine("test.text \"{0}\"", test.text);
            Trace.WriteLine("test.i    {0}", test.i);
        }

        public static void Test_FastJson_GClass_01()
        {
            // ok car GClass2 est dans runsource.dll
            Trace.WriteLine("Test_FastJson_GClass_01");
            GClass2<string, string> gclass1 = new GClass2<string, string> { Value1 = "toto", Value2 = "tata" };
            Trace.WriteLine("JSON.ToJSON(gclass1)");
            string json = JSON.ToJSON(gclass1);
            Trace.WriteLine("json :");
            Trace.WriteLine(json);
            Trace.WriteLine("JSON.ToObject<GClass2<string, string>>(json)");
            GClass2<string, string> gclass2 = JSON.ToObject<GClass2<string, string>>(json);
            Trace.WriteLine("gclass2.Value1 \"{0}\"", gclass2.Value1);
            Trace.WriteLine("gclass2.Value2 \"{0}\"", gclass2.Value1);
        }

        public static void Test_Type_01()
        {
            // bug
            // Type.GetType("Test_FastJson.Test_01, RunSource_00011, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null") return null
            Trace.WriteLine();
            Trace.WriteLine("Test_Type_01");
            Test_01 test = new Test_01 { text = "toto", i = 1 };
            Type type = test.GetType();
            Trace.WriteLine("type.Name                    \"{0}\"", type.Name);
            Trace.WriteLine("type.FullName                \"{0}\"", type.FullName);
            Trace.WriteLine("type.AssemblyQualifiedName   \"{0}\"", type.AssemblyQualifiedName);
            Type type2 = Type.GetType(type.FullName);
            Trace.WriteLine("Type.GetType(\"{0}\")   {1}", type.FullName, type2 == null ? "null" : type2.AssemblyQualifiedName);
            type2 = Type.GetType(type.AssemblyQualifiedName);
            Trace.WriteLine("Type.GetType(\"{0}\")   {1}", type.AssemblyQualifiedName, type2 == null ? "null" : type2.AssemblyQualifiedName);
        }

        public static void Test_FastJson_NameValueCollection_01()
        {
            Trace.WriteLine("Test_FastJson_NameValueCollection_01");
            NameValueCollection nameValues1 = new NameValueCollection();
            nameValues1.Add("toto1", "tata1");
            nameValues1.Add("toto2", "tata2");
            Trace.WriteLine("nameValues1[\"toto1\"]   \"{0}\"", nameValues1["toto1"]);
            Trace.WriteLine("nameValues1[\"toto2\"]   \"{0}\"", nameValues1["toto2"]);
            Trace.WriteLine("JSON.ToJSON(nameValues1)");
            string json = JSON.ToJSON(nameValues1);
            Trace.WriteLine("json :");
            Trace.WriteLine(json);
            Trace.WriteLine("JSON.ToObject<NameValueCollection>(json)");
            NameValueCollection nameValues2 = JSON.ToObject<NameValueCollection>(json);
            Trace.WriteLine("nameValues2[\"toto1\"]   \"{0}\"", nameValues2["toto1"]);
            Trace.WriteLine("nameValues2[\"toto2\"]   \"{0}\"", nameValues2["toto2"]);
        }

        public static void FastJsonToJson(string file, object value)
        {
            zfile.WriteFile(file, JSON.ToJSON(value));
        }

        public static T FastJsonToObject<T>(string file)
        {
            return JSON.ToObject<T>(zfile.ReadAllText(file));
        }

        private static string GetDirectory()
        {
            return RunSource.CurrentRunSource.Config.GetExplicit("TestUnitDirectory");
        }
    }

    public class Test_01
    {
        public string text;
        public int i;
    }
}
