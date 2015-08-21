using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using pb.Data.Xml;
using pb.IO;

namespace pb.Compiler
{
    public class CompilerProject : ICompilerProject
    {
        private string _projectFile = null;
        private string _projectDirectory = null;
        //private XElement _projectXmlElement = null;
        private XmlConfigElement _projectXmlElement = null;

        //public CompilerProject(XElement projectXmlElement, string projectFile)
        //{
        //    _projectXmlElement = projectXmlElement;
        //    _projectFile = projectFile;
        //    if (projectFile != null)
        //        _projectDirectory = zPath.GetDirectoryName(projectFile);
        //}

        public CompilerProject(XmlConfigElement projectXmlElement)
        {
            if (projectXmlElement == null)
                throw new PBException("projectXmlElement is null when creating pb.Compiler.CompilerProject");
            _projectXmlElement = projectXmlElement;
            _projectFile = projectXmlElement.XmlConfig.ConfigFile;
            if (_projectFile != null)
                _projectDirectory = zPath.GetDirectoryName(_projectFile);
        }

        public static CompilerProject Create(XmlConfigElement projectXmlElement)
        {
            if (projectXmlElement != null)
                return new CompilerProject(projectXmlElement);
            else
                return null;
        }

        public string ProjectFile { get { return _projectFile; } }
        public string ProjectDirectory { get { return _projectDirectory; } }
        //public XElement ProjectXmlElement { get { return _projectXmlElement; } }
        //public XmlConfigElement ProjectXmlElement { get { return _projectXmlElement; } }

        public string GetLanguage()
        {
            return _projectXmlElement.Get("Language");
        }

        public IEnumerable<CompilerProviderOption> GetProviderOptions()
        {
            return _projectXmlElement.GetElements("ProviderOption").Select(xe => new CompilerProviderOption { Name = xe.zAttribValue("name"), Value = xe.zAttribValue("value") });
        }

        public string GetResourceCompiler()
        {
            return _projectXmlElement.Get("ResourceCompiler");
        }

        public string GetOutputDir()
        {
            return _projectXmlElement.Get("OutputDir").zRootPath(_projectDirectory);
        }

        public string GetOutput()
        {
            return _projectXmlElement.Get("Output").zRootPath(_projectDirectory);
        }

        public bool? GetGenerateExecutable()
        {
            return _projectXmlElement.Get("GenerateExecutable").zTryParseAs<bool?>();
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

        public string GetKeyFile()
        {
            return _projectXmlElement.Get("KeyFile").zRootPath(_projectDirectory);
        }

        public string GetTarget()
        {
            return _projectXmlElement.Get("Target");
        }

        public string GetIcon()
        {
            return _projectXmlElement.Get("Icon").zRootPath(_projectDirectory);
        }

        public string GetNameSpace()
        {
            //return _projectXmlElement.zXPathValue("NameSpace");
            return _projectXmlElement.Get("NameSpace");
        }

        public IEnumerable<ICompilerProject> GetIncludeProjects()
        {
            //return _projectXmlElement.GetValues("IncludeProject").Select(file => CompilerProject.Create(new XmlConfig(file.zRootPath(_projectDirectory)).GetConfigElement("Project"))).Where(compilerProject => compilerProject != null);
            foreach (string includeProject in _projectXmlElement.GetValues("IncludeProject"))
            {
                CompilerProject compilerProject = CompilerProject.Create(new XmlConfig(includeProject.zRootPath(_projectDirectory)).GetConfigElement("Project"));
                if (compilerProject != null)
                    yield return compilerProject;
                else
                    Trace.WriteLine("IncludeProject not found \"{0}\" from project \"{1}\"", includeProject, _projectFile);
            }
        }

        public IEnumerable<string> GetUsings()
        {
            // la suppression des usings doublons est faite dans GenerateAndExecute
            foreach (string value in _projectXmlElement.GetValues("Using"))
                yield return value;
            foreach (ICompilerProject includeProject in GetIncludeProjects())
            {
                foreach (string value in includeProject.GetUsings())
                    yield return value;
            }

        }

        public IEnumerable<CompilerFile> GetSources()
        {
            //foreach (string source in _projectXmlElement.GetValues("Source"))
            //{
            //    yield return source.zRootPath(_projectDirectory);
            //}
            //return _projectXmlElement.GetValues("Source").Select(file => new CompilerFile(file.zRootPath(_projectDirectory)));
            return _projectXmlElement.GetElements("Source").Select(xe => CreateCompilerFile(xe));
        }

        public IEnumerable<CompilerFile> GetFiles()
        {
            return _projectXmlElement.GetElements("File").Select(xe => CreateCompilerFile(xe));
                //.Select(xe =>
                //{
                //    CompilerFile compilerFile = new CompilerFile(xe.Attribute("value").Value.zRootPath(_projectDirectory));
                //    foreach (XAttribute xa in xe.Attributes())
                //    {
                //        if (xa.Name != "value")
                //            compilerFile.Attributes.Add(xa.Name.LocalName, xa.Value);
                //    }
                //    return compilerFile;
                //});
        }

        private CompilerFile CreateCompilerFile(XElement xe)
        {
            CompilerFile compilerFile = new CompilerFile(xe.Attribute("value").Value.zRootPath(_projectDirectory));
            foreach (XAttribute xa in xe.Attributes())
            {
                if (xa.Name != "value")
                    compilerFile.Attributes.Add(xa.Name.LocalName, xa.Value);
            }
            return compilerFile;
        }

        public IEnumerable<CompilerAssembly> GetAssemblies()
        {
            return _projectXmlElement.GetElements("Assembly")
                .Select(xe =>
                {
                    string file = xe.zAttribValue("value");
                    string dir = zPath.GetDirectoryName(file);
                    if (dir != "")
                        file = file.zRootPath(_projectDirectory);
                    bool resolve = xe.zAttribValue("resolve").zTryParseAs<bool>(false);
                    string resolveName = xe.zAttribValue("resolveName");
                    if (resolve && resolveName == null)
                        throw new PBException("error to resolve an assembly you must specify a resolveName (\"Test_dll, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\")");
                    return new CompilerAssembly(file, resolve, resolveName);
                });
        }

        public IEnumerable<string> GetCopyOutputs()
        {
            return _projectXmlElement.GetValues("CopyOutput");
        }
    }
}
