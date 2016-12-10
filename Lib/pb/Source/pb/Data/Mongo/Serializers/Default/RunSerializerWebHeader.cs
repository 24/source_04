using System.Net;

namespace pb.Data.Mongo.Serializers
{
    static partial class RunSerializer
    {
        // register WebHeaderSerializer in provider PBSerializationProvider_v2
        public static void InitWebHeader(bool traceSerializer = false)
        {
            //Trace.WriteLine($"RunSerializer.InitWebHeader() : traceSerializer {traceSerializer}");

            if (traceSerializer)
                WebHeaderSerializer.Trace = true;

            //PBSerializationProvider_v1.RegisterSerializer(typeof(WebHeaderCollection), typeof(WebHeaderSerializer));
            PBSerializationProvider_v2.Instance.RegisterSerializer(typeof(WebHeaderCollection), new WebHeaderSerializer());
        }

        // unregister WebHeaderSerializer in provider PBSerializationProvider_v2
        public static void EndWebHeader()
        {
            //PBSerializationProvider_v1.UnregisterSerializer(typeof(WebHeaderCollection));
            PBSerializationProvider_v2.Instance.UnregisterSerializer(typeof(WebHeaderCollection));

            WebHeaderSerializer.Trace = false;
        }
    }
}
