using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//namespace Print.download
namespace Download.Print
{
    public static class Pdf4fr_Exe
    {
        public static void Test_pdf4fr_loadPostHeader_01(int maxPage = 1, bool loadImage = false)
        {
            string url = "http://pdf4fr.com/";
            //LoadFromWeb1<Pdf4frPostHeader> load = new LoadFromWeb1<Pdf4frPostHeader>(new LoadPdf4frPostHeaderFromWeb(loadImage), url, maxPage);
            LoadPdf4frPostHeaderFromWeb load = new LoadPdf4frPostHeaderFromWeb(url, maxPage, loadImage);
            //RunSource.CurrentRunSource.View(load);
            load.zView();
        }
    }
}
