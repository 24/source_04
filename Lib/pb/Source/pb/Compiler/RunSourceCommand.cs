using System.Data;

namespace pb.Compiler
{
    public static class RunSourceCommand
    {
        public static int ProjectCompilerTraceLevel { get { return ProjectCompiler.TraceLevel; } set { ProjectCompiler.TraceLevel = value; } }

        //public static void SetWriter(string file, FileOption option)
        //{
        //    //TraceManager.Current.SetWriter(file, option);
        //    TraceManager.Current.SetWriter(WriteToFile.Create(file, option));
        //}

        public static ITraceManager TraceManager { get { return pb.TraceManager.Current; } }

        //public static void TraceSetWriter(IWriteToFile writer, string name = "_default")
        //{
        //    TraceManager.Current.SetWriter(writer, name);
        //}

        //public static void TraceRemoveWriter(string name = "_default")
        //{
        //    TraceManager.Current.RemoveWriter(name);
        //}

        //public static void TraceEnableViewer()
        //{
        //    TraceManager.Current.DisableViewer = false;
        //}

        //public static void TraceDisableViewer()
        //{
        //    TraceManager.Current.DisableViewer = true;
        //}

        public static void SetProjectFromSource()
        {
            RunSource.CurrentRunSource.SetProjectFromSource();
        }

        public static string GetCurrentProject()
        {
            return RunSource.CurrentRunSource.ProjectFile;
        }

        public static string SetProject(string file)
        {
            return RunSource.CurrentRunSource.SetProject(file);
        }

        public static void CompileProject(string projectName)
        {
            RunSource.CurrentRunSource.CompileProject(projectName);
        }

        public static void CompileProjects(string projectsFile, string runsourceSourceDirectory = null)
        {
            RunSource.CurrentRunSource.CompileProjects(projectsFile, runsourceSourceDirectory);
        }

        public static bool IsRunning()
        {
            return RunSource.CurrentRunSource.IsRunning();
        }

        public static bool IsExecutionAlive()
        {
            return RunSource.CurrentRunSource.IsExecutionAlive();
        }

        public static int GetRunningCount()
        {
            return RunSource.CurrentRunSource.GetRunningCount();
        }

        public static void PauseExecution(bool pause)
        {
            RunSource.CurrentRunSource.PauseExecution(pause);
        }

        public static bool IsExecutionPaused()
        {
            return RunSource.CurrentRunSource.IsExecutionPaused();
        }

        public static void AbortExecution(bool abort)
        {
            RunSource.CurrentRunSource.AbortExecution(abort);
        }

        public static void ForceAbortExecution()
        {
            RunSource.CurrentRunSource.ForceAbortExecution();
        }

        public static bool IsExecutionAborted()
        {
            return RunSource.CurrentRunSource.IsExecutionAborted();
        }

        public static void SetResult(DataTable table)
        {
            RunSource.CurrentRunSource.SetResult(table);
        }

        public static void SetResult(DataTable table, string xmlFormat)
        {
            RunSource.CurrentRunSource.SetResult(table, xmlFormat);
        }

        public static void SetResult(DataSet dataSet, string xmlFormat = null)
        {
            RunSource.CurrentRunSource.SetResult(dataSet, xmlFormat);
        }

        public static string GetProjectVariableValue(string value, bool throwError = false)
        {
            return RunSource.CurrentRunSource.GetProjectVariableValue(value, throwError);
        }

        public static string GetFilePath(string file)
        {
            return RunSource.CurrentRunSource.GetFilePath(file);
        }
    }
}
