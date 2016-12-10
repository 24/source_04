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
    //class PBSerializationProvider_v2
    // from BsonDefaultSerializationProvider
    // from http://dotnetinside.com/ru/type/MongoDB.Bson/BsonSerializer/0.9.1.20455
    // WARNING dont work with multi task : modify access to variable with __configLock.EnterWriteLock() and __configLock.ExitWriteLock()
    // v2 : change _serializers from Dictionary<Type, Type> to Dictionary<Type, IBsonSerializer>
    public class PBSerializationProvider_v2 : IBsonSerializationProvider
    {
        private static bool _trace = false;
        private static PBSerializationProvider_v2 _instance = new PBSerializationProvider_v2();
        //private Dictionary<Type, Type> __serializers = new Dictionary<Type, Type>();
        private Dictionary<Type, IBsonSerializer> _serializers = new Dictionary<Type, IBsonSerializer>();
        private DictionarySerializationOptions _dictionarySerializationOptions = DictionarySerializationOptions.ArrayOfDocuments;
        private bool _activateGetCollectionSerializer = false;

        public static bool Trace { get { return _trace; } set { _trace = value; } }
        public static PBSerializationProvider_v2 Instance { get { return _instance; } }

        public void RegisterProvider()
        {
            if (!BsonSerializer.zIsSerializationProviderRegistered(typeof(PBSerializationProvider_v2)))
            {
                if (_trace)
                    pb.Trace.WriteLine("PBSerializationProvider_v2 : register provider");
                BsonSerializer.RegisterSerializationProvider(_instance);
                //BsonSerializer.RegisterGenericSerializerDefinition(typeof(Dictionary<,>), typeof(DictionarySerializer<,>));
            }
        }

        public void UnregisterProvider()
        {
            if (BsonSerializer.zIsSerializationProviderRegistered(typeof(PBSerializationProvider_v2)))
            {
                if (_trace)
                    pb.Trace.WriteLine("PBSerializationProvider_v2 : unregister provider");
                BsonSerializer.zUnregisterSerializationProvider(typeof(PBSerializationProvider_v2));
            }
        }

        // pas vraiment util
        //public void RegisterSerializer(Type type, Type serializerType)
        //{
        //    RegisterSerializer(type, (IBsonSerializer)Activator.CreateInstance(serializerType));
        //}

        public void RegisterSerializer(Type type, IBsonSerializer serializer)
        {
            // don't allow a serializer to be registered for subclasses of BsonValue
            if (typeof(BsonValue).IsAssignableFrom(type))
            {
                throw new PBException("A serializer cannot be registered for type {0} because it is a subclass of BsonValue.", BsonUtils.GetFriendlyTypeName(type));
            }

            if (_trace)
                pb.Trace.WriteLine($"PBSerializationProvider_v2 : register type \"{type.zGetTypeName()}\" serializer count {_serializers.Count + 1}");
            if (_serializers.ContainsKey(type))
            {
                _serializers[type] = serializer;
            }
            else
                _serializers.Add(type, serializer);
        }

        public bool IsSerializerRegistered(Type type)
        {
            if (_serializers.ContainsKey(type))
                return true;
            else
                return false;
        }

        public void UnregisterSerializer(Type type)
        {
            if (!_serializers.ContainsKey(type))
            {
                throw new PBException("There is no serializer registered for type {0}.", type.FullName);
            }
            if (_trace)
                pb.Trace.WriteLine($"PBSerializationProvider_v2 : unregister type \"{type.zGetTypeName()}\" serializer count {_serializers.Count - 1}");
            _serializers.Remove(type);
        }

        //public void SetDictionarySerializationOptions(DictionarySerializationOptions options)
        //{
        //}

        public IBsonSerializer GetSerializer(Type type)
        {
            //Type serializerType;
            IBsonSerializer serializer;
            if (_serializers.TryGetValue(type, out serializer))
            {
                if (_trace)
                    pb.Trace.WriteLine($"PBSerializationProvider_v2 : get serializer for type \"{type.zGetTypeName()}\"");
                //return (IBsonSerializer)Activator.CreateInstance(serializerType);
                return serializer;
            }

            if (_activateGetCollectionSerializer)
            {
                serializer = GetCollectionSerializer(type);
                if (serializer != null)
                {
                    if (_trace)
                        pb.Trace.WriteLine($"PBSerializationProvider_v2 : get collection serializer for type \"{type.zGetTypeName()}\"");
                    return serializer;
                }
            }
            if (_trace)
                pb.Trace.WriteLine($"PBSerializationProvider_v2 : serializer not found for type \"{type.zGetTypeName()}\" serializer count {_serializers.Count}");
            return null;
        }

        // from MongoDB.Bson.Serialization.BsonDefaultSerializationProvider.GetCollectionSerializer()
        // actualy not activated
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
