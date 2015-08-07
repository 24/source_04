using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using pb.Data.Xml;
using pb.IO;

namespace pb.Compiler
{
    public class CompilerProject
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

        public string ProjectFile { get { return _projectFile; } }
        //public XElement ProjectXmlElement { get { return _projectXmlElement; } }
        public XmlConfigElement ProjectXmlElement { get { return _projectXmlElement; } }

        public string GetNameSpace()
        {
            //return _projectXmlElement.zXPathValue("NameSpace");
            return _projectXmlElement.Get("NameSpace");
        }

        public IEnumerable<string> GetUsings()
        {
            //return _projectXmlElement.zXPathValues("Using");
            return _projectXmlElement.GetValues("Using");
        }

        public IEnumerable<string> GetSources()
        {
            //string[] sSources = projectConfig.GetValues("Project/Source/@value");
            //for (int i = 0; i < sSources.Length; i++)
            //{
            //    sSources[i] = GetPathSource(sSources[i]);
            //}
            //return sSources;

            //foreach (string source in _projectXmlElement.zXPathValues("Source"))
            foreach (string source in _projectXmlElement.GetValues("Source"))
            {
                //string sourcePath = source;
                //if (!zPath.IsPathRooted(sourcePath) && _projectDirectory != null)
                //    sourcePath = zPath.Combine(_projectDirectory, sourcePath);
                //yield return sourcePath;
                yield return source.zRootPath(_projectDirectory);
            }
        }
    }
}
