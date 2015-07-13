using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using MongoDB.Bson.Serialization;
using pb;
using pb.Data;
using pb.Data.Xml;
using pb.Text;

namespace Download.Print.TelechargementPlus
{
    public class TelechargementPlus_Base
    {
        public string title = null;
        public List<string> description = new List<string>();
        public string language = null;
        public string size = null;
        public int? nbPages = null;
        public NamedValues<ZValue> infos = new NamedValues<ZValue>();

        public void SetTextValues(IEnumerable<string> texts)
        {
            string name = null;
            string text = null;
            foreach (string s in texts)
            {
                // PDF | 116 pages | 53 Mb | French
                //Trace.CurrentTrace.WriteLine("SetTextValues : \"{0}\"", s);
                if (s == "\r\n")
                {
                    if (text != null)
                    {
                        if (name != null)
                            infos.SetValue(name, new ZString(text));
                        else
                            description.Add(text);
                        text = null;
                    }
                    name = null;
                }
                else
                {
                    //string s2 = TelechargementPlus.TrimString(TelechargementPlus.ExtractTextValues(infos, s));
                    string s2 = TelechargementPlus.TrimFunc1(TelechargementPlus.ExtractTextValues(infos, s));
                    if (infos.ContainsKey("language"))
                    {
                        language = (string)infos["language"];
                        infos.Remove("language");
                    }
                    else if (infos.ContainsKey("size"))
                    {
                        size = (string)infos["size"];
                        infos.Remove("size");
                    }
                    else if (infos.ContainsKey("page_nb"))
                    {
                        nbPages = int.Parse((string)infos["page_nb"]);
                        infos.Remove("page_nb");
                    }
                    //Trace.WriteLine("text \"{0}\" => \"{1}\"", s, s2);
                    bool foundName = false;
                    if (s2.EndsWith(":"))
                    {
                        string s3 = s2.Substring(0, s2.Length - 1).Trim();
                        if (s3 != "")
                        {
                            name = s3;
                            foundName = true;
                        }
                    }
                    //else if (s2 != "" && s2 != title)
                    if (!foundName && s2 != "" && s2 != title)
                    {
                        if (text == null)
                            text = s2;
                        else
                            text += " " + s2;
                    }
                }
            }
            if (text != null)
            {
                if (name != null)
                    infos.SetValue(name, new ZString(text));
                else
                    description.Add(text);
            }
        }
    }

    public static class TelechargementPlus
    {
        private static char[] _trimChars = new char[] { ' ', '\t', '\n', '\r', ',', '»', '&', '+', '/', '|', '*', '=' };
        private static Func<string, string> _trimFunc1 = text => text.Trim(_trimChars);
        private static RegexValuesList _textInfoRegexList = null;
        private static Dictionary<string, string> _imagesToSkip = null;

        static TelechargementPlus()
        {
            Init();
        }

        public static Func<string, string> TrimFunc1 { get { return _trimFunc1; } }
        public static Dictionary<string, string> ImagesToSkip { get { return _imagesToSkip; } }

        public static void Init()
        {
            //TelechargementPlus_LoadHeader.ClassInit(XmlConfig.CurrentConfig.GetElement("TelechargementPlus/Header"));
            _textInfoRegexList = new RegexValuesList(XmlConfig.CurrentConfig.GetElements("TelechargementPlus/TextInfo"));
            InitImagesToSkip();
        }

        public static void InitMongoClassMap()
        {
            // Register all class derived from ZValue to deserialize field NamedValues<ZValue> infos (TelechargementPlus_Base)
            //   ZString, ZStringArray, ZInt
            if (!BsonClassMap.IsClassMapRegistered(typeof(ZString)))
                BsonClassMap.RegisterClassMap<ZString>();
            if (!BsonClassMap.IsClassMapRegistered(typeof(ZStringArray)))
                BsonClassMap.RegisterClassMap<ZStringArray>();
            if (!BsonClassMap.IsClassMapRegistered(typeof(ZInt)))
                BsonClassMap.RegisterClassMap<ZInt>();
        }

        private static void InitImagesToSkip()
        {
            _imagesToSkip = new Dictionary<string, string>();
            foreach (XElement xe in XmlConfig.CurrentConfig.GetElements("TelechargementPlus/ImageToSkip"))
                _imagesToSkip.Add(xe.zAttribValue("url"), xe.zAttribValue("name"));
        }

        public static string ExtractTextValues(NamedValues<ZValue> infos, string s)
        {
            // French | PDF | 107 MB -*- French | PDF |  22 Pages | 7 MB -*- PDF | 116 pages | 53 Mb | French -*- Micro Application | 2010 | ISBN: 2300028441 | 221 pages | PDF
            // pb : |I|N|F|O|S|, |S|Y|N|O|P|S|I|S|, |T|E|L|E|C|H|A|R|G|E|R|
            // example http://www.telechargement-plus.com/e-book-magazines/bande-dessines/136846-season-one-100-marvel-syrie-en-cours-10-tomes-comicmulti.html
            if (s.Contains('|'))
            {
                //Trace.CurrentTrace.WriteLine("info \"{0}\"", s);
                foreach (string s2 in zsplit.Split(s, '|', true))
                {
                    string s3 = s2;
                    NamedValues<ZValue> values = _textInfoRegexList.ExtractTextValues(ref s3);
                    infos.SetValues(values);
                    s3 = s3.Trim();
                    if (s3 != "")
                        infos.SetValue(s3, null);
                }
                return "";
            }
            else
            {
                NamedValues<ZValue> values = _textInfoRegexList.ExtractTextValues(ref s);
                infos.SetValues(values);
                return s;
            }
        }

        private static Regex _rgDate = new Regex("(aujourd'hui|hier)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static DateTime? ParseDateTime(string date)
        {
            // Aujourd'hui, 17:13
            if (date == null)
                return null;
            Match match = _rgDate.Match(date);
            if (match.Success)
            {
                string s = null;
                switch (match.Groups[1].Value.ToLower())
                {
                    case "aujourd'hui":
                        s = DateTime.Today.ToString("dd-MM-yyyy");
                        break;
                    case "hier":
                        s = DateTime.Today.AddDays(-1).ToString("dd-MM-yyyy");
                        break;
                }
                date = match.zReplace(date, s);
            }
            //if (string.Equals(date, "Aujourd'hui", StringComparison.CurrentCultureIgnoreCase))
            //    date = DateTime.Today.ToString("dd/MM/yyyy");
            //else if (string.Equals(date, "Hier", StringComparison.CurrentCultureIgnoreCase))
            //    date = DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy");
            DateTime dt;
            if (DateTime.TryParseExact(date, @"dd-MM-yyyy, HH:mm", CultureInfo.CurrentCulture, DateTimeStyles.None, out dt))
                //return true;
                return dt;
            Trace.WriteLine("unknow date time \"{0}\"", date);
            //return false;
            return null;
        }

        public static IEnumerable<TelechargementPlus_PostDetail> LoadDetailItemList(int startPage = 1, int maxPage = 1, bool reloadHeaderPage = false, bool reloadDetail = false, bool loadImage = false)
        {
            return from header in TelechargementPlus_LoadHeaderPages.LoadHeaderPages(startPage, maxPage, reloadHeaderPage, loadImage) select LoadDetailItem(header, reloadDetail, loadImage);
        }

        public static TelechargementPlus_PostDetail LoadDetailItem(TelechargementPlus_PostHeader header, bool reload = false, bool loadImage = false)
        {
            return TelechargementPlus_LoadDetail.Load(header.urlDetail, reload: reload, loadImage: loadImage);
        }
    }
}
