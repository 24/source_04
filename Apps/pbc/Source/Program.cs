using pb;
using pb.Compiler;
using System;

namespace pbc
{
    class Program
    {
        private static int __traceLevel = 1;
        private static string __projectFile = null;

        static void Main(string[] args)
        {
            try
            {
                Trace.CurrentTrace.AddOnWrite("console", text => Console.Write(text));
                if (!GetArguments(args))
                {
                    Help();
                    return;
                }
                //Trace.WriteLine($"args.Length {args.Length}");
                Compiler.TraceLevel = __traceLevel;
                Compile.CompileProject(__projectFile);
            }
            catch (Exception ex)
            {
                Trace.CurrentTrace.WriteError(ex);
            }
        }

        private static bool GetArguments(string[] args)
        {
            bool ret = true;
            if (args.Length == 0)
            {
                Trace.WriteLine("missing project file");
                ret = false;
            }
            else
            {
                string projectFile = args[0];
                if (args[0] == "--trace")
                {
                    __traceLevel = 2;
                    if (args.Length == 1)
                    {
                        Trace.WriteLine("missing project file");
                        ret = false;
                    }
                    else
                        projectFile = args[1];
                }
                if (ret)
                    __projectFile = projectFile;
            }
            return ret;
        }

        private static void Help()
        {
            // 0 no message, 1 default messages, 2 detailled messaged
            Trace.WriteLine("usage : pbc.exe [--trace] project");
            Trace.WriteLine(@"example : pbc.exe --trace c:\...\test.project.xml");
        }
    }
}
