namespace pb.Compiler
{
    public static partial class RunSourceCommand
    {
        public static void UpdateVSProject(string runsourceProject = null, VSProjectUpdateOptions options = VSProjectUpdateOptions.None)
        {
            if (runsourceProject != null)
                runsourceProject = GetFilePath(GetProjectVariableValue(runsourceProject));
            else
                runsourceProject = GetCurrentProject();

            options |= VSProjectUpdateOptions.BackupVSProject;

            new RunSourceVSProjectManager().UpdateVSProject(runsourceProject, options);
        }
    }
}
