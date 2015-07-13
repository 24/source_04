using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using pb.Compiler;
using Download.Print.TelechargementPlus;

//namespace Print.download
namespace Download.Print
{
    public static class TelechargementPlus_Exe_v2
    {
        public static void Test_TelechargementPlus_LoadHeaderPages_01_old(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false)
        {
            TelechargementPlus_v2.Init();
            //RunSource.CurrentRunSource.View(new TelechargementPlus_LoadHeaderPages_v2(startPage, maxPage, reload, loadImage));
            new TelechargementPlus_LoadHeaderPages_v2(startPage, maxPage, reload, loadImage).zView();
        }

        public static void Test_TelechargementPlus_LoadDetailItemList_01_old(int startPage = 1, int maxPage = 1, bool reloadHeaderPage = false, bool reloadDetail = false, bool loadImage = false)
        {
            TelechargementPlus_v2.Init();
            //RunSource.CurrentRunSource.View(TelechargementPlus_v2.LoadDetailItemList(startPage, maxPage, reloadHeaderPage, reloadDetail, loadImage));
            TelechargementPlus_v2.LoadDetailItemList(startPage, maxPage, reloadHeaderPage, reloadDetail, loadImage).zView();
        }

        public static void Test_TelechargementPlus_LoadDetailItemList_02_old(int startPage = 1, int maxPage = 1, bool reloadHeaderPage = false, bool reloadDetail = false, bool loadImage = false)
        {
            TelechargementPlus_v2.Init();
            // RunSource.CurrentRunSource.View
            (from item in TelechargementPlus_v2.LoadDetailItemList(startPage, maxPage, reloadHeaderPage, reloadDetail, loadImage)
                     select new
                     {
                        url = item.detail.sourceUrl,
                        creationDate = item.detail.creationDate,
                        title = item.detail.title,
                        images = (from image in item.detail.images select image.Image).ToArray(),
                        downloadLinks = item.detail.downloadLinks
                     }).zView();
        }
    }
}
