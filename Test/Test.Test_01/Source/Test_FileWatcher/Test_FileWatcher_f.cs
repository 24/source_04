using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using System.Text;
using pb;
using pb.Compiler;

namespace Test_FileWatcher
{
    static partial class w
    {
        private static ITrace _tr = Trace.CurrentTrace;
        private static RunSource _wr = RunSource.CurrentRunSource;

        public static void Init()
        {
        }

        public static void Test_01()
        {
            _tr.WriteLine("Test_01");
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static void Test_FileSystemWatcher_01()
        {
            string path = @"c:\Users\Pierre\AppData\Local\Temp";
            string filter = "*.cmdline";
            _tr.WriteLine("Test_FileSystemWatcher_01 \"{0}\" \"{1}\"", path, filter);
            FileSystemWatcher watcher = new FileSystemWatcher(path);
            watcher.Filter = filter;
            watcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size;
            watcher.Changed += new FileSystemEventHandler(watcher_Changed);
            watcher.Created += new FileSystemEventHandler(watcher_Changed);
            watcher.Deleted += new FileSystemEventHandler(watcher_Changed);
            watcher.Renamed += new RenamedEventHandler(watcher_Renamed);

            try
            {
                watcher.EnableRaisingEvents = true;
                while (true)
                {
                    //if (_wr.Abort)
                    if (_wr.IsExecutionAborted())
                        break;
                }
            }
            finally
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
                if (__fs != null)
                {
                    __fs.Close();
                    __fs = null;
                }
            }
        }

        private static FileStream __fs = null;
        static void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            _tr.WriteLine("{0:dd/MM/yyyy HH:mm:ss.ffffff} watcher : {1} \"{2}\"", DateTime.Now, e.ChangeType, e.Name);
            if (e.ChangeType == WatcherChangeTypes.Created)
            {
                if (__fs != null)
                {
                    __fs.Close();
                    __fs = null;
                }
                __fs = new FileStream(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }
        }

        static void watcher_Renamed(object sender, RenamedEventArgs e)
        {
            _tr.WriteLine("{0:dd/MM/yyyy HH:mm:ss.ffffff} watcher : {1} \"{2}\" to \"{3}\"", DateTime.Now, e.ChangeType, e.OldName, e.Name);
        }

    }
}
