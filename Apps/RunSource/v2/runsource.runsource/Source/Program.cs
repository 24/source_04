using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using pb;
using pb.IO;
using pb.Data.Xml;
using pb.Compiler;
using pb.Windows.Forms;

namespace runsourced
{
    static class Program
    {
        //private static bool _updateProgram = false;
        //private static Dictionary<string, List<string>> _projectFiles = null;
        private static RunSourceRestartParameters _runSourceRestartParameters = null;

        [STAThread]
        //static int Main(string[] args)
        static void Main()
        {
            try
            {

                XmlConfig config = new XmlConfig();

                //Trace.CurrentTrace.SetLogFile("log.txt", LogOptions.None);
                //string path = config.Get("Log").zSetRootDirectory();
                //if (path != null)
                //    _runSource.Trace.SetLogFile(path, LogOptions.IndexedFile);
                //Trace.WriteLine(config.Get("Log").zSetRootDirectory());
                //LogOptions logOptions = config.Get("Log/@LogOptions").zTextDeserialize(LogOptions.None);
                //Trace.WriteLine(logOptions.ToString());
                //Trace.CurrentTrace.SetLogFile(config.Get("Log").zSetRootDirectory(), config.Get("Log/@LogOptions").zTextDeserialize(LogOptions.None));
                //Trace.WriteLine(Trace.CurrentTrace.GetLogFile());

                FormatInfo.SetInvariantCulture();
                Application.CurrentCulture = FormatInfo.CurrentFormat.CurrentCulture;
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                RemoteRunSource remoteRunSource = new RemoteRunSource();
                remoteRunSource.RunsourceDllFilename = config.GetExplicit("RunsourceDllFilename");
                remoteRunSource.RunsourceClassName = config.GetExplicit("RunsourceClassName");
                remoteRunSource.TraceClassName = config.GetExplicit("TraceClassName");
                //remoteRunSource.CreateRunSourceDomain = config.Get("CreateRunSourceDomain").zTryParseAs<bool>(true);
                // ATTENTION si CreateRunSourceDomain = true runsource.launch.exe ne peut pas mettre à jour runsource.runsource.exe
                remoteRunSource.CreateRunSourceDomain = config.Get("CreateRunSourceDomain").zTryParseAs<bool>(false);
                IRunSource runSource = remoteRunSource.GetRunSource();
                // ATTENTION Trace exists in both 'runsource.runsource.exe' and 'runsource.dll'
                // donc il faut utiliser RemoteRunSource.GetTrace()
                ITrace trace = remoteRunSource.GetTrace();
                //trace.SetLogFile(config.Get("Log").zRootPath(zapp.GetAppDirectory()), config.Get("Log/@LogOptions").zTextDeserialize(LogOptions.None));
                // .zRootPath(zapp.GetAppDirectory())
                trace.SetWriter(config.Get("Log"), config.Get("Log/@option").zTextDeserialize(FileOption.None));

                //runSource.GenerateAndExecuteManager.GenerateAssemblyDirectory = config.Get("GenerateAssemblyDirectory", "run").zRootPath(zapp.GetAppDirectory());
                // ATTENTION appeler DeleteGeneratedAssemblies() après SetRunSourceConfig()
                //runSource.DeleteGeneratedAssemblies();
                RunSourceForm form = new RunSourceForm(runSource, trace, config, GetRunSourceRestartParameters());

                //form.UpdateRunsourceFiles += FormUpdateRunsourceFiles;
                form.SetRestartRunsource += FormSetRestartRunsource;
                Application.Run(form);
                //if (_projectFiles != null)
                SetRunSourceRestartParameters();
                //SetRunSourceUpdateParameters();

                //if (_updateProgram)
                //{
                //    _updateProgram = false;
                //    return 1;
                //}
                //else
                //    return 0;
                //Application.Exit();
            }
            catch (Exception ex)
            {
                zerrf.ErrorMessageBox(ex);
                //return 0;
            }
        }

        //static void FormUpdateRunsourceFiles(Dictionary<string, List<string>> projectFiles)
        //{
        //    _projectFiles = projectFiles;
        //}

        static void FormSetRestartRunsource(RunSourceRestartParameters runSourceRestartParameters)
        {
            _runSourceRestartParameters = runSourceRestartParameters;
        }

        //static void SetRunSourceUpdateAndRestartParameters()
        static void SetRunSourceRestartParameters()
        {
            if (_runSourceRestartParameters != null)
                AppDomain.CurrentDomain.SetData("RunSourceRestartParameters", _runSourceRestartParameters.zXmlSerialize());
        }

        //static void SetRunSourceUpdateParameters()
        //{
        //    AppDomain.CurrentDomain.SetData("RunSourceUpdateParameters", );
        //}

        static RunSourceRestartParameters GetRunSourceRestartParameters()
        {
            RunSourceRestartParameters runSourceRestartParameters = null;
            try
            {
                object o = AppDomain.CurrentDomain.GetData("RunSourceRestartParameters");
                AppDomain.CurrentDomain.SetData("RunSourceRestartParameters", null);
                if (o != null)
                    runSourceRestartParameters = ((string)o).zXmlDeserialize<RunSourceRestartParameters>();
            }
            catch(Exception ex)
            {
                MessageBox.Show(Error.GetErrorMessage(ex), "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return runSourceRestartParameters;
        }

    }
}
