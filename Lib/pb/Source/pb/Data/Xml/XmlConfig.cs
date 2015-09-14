using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;
using pb.IO;

// doc
//
// XmlConfig
//   AlternateConfigDirectory :
//     from Pib.config.xml      : <!--<AlternateConfigDirectory                                  value = ".." />-->
//     from Download_config.xml : <!--<AlternateConfigDirectory                                  value = ".." />-->

namespace pb.Data.Xml
{
    //public class XmlConfigException : Exception
    //{
    //    public XmlConfigException(string sMessage) : base(sMessage) { }
    //    public XmlConfigException(string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm)) { }
    //    public XmlConfigException(Exception InnerException, string sMessage) : base(sMessage, InnerException) { }
    //    public XmlConfigException(Exception InnerException, string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm), InnerException) { }
    //}

    public class XmlConfig : XmlConfigElement
    {
        private static XmlConfig __currentConfig = null;
        //private static bool __initAllVariablesValues = true;
        private static string _defaultSuffixConfigName = ".config.xml";
        private static string _defaultSuffixLocalConfigName = ".local.xml"; // "_local"

        //private string _configPath = null;
        private string _configFile = null;
        private DateTime _configFileDateTime = DateTime.MinValue;
        private XDocument _xdocument = null;
        //private string _configLocalPath = null;
        private string _configLocalFile = null;
        private DateTime _configLocalFileDateTime;
        private XDocument _localXDocument = null;
        //private string _rootDir = null;
        //private Dictionary<string, string> _textVariables = null;

        public XmlConfig(string configFile = null, string configLocalFile = null, bool createConfigIfFileDontExist = false)
        {
            //SetConfigPath(configPath);
            //SetConfigLocalPath(configLocalPath);

            if (configFile != null)
                _configFile = configFile.zRootPath(zapp.GetEntryAssemblyDirectory());
            else
            {
                // default config file, ex : c:\...\runsource.runsource.config.xml
                string file = zapp.GetEntryAssemblyFile();
                if (file != null)
                    //_configFile = file + _defaultSuffixConfigName;
                    _configFile = zPath.Combine(zPath.GetDirectoryName(file), zPath.GetFileNameWithoutExtension(file) + _defaultSuffixConfigName);
            }

            if (configLocalFile != null)
                _configLocalFile = configLocalFile.zRootPath(zapp.GetEntryAssemblyDirectory());
            else if (_configFile != null)
                _configLocalFile = zpath.PathSetFileName(_configFile, zPath.GetFileNameWithoutExtension(_configFile) + _defaultSuffixLocalConfigName);

            Init(createConfigIfFileDontExist);
        }

        //public XmlConfig(string configPath, string configLocalPath)
        //{
        //    SetConfigPath(configPath);
        //    SetConfigLocalPath(configLocalPath);
        //    Init();
        //}

        //public XmlConfig(string configPath)
        //{
        //    SetConfigPath(configPath);
        //    _configLocalPath = GetDefaultConfigLocalPath();
        //    Init();
        //}

        //public XmlConfig()
        //{
        //    _configFile = GetDefaultConfigFile();
        //    _configLocalFile = GetDefaultConfigLocalPath();
        //    Init();
        //}

        //public static bool InitAllVariablesValues { get { return __initAllVariablesValues; } }

        public static XmlConfig CurrentConfig
        {
            get
            {
                if (__currentConfig == null) __currentConfig = new XmlConfig();
                return __currentConfig;
            }
            set { __currentConfig = value; }
        }

        public string ConfigFile { get { return _configFile; } }
        public XDocument XDocument { get { return _xdocument; } }
        public string ConfigLocalFile { get { return _configLocalFile; } }
        public XDocument LocalXDocument { get { return _localXDocument; } }

        private void Init(bool createConfigIfFileDontExist)
        {
            if (zFile.Exists(_configFile))
            {
                _xdocument = XDocument.Load(_configFile);
                string alternateConfigDirectory = _xdocument.Root.zXPathValue("AlternateConfigDirectory");
                if (alternateConfigDirectory != null)
                {
                    _configFile = zpath.PathSetDirectory(_configFile, alternateConfigDirectory);
                    _xdocument = XDocument.Load(_configFile);
                }
                //_configFileDateTime = new FileInfo(_configFile).LastWriteTime;
                _configFileDateTime = zFile.CreateFileInfo(_configFile).LastWriteTime;
                //SetXmlAttributesTextVariables();
            }
            else if (createConfigIfFileDontExist)
                _xdocument = new XDocument();
            else
                throw new PBException("file not found \"{0}\"", _configFile);

            if (_xdocument.Root == null)
                _xdocument.Add(new XElement("configuration"));

            if (zFile.Exists(_configLocalFile))
            {
                _localXDocument = XDocument.Load(_configLocalFile);
                string alternateConfigDirectory = _localXDocument.Root.zXPathValue("AlternateConfigDirectory");
                if (alternateConfigDirectory != null)
                {
                    _configLocalFile = zpath.PathSetDirectory(_configLocalFile, alternateConfigDirectory);
                    _localXDocument = XDocument.Load(_configLocalFile);
                }
                //_configLocalFileDateTime = new FileInfo(_configLocalFile).LastWriteTime;
                _configLocalFileDateTime = zFile.CreateFileInfo(_configLocalFile).LastWriteTime;
            }
            else
                _localXDocument = new XDocument();

            if (_localXDocument.Root == null)
                _localXDocument.Add(new XElement("configuration"));

            // set XmlConfigElement values
            _xmlConfig = this;
            _xpathElement = "/";
            _element = _xdocument.Root;
            _localElement = _localXDocument.Root;

            SetAllVariablesValues();
        }

        public bool Refresh()
        {
            bool reload = false;
            //if (zFile.Exists(_configFile) && new FileInfo(_configFile).LastWriteTime > _configFileDateTime)
            if (zFile.Exists(_configFile) && zFile.CreateFileInfo(_configFile).LastWriteTime > _configFileDateTime)
            {
                _xdocument = XDocument.Load(_configFile);
                //_configFileDateTime = new FileInfo(_configFile).LastWriteTime;
                _configFileDateTime = zFile.CreateFileInfo(_configFile).LastWriteTime;
                //_textVariables = null;
                //SetXmlAttributesTextVariables();
                if (_xdocument.Root == null)
                    _xdocument.Add(new XElement("configuration"));
                _element = _xdocument.Root;
                reload = true;
            }
            //if (zFile.Exists(_configLocalFile) && new FileInfo(_configLocalFile).LastWriteTime > _configLocalFileDateTime)
            if (zFile.Exists(_configLocalFile) && zFile.CreateFileInfo(_configLocalFile).LastWriteTime > _configLocalFileDateTime)
            {
                _localXDocument = XDocument.Load(_configLocalFile);
                //_configLocalFileDateTime = new FileInfo(_configLocalFile).LastWriteTime;
                _configLocalFileDateTime = zFile.CreateFileInfo(_configLocalFile).LastWriteTime;
                if (_localXDocument.Root == null)
                    _localXDocument.Add(new XElement("configuration"));
                _localElement = _localXDocument.Root;
                reload = true;
            }
            if (reload)
            {
                //RootDirInit();
                //if (__initAllVariablesValues)
                SetAllVariablesValues();
            }
            return reload;
        }

        //private void SetConfigPath(string configPath)
        //{
        //    if (configPath != null && !zPath.IsPathRooted(configPath))
        //    {
        //        string exePath = zapp.GetExecutablePath();
        //        if (exePath != null)
        //            configPath = zPath.Combine(zPath.GetDirectoryName(exePath), configPath);
        //    }
        //    _configPath = configPath;
        //}

        //private void SetConfigLocalPath(string configLocalPath)
        //{
        //    if (!zPath.IsPathRooted(configLocalPath))
        //    {
        //        string exePath = zapp.GetEntryAssemblyFilename();
        //        if (exePath != null)
        //            configLocalPath = zPath.Combine(zPath.GetDirectoryName(exePath), configLocalPath);
        //    }
        //    _configLocalFile = configLocalPath;
        //}

        //private static string GetDefaultConfigFile()
        //{
        //    string file = zapp.GetEntryAssemblyFilename();
        //    if (file == null)
        //        return null;
        //    //return zapp.GetPathFichier(sPath + "_config.xml");
        //    return zapp.GetPathFichier(file + _defaultSuffixConfigName);
        //}

        //private string GetDefaultConfigLocalPath()
        //{
        //    if (_configFile == null)
        //        return null;
        //    //return zpath.PathSetFileName(_configPath, zpath.PathGetFileName(_configPath) + _defaultSuffixLocalConfigName);
        //    return zpath.PathSetFileNameWithExtension(_configFile, zpath.PathGetFileName(_configFile) + _defaultSuffixLocalConfigName);
        //}

        //private void RootDirInit()
        //{
        //    string s;

        //    s = Get("RootDir", ".");
        //    if (!zPath.IsPathRooted(s))
        //    {
        //        s = zapp.GetAppDirectory() + "\\" + s;
        //        s = zPath.GetFullPath(s);
        //    }
        //    if (s[s.Length - 1] != '\\') s += "\\";
        //    if (!Directory.Exists(s))
        //        Directory.CreateDirectory(s);
        //    _rootDir = s;
        //}

        // used by CompilerProject
        public XmlConfigElement GetConfigElement(string xpath)
        {
            XElement xeLocal = null;
            if (_localXDocument != null)
                xeLocal = _localXDocument.Root.zXPathElement(xpath);
            XElement xe = _xdocument.Root.zXPathElement(xpath);
            if (xe == null && xeLocal == null)
                return null;
            return new XmlConfigElement(this, xpath, xe, xeLocal);
        }

        public XmlConfigElement GetConfigElementExplicit(string xpath)
        {
            XmlConfigElement configElement = GetConfigElement(xpath);
            if (configElement == null)
                throw new PBException("element \"{0}\" dont exist in \"{1}\" and in \"{2}\"", xpath, _configFile, _configLocalFile);
            return configElement;
        }

        //public string GetRootSubDir(string sXPath, string sDefaut)
        //{
        //    return GetRootSubDir(sXPath, sDefaut, false);
        //}

        //public string GetRootSubDir(string sXPath, string sDefaut, bool bCreateDir)
        //{
        //    return GetSubDir(sXPath, sDefaut, bCreateDir, _rootDir);
        //}

        //public string GetSubDir(string sXPath, string sDefaut, bool bCreateDir, string sRootDir)
        //{
        //    string s = Get(sXPath, sDefaut);
        //    if (s != "" && s != null)
        //    {
        //        if (!zPath.IsPathRooted(s))
        //        {
        //            s = sRootDir + s;
        //            s = zPath.GetFullPath(s);
        //        }
        //        if (s[s.Length - 1] != '\\') s += "\\";
        //        if (bCreateDir && !Directory.Exists(s))
        //            Directory.CreateDirectory(s);
        //    }
        //    return s;
        //}

        //public string GetRootSubPath(string sXPath, string sDefaut)
        //{
        //    return GetSubPath(sXPath, sDefaut, _rootDir);
        //}

        //public string GetSubPath(string sXPath, string sDefaut, string sRootDir)
        //{
        //    string s = Get(sXPath, sDefaut);
        //    if (s != "" && s != null)
        //    {
        //        if (!zPath.IsPathRooted(s))
        //        {
        //            s = sRootDir + s;
        //            s = zPath.GetFullPath(s);
        //        }
        //    }
        //    return s;
        //}

        private void SetAllVariablesValues()
        {
            //foreach (XElement xe in _xdocument.Root.DescendantsAndSelf())
            //{
            //    foreach (XAttribute xa in xe.Attributes())
            //        xa.Value = xe.zGetVariableValue(xa.Value, GetConstantValue);
            //}
            //foreach (XElement xe in _localXDocument.Root.DescendantsAndSelf())
            //{
            //    foreach (XAttribute xa in xe.Attributes())
            //        xa.Value = xe.zGetVariableValue(xa.Value, GetConstantValue);
            //}
            _xdocument.zSetAllVariablesValues(GetConstantValue);
            _localXDocument.zSetAllVariablesValues(GetConstantValue);
        }

        private static string GetConstantValue(string name)
        {
            switch (name.ToLowerInvariant())
            {
                case "appdirectory":
                    return zapp.GetAppDirectory();
                default:
                    return null;
            }
        }
    }

    public static class XmlConfigExtension
    {
        public static XmlConfigElement zGetConfigElement(this XmlConfig config, string xpath)
        {
            if (config != null)
                return config.GetConfigElement(xpath);
            else
                return null;
        }

        public static XmlConfigElement zGetConfigElementExplicit(this XmlConfig config, string xpath)
        {
            if (config != null)
                return config.GetConfigElementExplicit(xpath);
            else
                return null;
        }
    }

    public class XmlConfigFile
    {
        public XmlConfig Config;
        public DateTime FileTime;
    }

    // todo : move XmlConfigElement.GetConfig() in new static class XmlConfigFiles
    public class XmlConfigElement
    {
        protected XmlConfig _xmlConfig;
        protected string _xpathElement;
        protected XElement _element;
        protected XElement _localElement;
        protected Dictionary<string, XmlConfigFile> _configFiles = new Dictionary<string, XmlConfigFile>();

        public XmlConfigElement()
        {
        }

        public XmlConfigElement(XmlConfig config, string xpath, XElement element, XElement elementLocal)
        {
            _xpathElement = xpath;
            _xmlConfig = config;
            _element = element;
            _localElement = elementLocal;
        }

        public XmlConfig XmlConfig
        {
            get { return _xmlConfig; }
        }

        public string XPathElement
        {
            get { return _xpathElement; }
        }

        public XElement Element
        {
            get { return _element; }
        }

        public XElement LocalElement
        {
            get { return _localElement; }
        }

        public string Get(string xpath)
        {
            return Get(xpath, null);
        }

        public string Get(string xpath, string defaultValue)
        {
            string value = null;
            if (_localElement != null)
                value = _localElement.zXPathValue(xpath);
            if (value == null)
                value = _element.zXPathValue(xpath, defaultValue);
            return value;
        }

        public string GetExplicit(string xpath)
        {
            string stringValue = Get(xpath);
            if (stringValue == null)
                throw new PBException("missing value for item \"{0}\" + \"{1}\" in configuration file \"{2}\"", _xpathElement, xpath, _xmlConfig.ConfigFile);
            return stringValue;
        }

        //public string[] GetValues(string xpath)
        //{
        //    string[] values = _element.zXPathValues(xpath).ToArray();
        //    if (_localElement != null)
        //    {
        //        string[] localValues = _localElement.zXPathValues(xpath).ToArray();
        //        string[] values2 = new string[localValues.Length + values.Length];
        //        localValues.CopyTo(values2, 0);
        //        values.CopyTo(values2, localValues.Length);
        //        values = values2;
        //    }
        //    return values;
        //}

        public IEnumerable<string> GetValues(string xpath)
        {
            return _localElement.zXPathValues(xpath).Concat(_element.zXPathValues(xpath));
        }

        public XElement GetElement(string xpath)
        {
            if (_localElement != null)
            {
                XElement xe = _localElement.XPathSelectElement(xpath);
                if (xe != null) return xe;
            }
            return _element.XPathSelectElement(xpath);
        }

        public IEnumerable<XElement> GetElements(string xpath)
        {
            //if (_localElement != null)
            //    return _localElement.zXPathElements(xpath).Concat(_element.zXPathElements(xpath));
            //else
            //    return _element.zXPathElements(xpath);
            return _localElement.zXPathElements(xpath).Concat(_element.zXPathElements(xpath));
        }

        // used in WebData download.config.xml LocalConfig PrintList1Config PrintList2Config (DownloadAutomate_f.cs)
        public XmlConfig GetConfig(string xpath)
        {
            //string file = GetExplicit(xpath);
            //if (!zPath.IsPathRooted(file))
            //    file = zPath.Combine(zPath.GetDirectoryName(_xmlConfig.ConfigPath), file);
            string file = GetExplicit(xpath).zRootPath(zPath.GetDirectoryName(_xmlConfig.ConfigFile));
            if (!zFile.Exists(file))
                throw new pb.PBException("file does'nt exists \"{0}\"", file);
            XmlConfigFile configFile;
            if (!_configFiles.ContainsKey(file))
            {
                configFile = new XmlConfigFile { Config = new XmlConfig(file), FileTime = zFile.GetLastWriteTime(file) };
                _configFiles.Add(file, configFile);
            }
            else
            {
                configFile = _configFiles[file];
                DateTime fileTime = zFile.GetLastWriteTime(file);
                if (fileTime > configFile.FileTime)
                    configFile.Config = new XmlConfig(file);
            }
            return configFile.Config;
        }
    }

    #region //class SerializeUserParameter
    /********************************************************
    public class SerializeUserParameter
    {
        #region Save
        public static void Save(object oPrm, string sPrmName)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(oPrm.GetType());
                string sPath = GetPath(sPrmName);
                string sDir = zPath.GetDirectoryName(sPath);
                Directory.CreateDirectory(sDir);
                StreamWriter sw = new StreamWriter(sPath, false, Encoding.Default);
                try
                {
                    xs.Serialize(sw, oPrm);
                }
                finally
                {
                    sw.Close();
                }
            }
            catch
            {
            }
        }
        #endregion

        #region Load
        public static object Load(Type typePrm, string sPrmName)
        {
            object oPrm;

            try
            {
                XmlSerializer x = new XmlSerializer(typePrm);
                string sPath = GetPath(sPrmName);
                if (!File.Exists(sPath)) return null;
                StreamReader sr = new StreamReader(sPath, Encoding.Default);
                try
                {
                    oPrm = x.Deserialize(sr);
                }
                finally
                {
                    sr.Close();
                }
                return oPrm;
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region GetPath
        private static string GetPath(string sName)
        {
            string sPath;

            //sPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\" + Application.CompanyName
            //    + "\\" + Application.ProductName + "\\" + sName + ".xlm";
            sPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string sCompany = cu.GetApplicationCompany();
            if (sCompany != null) sPath += "\\" + sCompany;
            string sProduct = cu.GetApplicationProduct();
            if (sProduct != null) sPath += "\\" + sProduct;
            sPath += "\\" + sName + ".xlm";
            return sPath;
        }
        #endregion
    }
    ********************************************************/
    #endregion
}
