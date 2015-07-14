using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using pb;
using pb.Data.Xml;
using pb.IO;
using pb.Windows.Forms;

// A faire :
//    Dans Exit() lancer un 2ème process pour vérifier que le process en cours est bien arreter

namespace Pib
{
    class main
    {
        public static Form gFormBase = null;
        //public static Pib_TrayIcon gTrayIcon = null;
        public static MainForm gMainForm = null;
        public static Point gMainFormLocation = new Point(-1, -1);
        //public static Form gLogForm = null;
        //public static Point gLogFormLocation = new Point(-1, -1);
        public static Form gLogBackupForm = null;
        //public static ResourceManager gResourceManager = null;
        public static Icon gIcon = null;
        //private static ITrace _trace = Trace.CurrentTrace;
        private static bool gbExitInProgress = false;
        private static bool gControlApplicationExit = false;

        public static PB_Library.TaskScheduler gTaskScheduler = null;
        //public static RapidshareTask gRapidshareTask = null;

        [STAThread] // STAThread est nécessaire pour accéder au clipboard (Clipboard.GetText)
        public static void Main(string[] args)
        {
            try
            {
                XmlConfig.CurrentConfig = new XmlConfig();

                //Trace.CurrentTrace.SetLogFile(XmlConfig.CurrentConfig.Get("Log"), LogOptions.IndexedFile);
                //Trace.CurrentTrace.SetLogFile(XmlConfig.CurrentConfig.Get("Log").zRootPath(zapp.GetAppDirectory()), XmlConfig.CurrentConfig.Get("Log/@LogOptions").zTextDeserialize(LogOptions.None));
                Trace.CurrentTrace.SetWriter(XmlConfig.CurrentConfig.Get("Log"), XmlConfig.CurrentConfig.Get("Log/@option").zTextDeserialize(FileOption.None));

                FormatInfo.SetInvariantCulture();
                Application.CurrentCulture = FormatInfo.CurrentFormat.CurrentCulture;
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                //bool trace = XmlConfig.CurrentConfig.Get<bool>("WinProcTrace", false);
                bool trace = XmlConfig.CurrentConfig.Get("WinProcTrace").zTryParseAs<bool>(false);
                WindowsApp.Trace = trace;

                if (!ExecuteArguments(args))
                    return;

                //gIcon = PibApp.Pib_resource.AppIcon;
                //gIcon = Source.Resource.AppIcon;
                gIcon = Properties.Resources.AppIcon;

                //WindowsApp.CurrentWindowsApp = new WindowsApp();
                WindowsApp.CurrentWindowsApp = new WindowsApp(gIcon, WAOptions.Default);
                WindowsApp.CurrentWindowsApp.Icon = gIcon;
                WindowsApp.CurrentWindowsApp.MinimizeParentWindowWhenMinimize = false;
                WindowsApp.CurrentWindowsApp.HideWindowWhenMinimize = true;
                WindowsApp.CurrentWindowsApp.HideWindowWhenClose = true;
                WindowsApp.CurrentWindowsApp.EnableCtrlF4Close = false;
                WindowsApp.CurrentWindowsApp.CloseApplicationOnAltF4 = false;
                //WindowsApp.CurrentWindowsApp.CreateNotifyIcon(gIcon);

                gMainForm = new MainForm();
                // il faut faire l'appel à AddWindow() ici et non pas dans le constructeur de la Form, sinon il y a un icone qui apparait dans la liste des taches (alt-tab)
                WindowData windowData = WindowsApp.CurrentWindowsApp.AddWindow(gMainForm);

                Pib_TrayIcon.ConfigureTrayIcon(WindowsApp.CurrentWindowsApp.TrayIcon);

                Application.Run();
            }
            catch (Exception ex)
            {
                zerrf.ErrorMessageBox(ex);
                Exit();
            }
        }

        public static bool ExecuteArguments(string[] args)
        {
            foreach (string arg in args)
            {
                if (arg.StartsWith("-ControlApplicationExit:", StringComparison.InvariantCultureIgnoreCase))
                {
                    int creationProcessId;
                    if (int.TryParse(arg.Substring(24, arg.Length - 24), out creationProcessId))
                    {
                        ControlApplicationExit(creationProcessId);
                    }

                    return false;
                }
            }
            return true;
        }

        public static void ControlApplicationExit(int creationProcessId)
        {
            gControlApplicationExit = true;
            Trace.WriteLine("Control application exit {0}", creationProcessId);
            //MessageBox.Show(string.Format("ControlApplicationExit ProcessId {0}, CreatingProcessId {1}", GetProcessId(), creationProcessId));
            System.Diagnostics.Process creationProcess = null;
            string creationProcessName = null;
            try
            {
                creationProcess = System.Diagnostics.Process.GetProcessById(creationProcessId);
                creationProcessName = creationProcess.ProcessName;
            }
            catch
            {
                Trace.WriteLine("Control application exit : process already exit");
                return;
            }
            //creationProcess.Exited += new EventHandler(CreationProcess_Exited);
            Trace.WriteLine("Control application exit : wait process exit {0}-{1}", creationProcessName, creationProcessId);
            DateTime dt = DateTime.Now;
            while (true)
            {
                Thread.Sleep(10);
                if (creationProcess.HasExited)
                {
                    Trace.WriteLine("Control application exit : process exit {0}-{1}", creationProcessName, creationProcessId);
                    //MessageBox.Show(string.Format("l'application {0}-{1} s'est arrêté", creationProcessName, creationProcessId));
                    break;
                }
                if (DateTime.Now.Subtract(dt).TotalMilliseconds > 2000)
                {
                    string msg = string.Format("l'application {0}-{1} n'est pas encore arrêté, voulez-vous détruire l'application (yes) ou attendre (no) ?", creationProcessName, creationProcessId);
                    DialogResult r = MessageBox.Show(msg, "Quitter", MessageBoxButtons.YesNo, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2);
                    if (r == DialogResult.Yes)
                    {
                        if (!creationProcess.HasExited)
                        {
                            Trace.WriteLine("Control application exit : kill process {0}-{1}", creationProcessName, creationProcessId);
                            creationProcess.Kill();
                        }
                    }
                    dt = DateTime.Now;
                }
            }
        }

        public static void RunControlApplicationExit()
        {
            if (gControlApplicationExit) return;
            Trace.WriteLine("Run control application exit");
            System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = process.MainModule.FileName;
            startInfo.Arguments = "-ControlApplicationExit:" + process.Id;
            //startInfo.ErrorDialog = true;
            //cTrace.Trace("  FileName : {0}", startInfo.FileName);
            //cTrace.Trace("  Verb : {0}", startInfo.Verb);
            //cTrace.Trace("  Verbs : {0}", startInfo.Verbs.zToStringValues());
            //cTrace.Trace("  Start new process ...");
            System.Diagnostics.Process.Start(startInfo);
        }

        public static void Exit()
        {
            // ATTENTION : l'appel à thread.Join() à partir du thread principal va bloquer si le thread appel cTrace.Trace()
            //   et que la trace s'affiche dans une TextBox qui fait appel TextBox.Invoke
            //      gRapidshareTask.TaskThread.Join(1000);
            //      gTaskScheduler.Thread.Join(1000);
            Thread thread = new Thread(new ThreadStart(_Exit));
            thread.Start();
        }

        private static void _Exit()
        {
            if (gbExitInProgress) return;
            gbExitInProgress = true;


            Trace.WriteLine("Exit in progress");
            //cTrace.Trace("Current thread Name           : {0}", Thread.CurrentThread.Name);
            //cTrace.Trace("Current thread Priority       : {0}", Thread.CurrentThread.Priority);
            //cTrace.Trace("Current thread ApartmentState : {0}", Thread.CurrentThread.GetApartmentState());

            if (gTaskScheduler != null && gTaskScheduler.IsRunning())
            {
                //cTrace.Trace("TaskScheduler thread Name           : {0}", gTaskScheduler.Thread.Name);
                //cTrace.Trace("TaskScheduler thread Priority       : {0}", gTaskScheduler.Thread.Priority);
                //cTrace.Trace("TaskScheduler thread ApartmentState : {0}", gTaskScheduler.Thread.GetApartmentState());

                DialogResult r = MessageBox.Show("Une tâche est en cours d'exécution, voulez-vous l'interrompre ?", "Quitter", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2);
                if (r == DialogResult.Cancel)
                {
                    gbExitInProgress = false;
                    return;
                }
                gTaskScheduler.AbortTask();
                DateTime dt = DateTime.Now;
                while (true)
                {
                    gTaskScheduler.Thread.Join(1000);
                    if (!gTaskScheduler.IsRunning())
                    {
                        Thread.Sleep(1000);
                        break;
                    }
                    if (DateTime.Now.Subtract(dt).TotalMilliseconds > 4000)
                    {
                        r = MessageBox.Show("La tâche est toujours en cours d'exécution, voulez-vous détruire la tâche (yes) ou réessayer de l'interrompre (no) ?", "Quitter", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2);
                        if (r == DialogResult.Cancel)
                        {
                            gbExitInProgress = false;
                            return;
                        }
                        if (r == DialogResult.Yes)
                        {
                            gTaskScheduler.KillTask();
                            break;
                        }
                        dt = DateTime.Now;
                    }
                }

            }

            RunControlApplicationExit();

            if (WindowsApp.CurrentWindowsApp != null)
                WindowsApp.CurrentWindowsApp.ApplicationExit();
            Application.Exit();
        }

        static void FormBase_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Form form = (Form)sender;
            //cTrace.Trace("FormBase                          : form closed event (exit)                  window {0,-25} hWnd {1}", form.Text, form.Handle);
            Exit();
        }

        //public static void CreateBackupLogForm(ITrace trace)
        public static void CreateBackupLogForm()
        {
            //gLogBackupForm = new LogForm(trace);
            gLogBackupForm = new LogForm();

            WindowData windowData = WindowsApp.CurrentWindowsApp.AddWindow(gLogBackupForm);

            gLogBackupForm.Icon = main.gIcon;
            gLogBackupForm.Text = "Pib - log backup";
            gLogBackupForm.ShowInTaskbar = false;
        }

        //private static void LoadParameters()
        //{
        //    XmlParameters xp = new XmlParameters("Pib");
        //    object oX, oY;
        //    oX = xp.Get("MainForm_X"); oY = xp.Get("MainForm_Y"); if (oX != null && oY != null) { gMainFormLocation.X = (int)oX; gMainFormLocation.Y = (int)oY; }
        //    if (gMainFormLocation.X > 0 && gMainFormLocation.Y > 0) gMainForm.Location = gMainFormLocation;
        //}

        //private static void SaveParameters()
        //{
        //    XmlParameters xp = new XmlParameters("Pib");
        //    Point location;
        //    if (gMainForm.Visible) location = gMainForm.Location; else location = gMainFormLocation;
        //    xp.Set("MainForm_X", location.X);
        //    xp.Set("MainForm_Y", location.Y);
        //    xp.Save();
        //}
    }

    public class Test_Thread
    {
        //private static ITrace _trace = Trace.CurrentTrace;
        private static Thread gUnfinishedThread = null;

        private static void TestThread_Function()
        {
            Trace.WriteLine("TestThread_Function begin");
            for (int i = 1; i < 100; i++)
            {
                Trace.WriteLine("TestThread_Function i = {0}", i);
                Thread.Sleep(1);
            }
            Trace.WriteLine("TestThread_Function end");
        }

        public static void TestThread1()
        {
            // bloque si TestThread1 est appelé directement
            Trace.WriteLine("TestThread1 begin");
            Thread thread = new Thread(new ThreadStart(TestThread_Function));
            thread.Start();
            //Thread.Sleep(500);
            thread.Join();
            Trace.WriteLine("TestThread1 end");
            MessageBox.Show("TestThread1 end");
        }

        public static void TestThread2()
        {
            // ne bloque pas
            Trace.WriteLine("TestThread2 begin");
            Thread thread = new Thread(new ThreadStart(TestThread1));
            thread.Start();
            Trace.WriteLine("TestThread2 end");
            MessageBox.Show("TestThread2 end");
        }

        private void UnfinishedPump()
        {
            while (true)
            {
                Thread.Sleep(10);
            }
        }

        public static void RunUnfinishedThread()
        {
            if (gUnfinishedThread != null)
            {
                Trace.WriteLine("Unfinished thread is already running");
                return;
            }
            Trace.WriteLine("Run unfinished thread");
            Test_Thread obj = new Test_Thread();
            gUnfinishedThread = new Thread(new ThreadStart(obj.UnfinishedPump));
            gUnfinishedThread.Start();
        }

        public static void AbortUnfinishedThread()
        {
            if (gUnfinishedThread == null)
            {
                Trace.WriteLine("Unfinished thread is not running");
                return;
            }
            Trace.WriteLine("Abort unfinished thread");
            gUnfinishedThread.Abort();
            Trace.WriteLine("Wait unfinished thread terminate");
            gUnfinishedThread.Join();
        }
    }
}
