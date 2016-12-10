using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;

namespace pb.Data.Mongo.Serializers
{
    public static class PBDictionarySerializer
    {
        private static DictionaryRepresentation __defaultDictionaryRepresentation = DictionaryRepresentation.Document;

        public static DictionaryRepresentation DefaultDictionaryRepresentation { get { return __defaultDictionaryRepresentation; } set { __defaultDictionaryRepresentation = value; } }

        public static void RegisterGenericDictionarySerializer()
        {
            //Trace.WriteLine("**** BsonSerializer.RegisterGenericSerializerDefinition(typeof(Dictionary<,>), typeof(PBDictionarySerializer<,>))");
            BsonSerializer.RegisterGenericSerializerDefinition(typeof(Dictionary<,>), typeof(PBDictionarySerializer<,>));
        }

        public static void UnregisterGenericDictionarySerializer()
        {
            //BsonSerializer.UnregisterGenericSerializerDefinition(typeof(Dictionary<,>), typeof(PBDictionarySerializer<,>));
        }

        //public static void SetDictionaryRepresentation(DictionaryRepresentation defaultDictionaryRepresentation)
        //{
        //    __defaultDictionaryRepresentation = defaultDictionaryRepresentation;
        //}
    }

    //public class PBDictionarySerializer<TKey, TValue> : DictionarySerializer
    //{
    //    public PBDictionarySerializer()
    //        : base(new DictionarySerializationOptions(PBDictionarySerializer.DefaultDictionaryRepresentation))
    //    {
    //    }
    //}


    public class PBDictionarySerializer<TKey, TValue> : BsonBaseSerializer
    {
        // from MongoDB.Bson.Serialization.Serializers.DictionarySerializer<TKey, TValue> (DictionaryGenericSerializer.cs)

        private readonly PBKeyValuePairSerializer<TKey, TValue> _keyValuePairSerializer;

        public PBDictionarySerializer()
#pragma warning disable 618
            : this(DictionarySerializationOptions.Defaults)
#pragma warning restore
        {
        }

        public PBDictionarySerializer(DictionarySerializationOptions defaultSerializationOptions)
            : base(defaultSerializationOptions)
        {
            _keyValuePairSerializer = new PBKeyValuePairSerializer<TKey, TValue>();
        }

        public override object Deserialize(BsonReader bsonReader, Type nominalType, Type actualType, IBsonSerializationOptions options)
        {
            var dictionarySerializationOptions = EnsureSerializationOptions(options);
            var dictionaryRepresentation = dictionarySerializationOptions.Representation;
            var keyValuePairSerializationOptions = dictionarySerializationOptions.KeyValuePairSerializationOptions;

            var bsonType = bsonReader.GetCurrentBsonType();
            if (bsonType == BsonType.Null)
            {
                bsonReader.ReadNull();
                return null;
            }
            else if (bsonType == BsonType.Document)
            {
                if (nominalType == typeof(object))
                {
                    bsonReader.ReadStartDocument();
                    bsonReader.ReadString("_t"); // skip over discriminator
                    bsonReader.ReadName("_v");
                    var value = Deserialize(bsonReader, actualType, options); // recursive call replacing nominalType with actualType
                    bsonReader.ReadEndDocument();
                    return value;
                }

                var dictionary = CreateInstance(actualType);
                var valueDiscriminatorConvention = BsonSerializer.LookupDiscriminatorConvention(typeof(TValue));

                bsonReader.ReadStartDocument();
                while (bsonReader.ReadBsonType() != BsonType.EndOfDocument)
                {
                    var key = (TKey)(object)bsonReader.ReadName();
                    var valueType = valueDiscriminatorConvention.GetActualType(bsonReader, typeof(TValue));
                    var valueSerializer = BsonSerializer.LookupSerializer(valueType);
                    var value = (TValue)valueSerializer.Deserialize(bsonReader, typeof(TValue), valueType, keyValuePairSerializationOptions.ValueSerializationOptions);
                    dictionary.Add(key, value);
                }
                bsonReader.ReadEndDocument();

                return dictionary;
            }
            else if (bsonType == BsonType.Array)
            {
                var dictionary = CreateInstance(actualType);

                bsonReader.ReadStartArray();
                while (bsonReader.ReadBsonType() != BsonType.EndOfDocument)
                {
                    var keyValuePair = (KeyValuePair<TKey, TValue>)_keyValuePairSerializer.Deserialize(
                        bsonReader,
                        typeof(KeyValuePair<TKey, TValue>),
                        keyValuePairSerializationOptions);
                    dictionary.Add(keyValuePair);
                }
                bsonReader.ReadEndArray();

                return dictionary;
            }
            else
            {
                //var message = string.Format("Can't deserialize a {0} from BsonType {1}.", nominalType.FullName, bsonType);
                //throw new FileFormatException(message);
                throw new PBException("Can't deserialize a {0} from BsonType {1}.", nominalType.FullName, bsonType);
            }
        }

        public override void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
        {
            if (value == null)
            {
                bsonWriter.WriteNull();
            }
            else
            {
                if (nominalType == typeof(object))
                {
                    var actualType = value.GetType();
                    bsonWriter.WriteStartDocument();
                    bsonWriter.WriteString("_t", TypeNameDiscriminator.GetDiscriminator(actualType));
                    bsonWriter.WriteName("_v");
                    Serialize(bsonWriter, actualType, value, options); // recursive call replacing nominalType with actualType
                    bsonWriter.WriteEndDocument();
                    return;
                }

                var dictionary = (IDictionary<TKey, TValue>)value;
                var dictionarySerializationOptions = EnsureSerializationOptions(options);
                var dictionaryRepresentation = dictionarySerializationOptions.Representation;
                var keyValuePairSerializationOptions = dictionarySerializationOptions.KeyValuePairSerializationOptions;

                if (dictionaryRepresentation == DictionaryRepresentation.Dynamic)
                {
                    if (typeof(TKey) == typeof(string) || typeof(TKey) == typeof(object))
                    {
                        dictionaryRepresentation = DictionaryRepresentation.Document;
                        foreach (object key in dictionary.Keys)
                        {
                            var name = key as string; // key might not be a string
                            if (string.IsNullOrEmpty(name) || name[0] == '$' || name.IndexOf('.') != -1 || name.IndexOf('\0') != -1)
                            {
                                dictionaryRepresentation = DictionaryRepresentation.ArrayOfArrays;
                                break;
                            }
                        }
                    }
                    else
                    {
                        dictionaryRepresentation = DictionaryRepresentation.ArrayOfArrays;
                    }
                }

                // DictionaryRepresentation.Document:
                //   { "infos2_string_key" : "infos2_string_value", "infos2_int_key" : 1234 }
                // DictionaryRepresentation.ArrayOfArrays:
                //   [["infos2_string_key", "infos2_string_value"], ["infos2_int_key", 1234]]
                // DictionaryRepresentation.ArrayOfDocuments:
                //   old [{ "k" : "infos2_string_key", "v" : "infos2_string_value" }, { "k" : "infos2_int_key", "v" : 1234 }]
                //   new [{ "infos2_string_key" : "infos2_string_value" }, { "infos2_int_key" : 1234 }]

                switch (dictionaryRepresentation)
                {
                    case DictionaryRepresentation.Document:
                        bsonWriter.WriteStartDocument();
                        foreach (var keyValuePair in dictionary)
                        {
                            bsonWriter.WriteName((string)(object)keyValuePair.Key);
                            BsonSerializer.Serialize(bsonWriter, typeof(TValue), keyValuePair.Value, keyValuePairSerializationOptions.ValueSerializationOptions);
                        }
                        bsonWriter.WriteEndDocument();
                        break;

                    case DictionaryRepresentation.ArrayOfArrays:
                    case DictionaryRepresentation.ArrayOfDocuments:
                        // override KeyValuePair representation if necessary
                        var keyValuePairRepresentation = (dictionaryRepresentation == DictionaryRepresentation.ArrayOfArrays) ? BsonType.Array : BsonType.Document;
                        if (keyValuePairSerializationOptions.Representation != keyValuePairRepresentation)
                        {
                            keyValuePairSerializationOptions = new KeyValuePairSerializationOptions(
                                keyValuePairRepresentation,
                                keyValuePairSerializationOptions.KeySerializationOptions,
                                keyValuePairSerializationOptions.ValueSerializationOptions);
                        }

                        bsonWriter.WriteStartArray();
                        foreach (var keyValuePair in dictionary)
                        {
                            _keyValuePairSerializer.Serialize(
                                bsonWriter,
                                typeof(KeyValuePair<TKey, TValue>),
                                keyValuePair,
                                keyValuePairSerializationOptions);
                        }
                        bsonWriter.WriteEndArray();
                        break;

                    default:
                        //var message = string.Format("'{0}' is not a valid IDictionary<{1}, {2}> representation.",
                        //    dictionaryRepresentation,
                        //    BsonUtils.GetFriendlyTypeName(typeof(TKey)),
                        //    BsonUtils.GetFriendlyTypeName(typeof(TValue)));
                        //throw new BsonSerializationException(message);
                        throw new PBException("'{0}' is not a valid IDictionary<{1}, {2}> representation.", dictionaryRepresentation, BsonUtils.GetFriendlyTypeName(typeof(TKey)),
                            BsonUtils.GetFriendlyTypeName(typeof(TValue)));
                }
            }
        }

        // private methods
        private IDictionary<TKey, TValue> CreateInstance(Type type)
        {
            if (type.IsInterface)
            {
                // in the case of an interface pick a reasonable class that implements that interface
                if (type == typeof(IDictionary<TKey, TValue>))
                {
                    return new Dictionary<TKey, TValue>();
                }
            }
            else
            {
                if (type == typeof(Dictionary<TKey, TValue>))
                {
                    return new Dictionary<TKey, TValue>();
                }
                else if (type == typeof(SortedDictionary<TKey, TValue>))
                {
                    return new SortedDictionary<TKey, TValue>();
                }
                else if (type == typeof(SortedList<TKey, TValue>))
                {
                    return new SortedList<TKey, TValue>();
                }
                else if (typeof(IDictionary<TKey, TValue>).IsAssignableFrom(type))
                {
                    return (IDictionary<TKey, TValue>)Activator.CreateInstance(type);
                }
            }

            //var message = string.Format("DictionarySerializer<{0}, {1}> can't be used with type {1}.",
            //    BsonUtils.GetFriendlyTypeName(typeof(TKey)),
            //    BsonUtils.GetFriendlyTypeName(typeof(TValue)),
            //    BsonUtils.GetFriendlyTypeName(type));
            //throw new BsonSerializationException(message);
            throw new PBException("DictionarySerializer<{0}, {1}> can't be used with type {1}.", BsonUtils.GetFriendlyTypeName(typeof(TKey)), BsonUtils.GetFriendlyTypeName(typeof(TValue)),
                BsonUtils.GetFriendlyTypeName(type));
        }

        private DictionarySerializationOptions EnsureSerializationOptions(IBsonSerializationOptions options)
        {
            // support RepresentationSerializationOptions for backward compatibility
            var representationSerializationOptions = options as RepresentationSerializationOptions;
            if (representationSerializationOptions != null)
            {
                switch (representationSerializationOptions.Representation)
                {
                    case BsonType.Array:
                        options = DictionarySerializationOptions.ArrayOfArrays;
                        break;
                    case BsonType.Document:
                        options = DictionarySerializationOptions.Document;
                        break;
                    default:
                        //var message = string.Format("BsonType {0} is not a valid representation for a Dictionary.", representationSerializationOptions.Representation);
                        //throw new BsonSerializationException(message);
                        throw new PBException("BsonType {0} is not a valid representation for a Dictionary.", representationSerializationOptions.Representation);
                }
            }

            return EnsureSerializationOptions<DictionarySerializationOptions>(options);
        }
    }
}
