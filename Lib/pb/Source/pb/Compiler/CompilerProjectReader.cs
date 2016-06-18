using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using pb.Data.Xml;
using pb.IO;

namespace pb.Compiler
{
    public class CompilerProjectReader : ICompilerProjectReader
    {
        private string _projectFile = null;
        private string _projectDirectory = null;
        //private XElement _projectXmlElement = null;
        private XmlConfigElement _projectXmlElement = null;
        private bool _isIncludeProject = false;
        private string _rootDirectory = null;
        private List<ICompilerProjectReader> _includeProjects = null;

        //public CompilerProject(XElement projectXmlElement, string projectFile)
        //{
        //    _projectXmlElement = projectXmlElement;
        //    _projectFile = projectFile;
        //    if (projectFile != null)
        //        _projectDirectory = zPath.GetDirectoryName(projectFile);
        //}

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
        //public XElement ProjectXmlElement { get { return _projectXmlElement; } }
        //public XmlConfigElement ProjectXmlElement { get { return _projectXmlElement; } }


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

        //public string GetLanguage()
        //{
        //    return _projectXmlElement.Get("Language");
        //}

        public CompilerLanguage GetLanguage()
        {
            //return _projectXmlElement.Get("Language");
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
            //string symbols = _projectXmlElement.Get("PreprocessorSymbol");
            //if (symbols != null)
            //{
            //    foreach (string symbol in symbols.Split(';'))
            //    {
            //        string symbol2 = symbol.Trim();
            //        if (symbol2 != "")
            //        {
            //            yield return symbol2;
            //        }
            //    }
            //}
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

        //public string GetWin32Resource()
        public CompilerFile GetWin32Resource()
        {
            //return GetFile("Win32Resource");
            return CreateCompilerFile(_projectXmlElement.GetElement("Win32Resource"));
        }

        public string GetIcon()
        {
            return GetFile("Icon");
        }

        //public IEnumerable<CompilerProviderOption> GetProviderOptions()
        //{
        //    return _projectXmlElement.GetElements("ProviderOption").Select(xe => new CompilerProviderOption { Name = xe.zAttribValue("name"), Value = xe.zAttribValue("value") });
        //}

        //public string GetResourceCompiler()
        //{
        //    return _projectXmlElement.Get("ResourceCompiler");
        //}

        // not used
        //public string GetOutputDir()
        //{
        //    //return _projectXmlElement.Get("OutputDir").zRootPath(_projectDirectory);
        //    return GetFile("OutputDir");
        //}

        //public bool? GetGenerateExecutable()
        //{
        //    return _projectXmlElement.Get("GenerateExecutable").zTryParseAs<bool?>();
        //}

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

        //public string GetInitMethod()
        //{
        //    return _projectXmlElement.Get("InitMethod");
        //}

        public IEnumerable<string> GetInitMethods()
        {
            //return _projectXmlElement.Get("InitMethod");
            // la suppression des InitMethod doublons est faite dans ???
            foreach (string value in _projectXmlElement.GetValues("InitMethod"))
                yield return value;
            foreach (ICompilerProjectReader includeProject in GetIncludeProjects())
            {
                foreach (string value in includeProject.GetInitMethods())
                    yield return value;
            }
        }

        //public string GetEndMethod()
        //{
        //    return _projectXmlElement.Get("EndMethod");
        //}

        public IEnumerable<string> GetEndMethods()
        {
            //return _projectXmlElement.Get("InitMethod");
            // la suppression des InitMethod doublons est faite dans ???
            foreach (string value in _projectXmlElement.GetValues("EndMethod"))
                yield return value;
            foreach (ICompilerProjectReader includeProject in GetIncludeProjects())
            {
                foreach (string value in includeProject.GetInitMethods())
                    yield return value;
            }
        }

        public IEnumerable<ICompilerProjectReader> GetIncludeProjects()
        {
            if (_includeProjects == null)
            {
                _includeProjects = new List<ICompilerProjectReader>();
                foreach (string includeProject in _projectXmlElement.GetValues("IncludeProject"))
                {
                    CompilerProjectReader compilerProject = CompilerProjectReader.Create(new XmlConfig(GetPathFile(includeProject)).GetConfigElement("/AssemblyProject"), isIncludeProject: true);
                    if (compilerProject != null)
                    {
                        //yield return compilerProject;
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
            foreach (ICompilerProjectReader includeProject in GetIncludeProjects())
            {
                foreach (string value in includeProject.GetUsings())
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
            //CompilerFile compilerFile = new CompilerFile(xe.Attribute("value").Value.zRootPath(_projectDirectory));
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
            //return _projectXmlElement.GetElements("Assembly")
            //    .Select(xe =>
            //    {
            //        string file = xe.zAttribValue("value");
            //        string dir = zPath.GetDirectoryName(file);
            //        if (dir != "")
            //            //file = file.zRootPath(_projectDirectory);
            //            file = GetPathFile(file);
            //        bool resolve = xe.zAttribValue("resolve").zTryParseAs<bool>(false);
            //        string resolveName = xe.zAttribValue("resolveName");
            //        if (resolve && resolveName == null)
            //            throw new PBException("error to resolve an assembly you must specify a resolveName (\"Test_dll, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\")");
            //        //bool copySource = xe.zAttribValue("copySource").zTryParseAs<bool>(false);
            //        return new CompilerAssembly(file, resolve, resolveName, this);
            //    });

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

        //public IEnumerable<CompilerUpdateDirectory> GetUpdateDirectory()
        //{
        //    foreach (XElement element in _projectXmlElement.GetElements("UpdateDirectory"))
        //    {
        //        yield return new CompilerUpdateDirectory { SourceDirectory = GetPathFile(element.zAttribValue("source")), DestinationDirectory = GetPathFile(element.zAttribValue("destination")) };
        //    }
        //}

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
