using pb;
using pb.Data.Xml;
using pb.IO;
using pb.Web.Data;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace hts.WebData.Unea
{
    public class UneaExport
    {
        private string _xmlFile;
        private string _detailXmlFile;
        private string _duplicatesFile;
        private string _activitiesFile;
        private string _sectorsFile;
        private string _downloadDocumentsNameFile;
        private string _downloadDocumentsUrlFile;
        private string _photosFile;
        private string _unknowInfosFile;

        public static UneaExport Create(XElement xe)
        {
            UneaExport export = new UneaExport();
            xe = xe.Element("Export");
            if (xe == null)
                throw new PBException("Export element not found in config file");
            export._xmlFile = xe.zXPathExplicitValue("XmlFile");
            export._detailXmlFile = xe.zXPathExplicitValue("DetailXmlFile");
            export._duplicatesFile = xe.zXPathExplicitValue("DuplicatesFile");
            export._activitiesFile = xe.zXPathExplicitValue("ActivitiesFile");
            export._sectorsFile = xe.zXPathExplicitValue("SectorsFile");
            export._downloadDocumentsNameFile = xe.zXPathExplicitValue("DownloadDocumentsNameFile");
            export._downloadDocumentsUrlFile = xe.zXPathExplicitValue("DownloadDocumentsUrlFile");
            export._photosFile = xe.zXPathExplicitValue("PhotosFile");
            export._unknowInfosFile = xe.zXPathExplicitValue("UnknowInfosFile");
            return export;
        }

        public void ExportXml(IEnumerable<HeaderDetail<Unea_Header_v2, Unea_Detail_v2>> headerDetails)
        {
            Trace.WriteLine("export Unea");
            Trace.WriteLine($"   file        \"{_xmlFile}\"");
            Trace.WriteLine($"   file detail \"{_detailXmlFile}\"");
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;

            zfile.CreateFileDirectory(_xmlFile);
            zfile.CreateFileDirectory(_detailXmlFile);

            Unea_Company_DuplicateExists duplicate = new Unea_Company_DuplicateExists();
            Unea_CompanyUniqueValues uniqueValues = new Unea_CompanyUniqueValues();
            using (XmlWriter xw = XmlWriter.Create(_xmlFile, settings), xwDetail = XmlWriter.Create(_detailXmlFile, settings))
            {
                xw.WriteStartElement("Unea");
                xwDetail.WriteStartElement("Unea");

                foreach (HeaderDetail<Unea_Header_v2, Unea_Detail_v2> headerDetail in headerDetails)
                {
                    Unea_Company company = UneaData.AggregateCompanyData(headerDetail.Header, headerDetail.Detail);
                    UneaData.AggregateDuplicateData(company);
                    ExportXml_Company(xw, company, false);
                    ExportXml_Company(xwDetail, company, true);
                    UneaData.GetCompany_Duplicate(company, duplicate);
                    UneaData.AddUniqueValues(uniqueValues, company);
                }

                xw.WriteEndElement();
                xwDetail.WriteEndElement();
            }

            Export_Duplicate(duplicate);
            ExportUniqueValues(uniqueValues);
        }

        private static void ExportXml_Company(XmlWriter xw, Unea_Company company, bool detail, Unea_Company_DuplicateExists duplicate = null)
        {
            IEnumerator<string> headerActivities = null;
            IEnumerator<string> detail1Activities = null;
            IEnumerator<string> detail2Activities = null;
            IEnumerator<string> detail1Sectors = null;
            IEnumerator<string> detail2Sectors = null;
            IEnumerator<Unea_Document> detail1DownloadDocuments = null;
            IEnumerator<Unea_Document> detail2DownloadDocuments = null;
            IEnumerator<string> detail1Photos = null;
            IEnumerator<string> detail2Photos = null;
            IEnumerator<string> headerUnknowInfos = null;
            IEnumerator<string> detail1UnknowInfos = null;
            IEnumerator<string> detail2UnknowInfos = null;

            if (detail)
            {
                //headerActivities = ((IEnumerable<string>)company.HeaderActivities.Keys).GetEnumerator();
                //detail1Activities = ((IEnumerable<string>)company.Detail1Activities.Keys).GetEnumerator();
                //detail2Activities = ((IEnumerable<string>)company.Detail2Activities.Keys).GetEnumerator();
                headerActivities = ((IEnumerable<string>)company.Activities1).GetEnumerator();
                detail1Activities = ((IEnumerable<string>)company.Activities2).GetEnumerator();
                detail2Activities = ((IEnumerable<string>)company.Activities3).GetEnumerator();
                detail1Sectors = ((IEnumerable<string>)company.Detail1Sectors.Keys).GetEnumerator();
                detail2Sectors = ((IEnumerable<string>)company.Detail2Sectors.Keys).GetEnumerator();
                detail1DownloadDocuments = ((IEnumerable<Unea_Document>)company.Detail1DownloadDocuments.Values).GetEnumerator();
                detail2DownloadDocuments = ((IEnumerable<Unea_Document>)company.Detail2DownloadDocuments.Values).GetEnumerator();
                detail1Photos = ((IEnumerable<string>)company.Detail1Photos.Keys).GetEnumerator();
                detail2Photos = ((IEnumerable<string>)company.Detail2Photos.Keys).GetEnumerator();
                headerUnknowInfos = ((IEnumerable<string>)company.HeaderUnknowInfos).GetEnumerator();
                detail1UnknowInfos = ((IEnumerable<string>)company.Detail1UnknowInfos).GetEnumerator();
                detail2UnknowInfos = ((IEnumerable<string>)company.Detail2UnknowInfos).GetEnumerator();
            }

            xw.WriteStartElement("Company");
            xw.zWriteElementText("société", company.HeaderName);
            xw.zWriteElementText("société2", company.Detail1Name);
            xw.zWriteElementText("société3", company.Detail2Name);
            xw.zWriteElementText("emplacement", company.HeaderLocation);
            xw.zWriteElementText("emplacement2", company.Detail1Location);
            if (detail)
            {
                string text1, text2;

                text1 = null;
                if (headerActivities.MoveNext())
                    text1 = headerActivities.Current;
                xw.zWriteElementText("activité", text1);

                text1 = null;
                if (detail1Activities.MoveNext())
                    text1 = detail1Activities.Current;
                xw.zWriteElementText("activité2", text1);

                text1 = null;
                if (detail2Activities.MoveNext())
                    text1 = detail2Activities.Current;
                xw.zWriteElementText("activité3", text1);

                text1 = null;
                if (detail1Sectors.MoveNext())
                    text1 = detail1Sectors.Current;
                xw.zWriteElementText("filière", text1);

                text1 = null;
                if (detail2Sectors.MoveNext())
                    text1 = detail2Sectors.Current;
                xw.zWriteElementText("filière2", text1);

                text1 = null; text2 = null;
                if (detail1DownloadDocuments.MoveNext())
                {
                    text1 = detail1DownloadDocuments.Current.Name;
                    text2 = detail1DownloadDocuments.Current.Url;
                }
                xw.zWriteElementText("document", text1);
                xw.zWriteElementText("document_url", text2);

                text1 = null; text2 = null;
                if (detail2DownloadDocuments.MoveNext())
                {
                    text1 = detail2DownloadDocuments.Current.Name;
                    text2 = detail2DownloadDocuments.Current.Url;
                }
                xw.zWriteElementText("document2", text1);
                xw.zWriteElementText("document2_url", text2);

                text1 = null;
                if (detail1Photos.MoveNext())
                    text1 = detail1Photos.Current;
                xw.zWriteElementText("image", text1);

                text1 = null;
                if (detail2Photos.MoveNext())
                    text1 = detail2Photos.Current;
                xw.zWriteElementText("image2", text1);

                text1 = null;
                if (headerUnknowInfos.MoveNext())
                    text1 = headerUnknowInfos.Current;
                xw.zWriteElementText("inconnu", text1);

                text1 = null;
                if (detail1UnknowInfos.MoveNext())
                    text1 = detail1UnknowInfos.Current;
                xw.zWriteElementText("inconnu2", text1);

                text1 = null;
                if (detail2UnknowInfos.MoveNext())
                    text1 = detail2UnknowInfos.Current;
                xw.zWriteElementText("inconnu3", text1);
            }
            xw.zWriteElementText("adresse", company.Detail1Address);
            xw.zWriteElementText("adresse2", company.Detail2Address);
            xw.zWriteElementText("tel", company.HeaderPhone);
            xw.zWriteElementText("tel2", company.Detail1Phone);
            xw.zWriteElementText("tel3", company.Detail2Phone);
            xw.zWriteElementText("fax", company.HeaderFax);
            xw.zWriteElementText("fax2", company.Detail1Fax);
            xw.zWriteElementText("fax3", company.Detail2Fax);
            xw.zWriteElementText("email", company.HeaderEmail);
            xw.zWriteElementText("email2", company.Detail1Email);
            xw.zWriteElementText("email3", company.Detail2Email);
            xw.zWriteElementText("site", company.Detail1WebSite);
            xw.zWriteElementText("site2", company.Detail2WebSite);
            xw.zWriteElementText("présentation", company.Detail1Presentation);
            xw.zWriteElementText("présentation2", company.Detail2Presentation);
            xw.zWriteElementText("client", company.Detail1Clients);
            xw.zWriteElementText("client2", company.Detail2Clients);
            xw.zWriteElementText("dirigeant", company.Detail1Leader);
            xw.zWriteElementText("dirigeant2", company.Detail2Leader);
            xw.zWriteElementText("nb_salarié", company.Detail1EmployeNumber.ToString());
            xw.zWriteElementText("nb_salarié2", company.Detail2EmployeNumber.ToString());
            xw.zWriteElementText("chiffre_affaire", company.Detail1LastYearRevenue);
            xw.zWriteElementText("chiffre_affaire2", company.Detail2LastYearRevenue);
            xw.zWriteElementText("certification", company.Detail1Certification);
            xw.zWriteElementText("certification2", company.Detail2Certification);
            xw.zWriteElementText("siret", company.Detail1Siret);
            xw.zWriteElementText("siret2", company.Detail2Siret);

            xw.zWriteElementText("url_detail1", company.UrlDetail1);
            xw.zWriteElementText("url_detail2", company.UrlDetail2);
            xw.WriteEndElement();

            while (detail)
            {
                bool headerActivity = headerActivities.MoveNext();
                bool detail1Activity = detail1Activities.MoveNext();
                bool detail2Activity = detail2Activities.MoveNext();
                bool detail1Sector = detail1Sectors.MoveNext();
                bool detail2Sector = detail2Sectors.MoveNext();
                bool detail1DownloadDocument = detail1DownloadDocuments.MoveNext();
                bool detail2DownloadDocument = detail2DownloadDocuments.MoveNext();
                bool detail1Photo = detail1Photos.MoveNext();
                bool detail2Photo = detail2Photos.MoveNext();
                bool headerUnknowInfo = headerUnknowInfos.MoveNext();
                bool detail1UnknowInfo = detail1UnknowInfos.MoveNext();
                bool detail2UnknowInfo = detail2UnknowInfos.MoveNext();

                if (!headerActivity && !detail1Activity && !detail2Activity && !detail1Sector && !detail2Sector && !detail1DownloadDocument && !detail2DownloadDocument
                    && !headerUnknowInfo && !detail1UnknowInfo && !detail2UnknowInfo)
                    break;

                xw.WriteStartElement("Company");

                if (headerActivity)
                    xw.zWriteElementText("activité", headerActivities.Current);
                if (detail1Activity)
                    xw.zWriteElementText("activité2", detail1Activities.Current);
                if (detail2Activity)
                    xw.zWriteElementText("activité3", detail2Activities.Current);
                if (detail1Sector)
                    xw.zWriteElementText("filière", detail1Sectors.Current);
                if (detail2Sector)
                    xw.zWriteElementText("filière2", detail2Sectors.Current);
                if (detail1DownloadDocument)
                {
                    xw.zWriteElementText("document", detail1DownloadDocuments.Current.Name);
                    xw.zWriteElementText("document_url", detail1DownloadDocuments.Current.Url);
                }
                if (detail2DownloadDocument)
                {
                    xw.zWriteElementText("document2", detail2DownloadDocuments.Current.Name);
                    xw.zWriteElementText("document2_url", detail2DownloadDocuments.Current.Url);
                }
                if (detail1Photo)
                    xw.zWriteElementText("image", detail1Photos.Current);
                if (detail2Photo)
                    xw.zWriteElementText("image2", detail2Photos.Current);
                if (headerUnknowInfo)
                    xw.zWriteElementText("inconnu", headerUnknowInfos.Current);
                if (detail1UnknowInfo)
                    xw.zWriteElementText("inconnu2", detail1UnknowInfos.Current);
                if (detail2UnknowInfo)
                    xw.zWriteElementText("inconnu3", detail2UnknowInfos.Current);

                xw.WriteEndElement();
            }
        }

        private void Export_Duplicate(Unea_Company_DuplicateExists duplicate)
        {
            //string file = zPath.Combine(dir, "Unea_duplicate.txt");
            using (StreamWriter sw = new StreamWriter(new FileStream(_duplicatesFile, FileMode.Create, FileAccess.Write, FileShare.Read), Encoding.UTF8))
            {
                sw.WriteLine("société2                  : {0}", duplicate.Detail1Name ? "contient des valeurs" : "");
                sw.WriteLine("société3                  : {0}", duplicate.Detail2Name ? "contient des valeurs" : "");
                sw.WriteLine("emplacement2              : {0}", duplicate.Detail1Location ? "contient des valeurs" : "");
                sw.WriteLine("adresse2                  : {0}", duplicate.Detail2Address ? "contient des valeurs" : "");
                sw.WriteLine("tel2                      : {0}", duplicate.Detail1Phone ? "contient des valeurs" : "");
                sw.WriteLine("tel3                      : {0}", duplicate.Detail2Phone ? "contient des valeurs" : "");
                sw.WriteLine("fax2                      : {0}", duplicate.Detail1Fax ? "contient des valeurs" : "");
                sw.WriteLine("fax3                      : {0}", duplicate.Detail2Fax ? "contient des valeurs" : "");
                sw.WriteLine("email2                    : {0}", duplicate.Detail1Email ? "contient des valeurs" : "");
                sw.WriteLine("email3                    : {0}", duplicate.Detail2Email ? "contient des valeurs" : "");
                sw.WriteLine("site2                     : {0}", duplicate.Detail2WebSite ? "contient des valeurs" : "");
                sw.WriteLine("présentation2             : {0}", duplicate.Detail2Presentation ? "contient des valeurs" : "");
                sw.WriteLine("client2                   : {0}", duplicate.Detail2Clients ? "contient des valeurs" : "");
                sw.WriteLine("dirigeant2                : {0}", duplicate.Detail2Leader ? "contient des valeurs" : "");
                sw.WriteLine("nb_salarié2               : {0}", duplicate.Detail2EmployeNumber ? "contient des valeurs" : "");
                sw.WriteLine("chiffre_affaire2          : {0}", duplicate.Detail2LastYearRevenue ? "contient des valeurs" : "");
                sw.WriteLine("certification2            : {0}", duplicate.Detail2Certification ? "contient des valeurs" : "");
                sw.WriteLine("siret2                    : {0}", duplicate.Detail2Siret ? "contient des valeurs" : "");

                sw.WriteLine("activité2                 : {0}", duplicate.Detail1Activities ? "contient des valeurs" : "");
                sw.WriteLine("activité3                 : {0}", duplicate.Detail2Activities ? "contient des valeurs" : "");

                sw.WriteLine("filière2                  : {0}", duplicate.Detail2Sectors ? "contient des valeurs" : "");

                sw.WriteLine("document2                 : {0}", duplicate.Detail2DownloadDocuments ? "contient des valeurs" : "");

                sw.WriteLine("image2                    : {0}", duplicate.Detail2Photos ? "contient des valeurs" : "");

                sw.WriteLine("inconnu                   : {0}", duplicate.HeaderUnknowInfos ? "contient des valeurs" : "");
                sw.WriteLine("inconnu2                  : {0}", duplicate.Detail1UnknowInfos ? "contient des valeurs" : "");
                sw.WriteLine("inconnu3                  : {0}", duplicate.Detail2UnknowInfos ? "contient des valeurs" : "");
            }
        }

        private void ExportUniqueValues(Unea_CompanyUniqueValues uniqueValues)
        {
            //zfile.CreateFileDirectory(_activitiesFile);
            //ExportValues(_activitiesFile, uniqueValues.Activities.Keys);
            zfile.CreateFileDirectory(_sectorsFile);
            ExportValues(_sectorsFile, uniqueValues.Sectors.Keys);
            zfile.CreateFileDirectory(_downloadDocumentsNameFile);
            ExportValues(_downloadDocumentsNameFile, uniqueValues.DownloadDocumentsName.Keys);
            zfile.CreateFileDirectory(_downloadDocumentsUrlFile);
            ExportValues(_downloadDocumentsUrlFile, uniqueValues.DownloadDocumentsUrl.Keys);
            zfile.CreateFileDirectory(_photosFile);
            ExportValues(_photosFile, uniqueValues.Photos.Keys);
            zfile.CreateFileDirectory(_unknowInfosFile);
            ExportValues(_unknowInfosFile, uniqueValues.UnknowInfos.Keys);
        }

        private static void ExportValues(string file, IEnumerable<string> values)
        {
            using (StreamWriter sw = new StreamWriter(new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Read), Encoding.UTF8))
            {
                foreach (string value in values)
                    sw.WriteLine(value);
            }
        }
    }
}
