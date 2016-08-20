using System;
using System.Collections.Generic;
using System.Security.Permissions;
using pb.Data.Xml;
using pb.IO;

namespace pb.Compiler
{
    public partial class RunSource : MarshalByRefObject, IRunSource
    {
        private static string _defaultSuffixProjectName = ".project.xml";
        private static string __sourceSuffixName = ".run.cs";
        //private ITrace _trace = null;
        //private string gsDir = null;
        private string _dataDirectory = null;                                             // not used inside RunSource class

        private SortedList<string, object> gData = new SortedList<string, object>();      // not used inside RunSource class

        //private bool gbIncludeActive = true;
        //private SortedList<string, object> gIncludes = new SortedList<string, object>();
        private XmlConfig _runSourceConfig = null;
        private bool _refreshRunSourceConfig = false;

        // project variables
        private string _sourceFile = null;
        //private string _projectFile = null;
        //private string _projectDirectory = null;
        //private XmlConfig _projectConfig = null;
        //private bool _refreshProjectConfig = false;

        // projectDefaultValues
        private string _defaultProjectFile = null;
        private XmlConfig _defaultProjectXmlConfig = null;
        private CompilerProjectReader _defaultProject = null;
        //private ResourceCompiler _resourceCompiler = null;

        //private AppDomain gDomain = null;
        //private const string gsDefaultAssemblyName = "WRunSource";
        //private const string gsDefaultAssemblyName = "RunSource";
        //private string gsAssemblyPath = null;
        //private MethodInfo gMethodRun = null;
        //private MethodInfo gMethodEnd = null;

        //private static readonly Encoding gIsoEncoding = Encoding.GetEncoding("iso-8859-1");

        //public delegate void fMessageEvent(string sMsg, params object[] prm);
        //public fMessageEvent MessageEvent;
        //public event MessageSendEvent MessageSend;

        //public delegate void fResultEvent(DataTable dt, string sXmlFormat);
        //public fResultEvent ResultEvent;
        //public delegate void fDataSetResultEvent(DataSet ds, string sXmlFormat);
        //public fDataSetResultEvent DataSetResultEvent;

        //public event TreeViewResultAddEvent TreeViewResultAdd;
        //public event TreeViewResultSelectEvent TreeViewResultSelect;

        //public delegate void XmlDocumentLoadedEvent(XmlDocument xml, string sUrl, Http http);
        //public event XmlDocumentLoadedEvent XmlDocumentLoaded = null;

        private bool gbDisableMessage = false; // si true les messages ne doivent plus être affichés
        public event DisableMessageChangedEvent DisableMessageChanged;

        //public event SetDataTableEvent ErrorResultSet;
        public event ProgressChangeEvent ProgressChange;

        private bool gbDontSelectResultTab = false;

        private string gsProgressTxt = null;
        private int giProgressValue = 0;
        private int giProgressTotal = 0;
        private bool gbProgress_AddProgressValueToMessage = true;
        private bool gbProgress_PutProgressMessageToWindowsTitle = true;
        private int giProgressMinimumMillisecondsBetweenMessage = 0;
        private DateTime gLastProgressTime;

        public RunSource()
        {
            //_trace = pb.Trace.CurrentTrace;
            //_trace.Writed += new WritedEvent(EventWrited);
            //CreateGenerateAndExecute();
            InitRunCode();
        }

        public void Dispose()
        {
            _runSourceInitEndMethods.CallEndMethods();
            if (_currentRunSource == this)
                _currentRunSource = null;
            DisposeData();
            if (gdtResult != null)
            {
                gdtResult.Dispose();
                gdtResult = null;
            }
            AssemblyResolve.Stop();
            //_trace.Writed -= new WritedEvent(EventWrited);
        }

        private void DisposeData()
        {
            foreach (object data in gData.Values)
            {
                if (data is IDisposable)
                {
                    ((IDisposable)data).Dispose();
                }
            }
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            // from http://stackoverflow.com/questions/2410221/appdomain-and-marshalbyrefobject-life-time-how-to-avoid-remotingexception
            return null;
        }

        public void SetAsCurrentRunSource()
        {
            _currentRunSource = this;
        }

        public string SourceFile { get { return _sourceFile; } set { _sourceFile = value; } }
        public string ProjectFile { get { return _projectFile; } }
        // set { _projectDirectory = value; }

        public string DataDirectory { get { return _dataDirectory; } set { _dataDirectory = value; } }

        public bool DisableMessage
        {
            get { return gbDisableMessage; }
            set
            {
                if (gbDisableMessage != value)
                {
                    gbDisableMessage = value;
                    if (DisableMessageChanged != null)
                        DisableMessageChanged(value);
                }
            }
        }

        public bool DontSelectResultTab
        {
            get { return gbDontSelectResultTab; }
            set { gbDontSelectResultTab = value; }
        }

        public string ProgressTxt
        {
            get { return gsProgressTxt; }
        }

        public int ProgressValue
        {
            get { return giProgressValue; }
        }

        public int ProgressTotal
        {
            get { return giProgressTotal; }
        }

        public bool Progress_AddProgressValueToMessage
        {
            get { return gbProgress_AddProgressValueToMessage; }
            set { gbProgress_AddProgressValueToMessage = value; Progress(); }
        }

        public bool Progress_PutProgressMessageToWindowsTitle
        {
            get { return gbProgress_PutProgressMessageToWindowsTitle; }
            set { gbProgress_PutProgressMessageToWindowsTitle = value; }
        }

        public int Progress_MinimumMillisecondsBetweenMessage
        {
            get { return giProgressMinimumMillisecondsBetweenMessage; }
            set { giProgressMinimumMillisecondsBetweenMessage = value; }
        }

        public SortedList<string, object> Data
        {
            get { return gData; }
        }

        public string SortResult
        {
            get
            {
                if (gdtResult == null)
                    return null;
                return gdtResult.DefaultView.Sort;
            }

            set
            {
                if (gdtResult == null)
                    return;
                gdtResult.DefaultView.Sort = value;
                //if (!gbDisableResultEvent && ResultEvent != null) ResultEvent(gdtResult, gsXmlResultFormat);
                if (!gbDisableResultEvent && GridResultSetDataTable != null)
                    GridResultSetDataTable(gdtResult, gsXmlResultFormat);
            }
        }

        public string FilterResult
        {
            get
            {
                if (gdtResult == null)
                    return null;
                return gdtResult.DefaultView.RowFilter;
            }

            set
            {
                if (gdtResult == null)
                    return;
                gdtResult.DefaultView.RowFilter = value;
                //if (!gbDisableResultEvent && ResultEvent != null) ResultEvent(gdtResult, gsXmlResultFormat);
                if (!gbDisableResultEvent && GridResultSetDataTable != null)
                    GridResultSetDataTable(gdtResult, gsXmlResultFormat);
            }
        }

        public bool DisableResultEvent
        {
            get { return gbDisableResultEvent; }
            set { gbDisableResultEvent = value; }
        }

        public void Init(string configFile)
        {
            SetRunSourceConfig(configFile);
            InitCompiler();
        }

        public void SetRunSourceConfig(string configFile)
        {
            _runSourceConfig = new XmlConfig(configFile);
            _refreshRunSourceConfig = false;
            //XmlConfigElement projectDefaultValues = _runSourceConfig.GetConfigElement("ProjectDefaultValues");
            //if (projectDefaultValues != null)
            //    //_defaultProject = new CompilerProject(projectDefaultValues, _runSourceConfig.ConfigPath);
            //    _defaultProject = new CompilerProject(projectDefaultValues);
            //else
            //    _defaultProject = null;
        }

        public void InitCompiler()
        {
            XmlConfig runSourceConfig = GetRunSourceConfig();
            CompilerManager.Current.Init(runSourceConfig.GetConfigElementExplicit("CompilerConfig"));
            CompilerManager.Current.AddCompiler("CSharp1", () => new CSharp1Compiler());
            CompilerManager.Current.AddCompiler("CSharp5", () => new CSharp5Compiler(CompilerManager.Current.FrameworkDirectories, CompilerManager.Current.MessageFilter));
            CompilerManager.Current.AddCompiler("JScript", () => new JScriptCompiler());
            //_resourceCompiler = new ResourceCompiler(CompilerManager.Current.ResourceCompiler);
            RunSourceInitEndMethods.TraceRunOnce = runSourceConfig.Get("TraceInitEndOnceMethods").zTryParseAs(false);
            RunSourceInitEndMethods.TraceRunAlways = runSourceConfig.Get("TraceInitEndAlwaysMethods").zTryParseAs(false);
        }

        public XmlConfig GetRunSourceConfig()
        {
            if (_runSourceConfig != null)
            {
                if (_refreshRunSourceConfig)
                    _runSourceConfig.Refresh();
            }
            _refreshRunSourceConfig = false;
            return _runSourceConfig;
        }

        public void StartAssemblyResolve()
        {
            XmlConfig config = GetRunSourceConfig();
            if (config != null)
            {
                AssemblyResolve.TraceAssemblyResolve = config.Get("TraceAssemblyResolve").zTryParseAs(false);
                //AssemblyResolve.TraceAssemblyLoad = config.Get("TraceAssemblyLoad").zTryParseAs(false);
                AssemblyResolve.UpdateAssembly = config.Get("UpdateAssembly").zTryParseAs(false); ;
                AssemblyResolve.UpdateSubDirectory = config.Get("UpdateAssemblySubDirectory", AssemblyResolve.UpdateSubDirectory); ;
                AssemblyResolve.TraceUpdateAssembly = config.Get("TraceUpdateAssembly").zTryParseAs(false);
            }
            AssemblyResolve.Start();
        }

        // $$ProjectDefaultValues disable
        // used to compile without project
        public CompilerProjectReader GetDefaultProject()
        {
            string projectFile = GetRunSourceConfig().Get("DefaultProject");
            bool createCompilerProject = false;
            if (projectFile == null)
            {
                _defaultProjectFile = null;
                _defaultProjectXmlConfig = null;
                _defaultProject = null;
                Trace.WriteLine("no default project");
            }
            else if (projectFile != _defaultProjectFile)
            {
                _defaultProjectFile = projectFile;
                _defaultProjectXmlConfig = new XmlConfig(projectFile);
                createCompilerProject = true;
                Trace.WriteLine("create default project from \"{0}\"", _defaultProjectFile);
            }
            else
            {
                createCompilerProject = _defaultProjectXmlConfig.Refresh();
                if (createCompilerProject)
                    Trace.WriteLine("refresh default project from \"{0}\"", _defaultProjectFile);
            }
            if (createCompilerProject)
                _defaultProject = CompilerProjectReader.Create(_defaultProjectXmlConfig.GetConfigElement("/AssemblyProject"));
            return _defaultProject;
        }

        public CompilerProjectReader GetProjectCompilerProject()
        {
            XmlConfig config = GetProjectConfig();
            if (config != null)
                return CompilerProjectReader.Create(GetProjectConfig().GetConfigElementExplicit("/AssemblyProject"));
            else
                return null;
        }

        public string SetProjectFromSource()
        {
            return SetProject(_sourceFile);
        }

        public string SetProject(string file)
        {
            _runSourceInitEndMethods.CallEndMethods();
            _runSourceInitEndMethods.CallInitRunOnce = true;
            if (file != null)
            {
                // get project variable : "$//Root$\Source\..."
                //Trace.WriteLine("SetProject : \"{0}\"", file);
                //string newFile;
                //if (!new XmlConfig(_projectFile).XDocument.Root.zTryGetVariableValue(file, out newFile))
                //    throw new PBException("cant set project \"{0}\"", file);
                //file = newFile;
                file = GetProjectVariableValue(file, throwError: true);
                //Trace.WriteLine("SetProject : \"{0}\"", file);

                //__sourceSuffixName
                if (file.ToLower().EndsWith(__sourceSuffixName.ToLower()))
                    file = file.Substring(0, file.Length - __sourceSuffixName.Length) + zPath.GetExtension(file);

                if (!file.ToLower().EndsWith(_defaultSuffixProjectName.ToLower()))
                    file = zpath.PathSetFileName(file, zPath.GetFileNameWithoutExtension(file) + _defaultSuffixProjectName);
                _projectFile = GetFilePath(file);
                Trace.WriteLine("set project \"{0}\"", _projectFile);
                _projectDirectory = zPath.GetDirectoryName(_projectFile);
            }
            else
            {
                Trace.WriteLine("set project as null");
                _projectFile = null;
            }
            _projectConfig = null;
            return _projectFile;
        }

        //public void CopyProjectSourceFiles(string projectName, string destinationDirectory)
        //{
        //    string pathProject = GetPathProject(projectName);
        //    Trace.WriteLine("Copy project source files \"{0}\" to \"{1}\"", pathProject, destinationDirectory);
        //    Compiler compiler = CreateProjectCompiler(pathProject);
        //    compiler.CopySourceFiles(destinationDirectory.zRootPath(zPath.GetDirectoryName(pathProject)));
        //}

        //public void ZipProjectSourceFiles(string projectName, string zipFile)
        //{
        //    string pathProject = GetPathProject(projectName);
        //    Trace.WriteLine("Zip project source files \"{0}\" to \"{1}\"", pathProject, zipFile);
        //    Compiler compiler = CreateProjectCompiler(pathProject);
        //    compiler.ZipSourceFiles(zipFile.zRootPath(zPath.GetDirectoryName(pathProject)));
        //}

        public IProjectCompiler CompileProject(string projectName)
        {
            // - compile assembly project (like runsource.dll.project.xml) and runsource project (like download.project.xml)
            // - for assembly project use CompilerDefaultValues from runsource.runsource.config.xml runsource.runsource.config.local.xml
            // - for runsource project use CompilerDefaultValues from runsource.runsource.config.xml runsource.runsource.config.local.xml

            //Compiler compiler = new Compiler();
            //projectName = GetProjectVariableValue(projectName, throwError: true);
            //string pathProject = GetFilePath(projectName);
            //compiler.DefaultDir = zPath.GetDirectoryName(pathProject);
            //// CompilerDefaultValues from runsource.runsource.config.xml runsource.runsource.config.local.xml
            //compiler.SetParameters(GetRunSourceConfigCompilerDefaultValues(), dontSetOutput: true);
            //Trace.WriteLine("Compile project \"{0}\"", pathProject);
            //compiler.SetParameters(CompilerProject.Create(new XmlConfig(pathProject).GetConfigElementExplicit("/AssemblyProject")));

            string pathProject = GetPathProject(projectName);
            Trace.WriteLine("Compile project \"{0}\"", pathProject);
            //ProjectCompiler compiler = CreateProjectCompiler(pathProject);
            ProjectCompiler compiler = ProjectCompiler.Create(pathProject, CompilerManager.Current.Win32ResourceCompiler, CompilerManager.Current.ResourceCompiler);
            //compiler.RunsourceSourceDirectory = GetRunSourceConfig().Get("UpdateRunSource/UpdateDirectory").zRootPath(zapp.GetEntryAssemblyDirectory());
            compiler.RunsourceSourceDirectory = GetRunSourceConfig().Get("RunsourceSourceDirectory").zRootPath(zapp.GetEntryAssemblyDirectory());

            compiler.Compile();
            string s = null;
            if (!compiler.Success)
            {
                SetResult(compiler.GetCompilerMessagesDataTable());
                s = " with error(s)";
            }
            else
            {
                // trace warning
                compiler.TraceMessages();
                //if (compiler.CopyRunSourceSourceFiles)
                //{
                //    string runsourceDirectory = GetRunSourceConfig().Get("UpdateRunSource/UpdateDirectory").zRootPath(zapp.GetEntryAssemblyDirectory());
                //    if (runsourceDirectory != null)
                //    {
                //        foreach (string directory in compiler.CopyOutputDirectories)
                //        {
                //            Trace.WriteLine("  copy runsource source files from \"{0}\" to \"{1}\"", runsourceDirectory, directory);
                //            foreach (string file in zDirectory.EnumerateFiles(runsourceDirectory, "*" + ProjectCompiler.ZipSourceFilename))
                //                zfile.CopyFileToDirectory(file, directory, options: CopyFileOptions.OverwriteReadOnly | CopyFileOptions.CopyOnlyIfNewer);
                //        }
                //    }
                //}
            }
            Trace.WriteLine("  compiled{0} : {1}", s, compiler.OutputAssembly);
            return compiler;
        }

        private string GetPathProject(string projectName)
        {
            projectName = GetProjectVariableValue(projectName, throwError: true);
            return GetFilePath(projectName);
        }

        //private ProjectCompiler CreateProjectCompiler(string pathProject)
        //{
        //    ProjectCompiler compiler = new ProjectCompiler(_resourceCompiler);
        //    CompilerProjectReader compilerProject = CompilerProjectReader.Create(new XmlConfig(pathProject).GetConfigElementExplicit("/AssemblyProject"));
        //    compiler.SetParameters(compilerProject);
        //    compiler.SetProjectCompilerFile(compilerProject.GetProjectCompilerFile());
        //    return compiler;
        //}

        public void Progress()
        {
            Progress(giProgressValue, giProgressTotal, gsProgressTxt);
        }

        public void Progress(int iCurrent, int iTotal)
        {
            Progress(iCurrent, iTotal, gsProgressTxt);
        }

        public void Progress(string sMessage, params object[] prm)
        {
            Progress(giProgressValue, giProgressTotal, sMessage, prm);
        }

        public void Progress(int value, int total, string message, params object[] prm)
        {
            if (ProgressChange == null) return;
            if (giProgressMinimumMillisecondsBetweenMessage > 0)
            {
                DateTime t = DateTime.Now;
                if (t.Subtract(gLastProgressTime).TotalMilliseconds < giProgressMinimumMillisecondsBetweenMessage) return;
                gLastProgressTime = t;
            }

            if (total < value) total = value;
            string s = "";
            if (message != null)
                s = string.Format(message, prm);
            gsProgressTxt = s;
            giProgressValue = value;
            giProgressTotal = total;
            if (gbProgress_AddProgressValueToMessage)
                s += string.Format(" ({0} / {1})", value, total);
            //if (Trace.CurrentTrace.TraceLevel >= 2)
            //    Trace.WriteLine("{0:yyyy-MM-dd HH:mm:ss} {1}", DateTime.Now, s);
            Trace.WriteLine(2, "{0:yyyy-MM-dd HH:mm:ss} {1}", DateTime.Now, s);
            ProgressChange(value, total, s);
        }
    }
}
