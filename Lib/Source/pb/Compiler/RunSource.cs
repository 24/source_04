using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
//using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using System.Xml.XPath;
using pb.Data.Xml;
using pb.IO;

namespace pb.Compiler
{
    public class RunSource : MarshalByRefObject, IRunSource // IDisposable
    {
        private static RunSource _currentRunSource = null;
        private static string _defaultSuffixProjectName = ".project.xml";
        //private ITrace _trace = null;
        //private string gsDir = null;
        private string _dataDirectory = null;
        private DataTable gdtResult = null;
        private DataSet gdsResult = null;
        private string gsXmlResultFormat = null;

        //private XmlParameters_v1 _xmlParameter = null;

        private SortedList<string, object> gData = new SortedList<string, object>();

        //private bool gbIncludeActive = true;
        //private SortedList<string, object> gIncludes = new SortedList<string, object>();
        private XmlConfig _runSourceConfig = null;
        private CompilerProject _defaultProject = null;  // <ProjectDefaultValues> in _runSourceConfig  (runsource.runsource.config.xml runsource.runsource.config.local.xml)
        //private string gsProjectName = null;
        private string _projectFile = null;
        private string _projectDirectory = null;
        //private XmlConfig gProjectConfig = null;
        //private string gsProjectNameSpace = null;
        //private bool _compileWithoutProject = false;          // used by runsource to be able to run RunSource.CurrentRunSource.Compile_Project() without current project
        //private string gsSourceConfigName = null;       // not used
        //private XmlConfig gSourceConfig = null;         // not used
        //private string gsSourceDir = null;

        private Chrono gExecutionChrono = new Chrono();
        //private AppDomain gDomain = null;
        //private const string gsDefaultAssemblyName = "WRunSource";
        //private const string gsDefaultAssemblyName = "RunSource";
        //private string gsAssemblyPath = null;
        //private MethodInfo gMethodRun = null;
        //private MethodInfo gMethodEnd = null;
        private GenerateAndExecuteManager _generateAndExecuteManager = null;
        private Thread gExecutionThread = null;
        private bool _executionPaused = false;
        private bool _executionAborted = false;

        private static readonly Encoding gIsoEncoding = Encoding.GetEncoding("iso-8859-1");

        //public delegate void fMessageEvent(string sMsg, params object[] prm);
        //public fMessageEvent MessageEvent;
        //public event MessageSendEvent MessageSend;

        //public delegate void fResultEvent(DataTable dt, string sXmlFormat);
        //public fResultEvent ResultEvent;
        //public delegate void fDataSetResultEvent(DataSet ds, string sXmlFormat);
        //public fDataSetResultEvent DataSetResultEvent;

        public event OnPauseEvent OnPauseExecution;
        //public OnAbortEvent OnAbortExecution;
        public OnAbortEvent OnAbortExecution { get; set; }

        //public event WritedEvent2 Writed;
        public event SetDataTableEvent GridResultSetDataTable;
        public event SetDataSetEvent GridResultSetDataSet;

        //public event TreeViewResultAddEvent TreeViewResultAdd;
        //public event TreeViewResultSelectEvent TreeViewResultSelect;

        //public delegate void XmlDocumentLoadedEvent(XmlDocument xml, string sUrl, Http http);
        //public event XmlDocumentLoadedEvent XmlDocumentLoaded = null;


        private bool gbDisableMessage = false; // si true les messages ne doivent plus être affichés
        public event DisableMessageChangedEvent DisableMessageChanged;

        private bool gbDisableResultEvent = false; // si true ResultEvent n'est pas appelé quand un nouveau résultat est disponible
        //public fResultEvent ErrorResultEvent;
        public event SetDataTableEvent ErrorResultSet;
        //public fProgressEvent ProgressEvent;
        public event ProgressChangeEvent ProgressChange;
        public event EndRunEvent EndRun;

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
            CreateGenerateAndExecute();
        }

        public void Dispose()
        {
            if (_currentRunSource == this)
                _currentRunSource = null;
            DisposeData();
            if (gdtResult != null)
            {
                gdtResult.Dispose();
                gdtResult = null;
            }
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

        public static RunSource CurrentRunSource
        {
            get
            {
                if (_currentRunSource == null) _currentRunSource = new RunSource();
                return _currentRunSource;
            }
            set { _currentRunSource = value; }
        }

        public void SetAsCurrentRunSource()
        {
            _currentRunSource = this;
        }

        //public static RunSource CurrentDomainRunSource
        //{
        //    get
        //    {
        //        return (RunSource)AppDomain.CurrentDomain.GetData("RunSource");
        //    }
        //}

        //public ITrace Trace
        //{
        //    get { return _trace; }
        //    set { _trace = value; }
        //}

        public XmlConfig RunSourceConfig
        {
            get { return _runSourceConfig; }
            //set { _runSourceConfig = value; }
        }

        public IGenerateAndExecuteManager GenerateAndExecuteManager { get { return _generateAndExecuteManager; } }
        public string ProjectFile { get { return _projectFile; } }
        public string ProjectDirectory { get { return _projectDirectory; } set { _projectDirectory = value; } }

        //public string ProjectName
        //{
        //    get { return gsProjectName; }
        //    set
        //    {
        //        InitProject(value);
        //    }
        //}

        //private void InitProject(string file)
        //{
        //    gsProjectName = file;
        //    if (file == null)
        //        gsProjectConfigPath = null;
        //    else
        //        gsProjectConfigPath = GetProjectFile(file);
        //}

        //public string GetProjectConfigPath(string projectName)
        //{
        //    string file = Path.GetFileName(projectName);
        //    if (Path.GetExtension(file).ToLower() != ".xml")
        //    {
        //        file = Path.GetFileNameWithoutExtension(projectName);
        //        if (!file.ToLower().EndsWith("_project"))
        //            file += "_project";
        //        file += ".xml";
        //    }
        //    string dir = Path.GetDirectoryName(projectName);
        //    dir = GetFilePath(dir);
        //    if (!dir.EndsWith("\\")) dir += "\\";
        //    return dir + file;
        //}

        //public XmlConfig ProjectConfig
        //{
        //    get { return gProjectConfig; }
        //    set { gProjectConfig = value; }
        //}

        //public string ProjectNameSpace
        //{
        //    get { return gsProjectNameSpace; }
        //    set { gsProjectNameSpace = value; }
        //}

        // not used
        //public XmlConfig Config
        //{
        //    get { return gSourceConfig; }
        //    set
        //    {
        //        gSourceConfig = value;
        //        XmlConfig.CurrentConfig = gSourceConfig;
        //    }
        //}

        //public string SourceDir
        //{
        //    get { return gsSourceDir; }
        //    set
        //    {
        //        gsSourceDir = value;
        //        //if (gsSourceDir != null && !gsSourceDir.EndsWith("\\"))
        //        //    gsSourceDir += "\\";
        //        //if (!Directory.Exists(gsSourceDir))
        //        //    Directory.CreateDirectory(gsSourceDir);
        //    }
        //}

        //public string Dir
        //{
        //    get
        //    {
        //        if (gsDir != null)
        //            return gsDir;
        //        else
        //            return gsSourceDir;
        //    }
        //    set
        //    {
        //        value = value.Replace("?", "");
        //        value = value.Replace("*", "");
        //        string sDrive = "";
        //        if (value.Length >= 2 && char.IsLetter(value[0]) && value[1] == ':')
        //        {
        //            sDrive = value.Substring(0, 2);
        //            value = value.Remove(0, 2);
        //        }
        //        value = value.Replace(":", "");
        //        value = sDrive + value;
        //        value = value.Trim();
        //        if (!value.EndsWith("\\")) value += "\\";
        //        _trace.WriteLine("Dir = \"{0}\";", value);
        //        gsDir = value;
        //        if (!Directory.Exists(gsDir)) Directory.CreateDirectory(gsDir);
        //    }
        //}

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

        public DataTable Result
        {
            get { return gdtResult; }
            set
            {
                gdtResult = value;
                gdsResult = null;
                gsXmlResultFormat = null;
                if (!gbDisableResultEvent && GridResultSetDataTable != null) GridResultSetDataTable(gdtResult, null);
            }
        }

        public DataSet DataSetResult
        {
            get { return gdsResult; }
            //set
            //{
            //    gdsResult = value;
            //    gdtResult = null;
            //    gsXmlResultFormat = null;
            //    //if (!gbDisableResultEvent && DataSetResultEvent != null) DataSetResultEvent(gdsResult, null);
            //    if (!gbDisableResultEvent && GridResultSetDataSet != null) GridResultSetDataSet(gdsResult, null);
            //}
        }

        //public void SetResult(DataTable dt)
        //{
        //    SetResult(dt, null);
        //}

        public void SetResult(DataTable table, string xmlFormat = null)
        {
            gdtResult = table;
            gsXmlResultFormat = xmlFormat;
            gdsResult = null;
            if (!gbDisableResultEvent && GridResultSetDataTable != null)
                GridResultSetDataTable(gdtResult, gsXmlResultFormat);
        }

        public void SetResult(DataSet dataSet, string xmlFormat = null)
        {
            gdsResult = dataSet;
            gsXmlResultFormat = xmlFormat;
            gdtResult = null;
            if (!gbDisableResultEvent && GridResultSetDataSet != null)
                GridResultSetDataSet(gdsResult, gsXmlResultFormat);
        }

        //public DataTable View<T>(T v)
        //{
        //    DataTable table;
        //    if (!(v is DataTable))
        //        table = v.zToDataTable();
        //    else
        //        table = v as DataTable;
        //    SetResult(table);
        //    return table;
        //}

        //public DataTable ViewType<T>(T v)
        //{
        //    DataTable dt = v.zTypeToDataTable();
        //    SetResult(dt);
        //    return dt;
        //}


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
                if (gdtResult == null) return null;
                return gdtResult.DefaultView.Sort;
            }

            set
            {
                if (gdtResult == null) return;
                gdtResult.DefaultView.Sort = value;
                //if (!gbDisableResultEvent && ResultEvent != null) ResultEvent(gdtResult, gsXmlResultFormat);
                if (!gbDisableResultEvent && GridResultSetDataTable != null) GridResultSetDataTable(gdtResult, gsXmlResultFormat);
            }
        }

        public string FilterResult
        {
            get
            {
                if (gdtResult == null) return null;
                return gdtResult.DefaultView.RowFilter;
            }

            set
            {
                if (gdtResult == null) return;
                gdtResult.DefaultView.RowFilter = value;
                //if (!gbDisableResultEvent && ResultEvent != null) ResultEvent(gdtResult, gsXmlResultFormat);
                if (!gbDisableResultEvent && GridResultSetDataTable != null) GridResultSetDataTable(gdtResult, gsXmlResultFormat);
            }
        }

        public bool DisableResultEvent
        {
            get { return gbDisableResultEvent; }
            set { gbDisableResultEvent = value; }
        }

        //public Thread ExecutionThread
        //{
        //    get { return gExecutionThread; }
        //}

        public IChrono ExecutionChrono
        {
            get { return gExecutionChrono; }
        }

        //public bool Abort
        //{
        //    get { return gbAbort; }
        //    set { gbAbort = value; }
        //}

        //public bool Pause
        //{
        //    get { return gbPause; }
        //    set { gbPause = value; }
        //}

        //public void KeepAlive()
        //{
        //}

        public void SetRunSourceConfig(string file)
        {
            _runSourceConfig = new XmlConfig(file);
            XmlConfigElement projectDefaultValues = _runSourceConfig.GetConfigElement("ProjectDefaultValues");
            if (projectDefaultValues != null)
                //_defaultProject = new CompilerProject(projectDefaultValues, _runSourceConfig.ConfigPath);
                _defaultProject = new CompilerProject(projectDefaultValues);
            else
                _defaultProject = null;
        }

        public string SetProjectFile(string file)
        {
            if (file != null)
            {
                if (!file.ToLower().EndsWith(_defaultSuffixProjectName.ToLower()))
                    file = zpath.PathSetFileName(file, Path.GetFileNameWithoutExtension(file) + _defaultSuffixProjectName);
                _projectFile = GetFilePath(file);
                _projectDirectory = Path.GetDirectoryName(_projectFile);
            }
            else
            {
                _projectFile = null;
            }
            return _projectFile;
        }

        public bool IsRunning()
        {
            return gExecutionThread != null;
        }

        public bool IsExecutionPaused()
        {
            return _executionPaused;
        }

        public void PauseExecution(bool pause)
        {
            _executionPaused = pause;
            if (OnPauseExecution != null)
                OnPauseExecution(pause);
        }

        public bool IsExecutionAborted()
        {
            return _executionAborted;
        }

        public void AbortExecution(bool abort)
        {
            if (gExecutionThread == null)
                return;
            _executionAborted = abort;
            if (abort && OnAbortExecution != null)
                OnAbortExecution();
        }

        public void ForceAbortExecution()
        {
            if (gExecutionThread != null)
                gExecutionThread.Abort();
        }

        public bool IsExecutionAlive()
        {
            if (gExecutionThread != null)
                return gExecutionThread.IsAlive;
            else
                return false;
        }

        public void Run(string code, bool useNewThread = true, bool compileWithoutProject = false)
        {
            if (code == "")
                return;

            bool bError = false;
            if (gExecutionThread != null)
                throw new PBException("error program already running");

            bool bOk = false;
            gExecutionChrono = new Chrono();
            try
            {
                AssemblyResolve.Stop();
                AssemblyResolve.Clear();

                ////Compiler compiler = _GenerateCodeAndCompile(generateAndExecute, code, compileWithoutProject);
                ////GenerateAndExecute generateAndExecute = _GenerateCodeAndCompile(code, compileWithoutProject);
                //_GenerateCodeAndCompile(code, compileWithoutProject);
                //if (_generateAndExecute.Compiler.HasError)
                //    return;

                IGenerateAndExecute generateAndExecute = _generateAndExecuteManager.New();
                _GenerateCodeAndCompile_v2(generateAndExecute, code, compileWithoutProject);
                if (generateAndExecute.Compiler.HasError())
                    return;

                //Assembly assembly = _generateAndExecute.Compiler.Results.CompiledAssembly;
                //string sClass = "w";
                //if (gsProjectNameSpace != null)
                //    sClass = gsProjectNameSpace + ".w";
                //Type typeMain = assembly.GetType(sClass);
                //if (typeMain == null)
                //    throw new PBException("class w not found");

                //AssemblyResolve.Start();
                //MethodInfo method = typeMain.GetMethod("Init");
                //if (method != null)
                //{
                //    gExecutionChrono.Start();
                //    method.Invoke(null, null);
                //    gExecutionChrono.Stop();
                //}

                //gMethodRun = typeMain.GetMethod("Run");
                //if (gMethodRun == null)
                //    throw new PBException("function Run not found");

                //gMethodEnd = typeMain.GetMethod("End");

                //if (useNewThread)
                //{
                //    gExecutionThread = new Thread(new ThreadStart(ThreadStart));
                //    gExecutionThread.CurrentCulture = FormatInfo.CurrentFormat.CurrentCulture;
                //    gExecutionThread.SetApartmentState(ApartmentState.STA);
                //    gExecutionThread.Start();
                //}
                //else
                //{
                //    _trace.WriteLine("execute on main thread");
                //    this.ThreadStart();
                //}

                MethodInfo runMethod = generateAndExecute.GetAssemblyRunMethod();
                MethodInfo initMethod = generateAndExecute.GetAssemblyInitMethod();
                MethodInfo endMethod = generateAndExecute.GetAssemblyEndMethod();

                AssemblyResolve.Start();
                if (useNewThread)
                {
                    gExecutionThread = new Thread(new ThreadStart(() => _Run(runMethod, initMethod, endMethod)));
                    gExecutionThread.CurrentCulture = FormatInfo.CurrentFormat.CurrentCulture;
                    gExecutionThread.SetApartmentState(ApartmentState.STA);
                    gExecutionThread.Start();
                }
                else
                {
                    Trace.WriteLine("execute on main thread");
                    _Run(runMethod, initMethod, endMethod);
                }

                bOk = true;
            }
            catch
            {
                bError = true;
                throw;
            }
            finally
            {
                if (!bOk && EndRun != null)
                    EndRun(bError);
            }
        }

        private void CreateGenerateAndExecute()
        {
            _generateAndExecuteManager = new GenerateAndExecuteManager();
        }

        public void GenerateWRSourceAndCompile(string code, bool compileWithoutProject = false)
        {
            //_GenerateCodeAndCompile(code, compileWithoutProject);
            _GenerateCodeAndCompile_v2(_generateAndExecuteManager.New(), code, compileWithoutProject);
        }

        private void _GenerateCodeAndCompile_v2(IGenerateAndExecute generateAndExecute, string code, bool compileWithoutProject = false)
        {
            if (code == "")
                return;

            CompilerProject compilerProject = null;
            if (!compileWithoutProject && _projectFile != null && File.Exists(_projectFile))
            {
                //pb.Trace.WriteLine("RunSource._GenerateCodeAndCompile_v2() : use project \"{0}\"", _projectFile);
                XmlConfig xmlConfig = new XmlConfig(_projectFile);
                //compilerProject = new CompilerProject(xmlConfig.GetElement("Project"), gsProjectConfigPath);
                //compilerProject = new CompilerProject(xmlConfig.GetConfigElementExplicit("Project"));
                XmlConfigElement configElement = xmlConfig.GetConfigElement("Project");
                if (configElement != null)
                    compilerProject = new CompilerProject(configElement);
                else
                    pb.Trace.WriteLine("RunSource._GenerateCodeAndCompile_v2() : element \"Project\" not found in project \"{0}\"", _projectFile);
            }
            //else
            //    pb.Trace.WriteLine("RunSource._GenerateCodeAndCompile_v2() : dont use project");

            if (compilerProject != null)
                generateAndExecute.NameSpace = compilerProject.GetNameSpace();

            if (_defaultProject != null)
                generateAndExecute.AddUsings(_defaultProject.GetUsings());
            if (compilerProject != null)
                generateAndExecute.AddUsings(compilerProject.GetUsings());

            generateAndExecute.GenerateCSharpCode(code);

            SetCompilerValues(generateAndExecute, compilerProject);

            ICompiler compiler = generateAndExecute.Compiler;
            compiler.Compile();
            //if (compiler.ResourceResults.HasError || (compiler.Results != null && compiler.Results.Errors.HasErrors))
            if (compiler.HasError())
            {
                DataTable dtMessage = compiler.GetCompilerMessagesDataTable();
                if (dtMessage != null)
                {
                    gdtResult = dtMessage;
                    gdsResult = null;
                    gsXmlResultFormat = null;
                    if (ErrorResultSet != null)
                        ErrorResultSet(gdtResult, null);
                }
            }
            else
                // trace warning
                compiler.TraceMessages();
                //TraceCompilerMessages(compiler);
        }

        //private void TraceCompilerMessages(ICompiler compiler)
        //{
        //    if (_trace == null)
        //        return;
        //    if (compiler.Results != null)
        //    {
        //        foreach (CompilerError err in compiler.Results.Errors)
        //            Trace.WriteLine("{0} no {1,-6} source \"{2}\" line {3} col {4} \"{5}\"", err.IsWarning ? "warning" : "error", err.ErrorNumber, Path.GetFileName(err.FileName), err.Line, err.Column, err.ErrorText);
        //    }
        //    if (compiler.ResourceResults != null)
        //    {
        //        foreach (ResourceCompilerError err in compiler.ResourceResults.Errors)
        //            Trace.WriteLine("source \"{0}\" \"{1}\"", Path.GetFileName(err.FileName), err.ErrorText);
        //    }
        //}

        //private void _GenerateCodeAndCompile(string code, bool compileWithoutProject = false)
        //{
        //    if (code == "")
        //        return;

        //    //GenerateAndExecute generateAndExecute = new GenerateAndExecute();

        //    // compileWithoutProject is used by runsource to be able to run RunSource.CurrentRunSource.Compile_Project() without current project
        //    //try
        //    //{
        //        //_compileWithoutProject = compileWithoutProject;
        //        CompilerProject compilerProject = null;
        //        if (!compileWithoutProject)
        //        {
        //            XmlConfig xmlConfig = new XmlConfig(gsProjectConfigPath);
        //            compilerProject = new CompilerProject(xmlConfig.GetElement("Project"), gsProjectConfigPath);
        //        }

        //        //gProjectConfig = null;
        //        //gProjectConfig = new XmlConfig(gsProjectConfigPath);
        //        //gsAssemblyPath = GetNewAssemblyPath();
        //        string assemblyPath = GetNewAssemblyPath();


        //        //string pathSource = GenerateWRSource(code, compilerProject);
        //        string file = zpath.PathSetExtension(assemblyPath, ".cs");

        //        if (compilerProject != null)
        //            _generateAndExecute.NameSpace = compilerProject.GetNameSpace();
        //        else
        //            _generateAndExecute.NameSpace = "RunSourceExecute";
        //        _generateAndExecute.ClassName = "w";
        //        _generateAndExecute.FunctionName = "Run";

        //        GenerateCSharpCode(file, code, compilerProject);

        //        //if (pathSource == null)
        //        //    return null;
        //        //return Compile(compilerProject, gsAssemblyPath, file);
        //        _generateAndExecute.Compiler = Compile(compilerProject, assemblyPath, file);
        //        //return generateAndExecute;
        //    //}
        //    //finally
        //    //{
        //    //    _compileWithoutProject = false;
        //    //}
        //}

        //public Compiler Compile(params string[] sPathSources)
        //{
        //    return Compile(null, sPathSources);
        //}

        public void SetCompilerValues(IGenerateAndExecute generateAndExecute, CompilerProject compilerProject)
        {
            ICompiler compiler = generateAndExecute.Compiler;
            if (compilerProject != null)
                compiler.DefaultDir = Path.GetDirectoryName(compilerProject.ProjectFile);
            compiler.zSetCompilerParameters(_runSourceConfig.XDocument.Root.XPathSelectElement("CompilerDefaultValues"));
            compiler.zSetCompilerParameters(_runSourceConfig.XDocument.Root.XPathSelectElement("ProjectDefaultValues"));
            if (compilerProject != null)
                compiler.zSetCompilerParameters(compilerProject.ProjectXmlElement);
            //if (compilerProject != null)
            //    compiler.AddSources(compilerProject.GetSources());
        }

        //public Compiler Compile(CompilerProject compilerProject, string assemblyPath, params string[] pathSources)
        //{
        //    Compiler compiler = new Compiler();
        //    //compiler.DefaultDir = Path.GetDirectoryName(gProjectConfig.ConfigPath);
        //    if (compilerProject != null)
        //        compiler.DefaultDir = Path.GetDirectoryName(compilerProject.ProjectFile);
        //    compiler.SetCompilerParameters(_runSourceConfig.XDocument.Root.XPathSelectElement("CompilerDefaultValues"));
        //    compiler.SetCompilerParameters(_runSourceConfig.XDocument.Root.XPathSelectElement("ProjectDefaultValues"));
        //    //compiler.SetCompilerParameters(gProjectConfig.XDocument.Root.XPathSelectElement("Project"));
        //    if (compilerProject != null)
        //        compiler.SetCompilerParameters(compilerProject.ProjectXmlElement);
        //    compiler.SourceList.Clear();
        //    compiler.AddSources(pathSources);
        //    //compiler.AddSources(GetSourceList());
        //    if (compilerProject != null)
        //        compiler.AddSources(compilerProject.GetSources());
        //    compiler.OutputAssembly = assemblyPath;
        //    compiler.Compile();
        //    if (compiler.ResourceResults.HasError || (compiler.Results != null && compiler.Results.Errors.HasErrors))
        //    {
        //        DataTable dtMessage = compiler.GetCompilerMessagesDataTable();
        //        if (dtMessage != null)
        //        {
        //            gdtResult = dtMessage;
        //            gdsResult = null;
        //            gsXmlResultFormat = null;
        //            if (ErrorResultSet != null) ErrorResultSet(gdtResult, null);
        //        }
        //    }
        //    else
        //        // trace warning
        //        compiler.TraceMessages();

        //    return compiler;
        //}

        public ICompiler Compile_Project(string projectName)
        {
            //Compiler compiler = new Compiler(GetPathFichier(ProjectName));
            //compiler.SetCompilerParameters(_runSourceConfig, "CompilerDefaultValues");
            Compiler compiler = new Compiler();
            string pathProject = GetFilePath(projectName);
            compiler.DefaultDir = Path.GetDirectoryName(pathProject);
            //compiler.SetCompilerParameters(_runSourceConfig, "CompilerDefaultValues");
            compiler.zSetCompilerParameters(_runSourceConfig.XDocument.Root.XPathSelectElement("CompilerDefaultValues"));
            Trace.WriteLine("Compile project \"{0}\"", pathProject);
            compiler.zSetCompilerParameters(XDocument.Load(pathProject).Root);
            compiler.Compile();
            string s = null;
            if (compiler.HasError())
            {
                Result = compiler.GetCompilerMessagesDataTable();
                s = " with error(s)";
            }
            Trace.WriteLine("  compiled{0} : {1}", s, compiler.OutputAssembly);
            return compiler;
        }

        //public Compiler CompileAndRun_Project(string projectName)
        //{
        //    return CompileAndRun_Project(projectName, false);
        //}

        //public Compiler CompileAndRun_Project(string projectName, bool killActiveProcess)
        //{
        //    Compiler compiler = new Compiler();
        //    compiler.DefaultDir = Path.GetDirectoryName(GetFilePath(projectName));
        //    compiler.zSetCompilerParameters(_runSourceConfig.XDocument.Root.XPathSelectElement("CompilerDefaultValues"));
        //    compiler.zSetCompilerParameters(XDocument.Load(GetFilePath(projectName)).Root);
        //    if (killActiveProcess)
        //        compiler.CloseProcess();
        //    compiler.Compile();
        //    string s = "";
        //    if (compiler.HasError)
        //    {
        //        Result = compiler.GetCompilerMessagesDataTable();
        //        s = "with error(s) ";
        //    }
        //    _trace.WriteLine("{0} compiled {1}: {2}", projectName, s, compiler.OutputAssembly);

        //    if (!compiler.HasError && compiler.Results.PathToAssembly != null)
        //    {
        //        Process.Start(compiler.Results.PathToAssembly);
        //        //ProcessStartInfo psi = new ProcessStartInfo(compiler.CompiledAssemblyPath);
        //        //psi.UseShellExecute = true;
        //        ////psi.WindowStyle = ProcessWindowStyle.Hidden;
        //        //Process.Start(psi);
        //    }
        //    return compiler;
        //}

        //private static void DeleteAssemblies()
        //{
        //    zfile.DeleteFiles(GetAssemblyDirectory(), gsDefaultAssemblyName + "*.*", false);
        //}

        public void DeleteGeneratedAssemblies()
        {
            _generateAndExecuteManager.DeleteGeneratedAssemblies();
        }

        //private static string GetNewAssemblyPath()
        //{
        //    string dir = GetAssemblyDirectory();
        //    if (!Directory.Exists(dir))
        //        Directory.CreateDirectory(dir);
        //    int i = zfile.GetLastFileNameIndex(dir) + 1;
        //    return Path.Combine(dir, gsDefaultAssemblyName + string.Format("_{0:00000}", i));
        //}

        //private static string GetAssemblyPath(string sProjectName)
        //{
        //    string sPath = zpath.PathGetFileName(sProjectName);
        //    string sDir = GetAssemblyDirectory();
        //    if (!Directory.Exists(sDir)) Directory.CreateDirectory(sDir);
        //    sPath = sDir + sPath;
        //    sPath = zpath.PathSetExtension(sPath, "");
        //    //gsAssemblyPath = sPath;
        //    return sPath;
        //}

        //private static string GetAssemblyDirectory()
        //{
        //    string sDir = zapp.GetAppDirectory();
        //    if (!sDir.EndsWith("\\")) sDir += "\\";
        //    //sDir += "WRunDll\\";
        //    sDir += "run\\";
        //    return sDir;
        //}

        //private void ThreadStart()
        //{
        //    bool bError = false;
        //    try
        //    {
        //        _executionAborted = false;
        //        try
        //        {
        //            gExecutionChrono.Start();
        //            //gMethodRun.Invoke(null, new object[] { this });
        //            gMethodRun.Invoke(null, null);
        //            gExecutionChrono.Stop();
        //        }
        //        catch (Exception ex)
        //        {
        //            gExecutionChrono.Stop();
        //            bError = true;
        //            _trace.WriteError(ex);
        //        }
        //    }
        //    finally
        //    {
        //        try
        //        {
        //            if (gMethodEnd != null)
        //            {
        //                gExecutionChrono.Start();
        //                gMethodEnd.Invoke(null, null);
        //                gExecutionChrono.Stop();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            bError = true;
        //            _trace.WriteError(ex);
        //        }
        //        finally
        //        {
        //            // modif le 30/03/2013
        //            gExecutionThread = null;
        //            gMethodRun = null;
        //            _executionPaused = false;
        //            _executionAborted = false;
        //            //gExecutionChrono.Stop();
        //            if (EndRun != null) EndRun(bError);
        //            #region AppDomain ...
        //            //******************************************************************************************************************************************************************************
        //            //                                                                              AppDomain
        //            //AppDomain.Unload(gDomain);
        //            //gDomain = null;
        //            //******************************************************************************************************************************************************************************
        //            #endregion
        //        }
        //    }
        //}

        private void _Run(MethodInfo runMethod, MethodInfo initMethod = null, MethodInfo endMethod = null)
        {
            bool bError = false;
            try
            {
                _executionAborted = false;
                try
                {
                    if (initMethod != null)
                    {
                        gExecutionChrono.Start();
                        initMethod.Invoke(null, null);
                        gExecutionChrono.Stop();
                    }

                    gExecutionChrono.Start();
                    runMethod.Invoke(null, null);
                    gExecutionChrono.Stop();
                }
                catch (Exception ex)
                {
                    gExecutionChrono.Stop();
                    bError = true;
                    Trace.CurrentTrace.WriteError(ex);
                }
            }
            finally
            {
                try
                {
                    if (endMethod != null)
                    {
                        gExecutionChrono.Start();
                        endMethod.Invoke(null, null);
                        gExecutionChrono.Stop();
                    }
                }
                catch (Exception ex)
                {
                    gExecutionChrono.Stop();
                    bError = true;
                    Trace.CurrentTrace.WriteError(ex);
                }
                finally
                {
                    gExecutionThread = null;
                    _executionPaused = false;
                    _executionAborted = false;
                    if (EndRun != null)
                        EndRun(bError);
                    #region AppDomain ...
                    //******************************************************************************************************************************************************************************
                    //                                                                              AppDomain
                    //AppDomain.Unload(gDomain);
                    //gDomain = null;
                    //******************************************************************************************************************************************************************************
                    #endregion
                }
            }
        }

        //private void GenerateCSharpCode(string file, string code, CompilerProject compilerProject)
        //{
        //    //if (code == "")
        //    //    return null;
        //    //string codeFile = zpath.PathSetExtension(gsAssemblyPath, ".cs");
        //    using (StreamWriter sw = File.CreateText(file))
        //    {
        //        GenerateCSharpCode generateCode = new GenerateCSharpCode(sw);

        //        // using
        //        if (_defaultProject != null)
        //            generateCode.AddUsings(_defaultProject.GetUsings());
        //        if (compilerProject != null)
        //            generateCode.AddUsings(compilerProject.GetUsings());

        //        // open namespace
        //        generateCode.OpenNameSpace(_generateAndExecute.NameSpace);

        //        // open class
        //        // public static partial class ...
        //        generateCode.OpenClass(_generateAndExecute.ClassName, ClassOptions.Public | ClassOptions.Static | ClassOptions.Partial);

        //        // open function
        //        // public static void Run()
        //        generateCode.WriteLine("public static void {0}()", _generateAndExecute.FunctionName);
        //        generateCode.WriteLine("{");
        //        generateCode.WriteLine(code);
        //        generateCode.WriteLine("}");

        //        // close class
        //        generateCode.CloseClass();

        //        // close namespace
        //        generateCode.CloseNameSpace();
        //    }
        //}

        //private string GenerateWRSource(string sSource, CompilerProject compilerProject)
        //{
        //    sSource = SourceReadInclude(sSource);
        //    if (sSource == "")
        //        return null;

        //    string sPathSource = zpath.PathSetExtension(gsAssemblyPath, ".cs");
        //    string sUsing = GetUsingList();
        //    zfile.WriteFile(sPathSource, sUsing);

        //    gsProjectNameSpace = null;
        //    //gsProjectNameSpace = gProjectConfig.Get("Project/NameSpace");
        //    if (compilerProject != null)
        //        gsProjectNameSpace = compilerProject.GetNameSpace();

        //    string s;
        //    s = "\r\n";
        //    if (gsProjectNameSpace != null)
        //    {
        //        s += "namespace " + gsProjectNameSpace + "\r\n";
        //        s += "{\r\n";
        //        s += "\r\n";
        //    }
        //    s += "public static partial class w\r\n";
        //    s += "{\r\n";
        //    s += "\tpublic static void Run()\r\n";
        //    s += "\t{\r\n";
        //    s += "\r\n";
        //    zfile.WriteFile(sPathSource, s, true);
        //    zfile.WriteFile(sPathSource, sSource, true);
        //    s = "\r\n";
        //    s += "\t}\r\n";
        //    s += "}\r\n";
        //    s += "\r\n";
        //    if (gsProjectNameSpace != null)
        //    {
        //        s += "}\r\n";
        //        s += "\r\n";
        //    }
        //    zfile.WriteFile(sPathSource, s, true);

        //    //string[] sPathSources1 = GetSourceList();
        //    //string[] sPathSources2 = new string[sPathSources1.Length + 1];
        //    //sPathSources2[0] = sPathSource;
        //    //sPathSources1.CopyTo(sPathSources2, 1);
        //    //return sPathSources2;
        //    return sPathSource;
        //}

        //private string GetUsingList()
        //{
        //    string[] sUsingList = _runSourceConfig.GetValues("ProjectDefaultValues/Using/@value");
        //    string sUsing = "";
        //    foreach (string s in sUsingList)
        //        sUsing += "using " + s + ";\r\n";
        //    if (gProjectConfig != null)
        //    {
        //        sUsingList = gProjectConfig.GetValues("Project/Using/@value");
        //        foreach (string s in sUsingList)
        //            sUsing += "using " + s + ";\r\n";
        //    }
        //    return sUsing;
        //}

        // désactivé le 21/05/2015
        //private string SourceReadInclude(string sSource)
        //{
        //    int i;
        //    char[] cEol = new char[] { '\r', '\n' };
        //    char[] cTrim = new char[] { ' ', '\t', '\r', '\n' };
        //    char[] cSep = new char[] { ' ', '\t' };
        //    while (true)
        //    {
        //        sSource = sSource.TrimStart(cTrim);
        //        if (sSource.StartsWith("//"))
        //        {
        //            i = sSource.IndexOfAny(cEol);
        //            if (i == -1)
        //            {
        //                sSource = "";
        //                break;
        //            }
        //            sSource = sSource.Remove(0, i + 1).TrimStart(cTrim);
        //        }
        //        else if (sSource.StartsWith("raz_include", StringComparison.InvariantCultureIgnoreCase))
        //        {
        //            _trace.WriteLine("raz_include;");
        //            sSource = sSource.Remove(0, 11).TrimStart(cSep);
        //            if (sSource.StartsWith(";")) sSource = sSource.Remove(0, 1);
        //            sSource = sSource.TrimStart(cTrim);
        //            gIncludes.Clear();
        //        }
        //        else if (sSource.StartsWith("include", StringComparison.InvariantCultureIgnoreCase))
        //        {
        //            sSource = sSource.Remove(0, 7).TrimStart(cSep);
        //            if (sSource.StartsWith("on", StringComparison.InvariantCultureIgnoreCase))
        //            {
        //                sSource = sSource.Remove(0, 2).TrimStart(cSep);
        //                gbIncludeActive = true;
        //            }
        //            else if (sSource.StartsWith("off", StringComparison.InvariantCultureIgnoreCase))
        //            {
        //                sSource = sSource.Remove(0, 3).TrimStart(cSep);
        //                gbIncludeActive = false;
        //            }
        //            else if (sSource.StartsWith("\""))
        //            {
        //                i = sSource.IndexOf('\"', 1);
        //                if (i == -1)
        //                    throw new PBException("error bad include command");
        //                string sInclude = sSource.Substring(1, i - 1);
        //                _trace.WriteLine("include \"{0}\";", sInclude);
        //                if (!gIncludes.ContainsKey(sInclude))
        //                    gIncludes.Add(sInclude, null);
        //                sSource = sSource.Remove(0, i + 1);
        //            }
        //            else
        //                throw new PBException("error bad include command");
        //            if (sSource.StartsWith(";")) sSource = sSource.Remove(0, 1);
        //            sSource = sSource.TrimStart(cTrim);
        //        }
        //        else
        //            break;
        //    }
        //    return sSource;
        //}

        //private string[] GetSourceList()
        //{
        //    string[] sProjectSources = GetProjectSourceList(gProjectConfig);
        //    string[] sIncludeSources = GetIncludeSourceList();
        //    string[] sSources = new string[sIncludeSources.Length + sProjectSources.Length];
        //    sIncludeSources.CopyTo(sSources, 0);
        //    sProjectSources.CopyTo(sSources, sIncludeSources.Length);
        //    return sSources;
        //}

        //private string[] GetProjectSourceList(XmlConfig projectConfig)
        //{
        //    if (projectConfig == null)
        //        return new string[0];
        //    string[] sSources = projectConfig.GetValues("Project/Source/@value");
        //    for (int i = 0; i < sSources.Length; i++)
        //    {
        //        sSources[i] = GetPathSource(sSources[i]);
        //    }
        //    return sSources;
        //}

        // désactivé le 21/05/2015
        //private string[] GetIncludeSourceList()
        //{
        //    if (!gbIncludeActive) return new string[0];
        //    string[] sSources = new string[gIncludes.Keys.Count];
        //    gIncludes.Keys.CopyTo(sSources, 0);
        //    for (int i = 0; i < sSources.Length; i++)
        //    {
        //        //string sPathSource = sSources[i];
        //        //if (!Path.IsPathRooted(sPathSource) && gsSourceDir != null)
        //        //{
        //        //    sPathSource = gsSourceDir + sPathSource;
        //        //    sSources[i] = sPathSource;
        //        //}
        //        sSources[i] = GetPathSource(sSources[i]);
        //    }
        //    return sSources;
        //}

        // not used
        //public void InitConfig(string sConfigName)
        //{
        //    if (!Path.HasExtension(sConfigName))
        //        sConfigName += ".config.xml";
        //    sConfigName = GetFilePath(sConfigName);
        //    gsSourceConfigName = sConfigName;
        //    Config = new XmlConfig(sConfigName);
        //}

        //public void Export(string sPath)
        //{
        //    Export(sPath, false, false);
        //}

        //public void Export(string file, bool fieldNameHeader = false, bool append = false)
        //{
        //    if (_trace.TraceLevel >= 1)
        //        _trace.WriteLine("Export(\"{0}\");", file);
        //    zdt.Save(gdtResult.DefaultView, file, append, fieldNameHeader, null, Encoding.Default);
        //}

        //public DataTable Transpose(string sValueColumn, string sFieldDef)
        //{
        //    FieldList fields = new FieldList(sFieldDef);
        //    return Transpose(sValueColumn, fields);
        //}

        //public DataTable Transpose(string sValueColumn, FieldList fields)
        //{
        //    DataTable dtTranspose = zdt.Create(fields);
        //    DataRow row = dtTranspose.NewRow();
        //    zdt.Transpose(gdtResult, sValueColumn, fields, row);
        //    dtTranspose.Rows.Add(row);
        //    Result = dtTranspose;
        //    return gdtResult;
        //}

        //public void Transpose(string sValueColumn, string sFieldDef, DataRow row)
        //{
        //    FieldList fields = new FieldList(sFieldDef);
        //    Transpose(sValueColumn, fields, row);
        //}

        //public void Transpose(string sValueColumn, FieldList fields, DataRow row)
        //{
        //    zdt.Transpose(gdtResult, sValueColumn, fields, row);
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

        //public void AddTreeViewResult(string nodeName, string xmlPath, XFormat xFormat)
        //{
        //    if (!gbDisableResultEvent && TreeViewResultAdd != null)
        //    {
        //        XDocument xd = XDocument.Load(xmlPath);
        //        TreeViewResultAdd(nodeName, xd.Root, xFormat);
        //    }
        //}

        //public void AddTreeViewResult(string nodeName, XElement xmlElement, XFormat xFormat)
        //{
        //    if (!gbDisableResultEvent && TreeViewResultAdd != null) TreeViewResultAdd(nodeName, xmlElement, xFormat);
        //}

        //public void SelectTreeViewResult()
        //{
        //    if (!gbDisableResultEvent && TreeViewResultSelect != null)
        //        TreeViewResultSelect();
        //}

        //public string GetParametersName()
        //{
        //    string company = zapp.GetEntryAssemblyCompany();
        //    string product = zapp.GetEntryAssemblyProduct();
        //    if (company == null || product == null)
        //    {
        //        company = zapp.GetExecutingAssemblyCompany();
        //        product = zapp.GetExecutingAssemblyProduct();
        //    }
        //    if (product == null && company == null)
        //        throw new PBException("error unknow assembly product and company");
        //    if (product == null)
        //        return company;
        //    if (company == null)
        //        return product;
        //    return company + "_" + product;
        //}

        //public XmlParameters_v1 CreateParameters()
        //{
        //    _xmlParameter = new XmlParameters_v1(GetParametersName());
        //    return _xmlParameter;
        //}

        //public void SaveParameters()
        //{
        //    //XmlParameter xp = new XmlParameter(GetParametersName());
        //    //string sName = "WebRun_";
        //    if (_xmlParameter == null)
        //        CreateParameters();
        //    string sName = "RunSource_";
        //    _xmlParameter.Set(sName + "CurrentDirectory", Directory.GetCurrentDirectory());
        //    _xmlParameter.Set(sName + "SourceDir", gsSourceDir);
        //    _xmlParameter.Save();
        //}

        //public XmlParameters_v1 LoadParameters()
        //{
        //    XmlParameters_v1 xp = new XmlParameters_v1(GetParametersName());
        //    //xp.Deserialize();
        //    //string sName = "WebRun_";
        //    string sName = "RunSource_";
        //    object o;
        //    o = xp.Get(sName + "CurrentDirectory");
        //    if (o != null && o is string)
        //    {
        //        string dir = (string)o;
        //        //_trace.WriteLine("set current directory \"{0}\"", dir);
        //        if (Directory.Exists(dir))
        //            Directory.SetCurrentDirectory(dir);
        //    }
        //    o = xp.Get(sName + "SourceDir");
        //    if (o != null && o is string)
        //    {
        //        //_trace.WriteLine("set source directory \"{0}\"", o);
        //        gsSourceDir = (string)o;
        //    }
        //    return xp;
        //}

        public string GetFilePath(string file)
        {
            //if (!Path.IsPathRooted(file) && gsSourceDir != null)
            //if (!Path.IsPathRooted(file) && _projectDirectory != null)
            //    //file = Path.Combine(gsSourceDir, file);
            //    file = Path.Combine(_projectDirectory, file);
            //return file;
            return file.zRootPath(_projectDirectory);
        }

        //public string GetPathSource(string sPath)
        //{
        //    if (!Path.IsPathRooted(sPath) && gsSourceDir != null) sPath = gsSourceDir + sPath;
        //    return sPath;
        //}

        //private void EventWrited(string msg)
        //{
        //    if (Writed != null)
        //    {
        //        Writed(msg);
        //    }
        //}
    }

    //public static class RunSource_XmlLinq_Extension
    //{
        //public static DataTable zSelect_old(this RunSource wr, IEnumerable<XNode> list)
        //{
        //    DataTable dt = list.zToDataTable();
        //    wr.Result = dt;
        //    return dt;
        //}

        //public static DataTable zSelect_old(this RunSource wr, IEnumerable<XElement> list)
        //{
        //    DataTable dt = list.zToDataTable();
        //    wr.Result = dt;
        //    return dt;
        //}

        //public static DataTable zSelect_old(this RunSource wr, IEnumerable<XAttribute> list)
        //{
        //    DataTable dt = list.zToDataTable();
        //    wr.Result = dt;
        //    return dt;
        //}

        //public static DataTable zSelect_old<T>(this RunSource wr, IEnumerable<T> q)
        //{
        //    DataTable dt = q.zToDataTable();
        //    wr.Result = dt;
        //    return dt;
        //}

        //public static void zPrint_old<T>(this RunSource wr, T v)
        //{
        //    wr.zPrint_old(v, true);
        //}

        //public static void zPrint_old<T>(this RunSource wr, T v, bool bWithName)
        //{
        //    TypeView view = new TypeView(v);
        //    string s = "";
        //    foreach (NamedValue value in view.Values)
        //    {
        //        if (bWithName)
        //        {
        //            if (s != "") s += ", ";
        //            string sName = value.Name; if (sName == "") sName = "value";
        //            s += string.Format("{0}=\"{1}\"", sName, value.Value);
        //        }
        //        else
        //        {
        //            if (s != "") s += " ";
        //            s += value.Value.ToString();
        //        }
        //    }
        //    Trace.CurrentTrace.WriteLine(s);
        //}

        //public static void zPrint_old<T>(this RunSource wr, T v, string sFormat)
        //{
        //    TypeView view = new TypeView(v);
        //    object[] values = new object[view.Values.Count];
        //    for (int i = 0; i < view.Values.Count; i++) values[i] = view.Values[i].Value;
        //    Trace.CurrentTrace.WriteLine(string.Format(sFormat, values));
        //}

        //public static void zPrint_old<T>(this RunSource wr, IEnumerable<T> q)
        //{
        //    wr.zPrint_old(q, true);
        //}

        //public static void zPrint_old<T>(this RunSource wr, IEnumerable<T> q, bool bWithName)
        //{
        //    foreach (T v in q)
        //        wr.zPrint_old(v, bWithName);
        //}

        //public static void zPrint_old<T>(this RunSource wr, IEnumerable<T> q, string sFormat)
        //{
        //    foreach (T v in q)
        //        wr.zPrint_old(v, sFormat);
        //}

        //public static void zViewType_old<T>(this RunSource wr, IEnumerable<T> q)
        //{
        //    DataTable dt = zdt.Create("type, value");
        //    foreach (T v in q)
        //    {
        //        dt.Rows.Add(v.GetType(), v);
        //    }
        //    wr.Result = dt;
        //}

        //public static DataTable zViewType_old<T>(this RunSource wr, T v)
        //{
        //    DataTable dt = v.zViewType();
        //    wr.Result = dt;
        //    return dt;
        //}
    //}
}
