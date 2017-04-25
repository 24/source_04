using System;

namespace pb.Compiler
{
    [Flags]
    public enum VSProjectUpdateOptions
    {
        None = 0,
        BackupVSProject = 0x0001,
        Simulate = 0x0002,
        AddSource = 0x0004,
        RemoveSource = 0x0008,
        AddSourceLink = 0x0010,
        RemoveSourceLink = 0x0020,
        AddAssemblyReference = 0x0040,
        RemoveAssemblyReference = 0x0080,
        All = AddSource | RemoveSource | AddSourceLink | RemoveSourceLink | AddAssemblyReference | RemoveAssemblyReference
    }

    public interface IRunSourceVSProjectManager
    {
        void UpdateVSProject(string runsourceProject, VSProjectUpdateOptions options);
    }
}
