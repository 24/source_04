using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PB_Util;
using Download.Unea;
using pb.Web.old;
using pb.Web;

namespace Download.Damien
{
    static partial class w
    {
        public static void Test_Unea_request_01()
        {
            //   req region :
            //     method post    
            //     url http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/annuaire.asp
            //     Cookie __atuvc=1%7C4; PHPSESSID=t85siq9oqn4sqmi0eg09schbh0; ASPSESSIONIDQABATCST=BDBNKJFAHJNFHLOCOJGCGOOF; __utma=169855717.692259951.1390462116.1391519633.1391522222.4; __utmc=169855717; __utmz=169855717.1390462116.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none)
            //     Referer http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/annuaire.asp?txtRecherche=Entrez%20le%20nom%20d'une%20entreprise&hiddenValider=true
            //     postData hiddenValider=true&txtRecherche=Par+Nom+Entreprise&txtRecherche1=&txtRecherche2=1&txtRecherche3=&txtRecherche4=
            //     txtRecherche2=1 ==> 1 = Alsace

            string url = "http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/annuaire.asp";
            string request = "hiddenValider=true&txtRecherche=Par+Nom+Entreprise&txtRecherche1=&txtRecherche2=1&txtRecherche3=&txtRecherche4=";
            string referer = "http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/annuaire.asp?txtRecherche=Entrez%20le%20nom%20d'une%20entreprise&hiddenValider=true";

            //HtmlXmlReader hr = Http2.HtmlReader;
            //hr.WebRequestMethod = HttpRequestMethod.Post;
            //hr.WebRequestContent = request;
            //hr.WebRequestContentType = "application/x-www-form-urlencoded";
            //hr.WebRequestReferer = referer;
            HttpRequestParameters_v1 requestParameters = new HttpRequestParameters_v1();
            requestParameters.method = HttpRequestMethod.Post;
            requestParameters.content = request;
            requestParameters.contentType = "application/x-www-form-urlencoded";
            requestParameters.referer = referer;
            Http_v3.LoadUrl(url, requestParameters);
        }

        public static void Test_Unea_LoadHeaderFromWeb_01()
        {
            Unea.Unea.Init();
            //string request = "hiddenValider=true&txtRecherche=Par+Nom+Entreprise&txtRecherche1=&txtRecherche2=1&txtRecherche3=&txtRecherche4=";
            bool reload = false;
            bool loadImage = false;

            string url = "http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/annuaire.asp";
            string referer = "http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/annuaire.asp?txtRecherche=Entrez%20le%20nom%20d'une%20entreprise&hiddenValider=true";
            string request = "hiddenValider=true&txtRecherche=&txtRecherche1=&txtRecherche2=1&txtRecherche3=&txtRecherche4=";
            string requestContentType = "application/x-www-form-urlencoded";

            HttpRequestParameters_v1 requestParameters = new HttpRequestParameters_v1();
            requestParameters.method = HttpRequestMethod.Post;
            requestParameters.content = request;
            requestParameters.contentType = requestContentType;
            requestParameters.referer = referer;

            Unea_LoadHeaderFromWeb load = new Unea_LoadHeaderFromWeb(url, requestParameters, reload, loadImage);
            load.Load();
            load.Data.zView();
        }

        public static void Test_Unea_LoadHeader_01()
        {
            Unea.Unea.Init();
            //string request = "hiddenValider=true&txtRecherche=Par+Nom+Entreprise&txtRecherche1=&txtRecherche2=1&txtRecherche3=&txtRecherche4=";
            bool reload = false;
            bool loadImage = false;

            string url = "http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/annuaire.asp";
            string referer = "http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/annuaire.asp?txtRecherche=Entrez%20le%20nom%20d'une%20entreprise&hiddenValider=true";
            string request = "hiddenValider=true&txtRecherche=&txtRecherche1=&txtRecherche2=1&txtRecherche3=&txtRecherche4=";
            string requestContentType = "application/x-www-form-urlencoded";

            HttpRequestParameters_v1 requestParameters = new HttpRequestParameters_v1();
            requestParameters.method = HttpRequestMethod.Post;
            requestParameters.content = request;
            requestParameters.contentType = requestContentType;
            requestParameters.referer = referer;

            Unea_LoadHeader load = new Unea_LoadHeader(url, requestParameters);
            load.Load(reload, loadImage);
            load.Data.zView();
        }

        public static void Test_Unea_LoadHeaderPages_01(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false)
        {
            Unea.Unea.Init();
            Unea_LoadHeaderPages load = new Unea_LoadHeaderPages(startPage, maxPage, reload, loadImage);
            load.zView();
        }

        public static void Unea_LoadDetailCompany1FromWeb_01(string url, bool reload = false, bool loadImage = false)
        {
            Unea.Unea.Init();
            //string url = "http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/detail.asp?id=4017";
            //bool reload = false;
            //bool loadImage = false;
            Unea_DetailCompany1 company = Unea_LoadDetailCompany1FromWeb.LoadCompany(url, reload, loadImage);
            //_rs.View(company);
            company.zView();
        }

        public static void Unea_LoadDetailCompany2FromWeb_01()
        {
            Unea.Unea.Init();
            string url = "http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/4017/ALSACE%20ENTREPRISE%20ADAPTEE.htm";
            bool reload = false;
            bool loadImage = false;
            Unea_DetailCompany2 company = Unea_LoadDetailCompany2FromWeb.LoadCompany(url, reload, loadImage);
            //_rs.View(company);
            company.zView();
        }

        public static void Test_Unea_LoadCompanyList_01(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false, string companyName = null)
        {
            Unea.Unea.Init();
            if (companyName == null)
                //_rs.View(Unea.Unea.LoadDetailCompanyList(startPage, maxPage, reload, loadImage));
                Unea.Unea.LoadDetailCompanyList(startPage, maxPage, reload, loadImage).zView();
            else
                //_rs.View((from company in Unea.Unea.LoadDetailCompanyList(startPage, maxPage, reload, loadImage) where company.headerName == companyName select company).FirstOrDefault());
                ((from company in Unea.Unea.LoadDetailCompanyList(startPage, maxPage, reload, loadImage) where company.headerName == companyName select company).FirstOrDefault()).zView();
        }
    }
}
