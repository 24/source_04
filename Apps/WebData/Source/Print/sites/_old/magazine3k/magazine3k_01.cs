using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

//namespace Print.download
namespace Download.Print
{
    //public class magazine3kSearch
    //{
    //    public string url;
    //    public string title;
    //    public string language;
    //    public string category;
    //    public int downloads;
    //    public Date updated;
    //    public string imageUrl;
    //    public string[] infos;
    //}

    public class magazine3kPrint_01
    {
        public string url;
        public string title;
        public string title2;
        public string language;
        public string category;
        public int downloads;
        public Date updated;
        public string imageUrl;
        public string[] infos;
        public string comment;
        public string[] downloadLinks;

        // Category : magazine > for-women
        private static Regex _rgCategory = new Regex(@"category\s*\:\s*magazine\s*>\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        // Base64.encrypt(Base64.decode('TFlwdFg6Sldvb0ZMLjNRaWRJOFBFSHMuUjZGY3c/P01PeDZKOXZrQjdNQmtOVFY='))
        private static Regex _rgScriptCode = new Regex(@"Base64\.encrypt\(Base64\.decode\('([^']*)'\)\)", RegexOptions.Compiled);
        // L'Express(+ Supplément : L'ExpressStyles) 3160 - 25 au 31 Janvier 2012 French | 134+84 pages | PDF | 132+72 MB A la une dans ce numéro : 2007-2012 : le vrai bilanEt également : Afghanistan booster l'hiver Côte d'Ivoire Fendi Finlande Jean-Luc Mélenchon la peur du déclin Lagardère Paris Racing Lana Del Rey les saisons des émotions Michel Galabru Nicolas Sarkozy Patti Smith Petit Bateau Riom Spiegelman Vanessa Paradis Vincent Darré
        private static Regex _rgInfo = new Regex(@"^(.*)\s+([^\s]+)\s*\|\s*(.*)\s*\|\s*(.*)\s*\|\s*(.*?\s*mb)\s*(.*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public void SetInfo1(string[] infos)
        {
            for (int i = 0; i < infos.Length; i++)
            {
                string name = infos[i];
                switch (name.ToLower())
                {
                    case "downloads:":
                        int n;
                        if (int.TryParse(infos[++i], out n))
                            downloads = n;
                        break;
                    case "updated:":
                        Date date;
                        if (Date.TryParseExact(infos[++i], "dd-MM-yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out date))
                            updated = date;
                        break;
                }
            }
        }

        public void SetInfo2(string info)
        {
            // French | 140+96 pages | HQ PDF | 129+79 MB
            string[] infoList = zsplit.Split(info, '|', true);
            if (infoList.Length > 0)
            {
                language = infoList[0];
                infos = new string[infoList.Length - 1];
                Array.Copy(infoList, 1, infos, 0, infoList.Length - 1);
            }
        }

        public void SetInfo3(string info)
        {
            // L'Express(+ Supplément : L'ExpressStyles) 3160 - 25 au 31 Janvier 2012 French | 134+84 pages | PDF | 132+72 MB A la une dans ce numéro : 2007-2012 : le vrai bilanEt également : Afghanistan booster l'hiver Côte d'Ivoire Fendi Finlande Jean-Luc Mélenchon la peur du déclin Lagardère Paris Racing Lana Del Rey les saisons des émotions Michel Galabru Nicolas Sarkozy Patti Smith Petit Bateau Riom Spiegelman Vanessa Paradis Vincent Darré
            Match match = _rgInfo.Match(info);
            if (match.Success)
            {
                title2 = match.Groups[1].Value;
                language = match.Groups[2].Value;
                infos = new string[] { match.Groups[3].Value, match.Groups[4].Value, match.Groups[5].Value };
                comment = match.Groups[6].Value;
            }
        }

        public void SetCategory(string categoryLabel)
        {
            // Category : magazine > for-women
            Match match = _rgCategory.Match(categoryLabel);
            if (match.Success)
                category = match.Groups[1].Value;
        }

        public void SetDownloadLinks(string[] scriptLinks)
        {
            List<string> links = new List<string>();
            foreach (string scriptLink in scriptLinks)
            {
                Match match = _rgScriptCode.Match(scriptLink);
                if (match.Success)
                {
                    string code = match.Groups[1].Value;
                    links.Add(DecodeDownloadLink(code));
                }
            }
            downloadLinks = links.ToArray();
        }

        public static string DecodeDownloadLink(string code)
        {
            byte[] bytes = System.Convert.FromBase64String(code);
            //magazine3k.magazine3k m3k = new magazine3k.magazine3k();
            //m3k.init();
            //Magazine3kEncrypt.init();
            //return (string)m3k.encrypt(new string(Encoding.Default.GetChars(bytes)));
            return (string)Magazine3kEncrypt.encrypt(new string(Encoding.Default.GetChars(bytes)));
        }
    }

    public class magazine3k_01
    {
        public static HtmlXmlReader ghxr = HtmlXmlReader.CurrentHtmlXmlReader;

        public static magazine3kPrint_01 loadPrint(string url)
        {
            ghxr.Load(url);
            magazine3kPrint_01 print = new magazine3kPrint_01();
            print.url = url;
            print.title = ghxr.SelectValue("//div[@class='headline']", ".//text()");
            print.SetInfo1(ghxr.SelectValues("//div[@class='res_data']//text()"));
            print.imageUrl = ghxr.SelectValue("//div[@class='res_image']//img/@src");
            XmlNode node = ghxr.SelectNode("//div[@class='justi']:.:EmptyRow");
            print.SetInfo2(ghxr.SelectValue(node, ".//div/text()"));
            print.comment = ghxr.SelectValue(node, ".//div/following-sibling::text()");
            print.SetDownloadLinks(ghxr.SelectValues("//div[@class='download_top']/following-sibling::div", "./script/text()"));
            return print;
        }

        public static magazine3kPrint_01[] search(string url, bool detail = false)
        {
            List<magazine3kPrint_01> prints = new List<magazine3kPrint_01>();
            ghxr.Load(url);
            while (true)
            {
                string urlNextPage = ghxr.SelectValue("//a[@class='pBtnSelected']/following-sibling::a/@href:.:EmptyRow");
                XmlSelect select = ghxr.Select("//div[@class='res']", "@class", ".//a/@href:.:n(href)", ".//a//text():.:n(label1)",
                  ".//span[1]//text():.:n(info1)", ".//span[2]//text():.:n(info2)", ".//span[3]//text():.:n(info3)", ".//span[4]//text():.:n(info4)",
                  ".//img/@src:.:n(img)", ".//div[@class='justi']//text():.:n(label2):Concat()", ".//div[@class='cat']/text():.:n(category)");
                while (select.Get())
                {
                    magazine3kPrint_01 print;
                    if (!detail)
                    {
                        print = new magazine3kPrint_01();
                        print.url = (string)select["href"];
                        print.title = (string)select["label1"];
                        print.SetCategory((string)select["category"]);
                        print.SetInfo1(new string[] { (string)select["info1"], (string)select["info2"], (string)select["info3"], (string)select["info4"] });
                        print.imageUrl = (string)select["img"];
                        print.SetInfo3((string)select["label2"]);
                    }
                    else
                    {
                        string urlDetail = (string)select["href"];
                        print = loadPrint(urlDetail);
                    }
                    prints.Add(print);
                }
                if (urlNextPage == null) break;
                ghxr.Load(urlNextPage);
            }
            return prints.ToArray();
        }
    }
}
