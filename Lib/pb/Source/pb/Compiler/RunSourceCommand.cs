using pb.IO;
using System.Data;

namespace pb.Compiler
{
    public static class RunSourceCommand
    {
        public static int ProjectCompilerTraceLevel { get { return ProjectCompiler.TraceLevel; } set { ProjectCompiler.TraceLevel = value; } }

        public static void SetWriter(string file, FileOption option)
        {
            TraceManager.Current.SetWriter(file, option);
        }

        public static void SetProjectFromSource()
        {
            RunSource.CurrentRunSource.SetProjectFromSource();
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
