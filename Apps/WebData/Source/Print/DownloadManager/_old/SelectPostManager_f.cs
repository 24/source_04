using System;
using MongoDB.Bson;
using MongoDB.Driver;
using pb;
using pb.Compiler;
using pb.Data;
using pb.Data.Xml;
using pb.Text;

namespace Download.Print.old
{
    public static class SelectPostManager_f
    {
        //public static void Test_SelectPostManager_01(string query, int limit = 0)
        //{
        //    SelectPostManager selectPostManager = new SelectPostManager(XmlConfig.CurrentConfig.GetElement("SelectPosts"));
        //    MongoCursor<BsonDocument> cursor = pb.Data.Mongo.MongoCommand.Find("dl", "RapideDdl_Detail2", query, fields: "{ 'download.title': 1 }", limit: limit, sort: "{ 'download.creationDate': -1 }");
        //    foreach (BsonDocument document in cursor)
        //    {
        //        string title = document["download"]["title"].AsString;
        //        //Trace.Write("FindDate : {0,-100}", "\"" + title + "\"");
        //        Trace.WriteLine("FindDate : \"{0}\"", title);
        //        selectPostManager.FindDate(title);
        //    }
        //}

        public static void Test_FindDate_01(string query, int limit = 0)
        {
            //SelectPostManager selectPostManager = new SelectPostManager(XmlConfig.CurrentConfig.GetElement("SelectPosts"));
            // zElements("Dates/Date")
            RegexValuesList dateRegexList = new RegexValuesList(XmlConfig.CurrentConfig.GetElement("SelectPosts").zXPathElements("Dates/Date"), compileRegex: true);
            MongoCursor<BsonDocument> cursor = pb.Data.Mongo.MongoCommand.Find("dl", "RapideDdl_Detail2", query, fields: "{ 'download.title': 1 }", limit: limit, sort: "{ 'download.creationDate': -1 }");
            foreach (BsonDocument document in cursor)
            {
                string title = document["download"]["title"].AsString;
                //Trace.Write("FindDate : {0,-100}", "\"" + title + "\"");
                Trace.WriteLine("FindDate : \"{0}\"", title);
                FindDate(dateRegexList, title);
                Trace.WriteLine();
            }
        }

        public static void FindDate(RegexValuesList dateRegexList, string title)
        {
            bool found = false;
            foreach (RegexValues rv in dateRegexList.Values)
            {
                //rv.Match_old(title);
                MatchValues matchValues = rv.Match(title);
                //if (rv.Success_old)
                if (matchValues.Success)
                {
                    //NamedValues<ZValue> values = rv.GetValues_old();
                    NamedValues<ZValue> values = matchValues.GetValues();
                    Date date;
                    DateType dateType;
                    //Trace.Write(" date ");
                    //found = zdate.TryCreateDate(values, out date);
                    //if (found)
                    //    Trace.Write("{0:dd-MM-yyyy}", date);
                    //else
                    //    Trace.Write("not found ");
                    //Trace.Write("   ");
                    //values.zTrace();

                    //Trace.WriteLine("    found date  : {0}", rv.MatchValue_old);
                    Trace.WriteLine("    found date  : {0}", matchValues.Match.Value);

                    //Trace.WriteLine("    remain text : {0}", rv.MatchReplace_old("_"));
                    Trace.WriteLine("    remain text : {0}", matchValues.Replace("_"));

                    Trace.Write("    date        : ");
                    found = zdate.TryCreateDate(values, out date, out dateType);
                    if (found)
                        Trace.Write("{0:dd-MM-yyyy} type {1}", date, dateType);
                    else
                        Trace.Write("not found ");
                    Trace.WriteLine();

                    Trace.Write("    values      : ");
                    values.zTrace();
                    Trace.WriteLine();

                    if (found)
                        break;
                }
            }
            if (!found)
                Trace.WriteLine("    date not found ");
        }
    }
}
