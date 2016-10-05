using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
using pb.Web;
using System.Net;

namespace pb.Data.Mongo.Serializers
{
    public static class DefaultMongoSerialization
    {
        public static void SetDefaultMongoSerializationOptions()
        {
            // enum as string, ATTENTION vérifier d'abord 
            ConventionPack conventions = new ConventionPack();
            conventions.Add(new EnumRepresentationConvention(BsonType.String));
            ConventionRegistry.Register("PB_EnumRepresentationConvention", conventions, t => true);
        }

        public static void RemoveDefaultMongoSerializationOptions()
        {
            ConventionRegistry.Remove("PB_EnumRepresentationConvention");
        }

        public static void RegisterDefaultMongoSerializer()
        {
            if (!BsonSerializer.zIsSerializerRegistered(typeof(DateTime)))
            {
                DateTimeSerializer dateTimeSerializer = new DateTimeSerializer(DateTimeSerializationOptions.LocalInstance);
                BsonSerializer.RegisterSerializer(typeof(DateTime), dateTimeSerializer);
            }

            // use BsonSerializer.RegisterSerializer() and not BsonPBSerializationProvider.RegisterProvider() for Date?
            if (!BsonSerializer.zIsSerializerRegistered(typeof(Date)))
                BsonSerializer.RegisterSerializer(typeof(Date), new DateSerializer());

            BsonPBSerializationProvider.RegisterProvider();
            //BsonPBSerializationProvider.RegisterSerializer(typeof(Date), typeof(DateSerializer));
            BsonPBSerializationProvider.RegisterSerializer(typeof(ZValue), typeof(ZValueSerializer));
            BsonPBSerializationProvider.RegisterSerializer(typeof(ZInt), typeof(ZIntSerializer));
            BsonPBSerializationProvider.RegisterSerializer(typeof(ZString), typeof(ZStringSerializer));
            BsonPBSerializationProvider.RegisterSerializer(typeof(ZStringArray), typeof(ZStringArraySerializer));
            //BsonPBSerializationProvider.RegisterSerializer(typeof(WebImage), typeof(UrlImageSerializer));
            BsonPBSerializationProvider.RegisterSerializer(typeof(WebHeaderCollection), typeof(WebHeaderSerializer));
        }

        public static void UnregisterDefaultMongoSerializer()
        {
            BsonPBSerializationProvider.UnregisterProvider();
            //BsonPBSerializationProvider.UnregisterSerializer(typeof(Date));
            BsonPBSerializationProvider.UnregisterSerializer(typeof(ZValue));
            BsonPBSerializationProvider.UnregisterSerializer(typeof(ZInt));
            BsonPBSerializationProvider.UnregisterSerializer(typeof(ZString));
            BsonPBSerializationProvider.UnregisterSerializer(typeof(ZStringArray));
            //BsonPBSerializationProvider.UnregisterSerializer(typeof(WebImage));
            BsonPBSerializationProvider.UnregisterSerializer(typeof(WebHeaderCollection));
        }
    }
}
