using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
using System;

namespace pb.Data.Mongo.Serializers
{
    // register PBSerializationProvider_v2 provider
    // register DateTimeSerializer, DateSerializer in provider PBSerializationProvider_v2
    public static partial class RunSerializer
    {
        public static void InitDefault(bool traceProvider = false, bool traceSerializer = false)
        {
            //Trace.WriteLine($"RunSerializer.InitDefault() : traceProvider {traceProvider} traceSerializer {traceSerializer}");

            if (traceProvider)
                PBSerializationProvider_v2.Trace = true;
            if (traceSerializer)
                DateSerializer.Trace = true;

            MongoSerializationManager.SetDefaultMongoSerializationOptions();

            //MongoSerializationManager.RegisterDefaultMongoSerializer();

            //if (!BsonSerializer.zIsSerializerRegistered(typeof(DateTime)))
            //{
            //    DateTimeSerializer dateTimeSerializer = new DateTimeSerializer(DateTimeSerializationOptions.LocalInstance);
            //    BsonSerializer.RegisterSerializer(typeof(DateTime), dateTimeSerializer);
            //}
            PBSerializationProvider_v2.Instance.RegisterSerializer(typeof(DateTime), new DateTimeSerializer(DateTimeSerializationOptions.LocalInstance));

            //if (!BsonSerializer.zIsSerializerRegistered(typeof(Date)))
            //    BsonSerializer.RegisterSerializer(typeof(Date), new DateSerializer());
            PBSerializationProvider_v2.Instance.RegisterSerializer(typeof(Date), new DateSerializer());

            //PBSerializationProvider_v1.RegisterProvider();
            PBSerializationProvider_v2.Instance.RegisterProvider();
        }

        // unregister PBSerializationProvider_v2 provider
        // unregister DateTimeSerializer, DateSerializer in PBSerializationProvider_v2
        public static void EndDefault()
        {
            //Trace.WriteLine($"RunSerializer.EndDefault()");

            MongoSerializationManager.RemoveDefaultMongoSerializationOptions();

            //PBSerializationProvider_v1.UnregisterProvider();
            PBSerializationProvider_v2.Instance.UnregisterProvider();

            //MongoSerializationManager.UnregisterDefaultMongoSerializer();

            //MongoSerializationManager.UnregisterDateTimeSerializer();
            //BsonSerializer.zUnregisterSerializer(typeof(DateTime));
            PBSerializationProvider_v2.Instance.UnregisterSerializer(typeof(DateTime));

            //BsonSerializer.zUnregisterSerializer(typeof(Date));
            PBSerializationProvider_v2.Instance.UnregisterSerializer(typeof(Date));

            PBSerializationProvider_v2.Trace = false;
            DateSerializer.Trace = false;
        }
    }
}
