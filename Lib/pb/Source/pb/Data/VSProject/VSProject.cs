using pb.Compiler;
using pb.Data.Xml;
using pb.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace pb.Data.VSProject
{
    public class VSProject
    {
        private string _vsProjectFile = null;
        private XDocument _vsProjectDocument = null;
        private string _linkBaseDirectory = "SourceLnk";
        private Dictionary<string, string> _vsProjectSourceLinks = null;
        private Dictionary<string, string> _vsProjectReferences = null;
        private XNamespace _vsProjectNamespace = null;
        private XElement _vsProjectSourceItemGroup = null;
        private XElement _vsProjectReferenceItemGroup = null;
        private bool _vsProjectModified = false;

        private string _compilerProjectFile = null;
        private CompilerProjectReader _projectReader = null;

        public VSProject(string vsProjectFile, string compilerProjectFile)
        {
            _vsProjectFile = vsProjectFile;
            _compilerProjectFile = compilerProjectFile;
        }

        private void _AddCompilerProject()
        {
            AddSourceLinks();
            AddReferences();
            if (_vsProjectModified)
                _vsProjectDocument.Save(_vsProjectFile);
        }

        private void AddSourceLinks()
        {
            ReadVSProject();
            ReadVSProjectSourceLinks();
            foreach (CompilerFile source in GetSources())
            {
                string relativePath = zpath.GetRelativePath(source.File, _vsProjectFile);
                string link = GetLinkPath(relativePath);
                if (!_vsProjectSourceLinks.ContainsKey(relativePath))
                {
                    _vsProjectSourceLinks.Add(relativePath, link);
                    AddSourceLink(relativePath, link);
                }
            }
        }

        private void AddSourceLink(string relativePath, string link)
        {
            if (_vsProjectSourceItemGroup == null)
            {
                Trace.WriteLine($"add ItemGroup");
                _vsProjectSourceItemGroup = new XElement(_vsProjectNamespace + "ItemGroup");
                _vsProjectDocument.Root.Add(_vsProjectSourceItemGroup);
            }
            Trace.WriteLine($"add source \"{relativePath}\"");
            _vsProjectSourceItemGroup.Add(new XElement(_vsProjectNamespace + "Compile", new XAttribute("Include", relativePath), new XElement(_vsProjectNamespace + "Link", new XText(link))));
            _vsProjectModified = true;
        }

        private Regex _relativeDirectory = new Regex(@"^[\.\\/]*", RegexOptions.Compiled);
        private string GetLinkPath(string relativePath)
        {
            relativePath = _relativeDirectory.Replace(relativePath, "");
            if (relativePath.StartsWith(@"Lib\pb\Source\"))
                relativePath = @"Lib\" + relativePath.Substring(14);
            return zPath.Combine(_linkBaseDirectory, relativePath);
        }

        private void AddReferences()
        {
            ReadVSProject();
            ReadVSProjectReferences();
            foreach (CompilerAssembly assembly in GetReferences())
            {
                string relativePath = null;
                if (!assembly.FrameworkAssembly)
                    relativePath = zpath.GetRelativePath(assembly.File, _vsProjectFile);
                string file = zPath.GetFileNameWithoutExtension(assembly.File);
                string file2 = file.ToLower();
                if (file2 == "mscorlib" || file2 == "runsource" || file2 == "runsource.irunsource" || file2 == "runsource.command")
                    continue;
                if (!_vsProjectReferences.ContainsKey(file))
                {
                    _vsProjectReferences.Add(file, null);
                    AddReference(file, relativePath);
                }
            }
        }

        private void AddReference(string file, string relativePath)
        {
            if (_vsProjectReferenceItemGroup == null)
            {
                _vsProjectReferenceItemGroup = new XElement(_vsProjectNamespace + "ItemGroup");
                _vsProjectDocument.Root.Add(_vsProjectReferenceItemGroup);
            }
            Trace.WriteLine($"add reference \"{file}\"");
            XElement reference = new XElement(_vsProjectNamespace + "Reference", new XAttribute("Include", file));
            if (relativePath != null)
                reference.Add(new XElement(_vsProjectNamespace + "HintPath", new XText(relativePath)));
            _vsProjectReferenceItemGroup.Add(reference);
            _vsProjectModified = true;
        }

        private IEnumerable<CompilerFile> GetSources()
        {
            OpenCompilerProject();
            foreach (CompilerFile source in _projectReader.GetSources())
            {
                yield return source;
            }
            foreach (CompilerProjectReader projectReader2 in _projectReader.GetIncludeProjects())
            {
                foreach (CompilerFile source in projectReader2.GetSources())
                {
                    yield return source;
                }
            }
        }

        private IEnumerable<CompilerAssembly> GetReferences()
        {
            OpenCompilerProject();
            foreach (CompilerAssembly assembly in _projectReader.GetAssemblies())
            {
                yield return assembly;
            }
            foreach (CompilerProjectReader projectReader2 in _projectReader.GetIncludeProjects())
            {
                foreach (CompilerAssembly assembly in projectReader2.GetAssemblies())
                {
                    yield return assembly;
                }
            }
        }

        private void ReadVSProject()
        {
            if (_vsProjectDocument == null)
            {
                _vsProjectDocument = XDocument.Load(_vsProjectFile);
                _vsProjectNamespace = _vsProjectDocument.Root.GetDefaultNamespace();
                //Trace.WriteLine($"root default namespace {_vsProjectDocument.Root.GetDefaultNamespace()}");
            }
        }

        private void OpenCompilerProject()
        {
            if (_projectReader == null)
                _projectReader = CompilerProjectReader.Create(new XmlConfig(_compilerProjectFile).GetConfigElement("/AssemblyProject"));
        }

        private void ReadVSProjectSourceLinks()
        {
            if (_vsProjectSourceLinks == null)
            {
                //Trace.WriteLine("read project source links");
                _vsProjectSourceLinks = new Dictionary<string, string>();
                // //*[local-name()='ItemGroup']
                // "//ItemGroup/Compile"
                //XNamespace ns = new XNamespace();
                //_vsProjectDocument.Root.Elements(_vsProjectNamespace + "ItemGroup").SelectMany(xe => xe.Elements(_vsProjectNamespace + "Compile"));
                //foreach (XElement compile in _vsProjectDocument.Root.zXPathElements("//*[local-name()='ItemGroup']/*[local-name()='Compile']"))
                foreach (XElement compile in _vsProjectDocument.Root.Elements(_vsProjectNamespace + "ItemGroup").SelectMany(xe => xe.Elements(_vsProjectNamespace + "Compile")))
                {
                    //string include = compile.Attribute("Include").Value;
                    //Trace.WriteLine($"search source link \"{include}\"");
                    XElement link = compile.Element(_vsProjectNamespace + "Link");
                    if (link != null)
                    {
                        string include = compile.Attribute("Include").Value;
                        //Trace.WriteLine($"add source link \"{include}\"");
                        if (!_vsProjectSourceLinks.ContainsKey(include))
                            _vsProjectSourceLinks.Add(include, link.Value);
                    }
                }
            }
        }

        private void ReadVSProjectReferences()
        {
            if (_vsProjectReferences == null)
            {
                _vsProjectReferences = new Dictionary<string, string>();
                //foreach (XElement reference in _vsProjectDocument.zXPathElements("Project/ItemGroup/Reference"))
                foreach (XElement reference in _vsProjectDocument.Root.Elements(_vsProjectNamespace + "ItemGroup").SelectMany(xe => xe.Elements(_vsProjectNamespace + "Reference")))
                {
                    _vsProjectReferences.Add(reference.Attribute("Include").Value, null);
                }
            }
        }

        public static void AddCompilerProject(string vsProjectFile, string compilerProjectFile)
        {
            new VSProject(vsProjectFile, compilerProjectFile)._AddCompilerProject();
        }
    }
}
