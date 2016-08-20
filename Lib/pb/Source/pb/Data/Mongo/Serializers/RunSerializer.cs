namespace pb.Data.Mongo.Serializers
{
    public static class RunSerializer
    {
        public static void Init()
        {
            DefaultMongoSerialization.SetDefaultMongoSerializationOptions();
            DefaultMongoSerialization.RegisterDefaultMongoSerializer();
        }

        public static void End()
        {
            DefaultMongoSerialization.RemoveDefaultMongoSerializationOptions();
            DefaultMongoSerialization.UnregisterDefaultMongoSerializer();
        }
    }
}
