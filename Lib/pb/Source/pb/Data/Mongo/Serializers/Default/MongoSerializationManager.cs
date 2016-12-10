using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;

// c'est quoi la différence entre BsonSerializer.RegisterSerializer() et BsonPBSerializationProvider.RegisterSerializer()
// BsonSerializer c'est mongo, BsonPBSerializationProvider c'est mon provider
namespace pb.Data.Mongo.Serializers
{
    public static partial class MongoSerializationManager
    {
        //private static bool _usePBSerializationProvider_v2 = true;

        //public static bool UsePBSerializationProvider_v2 { get { return _usePBSerializationProvider_v2; } set { _usePBSerializationProvider_v2 = value; } }

        // set mongo serialization options : save enum as string
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

        //public static void RegisterProvider()
        //{
        //    if (!_usePBSerializationProvider_v2)
        //    {
        //        PBSerializationProvider_v1.RegisterProvider();
        //    }
        //    else
        //    {
        //        PBSerializationProvider_v2.Instance.RegisterProvider();
        //    }
        //}

        //public static void UnregisterProvider()
        //{
        //    if (!_usePBSerializationProvider_v2)
        //    {
        //        PBSerializationProvider_v1.UnregisterProvider();
        //    }
        //    else
        //    {
        //        PBSerializationProvider_v2.Instance.UnregisterProvider();
        //    }
        //}

        // DateTime register
        //public static void RegisterDateTimeSerializer()
        //{
        //    // WARNING ça ne marche pas, affiche l'heure avec un décalage d'une heure
        //    // ex : ISODate("2016-11-29T15:11:47.927Z") à 16h11

        //    if (!_usePBSerializationProvider_v2)
        //    {
        //        if (!BsonSerializer.zIsSerializerRegistered(typeof(DateTime)))
        //        {
        //            DateTimeSerializer dateTimeSerializer = new DateTimeSerializer(DateTimeSerializationOptions.LocalInstance);
        //            BsonSerializer.RegisterSerializer(typeof(DateTime), dateTimeSerializer);
        //        }
        //    }
        //    else
        //    {
        //        DateTimeSerializer dateTimeSerializer = new DateTimeSerializer(DateTimeSerializationOptions.LocalInstance);
        //        PBSerializationProvider_v2.Instance.RegisterSerializer(typeof(DateTime), dateTimeSerializer);
        //    }
        //}

        // DateTime unregister
        //public static void UnregisterDateTimeSerializer()
        //{
        //    if (!_usePBSerializationProvider_v2)
        //    {
        //        //Trace.WriteLine("UnregisterDateTimeSerializer");
        //        BsonSerializer.zUnregisterSerializer(typeof(DateTime));
        //    }
        //    else
        //    {
        //        PBSerializationProvider_v2.Instance.UnregisterSerializer(typeof(DateTime));
        //    }
        //}

        // Date register
        //public static void RegisterDateSerializer()
        //{
        //    if (!_usePBSerializationProvider_v2)
        //    {
        //        if (!BsonSerializer.zIsSerializerRegistered(typeof(Date)))
        //            BsonSerializer.RegisterSerializer(typeof(Date), new DateSerializer());
        //    }
        //    else
        //    {
        //        PBSerializationProvider_v2.Instance.RegisterSerializer(typeof(Date), new DateSerializer());
        //    }
        //}

        // Date unregister
        //public static void UnregisterDateSerializer()
        //{
        //    if (!_usePBSerializationProvider_v2)
        //    {
        //        BsonSerializer.zUnregisterSerializer(typeof(Date));
        //    }
        //    else
        //    {
        //        PBSerializationProvider_v2.Instance.UnregisterSerializer(typeof(Date));
        //    }
        //}

        //public static void RegisterWebHeaderCollectionSerializer()
        //{
        //    if (!_usePBSerializationProvider_v2)
        //    {
        //        PBSerializationProvider_v1.RegisterSerializer(typeof(WebHeaderCollection), typeof(WebHeaderSerializer));
        //    }
        //    else
        //    {
        //        PBSerializationProvider_v2.Instance.RegisterSerializer(typeof(WebHeaderCollection), new WebHeaderSerializer());
        //    }
        //}

        //public static void UnregisterWebHeaderCollectionSerializer()
        //{
        //    if (!_usePBSerializationProvider_v2)
        //    {
        //        PBSerializationProvider_v1.UnregisterSerializer(typeof(WebHeaderCollection));
        //    }
        //    else
        //    {
        //        PBSerializationProvider_v2.Instance.UnregisterSerializer(typeof(WebHeaderCollection));
        //    }
        //}

        //public static void RegisterDefaultMongoSerializer()
        //{
        //    // use BsonSerializer.RegisterSerializer() and not BsonPBSerializationProvider.RegisterProvider() for Date?

        //    //BsonPBSerializationProvider.RegisterProvider();
        //    //BsonPBSerializationProvider.RegisterSerializer(typeof(WebImage), typeof(UrlImageSerializer));
        //}

        //public static void UnregisterDefaultMongoSerializer()
        //{
        //    //BsonPBSerializationProvider.UnregisterProvider();
        //    //BsonPBSerializationProvider.UnregisterSerializer(typeof(WebImage));
        //}
    }
}
