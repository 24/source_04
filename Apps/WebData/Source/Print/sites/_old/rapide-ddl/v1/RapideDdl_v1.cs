using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using MongoDB.Bson.Serialization;
using pb.Data;

namespace Download.Print.RapideDdl.v1
{
    public class RapideDdl_Base
    {
        public string title = null;
        public List<string> description = new List<string>();
        public string language = null;
        public string size = null;
        public int? nbPages = null;
        public NamedValues<ZValue> infos = new NamedValues<ZValue>();

        public void SetTextValues(IEnumerable<string> texts)
        {
            // read : title
            // modify : infos, description, language, size, nbPages

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
                    //////////////////////////////////////string s2 = Download.Print.RapideDdl.RapideDdl.TrimFunc1(Download.Print.RapideDdl.RapideDdl.ExtractTextValues(infos, s));
                    string s2 = null;
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

    public static class RapideDdl
    {
        public static void InitMongoClassMap()
        {
            // Register all class derived from ZValue to deserialize field NamedValues<ZValue> infos (RapideDdl_Base)
            //   ZString, ZStringArray, ZInt
            if (!BsonClassMap.IsClassMapRegistered(typeof(ZString)))
                BsonClassMap.RegisterClassMap<ZString>();
            if (!BsonClassMap.IsClassMapRegistered(typeof(ZStringArray)))
                BsonClassMap.RegisterClassMap<ZStringArray>();
            if (!BsonClassMap.IsClassMapRegistered(typeof(ZInt)))
                BsonClassMap.RegisterClassMap<ZInt>();
            if (!BsonClassMap.IsClassMapRegistered(typeof(pb.old.ImageHtml)))
            {
                BsonClassMap.RegisterClassMap<pb.old.ImageHtml>(cm => { cm.AutoMap(); cm.UnmapProperty(c => c.Image); });
            }
        }

        // bool desactivateDocumentStore = false
        public static IEnumerable<RapideDdl_PostDetail> LoadDetailItemList(int startPage = 1, int maxPage = 1, bool reloadHeaderPage = false, bool reloadDetail = false, bool loadImage = false,
            bool refreshDocumentStore = false)
        {
            return from header in RapideDdl_LoadHeaderPages.LoadHeaderPages(startPage, maxPage, reloadHeaderPage, false) select LoadDetailItem(header, reloadDetail, loadImage, refreshDocumentStore);
        }

        // bool desactivateDocumentStore = false
        public static RapideDdl_PostDetail LoadDetailItem(RapideDdl_PostHeader header, bool reload = false, bool loadImage = false, bool refreshDocumentStore = false)
        {
            return RapideDdl_LoadDetail.Load(header.urlDetail, reload: reload, loadImage: loadImage, refreshDocumentStore: refreshDocumentStore);
        }
    }
}
