using System;
using pb;
using pb.Compiler;
using pb.Data.Mongo;
using pb.Linq;
using pb.Web;

namespace Download.Print
{
    public static class images_cache_f
    {
        public static void Test_ViewImagesCache_01()
        {
            //RunSource.CurrentRunSource.View(MongoCommand.Find<MongoImage>("dl", "Images", "{}", sort: "{ 'Height': 1 }").zAction(mongoImage => mongoImage.Image = DownloadPrint.LoadImage(mongoImage.Url)));
            MongoCommand.Find<MongoImage>("dl", "Images", "{}", sort: "{ 'Height': 1 }").zAction(mongoImage => mongoImage.Image = DownloadPrint.LoadImage(mongoImage.Url)).zView();
        }
    }
}
