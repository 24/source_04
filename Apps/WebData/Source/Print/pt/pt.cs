using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using pb;
using pb.Data.Xml;
using pb.IO;
using Print;

namespace pt
{
    class Program
    {
        private static string _cmd;
        private static string[] _args;
        private static string[] _options;
        private static XmlConfig _config = XmlConfig.CurrentConfig;
        private static XmlConfig _printConfig = XmlConfig.CurrentConfig;
        //private static XmlConfig _config = new XmlConfig("pt_config.xml");

        public static void Main(string[] args)
        {
            try
            {
                //Trace.CurrentTrace.SetLogFile(null, LogOptions.LogToConsole);
                Trace.CurrentTrace.SetViewer(Console.Out.Write);
                //string log = _config.Get("Log").zRootPath(zapp.GetEntryAssemblyDirectory());
                //if (log != null)
                //{
                //    //Trace.WriteLine("log to \"{0}\"", log);
                //    //Trace.AddTraceFile(log, LogOptions.IndexedFile);
                //    Trace.CurrentTrace.AddTraceFile(log);
                //}
                Trace.CurrentTrace.SetWriter(WriteToFile.Create(_config.Get("Log").zRootPath(zapp.GetEntryAssemblyDirectory()), _config.Get("Log/@option").zTextDeserialize(FileOption.None)));
                string path = _config.Get("PrintConfig");
                if (path != null)
                {
                    //Trace.WriteLine("load print config \"{0}\"", path);
                    _printConfig = new XmlConfig(path);
                }
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
            //PrintManager pm = new PrintManager(_config.GetElement("Print"));
            PrintManager_v2 pm = new PrintManager_v2(_printConfig.GetElement("Print"));
            //Trace.WriteLine("init PrintManager end");

            SearchOption option = SearchOption.TopDirectoryOnly;
            if (recurseDir) option = SearchOption.AllDirectories;
            string dir = ".";
            string filePattern = "*.pdf";
            if (_args.Length > 0)
            {
                filePattern = _args[0];
                string dir2 = Path.GetDirectoryName(filePattern);
                if (dir2 != "")
                    dir = dir2;
                filePattern = Path.GetFileName(filePattern);
            }
            //Trace.WriteLine("read directory start");
            string[] files = Directory.GetFiles(dir, filePattern, option);
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
                    zprint.RenameFile(pm, file, simulate, moveFile, printFile);
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
