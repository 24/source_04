using pb.Data.VSProject;
using pb.Data.Xml;
using pb.IO;
using pb.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace pb.Compiler
{
    public class VSProjectUpdateResult
    {
        //public bool Success;
        public int SourceAddedCount;
        public int SourceRemovedCount;
        public int SourceLinkAddedCount;
        public int SourceLinkRemovedCount;
        public int AssemblyReferenceAddedCount;
        public int AssemblyReferenceRemovedCount;
    }

    public class RunSourceSource
    {
        public string File;
        public string RelativePath;
        public string ProjectFile;
    }

    public class RunSourceReference
    {
        public string Name;
        public string RelativePath;
        public bool FrameworkAssembly = false;
        public bool RunSourceAssembly = false;
        public string ProjectFile;
    }

    public class RunSourceVSProjectManager : IRunSourceVSProjectManager
    {
        private VSProjectUpdateOptions _options;

        private VSProjectManager _vsProjectManager = null;
        private static string _defaultBackupVSProjectDirectory = "_projectBackup";
        private string _backupVSProjectDirectory = _defaultBackupVSProjectDirectory;
        private string _vsLinkBaseDirectory = "SourceLnk";
        private Dictionary<string, string> _vsProjectSource = null;
        private Dictionary<string, string> _vsProjectSourceLinks = null;
        private Dictionary<string, string> _vsProjectReferences = null;

        //private CompilerProjectReader _runsourceProjectReader = null;
        private List<CompilerProjectReader> _runsourceProjectsReaders = null;
        private Dictionary<string, string> _runsourceProjectSources = null;
        private Dictionary<string, string> _runsourceProjectSourceLinks = null;
        private Dictionary<string, string> _runsourceProjectAssembliesReferences = null;

        //public RunSourceVSProjectManager(string vsProject, string runsourceProject, VSProjectUpdateOptions options)
        //public RunSourceVSProjectManager(string vsProject, IEnumerable<string> runsourceProjects, VSProjectUpdateOptions options)
        //{
        //    _vsProjectManager = new VSProjectManager(vsProject);
        //    //_runsourceProjectReader = CompilerProjectReader.Create(new XmlConfig(runsourceProject).GetConfigElement("/AssemblyProject"));
        //    _runsourceProjectsReaders = new List<CompilerProjectReader>();
        //    foreach (string runsourceProject in runsourceProjects)
        //        _runsourceProjectsReaders.Add(CompilerProjectReader.Create(new XmlConfig(runsourceProject).GetConfigElement("/AssemblyProject")));
        //    _options = options;
        //}

        public string BackupVSProjectDirectory { get { return _backupVSProjectDirectory; } set { _backupVSProjectDirectory = value; } }

        public void UpdateVSProject(string runsourceProject, VSProjectUpdateOptions options)
        {
            //RunSourceVSProjectManager manager = new RunSourceVSProjectManager();
            if (!Init(runsourceProject, options))
                return;
            TraceHeader();
            VSProjectUpdateResult result = _UpdateVSProject();
            TraceResult(result);
        }

        private bool Init(string runsourceProject, VSProjectUpdateOptions options)
        {
            CompilerProjectReader runsourceProjectReader = CompilerProjectReader.Create(new XmlConfig(runsourceProject).GetConfigElement("/AssemblyProject"));
            string vsProject = runsourceProjectReader.GetVSProject();
            if (vsProject == null)
            {
                Trace.WriteLine($"visual studio project is not defined");
                return false;
            }
            if (!zFile.Exists(vsProject))
            {
                Trace.WriteLine($"visual studio project not found \"{vsProject}\"");
                return false;
            }
            _vsProjectManager = new VSProjectManager(vsProject);

            _runsourceProjectsReaders = new List<CompilerProjectReader>();
            _runsourceProjectsReaders.Add(runsourceProjectReader);
            foreach (string complementaryProject in runsourceProjectReader.GetComplementaryProjects())
                _runsourceProjectsReaders.Add(CompilerProjectReader.Create(new XmlConfig(complementaryProject).GetConfigElement("/AssemblyProject")));
            _options = options;
            return true;
        }

        private VSProjectUpdateResult _UpdateVSProject()
        {
            VSProjectUpdateResult result = new VSProjectUpdateResult();

            if ((_options & VSProjectUpdateOptions.RemoveSource) == VSProjectUpdateOptions.RemoveSource)
                result.SourceRemovedCount = RemoveSources();

            if ((_options & VSProjectUpdateOptions.AddSource) == VSProjectUpdateOptions.AddSource)
                result.SourceAddedCount = AddSources();

            if ((_options & VSProjectUpdateOptions.RemoveSourceLink) == VSProjectUpdateOptions.RemoveSourceLink)
                result.SourceLinkRemovedCount = RemoveSourcesLinks();

            if ((_options & VSProjectUpdateOptions.AddSourceLink) == VSProjectUpdateOptions.AddSourceLink)
                result.SourceLinkAddedCount = AddSourcesLinks();

            if ((_options & VSProjectUpdateOptions.RemoveAssemblyReference) == VSProjectUpdateOptions.RemoveAssemblyReference)
                result.AssemblyReferenceRemovedCount = RemoveAssembliesReferences();

            if ((_options & VSProjectUpdateOptions.AddAssemblyReference) == VSProjectUpdateOptions.AddAssemblyReference)
                result.AssemblyReferenceAddedCount = AddAssembliesReferences();

            if (_vsProjectManager.ProjectModified && (_options & VSProjectUpdateOptions.Simulate) != VSProjectUpdateOptions.Simulate)
            {
                if ((_options & VSProjectUpdateOptions.BackupVSProject) == VSProjectUpdateOptions.BackupVSProject)
                {
                    //Trace.WriteLine($"  backup \"{_vsProjectManager.ProjectFile}\"");
                    BackupVSProject(_vsProjectManager.ProjectFile, _backupVSProjectDirectory);
                }
                //Trace.WriteLine($"  save \"{_vsProjectManager.ProjectFile}\"");
                _vsProjectManager.Save();
            }

            //result.Success = true;
            return result;
        }

        private static void BackupVSProject(string vsProjectFile, string backupVSProjectDirectory)
        {
            //string vsProjectFile = _vsProjectManager.ProjectFile;
            string directory = zPath.Combine(zPath.GetDirectoryName(vsProjectFile), backupVSProjectDirectory);
            string file = zfile.GetNewFilename(zPath.Combine(directory, zPath.GetFileName(vsProjectFile)), forceNewFilename: true);
            zdir.CreateDirectory(directory);
            zFile.Copy(vsProjectFile, file);
        }

        private int RemoveSources()
        {
            ReadRunSourceProjectSources();
            int sourceRemovedCount = 0;
            // ToArray() is needed because XDocument is modified by RemoveSourceLink()
            foreach (VSSource source in _vsProjectManager.GetSources().ToArray())
            {
                if (source.Link == null && !_runsourceProjectSources.ContainsKey(source.File.ToLower()))
                {
                    if (_vsProjectManager.RemoveSource(source.File))
                        Trace.WriteLine($"  remove source \"{source.File}\"");
                    else
                        Trace.WriteLine($"  warning can't remove source, source not found \"{source.File}\"");
                    sourceRemovedCount++;
                }
            }
            return sourceRemovedCount;
        }

        private int AddSources()
        {
            ReadVSProjectSource();
            int sourceAddedCount = 0;
            //foreach (RunSourceSource source in GetRunSourceSources(_runsourceProjectReader, _vsProjectManager.ProjectFile))
            foreach (RunSourceSource source in GetRunSourceSources(_runsourceProjectsReaders, _vsProjectManager.ProjectFile))
            {
                string relativePath = source.RelativePath;
                string relativePath2 = relativePath.ToLower();
                if (!_vsProjectSource.ContainsKey(relativePath2))
                {
                    Trace.WriteLine($"  add source \"{relativePath}\" (\"{zPath.GetFileName(source.ProjectFile)}\")");
                    _vsProjectSource.Add(relativePath2, null);
                    _vsProjectManager.AddSource(relativePath);
                    sourceAddedCount++;
                }
            }
            return sourceAddedCount;
        }

        private int RemoveSourcesLinks()
        {
            ReadRunSourceProjectSourceLinks();
            int sourceLinkRemovedCount = 0;
            // ToArray() is needed because XDocument is modified by RemoveSourceLink()
            foreach (VSSource source in _vsProjectManager.GetSources().ToArray())
            {
                //Trace.WriteLine($"RemoveSourcesLinks() : file \"{source.File}\" link {(source.Link != null ? source.Link : "--null--")}");
                if (source.Link != null && !_runsourceProjectSourceLinks.ContainsKey(source.File.ToLower()))
                {
                    if (_vsProjectManager.RemoveSourceLink(source.File))
                        Trace.WriteLine($"  remove source link \"{source.File}\"");
                    else
                        Trace.WriteLine($"  warning can't remove source link, source link not found \"{source.File}\"");
                    sourceLinkRemovedCount++;
                }
            }
            return sourceLinkRemovedCount;
        }

        private int AddSourcesLinks()
        {
            ReadVSProjectSourceLinks();
            int sourceLinkAddedCount = 0;
            //foreach (RunSourceSource source in GetRunSourceSourcesLinks(_runsourceProjectReader, _vsProjectManager.ProjectFile))
            foreach (RunSourceSource source in GetRunSourceSourcesLinks(_runsourceProjectsReaders, _vsProjectManager.ProjectFile))
            {
                // relativePath : "..\..\..\Lib\pb\Source\pb\_pb\PBException.cs"
                // link         : "SourceLnk\Lib\pb\_pb\PBException.cs"
                //Trace.WriteLine($"zpath.GetRelativePath() : \"{source.File}\" \"{_vsProjectManager.ProjectFile}\" relativePath \"{relativePath}\"");
                string relativePath = source.RelativePath;
                string link = GetLinkPath(relativePath);
                string relativePath2 = relativePath.ToLower();
                if (!_vsProjectSourceLinks.ContainsKey(relativePath2))
                {
                    Trace.WriteLine($"  add source link \"{relativePath}\" (\"{zPath.GetFileName(source.ProjectFile)}\")");
                    _vsProjectSourceLinks.Add(relativePath2, link);
                    _vsProjectManager.AddSourceLink(relativePath, link);
                    sourceLinkAddedCount++;
                }
            }
            return sourceLinkAddedCount;
        }

        //public static IEnumerable<RunSourceSource> GetRunSourceSources(CompilerProjectReader runsourceProjectReader, string baseFile = null)
        public static IEnumerable<RunSourceSource> GetRunSourceSources(IEnumerable<CompilerProjectReader> runsourceProjectsReaders, string baseFile = null)
        {
            string baseFile2 = baseFile;
            foreach (CompilerProjectReader runsourceProjectReader in runsourceProjectsReaders)
            {
                if (baseFile == null)
                    baseFile2 = runsourceProjectReader.ProjectFile;
                foreach (CompilerFile source in runsourceProjectReader.GetSources(recurse: true, withoutExtensionProject: true))
                {
                    // relativePath : "..\..\..\Lib\pb\Source\pb\_pb\PBException.cs"
                    // link         : "SourceLnk\Lib\pb\_pb\PBException.cs"
                    if (source.Attributes.ContainsKey("excludeVS") && source.Attributes["excludeVS"].ToLower() == "true")
                        continue;
                    string relativePath = zpath.GetRelativePath(source.File, baseFile2);
                    yield return new RunSourceSource { File = source.File, RelativePath = relativePath, ProjectFile = source.ProjectFile };
                }
                foreach (CompilerFile source in runsourceProjectReader.GetVSSources(recurse: true, withoutExtensionProject: true))
                {
                    // relativePath : "..\..\..\Lib\pb\Source\pb\_pb\PBException.cs"
                    // link         : "SourceLnk\Lib\pb\_pb\PBException.cs"
                    string relativePath = zpath.GetRelativePath(source.File, baseFile2);
                    yield return new RunSourceSource { File = source.File, RelativePath = relativePath, ProjectFile = source.ProjectFile };
                }
            }
        }

        //public static IEnumerable<RunSourceSource> GetRunSourceSourcesLinks(CompilerProjectReader runsourceProjectReader, string baseFile = null)
        public static IEnumerable<RunSourceSource> GetRunSourceSourcesLinks(IEnumerable<CompilerProjectReader> runsourceProjectsReaders, string baseFile = null)
        {
            string baseFile2 = baseFile;
            foreach (CompilerProjectReader runsourceProjectReader in runsourceProjectsReaders)
            {
                if (baseFile == null)
                    baseFile2 = runsourceProjectReader.ProjectFile;
                foreach (CompilerFile source in runsourceProjectReader.GetSourcesLinks(recurse: true, withoutExtensionProject: true))
                {
                    // relativePath : "..\..\..\Lib\pb\Source\pb\_pb\PBException.cs"
                    // link         : "SourceLnk\Lib\pb\_pb\PBException.cs"
                    if (source.Attributes.ContainsKey("excludeVS") && source.Attributes["excludeVS"].ToLower() == "true")
                        continue;
                    string relativePath = zpath.GetRelativePath(source.File, baseFile2);
                    yield return new RunSourceSource { File = source.File, RelativePath = relativePath, ProjectFile = source.ProjectFile };
                }
            }
        }

        private Regex _relativeDirectory = new Regex(@"^[\.\\/]*", RegexOptions.Compiled);
        private string GetLinkPath(string relativePath)
        {
            // relativePath : "..\..\..\Lib\pb\Source\pb\_pb\PBException.cs"
            // link         : "SourceLnk\Lib\pb\_pb\PBException.cs"
            relativePath = _relativeDirectory.Replace(relativePath, "");
            if (relativePath.StartsWith(@"Lib\pb\Source\"))
                relativePath = @"Lib\" + relativePath.Substring(14);
            return zPath.Combine(_vsLinkBaseDirectory, relativePath);
        }

        private int RemoveAssembliesReferences()
        {
            ReadRunSourceProjectAssembliesReferences();
            int assemblyReferenceRemovedCount = 0;
            // ToArray() is needed because XDocument is modified by RemoveAssemblyReference()
            foreach (VSReference reference in _vsProjectManager.GetReferences().ToArray())
            {
                if (reference.ProjectReference)
                    continue;
                if (!_runsourceProjectAssembliesReferences.ContainsKey(reference.Name.ToLower()))
                {
                    if (_vsProjectManager.RemoveAssemblyReference(reference.Name))
                        Trace.WriteLine($"  remove reference \"{reference.Name}\"");
                    else
                        Trace.WriteLine($"  warning can't remove reference, reference not found \"{reference.Name}\"");
                    assemblyReferenceRemovedCount++;
                }
            }
            return assemblyReferenceRemovedCount;
        }

        private int AddAssembliesReferences()
        {
            ReadVSProjectReferences();
            int assemblyReferenceAddedCount = 0;
            //foreach (RunSourceReference reference in GetRunSourceAssemblies(_runsourceProjectReader, _vsProjectManager.ProjectFile))
            foreach (RunSourceReference reference in GetRunSourceAssemblies(_runsourceProjectsReaders, _vsProjectManager.ProjectFile))
            {
                string referenceName = reference.Name.ToLower();
                if (!_vsProjectReferences.ContainsKey(referenceName))
                {
                    Trace.WriteLine($"  add reference \"{reference.Name}\" (\"{zPath.GetFileName(reference.ProjectFile)}\")");
                    _vsProjectReferences.Add(referenceName, null);
                    _vsProjectManager.AddReference(reference.Name, reference.RelativePath);
                    assemblyReferenceAddedCount++;
                }
            }
            return assemblyReferenceAddedCount;
        }

        //public static IEnumerable<RunSourceReference> GetRunSourceAssemblies(CompilerProjectReader runsourceProjectReader, string baseFile = null)
        public static IEnumerable<RunSourceReference> GetRunSourceAssemblies(IEnumerable<CompilerProjectReader> runsourceProjectsReaders, string baseFile = null)
        {
            string baseFile2 = baseFile;
            foreach (CompilerProjectReader runsourceProjectReader in runsourceProjectsReaders)
            {
                if (baseFile == null)
                    baseFile2 = runsourceProjectReader.ProjectFile;
                foreach (CompilerAssembly assembly in runsourceProjectReader.GetAssemblies(recurse: true, withoutExtensionProject: true))
                {
                    if (assembly.VSExclude)
                        continue;
                    string relativePath = null;
                    if (!assembly.FrameworkAssembly)
                    {
                        relativePath = zpath.GetRelativePath(assembly.File, baseFile2);
                        //Trace.WriteLine($"GetRunSourceAssemblies() : relativePath \"{relativePath}\" file \"{assembly.File}\" project file \"{_vsProjectManager.ProjectFile}\"");
                    }
                    string name = zPath.GetFileNameWithoutExtension(assembly.File);
                    string name2 = name.ToLower();
                    //if (name2 == "mscorlib" || name2 == "runsource" || name2 == "runsource.irunsource" || name2 == "runsource.command")
                    //    continue;
                    yield return new RunSourceReference { Name = name, RelativePath = relativePath, FrameworkAssembly = assembly.FrameworkAssembly, RunSourceAssembly = assembly.RunSourceAssembly, ProjectFile = assembly.ProjectFile };
                }
            }
        }

        private void ReadVSProjectSource()
        {
            if (_vsProjectSource == null)
            {
                _vsProjectSource = new Dictionary<string, string>();
                foreach (VSSource source in _vsProjectManager.GetSources())
                {
                    string sourceFile = source.File.ToLower();
                    if (source.Link == null && !_vsProjectSource.ContainsKey(sourceFile))
                        _vsProjectSource.Add(sourceFile, null);
                }
            }
        }

        private void ReadVSProjectSourceLinks()
        {
            if (_vsProjectSourceLinks == null)
            {
                _vsProjectSourceLinks = new Dictionary<string, string>();
                foreach (VSSource source in _vsProjectManager.GetSources())
                {
                    string sourceFile = source.File.ToLower();
                    if (source.Link != null && !_vsProjectSourceLinks.ContainsKey(sourceFile))
                        _vsProjectSourceLinks.Add(sourceFile, source.Link);
                }
            }
        }

        private void ReadRunSourceProjectSources()
        {
            if (_runsourceProjectSources == null)
            {
                _runsourceProjectSources = new Dictionary<string, string>();
                foreach (CompilerProjectReader runsourceProjectReader in _runsourceProjectsReaders)
                {
                    foreach (CompilerFile source in runsourceProjectReader.GetSources(recurse: true, withoutExtensionProject: true))
                    {
                        string relativePath = zpath.GetRelativePath(source.File, _vsProjectManager.ProjectFile).ToLower();
                        if (!_runsourceProjectSources.ContainsKey(relativePath))
                            _runsourceProjectSources.Add(relativePath, null);
                    }
                    foreach (CompilerFile source in runsourceProjectReader.GetVSSources(recurse: true, withoutExtensionProject: true))
                    {
                        string relativePath = zpath.GetRelativePath(source.File, _vsProjectManager.ProjectFile).ToLower();
                        if (!_runsourceProjectSources.ContainsKey(relativePath))
                            _runsourceProjectSources.Add(relativePath, null);
                    }
                }
            }
        }

        private void ReadRunSourceProjectSourceLinks()
        {
            if (_runsourceProjectSourceLinks == null)
            {
                _runsourceProjectSourceLinks = new Dictionary<string, string>();
                foreach (CompilerProjectReader runsourceProjectReader in _runsourceProjectsReaders)
                {
                    foreach (CompilerFile source in runsourceProjectReader.GetSourcesLinks(recurse: true, withoutExtensionProject: true))
                    {
                        string relativePath = zpath.GetRelativePath(source.File, _vsProjectManager.ProjectFile).ToLower();
                        string link = GetLinkPath(relativePath);
                        if (!_runsourceProjectSourceLinks.ContainsKey(relativePath))
                            _runsourceProjectSourceLinks.Add(relativePath, link);
                    }
                }
            }
        }

        private void ReadRunSourceProjectAssembliesReferences()
        {
            if (_runsourceProjectAssembliesReferences == null)
            {
                _runsourceProjectAssembliesReferences = new Dictionary<string, string>();
                //foreach (RunSourceReference reference in GetRunSourceAssemblies(_runsourceProjectReader, _vsProjectManager.ProjectFile))
                foreach (RunSourceReference reference in GetRunSourceAssemblies(_runsourceProjectsReaders, _vsProjectManager.ProjectFile))
                {
                    string referenceName = reference.Name.ToLower();
                    if (!_runsourceProjectAssembliesReferences.ContainsKey(referenceName))
                        _runsourceProjectAssembliesReferences.Add(referenceName, reference.RelativePath);
                }
            }
        }

        // <ItemGroup>
        //   <ProjectReference Include="..\..\RunSource\v2\runsource.command\runsource.v2.command.csproj">
        //     <Project>{12d4a2fb-84e4-4067-8655-a019e2cf7b5f}</Project>
        //     <Name>runsource.v2.command</Name>
        //   </ProjectReference>
        // </ItemGroup>
        private void ReadVSProjectReferences()
        {
            if (_vsProjectReferences == null)
            {
                _vsProjectReferences = new Dictionary<string, string>();
                foreach (VSReference reference in _vsProjectManager.GetReferences())
                {
                    if (!reference.ProjectReference)
                        _vsProjectReferences.Add(reference.Name.ToLower(), null);
                }
            }
        }

        //string vsProject, IEnumerable<string> runsourceProjects, VSProjectUpdateOptions options
        private void TraceHeader()
        {
            string label = "";
            if ((_options & VSProjectUpdateOptions.Simulate) == VSProjectUpdateOptions.Simulate)
                label = "simulate";
            else
                label = "backup";
            Trace.WriteLine($"update visual studio project ({label}) \"{zPath.GetFileName(_vsProjectManager.ProjectFile)}\" from runsource project {_runsourceProjectsReaders.zToStringValues(runsourceProjectReader => "\"" + zPath.GetFileName(runsourceProjectReader.ProjectFile) + "\"")} operations {GetOperationLabel()}");
            //Trace.WriteLine($"  from runsource project {_runsourceProjectsReaders.zToStringValues(runsourceProjectReader => "\"" + zPath.GetFileName(runsourceProjectReader.ProjectFile) + "\"")}");
            //Trace.WriteLine($"  operations : {GetOperationLabel()}");
        }

        private string GetOperationLabel()
        {
            // operations : remove source, source link, assembly reference, add source, source link, assembly reference
            StringBuilder sb = new StringBuilder();
            bool first = true;
            string label = GetRemoveOperationLabels().zConcatStrings(", ");
            if (label != null)
            {
                sb.Append("remove ");
                sb.Append(label);
                first = false;
            }
            label = GetAddOperationLabels().zConcatStrings(", ");
            if (label != null)
            {
                if (!first)
                    sb.Append(", ");
                sb.Append("add ");
                sb.Append(label);
            }
            if (first)
                sb.Append("none");
            return sb.ToString();
        }

        private IEnumerable<string> GetAddOperationLabels()
        {
            if ((_options & VSProjectUpdateOptions.AddSourceLink) == VSProjectUpdateOptions.AddSourceLink)
                yield return "source";
            if ((_options & VSProjectUpdateOptions.AddSourceLink) == VSProjectUpdateOptions.AddSourceLink)
                yield return "source link";
            if ((_options & VSProjectUpdateOptions.AddAssemblyReference) == VSProjectUpdateOptions.AddAssemblyReference)
                yield return "assembly reference";
        }

        private IEnumerable<string> GetRemoveOperationLabels()
        {
            if ((_options & VSProjectUpdateOptions.RemoveSource) == VSProjectUpdateOptions.RemoveSource)
                yield return "source";
            if ((_options & VSProjectUpdateOptions.RemoveSourceLink) == VSProjectUpdateOptions.RemoveSourceLink)
                yield return "source link";
            if ((_options & VSProjectUpdateOptions.RemoveAssemblyReference) == VSProjectUpdateOptions.RemoveAssemblyReference)
                yield return "assembly reference";
        }

        private static void TraceResult(VSProjectUpdateResult result)
        {
            Trace.WriteLine($"  {GetResultLabel(result)}");
        }

        private static string GetResultLabel(VSProjectUpdateResult result)
        {
            // 0 source removed, 0 source added, 0 source link removed, 0 source link added, 0 assembly reference removed, 0 assembly reference added
            string label = GetResultLabels(result).zConcatStrings(", ");
            if (label == null)
                label = "nothing done";
            return label;
        }

        private static IEnumerable<string> GetResultLabels(VSProjectUpdateResult result)
        {
            if (result.SourceRemovedCount != 0)
                yield return $"{result.SourceRemovedCount} source removed";
            if (result.SourceAddedCount != 0)
                yield return $"{result.SourceAddedCount} source added";
            if (result.SourceLinkRemovedCount != 0)
                yield return $"{result.SourceLinkRemovedCount} source link removed";
            if (result.SourceLinkAddedCount != 0)
                yield return $"{result.SourceLinkAddedCount} source link added";
            if (result.AssemblyReferenceRemovedCount != 0)
                yield return $"{result.AssemblyReferenceRemovedCount} assembly reference removed";
            if (result.AssemblyReferenceAddedCount != 0)
                yield return $"{result.AssemblyReferenceAddedCount} assembly reference added";
        }

        //public static VSProjectUpdateResult UpdateVSProject(string vsProject, string runsourceProject, VSProjectUpdateOptions options)
        //public static void UpdateVSProject(string vsProject, IEnumerable<string> runsourceProjects, VSProjectUpdateOptions options)
        //{
        //    if (!zFile.Exists(vsProject))
        //    {
        //        Trace.WriteLine($"visual studio project not found \"{vsProject}\"");
        //        return;
        //    }
        //    TraceHeader(vsProject, runsourceProjects, options);
        //    VSProjectUpdateResult result = new RunSourceVSProjectManager(vsProject, runsourceProjects, options)._UpdateVSProject();
        //    TraceResult(result);
        //}

        //public static void UpdateVSProject(string runsourceProject, VSProjectUpdateOptions options)
        //{
        //    RunSourceVSProjectManager manager = new RunSourceVSProjectManager();
        //    if (!manager.Init(runsourceProject, options))
        //        return;
        //    manager.TraceHeader();
        //    VSProjectUpdateResult result = manager._UpdateVSProject();
        //    TraceResult(result);
        //}

        public static void Test_BackupVSProject(string vsProjectFile)
        {
            //new RunSourceVSProjectManager(vsProjectFile, null, VSProjectUpdateOptions.None).BackupVSProject();
            BackupVSProject(vsProjectFile, _defaultBackupVSProjectDirectory);
        }
    }
}
