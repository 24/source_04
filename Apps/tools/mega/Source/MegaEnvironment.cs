using pb.Data.Xml;
using pb.IO;
using System.Xml.Linq;

namespace Mega
{
    public class MegaEnvironment
    {
        private string _file = null;
        private XDocument _document = null;

        public MegaEnvironment()
        {
            _file = XmlConfig.CurrentConfig.GetExplicit("LocalEnvironment");
            if (zFile.Exists(_file))
                _document = XDocument.Load(_file);
            else
            {
                _document = new XDocument();
                _document.Add(new XElement("MegaEnvironment"));
            }
        }

        public string GetLogin()
        {
            return _document.Root.zXPathExplicitValue("Login");
        }

        public void SetLogin(string login)
        {
            _document.Root.zSetValue("Login", login);
            _document.Save(_file);
        }

        public string GetDirectory()
        {
            return _document.Root.zXPathValue("Directory");
        }

        public void SetDirectory(string directory)
        {
            _document.Root.zSetValue("Directory", directory);
            _document.Save(_file);
        }
    }
}
