using pb.Data.VSProject;
using pb.Data.Xml;
using pb.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace pb.Compiler
{
    [Flags]
    public enum VSProjectUpdateOptions
    {
        None                        = 0,
        BackupVSProject             = 0x0001,
        Simulate                    = 0x0002,
        AddSource                   = 0x0004,
        RemoveSource                = 0x0008,
        AddSourceLink               = 0x0010,
        RemoveSourceLink            = 0x0020,
        AddAssemblyReference        = 0x0040,
        RemoveAssemblyReference     = 0x0080,
        All = AddSource | RemoveSource | AddSourceLink | RemoveSourceLink | AddAssemblyReference | RemoveAssemblyReference
    }

    public class VSProjectUpdateResult
    {
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

    public class RunSourceVSProjectManager
    {
        //private string _runsourceProject = null;
        private VSProjectUpdateOptions _options;

        private VSProjectManager _vsProjectManager = null;
        private string _backupVSProjectDirectory = "_projectBackup";
        private string _vsLinkBaseDirectory = "SourceLnk";
        private Dictionary<string, string> _vsProjectSource = null;
        private Dictionary<string, string> _vsProjectSourceLinks = null;
        private Dictionary<string, string> _vsProjectReferences = null;

        private CompilerProjectReader _runsourceProjectReader = null;
        private Dictionary<string, string> _runsourceProjectSources = null;
        private Dictionary<string, string> _runsourceProjectSourceLinks = null;
        private Dictionary<string, string> _runsourceProjectAssembliesReferences = null;

        public RunSourceVSProjectManager(string vsProject, string runsourceProject, VSProjectUpdateOptions options)
        {
            _vsProjectManager = new VSProjectManager(vsProject);
            //_runsourceProject = runsourceProject;
            _runsourceProjectReader = CompilerProjectReader.Create(new XmlConfig(runsourceProject).GetConfigElement("/AssemblyProject"));
            _options = options;
        }

        public string BackupVSProjectDirectory { get { return _backupVSProjectDirectory; } set { _backupVSProjectDirectory = value; } }

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
                    BackupVSProject();
                }
                //Trace.WriteLine($"  save \"{_vsProjectManager.ProjectFile}\"");
                _vsProjectManager.Save();
            }

            return result;
        }

        private void BackupVSProject()
        {
            string vsProjectFile = _vsProjectManager.ProjectFile;
            string directory = zPath.Combine(zPath.GetDirectoryName(vsProjectFile), _backupVSProjectDirectory);
            //string file = zfile.GetNewIndexedFileName(directory) + "_" + zPath.GetFileName(vsProjectFile);
            string file = zfile.GetNewFilename(zPath.Combine(directory, zPath.GetFileName(vsProjectFile)), forceNewFilename: true);
            //zfile.CreateFileDirectory(file);
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
                    _vsProjectManager.RemoveSource(source.File);
                    sourceRemovedCount++;
                }
            }
            return sourceRemovedCount;
        }

        private int AddSources()
        {
            ReadVSProjectSource();
            int sourceAddedCount = 0;
            foreach (RunSourceSource source in GetRunSourceSources(_runsourceProjectReader, _vsProjectManager.ProjectFile))
            {
                // relativePath : "Properties\AssemblyInfo.cs"
                string relativePath = source.RelativePath;
                string relativePath2 = relativePath.ToLower();
                if (!_vsProjectSource.ContainsKey(relativePath2))
                {
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
                    _vsProjectManager.RemoveSourceLink(source.File);
                    sourceLinkRemovedCount++;
                }
            }
            return sourceLinkRemovedCount;
        }

        private int AddSourcesLinks()
        {
            ReadVSProjectSourceLinks();
            int sourceLinkAddedCount = 0;
            //foreach (CompilerFile source in GetRunsourceProjectSourcesLinks())
            //foreach (CompilerFile source in _runsourceProjectReader.GetSourcesLinks(recurse: true, withoutExtensionProject: true))
            foreach (RunSourceSource source in GetRunSourceSourcesLinks(_runsourceProjectReader, _vsProjectManager.ProjectFile))
            {
                // relativePath : "..\..\..\Lib\pb\Source\pb\_pb\PBException.cs"
                // link         : "SourceLnk\Lib\pb\_pb\PBException.cs"
                //string relativePath = zpath.GetRelativePath(source.File, _vsProjectManager.ProjectFile);
                //Trace.WriteLine($"zpath.GetRelativePath() : \"{source.File}\" \"{_vsProjectManager.ProjectFile}\" relativePath \"{relativePath}\"");
                string relativePath = source.RelativePath;
                string link = GetLinkPath(relativePath);
                string relativePath2 = relativePath.ToLower();
                if (!_vsProjectSourceLinks.ContainsKey(relativePath2))
                {
                    _vsProjectSourceLinks.Add(relativePath2, link);
                    _vsProjectManager.AddSourceLink(relativePath, link);
                    sourceLinkAddedCount++;
                }
            }
            return sourceLinkAddedCount;
        }

        public static IEnumerable<RunSourceSource> GetRunSourceSources(CompilerProjectReader runsourceProjectReader, string baseFile = null)
        {
            if (baseFile == null)
                baseFile = runsourceProjectReader.ProjectFile;
            foreach (CompilerFile source in runsourceProjectReader.GetSources(recurse: true, withoutExtensionProject: true))
            {
                // relativePath : "..\..\..\Lib\pb\Source\pb\_pb\PBException.cs"
                // link         : "SourceLnk\Lib\pb\_pb\PBException.cs"
                string relativePath = zpath.GetRelativePath(source.File, baseFile);
                yield return new RunSourceSource { File = source.File, RelativePath = relativePath, ProjectFile = source.ProjectFile };
            }
        }

        public static IEnumerable<RunSourceSource> GetRunSourceSourcesLinks(CompilerProjectReader runsourceProjectReader, string baseFile = null)
        {
            if (baseFile == null)
                baseFile = runsourceProjectReader.ProjectFile;
            foreach (CompilerFile source in runsourceProjectReader.GetSourcesLinks(recurse: true, withoutExtensionProject: true))
            {
                // relativePath : "..\..\..\Lib\pb\Source\pb\_pb\PBException.cs"
                // link         : "SourceLnk\Lib\pb\_pb\PBException.cs"
                string relativePath = zpath.GetRelativePath(source.File, baseFile);
                yield return new RunSourceSource { File = source.File, RelativePath = relativePath, ProjectFile = source.ProjectFile };
            }
        }

        //private void AddSourceLink(string relativePath, string link)
        //{
        //    if (_vsProjectSourceItemGroup == null)
        //    {
        //        Trace.WriteLine($"add ItemGroup");
        //        _vsProjectSourceItemGroup = new XElement(_vsProjectNamespace + "ItemGroup");
        //        _vsProjectDocument.Root.Add(_vsProjectSourceItemGroup);
        //    }
        //    Trace.WriteLine($"add source \"{relativePath}\"");
        //    _vsProjectSourceItemGroup.Add(new XElement(_vsProjectNamespace + "Compile", new XAttribute("Include", relativePath), new XElement(_vsProjectNamespace + "Link", new XText(link))));
        //    _vsProjectModified = true;
        //}

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
                    _vsProjectManager.RemoveAssemblyReference(reference.Name);
                    assemblyReferenceRemovedCount++;
                }
            }
            return assemblyReferenceRemovedCount;
        }

        private int AddAssembliesReferences()
        {
            ReadVSProjectReferences();
            int assemblyReferenceAddedCount = 0;
            foreach (RunSourceReference reference in GetRunSourceAssemblies(_runsourceProjectReader, _vsProjectManager.ProjectFile))
            {
                string referenceName = reference.Name.ToLower();
                if (!_vsProjectReferences.ContainsKey(referenceName))
                {
                    _vsProjectReferences.Add(referenceName, null);
                    _vsProjectManager.AddReference(reference.Name, reference.RelativePath);
                    assemblyReferenceAddedCount++;
                }
            }
            return assemblyReferenceAddedCount;
        }

        public static IEnumerable<RunSourceReference> GetRunSourceAssemblies(CompilerProjectReader runsourceProjectReader, string baseFile = null)
        {
            if (baseFile == null)
                baseFile = runsourceProjectReader.ProjectFile;
            foreach (CompilerAssembly assembly in runsourceProjectReader.GetAssemblies(recurse: true, withoutExtensionProject: true))
            {
                if (assembly.VSExclude)
                    continue;
                string relativePath = null;
                if (!assembly.FrameworkAssembly)
                {
                    //relativePath = zpath.GetRelativePath(assembly.File, _vsProjectManager.ProjectFile);
                    relativePath = zpath.GetRelativePath(assembly.File, baseFile);
                    //Trace.WriteLine($"GetRunSourceAssemblies() : relativePath \"{relativePath}\" file \"{assembly.File}\" project file \"{_vsProjectManager.ProjectFile}\"");
                }
                string name = zPath.GetFileNameWithoutExtension(assembly.File);
                string name2 = name.ToLower();
                //if (name2 == "mscorlib" || name2 == "runsource" || name2 == "runsource.irunsource" || name2 == "runsource.command")
                //    continue;
                yield return new RunSourceReference { Name = name, RelativePath = relativePath, FrameworkAssembly = assembly.FrameworkAssembly, RunSourceAssembly = assembly.RunSourceAssembly, ProjectFile = assembly.ProjectFile };
            }
        }

        //private void AddReference(string file, string relativePath)
        //{
        //    if (_vsProjectReferenceItemGroup == null)
        //    {
        //        _vsProjectReferenceItemGroup = new XElement(_vsProjectNamespace + "ItemGroup");
        //        _vsProjectDocument.Root.Add(_vsProjectReferenceItemGroup);
        //    }
        //    Trace.WriteLine($"add reference \"{file}\"");
        //    XElement reference = new XElement(_vsProjectNamespace + "Reference", new XAttribute("Include", file));
        //    if (relativePath != null)
        //        reference.Add(new XElement(_vsProjectNamespace + "HintPath", new XText(relativePath)));
        //    _vsProjectReferenceItemGroup.Add(reference);
        //    _vsProjectModified = true;
        //}

        //private IEnumerable<CompilerFile> GetRunsourceProjectSourcesLinks()
        //{
        //    OpenCompilerProject();
        //    foreach (CompilerFile source in _runsourceProjectReader.GetSourcesLinks())
        //    {
        //        yield return source;
        //    }
        //    foreach (CompilerProjectReader projectReader2 in _runsourceProjectReader.GetIncludeProjects(withoutExtensionProject: true))
        //    {
        //        foreach (CompilerFile source in projectReader2.GetSourcesLinks())
        //        {
        //            yield return source;
        //        }
        //    }
        //}

        //private IEnumerable<CompilerAssembly> GetRunsourceProjectReferences()
        //{
        //    //OpenCompilerProject();
        //    foreach (CompilerAssembly assembly in _runsourceProjectReader.GetAssemblies())
        //    {
        //        yield return assembly;
        //    }
        //    foreach (CompilerProjectReader projectReader2 in _runsourceProjectReader.GetIncludeProjects(withoutExtensionProject: true))
        //    {
        //        foreach (CompilerAssembly assembly in projectReader2.GetAssemblies())
        //        {
        //            yield return assembly;
        //        }
        //    }
        //}

        //private void ReadVSProject()
        //{
        //    if (_vsProjectDocument == null)
        //    {
        //        _vsProjectDocument = XDocument.Load(_vsProject);
        //        _vsProjectNamespace = _vsProjectDocument.Root.GetDefaultNamespace();
        //        //Trace.WriteLine($"root default namespace {_vsProjectDocument.Root.GetDefaultNamespace()}");
        //    }
        //}

        //private void OpenCompilerProject()
        //{
        //    if (_runsourceProjectReader == null)
        //        _runsourceProjectReader = CompilerProjectReader.Create(new XmlConfig(_runsourceProject).GetConfigElement("/AssemblyProject"));
        //}

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
                foreach (CompilerFile source in _runsourceProjectReader.GetSources(recurse: true, withoutExtensionProject: true))
                {
                    string relativePath = zpath.GetRelativePath(source.File, _vsProjectManager.ProjectFile).ToLower();
                    if (!_runsourceProjectSources.ContainsKey(relativePath))
                        _runsourceProjectSources.Add(relativePath, null);
                }
            }
        }

        private void ReadRunSourceProjectSourceLinks()
        {
            if (_runsourceProjectSourceLinks == null)
            {
                _runsourceProjectSourceLinks = new Dictionary<string, string>();
                foreach (CompilerFile source in _runsourceProjectReader.GetSourcesLinks(recurse: true, withoutExtensionProject: true))
                {
                    string relativePath = zpath.GetRelativePath(source.File, _vsProjectManager.ProjectFile).ToLower();
                    string link = GetLinkPath(relativePath);
                    if (!_runsourceProjectSourceLinks.ContainsKey(relativePath))
                        _runsourceProjectSourceLinks.Add(relativePath, link);
                }
            }
        }

        private void ReadRunSourceProjectAssembliesReferences()
        {
            if (_runsourceProjectAssembliesReferences == null)
            {
                _runsourceProjectAssembliesReferences = new Dictionary<string, string>();
                foreach (RunSourceReference reference in GetRunSourceAssemblies(_runsourceProjectReader, _vsProjectManager.ProjectFile))
                {
                    string referenceName = reference.Name.ToLower();
                    if (!_runsourceProjectAssembliesReferences.ContainsKey(referenceName))
                        _runsourceProjectAssembliesReferences.Add(referenceName, reference.RelativePath);
                }
            }
        }


        //private IEnumerable<VSSourceLink> GetVSProjectSourceLinks()
        //{
        //    // <ItemGroup>
        //    //   <Compile Include="..\..\..\Lib\pb\Source\pb\_pb\Application.cs">
        //    //     <Link>SourceLnk\Lib\pb\_pb\Application.cs</Link>
        //    //   </Compile>
        //    // </ItemGroup>
        //    foreach (XElement compile in _vsProjectDocument.Root.Elements(_vsProjectNamespace + "ItemGroup").SelectMany(xe => xe.Elements(_vsProjectNamespace + "Compile")))
        //    {
        //        XElement link = compile.Element(_vsProjectNamespace + "Link");
        //        if (link != null)
        //            yield return new VSSourceLink { File = compile.Attribute("Include").Value, Link = link.Value };
        //    }
        //}

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

        public static VSProjectUpdateResult UpdateVSProject(string vsProject, string runsourceProject, VSProjectUpdateOptions options)
        {
            return new RunSourceVSProjectManager(vsProject, runsourceProject, options)._UpdateVSProject();
        }

        public static void Test_BackupVSProject(string vsProjectFile)
        {
            new RunSourceVSProjectManager(vsProjectFile, null, VSProjectUpdateOptions.None).BackupVSProject();
        }
    }
}
