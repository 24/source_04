using pb.Data.Mongo.Serializers;

namespace pb.Web.TestUnit
{
    public static class TestUnit_HtmlToXmlRun
    {
        public static void Init()
        {
            DefaultMongoSerialization.SetDefaultMongoSerializationOptions();
        }

        public static void End()
        {
            DefaultMongoSerialization.RemoveDefaultMongoSerializationOptions();
        }
    }
}
