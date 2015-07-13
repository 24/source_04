using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using pb;

//namespace Print.download
namespace Download.Print
{
    public class LinkList : List<string>
    {
        [Visible]
        public string name;
    }

    public class telecharger_pdfPrint
    {
        public string url;
        public string title;
        public string title2;
        //public string language;
        //public string category;
        //public int downloads;
        public Date updated;
        public string imageUrl;
        public string[] infos;
        public string comment;
        //public string[] downloadLinks;
        public LinkList[] downloadLinks;

        public void SetInfo1(string info)
        {
            infos = new string[] { info };
        }
    }

    class telecharger_pdf
    {
        public static pb.old.HtmlXmlReader ghxr = pb.old.HtmlXmlReader.CurrentHtmlXmlReader;

        public static telecharger_pdfPrint[] search(string url, bool detail = false)
        {
            List<telecharger_pdfPrint> prints = new List<telecharger_pdfPrint>();
            ghxr.Load(url);
            while (true)
            {
                //string urlNextPage = ghxr.SelectValue("//a[@class='pBtnSelected']/following-sibling::a/@href:.:EmptyRow");
                string urlNextPage = null;
                //XmlSelect select = ghxr.Select("//div[@class='res']", "@class", ".//a/@href:.:n(href)", ".//a//text():.:n(label1)",
                //  ".//span[1]//text():.:n(info1)", ".//span[2]//text():.:n(info2)", ".//span[3]//text():.:n(info3)", ".//span[4]//text():.:n(info4)",
                //  ".//img/@src:.:n(img)", ".//div[@class='justi']//text():.:n(label2):Concat()", ".//div[@class='cat']/text():.:n(category)");
                pb.old.XmlSelect select = ghxr.Select("//article", "@class", ".//a/@href:.:n(href)", ".//a//text():.:n(label1)",
                    ".//time/@datetime:.:n(time)", ".//span[@class='by-author']//a//text():.:n(author_label)", ".//span[@class='by-author']//a/@href:.:n(author_href)",
                    ".//div[@class='entry-content']//img/@src:.:n(img)",
                    ".//div[@class='entry-content']//p[1]//text():.:n(label2)",
                    ".//div[@class='entry-content']//p[1]//text():.:Value(2):n(label3)",
                    ".//div[@class='entry-content']//p:.:n(p)"
                  );
                while (select.Get())
                {
                    telecharger_pdfPrint print;
                    if (!detail)
                    {
                        print = new telecharger_pdfPrint();
                        print.url = (string)select["href"];
                        print.title = (string)select["label1"];
                        print.title2 = (string)select["label2"];
                        //print.SetCategory((string)select["category"]);
                        print.SetInfo1((string)select["label3"]);
                        print.imageUrl = (string)select["img"];
                        //print.SetInfo3((string)select["label2"]);
                        List<LinkList> links = new List<LinkList>();
                        //Dictionary<string, LinkList> links = new Dictionary<string, LinkList>();
                        XmlNodeList nodes = select.CurrentNode.SelectNodes(".//p");
                        bool first = true;
                        LinkList currentLink = null;
                        string currentLinkName = null;
                        foreach (XmlNode node in nodes)
                        {
                            if (first)
                            {
                                first = false;
                                continue;
                            }
                            XmlNodeList nodes2 = node.SelectNodes(".//a/@href");
                            if (nodes2.Count == 0)
                            {
                                XmlNode node2 = node.SelectSingleNode("./text()");
                                if (print.comment == null && node2 != null)
                                    print.comment = node2.Value;
                            }
                            else
                            {
                                string name = null;
                                //string name2 = "";
                                XmlNode node2 = node.FirstChild;
                                if (node2 != null && node2.NodeType == XmlNodeType.Text)
                                {
                                    name = node2.Value;
                                    //name2 = node2.Value;
                                }
                                //LinkList link = null;
                                if (currentLink == null || name != currentLinkName)
                                {
                                    currentLink = new LinkList();
                                    currentLink.name = name;
                                    links.Add(currentLink);
                                }
                                //if (links.ContainsKey(name2))
                                //{
                                //    link = links[name2];
                                //}
                                //else
                                //{
                                //    link = new LinkList();
                                //    link.name = name;
                                //    links.Add(name2, link);
                                //}
                                foreach (XmlNode node3 in nodes2)
                                {
                                    currentLink.Add(node3.Value);
                                }
                            }
                        }
                        //print.downloadLinks = links.Values.ToArray();
                        print.downloadLinks = links.ToArray();
                    }
                    else
                    {
                        continue;
                        //string urlDetail = (string)select["href"];
                        //print = loadPrint(urlDetail);
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
