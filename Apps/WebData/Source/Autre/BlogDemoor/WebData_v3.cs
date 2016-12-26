using pb.Data;

namespace WebData.BlogDemoor
{
    public static partial class WebData
    {
        public static BlogDemoor_v3 CreateDataManager_v3(string parameters = null)
        {
            NamedValues<ZValue> parameters2 = ParseParameters(parameters);
            bool test = WebData.GetTestValue(parameters2);
            //InitLoadImage(test);
            return BlogDemoor_v3.Create(test);
        }
    }
}
