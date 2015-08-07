using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using pb;
using pb.Data.Xml;
using pb.IO;
using Print;

namespace pt1
{
    class Program
    {
        private static string _cmd;
        private static string[] _args;
        private static string[] _options;
        //private static ITrace _tr = Trace.CurrentTrace;
        private static XmlConfig _config = XmlConfig.CurrentConfig;
        //private static XmlConfig _config = new XmlConfig("pt_config.xml");

        public static void Main(string[] args)
        {
            try
            {
                //_tr.SetLogFile(null, LogOptions.LogToConsole);
                Trace.CurrentTrace.SetViewer(Console.Out.Write);
                //string log = _config.Get("Log").zRootPath(zapp.GetEntryAssemblyDirectory());
                //if (log != null)
                //{
                //    //_tr.WriteLine("log to \"{0}\"", log);
                //    _tr.AddTraceFile(log, LogOptions.IndexedFile);
                //}
                Trace.CurrentTrace.SetWriter(WriteToFile.Create(_config.Get("Log").zRootPath(zapp.GetEntryAssemblyDirectory()), _config.Get("Log/@option").zTextDeserialize(FileOption.None)));
                //_tr.WriteLine("print tools");
                Trace.WriteLine("config file \"{0}\"", _config.ConfigFile);
                pt(args);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                Trace.WriteLine(ex.StackTrace);
            }
        }

        public static void pt(string[] args)
        {
            if (!GetParams(args))
            {
                Help();
                return;
            }
            Execute();
        }

        public static void Execute()
        {
            switch (_cmd.ToLower())
            {
                case "r":
                case "ren":
                    RenameFile();
                    break;
            }
        }

        public static bool GetParams(string[] args)
        {
            if (args.Length == 0)
                return false;
            else
            {
                _cmd = args[0];
                //_args = new string[args.Length - 1];
                //Array.Copy(args, 1, _args, 0, args.Length - 1);
                List<string> arg = new List<string>();
                List<string> option = new List<string>();
                bool first = true;
                foreach (string s in args)
                {
                    if (first)
                    {
                        first = false;
                        continue;
                    }
                    if (s.StartsWith("-"))
                        option.Add(s);
                    else
                        arg.Add(s);
                }
                _args = arg.ToArray();
                _options = option.ToArray();
            }
            return true;
        }

        public static void Help()
        {
            Trace.WriteLine("pt.exe <cmd> [arg ...]");
            Trace.WriteLine("  cmd : ren <file> [-r (real) -s (simulate)]     (r)  rename pdf print file");
        }

        public static void RenameFile()
        {
            //Trace.WriteLine("current directory \"{0}\"", Directory.GetCurrentDirectory());
            bool recurseDir = false;
            bool simulate = true;
            bool moveFile = false;
            string printFile = null;
            foreach (string opt in _options)
            {
                if (opt == "-r")
                    simulate = false;
                if (opt == "-s")
                    simulate = true;
                if (opt == "-m")
                    moveFile = true;
                if (opt == "-nm")
                    moveFile = false;
                if (opt.StartsWith("-n:"))
                    printFile = opt.Substring(3);
            }
            Trace.Write("rename file (");
            if (simulate)
                Trace.Write("simulate");
            else
                Trace.Write("real");
            if (moveFile)
                Trace.Write(" rename and move file");
            else
                Trace.Write(" rename file");
            Trace.Write(") ");
            foreach (string arg in _args)
                Trace.Write(" {0}", arg);
            if (printFile != null)
                Trace.Write(" name:\"{0}\"", printFile);
            Trace.WriteLine();

            //Trace.WriteLine("init PrintManager start");
            PrintManager_v1 pm = new PrintManager_v1(_config.GetElement("Print"));
            //Trace.WriteLine("init PrintManager end");

            SearchOption option = SearchOption.TopDirectoryOnly;
            if (recurseDir) option = SearchOption.AllDirectories;
            string dir = ".";
            string filePattern = "*.pdf";
            if (_args.Length > 0)
            {
                filePattern = _args[0];
                string dir2 = zPath.GetDirectoryName(filePattern);
                if (dir2 != "")
                    dir = dir2;
                filePattern = zPath.GetFileName(filePattern);
            }
            //Trace.WriteLine("read directory start");
            string[] files = zDirectory.GetFiles(dir, filePattern, option);
            //Trace.WriteLine("read directory end");
            if (printFile != null && files.Length > 1)
                throw new PBException("can't use option name with more than one file");
            if (files.Length == 0)
                Trace.WriteLine("file not found");
            foreach (string file in files)
            {
                try
                {
                    //Trace.WriteLine("RenameFile start");
                    pu1.RenameFile(pm, file, simulate, moveFile, printFile);
                    //Trace.WriteLine("RenameFile end");
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                    Trace.WriteLine(ex.StackTrace);
                }
            }
        }

    }
}
