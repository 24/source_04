﻿using System.IO;
using System.Xml.Linq;

namespace pb.Web
{
    public static partial class GlobalExtension
    {
        public static XDocument zGetXDocument(this Http http)
        {
            XDocument xml = null;
            if (http.ResultContentType == "text/html")
            {
                HtmlToXml hx = new HtmlToXml(new StringReader(http.ResultText));
                //hx.ReadCommentInText = _readCommentInText;
                xml = hx.GenerateXDocument();
            }
            else if (http.ResultContentType == "text/xml")
            {
                xml = XDocument.Parse(http.ResultText, LoadOptions.PreserveWhitespace);
            }
            else
                throw new PBException("Error can't transform http content \"{0}\" to xml", http.ResultContentType);

            //if (http.ExportResult && http.ExportDirectory != null)
            //{
            //    string xmlExportPath = http.GetNewHttpFileName(http.ExportDirectory, ".xml");
            //    xml.Save(xmlExportPath);
            //}
            return xml;
        }
    }
}
