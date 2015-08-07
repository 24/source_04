using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using pb;
using pb.Compiler;
using pb.IO;

namespace Bug
{
    static partial class w
    {
        private static RunSource _rs = RunSource.CurrentRunSource;
        private static ITrace _tr = Trace.CurrentTrace;

        public static void Bug_Path_01()
        {
            // bug from GetNewIndexedFileName() in File.cs
            _tr.WriteLine("Bug_Path_01");
            string file = "C:\\pib\\prog\\tools\\runsource\\run\\RunSource_{0:00000}.*";
            _tr.WriteLine("file \"{0}\"", file);
            try
            {
                // zPath.GetDirectoryName(file); génère une exception System.NotSupportedException qui n'est pas capturé par le catch
                // et zPath.GetDirectoryName est correctement exécuté
                //System.NotSupportedException occurred
                //  HResult=-2146233067
                //  Message=The given path's format is not supported.
                //  Source=mscorlib
                //  StackTrace:
                //       at System.Security.Util.StringExpressionSet.CanonicalizePath(String path, Boolean needFullPath)
                //  InnerException: 

                string dir = zPath.GetDirectoryName(file);
                _tr.WriteLine("Path.GetDirectoryName(file) \"{0}\"", dir);
            }
            catch (NotSupportedException ex)
            {
                _tr.WriteLine("Path.GetDirectoryName(file) error NotSupportedException \"{0}\"", ex.Message);
            }
            catch (Exception ex)
            {
                _tr.WriteLine("Path.GetDirectoryName(file) error \"{0}\"", ex.Message);
            }
        }
    }
}
