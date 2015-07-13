using System;
using System.IO;
using PB_Util;

namespace runsource_launch
{
    static class Program
    {
        private static AppDomain __domain = null;
        private static string __updateDirectory = "..\\update";
        private static string __logFile = "log.txt";
        private static string __domainRestartParametersName = "RunSourceRestartParameters";
        private static string __runsourceDomainName = "runsourced32";
        private static string __runsourceExeName = "runsource.runsourced32.exe";  // old name "runsourced32.exe"

        [STAThread]
        static void Main()
        {
            try
            {
                Trace.CurrentTrace.SetLogFile(__logFile, LogOptions.None);
                //Trace.WriteLine("runsource_launch.Program.Main()");

                object runSourceRestartParameters = null;
                while (true)
                {
                    UpdateRunSourceFiles();
                    Run(runSourceRestartParameters);
                    // attention récupérer RunSourceRestartParameters avant UpdateRunSourceFiles(), UpdateRunSourceFiles() fait AppDomain.Unload()
                    runSourceRestartParameters = __domain.GetData(__domainRestartParametersName);
                    //UpdateRunSourceFiles();
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

        //private static void domain_DomainUnload(object sender, EventArgs e)
        //{
        //    Trace.WriteLine("domain_DomainUnload()");
        //}

        static void Run(object runSourceRestartParameters)
        {
            __domain = AppDomain.CreateDomain(__runsourceDomainName);
            if (runSourceRestartParameters != null)
            {
                __domain.SetData(__domainRestartParametersName, runSourceRestartParameters);
                runSourceRestartParameters = null;
            }
            __domain.ExecuteAssembly(Path.Combine(zapp.GetAppDirectory(), __runsourceExeName));
        }

        static void UpdateRunSourceFiles()
        {
            string appDir = zapp.GetAppDirectory();
            string dir = Path.Combine(appDir, __updateDirectory);
            if (Directory.Exists(dir))
            {
                foreach (string file in Directory.EnumerateFiles(dir))
                {
                    //Trace.WriteLine("copy file \"{0}\" to directory \"{1}\"", file, appDir);
                    try
                    {
                        zfile.CopyFileToDirectory(file, appDir, options: CopyFileOptions.OverwriteReadOnly | CopyFileOptions.CopyOnlyIfNewer);
                    }
                    catch (Exception exception)
                    {
                        Trace.WriteLine("error copying file \"{0}\" to directory \"{1}\"", file, appDir);
                        Trace.WriteLine(exception.Message);
                    }
                }
            }
        }

        //static void UpdateRunSourceFiles()
        //{
        //    object projectsFiles = _domain.GetData("RunSourceUpdateFiles");
        //    AppDomain.Unload(_domain);
        //    if (projectsFiles is Dictionary<string, List<string>>)
        //    {
        //        string dir = zapp.GetAppDirectory();
        //        foreach (KeyValuePair<string, List<string>> projectFiles in (Dictionary<string, List<string>>)projectsFiles)
        //        {
        //            zfile.MoveFiles(projectFiles.Value, dir, true);
        //        }
        //    }
        //}
    }
}
