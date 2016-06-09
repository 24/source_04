using System;
using pb;
using pb.Data.Xml;
using pb.IO;
using pb.Windows.Forms;

namespace runsource_launch
{
    static class Program
    {
        private static string __configFile = "runsource.runsource.config.xml";
        private static XmlConfig __config = null;
        //private static string __defaultUpdateDirectory = "..\\update";
        private static AppDomain __domain = null;
        private static string __domainRestartParametersName = "RunSourceRestartParameters";
        private static string __runsourceDomainName = "runsourced32";
        private static string __runsourceExeName = "runsource.runsource.exe";  // old name "runsource.runsourced32.exe"
        private static bool __traceUpdate = false;

        [STAThread]
        static void Main()
        {
            try
            {
                object runSourceRestartParameters = null;
                while (true)
                {
                    __config = new XmlConfig(__configFile);
                    Trace.CurrentTrace.SetWriter(__config.Get("Log").zRootPath(zapp.GetAppDirectory()), __config.Get("Log/@option").zTextDeserialize(FileOption.None));
                    __traceUpdate = __config.Get("UpdateRunSource/TraceUpdate").zTryParseAs(false);

                    UpdateRunSourceFiles();
                    Run(runSourceRestartParameters);
                    // attention récupérer RunSourceRestartParameters avant UpdateRunSourceFiles(), UpdateRunSourceFiles() fait AppDomain.Unload()
                    runSourceRestartParameters = __domain.GetData(__domainRestartParametersName);
                    if (runSourceRestartParameters == null)
                        break;
                    //__domain.DomainUnload += domain_DomainUnload;
                    //Trace.WriteLine("__domain.IsFinalizingForUnload() : {0}", __domain.IsFinalizingForUnload());
                    //Trace.WriteLine("AppDomain.Unload(__domain)");
                    AppDomain.Unload(__domain);
                    __domain = null;
                    //Trace.WriteLine("__domain.IsFinalizingForUnload() : {0}", __domain.IsFinalizingForUnload());
                }
            }
            catch (Exception ex)
            {
                zerrf.ErrorMessageBox(ex);
            }
        }

        static void Run(object runSourceRestartParameters)
        {
            __domain = AppDomain.CreateDomain(__runsourceDomainName);
            if (runSourceRestartParameters != null)
            {
                __domain.SetData(__domainRestartParametersName, runSourceRestartParameters);
                runSourceRestartParameters = null;
            }
            __domain.ExecuteAssembly(zPath.Combine(zapp.GetAppDirectory(), __runsourceExeName));
        }

        static void UpdateRunSourceFiles()
        {
            string appDir = zapp.GetAppDirectory();
            //string dir = __config.Get("UpdateRunSource/UpdateDirectory", __defaultUpdateDirectory).zRootPath(appDir);
            //string dir = __config.Get("UpdateRunSource/UpdateDirectory").zRootPath(appDir);
            string dir = __config.Get("RunsourceUpdateDirectory").zRootPath(appDir);
            UpdateFiles(dir, appDir);
            //if (zDirectory.Exists(dir))
            //{
            //    foreach (string file in zDirectory.EnumerateFiles(dir))
            //    {
            //        //Trace.WriteLine("copy file \"{0}\" to directory \"{1}\"", file, appDir);
            //        try
            //        {
            //            zfile.CopyFileToDirectory(file, appDir, options: CopyFileOptions.OverwriteReadOnly | CopyFileOptions.CopyOnlyIfNewer);
            //        }
            //        catch (Exception exception)
            //        {
            //            Trace.WriteLine("error copying file \"{0}\" to directory \"{1}\"", file, appDir);
            //            Trace.WriteLine(exception.Message);
            //        }
            //    }
            //}
        }

        static void UpdateFiles(string sourceDirectory, string destinationDirectory)
        {
            if (sourceDirectory == null)
                return;
            if (__traceUpdate)
                Trace.WriteLine("update files from \"{0}\" to \"{1}\"", sourceDirectory, destinationDirectory);
            bool update = true;
            if (!zDirectory.Exists(sourceDirectory))
            {
                if (__traceUpdate)
                    Trace.WriteLine("  source directory not found \"{0}\"", sourceDirectory);
                update = false;
            }
            if (!zDirectory.Exists(destinationDirectory))
            {
                if (__traceUpdate)
                    Trace.WriteLine("  destination directory not found \"{0}\"", destinationDirectory);
                update = false;
            }
            if (!update)
                return;

            foreach (string file in zDirectory.EnumerateFiles(sourceDirectory))
            {
                if (__traceUpdate)
                    Trace.WriteLine("  copy file \"{0}\" to directory \"{1}\"", file, destinationDirectory);
                try
                {
                    zfile.CopyFileToDirectory(file, destinationDirectory, options: CopyFileOptions.OverwriteReadOnly | CopyFileOptions.CopyOnlyIfNewer);
                }
                catch (Exception exception)
                {
                    Trace.WriteLine("error copying file \"{0}\" to directory \"{1}\"", file, destinationDirectory);
                    Trace.WriteLine(exception.Message);
                }
            }
        }
    }
}
