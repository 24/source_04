using System;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using pb;
using pb.Compiler;
using pb.Data.Mongo;
using pb.Web;
using Download.Print.TelechargementPlus;

// cache image :
//   telechargement_plus_old.cs TelechargementPlus_Print_old1.DocumentMongoLoad(BsonDocument doc, string fileDirectory = null)
//   telechargement_plus_old.cs TelechargementPlus_Print_old1.DocumentXmlLoad(XmlReader xmlReader, string fileDirectory = null)


//namespace Print.download
namespace Download.Print
{
    public static class TelechargementPlus_Exe
    {
        public static void Test_TelechargementPlus_LoadDetailItemList_01(int startPage = 1, int maxPage = 1, bool reloadHeaderPage = false, bool reloadDetail = false, bool loadImage = false)
        {
            //RunSource.CurrentRunSource.View(TelechargementPlus.TelechargementPlus.LoadDetailItemList(startPage, maxPage, reloadHeaderPage, reloadDetail, loadImage));
            TelechargementPlus.TelechargementPlus.LoadDetailItemList(startPage, maxPage, reloadHeaderPage, reloadDetail, loadImage).zView();
        }

        public static void Test_TelechargementPlus_LoadDetailItemList_02(int startPage = 1, int maxPage = 1, bool reloadHeaderPage = false, bool reloadDetail = false, bool loadImage = false)
        {
            // RunSource.CurrentRunSource.View
            (from item in TelechargementPlus.TelechargementPlus.LoadDetailItemList(startPage, maxPage, reloadHeaderPage, reloadDetail, loadImage)
                     select new
                     {
                         url = item.sourceUrl,
                         creationDate = item.creationDate,
                         title = item.title,
                         images = (from image in item.images select image.Image).ToArray(),
                         downloadLinks = item.downloadLinks
                     }).zView();
        }

        public static void Test_TelechargementPlus_LoadDetailItemList_03(int startPage = 1, int maxPage = 1, bool reloadHeaderPage = false, bool reloadDetail = false, bool loadImage = false)
        {
            foreach (TelechargementPlus_PostDetail post in TelechargementPlus.TelechargementPlus.LoadDetailItemList(startPage, maxPage, reloadHeaderPage, reloadDetail, loadImage))
            {
                MongoDB.Bson.IO.JsonWriterSettings ws = new MongoDB.Bson.IO.JsonWriterSettings();
                ws.Indent = true;
                ws.NewLineChars = "\r\n";
                //ws.OutputMode = MongoDB.Bson.IO.JsonOutputMode.JavaScript;
                Trace.WriteLine(post.ToBsonDocument().ToJson(ws));
            }
        }

        public static void Test_TelechargementPlus_LoadHeaderPages_01(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false)
        {
            //RunSource.CurrentRunSource.View(TelechargementPlus_LoadHeaderPages.LoadHeaderPages(startPage, maxPage, reload, loadImage));
            TelechargementPlus_LoadHeaderPages.LoadHeaderPages(startPage, maxPage, reload, loadImage).zView();
        }

        public static void Test_TelechargementPlus_LoadDetail_01(string url, bool reload = false, bool loadImage = false)
        {
            //RunSource.CurrentRunSource.View(TelechargementPlus_LoadDetail.Load(url, reload: reload, loadImage: loadImage));
            TelechargementPlus_LoadDetail.Load(url, reload: reload, loadImage: loadImage).zView();
        }
    }
}
