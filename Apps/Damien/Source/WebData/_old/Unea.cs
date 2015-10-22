using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using pb.Data.Xml;
using PB_Util;

// lecture de
//   http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire-unea.htm?idRechAnnuaire=Entrez%20le%20nom%20d%27une%20entreprise
//   http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/annuaire.asp?txtRecherche=Entrez%20le%20nom%20d'une%20entreprise&hiddenValider=true
//   req region :
//     method post    
//     url http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/annuaire.asp
//     Cookie __atuvc=1%7C4; PHPSESSID=t85siq9oqn4sqmi0eg09schbh0; ASPSESSIONIDQABATCST=BDBNKJFAHJNFHLOCOJGCGOOF; __utma=169855717.692259951.1390462116.1391519633.1391522222.4; __utmc=169855717; __utmz=169855717.1390462116.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none)
//     Referer http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/annuaire.asp?txtRecherche=Entrez%20le%20nom%20d'une%20entreprise&hiddenValider=true
//     postData hiddenValider=true&txtRecherche=Par+Nom+Entreprise&txtRecherche1=&txtRecherche2=1&txtRecherche3=&txtRecherche4=
//     txtRecherche2=1 ==> 1 = Alsace
//   http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/detail.asp?id=4583
//   http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/4583/ACCAA%20TAKTIM.htm
//   http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/4017/ALSACE%20ENTREPRISE%20ADAPTEE.htm

// to do :
//   ok : warning sector already exists "TRAVAUX PAYSAGERS" : VIDEAL AGK 30
//      http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/detail.asp?id=4678
//      http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/4678/VIDEAL%20AGK%2030.htm
//   Photos
//      EA EQUILIBRE
//      http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/detail.asp?id=3716
//      http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/3716/EA%20EQUILIBRE.htm
//   activité : à vérifier
//      http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/detail.asp?id=3710
//      http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/3710/ANR%20SERVICES%20EA%20LANNION.htm
//   photos dans les documents
//      ACTIV ADIS
//      http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/detail.asp?id=3982
//      http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/3982/ACTIV%20ADIS.htm

namespace Download.Unea
{
    public enum Unea_TextType
    {
        unknow = 0,
        novalue = 1,
        novalues,
        activity,
        location,
        phone,
        fax,
        email,
        sector,
        presentation,
        client,
        leader,
        employeNumber,
        lastYearRevenue,
        certification,
        siret,
        //downloadDocument,
        address,
        webSite
    }

    public class Unea_Document
    {
        public string name = null;
        public string url = null;
    }

    public class Unea_Company_DuplicateExists
    {
        //public bool detail1Name = false;

        //public string headerName = null;
        public bool detail1Name = false;
        public bool detail2Name = false;
        //public string headerLocation = null;
        public bool detail1Location = false;
        //public string detail1Address = null;
        public bool detail2Address = false;
        //public string headerPhone = null;
        public bool detail1Phone = false;
        public bool detail2Phone = false;
        //public string headerFax = null;
        public bool detail1Fax = false;
        public bool detail2Fax = false;
        //public string headerEmail = null;
        public bool detail1Email = false;
        public bool detail2Email = false;
        //public string detail1WebSite = null;
        public bool detail2WebSite = false;
        //public string detail1Presentation = null;
        public bool detail2Presentation = false;
        //public string detail1Clients = null;
        public bool detail2Clients = false;
        //public string detail1Leader = null; // dirigeant
        public bool detail2Leader = false; // falset
        //public int? detail1EmployeNumber = null; // nombre de salarié
        public bool detail2EmployeNumber = false; // nombre de falseé
        //public string detail1LastYearRevenue = null;  // chiffre d'affaire de l'année écoulée
        public bool detail2LastYearRevenue = false;  // chiffre d'affaire de l'année falsee
        //public string detail1Certification = null; // certification
        public bool detail2Certification = false; // falsen
        //public string detail1Siret = null;
        public bool detail2Siret = false;
        //public SortedDictionary<string, string> headerActivities = new SortedDictionary<string, string>();
        public bool detail1Activities = false;
        public bool detail2Activities = false;
        //public SortedDictionary<string, string> detail1Sectors = new SortedDictionary<string, string>();  // Filières Métiers UNEA
        public bool detail2Sectors = false;
        //public SortedDictionary<string, Unea_Document> detail1DownloadDocuments = new SortedDictionary<string, Unea_Document>();
        public bool detail2DownloadDocuments = false;
        //public SortedDictionary<string, string> detail1Photos = new SortedDictionary<string, string>();
        public bool detail2Photos = false;
        public bool headerUnknowInfos = false;
        public bool detail1UnknowInfos = false;
        public bool detail2UnknowInfos = false;
    }

    public class Unea_Company
    {
        //public Unea_HeaderCompany header;
        //public Unea_DetailCompany1 detail1;
        //public Unea_DetailCompany2 detail2;
        public DateTime? loadFromWebDate = null;
        public string urlHeader = null;
        public string urlDetail1 = null;
        public string urlDetail2 = null;
        public string headerName = null;
        public string detail1Name = null;
        public string detail2Name = null;
        public string headerLocation = null;
        public string detail1Location = null;
        public string detail1Address = null;
        public string detail2Address = null;
        public string headerPhone = null;
        public string detail1Phone = null;
        public string detail2Phone = null;
        public string headerFax = null;
        public string detail1Fax = null;
        public string detail2Fax = null;
        public string headerEmail = null;
        public string detail1Email = null;
        public string detail2Email = null;
        public string detail1WebSite = null;
        public string detail2WebSite = null;
        public string detail1Presentation = null;
        public string detail2Presentation = null;
        public string detail1Clients = null;
        public string detail2Clients = null;
        public string detail1Leader = null; // dirigeant
        public string detail2Leader = null; // dirigeant
        public int? detail1EmployeNumber = null; // nombre de salarié
        public int? detail2EmployeNumber = null; // nombre de salarié
        public string detail1LastYearRevenue = null;  // chiffre d'affaire de l'année écoulée
        public string detail2LastYearRevenue = null;  // chiffre d'affaire de l'année écoulée
        public string detail1Certification = null; // certification
        public string detail2Certification = null; // certification
        public string detail1Siret = null;
        public string detail2Siret = null;
        public SortedDictionary<string, string> headerActivities = new SortedDictionary<string, string>();
        public SortedDictionary<string, string> detail1Activities = new SortedDictionary<string, string>();
        public SortedDictionary<string, string> detail2Activities = new SortedDictionary<string, string>();
        public SortedDictionary<string, string> detail1Sectors = new SortedDictionary<string, string>();  // Filières Métiers UNEA
        public SortedDictionary<string, string> detail2Sectors = new SortedDictionary<string, string>();  // Filières Métiers UNEA
        public SortedDictionary<string, Unea_Document> detail1DownloadDocuments = new SortedDictionary<string, Unea_Document>();
        public SortedDictionary<string, Unea_Document> detail2DownloadDocuments = new SortedDictionary<string, Unea_Document>();
        public SortedDictionary<string, string> detail1Photos = new SortedDictionary<string, string>();
        public SortedDictionary<string, string> detail2Photos = new SortedDictionary<string, string>();
        public List<string> headerUnknowInfos = new List<string>();
        public List<string> detail1UnknowInfos = new List<string>();
        public List<string> detail2UnknowInfos = new List<string>();
    }

    public class Unea_CompanyUniqueValues
    {
        public SortedDictionary<string, string> activities = new SortedDictionary<string, string>();
        public SortedDictionary<string, string> sectors = new SortedDictionary<string, string>();
        public SortedDictionary<string, string> downloadDocumentsUrl = new SortedDictionary<string, string>();
        public SortedDictionary<string, string> downloadDocumentsName = new SortedDictionary<string, string>();
        public SortedDictionary<string, string> photos = new SortedDictionary<string, string>();
        public SortedDictionary<string, string> unknowInfos = new SortedDictionary<string, string>();
    }

    public static class Unea
    {
        private static string __xmlCompanyListFile;
        private static string __xmlDetailCompanyListFile;

        public static void Init()
        {
            Unea_LoadHeaderFromWeb.ClassInit(XmlConfig.CurrentConfig.GetElement("Unea/Header"));
            Unea_LoadHeader.ClassInit(XmlConfig.CurrentConfig.GetElement("Unea/Header"));
            Unea_LoadDetailCompany1FromWeb.ClassInit(XmlConfig.CurrentConfig.GetElement("Unea/DetailCompany1"));
            Unea_LoadDetailCompany1.ClassInit(XmlConfig.CurrentConfig.GetElement("Unea/DetailCompany1"));
            Unea_LoadDetailCompany2FromWeb.ClassInit(XmlConfig.CurrentConfig.GetElement("Unea/DetailCompany2"));
            Unea_LoadDetailCompany2.ClassInit(XmlConfig.CurrentConfig.GetElement("Unea/DetailCompany2"));
            __xmlCompanyListFile = XmlConfig.CurrentConfig.GetExplicit("Unea/Xml/XmlCompanyListFile");
            __xmlDetailCompanyListFile = XmlConfig.CurrentConfig.GetExplicit("Unea/Xml/XmlDetailCompanyListFile");
        }

        public static Unea_Company LoadDetailCompany(Unea_HeaderCompany header, bool reload = false, bool loadImage = false)
        {
            Unea_DetailCompany1 detail1 = Unea_LoadDetailCompany1.LoadCompany(header.urlDetail1, reload, loadImage);
            Unea_DetailCompany2 detail2 = Unea_LoadDetailCompany2.LoadCompany(header.urlDetail2, reload, loadImage);
            Unea_Company company = AggregateCompanyData(header, detail1, detail2);
            AggregateDuplicateData(company);
            return company;
        }

        public static Unea_Company AggregateCompanyData(Unea_HeaderCompany header, Unea_DetailCompany1 detail1, Unea_DetailCompany2 detail2)
        {
            Unea_Company company = new Unea_Company();

            company.urlHeader = header.sourceUrl;
            company.loadFromWebDate = header.loadFromWebDate;
            company.urlDetail1 = header.urlDetail1;
            company.urlDetail2 = header.urlDetail2;
            company.headerName = header.name;
            company.headerLocation = header.location;
            company.headerPhone = header.phone;
            company.headerFax = header.fax;
            company.headerEmail = header.email;
            company.headerActivities = header.activities;
            company.headerUnknowInfos = header.unknowInfos;

            company.detail1Name = detail1.name;
            company.detail1Location = detail1.location;
            company.detail1Activities = detail1.activities;
            company.detail1Sectors = detail1.sectors;
            company.detail1Presentation = detail1.presentation;
            company.detail1Clients = detail1.clients;
            company.detail1Leader = detail1.leader;
            company.detail1EmployeNumber = detail1.employeNumber;
            company.detail1LastYearRevenue = detail1.lastYearRevenue;
            company.detail1Certification = detail1.certification;
            company.detail1Siret = detail1.siret;
            company.detail1Photos = detail1.photos;
            company.detail1DownloadDocuments = detail1.downloadDocuments;
            company.detail1Address = detail1.address;
            company.detail1Phone = detail1.phone;
            company.detail1Fax = detail1.fax;
            company.detail1Email = detail1.email;
            company.detail1WebSite = detail1.webSite;
            company.detail1UnknowInfos = detail1.unknowInfos;

            company.detail2Name = detail2.name;
            company.detail2Presentation = detail2.presentation;
            company.detail2Activities = detail2.activities;
            company.detail2Sectors = detail2.sectors;
            company.detail2DownloadDocuments = detail2.downloadDocuments;
            company.detail2Address = detail2.address;
            company.detail2Phone = detail2.phone;
            company.detail2Fax = detail2.fax;
            company.detail2Email = detail2.email;
            company.detail2WebSite = detail2.webSite;
            company.detail2Leader = detail2.leader;
            company.detail2EmployeNumber = detail2.employeNumber;
            company.detail2LastYearRevenue = detail2.lastYearRevenue;
            company.detail2Siret = detail2.siret;
            company.detail2Certification = detail2.certification;
            company.detail2Clients = detail2.clients;
            company.detail2UnknowInfos = detail2.unknowInfos;

            return company;
        }

        public static void AggregateDuplicateData(Unea_Company company)
        {
            //RemoveDuplicate(ref company.detail1Name, company.headerName);
            //RemoveDuplicate(ref company.detail2Name, company.headerName, company.detail1Name);
            AggregateDuplicateData(ref company.headerName, ref company.detail1Name, ref company.detail2Name);
            //RemoveDuplicate(ref company.detail1Location, company.headerLocation);
            AggregateDuplicateData(ref company.headerLocation, ref company.detail1Location);
            //RemoveDuplicate(ref company.detail2Address, company.detail1Address);
            AggregateDuplicateData(ref company.detail1Address, ref company.detail2Address);
            //RemoveDuplicate(ref company.detail1Phone, company.headerPhone);
            //RemoveDuplicate(ref company.detail2Phone, company.headerPhone, company.detail1Phone);
            AggregateDuplicateData(ref company.headerPhone, ref company.detail1Phone, ref company.detail2Phone);
            //RemoveDuplicate(ref company.detail1Fax, company.headerFax);
            //RemoveDuplicate(ref company.detail2Fax, company.headerFax, company.detail1Fax);
            AggregateDuplicateData(ref company.headerFax, ref company.detail1Fax, ref company.detail2Fax);
            //RemoveDuplicate(ref company.detail1Email, company.headerEmail);
            //RemoveDuplicate(ref company.detail2Email, company.headerEmail, company.detail1Email);
            AggregateDuplicateData(ref company.headerEmail, ref company.detail1Email, ref company.detail2Email);
            //RemoveDuplicate(ref company.detail2WebSite, company.detail1WebSite);
            AggregateDuplicateData(ref company.detail1WebSite, ref company.detail2WebSite);
            //RemoveDuplicate(ref company.detail2Presentation, company.detail1Presentation);
            AggregateDuplicateData(ref company.detail1Presentation, ref company.detail2Presentation);
            //RemoveDuplicate(ref company.detail2Clients, company.detail1Clients);
            AggregateDuplicateData(ref company.detail1Clients, ref company.detail2Clients);
            //RemoveDuplicate(ref company.detail2Leader, company.detail1Leader);
            AggregateDuplicateData(ref company.detail1Leader, ref company.detail2Leader);
            //RemoveDuplicate(ref company.detail2EmployeNumber, company.detail1EmployeNumber);
            AggregateDuplicateData(ref company.detail1EmployeNumber, ref company.detail2EmployeNumber);
            //RemoveDuplicate(ref company.detail2LastYearRevenue, company.detail1LastYearRevenue);
            AggregateDuplicateData(ref company.detail1LastYearRevenue, ref company.detail2LastYearRevenue);
            //RemoveDuplicate(ref company.detail2Certification, company.detail1Certification);
            AggregateDuplicateData(ref company.detail1Certification, ref company.detail2Certification);
            //RemoveDuplicate(ref company.detail2Siret, company.detail1Siret);
            AggregateDuplicateData(ref company.detail1Siret, ref company.detail2Siret);
            AggregateDuplicateData(company.detail1Activities, company.headerActivities);
            AggregateDuplicateData(company.detail2Activities, company.headerActivities, company.detail1Activities);
            AggregateDuplicateData(company.detail2Sectors, company.detail1Sectors);
            AggregateDuplicateData(company.detail2DownloadDocuments, company.detail1DownloadDocuments);
            AggregateDuplicateData(company.detail2Photos, company.detail1Photos);
        }

        private static void AggregateDuplicateData(ref string text1, ref string text2)
        {
            if (text1 == null)
            {
                if (text2 != null)
                {
                    text1 = text2;
                    text2 = null;
                }
            }
            else if ((text2 != null && string.Equals(text1, text2, StringComparison.InvariantCultureIgnoreCase)))
                text2 = null;
        }

        private static void AggregateDuplicateData(ref string text1, ref string text2, ref string text3)
        {
            AggregateDuplicateData(ref text1, ref text2);
            AggregateDuplicateData(ref text1, ref text3);
            AggregateDuplicateData(ref text2, ref text3);
        }

        private static void AggregateDuplicateData(ref int? value1, ref int? value2)
        {
            if (value1 == null)
            {
                if (value2 != null)
                {
                    value1 = value2;
                    value2 = null;
                }
            }
            else if (value2 != null && value1 == value2)
                value2 = null;
        }

        //private static void RemoveDuplicate(ref string text1, string text2, string text3 = null)
        //{
        //    if ((text2 != null && string.Equals(text1, text2, StringComparison.InvariantCultureIgnoreCase))
        //        || (text3 != null && string.Equals(text1, text3, StringComparison.InvariantCultureIgnoreCase)))
        //        text1 = null;
        //}

        //private static void RemoveDuplicate(ref int? value1, int? value2)
        //{
        //    if (value1 == value2)
        //        value1 = null;
        //}

        private static void AggregateDuplicateData(SortedDictionary<string, string> activities1, SortedDictionary<string, string> activities2, SortedDictionary<string, string> activities3 = null)
        {
            foreach (string key in activities1.Keys.ToArray())
            {
                if (activities2.ContainsKey(key) || (activities3 != null && activities3.ContainsKey(key)))
                    activities1.Remove(key);
            }
        }

        private static void AggregateDuplicateData(SortedDictionary<string, Unea_Document> downloadDocuments1, SortedDictionary<string, Unea_Document> downloadDocuments2)
        {
            foreach (string key in downloadDocuments1.Keys.ToArray())
            {
                if (downloadDocuments2.ContainsKey(key) && downloadDocuments1[key].name == downloadDocuments2[key].name)
                    downloadDocuments1.Remove(key);
            }
        }

        public static IEnumerable<Unea_Company> LoadDetailCompanyList(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false)
        {
            return from header in new Unea_LoadHeaderPages(startPage, maxPage, reload, loadImage) select Unea.LoadDetailCompany(header, reload, loadImage);
        }

        public static void ExportXmlCompanyList(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false)
        {
            //WriteLine("ExportXmlCompanyList : startPage {0} maxPage {1} reload {2} loadImage {3}", startPage, maxPage, reload, loadImage);
            Init();
            //_ExportXmlCompanyList(Unea.LoadDetailCompanyList(startPage, maxPage, reload, loadImage));

            //string dir = @"c:\pib\dev_data\exe\wrun\damien\export";
            //string file = dir + @"\Unea.xml";
            //string fileDetail = dir + @"\UneaDetail.xml";
            WriteLine("export Unea");
            WriteLine("   file        \"{0}\"", __xmlCompanyListFile);
            WriteLine("   file detail \"{0}\"", __xmlDetailCompanyListFile);
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;

            Unea_Company_DuplicateExists duplicate = new Unea_Company_DuplicateExists();
            Unea_CompanyUniqueValues uniqueValues = new Unea_CompanyUniqueValues();
            using (XmlWriter xw = XmlWriter.Create(__xmlCompanyListFile, settings), xwDetail = XmlWriter.Create(__xmlDetailCompanyListFile, settings))
            {
                xw.WriteStartElement("Unea");
                xwDetail.WriteStartElement("Unea");
                Unea.LoadDetailCompanyList(startPage, maxPage, reload, loadImage).zForEach(company =>
                    {
                        ExportXml_Company(xw, company, false);
                        ExportXml_Company(xwDetail, company, true);
                        GetCompany_Duplicate(company, duplicate);
                        AddUniqueValues(uniqueValues, company);
                    });
                xw.WriteEndElement();
                xwDetail.WriteEndElement();
            }

            string dir = Path.GetDirectoryName(__xmlCompanyListFile);
            Export_Duplicate(duplicate, dir);
            ExportUniqueValues(uniqueValues, dir);
        }

        private static void AddUniqueValues(Unea_CompanyUniqueValues uniqueValues, Unea_Company company)
        {
            AddUniqueValues(uniqueValues.activities, company.headerActivities.Keys);
            AddUniqueValues(uniqueValues.activities, company.detail1Activities.Keys);
            AddUniqueValues(uniqueValues.activities, company.detail2Activities.Keys);
            AddUniqueValues(uniqueValues.sectors, company.detail1Sectors.Keys);
            AddUniqueValues(uniqueValues.sectors, company.detail2Sectors.Keys);
            AddUniqueValues(uniqueValues.downloadDocumentsName, from doc in company.detail1DownloadDocuments select doc.Value.name);
            AddUniqueValues(uniqueValues.downloadDocumentsUrl, from doc in company.detail1DownloadDocuments select doc.Value.url);
            AddUniqueValues(uniqueValues.downloadDocumentsName, from doc in company.detail2DownloadDocuments select doc.Value.name);
            AddUniqueValues(uniqueValues.downloadDocumentsUrl, from doc in company.detail2DownloadDocuments select doc.Value.url);
            AddUniqueValues(uniqueValues.photos, company.detail1Photos.Keys);
            AddUniqueValues(uniqueValues.photos, company.detail2Photos.Keys);
            AddUniqueValues(uniqueValues.unknowInfos, company.headerUnknowInfos);
            AddUniqueValues(uniqueValues.unknowInfos, company.detail1UnknowInfos);
            AddUniqueValues(uniqueValues.unknowInfos, company.detail2UnknowInfos);
        }

        private static void AddUniqueValues(SortedDictionary<string, string> uniqueValues, IEnumerable<string> values)
        {
            foreach (string value in values)
            {
                if (value != null && !uniqueValues.ContainsKey(value))
                    uniqueValues.Add(value, null);
            }
        }

        private static void Export_Duplicate(Unea_Company_DuplicateExists duplicate, string dir)
        {
            string file = Path.Combine(dir, "Unea_duplicate.txt");
            using (StreamWriter sw = new StreamWriter(new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Read), Encoding.UTF8))
            {
                sw.WriteLine("société2                  : {0}", duplicate.detail1Name ? "contient des valeurs" : "");
                sw.WriteLine("société3                  : {0}", duplicate.detail2Name ? "contient des valeurs" : "");
                sw.WriteLine("emplacement2              : {0}", duplicate.detail1Location ? "contient des valeurs" : "");
                sw.WriteLine("adresse2                  : {0}", duplicate.detail2Address ? "contient des valeurs" : "");
                sw.WriteLine("tel2                      : {0}", duplicate.detail1Phone ? "contient des valeurs" : "");
                sw.WriteLine("tel3                      : {0}", duplicate.detail2Phone ? "contient des valeurs" : "");
                sw.WriteLine("fax2                      : {0}", duplicate.detail1Fax ? "contient des valeurs" : "");
                sw.WriteLine("fax3                      : {0}", duplicate.detail2Fax ? "contient des valeurs" : "");
                sw.WriteLine("email2                    : {0}", duplicate.detail1Email ? "contient des valeurs" : "");
                sw.WriteLine("email3                    : {0}", duplicate.detail2Email ? "contient des valeurs" : "");
                sw.WriteLine("site2                     : {0}", duplicate.detail2WebSite ? "contient des valeurs" : "");
                sw.WriteLine("présentation2             : {0}", duplicate.detail2Presentation ? "contient des valeurs" : "");
                sw.WriteLine("client2                   : {0}", duplicate.detail2Clients ? "contient des valeurs" : "");
                sw.WriteLine("dirigeant2                : {0}", duplicate.detail2Leader ? "contient des valeurs" : "");
                sw.WriteLine("nb_salarié2               : {0}", duplicate.detail2EmployeNumber ? "contient des valeurs" : "");
                sw.WriteLine("chiffre_affaire2          : {0}", duplicate.detail2LastYearRevenue ? "contient des valeurs" : "");
                sw.WriteLine("certification2            : {0}", duplicate.detail2Certification ? "contient des valeurs" : "");
                sw.WriteLine("siret2                    : {0}", duplicate.detail2Siret ? "contient des valeurs" : "");

                sw.WriteLine("activité2                 : {0}", duplicate.detail1Activities ? "contient des valeurs" : "");
                sw.WriteLine("activité3                 : {0}", duplicate.detail2Activities ? "contient des valeurs" : "");

                sw.WriteLine("filière2                  : {0}", duplicate.detail2Sectors ? "contient des valeurs" : "");

                sw.WriteLine("document2                 : {0}", duplicate.detail2DownloadDocuments ? "contient des valeurs" : "");

                sw.WriteLine("image2                    : {0}", duplicate.detail2Photos ? "contient des valeurs" : "");

                sw.WriteLine("inconnu                   : {0}", duplicate.headerUnknowInfos ? "contient des valeurs" : "");
                sw.WriteLine("inconnu2                  : {0}", duplicate.detail1UnknowInfos ? "contient des valeurs" : "");
                sw.WriteLine("inconnu3                  : {0}", duplicate.detail2UnknowInfos ? "contient des valeurs" : "");
            }
        }

        private static void ExportUniqueValues(Unea_CompanyUniqueValues uniqueValues, string dir)
        {
            ExportValues(Path.Combine(dir, "Unea_activities.txt"), uniqueValues.activities.Keys);
            ExportValues(Path.Combine(dir, "Unea_sectors.txt"), uniqueValues.sectors.Keys);
            ExportValues(Path.Combine(dir, "Unea_downloadDocumentsName.txt"), uniqueValues.downloadDocumentsName.Keys);
            ExportValues(Path.Combine(dir, "Unea_downloadDocumentsUrl.txt"), uniqueValues.downloadDocumentsUrl.Keys);
            ExportValues(Path.Combine(dir, "Unea_photos.txt"), uniqueValues.photos.Keys);
            ExportValues(Path.Combine(dir, "Unea_unknowInfos.txt"), uniqueValues.unknowInfos.Keys);
        }

        private static void ExportValues(string file, IEnumerable<string> values)
        {
            using (StreamWriter sw = new StreamWriter(new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Read), Encoding.UTF8))
            {
                foreach (string value in values)
                    sw.WriteLine(value);
            }
        }

        public static void GetCompany_Duplicate(Unea_Company company, Unea_Company_DuplicateExists duplicate)
        {
            if (company.detail1Name != null)
                duplicate.detail1Name = true;
            if (company.detail2Name != null)
                duplicate.detail2Name = true;
            if (company.detail1Location != null)
                duplicate.detail1Location = true;
            if (company.detail2Address != null)
                duplicate.detail2Address = true;
            if (company.detail1Phone != null)
                duplicate.detail1Phone = true;
            if (company.detail2Phone != null)
                duplicate.detail2Phone = true;
            if (company.detail1Fax != null)
                duplicate.detail1Fax = true;
            if (company.detail2Fax != null)
                duplicate.detail2Fax = true;
            if (company.detail1Email != null)
                duplicate.detail1Email = true;
            if (company.detail2Email != null)
                duplicate.detail2Email = true;
            if (company.detail2WebSite != null)
                duplicate.detail2WebSite = true;
            if (company.detail2Presentation != null)
                duplicate.detail2Presentation = true;
            if (company.detail2Clients != null)
                duplicate.detail2Clients = true;
            if (company.detail2Leader != null)
                duplicate.detail2Leader = true;
            if (company.detail2EmployeNumber != null)
                duplicate.detail2EmployeNumber = true;
            if (company.detail2LastYearRevenue != null)
                duplicate.detail2LastYearRevenue = true;
            if (company.detail2Certification != null)
                duplicate.detail2Certification = true;
            if (company.detail2Siret != null)
                duplicate.detail2Siret = true;

            if (company.detail1Activities.Count > 0)
                duplicate.detail1Activities = true;
            if (company.detail2Activities.Count > 0)
                duplicate.detail2Activities = true;

            if (company.detail2Sectors.Count > 0)
                duplicate.detail2Sectors = true;

            if (company.detail2DownloadDocuments.Count > 0)
                duplicate.detail2DownloadDocuments = true;

            if (company.detail2Photos.Count > 0)
                duplicate.detail2Photos = true;

            if (company.headerUnknowInfos.Count > 0)
                duplicate.headerUnknowInfos = true;
            if (company.detail1UnknowInfos.Count > 0)
                duplicate.detail1UnknowInfos = true;
            if (company.detail2UnknowInfos.Count > 0)
                duplicate.detail2UnknowInfos = true;
        }

        public static void ExportXml_Company(XmlWriter xw, Unea_Company company, bool detail, Unea_Company_DuplicateExists duplicate = null)
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
                headerActivities = ((IEnumerable<string>)company.headerActivities.Keys).GetEnumerator();
                detail1Activities = ((IEnumerable<string>)company.detail1Activities.Keys).GetEnumerator();
                detail2Activities = ((IEnumerable<string>)company.detail2Activities.Keys).GetEnumerator();
                detail1Sectors = ((IEnumerable<string>)company.detail1Sectors.Keys).GetEnumerator();
                detail2Sectors = ((IEnumerable<string>)company.detail2Sectors.Keys).GetEnumerator();
                detail1DownloadDocuments = ((IEnumerable<Unea_Document>)company.detail1DownloadDocuments.Values).GetEnumerator();
                detail2DownloadDocuments = ((IEnumerable<Unea_Document>)company.detail2DownloadDocuments.Values).GetEnumerator();
                detail1Photos = ((IEnumerable<string>)company.detail1Photos.Keys).GetEnumerator();
                detail2Photos = ((IEnumerable<string>)company.detail2Photos.Keys).GetEnumerator();
                headerUnknowInfos = ((IEnumerable<string>)company.headerUnknowInfos).GetEnumerator();
                detail1UnknowInfos = ((IEnumerable<string>)company.detail1UnknowInfos).GetEnumerator();
                detail2UnknowInfos = ((IEnumerable<string>)company.detail2UnknowInfos).GetEnumerator();
            }

            xw.WriteStartElement("Company");
            xw.zWriteElementText("société", company.headerName);
            xw.zWriteElementText("société2", company.detail1Name);
            xw.zWriteElementText("société3", company.detail2Name);
            xw.zWriteElementText("emplacement", company.headerLocation);
            xw.zWriteElementText("emplacement2", company.detail1Location);
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
                    text1 = detail1DownloadDocuments.Current.name;
                    text2 = detail1DownloadDocuments.Current.url;
                }
                xw.zWriteElementText("document", text1);
                xw.zWriteElementText("document_url", text2);

                text1 = null; text2 = null;
                if (detail2DownloadDocuments.MoveNext())
                {
                    text1 = detail2DownloadDocuments.Current.name;
                    text2 = detail2DownloadDocuments.Current.url;
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
            xw.zWriteElementText("adresse", company.detail1Address);
            xw.zWriteElementText("adresse2", company.detail2Address);
            xw.zWriteElementText("tel", company.headerPhone);
            xw.zWriteElementText("tel2", company.detail1Phone);
            xw.zWriteElementText("tel3", company.detail2Phone);
            xw.zWriteElementText("fax", company.headerFax);
            xw.zWriteElementText("fax2", company.detail1Fax);
            xw.zWriteElementText("fax3", company.detail2Fax);
            xw.zWriteElementText("email", company.headerEmail);
            xw.zWriteElementText("email2", company.detail1Email);
            xw.zWriteElementText("email3", company.detail2Email);
            xw.zWriteElementText("site", company.detail1WebSite);
            xw.zWriteElementText("site2", company.detail2WebSite);
            xw.zWriteElementText("présentation", company.detail1Presentation);
            xw.zWriteElementText("présentation2", company.detail2Presentation);
            xw.zWriteElementText("client", company.detail1Clients);
            xw.zWriteElementText("client2", company.detail2Clients);
            xw.zWriteElementText("dirigeant", company.detail1Leader);
            xw.zWriteElementText("dirigeant2", company.detail2Leader);
            xw.zWriteElementText("nb_salarié", company.detail1EmployeNumber.ToString());
            xw.zWriteElementText("nb_salarié2", company.detail2EmployeNumber.ToString());
            xw.zWriteElementText("chiffre_affaire", company.detail1LastYearRevenue);
            xw.zWriteElementText("chiffre_affaire2", company.detail2LastYearRevenue);
            xw.zWriteElementText("certification", company.detail1Certification);
            xw.zWriteElementText("certification2", company.detail2Certification);
            xw.zWriteElementText("siret", company.detail1Siret);
            xw.zWriteElementText("siret2", company.detail2Siret);

            //xw.zWriteElementText("load_date", string.Format("{0:dd/MM/yyyy HH:mm}", company.loadFromWebDate));
            //xw.zWriteElementText("url_entête", company.urlHeader);
            xw.zWriteElementText("url_detail1", company.urlDetail1);
            xw.zWriteElementText("url_detail2", company.urlDetail2);
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
                    xw.zWriteElementText("document", detail1DownloadDocuments.Current.name);
                    xw.zWriteElementText("document_url", detail1DownloadDocuments.Current.url);
                }
                if (detail2DownloadDocument)
                {
                    xw.zWriteElementText("document2", detail2DownloadDocuments.Current.name);
                    xw.zWriteElementText("document2_url", detail2DownloadDocuments.Current.url);
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

        private static void WriteLine(string msg, params object[] prm)
        {
            Trace.CurrentTrace.WriteLine(msg, prm);
        }
    }
}
