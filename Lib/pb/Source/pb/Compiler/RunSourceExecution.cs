using pb.Data.Xml;
using pb.IO;
using System.Data;
using System.Xml.Linq;

namespace pb.Compiler
{
    partial class RunSource
    {
        private static RunSource _currentRunSource = null;
        private string _projectFile = null;
        private string _projectDirectory = null;
        private XmlConfig _projectConfig = null;
        private bool _refreshProjectConfig = false;
        private bool _executionPaused = false;
        private bool _executionAborted = false;
        private DataTable gdtResult = null;
        private DataSet gdsResult = null;
        private string gsXmlResultFormat = null;
        private bool gbDisableResultEvent = false; // si true ResultEvent n'est pas appelé quand un nouveau résultat est disponible

        public event SetDataTableEvent GridResultSetDataTable;
        public event SetDataSetEvent GridResultSetDataSet;

        public static RunSource CurrentRunSource
        {
            get
            {
                if (_currentRunSource == null)
                    _currentRunSource = new RunSource();
                return _currentRunSource;
            }
            set { _currentRunSource = value; }
        }

        public string ProjectDirectory { get { return _projectDirectory; } }
        public DataTable Result { get { return gdtResult; } }
        public DataSet DataSetResult { get { return gdsResult; } }

        public XmlConfig GetProjectConfig()
        {
            if (_projectConfig == null && _projectFile != null && zFile.Exists(_projectFile))
                _projectConfig = new XmlConfig(_projectFile);
            else if (_projectConfig != null && _refreshProjectConfig)
                _projectConfig.Refresh();
            _refreshProjectConfig = false;
            return _projectConfig;
        }

        public string GetProjectVariableValue(string value, bool throwError = false)
        {
            XmlConfig projectConfig = GetProjectConfig();
            XElement root = null;
            if (projectConfig != null)
                root = projectConfig.XDocument.Root;
            string newValue;
            if (!root.zTryGetVariableValue(value, out newValue, traceError: true) && throwError)
                throw new PBException("cant get variable value from \"{0}\"", value);
            value = newValue;
            return value;
        }

        public bool IsExecutionPaused()
        {
            return _executionPaused;
        }

        public bool IsExecutionAborted()
        {
            return _executionAborted;
        }

        public void SetResult(DataTable table)
        {
            SetResult(table, null);
        }

        public void SetResult(DataTable table, string xmlFormat)
        {
            gdtResult = table;
            gsXmlResultFormat = xmlFormat;
            gdsResult = null;
            if (!gbDisableResultEvent && GridResultSetDataTable != null)
                GridResultSetDataTable(gdtResult, gsXmlResultFormat);
        }

        public void SetResult(DataSet dataSet, string xmlFormat = null)
        {
            gdsResult = dataSet;
            gsXmlResultFormat = xmlFormat;
            gdtResult = null;
            if (!gbDisableResultEvent && GridResultSetDataSet != null)
                GridResultSetDataSet(gdsResult, gsXmlResultFormat);
        }

        public string GetFilePath(string file)
        {
            return file.zRootPath(_projectDirectory);
        }
    }
}
