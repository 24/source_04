using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//namespace Print.download
namespace Download.Print
{
    public static class Magazine3k_Exe
    {
        public static void Test_magazine3k_loadPostHeader_01(int maxPage = 1, bool loadImage = false)
        {
            string url = "http://magazine3k.com/";
            //LoadFromWeb1<Magazine3kPostHeader> load = new LoadFromWeb1<Magazine3kPostHeader>(new LoadMagazine3kPostHeaderFromWeb(loadImage), url, maxPage);
            LoadMagazine3kPostHeaderFromWeb load = new LoadMagazine3kPostHeaderFromWeb(url, maxPage, loadImage);
            //RunSource.CurrentRunSource.View(load);
            load.zView();
        }
    }
}
