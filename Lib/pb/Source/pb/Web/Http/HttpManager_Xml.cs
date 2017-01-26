using System.Xml.Linq;

namespace pb.Web.Http
{
    public partial class HttpManager
    {
        public XDocument GetXDocument(Http http)
        {
            XDocument xml = http.zGetXDocument();
            if (_exportResult && _exportDirectory != null)
                xml.Save(GetNewHttpFileName(http.HttpRequest, ".xml"));
            return xml;
        }
    }
}
