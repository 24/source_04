using pb.Data.Mongo.Serializers;

namespace pb.Web.Html.TestUnit
{
    public static class TestUnit_HtmlToXmlRun
    {
        public static void Init()
        {
            MongoSerializationManager.SetDefaultMongoSerializationOptions();
        }

        public static void End()
        {
            MongoSerializationManager.RemoveDefaultMongoSerializationOptions();
        }
    }
}
