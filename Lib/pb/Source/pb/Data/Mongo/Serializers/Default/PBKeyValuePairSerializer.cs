using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;

namespace pb.Data.Mongo.Serializers
{
    public class PBKeyValuePairSerializer<TKey, TValue> : BsonBaseSerializer
    {
        private static readonly BsonTrie<bool> __bsonTrie = BuildBsonTrie();

        private volatile IDiscriminatorConvention _cachedKeyDiscriminatorConvention;
        private volatile IDiscriminatorConvention _cachedValueDiscriminatorConvention;
        private volatile IBsonSerializer _cachedKeySerializer;
        private volatile IBsonSerializer _cachedValueSerializer;

        public PBKeyValuePairSerializer()
            : base(new KeyValuePairSerializationOptions { Representation = BsonType.Document })
        {
        }

        public PBKeyValuePairSerializer(IBsonSerializationOptions defaultSerializationOptions)
            : base(defaultSerializationOptions)
        {
        }

        public override object Deserialize(BsonReader bsonReader, Type nominalType, Type actualType, IBsonSerializationOptions options)
        {
            VerifyTypes(nominalType, actualType, typeof(KeyValuePair<TKey, TValue>));
            var keyValuePairSerializationOptions = EnsureSerializationOptions<KeyValuePairSerializationOptions>(options);

            var keyDiscriminatorConvention = GetKeyDiscriminatorConvention();
            var valueDiscriminatorConvention = GetValueDiscriminatorConvention();
            TKey key;
            TValue value;

            var bsonType = bsonReader.GetCurrentBsonType();
            if (bsonType == BsonType.Array)
            {
                bsonReader.ReadStartArray();
                bsonReader.ReadBsonType();
                var keyType = keyDiscriminatorConvention.GetActualType(bsonReader, typeof(TKey));
                var keySerializer = GetKeySerializer(keyType);
                key = (TKey)keySerializer.Deserialize(bsonReader, typeof(TKey), keyType, keyValuePairSerializationOptions.KeySerializationOptions);
                bsonReader.ReadBsonType();
                var valueType = valueDiscriminatorConvention.GetActualType(bsonReader, typeof(TValue));
                var valueSerializer = GetValueSerializer(valueType);
                value = (TValue)valueSerializer.Deserialize(bsonReader, typeof(TValue), valueType, keyValuePairSerializationOptions.ValueSerializationOptions);
                bsonReader.ReadEndArray();
            }
            else if (bsonType == BsonType.Document)
            {
                bsonReader.ReadStartDocument();
                key = default(TKey);
                value = default(TValue);
                bool keyFound = false, valueFound = false;
                bool elementFound;
                bool elementIsKey;
                while (bsonReader.ReadBsonType(__bsonTrie, out elementFound, out elementIsKey) != BsonType.EndOfDocument)
                {
                    var name = bsonReader.ReadName();
                    if (elementFound)
                    {
                        if (elementIsKey)
                        {
                            var keyType = keyDiscriminatorConvention.GetActualType(bsonReader, typeof(TKey));
                            var keySerializer = GetValueSerializer(keyType);
                            key = (TKey)keySerializer.Deserialize(bsonReader, typeof(TKey), keyType, keyValuePairSerializationOptions.KeySerializationOptions);
                            keyFound = true;
                        }
                        else
                        {
                            var valueType = valueDiscriminatorConvention.GetActualType(bsonReader, typeof(TValue));
                            var valueSerializer = GetValueSerializer(valueType);
                            value = (TValue)valueSerializer.Deserialize(bsonReader, typeof(TValue), valueType, keyValuePairSerializationOptions.ValueSerializationOptions);
                            valueFound = true;
                        }
                    }
                    else
                    {
                        //var message = string.Format("Element '{0}' is not valid for KeyValuePairs (expecting 'k' or 'v').", name);
                        //throw new BsonSerializationException(message);
                        throw new PBException("Element '{0}' is not valid for KeyValuePairs (expecting 'k' or 'v').", name);
                    }
                }
                bsonReader.ReadEndDocument();

                if (!keyFound)
                {
                    //throw new FileFormatException("KeyValuePair item was missing the 'k' element.");
                    throw new PBException("KeyValuePair item was missing the 'k' element.");
                }
                if (!valueFound)
                {
                    //throw new FileFormatException("KeyValuePair item was missing the 'v' element.");
                    throw new PBException("KeyValuePair item was missing the 'v' element.");
                }
            }
            else
            {
                //var message = string.Format(
                //    "Cannot deserialize '{0}' from BsonType {1}.",
                //    BsonUtils.GetFriendlyTypeName(typeof(KeyValuePair<TKey, TValue>)),
                //    bsonType);
                //throw new FileFormatException(message);
                throw new PBException("Cannot deserialize '{0}' from BsonType {1}.", BsonUtils.GetFriendlyTypeName(typeof(KeyValuePair<TKey, TValue>)), bsonType);
            }

            return new KeyValuePair<TKey, TValue>(key, value);
        }

        public override void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
        {
            var keyValuePair = (KeyValuePair<TKey, TValue>)value;
            var keyValuePairSerializationOptions = EnsureSerializationOptions<KeyValuePairSerializationOptions>(options);

            var keyType = (keyValuePair.Key == null) ? typeof(TKey) : keyValuePair.Key.GetType();
            var keySerializer = GetKeySerializer(keyType);
            var valueType = (keyValuePair.Value == null) ? typeof(TValue) : keyValuePair.Value.GetType();
            var valueSerializer = GetValueSerializer(valueType);

            switch (keyValuePairSerializationOptions.Representation)
            {
                case BsonType.Array:
                    bsonWriter.WriteStartArray();
                    keySerializer.Serialize(bsonWriter, typeof(TKey), keyValuePair.Key, keyValuePairSerializationOptions.KeySerializationOptions);
                    valueSerializer.Serialize(bsonWriter, typeof(TValue), keyValuePair.Value, keyValuePairSerializationOptions.ValueSerializationOptions);
                    bsonWriter.WriteEndArray();
                    break;
                case BsonType.Document:
                    bsonWriter.WriteStartDocument();
                    bsonWriter.WriteName("k");
                    keySerializer.Serialize(bsonWriter, typeof(TKey), keyValuePair.Key, keyValuePairSerializationOptions.KeySerializationOptions);
                    bsonWriter.WriteName("v");
                    valueSerializer.Serialize(bsonWriter, typeof(TValue), keyValuePair.Value, keyValuePairSerializationOptions.ValueSerializationOptions);
                    bsonWriter.WriteEndDocument();
                    break;
                default:
                    //var message = string.Format(
                    //    "'{0}' is not a valid {1} representation.",
                    //    keyValuePairSerializationOptions.Representation,
                    //    BsonUtils.GetFriendlyTypeName(typeof(KeyValuePair<TKey, TValue>)));
                    //throw new BsonSerializationException(message);
                    throw new PBException("'{0}' is not a valid {1} representation.", keyValuePairSerializationOptions.Representation, BsonUtils.GetFriendlyTypeName(typeof(KeyValuePair<TKey, TValue>)));
            }
        }

        private static BsonTrie<bool> BuildBsonTrie()
        {
            var bsonTrie = new BsonTrie<bool>();
            bsonTrie.Add("k", true); // is key
            bsonTrie.Add("v", false);
            return bsonTrie;
        }

        private IDiscriminatorConvention GetKeyDiscriminatorConvention()
        {
            // return a cached discriminator convention when possible
            var discriminatorConvention = _cachedKeyDiscriminatorConvention;
            if (discriminatorConvention == null)
            {
                // it's possible but harmless for multiple threads to do the initial lookup at the same time
                discriminatorConvention = BsonSerializer.LookupDiscriminatorConvention(typeof(TKey));
                _cachedKeyDiscriminatorConvention = discriminatorConvention;
            }
            return discriminatorConvention;
        }

        private IBsonSerializer GetKeySerializer(Type actualType)
        {
            // return a cached serializer when possible
            if (actualType == typeof(TKey))
            {
                var serializer = _cachedKeySerializer;
                if (serializer == null)
                {
                    // it's possible but harmless for multiple threads to do the initial lookup at the same time
                    serializer = BsonSerializer.LookupSerializer(typeof(TKey));
                    _cachedKeySerializer = serializer;
                }
                return serializer;
            }
            else
            {
                return BsonSerializer.LookupSerializer(actualType);
            }
        }

        private IDiscriminatorConvention GetValueDiscriminatorConvention()
        {
            // return a cached discriminator convention when possible
            var discriminatorConvention = _cachedValueDiscriminatorConvention;
            if (discriminatorConvention == null)
            {
                // it's possible but harmless for multiple threads to do the initial lookup at the same time
                discriminatorConvention = BsonSerializer.LookupDiscriminatorConvention(typeof(TValue));
                _cachedValueDiscriminatorConvention = discriminatorConvention;
            }
            return discriminatorConvention;
        }

        private IBsonSerializer GetValueSerializer(Type actualType)
        {
            // return a cached serializer when possible
            if (actualType == typeof(TValue))
            {
                var serializer = _cachedValueSerializer;
                if (serializer == null)
                {
                    // it's possible but harmless for multiple threads to do the initial lookup at the same time
                    serializer = BsonSerializer.LookupSerializer(typeof(TValue));
                    _cachedValueSerializer = serializer;
                }
                return serializer;
            }
            else
            {
                return BsonSerializer.LookupSerializer(actualType);
            }
        }
    }
}
