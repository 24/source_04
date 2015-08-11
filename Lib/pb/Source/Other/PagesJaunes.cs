#region using
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Linq;
using PB_Library;
using pb.Web;
#endregion

#region a faire ...
#endregion

namespace PB_Library
{
    #region class PagesJaunesException
    public class PagesJaunesException : Exception
    {
        public PagesJaunesException(string sMessage) : base(sMessage) { }
        public PagesJaunesException(string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm)) { }
        public PagesJaunesException(Exception InnerException, string sMessage) : base(sMessage, InnerException) { }
        public PagesJaunesException(Exception InnerException, string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm), InnerException) { }
    }
    #endregion

    #region enum Page
    public enum Page
    {
        NextPage = 1,
        PreviousPage
    }
    #endregion

    #region class PagesJaunes
    public class PagesJaunes
    {
        #region variable
        private Http gHttp = null;
        private XDocument gWebDocument = null;
        private string gsUrlPagesJaunes = "http://www.pagesjaunes.fr";
        private string gsSchemaPath = null;  // "PagesJaunes_schema_2.xml"
        private string gsSchemaName = null;
        private string gsTraceDir = null;
        private string gsResultDirectory = null;
        private XDocument gXmlResult = null;
        private bool gbAbort = false;

        private ZReadSchema gReadSchema = null;
        private int giPageNumber;

        private string gsWebRequest_XslPath = null;
        private string gsWebRequest_QuoiQui = null;
        private string gsWebRequest_Ou = null;
        private string gsWebRequest_UrlNextPage = null;
        private const string cWebRequest_UrlPrefix = "/pagesjaunes/";
        #endregion

        #region constructor
        public PagesJaunes(string SchemaPath, string SchemaName)
        {
            gsSchemaPath = SchemaPath;
            gsSchemaName = SchemaName;
            Init();
        }
        #endregion

        #region Init
        private void Init()
        {
            gHttp = new Http();
        }
        #endregion

        #region property ...
        #region SchemaPath
        public string SchemaPath
        {
            get { return gsSchemaPath; }
            set { gsSchemaPath = value; }
        }
        #endregion

        #region SchemaName
        public string SchemaName
        {
            get { return gsSchemaName; }
            set { gsSchemaName = value; }
        }
        #endregion

        #region TraceDir
        public string TraceDir
        {
            get { return gsTraceDir; }
            set { gsTraceDir = value; }
        }
        #endregion

        #region ResultDirectory
        public string ResultDirectory
        {
            get { return gsResultDirectory; }
        }
        #endregion

        #region XmlResult
        public XDocument XmlResult
        {
            get { return gXmlResult; }
        }
        #endregion

        #region Abort
        public bool Abort
        {
            get { return gbAbort; }
            set { gbAbort = value; }
        }
        #endregion

        #region WebRequest_XslPath
        public string WebRequest_XslPath
        {
            get { return gsWebRequest_XslPath; }
            set { gsWebRequest_XslPath = value; }
        }
        #endregion
        #endregion

        #region WebRequest
        public void WebRequest(Uri url, NameValueCollection queryValues)
        {
            // AbsolutePath :
            // /pagesjaunes/RechercheClassique.req?quoiqui=toto&ou=paris
            string path = url.AbsolutePath.ToLower();
            if (!path.StartsWith(cWebRequest_UrlPrefix))
                throw new PagesJaunesException("error in PagesJaunes WebRequest, bad url {0}", path);
            path = path.Remove(0, cWebRequest_UrlPrefix.Length);
            switch (path)
            {
                case "rechercheclassique.req":
                    WebRequestRechercheClassique(queryValues);
                    break;
                default:
                    throw new PagesJaunesException("error in PagesJaunes WebRequest, bad url {0}", path);
            }

        }
        #endregion

        #region WebRequestRechercheClassique
        private void WebRequestRechercheClassique(NameValueCollection queryValues)
        {
            // pagesjaunes/RechercheClassique.req?quoiqui=toto&ou=paris
            string QuoiQui = null;
            string Ou = null;
            string page = null;
            for (int i = 0; i < queryValues.Count; i++)
            {
                string key = queryValues.Keys[i];
                string[] values = queryValues.GetValues(i);
                switch (key.ToLower())
                {
                    case "quoiqui":
                        QuoiQui = values[0];
                        break;
                    case "ou":
                        Ou = values[0];
                        break;
                    case "page":
                        page = values[0];
                        break;
                    default:
                        throw new PagesJaunesException("error in PagesJaunes WebRequest, query value {0} is not valid", key);
                }
                if (values.Length > 1) throw new PagesJaunesException("error in PagesJaunes WebRequest, query value {0} has multiple values {1}", key, queryValues[i]);
            }
            if (QuoiQui == null) throw new PagesJaunesException("error in PagesJaunes WebRequest, missing \"quoiqui\" value");
            if (Ou == null) throw new PagesJaunesException("error in PagesJaunes WebRequest, missing \"ou\" value");
            if (gsWebRequest_QuoiQui != QuoiQui || gsWebRequest_Ou != Ou)
            {
                OpenRechercheClassique(QuoiQui, Ou);

                giPageNumber = 1;
                gReadSchema.Constants["Page"] = giPageNumber.ToString();
                gReadSchema.Read(gWebDocument, gHttp.TextExportPath, gHttp.XmlExportPath);
            }
            else
            {
                gReadSchema.Results.Clear();
                switch (page.ToLower())
                {
                    case "next":
                        RechercheClassiqueLoadPage(Page.NextPage);
                        break;
                    case "previous":
                        RechercheClassiqueLoadPage(Page.PreviousPage);
                        break;
                    default:
                        throw new PagesJaunesException("error in PagesJaunes WebRequest, query value {0} is not valid", page);
                }
            }
            gsWebRequest_UrlNextPage = GetNextPage();
        }
        #endregion

        #region GetHtmlResult
        public string GetHtmlResult()
        {
            gXmlResult = gReadSchema.ResultToXml();
            return Xml.XslTransformXmlReader(gsWebRequest_XslPath, gXmlResult.CreateReader());
        }
        #endregion


        #region RechercheClassique
        public void RechercheClassique(string QuoiQui, string Ou)
        {
            //WebGetRequest(gsUrlPagesJaunes);

            //string url = GetRequestUrl();
            //string content = "ambiguiteVoie=false&choixAmbiguite=false&choixMultiLoc=false&codeLieu=&flashInactif=true&ou=" + Ou + "&ouAmbiguChoisi=&pageAccueil=true&quoiqui=" + QuoiQui;
            //WebPostRequest(url, content);

            //gReadSchema = new ZReadSchema(gsSchemaPath);

            //SortedList<string, string> constants = new SortedList<string, string>();
            //constants.Add("Index", null);
            //constants.Add("Page", null);
            //gReadSchema.Constants = constants;

            //SortedList<string, string> constants = gReadSchema.Constants;

            //giPageNumber = 1;
            //constants["Page"] = giPageNumber.ToString();
            //gReadSchema.Read(gWebDocument, gHttp.TextExportPath, gHttp.XmlExportPath);

            OpenRechercheClassique(QuoiQui, Ou);

            while (true)
            {
                if (Abort) break;
                //string url = GetNextPage();
                //if (url == null) break;

                //WebGetRequest(url);
                //constants["Page"] = (++giPageNumber).ToString();
                //gReadSchema.Read(gWebDocument, gHttp.TextExportPath, gHttp.XmlExportPath);

                if (!RechercheClassiqueLoadPage(Page.NextPage)) break;
            }

            gsResultDirectory = GetNewResultDirectory();
            //readSchema.ExportNullValue = true;
            gXmlResult = gReadSchema.ResultToXml();
            gXmlResult.Save(gsResultDirectory + "Results.xml");
            gReadSchema.MessageToXml().Save(gsResultDirectory + "Messages.xml");
            gReadSchema.SaveSourceFile(gsResultDirectory + "SourceFile");
            gReadSchema.SaveXmlSource(gsResultDirectory + "XmlSource");
        }
        #endregion

        #region RechercheClassiqueLoadPage
        private bool RechercheClassiqueLoadPage(Page page)
        {
            string url = null;
            if (page == Page.NextPage)
            {
                url = GetNextPage();
                if (url == null) return false;
                giPageNumber++;
            }
            else
                return false;
            WebGetRequest(url);
            gReadSchema.Constants["Page"] = giPageNumber.ToString();
            gReadSchema.Read(gWebDocument, gHttp.TextExportPath, gHttp.XmlExportPath);
            return true;
        }
        #endregion

        #region OpenRechercheClassique
        private void OpenRechercheClassique(string QuoiQui, string Ou)
        {
            // Post
            // http://www.pagesjaunes.fr/trouverlesprofessionnels/rechercheClassique.do;jsessionid=94463604270DD257740D395752458C01.yas05g
            // ?codeLieu=&ouAmbiguChoisi=&ambiguiteVoie=false&choixAmbiguite=false&pageAccueil=true&choixMultiLoc=false&flashInactif=false&quoiqui=Restaurant&ou=paris+14

            WebGetRequest(gsUrlPagesJaunes);

            string url = GetRequestUrl();
            //string content = "ambiguiteVoie=false&choixAmbiguite=false&choixMultiLoc=false&codeLieu=&flashInactif=true&ou=" + Ou + "&ouAmbiguChoisi=&pageAccueil=true&quoiqui=" + QuoiQui;
            string content = "codeLieu=&ouAmbiguChoisi=&ambiguiteVoie=false&choixAmbiguite=false&pageAccueil=true&choixMultiLoc=false&flashInactif=false&quoiqui=" + QuoiQui + "&ou=" + Ou;


            //gHttp.TraceDirectory = gsTraceDir;
            WebPostRequest(url, content);

            gReadSchema = new ZReadSchema(gsSchemaPath, gsSchemaName);

            SortedList<string, string> constants = new SortedList<string, string>();
            constants.Add("Index", null);
            constants.Add("Page", null);
            gReadSchema.Constants = constants;

            giPageNumber = 1;
            constants["Page"] = giPageNumber.ToString();
            gReadSchema.Read(gWebDocument, gHttp.TextExportPath, gHttp.XmlExportPath);
        }
        #endregion

        #region WebGetRequest
        private void WebGetRequest(string url)
        {
            cTrace.Trace("Load : GET  {0}", url);
            gHttp.Load(url);
            gWebDocument = gHttp.GetXDocumentResult();
        }
        #endregion

        #region WebPostRequest
        private void WebPostRequest(string url, string content)
        {
            #region Expect: 100-continue
            // Pour supprimer Expect: 100-continue, HTTP/1.1 100 Continue (http://haacked.com/archive/2004/05/15/http-web-request-expect-100-continue.aspx)
            //System.Net.ServicePointManager.Expect100Continue = true;
            #endregion

            cTrace.Trace("Load : POST {0}    {1}", url, content);
            gHttp.Url = url;
            gHttp.Method = HttpRequestMethod.Post;
            gHttp.Referer = "http://www.pagesjaunes.fr/";
            gHttp.RequestContentType = "application/x-www-form-urlencoded; charset=UTF-8"; // indispensable
            gHttp.Content = content;
            gHttp.TraceDirectory = gsTraceDir;
            cTrace.Trace("TraceDirectory : {0}", gsTraceDir);
            gHttp.Load();
            gWebDocument = gHttp.GetXDocumentResult();

            #region entete requete http
            // entete de la requete http :
            //POST /trouverlesprofessionnels/rechercheClassique.do;jsessionid=ED60BB78FB6E0D9AABFABD7D6A4ECCBC.yas04g?idContext=1070591850&portail=PJ HTTP/1.1
            //Accept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8
            //Referer: http://www.pagesjaunes.fr/
            //Host: www.pagesjaunes.fr
            //Cookie: VisitorID=44124153260506674
            //Content-Length: 144
            //ambiguiteVoie=false&choixAmbiguite=false&choixMultiLoc=false&codeLieu=&flashInactif=false&ou=paris&ouAmbiguChoisi=&pageAccueil=true&quoiqui=sncf

            // requete http à partir de google chrome :
            //POST /trouverlesprofessionnels/rechercheClassique.do;jsessionid=59B347EE93802EBD6F448F5BDDDBBFCD.yas06g?idContext=-1296165720&portail=PJ HTTP/1.1
            //Host: www.pagesjaunes.fr
            //Connection: keep-alive
            //User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US) AppleWebKit/530.5 (KHTML, like Gecko) Chrome/2.0.172.8 Safari/530.5
            //Referer: http://www.pagesjaunes.fr/
            //Content-Length: 144
            //Cache-Control: max-age=0
            //Origin: http://www.pagesjaunes.fr
            //Content-Type: application/x-www-form-urlencoded
            //Accept: application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5
            //Accept-Encoding: gzip,deflate,bzip2,sdch
            //Cookie: JSESSIONID=59B347EE93802EBD6F448F5BDDDBBFCD.yas06g; RMID=513908194971ffb0; e=SXH-tsCoCh4AAFk97iY; ctr=1; rndNumber=54.76719243451953; crmseen=seen; VisitorID=178123220792083019; crm_cookieEnabled=1
            //Accept-Language: fr-FR,fr;q=0.8,en-US;q=0.6,en;q=0.4
            //Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.3
            //codeLieu=&ouAmbiguChoisi=&ambiguiteVoie=false&choixAmbiguite=false&pageAccueil=true&choixMultiLoc=false&flashInactif=false&quoiqui=Sncf&ou=paris


            // chargement des infos horaires
            //POST /trouverlesprofessionnels/infoHoraireAjax.do HTTP/1.1
            //Host: www.pagesjaunes.fr
            //Connection: keep-alive
            //User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US) AppleWebKit/530.5 (KHTML, like Gecko) Chrome/2.0.172.30 Safari/530.5
            //Referer: http://www.pagesjaunes.fr/trouverlesprofessionnels/rechercheClassique.do;jsessionid=059D94FB0E1D2E5E39DA6D7CE333BD19.yas07g?idContext=1408801085&portail=PJ
            //Content-Length: 94
            //Origin: http://www.pagesjaunes.fr
            //Content-Type: application/x-www-form-urlencoded
            //Accept: */*
            //Accept-Encoding: gzip,deflate,bzip2,sdch
            //Cookie: JSESSIONID=059D94FB0E1D2E5E39DA6D7CE333BD19.yas07g; RMID=513908194971ffb0; e=SXH-tsCoCh4AAFk97iY; VisitorID=XX-119190EF-14E32; myFormPref=S; ctr=1; rndNumber=15.067804977297783; crmseen=seen; crm_cookieEnabled=1; VisitorID=43124143876010160
            //Accept-Language: fr-FR,fr;q=0.8,en-US;q=0.6,en;q=0.4
            //Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.3
            //crypt=g4HT6Som0NKh1QaR7Z3GizLjmKU9dC6jxzcTih6Jvb5h3Ol+khOX/ajuovCR+DOk6tTRLgG1ioU1gCZdkWtKcw==
            #endregion
        }
        #endregion

        #region GetRequestUrl
        private string GetRequestUrl()
        {
            //return GetRequestUrl1();
            return GetRequestUrl2();
        }
        #endregion

        #region GetRequestUrl1
        private string GetRequestUrl1()
        {
            // formClassiqueHaut
            // /trouverlesprofessionnels/rechercheClassique.do;jsessionid=3ECAE8915EBAD4EB442918F73063B683.yas08g
            string sAction = (from item in gWebDocument.XPathSelectElements("//form") where item.zAttribValue("id") == "formClassiqueHaut" select item.zAttribValue("action")).FirstOrDefault();
            if (sAction == null)
                throw new PagesJaunesException("error in PagesJaunes form id formClassiqueHaut not found");

            // idContext
            // -252780064
            XElement formIdContext = (from item in gWebDocument.XPathSelectElements("//form") where item.zAttribValue("id") == "formIdContext" select item).FirstOrDefault();
            if (formIdContext == null)
                throw new PagesJaunesException("error in PagesJaunes form id formIdContext not found");
            string sIdContext = (from item in formIdContext.XPathSelectElements("input") where item.zAttribValue("id") == "idContext" select item.zAttribValue("value")).FirstOrDefault();
            sIdContext = "idContext=" + sIdContext;

            // portail
            // PJ
            XElement formPortail = (from item in gWebDocument.XPathSelectElements("//form") where item.zAttribValue("id") == "formIdPortail" select item).FirstOrDefault();
            if (formPortail == null)
                throw new PagesJaunesException("error in PagesJaunes form id formIdPortail not found");
            string sPortail = (from item in formPortail.XPathSelectElements("input") where item.zAttribValue("id") == "idPortail" select item.zAttribValue("value")).FirstOrDefault();
            sPortail = "portail=" + sPortail;

            return sAction + "?" + sIdContext + "&" + sPortail;


            //http://www.pagesjaunes.fr/trouverlesprofessionnels/rechercheClassique.do;jsessionid=94463604270DD257740D395752458C01.yas05g?codeLieu=&ouAmbiguChoisi=&ambiguiteVoie=false&choixAmbiguite=false&pageAccueil=true&choixMultiLoc=false&flashInactif=false&quoiqui=Restaurant&ou=paris+14
            //<form id="formClassiqueHaut" method="post" action="/trouverlesprofessionnels/rechercheClassique.do;jsessionid=4E8948548200C71707CE2F1555DEF2E6.yas07g" class="N2_formulaire_express frmTag_INFO idTag_TROUVER">
            //http://www.pagesjaunes.fr/trouverlesprofessionnels/rechercheClassique.do;jsessionid=4E8948548200C71707CE2F1555DEF2E6.yas07g
            //?codeLieu=&ouAmbiguChoisi=&ambiguiteVoie=false&choixAmbiguite=false&pageAccueil=true&choixMultiLoc=false&flashInactif=false&quoiqui=&ou=
        }
        #endregion

        #region GetRequestUrl2
        private string GetRequestUrl2()
        {
            // Post
            // http://www.pagesjaunes.fr/trouverlesprofessionnels/rechercheClassique.do;jsessionid=94463604270DD257740D395752458C01.yas05g
            // ?codeLieu=&ouAmbiguChoisi=&ambiguiteVoie=false&choixAmbiguite=false&pageAccueil=true&choixMultiLoc=false&flashInactif=false&quoiqui=Restaurant&ou=paris+14

            //<form id="formClassiqueHaut" method="post" action="/trouverlesprofessionnels/rechercheClassique.do;jsessionid=4E8948548200C71707CE2F1555DEF2E6.yas07g" class="N2_formulaire_express frmTag_INFO idTag_TROUVER">
            string sAction = (from item in gWebDocument.XPathSelectElements("//form") where item.zAttribValue("id") == "formClassiqueHaut" select item.zAttribValue("action")).FirstOrDefault();
            if (sAction == null)
                throw new PagesJaunesException("error in PagesJaunes form id formClassiqueHaut not found");

            return sAction + "?";
        }
        #endregion

        #region GetNextPage
        private string GetNextPage()
        {
            #region entete http chargement de la page suivante
            //      /trouverlesprofessionnels/changerPage.do;jsessionid=FC075E02492D4674B595A68CF90DAA97.yas04f?crypt=4ndouKSQzf+q0pC0QzWypw==
            //Load("/trouverlesprofessionnels/changerPage.do;jsessionid=FC075E02492D4674B595A68CF90DAA97.yas04f?crypt=4ndouKSQzf+q0pC0QzWypw==");
            //      /trouverlesprofessionnels/changerPage.do?crypt=4ndouKSQzf+q0pC0QzWypw==
            //  GET /trouverlesprofessionnels/changerPage.do?crypt=4ndouKSQzf+q0pC0QzWypw==&idContext=1172169940&portail=PJ HTTP/1.1
            //Host: www.pagesjaunes.fr
            //Connection: keep-alive
            //User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US) AppleWebKit/530.5 (KHTML, like Gecko) Chrome/2.0.172.30 Safari/530.5
            //Referer: http://www.pagesjaunes.fr/trouverlesprofessionnels/rechercheClassique.do;jsessionid=340DB83D543FA988F8EF15B1EF2D6516.yas01f?idContext=1172169939&portail=PJ
            //Accept: application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5
            //Accept-Encoding: gzip,deflate,bzip2,sdch
            //Cookie: JSESSIONID=340DB83D543FA988F8EF15B1EF2D6516.yas01f; RMID=513908194971ffb0; e=SXH-tsCoCh4AAFk97iY; VisitorID=XX-119190EF-14E32; myFormPref=S; ctr=1; crmseen=seen; crm_cookieEnabled=1; VisitorID=43124143876010160
            //Accept-Language: fr-FR,fr;q=0.8,en-US;q=0.6,en;q=0.4
            //Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.3
            #endregion

            string sUrl = (from item in gWebDocument.XPathSelectElements("//a") where item.zAttribValue("class") != null && item.zAttribValue("class").StartsWith("page_suivante") select item.zAttribValue("href")).FirstOrDefault();
            return sUrl;
        }
        #endregion

        #region GetNewResultDirectory
        private string GetNewResultDirectory()
        {
            string sDir = @"C:\_Data\_PagesJaunes\Result";
            if (!sDir.EndsWith("\\")) sDir += "\\";
            sDir = cu.GetNewIndexedDirectory(sDir + "Result_{0:0000}") + "\\";
            cTrace.Trace("SaveResult {0}", sDir);
            if (!Directory.Exists(sDir)) Directory.CreateDirectory(sDir);
            return sDir;
        }
        #endregion

    }
    #endregion
}
