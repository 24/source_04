using System;
using System.IO;
using System.Text;
using pb.Data.Xml;

namespace pb.IO.Test
{
    public static class Test_utf8
    {
        private static string __directory = @"c:\pib\dev_data\exe\runsource\test_unit\pb\IO\Test_utf8";
        //public static void Test_utf8_01()
        //{
        //    Trace.WriteLine("Test_utf8_01");
        //}

        public static void Test_utf8_01()
        {
            Test_utf8_Encoding_UTF8();
            Test_utf8_new_UTF8Encoding_true();
            Test_utf8_new_UTF8Encoding_false();
            Test_utf8_new_UTF8Encoding();
            Test_utf8_File_CreateText();
        }

        public static void Test_utf8_Encoding_UTF8()
        {
            // Encoding.UTF8            : génère un bom
            string file = zPath.Combine(__directory, "test_01_Encoding.UTF8_with_bom.txt");
            Encoding encoding = Encoding.UTF8;
            Test_utf8_StreamWriter(file, Encoding.UTF8);
        }

        public static void Test_utf8_new_UTF8Encoding_true()
        {
            // new UTF8Encoding(true)   : génère un bom
            string file = zPath.Combine(__directory, "test_02_new_UTF8Encoding(true)_with_bom.txt");
            Encoding encoding = new UTF8Encoding(true);
            Test_utf8_StreamWriter(file, encoding);
        }

        public static void Test_utf8_new_UTF8Encoding_false()
        {
            // new UTF8Encoding(false)  : ne génère pas de bom
            string file = zPath.Combine(__directory, "test_03_new_UTF8Encoding(false)_without_bom.txt");
            Encoding encoding = new UTF8Encoding(false);
            Test_utf8_StreamWriter(file, encoding);
        }

        public static void Test_utf8_new_UTF8Encoding()
        {
            // new UTF8Encoding()       : ne génère pas de bom
            string file = zPath.Combine(__directory, "test_04_new_UTF8Encoding()_without_bom.txt");
            Encoding encoding = new UTF8Encoding();
            Test_utf8_StreamWriter(file, encoding);
        }

        public static void Test_utf8_File_CreateText()
        {
            // File.CreateText()        : ne génère pas de bom
            string file = zPath.Combine(__directory, "test_05_File.CreateText()_without_bom.txt");
            zfile.CreateFileDirectory(file);
            if (zFile.Exists(file))
                zFile.Delete(file);
            using (StreamWriter sw = File.CreateText(file))
            {
                WriteTest(sw);
            }
        }

        public static void Test_utf8_StreamWriter(string file, Encoding encoding)
        {
            zfile.CreateFileDirectory(file);
            if (zFile.Exists(file))
                zFile.Delete(file);
            using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Read))
            using (StreamWriter sw = new StreamWriter(fs, encoding))
            {
                WriteTest(sw);
            }
        }

        private static void WriteTest(StreamWriter sw)
        {
            sw.WriteLine("toto");
            sw.WriteLine("éé à ù ç");
            sw.WriteLine("éé à ù ç");
            sw.WriteLine("toto");
        }

        //private static string GetDirectory()
        //{
        //    return zPath.Combine(XmlConfig.CurrentConfig.GetExplicit("TestUnitDirectory"), @"Print\FindPrint");
        //}
    }
}
