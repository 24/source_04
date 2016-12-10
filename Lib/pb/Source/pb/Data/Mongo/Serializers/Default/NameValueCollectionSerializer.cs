using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;

namespace pb.Data.Mongo.Serializers
{
    /// <summary>
    /// Represents a serializer for NameValueCollection.
    /// </summary>
    public class NameValueCollectionSerializer : BsonBaseSerializer
    {
        private static bool _trace = false;

        // take example from DictionarySerializer.cs
        // other example :
        //   EnumerableSerializerBase : BsonBaseSerializer, IBsonArraySerializer
        //   EnumerableSerializer : EnumerableSerializerBase, IBsonArraySerializer
        //   DictionarySerializer : BsonBaseSerializer
        //   DictionarySerializer<TKey, TValue> : BsonBaseSerializer

        // private static fields
        //private static DictionarySerializer __instance = new DictionarySerializer();

        // private fields
        //private readonly KeyValuePairSerializer<object, object> _keyValuePairSerializer;
        private readonly KeyValuePairSerializer<string, string> _keyValuePairSerializer;

        // constructors
        /// <summary>
        /// Initializes a new instance of the NameValueCollectionSerializer class.
        /// </summary>
        public NameValueCollectionSerializer()
#pragma warning disable 618
            : this(DictionarySerializationOptions.Defaults)
#pragma warning restore
        {
        }

        /// <summary>
        /// Initializes a new instance of the NameValueCollectionSerializer class.
        /// </summary>
        /// <param name="defaultSerializationOptions">The default serialization options.</param>
        public NameValueCollectionSerializer(DictionarySerializationOptions defaultSerializationOptions)
            : base(defaultSerializationOptions)
        {
            //_keyValuePairSerializer = new KeyValuePairSerializer<object, object>();
            _keyValuePairSerializer = new KeyValuePairSerializer<string, string>();
        }

        public static bool Trace { get { return _trace; } set { _trace = value; } }

        // public static properties
        /// <summary>
        /// Gets an instance of the DictionarySerializer class.
        /// </summary>
        //[Obsolete("Use constructor instead.")]
        //public static DictionarySerializer Instance
        //{
        //    get { return __instance; }
        //}

        // public methods
        /// <summary>
        /// Deserializes an object from a BsonReader.
        /// </summary>
        /// <param name="bsonReader">The BsonReader.</param>
        /// <param name="nominalType">The nominal type of the object.</param>
        /// <param name="actualType">The actual type of the object.</param>
        /// <param name="options">The serialization options.</param>
        /// <returns>An object.</returns>
        public override object Deserialize(BsonReader bsonReader, Type nominalType, Type actualType, IBsonSerializationOptions options)
        {
            if (_trace)
                pb.Trace.WriteLine("NameValueCollectionSerializer.Deserialize()");

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
                // dont know why nominalType can be an object
                if (nominalType == typeof(object))
                {
                    bsonReader.ReadStartDocument();
                    bsonReader.ReadString("_t"); // skip over discriminator
                    bsonReader.ReadName("_v");
                    var value = Deserialize(bsonReader, actualType, options); // recursive call replacing nominalType with actualType
                    bsonReader.ReadEndDocument();
                    return value;
                }

                //var dictionary = CreateInstance(actualType);
                //var valueDiscriminatorConvention = BsonSerializer.LookupDiscriminatorConvention(typeof(object));

                // { "toto1" : "tata1", "toto2" : "tata2" }
                NameValueCollection nameValueCollection = new NameValueCollection();

                bsonReader.ReadStartDocument();
                while (bsonReader.ReadBsonType() != BsonType.EndOfDocument)
                {
                    //var key = bsonReader.ReadName();
                    //var valueType = valueDiscriminatorConvention.GetActualType(bsonReader, typeof(object));
                    //var valueSerializer = BsonSerializer.LookupSerializer(valueType);
                    //var value = valueSerializer.Deserialize(bsonReader, typeof(object), valueType, keyValuePairSerializationOptions.ValueSerializationOptions);
                    //dictionary.Add(key, value);
                    var key = bsonReader.ReadName();
                    var value = bsonReader.ReadString();
                    nameValueCollection.Add(key, value);
                }
                bsonReader.ReadEndDocument();

                return nameValueCollection;
            }
            else if (bsonType == BsonType.Array)
            {
                //var dictionary = CreateInstance(actualType);

                // DictionaryRepresentation.ArrayOfArrays
                // [["toto1", "tata1"], ["toto2", "tata2"]]
                // DictionaryRepresentation.ArrayOfDocuments
                // [{ "k" : "toto1", "v" : "tata1" }, { "k" : "toto2", "v" : "tata2" }]
                NameValueCollection nameValueCollection = new NameValueCollection();
                //KeyValuePairSerializer<string, string> keyValuePairSerializer = new KeyValuePairSerializer<string, string>();

                bsonReader.ReadStartArray();
                while (bsonReader.ReadBsonType() != BsonType.EndOfDocument)
                {
                    //var keyValuePair = (KeyValuePair<object, object>)_keyValuePairSerializer.Deserialize(
                    //    bsonReader,
                    //    typeof(KeyValuePair<object, object>),
                    //    keyValuePairSerializationOptions);
                    //dictionary.Add(keyValuePair.Key, keyValuePair.Value);
                    var keyValuePair = (KeyValuePair<string, string>)_keyValuePairSerializer.Deserialize(bsonReader, typeof(KeyValuePair<string, string>), keyValuePairSerializationOptions);
                    nameValueCollection.Add(keyValuePair.Key, keyValuePair.Value);
                }
                bsonReader.ReadEndArray();

                return nameValueCollection;
            }
            else
            {
                throw new PBException("Can't deserialize a {0} from BsonType {1}.", nominalType.FullName, bsonType);
            }
        }

        //private KeyValuePair<string, string> DeserializeNameValue(BsonReader bsonReader)
        //{
        //    string key = null;
        //    string value = null;
        //    var bsonType = bsonReader.GetCurrentBsonType();
        //    if (bsonType == BsonType.Array)
        //    {
        //        // [["toto1", "tata1"], ["toto2", "tata2"]]

        //        bsonReader.ReadStartArray();
        //        //bsonReader.ReadBsonType();
        //        //var keyType = keyDiscriminatorConvention.GetActualType(bsonReader, typeof(TKey));
        //        //var keySerializer = GetKeySerializer(keyType);
        //        //key = (TKey)keySerializer.Deserialize(bsonReader, typeof(TKey), keyType, keyValuePairSerializationOptions.KeySerializationOptions);
        //        //bsonReader.ReadBsonType();
        //        //var valueType = valueDiscriminatorConvention.GetActualType(bsonReader, typeof(TValue));
        //        //var valueSerializer = GetValueSerializer(valueType);
        //        //value = (TValue)valueSerializer.Deserialize(bsonReader, typeof(TValue), valueType, keyValuePairSerializationOptions.ValueSerializationOptions);
        //        key = bsonReader.ReadString();
        //        value = bsonReader.ReadString();
        //        bsonReader.ReadEndArray();
        //    }
        //    else if (bsonType == BsonType.Document)
        //    {
        //        // [{ "k" : "toto1", "v" : "tata1" }, { "k" : "toto2", "v" : "tata2" }]

        //        bsonReader.ReadStartDocument();

        //        var bsonTrie = new BsonTrie<bool>();
        //        bsonTrie.Add("k", true); // is key
        //        bsonTrie.Add("v", false);

        //        bool keyFound = false, valueFound = false;
        //        bool elementFound;
        //        bool elementIsKey;
        //        while (bsonReader.ReadBsonType(bsonTrie, out elementFound, out elementIsKey) != BsonType.EndOfDocument)
        //        {
        //            var name = bsonReader.ReadName();
        //            if (elementFound)
        //            {
        //                if (elementIsKey)
        //                {
        //                    //var keyType = keyDiscriminatorConvention.GetActualType(bsonReader, typeof(TKey));
        //                    //var keySerializer = GetValueSerializer(keyType);
        //                    //key = (TKey)keySerializer.Deserialize(bsonReader, typeof(TKey), keyType, keyValuePairSerializationOptions.KeySerializationOptions);
        //                    key = bsonReader.ReadString();
        //                    keyFound = true;
        //                }
        //                else
        //                {
        //                    //var valueType = valueDiscriminatorConvention.GetActualType(bsonReader, typeof(TValue));
        //                    //var valueSerializer = GetValueSerializer(valueType);
        //                    //value = (TValue)valueSerializer.Deserialize(bsonReader, typeof(TValue), valueType, keyValuePairSerializationOptions.ValueSerializationOptions);
        //                    value = bsonReader.ReadString();
        //                    valueFound = true;
        //                }
        //            }
        //            else
        //            {
        //                var message = string.Format("Element '{0}' is not valid for KeyValuePairs (expecting 'k' or 'v').", name);
        //                throw new BsonSerializationException(message);
        //            }
        //        }
        //        bsonReader.ReadEndDocument();

        //        if (!keyFound)
        //        {
        //            //throw new FileFormatException("KeyValuePair item was missing the 'k' element.");
        //            throw new PBException("KeyValuePair item was missing the 'k' element.");
        //        }
        //        if (!valueFound)
        //        {
        //            //throw new FileFormatException("KeyValuePair item was missing the 'v' element.");
        //            throw new PBException("KeyValuePair item was missing the 'v' element.");
        //        }
        //    }
        //    else
        //    {
        //        //var message = string.Format("Cannot deserialize '{0}' from BsonType {1}.", BsonUtils.GetFriendlyTypeName(typeof(KeyValuePair<string, string>)), bsonType);
        //        //throw new FileFormatException(message);
        //        throw new PBException("Cannot deserialize '{0}' from BsonType {1}.", BsonUtils.GetFriendlyTypeName(typeof(KeyValuePair<string, string>)), bsonType);
        //    }
        //    return new KeyValuePair<string, string>(key, value);
        //}

        /// <summary>
        /// Serializes an object to a BsonWriter.
        /// </summary>
        /// <param name="bsonWriter">The BsonWriter.</param>
        /// <param name="nominalType">The nominal type.</param>
        /// <param name="value">The object.</param>
        /// <param name="options">The serialization options.</param>
        public override void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
        {
            if (_trace)
                pb.Trace.WriteLine("NameValueCollectionSerializer.Serialize()");

            if (value == null)
            {
                bsonWriter.WriteNull();
            }
            else
            {
                // dont know why nominalType can be an object
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

                // json Dictionary
                // { "toto1" : "tata1", "toto2" : "tata2" }

                //var dictionary = (IDictionary)value;
                NameValueCollection nameValueCollection = (NameValueCollection)value;
                var dictionarySerializationOptions = EnsureSerializationOptions(options);
                var dictionaryRepresentation = dictionarySerializationOptions.Representation;
                var keyValuePairSerializationOptions = dictionarySerializationOptions.KeyValuePairSerializationOptions;

                if (dictionaryRepresentation == DictionaryRepresentation.Dynamic)
                {
                    // if some keys contain '$', '.' or '\0' serialize as ArrayOfArrays otherwise serialize as Document
                    dictionaryRepresentation = DictionaryRepresentation.Document;
                    foreach (string key in nameValueCollection.Keys)
                    {
                        //var name = key as string; // key might not be a string
                        if (string.IsNullOrEmpty(key) || key[0] == '$' || key.IndexOf('.') != -1 || key.IndexOf('\0') != -1)
                        {
                            dictionaryRepresentation = DictionaryRepresentation.ArrayOfArrays;
                            break;
                        }
                    }
                }

                switch (dictionaryRepresentation)
                {
                    case DictionaryRepresentation.Document:
                        bsonWriter.WriteStartDocument();
                        //foreach (DictionaryEntry dictionaryEntry in dictionary)
                        //{
                        //    bsonWriter.WriteName((string)dictionaryEntry.Key);
                        //    BsonSerializer.Serialize(bsonWriter, typeof(object), dictionaryEntry.Value, keyValuePairSerializationOptions.ValueSerializationOptions);
                        //}
                        for (int i = 0; i < nameValueCollection.Count; i++)
                        {
                            bsonWriter.WriteString(nameValueCollection.GetKey(i), nameValueCollection.Get(i));
                        }
                        bsonWriter.WriteEndDocument();
                        break;

                    case DictionaryRepresentation.ArrayOfArrays:
                    case DictionaryRepresentation.ArrayOfDocuments:
                        // override KeyValuePair representation if necessary
                        var keyValuePairRepresentation = (dictionaryRepresentation == DictionaryRepresentation.ArrayOfArrays) ? BsonType.Array : BsonType.Document;
                        if (keyValuePairSerializationOptions.Representation != keyValuePairRepresentation)
                        {
                            keyValuePairSerializationOptions = new KeyValuePairSerializationOptions(keyValuePairRepresentation, keyValuePairSerializationOptions.KeySerializationOptions,
                                keyValuePairSerializationOptions.ValueSerializationOptions);
                        }

                        bsonWriter.WriteStartArray();
                        //foreach (DictionaryEntry dictionaryEntry in dictionary)
                        for (int i = 0; i < nameValueCollection.Count; i++)
                        {
                            //var keyValuePair = new KeyValuePair<object, object>(dictionaryEntry.Key, dictionaryEntry.Value);
                            var keyValuePair = new KeyValuePair<string, string>(nameValueCollection.GetKey(i), nameValueCollection.Get(i));
                            //_keyValuePairSerializer.Serialize(bsonWriter, typeof(KeyValuePair<object, object>), keyValuePair, keyValuePairSerializationOptions);
                            _keyValuePairSerializer.Serialize(bsonWriter, typeof(KeyValuePair<string, string>), keyValuePair, keyValuePairSerializationOptions);
                        }
                        bsonWriter.WriteEndArray();

                        //bsonWriter.WriteStartArray();
                        //for (int i = 0; i < nameValueCollection.Count; i++)
                        //{
                        //    bsonWriter.WriteStartArray();
                        //    bsonWriter.WriteString(nameValueCollection.GetKey(i), nameValueCollection.Get(i));
                        //    bsonWriter.WriteEndArray();
                        //}
                        //bsonWriter.WriteEndArray();
                        break;
                    //case DictionaryRepresentation.ArrayOfDocuments:
                    //    bsonWriter.WriteStartArray();
                    //    for (int i = 0; i < nameValueCollection.Count; i++)
                    //    {
                    //        bsonWriter.WriteStartDocument();
                    //        bsonWriter.WriteString(nameValueCollection.GetKey(i), nameValueCollection.Get(i));
                    //        bsonWriter.WriteEndDocument();
                    //    }
                    //    bsonWriter.WriteEndArray();
                    //    break;
                    default:
                        var message = string.Format("'{0}' is not a valid IDictionary representation.", dictionaryRepresentation);
                        throw new BsonSerializationException(message);
                }
            }
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
                        var message = string.Format("BsonType {0} is not a valid representation for a Dictionary.", representationSerializationOptions.Representation);
                        throw new BsonSerializationException(message);
                }
            }

            return EnsureSerializationOptions<DictionarySerializationOptions>(options);
        }
    }
}
