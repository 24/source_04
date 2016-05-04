using System;
using System.ComponentModel;
using System.Drawing;
using System.ServiceProcess;
using System.Windows.Forms;
using pb;
using pb.Data.Xml;
using pb.Windows.Forms;

namespace Pib
{
    public class Pib_TrayIcon : IDisposable
    {
        TrayIcon gTrayIcon = null;
        private ToolStripMenuItem gmMain = null;
        private ToolStripMenuItem gmAbortCurrentTask = null;
        private ToolStripMenuItem gmCraddleSaveFile = null;
        private ToolStripMenuItem gmRunCraddle = null;
        private ToolStripMenuItem gmRunCraddleLastLevel = null;
        private ToolStripMenuItem gmDeleteCraddleLastLevel = null;
        private ToolStripMenuItem gmSqlServerService = null;
        //private ToolStripMenuItem gmRunTaskRapidshare = null;
        //private ToolStripMenuItem gmAbortTaskRapidshare = null;

        private const string gSqlServerServiceName = "MSSQLSERVER";
        private ServiceController gSqlServerService = null;
        private ServiceControllerStatus gLastStatusSqlServerService;
        private Image gSqlServerServiceImageOn = null;
        private Image gSqlServerServiceImageOff = null;
        private PB_Library.CradleOfRomeWatcher gCradleWatcher = null;

        public Pib_TrayIcon(TrayIcon trayIcon)
        {
            //gTrayIcon = new TrayIcon();
            gTrayIcon = trayIcon;

            gTrayIcon.NotifyIcon.Text = "Pib";
            gTrayIcon.NotifyIcon.MouseDoubleClick += new MouseEventHandler(notifyIcon_MouseDoubleClick);

            gmMain = zForm.CreateMenuItem("Pib &main", true, new EventHandler(m_main_Click));
            gmCraddleSaveFile = zForm.CreateMenuItem("&Craddle save file", true, new EventHandler(m_craddle_save_file_Click));
            gmRunCraddle = zForm.CreateMenuItem("&Run craddle", false, new EventHandler(m_run_craddle_Click));
            gmRunCraddleLastLevel = zForm.CreateMenuItem("&Run craddle last level", false, new EventHandler(m_run_craddle_last_level_Click));
            gmDeleteCraddleLastLevel = zForm.CreateMenuItem("&Delete craddle last level", false, new EventHandler(m_delete_craddle_last_level_Click));
            InitCradleOfRomeWatcher();
            if (gCradleWatcher.IsStarted())
                gmCraddleSaveFile.Checked = true;
            else
                gmCraddleSaveFile.Checked = false;
            //RunCraddleSaveFile();
            //UpdateRunCradleMenu();
            //ToolStripMenuItem mBackup = zmenu.CreateMenuItem("&Backup", false, new EventHandler(m_backup_Click));
            gmAbortCurrentTask = zForm.CreateMenuItem("&Abort current task", false, new EventHandler(m_abort_current_task_Click));
            gmAbortCurrentTask.Enabled = false;
            //gmRunTaskRapidshare = zmenu.CreateMenuItem("Run rapidshare &download", false, new EventHandler(m_RunTaskRapidshare_Click));
            //gmAbortTaskRapidshare = zmenu.CreateMenuItem("Ab&ort rapidshare download", false, new EventHandler(m_AbortTaskRapidshare_Click));
            //gmAbortTaskRapidshare.Enabled = false;
            //ToolStripMenuItem mAddFilesTubeDownload = zmenu.CreateMenuItem("Add &FilesTube download", false, new EventHandler(m_Add_FilesTubeDownload_Click));
            //ToolStripMenuItem mAddRapidshareDownload = zmenu.CreateMenuItem("Add &Rapidshare download", false, new EventHandler(m_AddRapidshareDownload_Click));

            gmSqlServerService = zForm.CreateMenuItem("Sql server service", false, new EventHandler(m_sql_server_service_Click));
            InitSqlServerService();
            UpdateMenuItemSqlServerService();
            ToolStripMenuItem mTestProcess = zForm.CreateMenuItem("Test &process", false, new EventHandler(m_TestProcess_Click));
            ToolStripMenuItem mTestThread1 = zForm.CreateMenuItem("Test_Thread.TestThread1 (bloque)", false, new EventHandler(m_TestThread1_Click));
            ToolStripMenuItem mTestThread2 = zForm.CreateMenuItem("Test_Thread.TestThread2 (ne bloque pas)", false, new EventHandler(m_TestThread2_Click));
            ToolStripMenuItem mRunUnfinishedThread = zForm.CreateMenuItem("Run unfinished thread", false, new EventHandler(m_RunUnfinishedThread_Click));
            ToolStripMenuItem mAbortUnfinishedThread = zForm.CreateMenuItem("Abort unfinished thread", false, new EventHandler(m_AbortUnfinishedThread_Click));
            ToolStripMenuItem mQuit = zForm.CreateMenuItem("&Quit", false, new EventHandler(m_quit_Click));
            //gTrayIcon.NotifyMenu = gTrayIcon.CreateMenuStrip(new ToolStripItem[] {
            gTrayIcon.AddMenuItems(
                gmMain,
                new ToolStripSeparator(),
                gmCraddleSaveFile,
                gmRunCraddle,
                gmRunCraddleLastLevel,
                gmDeleteCraddleLastLevel,
                new ToolStripSeparator(),
                //mBackup,
                gmAbortCurrentTask,
                //new ToolStripSeparator(),
                //gmRunTaskRapidshare,
                //gmAbortTaskRapidshare,
                //mAddFilesTubeDownload,
                //mAddRapidshareDownload,
                new ToolStripSeparator(),
                gmSqlServerService,
                new ToolStripSeparator(),
                mTestProcess,
                mTestThread1,
                mTestThread2,
                new ToolStripSeparator(),
                mQuit);
            gTrayIcon.NotifyMenu.Opening += new CancelEventHandler(NotifyMenu_Opening);
            WindowsApp.CurrentWindowsApp.NotifyMenu = gTrayIcon.NotifyMenu;

            main.gMainForm.VisibleChanged += new EventHandler(MainForm_VisibleChanged);

        }

        void NotifyMenu_Opening(object sender, CancelEventArgs e)
        {
            UpdateRunCradleMenu();
        }

        #region Dispose
        public void Dispose()
        {
            if (gTrayIcon != null) gTrayIcon.Dispose();
        }
        #endregion

        #region notifyIcon_MouseDoubleClick
        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (main.gMainForm.Visible)
                main.gMainForm.Hide();
            else
                ShowMain();
        }
        #endregion

        #region MainForm_VisibleChanged
        private void MainForm_VisibleChanged(object sender, EventArgs e)
        {
            if (!main.gMainForm.Visible)
                HideMainEvent();
        }
        #endregion

        #region //NotifyMenu_Opening
        //private bool gbSetFormBaseVisible = false;
        //private bool gbFormBaseShowInTaskbar = false;
        //private void NotifyMenu_Opening(object sender, CancelEventArgs e)
        //{
        //    //this.Focus();
        //    gbSetFormBaseVisible = false;
        //    if (main.gFormBase.Visible == false)
        //    {
        //        gbFormBaseShowInTaskbar = main.gFormBase.ShowInTaskbar;
        //        main.gFormBase.ShowInTaskbar = false;
        //        gbSetFormBaseVisible = true;
        //        main.gFormBase.Visible = true;
        //        main.gFormBase.Focus();
        //    }
        //    //else if (Form.ActiveForm != null)
        //    //    Form.ActiveForm.Focus();
        //    else
        //        WindowsApp.CurrentWindowsApp.ActivateFirstWindow();
        //}
        #endregion

        #region //NotifyMenu_Closing
        //void NotifyMenu_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        //{
        //    if (gbSetFormBaseVisible)
        //    {
        //        main.gFormBase.Visible = false;
        //        main.gFormBase.ShowInTaskbar = gbFormBaseShowInTaskbar;
        //        gbSetFormBaseVisible = false;
        //    }
        //}
        #endregion

        #region m_main_Click
        private void m_main_Click(object sender, EventArgs e)
        {
            if (gmMain.Checked)
                ShowMain();
            else
                main.gMainForm.Hide();
        }
        #endregion

        #region m_quit_Click
        private void m_quit_Click(object sender, EventArgs e)
        {
            main.Exit();
        }
        #endregion

        #region Test ...
        private void m_TestProcess_Click(object sender, EventArgs e)
        {
            //MenuExe(new cf.fExe(main.RunControlApplicationExit));
            MenuExe(main.RunControlApplicationExit);
        }

        private void m_TestThread1_Click(object sender, EventArgs e)
        {
            //MenuExe(new cf.fExe(Test_Thread.TestThread1));
            MenuExe(Test_Thread.TestThread1);
        }

        private void m_TestThread2_Click(object sender, EventArgs e)
        {
            //MenuExe(new cf.fExe(Test_Thread.TestThread2));
            MenuExe(Test_Thread.TestThread2);
        }

        private void m_RunUnfinishedThread_Click(object sender, EventArgs e)
        {
            //cf.MenuExe(new cf.fExe(Test_Thread.RunUnfinishedThread));
            MenuExe(Test_Thread.RunUnfinishedThread);
        }

        private void m_AbortUnfinishedThread_Click(object sender, EventArgs e)
        {
            //cf.MenuExe(new cf.fExe(Test_Thread.AbortUnfinishedThread));
            MenuExe(Test_Thread.AbortUnfinishedThread);
        }

        #region m_test01_Click
        private void m_test01_Click(object sender, EventArgs e)
        {
            Test01();
        }
        #endregion

        #region m_test02_Click
        private void m_test02_Click(object sender, EventArgs e)
        {
            Test02();
        }
        #endregion

        #region m_test03_Click
        private void m_test03_Click(object sender, EventArgs e)
        {
            Test03();
        }
        #endregion

        #region m_test04_Click
        private void m_test04_Click(object sender, EventArgs e)
        {
            Test04();
        }
        #endregion
        #endregion

        private void m_craddle_save_file_Click(object sender, EventArgs e)
        {
            //MenuExe(new cf.fExe(RunCraddleSaveFile));
            MenuExe(RunCraddleSaveFile);
        }

        private void m_run_craddle_Click(object sender, EventArgs e)
        {
            //MenuExe(new cf.fExe(RunCraddle));
            MenuExe(RunCraddle);
        }

        private void m_run_craddle_last_level_Click(object sender, EventArgs e)
        {
            //MenuExe(new cf.fExe(RunCraddleLastLevel));
            MenuExe(RunCraddleLastLevel);
        }

        private void m_delete_craddle_last_level_Click(object sender, EventArgs e)
        {
            //MenuExe(new cf.fExe(DeleteCraddleLastLevel));
            MenuExe(DeleteCraddleLastLevel);
        }

        //private void MenuExe(cf.fExe f)
        private void MenuExe(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(Error.GetErrorMessage(ex, false, false));
            }
        }

        #region m_abort_current_task_Click
        private void m_abort_current_task_Click(object sender, EventArgs e)
        {
            MenuExe(AbortCurrentTask);
        }
        #endregion

        private void m_sql_server_service_Click(object sender, EventArgs e)
        {
            //cf.MenuExe(new cf.fExe(SqlServerService));
            MenuExe(SqlServerService);
        }

        #region ShowMain
        private void ShowMain()
        {
            main.gMainForm.Show(main.gFormBase);
            main.gMainForm.WindowState = FormWindowState.Normal;
            gmMain.Checked = true;
        }
        #endregion

        #region HideMainEvent
        private void HideMainEvent()
        {
            main.gMainFormLocation = main.gMainForm.Location;
            if (gmMain != null) gmMain.Checked = false;
        }
        #endregion

        private void RunCraddleSaveFile()
        {
            if (gmCraddleSaveFile.Checked)
            {
                //StartCradleOfRomeWatcher();
                //gCradleWatcher.Stop();
                gCradleWatcher.Start();
            }
            else
                //StopCradleOfRomeWatcher();
                gCradleWatcher.Stop();
        }

        private void RunCraddle()
        {
            gCradleWatcher.RunCraddle();
            gCradleWatcher.LastLevel = gCradleWatcher.CurrentLevel;
            //UpdateRunCradleMenu();
            //gmRunCraddle.Enabled = false;
            //gmRunCraddleLastLevel.Enabled = false;
        }

        private void RunCraddleLastLevel()
        {
            gCradleWatcher.RunCraddleLastLevel();
            //UpdateRunCradleMenu();
            //gmRunCraddle.Enabled = false;
            //gmRunCraddleLastLevel.Enabled = false;
        }

        private void DeleteCraddleLastLevel()
        {
            if (MessageBox.Show("Voulez-vous supprimer " + gCradleWatcher.LastLevel.GetFilename() + " ?", "supprimer", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                gCradleWatcher.DeleteCraddleLastLevel();
        }

        private void InitCradleOfRomeWatcher()
        {
            XmlConfig config = XmlConfig.CurrentConfig;
            gCradleWatcher = new PB_Library.CradleOfRomeWatcher();
            gCradleWatcher.CradleProgram = config.Get("CradleOfRomeWatcher/Program");
            if (gCradleWatcher.CradleProgram == null)
                return;
            gCradleWatcher.CradleFileDirectory = config.GetExplicit("CradleOfRomeWatcher/FileDirectory");
            gCradleWatcher.CradleFileSaveDirectory = config.Get("CradleOfRomeWatcher/FileSaveDirectory");
            gCradleWatcher.CradleFileArchiveDirectory = config.Get("CradleOfRomeWatcher/FileArchiveDirectory");
            //gCradleWatcher.Notify += new PB_Library.NotifyDelegate(Trace);
            gCradleWatcher.Notify += new PB_Library.NotifyDelegate(TraceMessage);
            //gCradleWatcher.CraddleFilesSaved += new CraddleFilesSavedDelegate(CraddleFilesSaved);
            gCradleWatcher.BlitzLevelType = PB_Library.CradleOfRomeBlitzLevelType.Hard;
            if (config.Get("CradleOfRomeWatcher/StartWatcher").zTryParseAs(false))
                gCradleWatcher.Start();

            Trace.WriteLine("CradleOfRomeWatcher/Program : \"{0}\"", gCradleWatcher.CradleProgram);
            Trace.WriteLine("CradleOfRomeWatcher/FileDirectory : \"{0}\"", gCradleWatcher.CradleFileDirectory);
            Trace.WriteLine("CradleOfRomeWatcher/FileSaveDirectory : \"{0}\"", gCradleWatcher.CradleFileSaveDirectory);
            Trace.WriteLine("CradleOfRomeWatcher/FileArchiveDirectory : \"{0}\"", gCradleWatcher.CradleFileArchiveDirectory);
            Trace.WriteLine("CradleOfRomeWatcher/StartWatcher : {0}", gCradleWatcher.IsStarted());
        }

        //private void CraddleFilesSaved()
        //{
        //    UpdateRunCradleMenu();
        //}

        private void UpdateRunCradleMenu()
        {
            gmRunCraddleLastLevel.Text = "&Run craddle " + gCradleWatcher.LastLevel.GetFilename();
            gmDeleteCraddleLastLevel.Text = "&Delete craddle " + gCradleWatcher.LastLevel.GetFilename();
            if (PB_Library.CradleOfRomeWatcher.IsCraddleRunning())
            {
                gmRunCraddle.Enabled = false;
                gmRunCraddleLastLevel.Enabled = false;
                gmDeleteCraddleLastLevel.Enabled = false;
            }
            else
            {
                gmRunCraddle.Enabled = true;
                gmRunCraddleLastLevel.Enabled = true;
                gmDeleteCraddleLastLevel.Enabled = true;
            }
        }

        //public void StartCradleOfRomeWatcher()
        //{
        //    StopCradleOfRomeWatcher();
        //    //gCradleWatcher = new CradleOfRomeWatcher();
        //    //gCradleWatcher.Notify += new NotifyDelegate(Trace);
        //    //gCradleWatcher.BlitzLevelType = CradleOfRomeBlitzLevelType.Hard;
        //    gCradleWatcher.Start();
        //}

        //public void StopCradleOfRomeWatcher()
        //{
        //    if (gCradleWatcher != null)
        //    {
        //        gCradleWatcher.Stop();
        //        //gCradleWatcher.Notify -= new NotifyDelegate(Trace);
        //        //gCradleWatcher = null;
        //    }
        //}

        //public static void Trace(string message)
        public static void TraceMessage(string message)
        {
            Trace.WriteLine(message);
        }

        //private void RunBackup()
        //{

        //    Backup backup = new Backup();

        //    //Task task = new Task();
        //    //task.Name = "Backup source";
        //    //task.Trace = backup.Trace;
        //    //task.Progress = backup.Progress;
        //    //task.Progress.PutMessageToWindowsTitle = true;
        //    //task.ProgressDetail = backup.ProgressDetail;
        //    //task.StartFunction += new TaskStartFunction(backup.Execute);
        //    /////////////////////////////task.EndFunction += new TaskEndFunction(EndBackup);
        //    backup.TaskEnded += new TaskEventHandler(EndBackup);

        //    main.gMainForm.ShowTask1();
        //    backup.TaskTrace.WriteEvent += new cTraced.fWriteEvent(main.gMainForm.Task1WriteLog);
        //    backup.TaskProgress.ProgressChanged += new Progress.ProgressChangedEventHandler(main.gMainForm.Task1ProgressControl.SetProgress);
        //    backup.TaskProgress.ProgressTextChanged += new Progress.ProgressTextChangedEventHandler(main.gMainForm.Task1ProgressControl.SetProgressText);
        //    backup.TaskProgressDetail.ProgressChanged += new Progress.ProgressChangedEventHandler(main.gMainForm.Task1DetailProgressControl.SetProgress);
        //    backup.TaskProgressDetail.ProgressTextChanged += new Progress.ProgressTextChangedEventHandler(main.gMainForm.Task1DetailProgressControl.SetProgressText);

        //    gmAbortCurrentTask.Enabled = true;

        //    AddTask(backup);
        //}

        private void EndBackup(PB_Library.ITask task)
        {
            gmAbortCurrentTask.Enabled = false;
            //gBackupTask.StartFunction -= new TaskStartFunction(backup.Execute);
            /////////////////////////////task.EndFunction -= new TaskEndFunction(EndBackup);
            task.TaskEnded -= new PB_Library.TaskEventHandler(EndBackup);
            //task.TaskTrace.WriteEvent -= new cTraced.fWriteEvent(main.gMainForm.Task1WriteLog);
            //task.TaskTrace.Writed -= new WritedEvent(main.gMainForm.Task1WriteLog);
            task.TaskProgress.ProgressChanged -= new Progress.ProgressChangedEventHandler(main.gMainForm.Task1ProgressControl.SetProgress);
            task.TaskProgress.ProgressTextChanged -= new Progress.ProgressTextChangedEventHandler(main.gMainForm.Task1ProgressControl.SetProgressText);
            task.TaskProgressDetail.ProgressChanged -= new Progress.ProgressChangedEventHandler(main.gMainForm.Task1DetailProgressControl.SetProgress);
            task.TaskProgressDetail.ProgressTextChanged -= new Progress.ProgressTextChangedEventHandler(main.gMainForm.Task1DetailProgressControl.SetProgressText);
            //main.gMainForm.RemoveProgressText();
            task.TaskProgress.SetProgressText(null);
            task.TaskProgressDetail.SetProgressText(null);
        }

        #region AddTask
        private void AddTask(PB_Library.ITask task)
        {
            if (main.gTaskScheduler == null)
                main.gTaskScheduler = new PB_Library.TaskScheduler();
            main.gTaskScheduler.AddTask(task);
            main.gTaskScheduler.ExecuteTask();
        }
        #endregion

        #region AbortCurrentTask
        private void AbortCurrentTask()
        {
            if (main.gTaskScheduler == null) main.gTaskScheduler.AbortTask();
        }
        #endregion

        //private void InitTaskRapidshare()
        //{
        //    if (main.gRapidshareTask == null)
        //        main.gRapidshareTask = new RapidshareTask(XmlConfig.CurrentConfig.GetConfigElement("Rapidshare"));
        //}

        //private void RunTaskRapidshare()
        //{
        //    InitTaskRapidshare();
        //    main.gRapidshareTask.TaskEnded += new TaskEventHandler(EndTaskRapidshare);

        //    main.gMainForm.ShowTask1();
        //    main.gRapidshareTask.TaskTrace.WriteEvent += new cTraced.fWriteEvent(main.gMainForm.Task1WriteLog);
        //    main.gRapidshareTask.TaskProgress.ProgressChanged += new Progress.ProgressChangedEventHandler(main.gMainForm.Task1ProgressControl.SetProgress);
        //    main.gRapidshareTask.TaskProgress.ProgressTextChanged += new Progress.ProgressTextChangedEventHandler(main.gMainForm.Task1ProgressControl.SetProgressText);
        //    main.gRapidshareTask.TaskProgressDetail.ProgressChanged += new Progress.ProgressChangedEventHandler(main.gMainForm.Task1DetailProgressControl.SetProgress);
        //    main.gRapidshareTask.TaskProgressDetail.ProgressTextChanged += new Progress.ProgressTextChangedEventHandler(main.gMainForm.Task1DetailProgressControl.SetProgressText);
        //    //gRapidshareTask.TaskProgress.ProgressControl = main.gMainForm.Task1ProgressControl;
        //    //gRapidshareTask.TaskProgressDetail.ProgressControl = main.gMainForm.Task1DetailProgressControl;

        //    gmRunTaskRapidshare.Enabled = false;
        //    gmAbortTaskRapidshare.Enabled = true;

        //    main.gRapidshareTask.ExecuteTaskThread();
        //}

        //private void EndTaskRapidshare(ITask task)
        //{
        //    gmRunTaskRapidshare.Enabled = true;
        //    gmAbortTaskRapidshare.Enabled = false;

        //    main.gRapidshareTask.TaskEnded -= new TaskEventHandler(EndTaskRapidshare);
        //    main.gRapidshareTask.TaskTrace.WriteEvent -= new cTraced.fWriteEvent(main.gMainForm.Task1WriteLog);
        //    main.gRapidshareTask.TaskProgress.ProgressChanged -= new Progress.ProgressChangedEventHandler(main.gMainForm.Task1ProgressControl.SetProgress);
        //    main.gRapidshareTask.TaskProgress.ProgressTextChanged -= new Progress.ProgressTextChangedEventHandler(main.gMainForm.Task1ProgressControl.SetProgressText);
        //    main.gRapidshareTask.TaskProgressDetail.ProgressChanged -= new Progress.ProgressChangedEventHandler(main.gMainForm.Task1DetailProgressControl.SetProgress);
        //    main.gRapidshareTask.TaskProgressDetail.ProgressTextChanged -= new Progress.ProgressTextChangedEventHandler(main.gMainForm.Task1DetailProgressControl.SetProgressText);

        //    //main.gMainForm.RemoveProgressText();
        //    task.TaskProgress.SetProgressText(null);
        //    task.TaskProgressDetail.SetProgressText(null);
        //}

        //private void AbortTaskRapidshare()
        //{
        //    gmAbortTaskRapidshare.Enabled = false;
        //    main.gRapidshareTask.AbortTask();
        //}

        //private void Add_FilesTubeDownload_FromClipboard()
        //{
        //    InitTaskRapidshare();
        //    main.gRapidshareTask.AddTask_FilesTubeDownload_FromClipboard();
        //}

        //private void AddRapidshareDownload_FromClipboard()
        //{
        //    InitTaskRapidshare();
        //    main.gRapidshareTask.AddTask_RapidshareDownload_FromClipboard();
        //}

        #region GetSqlServerServiceState
        private void InitSqlServerService()
        {
            gSqlServerService = GetServiceController(gSqlServerServiceName);
            //gSqlServerServiceImageOn = PibApp.Pib_resource.SqlServerServiceOn_16x16;
            //gSqlServerServiceImageOn = Source.Resource.SqlServerServiceOn_16x16;
            gSqlServerServiceImageOn = Properties.Resources.SqlServerServiceOn_16x16;
            //gSqlServerServiceImageOff = PibApp.Pib_resource.SqlServerServiceOff_16x16;
            //gSqlServerServiceImageOff = Source.Resource.SqlServerServiceOff_16x16;
            gSqlServerServiceImageOff = Properties.Resources.SqlServerServiceOff_16x16;
        }
        #endregion

        #region UpdateMenuItemSqlServerService
        private void UpdateMenuItemSqlServerService()
        {
            if (gSqlServerService == null)
                gmSqlServerService.Enabled = false;
            else
            {
                gSqlServerService.Refresh();
                gLastStatusSqlServerService = gSqlServerService.Status;
                switch (gSqlServerService.Status)
                {
                    case ServiceControllerStatus.Paused:
                    case ServiceControllerStatus.Running:
                        gmSqlServerService.Text = "Stop Sql server";
                        gmSqlServerService.Image = gSqlServerServiceImageOn;
                        gmSqlServerService.ImageScaling = ToolStripItemImageScaling.None;
                        gmSqlServerService.Invalidate();
                        break;
                    case ServiceControllerStatus.Stopped:
                        gmSqlServerService.Text = "Start Sql server";
                        gmSqlServerService.Image = gSqlServerServiceImageOff;
                        gmSqlServerService.ImageScaling = ToolStripItemImageScaling.None;
                        gmSqlServerService.Invalidate();
                        break;
                    default:
                        gmSqlServerService.Enabled = false;
                        break;
                }
            }
        }
        #endregion

        #region GetServiceController
        private static ServiceController GetServiceController(string serviceName)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                ServiceControllerStatus status = service.Status;
            }
            catch
            {
                service = null;
            }
            return service;
        }
        #endregion

        #region SqlServerService
        private void SqlServerService()
        {
            if (gSqlServerService == null) return;
            gSqlServerService.Refresh();
            if (gLastStatusSqlServerService != gSqlServerService.Status)
            {
                UpdateMenuItemSqlServerService();
                MessageBox.Show("Attention le status de Sql Server a changé, le menu a été mis à jour", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            switch (gSqlServerService.Status)
            {
                case ServiceControllerStatus.Paused:
                case ServiceControllerStatus.Running:
                    gSqlServerService.Stop();
                    do
                    {
                        gSqlServerService.Refresh();
                    }
                    while (gSqlServerService.Status == ServiceControllerStatus.StopPending);
                    break;
                case ServiceControllerStatus.Stopped:
                    gSqlServerService.Start();
                    do
                    {
                        gSqlServerService.Refresh();
                    }
                    while (gSqlServerService.Status == ServiceControllerStatus.StartPending);
                    break;
            }
            UpdateMenuItemSqlServerService();
        }
        #endregion

        #region Test ...
        #region InitTest
        private Progress gTestProgress = null;
        private void InitTest()
        {
            if (gTestProgress != null) return;
            gTestProgress = new Progress();
            //gTestProgress.ProgressChanged += new Progress.ProgressChangedEventHandler(main.gMainForm.Task1ProgressDetail);
            gTestProgress.ProgressChanged += new Progress.ProgressChangedEventHandler(main.gMainForm.Task1DetailProgressControl.SetProgress);
            main.gMainForm.ShowTask1();
        }
        #endregion

        #region Test01
        private void Test01()
        {
            InitTest();
            //main.gMainForm.Task1ProgressDetail(5, 10, "Test", false);
            string s = null;
            s += "Test";
            //gTestProgress.SetProgress(5, 10, s);
            gTestProgress.SetProgressText(s);
            gTestProgress.SetProgress(5, 10);
        }
        #endregion

        #region Test02
        private void Test02()
        {
            InitTest();
            //main.gMainForm.Task1ProgressDetail(0, 0, "", false);
            gTestProgress.SetProgressText("");
            gTestProgress.SetProgress(0, 0);
        }
        #endregion

        #region Test03
        private void Test03()
        {
            InitTest();
            //main.gMainForm.Task1ProgressDetail(0, 0, null, false);
            gTestProgress.SetProgress(0, 0);
        }
        #endregion

        #region Test04
        private void Test04()
        {
        }
        #endregion
        #endregion

        #region //LogBackupForm_FormClosed
        //private void LogBackupForm_FormClosed(object sender, FormClosedEventArgs e)
        //{
        //    main.gLogBackupForm.Dispose();
        //    main.gLogBackupForm = null;
        //    gmLogBackup.Enabled = false;
        //}
        #endregion

        #region static ConfigureTrayIcon
        public static Pib_TrayIcon ConfigureTrayIcon(TrayIcon trayIcon)
        {
            return new Pib_TrayIcon(trayIcon);
        }
        #endregion

    }
}
