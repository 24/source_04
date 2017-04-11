using System;
using System.Data;
using System.Threading.Tasks;

namespace pb.Compiler
{
    public delegate void OnPauseEvent(bool pause);
    //public delegate void OnAbortEvent(bool abort);
    public delegate void OnAbortEvent();
    //public delegate void WritedEvent2(string msg);
    public delegate void SetDataTableEvent(DataTable dataTable, string xmlFormat);
    public delegate void SetDataSetEvent(DataSet dataSet, string xmlFormat);
    public delegate void DisableMessageChangedEvent(bool disableMessage);
    //public delegate void TreeViewResultAddEvent(string nodeName, XElement xmlElement, XFormat xFormat);
    //public delegate void TreeViewResultSelectEvent();
    public delegate void ProgressChangeEvent(int current, int total, string message, params object[] prm);
    //public delegate void EndRunEvent(bool bError);

    public class EndRunCodeInfo
    {
        public bool Error;
        public IChrono RunCodeChrono;
    }

    public interface IRunSource : IDisposable
    {
        event OnPauseEvent OnPauseExecution;
        OnAbortEvent OnAbortExecution { get; set; }
        //event WritedEvent2 Writed;
        event SetDataTableEvent GridResultSetDataTable;
        event SetDataSetEvent GridResultSetDataSet;
        //event TreeViewResultAddEvent TreeViewResultAdd;
        //event TreeViewResultSelectEvent TreeViewResultSelect;
        event DisableMessageChangedEvent DisableMessageChanged;
        //event SetDataTableEvent ErrorResultSet;
        event ProgressChangeEvent ProgressChange;
        //event EndRunEvent EndRunCode;
        Action<EndRunCodeInfo> EndRunCode { get; set; }
        //bool AllowMultipleExecution { get; set; }
        //bool CallInit { get; set; }
        bool CallInitRunOnce { get; }

        //ITrace Trace { get; set; }
        //IGenerateAndExecuteManager GenerateAndExecuteManager { get; }
        string SourceFile { get; set; }
        string ProjectFile { get; }
        string ProjectDirectory { get; }
        //string ProjectName { get; set; }
        //string ProjectNameSpace { get; set; }
        //string SourceDir { get; set; }
        //string Dir { get; set; }
        bool DisableMessage { get; set; }
        bool DontSelectResultTab { get; set; }
        string ProgressTxt { get; }
        int ProgressValue { get; }
        int ProgressTotal { get; }
        bool Progress_AddProgressValueToMessage { get; set; }
        bool Progress_PutProgressMessageToWindowsTitle { get; set; }
        int Progress_MinimumMillisecondsBetweenMessage { get; set; }
        //IChrono RunCodeChrono { get; }

        void SetAsCurrentRunSource();
        void Init(string configFile);
        //void SetRunSourceConfig(string configFile);
        void StartAssemblyResolve();
        string SetProject(string file);
        bool IsRunning();
        bool IsExecutionPaused();
        void PauseExecution(bool pause);
        bool IsExecutionAborted();
        void AbortExecution(bool abort);
        void ForceAbortExecution();
        bool IsExecutionAlive();
        int GetRunningCount();
        void SetOptions(string options);
        //void TraceInit(bool trace);
        //void RunCode(string code, bool runOnMainThread = false, bool compileWithoutProject = false, bool allowMultipleRun = false, bool callInit = false);
        Task RunCode(string code, bool runOnMainThread = false, bool compileWithoutProject = false, bool allowMultipleRun = false, bool callInit = false);
        //void CompileCode(string source, bool compileWithoutProject = false);
        Task CompileCode(string source, bool compileWithoutProject = false);
        void DeleteGeneratedAssemblies();
        IProjectCompiler CompileProject(string projectName);
        bool CompileProjects(string projectsFile, string runsourceSourceDirectory = null);
        //string GetProjectConfigPath(string projectName);
        //XmlParameters_v1 CreateParameters();
        //void SaveParameters();
        //XmlParameters_v1 LoadParameters();
    }
}
