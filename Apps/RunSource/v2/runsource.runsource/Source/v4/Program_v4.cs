using System;
using System.Windows.Forms;
using pb;
using pb.IO;
using pb.Data.Xml;
using pb.Compiler;
using pb.Windows.Forms;

namespace runsourced
{
    static class Program_v4
    {
        private static RunSourceRestartParameters _runSourceRestartParameters = null;

        [STAThread]
        static void Main()
        {
            try
            {
                XmlConfig config = new XmlConfig();
                FormatInfo.SetInvariantCulture();
                Application.CurrentCulture = FormatInfo.CurrentFormat.CurrentCulture;
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                RemoteRunSource remoteRunSource = new RemoteRunSource();
                remoteRunSource.RunsourceDllFilename = config.GetExplicit("RunsourceDllFilename");
                remoteRunSource.RunsourceClassName = config.GetExplicit("RunsourceClassName");
                remoteRunSource.TraceClassName = config.GetExplicit("TraceClassName");
                // ATTENTION si CreateRunSourceDomain = true runsource.launch.exe ne peut pas mettre à jour runsource.runsource.exe
                remoteRunSource.CreateRunSourceDomain = config.Get("CreateRunSourceDomain").zTryParseAs<bool>(false);
                IRunSource runSource = remoteRunSource.GetRunSource();
                //runSource.AllowMultipleExecution = config.Get("AllowMultipleExecution").zTryParseAs(false);
                // ATTENTION Trace exists in both 'runsource.runsource.exe' and 'runsource.dll'
                // donc il faut utiliser RemoteRunSource.GetTrace()
                ITrace trace = remoteRunSource.GetTrace();
                trace.SetWriter(config.Get("Log"), config.Get("Log/@option").zTextDeserialize(FileOption.None));

                // ATTENTION appeler DeleteGeneratedAssemblies() après SetRunSourceConfig()
                //RunSourceForm form = new RunSourceForm(runSource, trace, config, GetRunSourceRestartParameters());
                //RunSourceForm_v3 form = new RunSourceForm_v3(runSource, trace, config, GetRunSourceRestartParameters());
                RunSourceFormExe form = new RunSourceFormExe(runSource, trace, config, GetRunSourceRestartParameters());

                form.SetRestartRunsource += FormSetRestartRunsource;
                Application.Run(form);
                SetRunSourceRestartParameters();
            }
            catch (Exception ex)
            {
                zerrf.ErrorMessageBox(ex);
            }
        }

        static void FormSetRestartRunsource(RunSourceRestartParameters runSourceRestartParameters)
        {
            _runSourceRestartParameters = runSourceRestartParameters;
        }

        static void SetRunSourceRestartParameters()
        {
            if (_runSourceRestartParameters != null)
                AppDomain.CurrentDomain.SetData("RunSourceRestartParameters", _runSourceRestartParameters.zXmlSerialize());
        }

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
