using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using pb.Data.Xml;
using pb.IO;
using pb.Data;

// GetIncludeProjects() used by
//   GetInitEndMethods()
//   GetUsings()


namespace pb.Compiler
{
    public class CompilerLanguage
    {
        public string Name;
        public string Version;
    }

    public class CompilerProviderOption
    {
        public string Name;
        public string Value;
    }

    public class CompilerFile
    {
        public string File = null;
        public string RelativePath = null;
        public Dictionary<string, string> Attributes = new Dictionary<string, string>();
        public CompilerProjectReader Project = null;

        public CompilerFile(string file, string rootDirectory = null)
        {
            File = file;
            if (rootDirectory != null && file.StartsWith(rootDirectory))
            {
                RelativePath = file.Substring(rootDirectory.Length);
                if (RelativePath.StartsWith("\\"))
                    RelativePath = RelativePath.Substring(1);
            }
            else
                RelativePath = zPath.Combine("NoRoot", zPath.GetFileName(file));
        }
    }

    public class CompilerAssembly
    {
        public string File = null;
        public bool FrameworkAssembly = false;
        public bool Resolve = false;
        public string ResolveName = null;
        public CompilerProjectReader Project = null;
    }

    public enum RunType
    {
        Always = 0,
        Once
    }

    public class InitEndMethod
    {
        public string Name;
        //public object[] Parameters;          // used in RunSourceInitEndMethods_v2
        public NamedValues<ZValue> Parameters;     // used in RunSourceInitEndMethods_v2
        public RunType RunType;
    }

    public class CompilerProjectReader
    {
        private string _projectFile = null;
        private string _projectDirectory = null;
        private XmlConfigElement _projectXmlElement = null;
        private bool _isIncludeProject = false;
        private string _rootDirectory = null;
        private List<CompilerProjectReader> _includeProjects = null;

        private CompilerProjectReader(XmlConfigElement projectXmlElement, bool isIncludeProject = false)
        {
            if (projectXmlElement == null)
                throw new PBException("projectXmlElement is null when creating pb.Compiler.CompilerProject");
            _projectXmlElement = projectXmlElement;
            _projectFile = projectXmlElement.XmlConfig.ConfigFile;
            if (_projectFile != null)
                _projectDirectory = zPath.GetDirectoryName(_projectFile);
            _isIncludeProject = isIncludeProject;
        }

        public static CompilerProjectReader Create(XmlConfigElement projectXmlElement, bool isIncludeProject = false)
        {
            if (projectXmlElement != null)
                return new CompilerProjectReader(projectXmlElement, isIncludeProject);
            else
                return null;
        }

        public string ProjectFile { get { return _projectFile; } }
        public string ProjectDirectory { get { return _projectDirectory; } }
        public bool IsIncludeProject { get { return _isIncludeProject; } }

        public CompilerFile GetProjectCompilerFile()
        {
            CompilerFile compilerFile = new CompilerFile(_projectFile, GetRootDirectory());
            compilerFile.Project = this;
            return compilerFile;
        }

        public string GetRootDirectory()
        {
            if (_rootDirectory == null)
                _rootDirectory = GetFile("Root");
            return _rootDirectory;
        }

        public CompilerLanguage GetLanguage()
        {
            XElement xe = _projectXmlElement.GetElement("Language");
            if (xe != null)
                return new CompilerLanguage { Name = xe.zExplicitAttribValue("name"), Version = xe.zAttribValue("version") };
            else
                return null;
        }

        public string GetFrameworkVersion()
        {
            return _projectXmlElement.Get("FrameworkVersion");
        }

        public string GetTarget()
        {
            return _projectXmlElement.Get("Target");
        }

        public string GetPlatform()
        {
            return _projectXmlElement.Get("Platform");
        }

        public bool? GetGenerateInMemory()
        {
            return _projectXmlElement.Get("GenerateInMemory").zTryParseAs<bool?>();
        }

        public bool? GetDebugInformation()
        {
            return _projectXmlElement.Get("DebugInformation").zTryParseAs<bool?>();
        }

        public int? GetWarningLevel()
        {
            return _projectXmlElement.Get("WarningLevel").zTryParseAs<int?>();
        }

        public IEnumerable<string> GetCompilerOptions()
        {
            return _projectXmlElement.GetValues("CompilerOptions");
        }

        public IEnumerable<string> GetPreprocessorSymbols()
        {
            foreach (string symbols in _projectXmlElement.GetValues("PreprocessorSymbol"))
            {
                foreach (string symbol in symbols.Split(';'))
                {
                    string symbol2 = symbol.Trim();
                    if (symbol2 != "")
                    {
                        yield return symbol2;
                    }
                }
            }
        }

        public string GetOutput()
        {
            return GetFile("Output");
        }

        public CompilerFile GetWin32Resource()
        {
            return CreateCompilerFile(_projectXmlElement.GetElement("Win32Resource"));
        }

        public string GetIcon()
        {
            return GetFile("Icon");
        }

        public string GetKeyFile()
        {
            return GetFile("KeyFile");
        }

        public bool? GetCopySourceFiles()
        {
            return _projectXmlElement.Get("CopySourceFiles").zTryParseAs<bool?>();
        }

        public bool? GetCopyRunSourceSourceFiles()
        {
            return _projectXmlElement.Get("CopyRunSourceSourceFiles").zTryParseAs<bool?>();
        }

        public string GetNameSpace()
        {
            return _projectXmlElement.GetExplicit("NameSpace");
        }

        public IEnumerable<InitEndMethod> GetInitMethods()
        {
            return GetInitEndMethods("InitMethod");
        }

        public IEnumerable<InitEndMethod> GetEndMethods()
        {
            return GetInitEndMethods("EndMethod");
        }

        private IEnumerable<InitEndMethod> GetInitEndMethods(string name)
        {
            foreach (XElement xe in _projectXmlElement.GetElements(name))
                yield return new InitEndMethod { Name = xe.zExplicitAttribValue("value"), RunType = GetRunType(xe.zAttribValue("run")), Parameters = GetParameters(xe.zAttribValue("params")) };
            foreach (CompilerProjectReader includeProject in GetIncludeProjects())
            {
                foreach (XElement xe in includeProject._projectXmlElement.GetElements(name))
                    yield return new InitEndMethod { Name = xe.zExplicitAttribValue("value"), RunType = GetRunType(xe.zAttribValue("run")), Parameters = GetParameters(xe.zAttribValue("params")) };
            }
        }

        private static RunType GetRunType(string runType)
        {
            if (runType == null)
                return RunType.Once;
            switch (runType.ToLowerInvariant())
            {
                case "always":
                    return RunType.Always;
                case "one":
                    return RunType.Once;
                default:
                    throw new PBException($"unknow run type \"{runType}\"");
            }
        }

        private static NamedValues<ZValue> GetParameters(string parameters)
        {
            if (parameters != null)
                // warning dont set useLowercaseKey to true these parameters are used to call init and end method
                return ParseNamedValues.ParseValues(parameters);
            else
                return null;
        }

        // attention il peut y avoir des doublons, used by GetInitEndMethods() and GetUsings()
        public IEnumerable<CompilerProjectReader> GetIncludeProjects()
        {
            if (_includeProjects == null)
            {
                _includeProjects = new List<CompilerProjectReader>();
                foreach (string includeProject in _projectXmlElement.GetValues("IncludeProject"))
                {
                    CompilerProjectReader compilerProject = CompilerProjectReader.Create(new XmlConfig(GetPathFile(includeProject)).GetConfigElement("/AssemblyProject"), isIncludeProject: true);
                    if (compilerProject != null)
                    {
                        _includeProjects.Add(compilerProject);
                        _includeProjects.AddRange(compilerProject.GetIncludeProjects());
                    }
                    else
                        Trace.WriteLine("IncludeProject not found \"{0}\" from project \"{1}\"", includeProject, _projectFile);
                }
            }
            return _includeProjects;
        }

        public IEnumerable<string> GetUsings()
        {
            // la suppression des usings doublons est faite dans GenerateAndExecute
            foreach (string value in _projectXmlElement.GetValues("Using"))
                yield return value;
            foreach (CompilerProjectReader includeProject in GetIncludeProjects())
            {
                foreach (string value in includeProject._projectXmlElement.GetValues("Using"))
                    yield return value;
            }
        }

        public IEnumerable<CompilerFile> GetSources()
        {
            return _projectXmlElement.GetElements("Source").Select(xe => CreateCompilerFile(xe));
        }

        public IEnumerable<CompilerFile> GetFiles()
        {
            return _projectXmlElement.GetElements("File").Select(xe => CreateCompilerFile(xe));
        }

        public IEnumerable<CompilerFile> GetSourceFiles()
        {
            return _projectXmlElement.GetElements("SourceFile").Select(xe => CreateCompilerFile(xe));
        }

        private CompilerFile CreateCompilerFile(XElement xe)
        {
            if (xe == null)
                return null;
            CompilerFile compilerFile = new CompilerFile(GetPathFile(xe.Attribute("value").Value), GetRootDirectory());
            compilerFile.Project = this;
            foreach (XAttribute xa in xe.Attributes())
            {
                if (xa.Name != "value")
                    compilerFile.Attributes.Add(xa.Name.LocalName, xa.Value);
            }
            return compilerFile;
        }

        public IEnumerable<CompilerAssembly> GetAssemblies()
        {
            return
                _projectXmlElement.GetValues("FrameworkAssembly")
                .Select(file =>
                {
                    return new CompilerAssembly { File = file, FrameworkAssembly = true, Project = this };
                }).Concat(
                _projectXmlElement.GetElements("Assembly")
                .Select(xe =>
                {
                    string file = xe.zAttribValue("value");
                    string dir = zPath.GetDirectoryName(file);
                    if (dir != "")
                        file = GetPathFile(file);
                    bool resolve = xe.zAttribValue("resolve").zTryParseAs<bool>(false);
                    string resolveName = xe.zAttribValue("resolveName");
                    if (resolve && resolveName == null)
                        throw new PBException("missing resolveName for assembly \"{0}\" (resolveName = \"Test_dll, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\")", file);
                    return new CompilerAssembly { File = file, Resolve = resolve, ResolveName = resolveName, Project = this };
                }));
        }

        public IEnumerable<string> GetCopyOutputs()
        {
            // modif le 26/03/2016
            //return _projectXmlElement.GetValues("CopyOutput");
            return _projectXmlElement.GetValues("CopyOutput").Select(dir => GetPathFile(dir));
        }

        private string GetFile(string xpath)
        {
            return GetPathFile(_projectXmlElement.Get(xpath));
        }

        private string GetPathFile(string file)
        {
            return file.zRootPath(_projectDirectory);
        }
    }
}
