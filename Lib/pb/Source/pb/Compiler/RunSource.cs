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

//Trace.WriteLine("toto");

namespace pb.Compiler
{
    public class RunSource : MarshalByRefObject, IRunSource // IDisposable
    {
        private static RunSource _currentRunSource = null;
        private static string _defaultSuffixProjectName = ".project.xml";
        //private ITrace _trace = null;
        //private string gsDir = null;
        private string _dataDirectory = null;                                             // not used inside RunSource class
        private DataTable gdtResult = null;
        private DataSet gdsResult = null;
        private string gsXmlResultFormat = null;

        private SortedList<string, object> gData = new SortedList<string, object>();      // not used inside RunSource class

        //private bool gbIncludeActive = true;
        //private SortedList<string, object> gIncludes = new SortedList<string, object>();
        private XmlConfig _runSourceConfig = null;
        private bool _refreshRunSourceConfig = false;
        private string _sourceFile = null;
        private string _projectFile = null;
        private string _projectDirectory = null;
        private XmlConfig _projectConfig = null;
        private bool _refreshProjectConfig = false;

        // projectDefaultValues
        private string _defaultProjectFile = null;
        private XmlConfig _defaultProjectXmlConfig = null;
        private CompilerProject _defaultProject = null;

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
        public OnAbortEvent OnAbortExecution { get; set; }

        public event SetDataTableEvent GridResultSetDataTable;
        public event SetDataSetEvent GridResultSetDataSet;

        //public event TreeViewResultAddEvent TreeViewResultAdd;
        //public event TreeViewResultSelectEvent TreeViewResultSelect;

        //public delegate void XmlDocumentLoadedEvent(XmlDocument xml, string sUrl, Http http);
        //public event XmlDocumentLoadedEvent XmlDocumentLoaded = null;


        private bool gbDisableMessage = false; // si true les messages ne doivent plus être affichés
        public event DisableMessageChangedEvent DisableMessageChanged;

        private bool gbDisableResultEvent = false; // si true ResultEvent n'est pas appelé quand un nouveau résultat est disponible
        public event SetDataTableEvent ErrorResultSet;
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

        public IGenerateAndExecuteManager GenerateAndExecuteManager { get { return _generateAndExecuteManager; } }
        public string SourceFile { get { return _sourceFile; } set { _sourceFile = value; } }
        public string ProjectFile { get { return _projectFile; } }
        // set { _projectDirectory = value; }
        public string ProjectDirectory { get { return _projectDirectory; } }

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

        public IChrono ExecutionChrono
        {
            get { return gExecutionChrono; }
        }

        public void SetRunSourceConfig(string file)
        {
            _runSourceConfig = new XmlConfig(file);
            _refreshRunSourceConfig = false;
            //XmlConfigElement projectDefaultValues = _runSourceConfig.GetConfigElement("ProjectDefaultValues");
            //if (projectDefaultValues != null)
            //    //_defaultProject = new CompilerProject(projectDefaultValues, _runSourceConfig.ConfigPath);
            //    _defaultProject = new CompilerProject(projectDefaultValues);
            //else
            //    _defaultProject = null;
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

        public CompilerProject GetRunSourceConfigCompilerDefaultValues()
        {
            return CompilerProject.Create(GetRunSourceConfig().zGetConfigElement("CompilerDefaultValues"));
        }

        // $$ProjectDefaultValues disable
        //public CompilerProject GetRunSourceConfigProjectDefaultValues()
        public CompilerProject GetDefaultProject()
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
                _defaultProject = CompilerProject.Create(_defaultProjectXmlConfig.GetConfigElement("/AssemblyProject"));
            return _defaultProject;
        }

        public XmlConfig GetProjectConfig()
        {
            if (_projectConfig == null && _projectFile != null && zFile.Exists(_projectFile))
                _projectConfig = new XmlConfig(_projectFile);
            else if (_projectConfig != null && _refreshProjectConfig)
                _projectConfig.Refresh();
            _refreshProjectConfig = false;
            return _projectConfig;
        }

        public CompilerProject GetProjectCompilerProject()
        {
            XmlConfig config = GetProjectConfig();
            if (config != null)
                return CompilerProject.Create(GetProjectConfig().GetConfigElementExplicit("/AssemblyProject"));
            else
                return null;
        }

        public string GetProjectVariableValue(string value, bool throwError = false)
        {
            XmlConfig projectConfig = GetProjectConfig();
            XElement root = null;
            if (projectConfig != null)
                root = projectConfig.XDocument.Root;
            string newValue;
            if (!root.zTryGetVariableValue(value, out newValue, traceError: true) && throwError)
                throw new PBException("cant get variable value from \"{0}\"", value);
            value = newValue;
            return value;
        }

        public string SetProjectFromSource()
        {
            return SetProject(_sourceFile);
        }

        public string SetProject(string file)
        {
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
            _Run_v1(code, useNewThread, compileWithoutProject);
            //_Run_v2(code, useNewThread, compileWithoutProject);
        }

        private void _Run_v2(string code, bool useNewThread = true, bool compileWithoutProject = false)
        {
            if (code == "")
                return;

            bool error = false;
            if (gExecutionThread != null)
                throw new PBException("error program already running");

            _refreshRunSourceConfig = true;
            _refreshProjectConfig = true;

            gExecutionChrono = new Chrono();
            try
            {
                _GenerateCode(code, compileWithoutProject);
            }
            catch
            {
                error = true;
                throw;
            }
            finally
            {
                if (error && EndRun != null)
                    EndRun(error);
            }
        }

        private void _GenerateCode(string code, bool compileWithoutProject = false)
        {
        }

        private void _Run_v1(string code, bool useNewThread = true, bool compileWithoutProject = false)
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

                _refreshRunSourceConfig = true;
                _refreshProjectConfig = true;

                IGenerateAndExecute generateAndExecute = _generateAndExecuteManager.New();
                _GenerateCodeAndCompile_v2(generateAndExecute, code, compileWithoutProject);
                if (generateAndExecute.Compiler.HasError())
                    return;

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
            _GenerateCodeAndCompile_v2(_generateAndExecuteManager.New(), code, compileWithoutProject);
        }

        private void _GenerateCodeAndCompile_v2(IGenerateAndExecute generateAndExecute, string code, bool compileWithoutProject = false)
        {
            // use CompilerDefaultValues from runsource.runsource.config.xml runsource.runsource.config.local.xml

            if (code == "")
                return;

            CompilerProject compilerProject = null;
            //if (!compileWithoutProject && _projectFile != null && zFile.Exists(_projectFile))
            //{
            //    compilerProject = CompilerProject.Create(GetProjectConfig().GetConfigElementExplicit("/AssemblyProject"));
            //}
            ////else
            ////    pb.Trace.WriteLine("RunSource._GenerateCodeAndCompile_v2() : dont use project");
            //else
            //    compilerProject = GetDefaultProject();
            if (!compileWithoutProject)
                compilerProject = GetProjectCompilerProject();
            if (compilerProject == null)
                compilerProject = GetDefaultProject();

            if (compilerProject != null)
            {
                generateAndExecute.NameSpace = compilerProject.GetNameSpace();
            }

            // $$ProjectDefaultValues disable
            //CompilerProject defaultProject = GetRunSourceConfigProjectDefaultValues();
            //if (defaultProject != null)
            //    generateAndExecute.AddUsings(defaultProject.GetUsings());

            if (compilerProject != null)
                generateAndExecute.AddUsings(compilerProject.GetUsings());

            generateAndExecute.GenerateCSharpCode(code);

            SetCompilerValues(generateAndExecute, compilerProject);

            ICompiler compiler = generateAndExecute.Compiler;
            //Compiler compiler = new Compiler();
            //compiler.SetOutputAssembly(assemblyFilename);
            //compiler.AddSource(new CompilerFile(file));
            compiler.Compile();
            //Assembly assembly = _compiler.Results.CompiledAssembly;

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
        }

        public void SetCompilerValues(IGenerateAndExecute generateAndExecute, CompilerProject compilerProject)
        {
            ICompiler compiler = generateAndExecute.Compiler;
            if (compilerProject != null)
                compiler.DefaultDir = zPath.GetDirectoryName(compilerProject.ProjectFile);
            // CompilerDefaultValues from runsource.runsource.config.xml runsource.runsource.config.local.xml
            compiler.SetParameters(GetRunSourceConfigCompilerDefaultValues(), dontSetOutput: true);

            // ProjectDefaultValues from runsource.runsource.config.xml runsource.runsource.config.local.xml
            // $$ProjectDefaultValues disable
            //compiler.SetParameters(GetRunSourceConfigProjectDefaultValues(), dontSetOutput: true);

            compiler.SetParameters(compilerProject, dontSetOutput: true);
        }

        public ICompiler Compile_Project(string projectName)
        {
            // - compile assembly project (like runsource.dll.project.xml) and runsource project (like download.project.xml)
            // - for assembly project use CompilerDefaultValues from runsource.runsource.config.xml runsource.runsource.config.local.xml
            // - for runsource project use CompilerDefaultValues from runsource.runsource.config.xml runsource.runsource.config.local.xml

            Compiler compiler = new Compiler();
            projectName = GetProjectVariableValue(projectName, throwError: true);
            string pathProject = GetFilePath(projectName);
            compiler.DefaultDir = zPath.GetDirectoryName(pathProject);
            // CompilerDefaultValues from runsource.runsource.config.xml runsource.runsource.config.local.xml
            compiler.SetParameters(GetRunSourceConfigCompilerDefaultValues(), dontSetOutput: true);
            Trace.WriteLine("Compile project \"{0}\"", pathProject);

            //XmlConfig projectConfig = new XmlConfig(pathProject);
            //XmlConfigElement projectElement = null;
            //if (projectConfig.XDocument.Root.Name == "AssemblyProject")
            //{
            //    // assembly project (like runsource.dll.project.xml)
            //    projectElement = projectConfig.GetConfigElement("/AssemblyProject");
            //}
            //else
            //{
            //    // runsource project (like download.project.xml)
            //    projectElement = projectConfig.GetConfigElement("Project");
            //    // ProjectDefaultValues from runsource.runsource.config.xml runsource.runsource.config.local.xml
            //    compiler.SetParameters(GetRunSourceConfigProjectDefaultValues(), dontSetOutput: true);
            //}
            //if (projectElement == null)
            //    throw new PBException("unknow project type \"{0}\"", pathProject);
            //compiler.SetParameters(CompilerProject.Create(projectElement));  // /AssemblyProject

            compiler.SetParameters(CompilerProject.Create(new XmlConfig(pathProject).GetConfigElementExplicit("/AssemblyProject")));

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

        public void DeleteGeneratedAssemblies()
        {
            _generateAndExecuteManager.DeleteGeneratedAssemblies();
        }

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

        public string GetFilePath(string file)
        {
            return file.zRootPath(_projectDirectory);
        }
    }
}
