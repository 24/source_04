using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using pb;
using PB_Util;
using pb.Data.Xml;

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
                Trace.CurrentTrace.SetLogFile("log.txt", LogOptions.None);

                XmlConfig config = new XmlConfig();
                FormatInfo.SetInvariantCulture();
                Application.CurrentCulture = FormatInfo.CurrentFormat.CurrentCulture;
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                RemoteRunSource remoteRunSource = new RemoteRunSource();
                remoteRunSource.RunsourceDllFilename = config.GetExplicit("RunsourceDllFilename");
                remoteRunSource.RunsourceClassName = config.GetExplicit("RunsourceClassName");
                //remoteRunSource.CreateRunSourceDomain = config.Get("CreateRunSourceDomain").zTryParseAs<bool>(true);
                // attention si CreateRunSourceDomain = true runsource.launch.exe ne peut pas mettre à jour runsource.runsourced32.exe
                remoteRunSource.CreateRunSourceDomain = config.Get("CreateRunSourceDomain").zTryParseAs<bool>(false);
                RunSourceForm form = new RunSourceForm(remoteRunSource.GetRunSource(), config, GetRunSourceRestartParameters());

                //form.UpdateRunsourceFiles += FormUpdateRunsourceFiles;
                form.SetRestartRunsource += FormSetRestartRunsource;
                Application.Run(form);
                //if (_projectFiles != null)
                SetRunSourceUpdateAndRestartParameters();

                //if (_updateProgram)
                //{
                //    _updateProgram = false;
                //    return 1;
                //}
                //else
                //    return 0;
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

        static void SetRunSourceUpdateAndRestartParameters()
        {
            //if (_projectFiles != null)
            //    AppDomain.CurrentDomain.SetData("RunSourceUpdateFiles", _projectFiles);
            if (_runSourceRestartParameters != null)
                AppDomain.CurrentDomain.SetData("RunSourceRestartParameters", _runSourceRestartParameters.zXmlSerialize());
        }
        static RunSourceRestartParameters GetRunSourceRestartParameters()
        {
            RunSourceRestartParameters runSourceRestartParameters = null;
            try
            {
                //runSourceRestartParameters = ((string)AppDomain.CurrentDomain.GetData("RunSourceRestartParameters")).zXmlDeserialize<RunSourceRestartParameters>();
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
