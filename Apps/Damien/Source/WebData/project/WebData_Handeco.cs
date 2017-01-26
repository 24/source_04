using hts.WebData.Handeco;
using pb.Data;

namespace hts.WebData
{
    static partial class WebData
    {
        public static Handeco_v2 CreateHandeco(string parameters = null)
        {
            NamedValues<ZValue> parameters2 = ParseParameters(parameters);
            bool test = WebData.GetTestValue(parameters2);
            return Handeco_v2.Create(test);
        }

        //public static void Test_01()
        //{
        //    //CreateHandeco().HeaderDetailManager.LoadDetails(startPage: 1, maxPage: 1, reloadHeaderPage: false, reloadDetail: false, loadImageFromWeb: false, loadImageToData: false, refreshImage: false, refreshDocumentStore: false);
        //    Trace.WriteLine("CreateHandeco()");
        //    var handeco = CreateHandeco();
        //    Trace.WriteLine("get HeaderDetailManager");
        //    var headerDetailManager = handeco.HeaderDetailManager;
        //    Trace.WriteLine("LoadDetails()");
        //    headerDetailManager.LoadDetails(startPage: 1, maxPage: 1, reloadHeaderPage: false, reloadDetail: false, loadImageFromWeb: false, loadImageToData: false, refreshImage: false, refreshDocumentStore: false);
        //    //headerDetailManager.Test();
        //    //headerDetailManager.LoadDetails_Test(startPage: 1, maxPage: 1, reloadHeaderPage: false, reloadDetail: false, loadImageFromWeb: false, loadImageToData: false, refreshImage: false, refreshDocumentStore: false);
        //}
    }
}
