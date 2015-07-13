using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
using pb;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Text;
using pb.Web;
using pb.Web.old;
using Print;

namespace Download.Print.RapideDdl
{
    //public class RapideDdl_Base
    //{
    //    public string title = null;
    //    public List<string> description = new List<string>();
    //    public string language = null;
    //    public string size = null;
    //    public int? nbPages = null;
    //    public NamedValues<ZValue> infos = new NamedValues<ZValue>();

    //    public void SetTextValues(IEnumerable<string> texts)
    //    {
    //        // read : title
    //        // modify : infos, description, language, size, nbPages

    //        string name = null;
    //        string text = null;
    //        foreach (string s in texts)
    //        {
    //            // PDF | 116 pages | 53 Mb | French
    //            //Trace.CurrentTrace.WriteLine("SetTextValues : \"{0}\"", s);
    //            if (s == "\r\n")
    //            {
    //                if (text != null)
    //                {
    //                    if (name != null)
    //                        infos.SetValue(name, new ZString(text));
    //                    else
    //                        description.Add(text);
    //                    text = null;
    //                }
    //                name = null;
    //            }
    //            else
    //            {
    //                string s2 = RapideDdl.TrimFunc1(RapideDdl.ExtractTextValues(infos, s));
    //                if (infos.ContainsKey("language"))
    //                {
    //                    language = (string)infos["language"];
    //                    infos.Remove("language");
    //                }
    //                else if (infos.ContainsKey("size"))
    //                {
    //                    size = (string)infos["size"];
    //                    infos.Remove("size");
    //                }
    //                else if (infos.ContainsKey("page_nb"))
    //                {
    //                    nbPages = int.Parse((string)infos["page_nb"]);
    //                    infos.Remove("page_nb");
    //                }
    //                //Trace.WriteLine("text \"{0}\" => \"{1}\"", s, s2);
    //                bool foundName = false;
    //                if (s2.EndsWith(":"))
    //                {
    //                    string s3 = s2.Substring(0, s2.Length - 1).Trim();
    //                    if (s3 != "")
    //                    {
    //                        name = s3;
    //                        foundName = true;
    //                    }
    //                }
    //                //else if (s2 != "" && s2 != title)
    //                if (!foundName && s2 != "" && s2 != title)
    //                {
    //                    if (text == null)
    //                        text = s2;
    //                    else
    //                        text += " " + s2;
    //                }
    //            }
    //        }
    //        if (text != null)
    //        {
    //            if (name != null)
    //                infos.SetValue(name, new ZString(text));
    //            else
    //                description.Add(text);
    //        }
    //    }
    //}

    //public interface IRapideDdl_Base
    //{
    //    string title { get; }
    //    List<string> description { get; }
    //    string language { get; set; }
    //    string size { get; set; }
    //    int? nbPages { get; set; }
    //    NamedValues<ZValue> infos { get; }
    //}

    //public abstract class RapideDdl_Base
    //{
    //    public abstract string title { get; set; }
    //    public abstract List<string> description { get; set; }
    //    public abstract string language { get; set; }
    //    public abstract string size { get; set; }
    //    public abstract int? nbPages { get; set; }
    //    public abstract NamedValues<ZValue> infos { get; set; }

    //    public void SetTextValues(IEnumerable<string> texts)
    //    {
    //        // read : title
    //        // modify : infos, description, language, size, nbPages

    //        string name = null;
    //        string text = null;
    //        foreach (string s in texts)
    //        {
    //            // PDF | 116 pages | 53 Mb | French
    //            //Trace.CurrentTrace.WriteLine("SetTextValues : \"{0}\"", s);
    //            if (s == "\r\n")
    //            {
    //                if (text != null)
    //                {
    //                    if (name != null)
    //                        infos.SetValue(name, new ZString(text));
    //                    else
    //                        description.Add(text);
    //                    text = null;
    //                }
    //                name = null;
    //            }
    //            else
    //            {
    //                string s2 = RapideDdl.TrimFunc1(RapideDdl.ExtractTextValues(infos, s));
    //                if (infos.ContainsKey("language"))
    //                {
    //                    language = (string)infos["language"];
    //                    infos.Remove("language");
    //                }
    //                else if (infos.ContainsKey("size"))
    //                {
    //                    size = (string)infos["size"];
    //                    infos.Remove("size");
    //                }
    //                else if (infos.ContainsKey("page_nb"))
    //                {
    //                    nbPages = int.Parse((string)infos["page_nb"]);
    //                    infos.Remove("page_nb");
    //                }
    //                //Trace.WriteLine("text \"{0}\" => \"{1}\"", s, s2);
    //                bool foundName = false;
    //                if (s2.EndsWith(":"))
    //                {
    //                    string s3 = s2.Substring(0, s2.Length - 1).Trim();
    //                    if (s3 != "")
    //                    {
    //                        name = s3;
    //                        foundName = true;
    //                    }
    //                }
    //                //else if (s2 != "" && s2 != title)
    //                if (!foundName && s2 != "" && s2 != title)
    //                {
    //                    if (text == null)
    //                        text = s2;
    //                    else
    //                        text += " " + s2;
    //                }
    //            }
    //        }
    //        if (text != null)
    //        {
    //            if (name != null)
    //                infos.SetValue(name, new ZString(text));
    //            else
    //                description.Add(text);
    //        }
    //    }
    //}

    public static class RapideDdl
    {
        private static bool __trace = false;
        private static char[] _trimChars = new char[] { ' ', '\t', '\n', '\r', ',', '»', '&', '+', '/', '|', '*', '=', '»', '.', ':', '_' };
        private static Func<string, string> _trimFunc1 = text => text.Trim(_trimChars);
        private static PrintTextValuesManager _printTextValuesManager = null;
        private static RegexValuesList _textInfoRegexList = null;
        private static WebImageMongoCacheManager_v1 __imageCacheManager = null;
        private static int __imageFilterMinHeight = 0;   // 70
        private static Predicate<ImageMongoCache_v1> __imageFilter = imageCache => (__imageFilterMinHeight == 0 || imageCache.Height > __imageFilterMinHeight) && imageCache.MongoImage.Category != "layout";

        static RapideDdl()
        {
            Init();
        }

        public static void Init()
        {
            InitImage(XmlConfig.CurrentConfig.GetElement("Image"));
            _textInfoRegexList = new RegexValuesList(XmlConfig.CurrentConfig.GetElements("RapideDdl/Parameters/TextInfos/TextInfo"));
            _printTextValuesManager = new PrintTextValuesManager(new RegexValuesList(XmlConfig.CurrentConfig.GetElements("TextInfos/TextInfo")), _trimFunc1);
            //InitImagesToSkip();
        }

        public static void InitImage(XElement xe)
        {
            //if (xe.zXPathValueBool("UseUrlCache", false))
            if (xe.zXPathValue("UseUrlCache").zTryParseAs(false))
                __imageCacheManager = new WebImageMongoCacheManager_v1(xe.zXPathValue("MongoServer"), xe.zXPathValue("MongoDatabase"), xe.zXPathValue("MongoCollection"), xe.zXPathValue("CacheDirectory"));
            __imageFilterMinHeight = xe.zXPathValue("ImageFilterMinHeight").zParseAs<int>();
        }

        public static bool Trace { get { return __trace; } set { __trace = value; } }
        public static PrintTextValuesManager PrintTextValuesManager { get { return _printTextValuesManager; } }

        //public static void LoadImages(List<ImageHtml> images, HttpRequestParameters requestParameters = null)
        //{
        //    for (int i = 0; i < images.Count;)
        //    {
        //        if (!LoadImage(images[i], requestParameters, __imageFilter))
        //            images.RemoveAt(i);
        //        else
        //            i++;
        //    }
        //}

        public static void LoadImages(List<WebImage> images, HttpRequestParameters_v1 requestParameters = null)
        {
            for (int i = 0; i < images.Count; )
            {
                if (!LoadImage(images[i], requestParameters, __imageFilter))
                    images.RemoveAt(i);
                else
                    i++;
            }
        }

        //public static bool LoadImage(ImageHtml image, HttpRequestParameters requestParameters = null, Predicate<ImageMongoCache> filter = null)
        //{
        //    if (image.Source != null && image.Image == null)
        //    {
        //        if (__imageCacheManager != null)
        //        {
        //            ImageMongoCache imageCache = (ImageMongoCache)__imageCacheManager.GetImageCache(image.Source, requestParameters);
        //            if (filter != null && !filter(imageCache))
        //                return false;
        //            image.Image = imageCache.Image;
        //        }
        //        else
        //            image.Image = Http2.LoadImageFromWeb(image.Source, requestParameters);
        //    }
        //    return true;
        //}

        public static bool LoadImage(WebImage image, HttpRequestParameters_v1 requestParameters = null, Predicate<ImageMongoCache_v1> filter = null)
        {
            if (image.Url != null && image.Image == null)
            {
                if (__imageCacheManager != null)
                {
                    ImageMongoCache_v1 imageCache = (ImageMongoCache_v1)__imageCacheManager.GetImageCache(image.Url, requestParameters);
                    if (filter != null && !filter(imageCache))
                        return false;
                    image.Image = imageCache.Image;
                }
                else
                    image.Image = pb.old.Http_v2.LoadImageFromWeb(image.Url, requestParameters);
            }
            return true;
        }

        public static Image LoadImage(string url, HttpRequestParameters_v1 requestParameters = null)
        {
            try
            {
                if (__imageCacheManager != null)
                {
                    ImageCache_v1 imageCache = __imageCacheManager.GetImageCache(url, requestParameters);
                    return imageCache.Image;
                }
                else
                    return pb.old.Http_v2.LoadImageFromWeb(url, requestParameters);
            }
            catch (Exception ex)
            {
                pb.Trace.WriteLine("error RapideDdl loading image : {0}", ex.Message);
                return null;
            }
        }

        public static Func<string, string> TrimFunc1 { get { return _trimFunc1; } }

        //public static void SetTextValues(IRapideDdl_Base data, IEnumerable<string> texts)
        //{
        //    // read : title
        //    // modify : infos, description, language, size, nbPages

        //    string name = null;
        //    string text = null;
        //    foreach (string s in texts)
        //    {
        //        //string s3 = s.Trim();
        //        // PDF | 116 pages | 53 Mb | French
        //        if (__trace)
        //            Trace.CurrentTrace.WriteLine("SetTextValues : \"{0}\"", s);
        //        if (s == "\r\n")
        //        {
        //            //if (text != null)
        //            //{
        //            //    if (name != null)
        //            //    {
        //            //        if (__trace)
        //            //            Trace.CurrentTrace.WriteLine("SetTextValues SetValue : \"{0}\" = \"{1}\"", name, text);
        //            //        data.infos.SetValue(name, new ZString(text));
        //            //    }
        //            //    else
        //            //    {
        //            //        if (__trace)
        //            //            Trace.CurrentTrace.WriteLine("SetTextValues description.Add : \"{0}\"", text);
        //            //        data.description.Add(text);
        //            //    }
        //            //    text = null;
        //            //}
        //            //name = null;
        //            if (name != null)
        //            {
        //                if (__trace)
        //                    Trace.CurrentTrace.WriteLine("SetTextValues SetValue : \"{0}\" = \"{1}\"", name, text);
        //                data.infos.SetValue(name, new ZString(text));
        //            }
        //            else if (text != null)
        //            {
        //                if (__trace)
        //                    Trace.CurrentTrace.WriteLine("SetTextValues description.Add : \"{0}\"", text);
        //                data.description.Add(text);
        //            }
        //            name = null;
        //            text = null;
        //        }
        //        else
        //        {
        //            //string s2 = RapideDdl.TrimFunc1(RapideDdl.ExtractTextValues(data.infos, s));
        //            //if (__trace)
        //            //    Trace.CurrentTrace.WriteLine("SetTextValues ExtractTextValues : \"{0}\" - \"{1}\"", s, s2);
        //            //if (data.infos.ContainsKey("language"))
        //            //{
        //            //    data.language = (string)data.infos["language"];
        //            //    data.infos.Remove("language");
        //            //}
        //            //else if (data.infos.ContainsKey("size"))
        //            //{
        //            //    data.size = (string)data.infos["size"];
        //            //    data.infos.Remove("size");
        //            //}
        //            //else if (data.infos.ContainsKey("page_nb"))
        //            //{
        //            //    data.nbPages = int.Parse((string)data.infos["page_nb"]);
        //            //    data.infos.Remove("page_nb");
        //            //}
        //            //bool foundName = false;
        //            string s2 = s.Trim();
        //            if (s2.EndsWith(":"))
        //            {
        //                string s3 = s2.Substring(0, s2.Length - 1).Trim();
        //                if (s3 != "")
        //                {
        //                    if (name != null)
        //                    {
        //                        if (__trace)
        //                            Trace.CurrentTrace.WriteLine("SetTextValues SetValue : \"{0}\" = \"{1}\"", name, text);
        //                        data.infos.SetValue(name, new ZString(text));
        //                    }
        //                    else if (text != null)
        //                    {
        //                        if (__trace)
        //                            Trace.CurrentTrace.WriteLine("SetTextValues description.Add : \"{0}\"", text);
        //                        data.description.Add(text);
        //                    }
        //                    name = s3;
        //                    text = null;
        //                    //foundName = true;
        //                    if (__trace)
        //                        Trace.CurrentTrace.WriteLine("SetTextValues Set name \"{0}\"", name);
        //                }
        //                else
        //                {
        //                    if (__trace)
        //                        Trace.CurrentTrace.WriteLine("SetTextValues Nothing to do");
        //                }
        //            }
        //            else
        //            {
        //                s2 = RapideDdl.TrimFunc1(RapideDdl.ExtractTextValues(data.infos, s));
        //                if (__trace)
        //                    Trace.CurrentTrace.WriteLine("SetTextValues ExtractTextValues : \"{0}\" - \"{1}\"", s, s2);
        //                if (data.infos.ContainsKey("language"))
        //                {
        //                    data.language = (string)data.infos["language"];
        //                    data.infos.Remove("language");
        //                }
        //                else if (data.infos.ContainsKey("size"))
        //                {
        //                    data.size = (string)data.infos["size"];
        //                    data.infos.Remove("size");
        //                }
        //                else if (data.infos.ContainsKey("page_nb"))
        //                {
        //                    data.nbPages = int.Parse((string)data.infos["page_nb"]);
        //                    data.infos.Remove("page_nb");
        //                }

        //                if (s2 != "" && s2 != data.title)
        //                {
        //                    if (text == null)
        //                        text = s2;
        //                    else
        //                        text += " " + s2;
        //                    if (__trace)
        //                        Trace.CurrentTrace.WriteLine("SetTextValues Add string to text : \"{0}\"", text);
        //                }
        //                else
        //                {
        //                    if (__trace)
        //                        Trace.CurrentTrace.WriteLine("SetTextValues dont Add string to text");
        //                }
        //            }
        //            //else if (s2 != "" && s2 != title)
        //            //if (!foundName && s2 != "" && s2 != data.title)
        //            //{
        //            //    if (text == null)
        //            //        text = s2;
        //            //    else
        //            //        text += " " + s2;
        //            //    if (__trace)
        //            //        Trace.CurrentTrace.WriteLine("SetTextValues Add string to text : \"{0}\"", text);
        //            //}
        //            //else
        //            //{
        //            //    if (__trace)
        //            //        Trace.CurrentTrace.WriteLine("SetTextValues dont Add string to text");
        //            //}
        //        }
        //    }
        //    if (text != null)
        //    {
        //        if (name != null)
        //        {
        //            if (__trace)
        //                Trace.CurrentTrace.WriteLine("SetTextValues SetValue : \"{0}\" = \"{1}\"", name, text);
        //            data.infos.SetValue(name, new ZString(text));
        //        }
        //        else
        //        {
        //            if (__trace)
        //                Trace.CurrentTrace.WriteLine("SetTextValues description.Add : \"{0}\"", text);
        //            data.description.Add(text);
        //        }
        //    }
        //}

        //public static string ExtractTextValues(NamedValues<ZValue> infos, string s)
        //{
        //    // French | PDF | 107 MB -*- French | PDF |  22 Pages | 7 MB -*- PDF | 116 pages | 53 Mb | French -*- Micro Application | 2010 | ISBN: 2300028441 | 221 pages | PDF
        //    // pb : |I|N|F|O|S|, |S|Y|N|O|P|S|I|S|, |T|E|L|E|C|H|A|R|G|E|R|
        //    // example http://www.telechargement-plus.com/e-book-magazines/bande-dessines/136846-season-one-100-marvel-syrie-en-cours-10-tomes-comicmulti.html
        //    if (s.Contains('|'))
        //    {
        //        //Trace.CurrentTrace.WriteLine("info \"{0}\"", s);
        //        foreach (string s2 in zsplit.Split(s, '|', true))
        //        {
        //            string s3 = s2;
        //            NamedValues<ZValue> values = _textInfoRegexList.ExtractTextValues(ref s3);
        //            infos.SetValues(values);
        //            s3 = s3.Trim();
        //            if (s3 != "")
        //                infos.SetValue(s3, null);
        //        }
        //        return "";
        //    }
        //    else
        //    {
        //        NamedValues<ZValue> values = _textInfoRegexList.ExtractTextValues(ref s);
        //        infos.SetValues(values);
        //        return s;
        //    }
        //}

        //private static Regex _rgDate = new Regex("(aujourd'hui|hier)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        //public static DateTime? ParseDateTime(string date, DateTime loadFromWebDate)
        //{
        //    // Aujourd'hui, 17:13
        //    if (date == null)
        //        return null;
        //    Match match = _rgDate.Match(date);
        //    if (match.Success)
        //    {
        //        string s = null;
        //        switch (match.Groups[1].Value.ToLower())
        //        {
        //            case "aujourd'hui":
        //                //s = DateTime.Today.ToString("dd-MM-yyyy");
        //                s = loadFromWebDate.ToString("dd-MM-yyyy");
        //                break;
        //            case "hier":
        //                //s = DateTime.Today.AddDays(-1).ToString("dd-MM-yyyy");
        //                s = loadFromWebDate.AddDays(-1).ToString("dd-MM-yyyy");
        //                break;
        //        }
        //        date = match.zReplace(date, s);
        //    }
        //    // unknow date time "9-09-2014, 23:25"
        //    DateTime dt;
        //    //if (DateTime.TryParseExact(date, @"dd-MM-yyyy, HH:mm", CultureInfo.CurrentCulture, DateTimeStyles.None, out dt))
        //    // "d-M-yyyy, HH:mm" work with "9-9-2014, 23:25" and "09-09-2014, 23:25"
        //    if (DateTime.TryParseExact(date, @"d-M-yyyy, HH:mm", CultureInfo.CurrentCulture, DateTimeStyles.None, out dt))
        //        return dt;
        //    else
        //    {
        //        Trace.WriteLine("unknow date time \"{0}\"", date);
        //        return null;
        //    }
        //}
    }
}
