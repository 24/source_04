namespace pb.Data.Mongo.Serializers
{
    static partial class RunSerializer
    {
        public static void InitZValue(bool traceSerializer = false)
        {
            //Trace.WriteLine($"RunSerializer.InitZValue() : traceSerializer {traceSerializer}");

            if (traceSerializer)
            {
                ZValueSerializer.Trace = true;
                ZStringSerializer.Trace = true;
                ZStringArraySerializer.Trace = true;
                ZIntSerializer.Trace = true;
            }

            //PBSerializationProvider_v1.RegisterSerializer(typeof(ZValue), typeof(ZValueSerializer));
            //PBSerializationProvider_v1.RegisterSerializer(typeof(ZInt), typeof(ZIntSerializer));
            //PBSerializationProvider_v1.RegisterSerializer(typeof(ZString), typeof(ZStringSerializer));
            //PBSerializationProvider_v1.RegisterSerializer(typeof(ZStringArray), typeof(ZStringArraySerializer));
            PBSerializationProvider_v2.Instance.RegisterSerializer(typeof(ZValue), new ZValueSerializer());
            PBSerializationProvider_v2.Instance.RegisterSerializer(typeof(ZInt), new ZIntSerializer());
            PBSerializationProvider_v2.Instance.RegisterSerializer(typeof(ZString), new ZStringSerializer());
            PBSerializationProvider_v2.Instance.RegisterSerializer(typeof(ZStringArray), new ZStringArraySerializer());
        }

        public static void EndZValue()
        {
            //PBSerializationProvider_v1.UnregisterSerializer(typeof(ZValue));
            //PBSerializationProvider_v1.UnregisterSerializer(typeof(ZInt));
            //PBSerializationProvider_v1.UnregisterSerializer(typeof(ZString));
            //PBSerializationProvider_v1.UnregisterSerializer(typeof(ZStringArray));
            PBSerializationProvider_v2.Instance.UnregisterSerializer(typeof(ZValue));
            PBSerializationProvider_v2.Instance.UnregisterSerializer(typeof(ZInt));
            PBSerializationProvider_v2.Instance.UnregisterSerializer(typeof(ZString));
            PBSerializationProvider_v2.Instance.UnregisterSerializer(typeof(ZStringArray));

            ZValueSerializer.Trace = false;
            ZStringSerializer.Trace = false;
            ZStringArraySerializer.Trace = false;
            ZIntSerializer.Trace = false;
        }
    }
}
