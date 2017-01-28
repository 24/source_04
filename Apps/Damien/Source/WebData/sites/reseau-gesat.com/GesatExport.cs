using pb;
using pb.Data.Xml;
using pb.IO;
using pb.Web.Data;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace hts.WebData.Gesat
{
    public class GesatExport
    {
        private string _xmlFile;
        private string _detailXmlFile;

        public static GesatExport Create(XElement xe)
        {
            GesatExport export = new GesatExport();
            xe = xe.Element("Export");
            if (xe == null)
                throw new PBException("Export element not found in config file");
            export._xmlFile = xe.zXPathExplicitValue("XmlFile");
            export._detailXmlFile = xe.zXPathExplicitValue("DetailXmlFile");
            //export._duplicatesFile = xe.zXPathExplicitValue("DuplicatesFile");
            //export._activitiesFile = xe.zXPathExplicitValue("ActivitiesFile");
            //export._sectorsFile = xe.zXPathExplicitValue("SectorsFile");
            //export._downloadDocumentsNameFile = xe.zXPathExplicitValue("DownloadDocumentsNameFile");
            //export._downloadDocumentsUrlFile = xe.zXPathExplicitValue("DownloadDocumentsUrlFile");
            //export._photosFile = xe.zXPathExplicitValue("PhotosFile");
            //export._unknowInfosFile = xe.zXPathExplicitValue("UnknowInfosFile");
            return export;
        }

        public void ExportXml(IEnumerable<HeaderDetail<Gesat_Header_v2, Gesat_Detail_v2>> headerDetails)
        {
            Trace.WriteLine("export Gesat");
            Trace.WriteLine($"   file        \"{_xmlFile}\"");
            Trace.WriteLine($"   file detail \"{_detailXmlFile}\"");
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;

            zfile.CreateFileDirectory(_xmlFile);
            zfile.CreateFileDirectory(_detailXmlFile);

            //Unea_Company_DuplicateExists duplicate = new Unea_Company_DuplicateExists();
            //Unea_CompanyUniqueValues uniqueValues = new Unea_CompanyUniqueValues();
            using (XmlWriter xw = XmlWriter.Create(_xmlFile, settings), xwDetail = XmlWriter.Create(_detailXmlFile, settings))
            {
                xw.WriteStartElement("Gesat");
                xwDetail.WriteStartElement("Gesat");

                foreach (HeaderDetail<Gesat_Header_v2, Gesat_Detail_v2> headerDetail in headerDetails)
                {
                    Gesat_Company company = GesatData.AggregateCompanyData(headerDetail.Header, headerDetail.Detail);
                    GesatData.AggregateDuplicateData(company);
                    ExportXml_Company(xw, company, false);
                    ExportXml_Company(xwDetail, company, true);
                    //UneaData.GetCompany_Duplicate(company, duplicate);
                    //UneaData.AddUniqueValues(uniqueValues, company);
                }

                xw.WriteEndElement();
                xwDetail.WriteEndElement();
            }

            //Export_Duplicate(duplicate);
            //ExportUniqueValues(uniqueValues);
        }

        public static void ExportXml_Company(XmlWriter xw, Gesat_Company company, bool detail)
        {
            IEnumerator<string> headerInfos = null;
            //IEnumerator<string> detailInfos = null;
            IEnumerator<string> activities = null;

            if (detail)
            {
                headerInfos = ((IEnumerable<string>)company.HeaderInfos).GetEnumerator();
                //detailInfos = ((IEnumerable<string>)company.DetailInfos).GetEnumerator();
                if (company.Activities != null)
                    activities = ((IEnumerable<string>)company.Activities).GetEnumerator();
            }

            xw.WriteStartElement("Company");
            xw.zWriteElementText("société", company.HeaderName);
            xw.zWriteElementText("société2", company.DetailName);
            xw.zWriteElementText("type", company.HeaderType);
            xw.zWriteElementText("type2", company.DetailType);
            if (detail)
            {
                if (headerInfos.MoveNext())
                    xw.zWriteElementText("info", headerInfos.Current);
                //if (detailInfos.MoveNext())
                //    xw.zWriteElementText("info", detailInfos.Current);
                if (activities != null && activities.MoveNext())
                    xw.zWriteElementText("activité", activities.Current);
            }
            xw.zWriteElementText("ville", company.City);
            xw.zWriteElementText("département", company.Department);
            xw.zWriteElementText("tel", company.HeaderPhone);
            xw.zWriteElementText("tel2", company.DetailPhone);
            xw.zWriteElementText("fax", company.Fax);
            xw.zWriteElementText("email", company.Email);
            xw.zWriteElementText("site", company.WebSite);
            xw.zWriteElementText("description", company.Description);
            xw.zWriteElementText("emplacement", company.HeaderLocation);
            xw.zWriteElementText("emplacement2", company.DetailLocation);
            xw.zWriteElementText("adresse", company.Address);
            xw.zWriteElementText("load_date", string.Format("{0:dd/MM/yyyy HH:mm}", company.LoadFromWebDate));
            xw.zWriteElementText("url", company.UrlDetail);
            xw.WriteEndElement();

            while (detail)
            {
                bool headerInfo = headerInfos.MoveNext();
                //bool detailInfo = detailInfos.MoveNext();
                bool activity = activities != null && activities.MoveNext();
                if (!headerInfo && !activity)
                    break;
                xw.WriteStartElement("Company");
                if (headerInfo)
                    xw.zWriteElementText("info", headerInfos.Current);
                //if (detailInfo)
                //    xw.zWriteElementText("info", detailInfos.Current);
                if (activity)
                    xw.zWriteElementText("activité", activities.Current);
                xw.WriteEndElement();
            }
        }
    }
}
