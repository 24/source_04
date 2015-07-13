using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Threading;

namespace PB_Library
{
    #region class RapidshareException
    public class RapidshareException : Exception
    {
        public RapidshareException(string sMessage) : base(sMessage) { }
        public RapidshareException(string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm)) { }
        public RapidshareException(Exception InnerException, string sMessage) : base(sMessage, InnerException) { }
        public RapidshareException(Exception InnerException, string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm), InnerException) { }
    }
    #endregion

    #region enum RapidshareError
    public enum RapidshareError
    {
        NoError = 0,
        UnknowError,
        FileNotFound,
        IllegalContentFileBlocked,
        FileRemoved,
        DownloadLimitExceeded
    }
    #endregion

    public class RapidshareTask : ITask
    {
        private static ITrace _trace = Trace.CurrentTrace;
        private const string gTaskName = "Rapidshare download";
        private bool gStopTask = false;
        private bool gAbortTask = false;
        private bool gSuspendTask = false;
        private bool gDownloading = false;
        //private cTraced gTaskTrace = null;
        //private Trace gTaskTrace = null;
        private Progress gTaskProgress = null;
        private Progress gTaskProgressDetail = null;
        public event TaskEventHandler TaskEnded = null;

        private string gTaskDirectory = null;
        private string gTaskErrorDirectory = null;
        //private string gArchiveTaskDirectory = null;
        private Rapidshare gRapidshare = null;

        private Thread gTaskThread = null;

        #region constructor ...
        #region RapidshareTask()
        public RapidshareTask()
        {
            Init(null);
        }
        #endregion

        #region RapidshareTask(XmlConfigElement config)
        public RapidshareTask(XmlConfigElement config)
        {
            Init(config);
        }
        #endregion
        #endregion

        #region property ...
        #region TaskName
        public string TaskName
        {
            get { return gTaskName; }
        }
        #endregion

        //public Trace TaskTrace
        //{
        //    get { return gTaskTrace; }
        //    set { gTaskTrace = value; }
        //}

        #region TaskProgress
        public Progress TaskProgress
        {
            get { return gTaskProgress; }
        }
        #endregion

        #region TaskProgressDetail
        public Progress TaskProgressDetail
        {
            get { return gTaskProgressDetail; }
        }
        #endregion

        #region TaskDirectory
        public string TaskDirectory
        {
            get { return gTaskDirectory; }
            set { gTaskDirectory = value; }
        }
        #endregion

        #region TaskErrorDirectory
        public string TaskErrorDirectory
        {
            get { return gTaskErrorDirectory; }
            set { gTaskErrorDirectory = value; }
        }
        #endregion

        #region //ArchiveTaskDirectory
        //public string ArchiveTaskDirectory
        //{
        //    get { return gArchiveTaskDirectory; }
        //    set { gArchiveTaskDirectory = value; }
        //}
        #endregion

        #region Rapidshare
        public Rapidshare Rapidshare
        {
            get { return gRapidshare; }
        }
        #endregion

        #region TaskThread
        public Thread TaskThread
        {
            get { return gTaskThread; }
        }
        #endregion
        #endregion

        #region Init
        public void Init(XmlConfigElement config)
        {
            if (config != null)
            {
                gTaskDirectory = config.GetExplicit("TaskDirectory");
                gTaskErrorDirectory = config.GetExplicit("TaskErrorDirectory");
                //gArchiveTaskDirectory = config.GetExplicit("ArchiveTaskDirectory");
            }
            gRapidshare = new Rapidshare(config);
            //gTaskTrace = new cTraced();
            gTaskProgress = new Progress();
            gTaskProgressDetail = new Progress();
            gRapidshare.Progress.ProgressChanged += new Progress.ProgressChangedEventHandler(gTaskProgress.SetProgress);
            gRapidshare.Progress.ProgressTextChanged += new Progress.ProgressTextChangedEventHandler(gTaskProgress.SetProgressText);
            gRapidshare.ProgressDetail.ProgressChanged += new Progress.ProgressChangedEventHandler(gTaskProgressDetail.SetProgress);
            gRapidshare.ProgressDetail.ProgressTextChanged += new Progress.ProgressTextChangedEventHandler(gTaskProgressDetail.SetProgressText);
            //gTaskProgressDetail.ProgressControlChanged += new Progress.ProgressControlChangedEventHandler(TaskProgressDetail_ProgressControlChanged);
        }
        #endregion

        #region //TaskProgressDetail_ProgressControlChanged
        //private void TaskProgressDetail_ProgressControlChanged(IProgressControl progressControl)
        //{
        //    gRapidshare.Progress.ProgressControl = progressControl;
        //}
        #endregion

        #region IsRunning
        public bool IsRunning()
        {
            return gTaskThread != null;
        }
        #endregion

        #region IsDownloading
        public bool IsDownloading()
        {
            return gDownloading;
        }
        #endregion

        #region StopTask
        public void StopTask()
        {
            gStopTask = true;
        }
        #endregion

        #region AbortTask
        public void AbortTask()
        {
            gAbortTask = true;
            gRapidshare.AbortTask();
        }
        #endregion

        #region KillTask
        public void KillTask()
        {
            if (gTaskThread != null)
                gTaskThread.Abort();
        }
        #endregion

        #region SuspendTask
        public void SuspendTask()
        {
            gSuspendTask = true;
        }
        #endregion

        #region ResumeTask
        public void ResumeTask()
        {
            gSuspendTask = false;
        }
        #endregion

        #region ExecuteTaskThread
        public void ExecuteTaskThread()
        {
            ThreadStart ts = new ThreadStart(ExecuteTask);
            gTaskThread = new Thread(ts);
            ApplicationThreads.Add(gTaskThread, this);
            gTaskThread.Start();
        }
        #endregion

        #region ExecuteTask()
        public void ExecuteTask()
        {
            try
            {
                //gSuspendTask = false;
                _trace.WriteLine("Execute rapidshare task on directory : {0}", gTaskDirectory);
                if (!Directory.Exists(gTaskDirectory)) Directory.CreateDirectory(gTaskDirectory);
                string runningTaskDirectory = gTaskDirectory + @"\_RunningTask";
                if (!Directory.Exists(runningTaskDirectory)) Directory.CreateDirectory(runningTaskDirectory);
                while (true)
                {
                    if (gStopTask)
                    {
                        _trace.WriteLine("Rapidshare task ended");
                        break;
                    }
                    if (gAbortTask)
                    {
                        _trace.WriteLine("Rapidshare task aborted");
                        break;
                    }
                    string[] files = Directory.GetFiles(runningTaskDirectory, "*.xml");
                    if (files.Length == 0)
                        files = Directory.GetFiles(gTaskDirectory, "*.xml");
                    string file = (from f in files orderby f select f).FirstOrDefault();
                    if (file == null)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                    bool moveFileToErrorDirectory = false;
                    try
                    {
                        gDownloading = true;
                        //file = ArchiveTask(file);
                        string file2 = Path.Combine(runningTaskDirectory, Path.GetFileName(file));
                        File.Move(file, file2);
                        file = file2;
                        if (!gRapidshare.ExecuteXml(file))
                        {
                            RapidshareError error = gRapidshare.Error;
                            //if (error == RapidshareError.FileNotFound || error == RapidshareError.FileRemoved || error == RapidshareError.IllegalContentFileBlocked)
                            if (error != RapidshareError.DownloadLimitExceeded && error != RapidshareError.NoError)
                                moveFileToErrorDirectory = true;
                            //if (error == RapidshareError.DownloadLimitExceeded)
                            else
                            {
                                if (error == RapidshareError.DownloadLimitExceeded)
                                    _trace.WriteLine("Rapidshare task ended due to download limit exceeded");
                                break;
                            }
                        }
                        else
                            File.Delete(file);
                    }
                    catch (Exception ex)
                    {
                        _trace.WriteLine(cError.GetErrorMessage(ex));
                        moveFileToErrorDirectory = true;
                    }
                    finally
                    {
                        if (moveFileToErrorDirectory)
                            MoveFileToTaskErrorDirectory(file);
                        gDownloading = false;
                    }
                }
            }
            catch (Exception ex)
            {
                _trace.WriteLine(cError.GetErrorMessage(ex));
            }
            finally
            {
                try
                {
                    gStopTask = false;
                    gAbortTask = false;
                    gTaskThread = null;
                    gRapidshare.CancelAbortTask();
                    if (TaskEnded != null) TaskEnded(this);
                }
                catch (Exception ex)
                {
                    string sError = cError.GetErrorMessage(ex, false, true);
                    _trace.WriteLine(sError);
                    //gTrace.Trace(sError);
                }
            }
        }
        #endregion

        #region MoveFileToTaskErrorDirectory
        private void MoveFileToTaskErrorDirectory(string path)
        {
            if (path == null) return;
            string filename = Path.GetFileName(path);
            string path2 = Path.Combine(gTaskErrorDirectory, filename);
            path2 = cu.GetNewFilename(path2);
            if (!Directory.Exists(gTaskErrorDirectory)) Directory.CreateDirectory(gTaskErrorDirectory);
            _trace.WriteLine("Move file {0} to {1}", filename, gTaskErrorDirectory);
            File.Move(path, path2);
        }
        #endregion

        #region //ArchiveTask
        //private string ArchiveTask(string path)
        //{
        //    if (!Directory.Exists(gArchiveTaskDirectory)) Directory.CreateDirectory(gArchiveTaskDirectory);
        //    string archivePath = cu.GetNewIndexedFileName(gArchiveTaskDirectory, "{0:0000}_") + Path.GetFileName(path);
        //    cTrace.Trace("Archive file : {0}", path);
        //    File.Move(path, archivePath);
        //    return archivePath;
        //}
        #endregion

        #region AddTask_FilesTubeDownload_FromClipboard
        public void AddTask_FilesTubeDownload_FromClipboard()
        {
            string[] ss = GetClipboardText();
            if (ss == null) return;
            AddTask_FilesTubeDownload(0, ss);
        }
        #endregion

        #region AddTask_FilesTubeDownload
        public void AddTask_FilesTubeDownload(int nbDownloadFileToSkip, params string[] urls)
        {
            if (urls == null || urls.Length == 0) return;
            string pathXml = cu.GetNewIndexedFileName(Path.Combine(gTaskDirectory, "{0:0000}_")) + "FilesTube_" + GetUrlFilename(urls[0]) + ".xml";
            Rapidshare.GenerateXml_FilesTubeDownload(pathXml, nbDownloadFileToSkip, urls);
        }
        #endregion

        #region AddTask_RapidshareDownload_FromClipboard
        public void AddTask_RapidshareDownload_FromClipboard()
        {
            string[] ss = GetClipboardText();
            if (ss == null) return;
            AddTask_RapidshareDownload(0, ss);
        }
        #endregion

        #region AddTask_RapidshareDownload
        public void AddTask_RapidshareDownload(int nbDownloadFileToSkip, params string[] urls)
        {
            if (urls == null || urls.Length == 0) return;
            string pathXml = cu.GetNewIndexedFileName(Path.Combine(gTaskDirectory, "{0:0000}_")) + "Rapidshare_" + GetUrlFilename(urls[0]) + ".xml";
            Rapidshare.GenerateXml_RapidshareDownload(pathXml, nbDownloadFileToSkip, urls);
        }
        #endregion

        #region GetClipboardText
        public static string[] GetClipboardText()
        {
            if (!Clipboard.ContainsText())
            {
                _trace.WriteLine("Clipboard doesn't contain text");
                return null;
            }
            return cu.Split(Clipboard.GetText(), SplitOption.All);
        }
        #endregion

        #region GetUrlFilename
        public static string GetUrlFilename(string url)
        {
            Uri uri = new Uri(url);
            return Path.GetFileNameWithoutExtension(uri.Segments[uri.Segments.Length - 1]);
        }
        #endregion
    }

    public class Rapidshare
    {
        private bool gAbortTask = false;
        private string gLogin = null;
        private string gPassword = null;
        private string gDownloadDirectory = null;
        //private string gErrorDirectory = null;
        private string gFileDownloadDirectory = null;
        //private string gUnzipDirectory = null;
        private string gTraceDirectory = null;
        private int gTraceLevel = 0;
        private Progress gProgress = null;
        private Progress gProgressDetail = null;
        private int gNbDownloadFileToSkip = 0;
        private string gExecuteXmlPath = null;
        private XElement gExecuteXmlElement = null;
        private List<string> gDownloadedDirectories = new List<string>();
        private List<string> gDownloadedFiles = null;
        private string gDownloadUrl = null;
        private Http gHttp = new Http();
        private GClass2<RapidshareError, string>[] gErrors = null;
        private RapidshareError gError = RapidshareError.NoError;
        private string gErrorText = null;
        private static ITrace _trace = Trace.CurrentTrace;
        private static Regex grxFilename = new Regex("filename=([^;]*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static Regex grxFilenamePart = new Regex(@"(.*)\.part[0-9]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        #region constructor
        public Rapidshare()
        {
            Init(null);
        }

        public Rapidshare(XmlConfigElement config)
        {
            Init(config);
        }
        #endregion

        #region Init
        private void Init(XmlConfigElement config)
        {
            if (config != null)
            {
                gLogin = config.Get("Login");
                gPassword = config.Get("Password");
                gDownloadDirectory = config.Get("DownloadDirectory");
                //gErrorDirectory = config.Get("ErrorDirectory");
                //gUnzipDirectory = config.Get("UnzipDirectory");
                gTraceDirectory = config.Get("TraceDirectory");
                gTraceLevel = config.Get<int>("TraceLevel");
            }
            gProgress = new Progress();
            gProgressDetail = new Progress();
            gHttp = new Http();
            gHttp.Progress.ProgressChanged += new Progress.ProgressChangedEventHandler(gProgressDetail.SetProgress);
            gHttp.Progress.ProgressTextChanged += new Progress.ProgressTextChangedEventHandler(gProgressDetail.SetProgressText);
        }
        #endregion

        #region //Progress_ProgressControlChanged
        //private void Progress_ProgressControlChanged(IProgressControl progressControl)
        //{
        //    gHttp.Progress.ProgressControl = progressControl;
        //}
        #endregion

        #region property ...
        #region Login
        public string Login
        {
            get { return gLogin; }
            set { gLogin = value; }
        }
        #endregion

        #region Password
        public string Password
        {
            get { return gPassword; }
            set { gPassword = value; }
        }
        #endregion

        #region DownloadDirectory
        public string DownloadDirectory
        {
            get { return gDownloadDirectory; }
            set { gDownloadDirectory = value; }
        }
        #endregion

        #region //ErrorDirectory
        //public string ErrorDirectory
        //{
        //    get { return gErrorDirectory; }
        //    set { gErrorDirectory = value; }
        //}
        #endregion

        #region DownloadedFiles
        public List<string> DownloadedFiles
        {
            get { return gDownloadedFiles; }
            set { gDownloadedFiles = value; }
        }
        #endregion

        #region //UnzipDirectory
        //public string UnzipDirectory
        //{
        //    get { return gUnzipDirectory; }
        //    set { gUnzipDirectory = value; }
        //}
        #endregion

        #region TraceDirectory
        public string TraceDirectory
        {
            get { return gTraceDirectory; }
            set { gTraceDirectory = value; }
        }
        #endregion

        #region TraceLevel
        public int TraceLevel
        {
            get { return gTraceLevel; }
            set { gTraceLevel = value; }
        }
        #endregion

        #region Progress
        public Progress Progress
        {
            get { return gProgress; }
        }
        #endregion

        #region ProgressDetail
        public Progress ProgressDetail
        {
            get { return gProgressDetail; }
        }
        #endregion

        #region NbDownloadFileToSkip
        public int NbDownloadFileToSkip
        {
            get { return gNbDownloadFileToSkip; }
            set { gNbDownloadFileToSkip = value; }
        }
        #endregion

        #region Error
        public RapidshareError Error
        {
            get { return gError; }
        }
        #endregion

        #region ErrorText
        public string ErrorText
        {
            get { return gErrorText; }
        }
        #endregion
        #endregion

        #region AbortTask
        public void AbortTask()
        {
            gAbortTask = true;
            if (gHttp != null) gHttp.AbortTransfer();
            _trace.WriteLine("Abort rapidshare task ...");
        }
        #endregion

        #region CancelAbortTask
        public void CancelAbortTask()
        {
            gAbortTask = false;
            if (gHttp != null) gHttp.CancelAbortTransfer();
        }
        #endregion

        #region HttpLoad
        public void HttpLoad(string url)
        {
            if (gTraceLevel >= 1)
            {
                string s = string.Format("Load {0,-4} \"{1}\"", gHttp.Method, url);
                if (gHttp.Content != null)
                    s += string.Format(" \"{0}\"", gHttp.Content);
                _trace.WriteLine(s);
                gProgressDetail.SetProgressText(s);
                if (gTraceLevel >= 2) gHttp.TraceDirectory = gTraceDirectory;
            }
            //if (gTraceLevel >= 1) cTrace.Trace("Load \"{0}\", \"{1}\"", url, parameters);
            gHttp.Load(url);
        }
        #endregion

        #region ExecuteXml
        public bool ExecuteXml(string pathXml)
        {
            _trace.WriteLine("ExecuteXml : {0}", pathXml);
            gProgress.SetProgressText("ExecuteXml : {0}", pathXml);
            XDocument xd = XDocument.Load(pathXml);
            gExecuteXmlPath = pathXml;
            //int i = 0;
            //int nb = xd.Root.Elements().Count();
            foreach (XElement xe in xd.Root.Elements())
            {
                gExecuteXmlElement = xe;
                //gProgress.SetProgress(++i, nb);
                switch (xe.Name.ToString().ToLower())
                {
                    case "filestubedownload":
                        if (!ExecuteXml_FilesTubeDownload(xe))
                            return false;
                        break;
                    case "rapidsharedownload":
                        if (!ExecuteXml_RapidshareDownload(xe))
                            return false;
                        break;
                }
            }
            return true;
        }
        #endregion

        #region ExecuteXml_FilesTubeDownload
        public bool ExecuteXml_FilesTubeDownload(XElement xe)
        {
            gNbDownloadFileToSkip = xe.zAttribValueInt("NbDownloadFileToSkip", 0);
            string[] urls = (from url in xe.Elements("url") select url.zAttribValue("value")).ToArray();
            return FilesTubeDownload(urls);
        }
        #endregion

        #region ExecuteXml_RapidshareDownload
        public bool ExecuteXml_RapidshareDownload(XElement xe)
        {
            gNbDownloadFileToSkip = xe.zAttribValueInt("NbDownloadFileToSkip", 0);
            string fileDownloadDirectory = xe.zAttribValue("DownloadDirectory");
            // $$pb ajouter la lecture de DownloadDirectory
            // $$pb ajouter le filtre DownloadLoaded != true
            string[] urls = (from url in xe.Elements("url") where !url.zAttribValueBool("DownloadLoaded", false) select url.zAttribValue("value")).ToArray();
            return RapidshareDownload(fileDownloadDirectory, urls);
            //cTrace.Trace("RapidshareDownload");
            //cTrace.Trace("  NbDownloadFileToSkip : {0}", gNbDownloadFileToSkip);
            //foreach (string url in urls)
            //    cTrace.Trace("  Url : {0}", url);
        }
        #endregion

        #region GenerateXml_FilesTubeDownload
        public static void GenerateXml_FilesTubeDownload(string pathXml, int nbDownloadFileToSkip, params string[] urls)
        {
            XDocument xd = new XDocument();
            XElement root = new XElement("root");
            xd.Add(root);
            XElement xe = new XElement("FilesTubeDownload", new XAttribute("NbDownloadFileToSkip", nbDownloadFileToSkip));
            root.Add(xe);
            foreach (string url in urls)
            {
                XElement xe2 = new XElement("url", new XAttribute("value", url));
                xe.Add(xe2);
            }
            string dir = Path.GetDirectoryName(pathXml);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            if (File.Exists(pathXml))
            {
                _trace.WriteLine("error file already exist : {0}", pathXml);
                return;
            }
            xd.Save(pathXml);
            _trace.WriteLine("Generate FilesTube task : {0}", pathXml);
        }
        #endregion

        #region GenerateXml_RapidshareDownload
        public static void GenerateXml_RapidshareDownload(string pathXml, int nbDownloadFileToSkip, params string[] urls)
        {
            XDocument xd = new XDocument();
            XElement root = new XElement("root");
            xd.Add(root);
            XElement xe = new XElement("RapidshareDownload", new XAttribute("NbDownloadFileToSkip", nbDownloadFileToSkip));
            root.Add(xe);
            foreach (string url in urls)
            {
                XElement xe2 = new XElement("url", new XAttribute("value", url));
                xe.Add(xe2);
            }
            string dir = Path.GetDirectoryName(pathXml);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            if (File.Exists(pathXml))
            {
                _trace.WriteLine("error file already exist : {0}", pathXml);
                return;
            }
            xd.Save(pathXml);
            _trace.WriteLine("Generate Rapidshare task : {0}", pathXml);
        }
        #endregion

        #region FilesTubeDownload
        public bool FilesTubeDownload(params string[] urls)
        {
            gDownloadedFiles = new List<string>();
            foreach (string url in urls)
            {
                if (gAbortTask)
                {
                    _trace.WriteLine("FilesTubeDownload task aborted");
                    CancelAbortTask();
                    return false;
                }
                HttpLoad(url);
                XElement xe = (from n in gHttp.GetXDocumentResult().Descendants("pre") where string.Equals(n.zAttribValue("id"), "copy_paste_links", StringComparison.InvariantCultureIgnoreCase) select n).FirstOrDefault();
                if (xe == null)
                    throw new RapidshareException("Error element <pre id=\"copy_paste_links\" ...> not found");
                //if (!_Download(xe.zValueList())) return false;
                gFileDownloadDirectory = null;
                if (!_Download(xe.zValueList(SplitOption.All))) return false;
            }
            Unzip();
            return true;
        }
        #endregion

        #region RapidshareDownload(params string[] urls)
        public bool RapidshareDownload(params string[] urls)
        {
            return RapidshareDownload(null, urls);
        }
        #endregion

        #region RapidshareDownload(string fileDownloadDirectory, params string[] urls)
        public bool RapidshareDownload(string fileDownloadDirectory, params string[] urls)
        {
            gFileDownloadDirectory = fileDownloadDirectory;
            gDownloadedFiles = new List<string>();
            if (!_Download(urls)) return false;
            Unzip();
            return true;
        }
        #endregion

        #region _Download(params string[] urls)
        private bool _Download(params string[] urls)
        {
            //gFileDownloadDirectory = null;
            int i = 0;
            int nb = urls.Length;
            foreach (string url in urls)
            {
                gProgress.SetProgress(++i, nb);
                if (!_Download(url)) return false;
            }
            return true;
        }
        #endregion

        #region _Download(string url)
        private bool _Download(string url)
        {
            gError = RapidshareError.NoError;
            gErrorText = null;

            if (gAbortTask)
            {
                _trace.WriteLine("RapidshareDownload aborted");
                CancelAbortTask();
                return false;
            }
            if (gNbDownloadFileToSkip != 0)
            {
                gNbDownloadFileToSkip--;
                _trace.WriteLine("Skip file {0}", url);
                return true;
            }
            if (gLogin == null) throw new RapidshareException("Error login is'nt defined");
            if (gPassword == null) throw new RapidshareException("Error password is'nt defined");
            if (gDownloadDirectory == null) throw new RapidshareException("Error download directory is'nt defined");
            //if (gErrorDirectory == null) throw new RapidshareException("Error error directory is'nt defined");

            //if (gTraceLevel >= 1) cTrace.Trace("Load \"{0}\"", url);
            //gHttp.Load(url);
            gDownloadUrl = url;
            HttpLoad(url);

            if (!LoadPage2()) return false;

            string downloadUrl = LoadPage3();
            if (downloadUrl == null)
            {
                if (!LoadPage4()) return false;
                LoadPage5();
                downloadUrl = LoadPage3();
                if (downloadUrl == null) throw new RapidshareException("Error downloading {0}", url);
            }
            return DownloadFile(downloadUrl);
        }
        #endregion

        #region Unzip
        public void Unzip()
        {
            SortedList<string, object> files = new SortedList<string, object>();

            //if (gUnzipDirectory == null) return;
            foreach (string rarPath in gDownloadedFiles)
            {
                cChrono chrono = new cChrono();
                chrono.Start();
                //if (gDownloadedFiles.Count == 0) return;

                //string rarPath = @"c:\__Download\z\_zip\aa\Young_And_Glamorous_2009_Dvdrip__Disc_2_.part1.rar";
                //string rarPath = gDownloadedFiles[0];
                if (!string.Equals(cu.PathGetExt(rarPath), ".rar", StringComparison.InvariantCultureIgnoreCase)) return;
                string file = cu.PathGetFile(rarPath);
                //if (file.EndsWith(".part1", StringComparison.InvariantCultureIgnoreCase)) file = file.Substring(0, file.Length - 6);
                Match match = grxFilenamePart.Match(file);
                if (match.Success) file = match.Groups[1].Value;
                if (files.ContainsKey(file)) continue;
                files.Add(file, null);
                //string rarOutputDirectory = Path.Combine(gUnzipDirectory, file);
                string rarOutputDirectory = Path.Combine(Path.GetDirectoryName(rarPath), "Unzip");
                //if (Directory.Exists(rarOutputDirectory)) throw new RapidshareException("Error during unzip directory already exist \"{0}\"", rarOutputDirectory);
                if (!Directory.Exists(rarOutputDirectory)) Directory.CreateDirectory(rarOutputDirectory);

                using (Chilkat.Rar rar = new Chilkat.Rar())
                {
                    if (!rar.Open(rarPath)) throw new RapidshareException("Error during opening rar file \"{0}\" : {1}", rarPath, rar.LastErrorText);
                    _trace.WriteLine("Unrar \"{0}\" to \"{1}\"", rarPath, rarOutputDirectory);
                    gProgress.SetProgressText("Unrar \"{0}\" to \"{1}\"", rarPath, rarOutputDirectory);
                    if (!rar.Unrar(rarOutputDirectory)) throw new RapidshareException("Error during unrar file \"{0}\" : {1}", rarPath, rar.LastErrorText);
                }
                gDownloadedDirectories.Add(rarOutputDirectory);
                chrono.Stop();
                _trace.WriteLine("Unrar duration {0}", chrono.TotalTimeString);
            }
        }
        #endregion

        #region GetError
        public void GetError()
        {
            //<div class="klappbox">
            //
            //<!-- E#4 -->The file could not be found.  Please check the download link.
            //<!-- E#4 -->This file is suspected to contain illegal content and has been blocked.  After the file has been blocked for 7 days it will automatically be deleted, if the block is not removed by RapidShare.  For this reason, a download of this file is currently not possible.
            //
            //  You want to download the following file:
            //  <p class="downloadlink">...</p>
            //  <p><!-- E#9 -->You have exceeded the download limit.</p>
            //  <p>Your account is credited with 5 Gigabytes per day. 5 Gigabytes are equal to 5.000.000.000 Bytes.</p>


            //RapidshareError error = RapidshareError.NoError;
            //errorText = null;
            gError = RapidshareError.NoError;
            gErrorText = null;

            XElement xe = (from e in gHttp.GetXDocumentResult().Descendants("div") where string.Equals(e.zAttribValue("class"), "klappbox", StringComparison.InvariantCultureIgnoreCase) select e).FirstOrDefault();
            if (xe != null)
            {
                gError = RapidshareError.UnknowError;
                //string s = xe.zFirstValue();
                string[] values = xe.zFirstValues(3, StringFormatOption.TrimAll | StringFormatOption.ReplaceNewLineWithSpace, GetErrorNodeFilter);
                string s = null;
                if (values.Length > 0)
                {
                    s = values[0];
                    gError = GetRapidshareError(s);
                    if (gError == RapidshareError.UnknowError)
                    {
                        if (s.StartsWith("You want to download the following file", StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (values.Length > 1)
                            {
                                //You have exceeded the download limit.
                                s = values[1];
                                gError = GetRapidshareError(s);
                                if (gError == RapidshareError.DownloadLimitExceeded && values.Length > 2)
                                    s += " " + values[2];
                            }
                        }
                    }
                    gErrorText = s;
                }
                //cTrace.Trace("Error : {0} - {1} - {2}", error, s, gHttp.Url);
            }
            //if (gError == RapidshareError.FileNotFound || gError == RapidshareError.FileRemoved || gError == RapidshareError.IllegalContentFileBlocked)
            //    MoveExecuteXmlToErrorDirectory();
        }
        #endregion

        #region //MoveExecuteXmlToErrorDirectory
        //private void MoveExecuteXmlToErrorDirectory()
        //{
        //    if (gExecuteXmlPath == null) return;
        //    string executeXmlPath2 = Path.Combine(gErrorDirectory, Path.GetFileName(gExecuteXmlPath));
        //    executeXmlPath2 = GetNewFilename(executeXmlPath2);
        //    File.Move(gExecuteXmlPath, executeXmlPath2);
        //}
        #endregion

        #region GetErrorNodeFilter
        public static bool GetErrorNodeFilter(XNode node)
        {
            // pour exclure :
            //   <script></script>
            //   <p class="downloadlink"></p>
            if (node is XElement)
            {
                XElement xe = (XElement)node;
                if (xe.Name.LocalName.Equals("script", StringComparison.InvariantCultureIgnoreCase))
                    return false;
                //<p class="downloadlink">
                if (xe.Name.LocalName.Equals("p", StringComparison.InvariantCultureIgnoreCase)
                    && string.Equals(xe.zAttribValue("class"), "downloadlink", StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region GetRapidshareError
        public RapidshareError GetRapidshareError(string text)
        {
            InitErrorList();
            if (text != null)
            {
                foreach (var error in gErrors)
                {
                    if (error.Value2 == null) continue;
                    if (text.StartsWith(error.Value2, StringComparison.InvariantCultureIgnoreCase))
                        return error.Value1;
                }
            }
            return RapidshareError.UnknowError;
        }
        #endregion

        #region InitErrorList
        public void InitErrorList()
        {
            if (gErrors != null) return;
            gErrors = new GClass2<RapidshareError,string>[]
            {
                new GClass2<RapidshareError, string>(RapidshareError.UnknowError, null),
                new GClass2<RapidshareError, string>(RapidshareError.FileNotFound, "The file could not be found"),
                new GClass2<RapidshareError, string>(RapidshareError.IllegalContentFileBlocked, "This file is suspected to contain illegal content and has been blocked"),
                new GClass2<RapidshareError, string>(RapidshareError.FileRemoved, "This file has been removed from the server"),
                new GClass2<RapidshareError, string>(RapidshareError.FileRemoved, "Due to a violation of our terms of use, the file has been removed from the server"),
                new GClass2<RapidshareError, string>(RapidshareError.DownloadLimitExceeded, "You have exceeded the download limit")
            };
        }
        #endregion

        #region LoadPage2
        public bool LoadPage2()
        {
            //<input type="submit" value="Premium user" />
            XElement xe = (from e in gHttp.GetXDocumentResult().Descendants("input") where string.Equals(e.zAttribValue("value"), "Premium user", StringComparison.InvariantCultureIgnoreCase) select e).FirstOrDefault();
            if (xe == null)
            {
                //<div class="klappbox">
                //<!-- E#4 -->The file could not be found.  Please check the download link.
                //<!-- E#4 -->This file is suspected to contain illegal content and has been blocked.  After the file has been blocked for 7 days it will automatically be deleted, if the block is not removed by RapidShare.  For this reason, a download of this file is currently not possible.
                //xe = (from e in gHttp.GetXDocumentResult().Descendants("div") where string.Equals(e.zAttribValue("class"), "klappbox", StringComparison.InvariantCultureIgnoreCase) select e).FirstOrDefault();
                //if (xe != null)
                //{
                //    string s = xe.zFirstValue();
                //    if (s != null && (s.StartsWith("The file could not be found", StringComparison.InvariantCultureIgnoreCase)
                //        || s.StartsWith("This file is suspected to contain illegal content and has been blocked", StringComparison.InvariantCultureIgnoreCase)
                //        || s.StartsWith("This file has been removed from the server", StringComparison.InvariantCultureIgnoreCase)
                //        ))
                //    {
                //        cTrace.Trace("The file could not be found : {0}", gHttp.Url);
                //        return false;
                //    }
                //    cTrace.Trace("Error : {0} - {1}", s, gHttp.Url);
                //    return false;
                //}
                //string s;
                //RapidshareError error = GetError(out s);
                GetError();
                if (gError != RapidshareError.NoError)
                {
                    _trace.WriteLine("Error : {0} - {1} - {2}", gError, gErrorText, gHttp.Url);
                    return false;
                }
                else
                    throw new RapidshareException("input Premium user not found");
            }
            xe = xe.Ancestors("form").FirstOrDefault();
            if (xe == null) throw new RapidshareException("form input Premium user not found");
            HtmlForm form = HtmlForm.GetForm(xe);

            #region post ...
            //POST /files/268690518/Young_And_Glamorous_2009_Dvdrip__Disc_2_.part1.rar HTTP/1.1
            //Host: rs766.rapidshare.com
            //Connection: keep-alive
            //User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US) AppleWebKit/532.6 (KHTML, like Gecko) Chrome/4.0.267.0 Safari/532.6
            //Referer: http://rapidshare.com/files/268690518/Young_And_Glamorous_2009_Dvdrip__Disc_2_.part1.rar
            //Content-Length: 16
            //Cache-Control: max-age=0
            //Origin: http://rapidshare.com
            //Content-Type: application/x-www-form-urlencoded
            //Accept: application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5
            //Accept-Encoding: gzip,deflate,sdch
            //Accept-Language: en-US,en;q=0.8
            //Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.3
            //dl.start=PREMIUM
            #endregion

            //string request = form.GetRequest();
            string parameters = form.GetRequestParameters();
            gHttp.Method = form.Method;
            gHttp.Referer = gHttp.Url;
            gHttp.Content = parameters;
            string url = form.Action;
            //if (gTraceLevel >= 1) cTrace.Trace("Load \"{0}\", \"{1}\"", url, parameters);
            //gHttp.Load(url);
            HttpLoad(url);
            return true;
        }
        #endregion

        #region LoadPage3
        public string LoadPage3()
        {
            XElement xe = (from e in gHttp.GetXDocumentResult().Descendants("form") where string.Equals(e.zAttribValue("name"), "dlf", StringComparison.InvariantCultureIgnoreCase) select e).FirstOrDefault();
            if (xe == null)
            {
                if (gTraceLevel >= 1) _trace.WriteLine("form dlf not found");
                return null;
            }
            HtmlForm form = HtmlForm.GetForm(xe);

            string parameters = form.GetRequestParameters();

            #region post ...
            //POST /files/268690518/dl/Young_And_Glamorous_2009_Dvdrip__Disc_2_.part1.rar HTTP/1.1
            //Host: rs766gc.rapidshare.com
            //Connection: keep-alive
            //User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US) AppleWebKit/532.6 (KHTML, like Gecko) Chrome/4.0.267.0 Safari/532.6
            //Referer: http://rs766.rapidshare.com/files/268690518/Young_And_Glamorous_2009_Dvdrip__Disc_2_.part1.rar
            //Content-Length: 19
            //Cache-Control: max-age=0
            //Origin: http://rs766.rapidshare.com
            //Content-Type: application/x-www-form-urlencoded
            //Accept: application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5
            //Accept-Encoding: gzip,deflate,sdch
            //Cookie: enc=B583484FD07DD2163F1C3424D0E5C06CE45AD0643BB32AAE32C1F6E7300F98BCF788B867D990AC7727ED551DD24ABF61
            //Accept-Language: en-US,en;q=0.8
            //Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.3
            //mirror=on&x=67&y=52HTTP/1.1 200 OK
            //Date: Sat, 02 Jan 2010 14:03:44 GMT
            //Connection: close
            //Content-Type: application/octet-stream
            //Accept-Ranges: bytes
            //Content-Disposition: Attachment; filename=Young_And_Glamorous_2009_Dvdrip__Disc_2_.part1.rar
            //Content-Length: 209715200
            #endregion

            gHttp.Method = form.Method;
            gHttp.Referer = gHttp.Url;
            gHttp.Content = parameters;
            return form.Action;
        }
        #endregion

        #region DownloadFile
        public bool DownloadFile(string url)
        {

            //if (gTraceLevel >= 1) cTrace.Trace("Load \"{0}\", \"{1}\"", url, parameters);
            cChrono chrono = new cChrono();
            chrono.Start();
            //gHttp.Load(url);
            HttpLoad(url);
            WebHeaderCollection headers = gHttp.Response.Headers;
            //wr.Print("Headers.Count {0}", gHttp.Response.Headers.Count);
            //wr.View(http.Response.Headers);
            //List<GClass2<string, string>> headerValues = new List<GClass2<string, string>>();
            //var q = from h in http.Response.Headers.Keys select h;
            //for (int i = 0; i < headers.Count; i++)
            //    headerValues.Add(new GClass2<string, string>() { Value1 = headers.Keys[i], Value2 = headers[i] });
            //wr.View(headerValues);


            //Content-Disposition: Attachment; filename=2009_Dvdrip__Disc_2_.part1.rar
            //Content-Length: 209715200
            string contentDisposition = headers["Content-Disposition"];
            string file = null;
            if (contentDisposition != null)
            {
                Match match = grxFilename.Match(contentDisposition);
                if (match.Success) file = match.Groups[1].Value;
            }
            if (file == null)
            {
                Uri uri = new Uri(gHttp.Url);
                file = uri.Segments[uri.Segments.Length - 1];
            }
            //string contentLength = headers[HttpResponseHeader.ContentLength];
            long l = gHttp.Response.ContentLength;
            //if (contentLength != null) l = int.Parse(contentLength);
            //cTrace.Trace("Download rapidshare file {0} ({1}) to {2}", file, contentLength, gDownloadDirectory);

            GetDownloadDirectory(file);
            //if (gFileDownloadDirectory == null)
            //{
            //    gFileDownloadDirectory = cu.GetNewDirectory(Path.Combine(gDownloadDirectory, file));

            //    string executeXmlPath2 = Path.Combine(gFileDownloadDirectory, Path.GetFileName(gExecuteXmlPath));
            //    File.Copy(gExecuteXmlPath, executeXmlPath2);
            //}

            //if (gExecuteXmlPath != null)
            //{
            //    string executeXmlPath2 = Path.Combine(gFileDownloadDirectory, Path.GetFileName(gExecuteXmlPath));
            //    File.Move(gExecuteXmlPath, executeXmlPath2);
            //    gExecuteXmlPath = null;
            //}

            _trace.WriteLine("Download rapidshare file {0} ({1}) to {2}", file, l, gFileDownloadDirectory);
            gProgress.SetProgressText("Download rapidshare file {0}", file);
            gProgressDetail.SetProgressText("Download rapidshare file {0}", file);

            string path = Path.Combine(gFileDownloadDirectory, file);
            if (gAbortTask || !gHttp.LoadToFile(path))
            {
                _trace.WriteLine("Download aborted");
                CancelAbortTask();
                return false;
            }
            SetFileAsDownloaded();
            gDownloadedFiles.Add(path);
            chrono.Stop();
            _trace.WriteLine("Download duration {0}", chrono.TotalTimeString);
            return true;
        }
        #endregion

        #region GetDownloadDirectory
        private void GetDownloadDirectory(string file)
        {
            if (gFileDownloadDirectory != null) return;

            gFileDownloadDirectory = cu.GetNewDirectory(Path.Combine(gDownloadDirectory, file));

            if (gExecuteXmlPath == null) return;
            string executeXmlPath2 = Path.Combine(gFileDownloadDirectory, Path.GetFileName(gExecuteXmlPath));
            File.Copy(gExecuteXmlPath, executeXmlPath2);

            gExecuteXmlElement.zSetValue("DownloadDirectory", gFileDownloadDirectory);
            //gExecuteXmlElement.Document.Save(gExecuteXmlPath);

            string filename = Path.GetFileName(gExecuteXmlPath);
            //executeXmlPath2 = Path.Combine(Path.GetDirectoryName(gExecuteXmlPath), "_RunningTask_" + filename);
            gExecuteXmlElement.zSetValue("SourceFile", filename);
            gExecuteXmlElement.Document.Save(gExecuteXmlPath);
            //File.Delete(gExecuteXmlPath);
            //gExecuteXmlPath = executeXmlPath2;
        }
        #endregion

        #region SetFileAsDownloaded
        private void SetFileAsDownloaded()
        {
            if (gExecuteXmlPath == null) return;
            //gExecuteXmlElement.zSetValue(string.Format("url[value=\"{0}\"]/@DownloadLoaded", gDownloadUrl), "true");
            string s = string.Format("url[@value=\"{0}\"]", gDownloadUrl);
            XElement xeUrl = gExecuteXmlElement.zXPathElement(s);
            if (xeUrl == null) throw new RapidshareException("error element <url> not found : {0}", s);
            xeUrl.zSetValue("DownloadLoaded", "true");
            gExecuteXmlElement.Document.Save(gExecuteXmlPath);
        }
        #endregion

        #region LoadPage4
        public bool LoadPage4()
        {
            XElement xe = (from e in gHttp.GetXDocumentResult().Descendants("input") where string.Equals(e.zAttribValue("name"), "premiumlogin", StringComparison.InvariantCultureIgnoreCase) select e).FirstOrDefault();
            if (xe == null)
            {
                //xe = (from e in gHttp.GetXDocumentResult().Descendants("div") where string.Equals(e.zAttribValue("class"), "klappbox", StringComparison.InvariantCultureIgnoreCase) select e).FirstOrDefault();
                //if (xe != null)
                //{
                //    xe = xe.Elements("p").ElementAt(1);
                //    if (xe != null)
                //    {
                //        if (string.Equals(xe.zFirstValue(), "You have exceeded the download limit", StringComparison.InvariantCultureIgnoreCase))
                //            throw new RapidshareException("You have exceeded the download limit");
                //    }
                //}
                //string s;
                //RapidshareError error = GetError(out s);
                //if (gError != RapidshareError.NoError)
                //    throw new RapidshareException(gErrorText);
                GetError();
                if (gError != RapidshareError.NoError)
                {
                    _trace.WriteLine("Error : {0} - {1} - {2}", gError, gErrorText, gHttp.Url);
                    return false;
                }
                else
                    throw new RapidshareException("input premiumlogin not found");
            }
            xe = xe.Ancestors("form").FirstOrDefault();
            if (xe == null) throw new RapidshareException("form input premiumlogin not found");
            //wr.Print(xe.ToString());
            HtmlForm form = HtmlForm.GetForm(xe);
            form.SetValue("accountid", gLogin);
            form.SetValue("password", gPassword);

            string parameters = form.GetRequestParameters();
            gHttp.Method = form.Method;
            gHttp.Referer = gHttp.Url;
            gHttp.Content = parameters;
            string url = form.Action;
            //if (gTraceLevel >= 1) cTrace.Trace("Load \"{0}\", \"{1}\"", url, parameters);
            //gHttp.Load(url);
            HttpLoad(url);
            return true;
        }
        #endregion

        #region LoadPage5
        public void LoadPage5()
        {
            XElement xe = (from e in gHttp.GetXDocumentResult().Descendants("input") where string.Equals(e.zAttribValue("name"), "dl.start", StringComparison.InvariantCultureIgnoreCase) select e).FirstOrDefault();
            if (xe == null) throw new RapidshareException("input dl.start not found");
            xe = xe.Ancestors("form").FirstOrDefault();
            if (xe == null) throw new RapidshareException("form input dl.start not found");
            HtmlForm form = HtmlForm.GetForm(xe);

            string parameters = form.GetRequestParameters();
            gHttp.Method = form.Method;
            gHttp.Referer = gHttp.Url;
            gHttp.Content = parameters;
            string url = form.Action;
            //if (gTraceLevel >= 1) cTrace.Trace("Load \"{0}\", \"{1}\"", url, parameters);
            //gHttp.Load(url);
            HttpLoad(url);
        }
        #endregion
    }
}
