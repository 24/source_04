using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;

namespace pb.Data.Mongo
{
    /// <summary>
    /// Represents PB serialization provider.
    /// </summary>
    public class BsonPBSerializationProvider : IBsonSerializationProvider
    {
        // from BsonDefaultSerializationProvider

        // private static fields
        private static Dictionary<Type, Type> __serializers = new Dictionary<Type, Type>();
        private static DictionarySerializationOptions __dictionarySerializationOptions = DictionarySerializationOptions.ArrayOfDocuments;
        private static bool __activateGetCollectionSerializer = false;

        // static constructor
        //static BsonPBSerializationProvider()
        //{
        //    __serializers = new Dictionary<Type, Type>
        //    {
        //        { typeof(NameValueCollection), typeof(NameValueCollectionSerializer) },
        //        { typeof(ZValue), typeof(ZValueSerializer) },
        //        { typeof(ZInt), typeof(ZIntSerializer) },
        //        { typeof(ZString), typeof(ZStringSerializer) }
        //    };
        //}

        public static void RegisterProvider()
        {
            if (!BsonSerializer.zIsSerializationProviderRegistered(typeof(BsonPBSerializationProvider)))
            {
                //Trace.WriteLine("**** BsonSerializer.RegisterSerializationProvider(new BsonPBSerializationProvider())");
                BsonSerializer.RegisterSerializationProvider(new BsonPBSerializationProvider());
                //Trace.WriteLine("**** BsonSerializer.RegisterGenericSerializerDefinition(typeof(Dictionary<,>), typeof(DictionarySerializer<,>))");
                //BsonSerializer.RegisterGenericSerializerDefinition(typeof(Dictionary<,>), typeof(DictionarySerializer<,>));
            }
        }

        public static void UnregisterProvider()
        {
            if (BsonSerializer.zIsSerializationProviderRegistered(typeof(BsonPBSerializationProvider)))
            {
                //Trace.WriteLine("**** BsonSerializer.zUnregisterSerializationProvider(typeof(BsonPBSerializationProvider)");
                BsonSerializer.zUnregisterSerializationProvider(typeof(BsonPBSerializationProvider));
            }
        }

        //public static void RegisterSerializer(Type type, IBsonSerializer serializer)
        public static void RegisterSerializer(Type type, Type serializerType)
        {
            // don't allow a serializer to be registered for subclasses of BsonValue
            if (typeof(BsonValue).IsAssignableFrom(type))
            {
                throw new PBException("A serializer cannot be registered for type {0} because it is a subclass of BsonValue.", BsonUtils.GetFriendlyTypeName(type));
            }

            //__configLock.EnterWriteLock();
            //try
            //{
                if (__serializers.ContainsKey(type))
                {
                    //throw new PBException("There is already a serializer registered for type {0}.", type.FullName);
                    __serializers[type] = serializerType;
                }
                else
                    //__serializers.Add(type, serializer);
                    __serializers.Add(type, serializerType);
            //}
            //finally
            //{
            //    __configLock.ExitWriteLock();
            //}
        }

        public static bool IsSerializerRegistered(Type type)
        {
            //__configLock.EnterReadLock();
            //try
            //{
                if (__serializers.ContainsKey(type))
                    return true;
                else
                    return false;
            //}
            //finally
            //{
            //    __configLock.ExitReadLock();
            //}
        }

        public static void UnregisterSerializer(Type type)
        {
            // from http://dotnetinside.com/ru/type/MongoDB.Bson/BsonSerializer/0.9.1.20455
            //__configLock.EnterWriteLock();
            //try
            //{
                if (!__serializers.ContainsKey(type))
                {
                    //var message = string.Format("There is no serializer registered for type {0}.", type.FullName);
                    throw new PBException("There is no serializer registered for type {0}.", type.FullName);
                }
                __serializers.Remove(type);
            //}
            //finally
            //{
            //    __configLock.ExitWriteLock();
            //}
        }

        public static void SetDictionarySerializationOptions(DictionarySerializationOptions options)
        {
        }

        // constructors
        /// <summary>
        /// Initializes a new instance of the BsonDefaultSerializer class.
        /// </summary>
        public BsonPBSerializationProvider()
        {
        }

        public IBsonSerializer GetSerializer(Type type)
        {
            //Trace.WriteLine("**** BsonPBSerializationProvider.GetSerializer(\"{0}\")", type.zGetName());
            Type serializerType;
            if (__serializers.TryGetValue(type, out serializerType))
            {
                //Trace.WriteLine("**** found serializer : \"{0}\"", serializerType.zGetName());
                return (IBsonSerializer)Activator.CreateInstance(serializerType);
            }

            if (__activateGetCollectionSerializer)
            {
                IBsonSerializer serializer = GetCollectionSerializer(type);
                if (serializer != null)
                {
                    //Trace.WriteLine("**** found collection serializer : \"{0}\"", serializer.GetType().zGetName());
                    return serializer;
                }
            }
            //Trace.WriteLine("**** serializer not found");
            return null;
        }

        private IBsonSerializer GetCollectionSerializer(Type type)
        {
            // from MongoDB.Bson.Serialization.BsonDefaultSerializationProvider.GetCollectionSerializer()

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
