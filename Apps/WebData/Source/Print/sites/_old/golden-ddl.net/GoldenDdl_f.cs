using System;
using System.Linq;
using pb.Compiler;

namespace Download.Print.GoldenDdl
{
    public static class GoldenDdl_Exe
    {
        public static void Test_GoldenDdl_LoadDetailItemList_01(string query = null, string sort = null, int limit = 10, bool loadImage = false)
        {
            //RunSource.CurrentRunSource.View
            (from post in GoldenDdl_LoadPostDetail.CurrentLoadPostDetail.FindDocuments(query, sort: sort, limit: limit, loadImage: loadImage)
                select new
                {
                    id = post.GetKey(),
                    loadFromWebDate = post.GetLoadFromWebDate(),
                    creationDate = post.GetPostCreationDate(),
                    printType = post.GetPrintType().ToString(),
                    //category = post.category,
                    title = post.GetTitle(),
                    url = post.GetDataHttpRequest().Url,
                    images = (from image in post.GetImages() select image.Image).ToArray(),
                    downloadLinks = post.GetDownloadLinks()
                }).zView();
        }

        //public static void Test_GoldenDdl_LoadDetailItemList_01(string query = null, string sort = null, int limit = 10, bool loadImage = false)
        //{
        //    RunSource.CurrentRunSource.View(from post in GoldenDdl_LoadPostDetail.Find(query, sort: sort, limit: limit, loadImage: loadImage)
        //                                    select new
        //                                    {
        //                                        id = post.id,
        //                                        loadFromWebDate = post.loadFromWebDate,
        //                                        creationDate = post.creationDate,
        //                                        printType = post.printType.ToString(),
        //                                        category = post.category,
        //                                        title = post.title,
        //                                        url = post.sourceUrl,
        //                                        images = (from image in post.images select image.Image).ToArray(),
        //                                        downloadLinks = post.downloadLinks
        //                                    });
        //}

        public static void Test_GoldenDdl_LoadHeaderPages_01(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false, bool refreshDocumentStore = false)
        {
            //RunSource.CurrentRunSource.View
            GoldenDdl_LoadHeaderPagesManager.CurrentLoadHeaderPagesManager.LoadPages(startPage, maxPage, reload, loadImage, refreshDocumentStore).zView();
        }
    }
}
