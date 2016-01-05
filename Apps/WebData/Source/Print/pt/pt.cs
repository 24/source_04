using System;
using System.Collections.Generic;
using System.IO;
using pb;
using pb.Compiler;
using pb.Data.Xml;
using pb.IO;
using Print;
using Print.old;

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

    public partial class zprint
    {
        public static void RenameFile(PrintManager_v2 pm, string path, bool simulate = false, bool moveFile = false, string printFile = null)
        {
            string fmt1 = "file {0,-70}";
            string fmt2 = " {0,-30}";
            string fmt3 = " {0,-60}";
            bool writeFilenameOk = true;
            bool writeUnknowPrint = true;
            bool logFileInDestinationDirectory = false;
            bool debug = false;
            //Trace.WriteLine("path \"{0}\"", path);
            if (!zPath.IsPathRooted(path))
                path = zPath.GetFullPath(path);
            //Trace.WriteLine("path \"{0}\"", path);
            string file = zPath.GetFileName(path);
            if (!zFile.Exists(path))
            {
                Trace.WriteLine("file dont exists \"{0}\"", file);
                return;
            }
            PrintIssue issue;
            if (printFile != null)
                issue = pm.Find(printFile + ".pdf");
            else
                issue = pm.Find(file);
            string msgFile = "\"" + file + "\"";
            if (printFile != null)
                msgFile += " (\"" + printFile + "\")";
            if (issue == null || issue.Error != null)
            {
                if (writeUnknowPrint)
                {
                    Trace.Write(fmt1, msgFile);
                    if (issue == null)
                        Trace.Write(" unknow print");
                    else
                        Trace.Write(" {0}", issue.Print.Name);
                    if (issue != null && issue.Error != null)
                        Trace.Write(" " + issue.Error);
                    Trace.WriteLine();
                }
                return;
            }
            if (debug)
            {
                Trace.Write(fmt1, msgFile);
                Trace.WriteLine(fmt2, issue.Print.Name);
                issue.PrintValues.zTrace();
            }
            string file2 = issue.GetFilename();
            //Trace.WriteLine("zPath.GetDirectoryName(path) \"{0}\"", zPath.GetDirectoryName(path));
            //Trace.WriteLine("issue.Print.Directory       \"{0}\"", issue.Print.Directory);
            if (file == file2 && (!moveFile || zPath.GetDirectoryName(path).Equals(issue.Print.Directory, StringComparison.InvariantCultureIgnoreCase)))
            {
                if (writeFilenameOk)
                {
                    Trace.Write(fmt1, msgFile);
                    Trace.Write(fmt2, issue.Print.Name);
                    Trace.Write(" filename ok");
                    if (moveFile)
                        Trace.Write(" move to same directory");
                    Trace.WriteLine();
                }
                return;
            }

            if (moveFile && !simulate && !zDirectory.Exists(issue.Print.Directory))
                zDirectory.CreateDirectory(issue.Print.Directory);
            string traceFile = null;
            if (moveFile && !simulate && logFileInDestinationDirectory)
            {
                traceFile = zPath.Combine(issue.Print.Directory, "log.txt");
                //_tr.AddTraceFile(traceFile);
                //_tr.AddTraceFile(traceFile, LogOptions.None);
                if (traceFile != null)
                    Trace.CurrentTrace.AddOnWrite("zprint", WriteToFile.Create(traceFile, FileOption.None).Write);
            }
            try
            {
                Trace.Write(fmt1, msgFile);
                Trace.Write(fmt2, issue.Print.Name);
                string path2;
                bool fileExists = false;
                bool filesEquals = false;
                if (moveFile)
                    path2 = zPath.Combine(issue.Print.Directory, file2);
                else
                    path2 = zpath.PathSetFileName(path, file2);
                int index = 2;
                while (zFile.Exists(path2))
                {
                    fileExists = true;
                    if (path == path2)
                        break;
                    //filesEquals = zfile.FilesEquals(path, path2);
                    filesEquals = zfile.AreFileEqual(path, path2);
                    if (filesEquals)
                        break;
                    file2 = issue.GetFilename(index++);
                    path2 = zpath.PathSetFileName(path2, file2);
                }
                Trace.Write(fmt3, "\"" + file2 + "\"");
                if (simulate)
                    Trace.Write(" simulate");
                if (fileExists)
                {
                    Trace.Write(" file exists");
                    if (filesEquals)
                        Trace.Write(" and is equals");
                    else
                        Trace.Write(" and is different");
                }
                if (filesEquals)
                    Trace.Write(" delete file");
                else
                {
                    Trace.Write(" rename");
                    if (moveFile)
                        Trace.Write(" and move");
                    Trace.Write(" file");
                }
                Trace.WriteLine();
                if (!simulate)
                {
                    if (filesEquals)
                        zFile.Delete(path);
                    else if (!zFile.Exists(path2))
                        zFile.Move(path, path2);
                }
            }
            finally
            {
                if (traceFile != null)
                    //_tr.RemoveTraceFile(traceFile);
                    Trace.CurrentTrace.RemoveOnWrite("zprint");
            }
        }
    }
}
