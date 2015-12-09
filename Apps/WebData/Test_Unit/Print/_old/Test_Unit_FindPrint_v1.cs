using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using pb;
using pb.Compiler;
using pb.Data.Mongo;
using pb.IO;
using Print;
using Download.Print;
using pb.Data.Xml;

namespace Test.Test_Unit.Print
{
    //public class TestPrint_old
    //{
    //    public string title;
    //    public string category;
    //    public bool isPrint;
    //    public NamedValues<ZValue> infos;
    //}

    public class TestFindPrint_v1
    {
        public string title;
        public string category;
        public bool isPrint;
        public FindPrintInfo findPrint;
    }

    public static class Test_Unit_SelectPost_v1
    {
        private static Regex __rgSpecial = new Regex(@"hors[-\s]*s.rie", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        //public static void Test_SelectPost_01(DownloadAutomateManager downloadAutomate, string file)
        //{
        //    string traceFile = zpath.PathSetFile(file, zPath.GetFileNameWithoutExtension(file) + "_out");
        //    Trace.WriteLine("Test_SelectPost \"{0}\"", file);
        //    Trace.CurrentTrace.DisableBaseLog();
        //    Trace.CurrentTrace.AddTraceFile(traceFile, LogOptions.RazLogFile);
        //    try
        //    {
        //        foreach (BsonDocument document in zmongo.BsonReader<BsonDocument>(file))
        //        {
        //            string title = document["text"].AsString;
        //            //string postFile = downloadAutomate.TestSelectPost2(title);
        //            //FindPrint findPrint = downloadAutomate.FindPrintManager.Find(title);
        //            FindPrint findPrint = downloadAutomate.SelectPost(title);
        //            Trace.WriteLine("post            : \"{0}\"", title);
        //            Trace.WriteLine("    file        : \"{0}\"", findPrint.file);
        //            Trace.WriteLine("    remain text : \"{0}\"", findPrint.found ? findPrint.remainText : null);
        //            Trace.WriteLine();
        //        }
        //    }
        //    finally
        //    {
        //        Trace.CurrentTrace.EnableBaseLog();
        //        Trace.CurrentTrace.RemoveTraceFile(traceFile);
        //    }
        //}

        //public static void Test_SelectPost_02(DownloadAutomateManager downloadAutomate, string file)
        //{
        //    Trace.WriteLine("Test_SelectPost (downloadAutomate.SelectPost()) \"{0}\"", file);
        //    Trace.WriteLine("  FindPrintManager.PostRegexList  : {0}", downloadAutomate.FindPrintManager.PrintRegexList.Count);
        //    file = zPath.Combine(GetDirectory(), file);
        //    string bsonFile = zpath.PathSetFile(file, zPath.GetFileNameWithoutExtension(file) + "_out_bson");
        //    Trace.CurrentTrace.DisableBaseLog();
        //    StreamWriter sw = null;
        //    BsonWriter bsonWriter = null;
        //    List<string> messages = new List<string>();
        //    WritedEvent getTrace = msg => messages.Add(msg.TrimEnd('\r', '\n'));
        //    Trace.CurrentTrace.WriteToTraceFiles += getTrace;
        //    try
        //    {
        //        sw = File.CreateText(bsonFile);
        //        JsonWriterSettings jsonSettings = new JsonWriterSettings();
        //        jsonSettings.Indent = true;
        //        bsonWriter = JsonWriter.Create(sw, jsonSettings);
        //        foreach (BsonDocument document in zmongo.BsonReader<BsonDocument>(file))
        //        {
        //            //string title = document["text"].AsString;
        //            string title = document["title"].AsString;
        //            string category = document["category"].AsString;
        //            FindPrint findPrint = downloadAutomate.FindPrint(title, category);
        //            bsonWriter.WriteStartDocument();
        //            bsonWriter.zWrite("postTitle", title);
        //            bsonWriter.zWrite("category", category);
        //            bsonWriter.zWrite("file", findPrint.file);
        //            bsonWriter.zWrite("print", findPrint.print != null ? findPrint.print.Name : null);
        //            bsonWriter.zWrite("title", findPrint.print != null ? findPrint.print.Title : findPrint.title);
        //            bsonWriter.zWrite("remainText", findPrint.remainText);
        //            bsonWriter.WriteStartArray("warnings");
        //            foreach (string message in messages)
        //                bsonWriter.WriteString(message);
        //            messages.Clear();
        //            bsonWriter.WriteEndArray();
        //            bsonWriter.WriteEndDocument();
        //            sw.WriteLine();
        //            bsonWriter.WriteName("fake");    // ??? pour éviter l'erreur : WriteString can only be called when State is Value or Initial, not when State is Name (System.InvalidOperationException)
        //        }
        //    }
        //    finally
        //    {
        //        Trace.CurrentTrace.WriteToTraceFiles -= getTrace;
        //        Trace.CurrentTrace.EnableBaseLog();
        //        if (bsonWriter != null)
        //            bsonWriter.Close();
        //        if (sw != null)
        //            sw.Close();
        //    }

        //    WriteSelectPost_old(zpath.PathSetFile(file, zPath.GetFileNameWithoutExtension(file) + "_out"), zmongo.BsonReader<BsonDocument>(bsonFile));
        //    WriteSelectPost_old(zpath.PathSetFile(file, zPath.GetFileNameWithoutExtension(file) + "_out_hors_serie"), from doc in zmongo.BsonReader<BsonDocument>(bsonFile) where __rgSpecial.IsMatch(doc["postTitle"].zAsString()) select doc);
        //    WriteSelectPost_old(zpath.PathSetFile(file, zPath.GetFileNameWithoutExtension(file) + "_out_remain_text"), from doc in zmongo.BsonReader<BsonDocument>(bsonFile) where !string.IsNullOrEmpty(doc["remainText"].zAsString()) select doc);
        //    WriteSelectPost_old(zpath.PathSetFile(file, zPath.GetFileNameWithoutExtension(file) + "_out_warning"), from doc in zmongo.BsonReader<BsonDocument>(bsonFile) where doc["warnings"].AsBsonArray.Count > 0 select doc);
        //    WriteSelectPost_old(zpath.PathSetFile(file, zPath.GetFileNameWithoutExtension(file) + "_out_no_title"), from doc in zmongo.BsonReader<BsonDocument>(bsonFile) where doc["file"].zAsString() != null && doc["title"].zAsString() == null select doc);
        //    WriteSelectPost_NotSelected_old(zpath.PathSetFile(file, zPath.GetFileNameWithoutExtension(file) + "_out_not_selected"), from doc in zmongo.BsonReader<BsonDocument>(bsonFile) where doc["file"].zAsString() == null select doc);
        //}

        //public static void Test_SelectPost_03(DownloadAutomateManager downloadAutomate, string file)
        //{
        //    Trace.WriteLine("Test_SelectPost \"{0}\"", file);
        //    if (downloadAutomate.FindPrintManager != null)
        //        Trace.WriteLine("  FindPrintManager.PostRegexList  : {0}", downloadAutomate.FindPrintManager.PrintRegexList.Count);
        //    if (downloadAutomate.FindPrintManager_new != null)
        //        Trace.WriteLine("  FindPrintManager2.PrintRegexList  : {0}", downloadAutomate.FindPrintManager_new.PrintRegexList.Count);
        //    file = zPath.Combine(GetDirectory(), file);
        //    string bsonFile = zpath.PathSetFile(file, zPath.GetFileNameWithoutExtension(file) + "_out_bson");
        //    Trace.CurrentTrace.DisableBaseLog();
        //    StreamWriter sw = null;
        //    BsonWriter bsonWriter = null;
        //    List<string> messages = new List<string>();
        //    WritedEvent getTrace = msg => messages.Add(msg.TrimEnd('\r', '\n'));
        //    Trace.CurrentTrace.WriteToTraceFiles += getTrace;
        //    try
        //    {
        //        sw = File.CreateText(bsonFile);
        //        JsonWriterSettings jsonSettings = new JsonWriterSettings();
        //        jsonSettings.Indent = true;
        //        bsonWriter = JsonWriter.Create(sw, jsonSettings);
        //        foreach (BsonDocument document in zmongo.BsonReader<BsonDocument>(file))
        //        {
        //            string title = document["title"].AsString;
        //            string category = document["category"].AsString;
        //            bool isPrint = document["isPrint"].AsBoolean;
        //            //FindPrint findPrint = downloadAutomate.SelectPost(title, category);
        //            FindPrint findPrint = downloadAutomate.FindPrint_new(title, isPrint);
        //            bsonWriter.WriteStartDocument();
        //            bsonWriter.zWrite("postTitle", title);
        //            bsonWriter.zWrite("category", category);
        //            bsonWriter.zWrite("isPrint", isPrint);
        //            bsonWriter.zWrite("file", findPrint.file);
        //            bsonWriter.zWrite("findPrintType", (int)findPrint.findPrintType);
        //            bsonWriter.zWrite("name", findPrint.name);
        //            bsonWriter.zWrite("print", findPrint.print != null ? findPrint.print.Name : null);
        //            bsonWriter.zWrite("title", findPrint.print != null ? findPrint.print.Title : findPrint.title);
        //            bsonWriter.zWrite("remainText", findPrint.remainText);
        //            bsonWriter.WriteStartArray("warnings");
        //            foreach (string message in messages)
        //                bsonWriter.WriteString(message);
        //            messages.Clear();
        //            bsonWriter.WriteEndArray();
        //            bsonWriter.WriteEndDocument();
        //            sw.WriteLine();
        //            bsonWriter.WriteName("fake");    // ??? pour éviter l'erreur : WriteString can only be called when State is Value or Initial, not when State is Name (System.InvalidOperationException)
        //        }
        //    }
        //    finally
        //    {
        //        Trace.CurrentTrace.WriteToTraceFiles -= getTrace;
        //        Trace.CurrentTrace.EnableBaseLog();
        //        if (bsonWriter != null)
        //            bsonWriter.Close();
        //        if (sw != null)
        //            sw.Close();
        //    }

        //    WriteSelectPost_old(zpath.PathSetFile(file, zPath.GetFileNameWithoutExtension(file) + "_out"), zmongo.BsonReader<BsonDocument>(bsonFile), old: true);
        //    WriteSelectPost_old(zpath.PathSetFile(file, zPath.GetFileNameWithoutExtension(file) + "_out_new"), zmongo.BsonReader<BsonDocument>(bsonFile));
        //    WriteSelectPost_old(zpath.PathSetFile(file, zPath.GetFileNameWithoutExtension(file) + "_out_hors_serie"), from doc in zmongo.BsonReader<BsonDocument>(bsonFile) where __rgSpecial.IsMatch(doc["postTitle"].zAsString()) select doc);
        //    WriteSelectPost_old(zpath.PathSetFile(file, zPath.GetFileNameWithoutExtension(file) + "_out_remain_text"), from doc in zmongo.BsonReader<BsonDocument>(bsonFile) where !string.IsNullOrEmpty(doc["remainText"].zAsString()) select doc);
        //    WriteSelectPost_old(zpath.PathSetFile(file, zPath.GetFileNameWithoutExtension(file) + "_out_warning"), from doc in zmongo.BsonReader<BsonDocument>(bsonFile) where doc["warnings"].AsBsonArray.Count > 0 select doc);
        //    WriteSelectPost_old(zpath.PathSetFile(file, zPath.GetFileNameWithoutExtension(file) + "_out_no_title"), from doc in zmongo.BsonReader<BsonDocument>(bsonFile) where doc["file"].zAsString() != null && doc["title"].zAsString() == null select doc);
        //    WriteSelectPost_NotSelected_old(zpath.PathSetFile(file, zPath.GetFileNameWithoutExtension(file) + "_out_not_selected"), from doc in zmongo.BsonReader<BsonDocument>(bsonFile) where doc["file"].zAsString() == null select doc);
        //}

        //public static void Test_SelectPost_04(DownloadAutomateManager downloadAutomate, string file)
        //{
        //    Trace.WriteLine("Test_SelectPost \"{0}\"", file);
        //    if (downloadAutomate.FindPrintManager != null)
        //        Trace.WriteLine("  FindPrintManager.PostRegexList  : {0}", downloadAutomate.FindPrintManager.PrintRegexList.Count);
        //    if (downloadAutomate.FindPrintManager_new != null)
        //        Trace.WriteLine("  FindPrintManager2.PrintRegexList  : {0}", downloadAutomate.FindPrintManager_new.PrintRegexList.Count);
        //    file = zPath.Combine(GetDirectory(), file);
        //    //zmongo.BsonReader<BsonDocument>(file).zFindPrint(downloadAutomate).zSaveFindPrint_old(file);
        //    zmongo.BsonReader<BsonDocument>(file).zFindPrint(downloadAutomate).zSaveFindPrint(file);
        //}

        //public static void Test_SelectPostFromMongo_03(DownloadAutomateManager downloadAutomate, string query, int limit = 0, string sort = null)
        //{
        //    if (sort == null)
        //        sort = "{ 'download.title': 1 }";
        //    //pb.Data.Mongo.TraceMongoCommand.Find("dl", "RapideDdl_Detail2", "{ 'download.title': /.*parisien.*/i }", limit: 0, sort: "{ 'download.title': 1 }", fields: "{ '_id': 1, 'download.title': 1, 'download.creationDate': 1, 'download.category': 1, 'download.isPrint': 1, 'download.sourceUrl': 1  }");
        //    var query2 = pb.Data.Mongo.MongoCommand.Find("dl", "RapideDdl_Detail2", "{ 'download.title': /.*parisien.*/i }", limit: limit, sort: sort, fields: "{ '_id': 1, 'download.title': 1, 'download.creationDate': 1, 'download.category': 1, 'download.isPrint': 1, 'download.sourceUrl': 1  }");
        //    var query3 = query2.Select(document =>
        //    {
        //        string title = document["download"]["title"].AsString;
        //        string category = document["download"]["category"].AsString;
        //        bool isPrint = document["download"]["isPrint"].AsBoolean;
        //        FindPrint findPrint = downloadAutomate.FindPrint_new(title, isPrint);

        //        return new
        //        {
        //            post_title = title,
        //            post_category = category,
        //            post_isPrint = isPrint,
        //            titleInfo_formatedTitle = findPrint.titleInfo.formatedTitle,
        //            findPrint_file = findPrint.file,
        //            findPrint_type = findPrint.findPrintType,
        //            findPrint_name = findPrint.name,
        //            print_name = findPrint.print != null ? findPrint.print.Name : null,
        //            print_title = findPrint.print != null ? findPrint.print.Title : null,
        //            //findPrint_title = findPrint.title,
        //            findPrint_remainText = findPrint.remainText
        //        };


        //    });
        //    RunSource.CurrentRunSource.View(query3);
        //    //RunSource.CurrentRunSource.SetResult(pb.Data.Mongo.BsonDocumentsToDataTable_old2.ToDataTable(query2));
        //}

        //public static void Test_SelectPostFromMongo_04(DownloadAutomateManager downloadAutomate, string file, string query, int limit = 0, string sort = null)
        //{
        //    //Trace.WriteLine("regex le_parisien : \"{0}\"", downloadAutomate.FindPrintManager2.PrintRegexList["le_parisien"].Pattern);
        //    Trace.WriteLine("regex le_monde : \"{0}\"", downloadAutomate.FindPrintManager_new.PrintRegexList["le_monde"].Pattern);
        //    file = zPath.Combine(GetDirectory(), file);
        //    string bsonFile = FindPrintFromMongo(downloadAutomate, query, limit, sort).zFindPrint(downloadAutomate).zSaveFindPrint(file);
        //    //var query1 = FindPrintFromMongo(downloadAutomate, query, limit, sort);
        //    //var query2 = query1.zFindPrint(downloadAutomate).Select(findPrint =>
        //    //        new
        //    //        {
        //    //            post_title = findPrint.title,
        //    //            post_category = findPrint.category,
        //    //            post_isPrint = findPrint.isPrint,
        //    //            titleInfo_formatedTitle = findPrint.findPrint.titleInfo.formatedTitle,
        //    //            findPrint_file = findPrint.findPrint.file,
        //    //            findPrint_type = findPrint.findPrint.findPrintType,
        //    //            findPrint_name = findPrint.findPrint.name,
        //    //            print_name = findPrint.findPrint.print != null ? findPrint.findPrint.print.Name : null,
        //    //            print_title = findPrint.findPrint.print != null ? findPrint.findPrint.print.Title : null,
        //    //            findPrint_remainText = findPrint.findPrint.remainText
        //    //        }
        //    //    );
        //    var query2 = zmongo.BsonReader<BsonDocument>(bsonFile).Select(document =>
        //            new
        //            {
        //                post_title = document["post_title"].zAsString(),
        //                post_category = document["post_category"].zAsString(),
        //                post_isPrint = document["post_isPrint"].AsBoolean,
        //                titleInfo_formatedTitle = document["titleInfo_formatedTitle"].zAsString(),
        //                findPrint_file = document["findPrint_file"].zAsString(),
        //                findPrint_type = (FindPrintType)document["findPrint_type"].AsInt32,
        //                findPrint_name = document["findPrint_name"].zAsString(),
        //                print_name = document["print_name"].zAsString(),
        //                print_title = document["print_title"].zAsString(),
        //                findPrint_remainText = document["findPrint_remainText"].zAsString()
        //            }
        //        );
        //    RunSource.CurrentRunSource.View(query2);
        //}

        public static IEnumerable<BsonDocument> FindPrintFromMongo(DownloadAutomateManager downloadAutomate, string query, int limit = 0, string sort = null)
        {
            if (sort == null)
                sort = "{ 'download.title': 1 }";
            var query2 = pb.Data.Mongo.MongoCommand.Find("dl", "RapideDdl_Detail2", query, limit: limit, sort: sort, fields: "{ '_id': 1, 'download.title': 1, 'download.creationDate': 1, 'download.category': 1, 'download.isPrint': 1, 'download.sourceUrl': 1  }");
            var query3 = query2.Select(document =>
                new BsonDocument
                {
                    { "title", document["download"]["title"].AsString },
                    { "category", document["download"]["category"].AsString },
                    { "isPrint", document["download"]["isPrint"].AsBoolean }
                }
            );
            return query3;
        }

        //public static IEnumerable<TestFindPrint_old> zFindPrint(this IEnumerable<BsonDocument> documents, DownloadAutomateManager downloadAutomate)
        //{
        //    foreach (BsonDocument document in documents)
        //    {
        //        string title = document["title"].AsString;
        //        string category = document["category"].AsString;
        //        bool isPrint = document["isPrint"].AsBoolean;
        //        //FindPrint findPrint = downloadAutomate.SelectPost(title, category);
        //        FindPrint findPrint = downloadAutomate.FindPrint_new(title, isPrint);
        //        yield return new TestFindPrint_old { title = title, category = category, isPrint = isPrint, findPrint = findPrint };
        //    }
        //}

        public static void zSaveFindPrint_old(this IEnumerable<TestFindPrint_v1> findPrintList, string file)
        {
            //Trace.CurrentTrace.DisableBaseLog();
            Trace.CurrentTrace.DisableViewer = true;
            string bsonFile = zpath.PathSetFileNameWithoutExtension(file, zPath.GetFileNameWithoutExtension(file) + "_out_bson");
            StreamWriter sw = null;
            BsonWriter bsonWriter = null;
            List<string> messages = new List<string>();
            //WritedEvent getTrace = msg => messages.Add(msg.TrimEnd('\r', '\n'));
            //Trace.CurrentTrace.WriteToTraceFiles += getTrace;
            Trace.CurrentTrace.AddOnWrite("zSaveFindPrint_old", msg => messages.Add(msg.TrimEnd('\r', '\n')));
            try
            {
                sw = zFile.CreateText(bsonFile);
                JsonWriterSettings jsonSettings = new JsonWriterSettings();
                jsonSettings.Indent = true;
                bsonWriter = JsonWriter.Create(sw, jsonSettings);
                //foreach (BsonDocument document in zmongo.BsonReader<BsonDocument>(file))
                foreach (TestFindPrint_v1 findPrint in findPrintList)
                {
                    bsonWriter.WriteStartDocument();
                    bsonWriter.zWrite("postTitle", findPrint.title);
                    bsonWriter.zWrite("category", findPrint.category);
                    bsonWriter.zWrite("isPrint", findPrint.isPrint);
                    bsonWriter.zWrite("file", findPrint.findPrint.file);
                    bsonWriter.zWrite("findPrintType", (int)findPrint.findPrint.findPrintType);
                    bsonWriter.zWrite("name", findPrint.findPrint.name);
                    bsonWriter.zWrite("print", findPrint.findPrint.print != null ? findPrint.findPrint.print.Name : null);
                    bsonWriter.zWrite("title", findPrint.findPrint.print != null ? findPrint.findPrint.print.Title : findPrint.findPrint.title);
                    bsonWriter.zWrite("remainText", findPrint.findPrint.remainText);
                    bsonWriter.WriteStartArray("warnings");
                    foreach (string message in messages)
                        bsonWriter.WriteString(message);
                    messages.Clear();
                    bsonWriter.WriteEndArray();
                    bsonWriter.WriteEndDocument();
                    sw.WriteLine();
                    bsonWriter.WriteName("fake");    // ??? pour éviter l'erreur : WriteString can only be called when State is Value or Initial, not when State is Name (System.InvalidOperationException)
                }
            }
            finally
            {
                //Trace.CurrentTrace.WriteToTraceFiles -= getTrace;
                Trace.CurrentTrace.RemoveOnWrite("zSaveFindPrint_old");
                Trace.CurrentTrace.DisableViewer = false;
                if (bsonWriter != null)
                    bsonWriter.Close();
                if (sw != null)
                    sw.Close();
            }

            WriteSelectPost_old(zpath.PathSetFileNameWithoutExtension(file, zPath.GetFileNameWithoutExtension(file) + "_out"), zmongo.FileReader<BsonDocument>(bsonFile), old: true);
            WriteSelectPost_old(zpath.PathSetFileNameWithoutExtension(file, zPath.GetFileNameWithoutExtension(file) + "_out_new"), zmongo.FileReader<BsonDocument>(bsonFile));
            WriteSelectPost_old(zpath.PathSetFileNameWithoutExtension(file, zPath.GetFileNameWithoutExtension(file) + "_out_hors_serie"), from doc in zmongo.FileReader<BsonDocument>(bsonFile) where __rgSpecial.IsMatch(doc["postTitle"].zAsString()) select doc);
            WriteSelectPost_old(zpath.PathSetFileNameWithoutExtension(file, zPath.GetFileNameWithoutExtension(file) + "_out_remain_text"), from doc in zmongo.FileReader<BsonDocument>(bsonFile) where !string.IsNullOrEmpty(doc["remainText"].zAsString()) select doc);
            WriteSelectPost_old(zpath.PathSetFileNameWithoutExtension(file, zPath.GetFileNameWithoutExtension(file) + "_out_warning"), from doc in zmongo.FileReader<BsonDocument>(bsonFile) where doc["warnings"].AsBsonArray.Count > 0 select doc);
            WriteSelectPost_old(zpath.PathSetFileNameWithoutExtension(file, zPath.GetFileNameWithoutExtension(file) + "_out_no_title"), from doc in zmongo.FileReader<BsonDocument>(bsonFile) where doc["file"].zAsString() != null && doc["title"].zAsString() == null select doc);
            WriteSelectPost_NotSelected_old(zpath.PathSetFileNameWithoutExtension(file, zPath.GetFileNameWithoutExtension(file) + "_out_not_selected"), from doc in zmongo.FileReader<BsonDocument>(bsonFile) where doc["file"].zAsString() == null select doc);
        }

        public static string zSaveFindPrint(this IEnumerable<TestFindPrint_v1> findPrintList, string file)
        {
            //Trace.CurrentTrace.DisableBaseLog();
            Trace.CurrentTrace.DisableViewer = true;
            string bsonFile = zpath.PathSetFileNameWithoutExtension(file, zPath.GetFileNameWithoutExtension(file) + "_out_bson_new");
            StreamWriter sw = null;
            BsonWriter bsonWriter = null;
            List<string> messages = new List<string>();
            //WritedEvent getTrace = msg => messages.Add(msg.TrimEnd('\r', '\n'));
            //Trace.CurrentTrace.WriteToTraceFiles += getTrace;
            Trace.CurrentTrace.AddOnWrite("zSaveFindPrint", msg => messages.Add(msg.TrimEnd('\r', '\n')));
            try
            {
                sw = zFile.CreateText(bsonFile);
                JsonWriterSettings jsonSettings = new JsonWriterSettings();
                jsonSettings.Indent = true;
                bsonWriter = JsonWriter.Create(sw, jsonSettings);
                //foreach (BsonDocument document in zmongo.BsonReader<BsonDocument>(file))
                foreach (TestFindPrint_v1 findPrint in findPrintList)
                {
                    bsonWriter.WriteStartDocument();
                    bsonWriter.zWrite("post_title", findPrint.title);
                    bsonWriter.zWrite("post_category", findPrint.category);
                    bsonWriter.zWrite("post_isPrint", findPrint.isPrint);
                    bsonWriter.zWrite("findPrint_file", findPrint.findPrint.file);
                    bsonWriter.zWrite("findPrint_type", (int)findPrint.findPrint.findPrintType);
                    bsonWriter.zWrite("findPrint_name", findPrint.findPrint.name);
                    bsonWriter.zWrite("findPrint_title", findPrint.findPrint.title);
                    bsonWriter.zWrite("print_name", findPrint.findPrint.print != null ? findPrint.findPrint.print.Name : null);
                    bsonWriter.zWrite("print_title", findPrint.findPrint.print != null ? findPrint.findPrint.print.Title : null);
                    bsonWriter.zWrite("titleInfo_formatedTitle", findPrint.findPrint.titleInfo.FormatedTitle);
                    bsonWriter.zWrite("findPrint_remainText", findPrint.findPrint.remainText);
                    bsonWriter.WriteStartArray("warnings");
                    foreach (string message in messages)
                        bsonWriter.WriteString(message);
                    messages.Clear();
                    bsonWriter.WriteEndArray();
                    bsonWriter.WriteEndDocument();
                    sw.WriteLine();
                    bsonWriter.WriteName("fake");    // ??? pour éviter l'erreur : WriteString can only be called when State is Value or Initial, not when State is Name (System.InvalidOperationException)
                }
            }
            finally
            {
                //Trace.CurrentTrace.WriteToTraceFiles -= getTrace;
                Trace.CurrentTrace.RemoveOnWrite("zSaveFindPrint");
                //Trace.CurrentTrace.EnableBaseLog();
                Trace.CurrentTrace.DisableViewer = false;
                if (bsonWriter != null)
                    bsonWriter.Close();
                if (sw != null)
                    sw.Close();
            }

            WriteSelectPost(zpath.PathSetFileNameWithoutExtension(file, zPath.GetFileNameWithoutExtension(file) + "_out"), zmongo.FileReader<BsonDocument>(bsonFile), old: true);
            WriteSelectPost(zpath.PathSetFileNameWithoutExtension(file, zPath.GetFileNameWithoutExtension(file) + "_out_new"), zmongo.FileReader<BsonDocument>(bsonFile));
            WriteSelectPost(zpath.PathSetFileNameWithoutExtension(file, zPath.GetFileNameWithoutExtension(file) + "_out_hors_serie"), from doc in zmongo.FileReader<BsonDocument>(bsonFile) where __rgSpecial.IsMatch(doc["post_title"].zAsString()) select doc);
            WriteSelectPost(zpath.PathSetFileNameWithoutExtension(file, zPath.GetFileNameWithoutExtension(file) + "_out_remain_text"), from doc in zmongo.FileReader<BsonDocument>(bsonFile) where !string.IsNullOrEmpty(doc["findPrint_remainText"].zAsString()) select doc);
            WriteSelectPost(zpath.PathSetFileNameWithoutExtension(file, zPath.GetFileNameWithoutExtension(file) + "_out_warning"), from doc in zmongo.FileReader<BsonDocument>(bsonFile) where doc["warnings"].AsBsonArray.Count > 0 select doc);
            WriteSelectPost(zpath.PathSetFileNameWithoutExtension(file, zPath.GetFileNameWithoutExtension(file) + "_out_no_title"), from doc in zmongo.FileReader<BsonDocument>(bsonFile) where doc["findPrint_file"].zAsString() != null && doc["findPrint_title"].zAsString() == null select doc);
            WriteSelectPost_NotSelected(zpath.PathSetFileNameWithoutExtension(file, zPath.GetFileNameWithoutExtension(file) + "_out_not_selected"), from doc in zmongo.FileReader<BsonDocument>(bsonFile) where doc["findPrint_file"].zAsString() == null select doc);
            return bsonFile;
        }

        private static void WriteSelectPost(string file, IEnumerable<BsonDocument> documents, bool old = false)
        {
            using (StreamWriter sw = zFile.CreateText(file))
            {
                foreach (BsonDocument document in documents)
                {
                    sw.WriteLine("post               : \"{0}\"", document["post_title"].zAsString());
                    sw.WriteLine("    category       : \"{0}\"", document["post_category"].zAsString());
                    sw.WriteLine("    file           : \"{0}\"", document["findPrint_file"].zAsString());
                    if (!old)
                    {
                        sw.WriteLine("    findPrintType  : \"{0}\"", (FindPrintType)document["findPrint_type"].zAsInt());
                        sw.WriteLine("    name           : \"{0}\"", document["findPrint_name"].zAsString());
                    }
                    sw.WriteLine("    print          : \"{0}\"", document["print_name"].zAsString());
                    sw.WriteLine("    title          : \"{0}\"", document["print_title"].zAsString());
                    sw.WriteLine("    remain text    : \"{0}\"", document["findPrint_remainText"].zAsString());
                    foreach (BsonValue warning in document["warnings"].AsBsonArray)
                        sw.WriteLine("    warning     : \"{0}\"", warning);
                    sw.WriteLine();
                }
            }
        }

        private static void WriteSelectPost_NotSelected(string file, IEnumerable<BsonDocument> documents)
        {
            using (StreamWriter sw = zFile.CreateText(file))
            {
                //foreach (BsonDocument document in documents)
                foreach (BsonDocument document in from doc in documents orderby doc["post_category"].zAsString(), doc["post_title"].zAsString() select doc)
                {
                    sw.WriteLine("post            : {0,-25} \"{1}\"", document["post_category"].zAsString(), document["post_title"].zAsString());
                }
            }
        }

        private static void WriteSelectPost_old(string file, IEnumerable<BsonDocument> documents, bool old = false)
        {
            using (StreamWriter sw = zFile.CreateText(file))
            {
                foreach (BsonDocument document in documents)
                {
                    sw.WriteLine("post               : \"{0}\"", document["postTitle"].zAsString());
                    sw.WriteLine("    category       : \"{0}\"", document["category"].zAsString());
                    sw.WriteLine("    file           : \"{0}\"", document["file"].zAsString());
                    if (!old)
                    {
                        sw.WriteLine("    findPrintType  : \"{0}\"", (FindPrintType)document["findPrintType"].zAsInt());
                        sw.WriteLine("    name           : \"{0}\"", document["name"].zAsString());
                    }
                    sw.WriteLine("    print          : \"{0}\"", document["print"].zAsString());
                    sw.WriteLine("    title          : \"{0}\"", document["title"].zAsString());
                    sw.WriteLine("    remain text    : \"{0}\"", document["remainText"].zAsString());
                    foreach (BsonValue warning in document["warnings"].AsBsonArray)
                        sw.WriteLine("    warning     : \"{0}\"", warning);
                    sw.WriteLine();
                }
            }
        }

        private static void WriteSelectPost_NotSelected_old(string file, IEnumerable<BsonDocument> documents)
        {
            using (StreamWriter sw = zFile.CreateText(file))
            {
                //foreach (BsonDocument document in documents)
                foreach (BsonDocument document in from doc in documents orderby doc["category"].zAsString(), doc["postTitle"].zAsString() select doc)
                {
                    sw.WriteLine("post            : {0,-25} \"{1}\"", document["category"].zAsString(), document["postTitle"].zAsString());
                }
            }
        }

        private static string GetDirectory()
        {
            return zPath.Combine(XmlConfig.CurrentConfig.GetExplicit("TestUnitDirectory"), @"Print\FindPrint");
        }
    }
}
