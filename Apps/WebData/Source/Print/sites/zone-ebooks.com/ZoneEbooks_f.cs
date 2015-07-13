using pb.Compiler;
using Download.Print.ZoneEbooks;

//namespace Print.download
namespace Download.Print
{
    public static class ZoneEbooks_Exe
    {
        public static void Test_zone_ebooks_loadPostHeader_01(int startPage = 1, int maxPage = 1, bool loadImage = false)
        {
            string url = "http://zone-ebooks.com/";
            if (startPage != 1)
                url += "page/" + (string)startPage.ToString() + "/";
            //LoadFromWeb1<ZoneEbooksPostHeader> load = new LoadFromWeb1<ZoneEbooksPostHeader>(new LoadZoneEbooksPostHeaderFromWeb(loadImage), url, maxPage);
            LoadZoneEbooksPostHeaderFromWeb load = new LoadZoneEbooksPostHeaderFromWeb(url, maxPage, loadImage);
            //RunSource.CurrentRunSource.View(load);
            load.zView();
        }

        public static void Test_ZoneEbooks_LoadHeaderPages_01(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false)
        {
            //RunSource.CurrentRunSource.View(ZoneEbooks_LoadHeaderPages.LoadHeaderPages(startPage, maxPage, reload, loadImage));
            ZoneEbooks_LoadHeaderPages.LoadHeaderPages(startPage, maxPage, reload, loadImage).zView();
        }
    }
}
