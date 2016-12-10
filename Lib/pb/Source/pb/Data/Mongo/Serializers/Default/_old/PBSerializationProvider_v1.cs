using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
using pb.Reflection;

namespace pb.Data.Mongo.Serializers
{
    // from BsonDefaultSerializationProvider
    // from http://dotnetinside.com/ru/type/MongoDB.Bson/BsonSerializer/0.9.1.20455
    // WARNING dont work with multi task : modify access to variable with __configLock.EnterWriteLock() and __configLock.ExitWriteLock()
    public class PBSerializationProvider_v1 : IBsonSerializationProvider
    {
        private static bool _trace = false;
        private static Dictionary<Type, Type> __serializers = new Dictionary<Type, Type>();
        private static DictionarySerializationOptions __dictionarySerializationOptions = DictionarySerializationOptions.ArrayOfDocuments;
        private static bool __activateGetCollectionSerializer = false;

        public static bool Trace { get { return _trace; } set { _trace = value; } }

        public static void RegisterProvider()
        {
            if (!BsonSerializer.zIsSerializationProviderRegistered(typeof(PBSerializationProvider_v1)))
            {
                if (_trace)
                    pb.Trace.WriteLine("PBSerializationProvider_v1 : register provider");
                BsonSerializer.RegisterSerializationProvider(new PBSerializationProvider_v1());
                //BsonSerializer.RegisterGenericSerializerDefinition(typeof(Dictionary<,>), typeof(DictionarySerializer<,>));
            }
        }

        public static void UnregisterProvider()
        {
            if (BsonSerializer.zIsSerializationProviderRegistered(typeof(PBSerializationProvider_v1)))
            {
                if (_trace)
                    pb.Trace.WriteLine("PBSerializationProvider_v1 : unregister provider");
                BsonSerializer.zUnregisterSerializationProvider(typeof(PBSerializationProvider_v1));
            }
        }

        public static void RegisterSerializer(Type type, Type serializerType)
        {
            // don't allow a serializer to be registered for subclasses of BsonValue
            if (typeof(BsonValue).IsAssignableFrom(type))
            {
                throw new PBException("A serializer cannot be registered for type {0} because it is a subclass of BsonValue.", BsonUtils.GetFriendlyTypeName(type));
            }

            if (_trace)
                pb.Trace.WriteLine($"PBSerializationProvider_v1 : register type \"{type.zGetTypeName()}\"");
            if (__serializers.ContainsKey(type))
            {
                __serializers[type] = serializerType;
            }
            else
                //__serializers.Add(type, serializer);
                __serializers.Add(type, serializerType);
        }

        public static bool IsSerializerRegistered(Type type)
        {
            if (__serializers.ContainsKey(type))
                return true;
            else
                return false;
        }

        public static void UnregisterSerializer(Type type)
        {
            if (!__serializers.ContainsKey(type))
            {
                throw new PBException("There is no serializer registered for type {0}.", type.FullName);
            }
            if (_trace)
                pb.Trace.WriteLine($"PBSerializationProvider_v1 : unregister type \"{type.zGetTypeName()}\"");
            __serializers.Remove(type);
        }

        //public static void SetDictionarySerializationOptions(DictionarySerializationOptions options)
        //{
        //}

        //public PBSerializationProvider()
        //{
        //}

        public IBsonSerializer GetSerializer(Type type)
        {
            Type serializerType;
            if (__serializers.TryGetValue(type, out serializerType))
            {
                if (_trace)
                    pb.Trace.WriteLine($"PBSerializationProvider_v1 : get serializer for type \"{type.zGetTypeName()}\"");
                return (IBsonSerializer)Activator.CreateInstance(serializerType);
            }

            if (__activateGetCollectionSerializer)
            {
                IBsonSerializer serializer = GetCollectionSerializer(type);
                if (serializer != null)
                {
                    if (_trace)
                        pb.Trace.WriteLine($"PBSerializationProvider_v1 : get collection serializer for type \"{type.zGetTypeName()}\"");
                    return serializer;
                }
            }
            if (_trace)
                pb.Trace.WriteLine($"PBSerializationProvider_v1 : serializer not found for type \"{type.zGetTypeName()}\" serializer count {__serializers.Count}");
            return null;
        }

        // from MongoDB.Bson.Serialization.BsonDefaultSerializationProvider.GetCollectionSerializer()
        private IBsonSerializer GetCollectionSerializer(Type type)
        {
            Type implementedGenericDictionaryInterface = null;
            Type implementedGenericEnumerableInterface = null;
            Type implementedDictionaryInterface = null;
            Type implementedEnumerableInterface = null;

            var implementedInterfaces = new List<Type>(type.GetInterfaces());
            if (type.IsInterface)
            {
                implementedInterfaces.Add(type);
            }
            foreach (var implementedInterface in implementedInterfaces)
            {
                if (implementedInterface.IsGenericType)
                {
                    var genericInterfaceDefinition = implementedInterface.GetGenericTypeDefinition();
                    if (genericInterfaceDefinition == typeof(IDictionary<,>))
                    {
                        implementedGenericDictionaryInterface = implementedInterface;
                    }
                    if (genericInterfaceDefinition == typeof(IEnumerable<>))
                    {
                        implementedGenericEnumerableInterface = implementedInterface;
                    }
                }
                else
                {
                    if (implementedInterface == typeof(IDictionary))
                    {
                        implementedDictionaryInterface = implementedInterface;
                    }
                    if (implementedInterface == typeof(IEnumerable))
                    {
                        implementedEnumerableInterface = implementedInterface;
                    }
                }
            }

            // the order of the tests is important
            if (implementedGenericDictionaryInterface != null)
            {
                var keyType = implementedGenericDictionaryInterface.GetGenericArguments()[0];
                var valueType = implementedGenericDictionaryInterface.GetGenericArguments()[1];
                //var genericSerializerDefinition = typeof(DictionarySerializer<,>);
                var genericSerializerDefinition = BsonSerializer.LookupGenericSerializerDefinition(typeof(Dictionary<,>));
                if (genericSerializerDefinition == null)
                    //throw new PBException("error missing generic dictionary serializer");
                    genericSerializerDefinition = typeof(DictionarySerializer<,>);
                var genericSerializerType = genericSerializerDefinition.MakeGenericType(keyType, valueType);
                return (IBsonSerializer)Activator.CreateInstance(genericSerializerType);
            }
            else if (implementedDictionaryInterface != null)
            {
                IBsonSerializer serializer = BsonSerializer.LookupSerializer(typeof(IDictionary));
                if (serializer != null)
                    return serializer;
                return new DictionarySerializer();
            }
            else if (implementedGenericEnumerableInterface != null)
            {
                var valueType = implementedGenericEnumerableInterface.GetGenericArguments()[0];
                var readOnlyCollectionType = typeof(ReadOnlyCollection<>).MakeGenericType(valueType);
                Type genericSerializerDefinition;
                if (readOnlyCollectionType.IsAssignableFrom(type))
                {
                    //genericSerializerDefinition = typeof(ReadOnlyCollectionSerializer<>);
                    genericSerializerDefinition = BsonSerializer.LookupGenericSerializerDefinition(typeof(ReadOnlyCollection<>));
                    if (genericSerializerDefinition == null)
                        genericSerializerDefinition = typeof(ReadOnlyCollectionSerializer<>);
                    if (type != readOnlyCollectionType)
                    {
                        BsonSerializer.RegisterDiscriminator(type, type.Name);
                    }
                }
                else
                {
                    //genericSerializerDefinition = typeof(EnumerableSerializer<>);
                    genericSerializerDefinition = BsonSerializer.LookupGenericSerializerDefinition(typeof(IEnumerable<>));
                    if (genericSerializerDefinition == null)
                        genericSerializerDefinition = typeof(EnumerableSerializer<>);
                }
                var genericSerializerType = genericSerializerDefinition.MakeGenericType(valueType);
                return (IBsonSerializer)Activator.CreateInstance(genericSerializerType);
            }
            else if (implementedEnumerableInterface != null)
            {
                IBsonSerializer serializer = BsonSerializer.LookupSerializer(typeof(IEnumerable));
                if (serializer != null)
                    return serializer;
                return new EnumerableSerializer();
            }

            return null;
        }
    }
}
