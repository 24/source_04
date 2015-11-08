using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using pb.IO;
using pb.Reflection;

namespace pb.Data.Mongo
{
    public class ZBsonElement
    {
        public string name;
        //public BsonValue value;
        public string value;

        public ZBsonElement(BsonElement element)
        {
            name = element.Name;
            //value = element.Value;
            value = element.Value.ToString();
        }

        public static explicit operator ZBsonElement(BsonElement element)
        {
            if (element != null)
                return new ZBsonElement(element);
            else
                return null;
        }

        public override string ToString()
        {
            return string.Format("{0} : {1}", name, value);
        }
    }

    public static class zmongo
    {
        public static MongoCollection GetCollection(string collectionName, string databaseName, string serverName = null)
        {
            if (collectionName == null)
                throw new PBException("cant generate id, mongo collection is'nt defined");
            if (databaseName == null)
                throw new PBException("cant generate id, mongo database is'nt defined");
            if (serverName == null)
                serverName = "mongodb://localhost";
            return new MongoClient(serverName).GetServer().GetDatabase(databaseName).GetCollection(collectionName);
        }

        public static IEnumerable<IEnumerable<ZBsonElement>> GetBsonDocumentsEnumerate(IEnumerable<BsonDocument> docs)
        {
            foreach (BsonDocument doc in docs)
            {
                yield return GetBsonElementsEnumerate(doc);
            }
        }

        public static IEnumerable<ZBsonElement> GetBsonElementsEnumerate(BsonDocument doc)
        {
            foreach (BsonElement element in doc.Elements)
            {
                yield return (ZBsonElement)element;
            }
        }

        public static object GetDocumentId(object document)
        {
            // from CSharpDriver-1.9.1\source\MongoDB.Driver\MongoCollection.cs
            IBsonSerializer serializer = BsonSerializer.LookupSerializer(document.GetType());
            IBsonIdProvider idProvider = serializer as IBsonIdProvider;
            if (idProvider != null)
            {
                object id;
                Type idNominalType;
                IIdGenerator idGenerator;
                bool hasId = idProvider.GetDocumentId(document, out id, out idNominalType, out idGenerator);
                if (hasId)
                    return id;
            }
            return null;
        }

        public static T ReadFileAs<T>(string file, Encoding encoding = null)
        {
            return BsonSerializer.Deserialize<T>(zfile.ReadAllText(file, encoding));
        }

        //public static IEnumerable<BsonValue> BsonValueReader(string file, Encoding encoding = null)
        public static IEnumerable<T> BsonReader<T>(string file, Encoding encoding = null, bool deleteFile = false)
        {
            FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
            try
            {
                if (encoding == null)
                    encoding = Encoding.UTF8;
                StreamReader streamReader = new StreamReader(fileStream, encoding);
                BsonReader reader = MongoDB.Bson.IO.BsonReader.Create(streamReader);
                while (reader.ReadBsonType() != BsonType.EndOfDocument)
                {
                    yield return BsonSerializer.Deserialize<T>(reader);
                }
            }
            finally
            {
                fileStream.Close();
                if (deleteFile)
                    zFile.Delete(file);
            }
        }


        //public static IEnumerable<BsonValue> BsonValueReader(TextReader textReader)
        public static IEnumerable<T> BsonReader<T>(TextReader textReader)
        {
            BsonReader reader = MongoDB.Bson.IO.BsonReader.Create(textReader);
            while (reader.ReadBsonType() != BsonType.EndOfDocument)
            {
                yield return BsonSerializer.Deserialize<T>(reader);
            }
        }

        //public static void SaveToJsonFile(string file, IEnumerable<BsonValue> values)
        //{
        //    zfile.CreateFileDirectory(file);
        //    using (StreamWriter sw = File.CreateText(file))
        //    {
        //        JsonWriterSettings jsonSettings = new JsonWriterSettings();
        //        jsonSettings.Indent = true;
        //        foreach (var value in values)
        //        {
        //            sw.WriteLine(value.ToJson(jsonSettings));
        //        }
        //    }
        //}

        public static void SaveToJsonFile<T>(string file, IEnumerable<T> values)
        {
            zfile.CreateFileDirectory(file);
            using (StreamWriter sw = zFile.CreateText(file))
            {
                JsonWriterSettings jsonSettings = new JsonWriterSettings();
                jsonSettings.Indent = true;
                foreach (var value in values)
                {
                    sw.WriteLine(value.ToJson(jsonSettings));
                }
            }
        }

        //public static void SaveToJsonFile(string file, BsonValue value)
        //{
        //    zfile.CreateFileDirectory(file);
        //    using (StreamWriter sw = File.CreateText(file))
        //    {
        //        JsonWriterSettings jsonSettings = new JsonWriterSettings();
        //        jsonSettings.Indent = true;
        //        sw.WriteLine(value.ToJson(jsonSettings));
        //    }
        //}

        public static void SaveToJsonFile<T>(string file, T value)
        {
            zfile.CreateFileDirectory(file);
            using (StreamWriter sw = zFile.CreateText(file))
            {
                JsonWriterSettings jsonSettings = new JsonWriterSettings();
                jsonSettings.Indent = true;
                sw.WriteLine(value.ToJson(jsonSettings));
            }
        }
    }

    public static partial class GlobalExtension
    {
        //public static BsonDocument zToBsonDocument(this object o)
        //{
        //    if (o != null)
        //        return o.ToBsonDocument(o.GetType());
        //    else
        //        return o.ToBsonDocument();
        //}

        public static IEnumerable<BsonDocument> zToBsonDocuments<T>(this IEnumerable<T> values)
        {
            foreach (T value in values)
            {
                yield return value.zToBsonDocument();
            }
        }

        // new version 30/09/2014
        public static BsonDocument zToBsonDocument<T>(this T value)
        {
            if (value != null)
                return value.ToBsonDocument(value.GetType());
            else
                return value.ToBsonDocument();
        }

        public static string zToJson(this object o, bool multiLine = true)
        {
            if (multiLine)
            {
                MongoDB.Bson.IO.JsonWriterSettings ws = new MongoDB.Bson.IO.JsonWriterSettings();
                ws.Indent = true;
                ws.NewLineChars = "\r\n";
                if (o != null)
                    return o.ToJson(o.GetType(), ws);
                else
                    return o.ToJson(ws);
            }
            else if (o != null)
                return o.ToJson(o.GetType());
            else
                return o.ToJson();
        }

        public static void zTraceJson(this object o, bool multiLine = true)
        {
            Trace.WriteLine(o.zToJson());
        }

        public static object zGetDocumentId(this object document)
        {
            return zmongo.GetDocumentId(document);
        }

        public static string zGetFullName(this MongoDatabase database)
        {
            return database.Server.Instance.Address + "." + database.Name;
        }

        public static string zGetFullName(this MongoCollection collection)
        {
            return collection.Database.zGetFullName() + "." + collection.Name;
        }

        public static BsonValue zGet(this BsonValue value, string name)
        {
            //Trace.WriteLine("zGet : get value \"{0}\" from {1}", name, value != null ? value.GetType().zGetName() : "null");
            //return value.zGet(name.Split('.'));
            BsonElement element = value.zGetElement(name.Split('.'));
            if (element != null)
                return element.Value;
            else
                return null;
            //foreach (string name2 in name.Split('.'))
            //{
            //    //Trace.WriteLine("zGet : get value \"{0}\" from {1}", name2, value.GetType().zGetName());
            //    if (!(value is BsonDocument))
            //    {
            //        //Trace.WriteLine("zGet : value is not BsonDocument return null");
            //        return null;
            //    }
            //    BsonDocument document = (BsonDocument)value;
            //    if (!document.Contains(name2))
            //    {
            //        //Trace.WriteLine("zGet : value does not contain \"{0}\" return null", name2);
            //        return null;
            //    }
            //    value = document[name2];
            //}
            //return value;
        }

        //public static BsonValue zGet(this BsonValue value, IEnumerable<string> names)
        //{
        //    if (value == null)
        //        return null;
        //    foreach (string name in names)
        //    {
        //        //Trace.WriteLine("zGet : get value \"{0}\" from {1}", name2, value.GetType().zGetName());
        //        if (!(value is BsonDocument))
        //        {
        //            //Trace.WriteLine("zGet : value is not BsonDocument return null");
        //            return null;
        //        }
        //        BsonDocument document = (BsonDocument)value;
        //        if (!document.Contains(name))
        //        {
        //            //Trace.WriteLine("zGet : value does not contain \"{0}\" return null", name2);
        //            return null;
        //        }
        //        value = document[name];
        //    }
        //    return value;
        //}

        public static BsonValue zGet(this BsonValue value, int index)
        {
            if (value == null)
                return null;
            if (!(value is BsonArray))
            {
                //Trace.WriteLine("zGet : value is not BsonArray return null");
                return null;
            }
            BsonArray array = (BsonArray)value;
            if (index >= array.Count)
            {
                //Trace.WriteLine("zGet : value does not contain element index {0} return null", index);
                return null;
            }
            return array[index];
        }

        public static BsonElement zGetElement(this BsonValue value, string name)
        {
            return value.zGetElement(name.Split('.'));
        }

        public static BsonElement zGetElement(this BsonValue value, IEnumerable<string> names)
        {
            if (value == null)
                return null;
            BsonElement element = null;
            foreach (string name in names)
            {
                //Trace.WriteLine("zGet : get value \"{0}\" from {1}", name2, value.GetType().zGetName());
                if (!(value is BsonDocument))
                {
                    //Trace.WriteLine("zGet : value is not BsonDocument return null");
                    return null;
                }
                BsonDocument document = (BsonDocument)value;
                if (!document.Contains(name))
                {
                    //Trace.WriteLine("zGet : value does not contain \"{0}\" return null", name2);
                    return null;
                }
                //value = document[name];
                element = document.GetElement(name);
                value = element.Value;
            }
            return element;
        }

        public static bool zSet(this BsonValue value, string name, BsonValue newValue)
        {
            if (value == null)
                return false;
            if (newValue == null)
                newValue = BsonNull.Value;
            string[] names = name.Split('.');
            //value = value.zGet(names.Take(names.Length - 1));
            //if (value != null && value is BsonDocument)
            //{
                //BsonDocument document = (BsonDocument)value;
                BsonDocument document = value.zCreateDocuments(names.Take(names.Length - 1));
                string name2 = names[names.Length - 1];
                if (document.Contains(name2))
                    document[name2] = newValue;
                else
                    document.Add(name2, newValue);
                return true;
            //}
            //return false;
        }

        public static BsonDocument zCreateDocuments(this BsonValue value, IEnumerable<string> names)
        {
            if (value == null)
                throw new PBException("unable to create documents from a null BsonValue");
            foreach (string name in names)
            {
                if (!(value is BsonDocument))
                    throw new PBException("unable to create documents, value is not a document ({0})", value.GetType().zGetTypeName());
                BsonDocument document = (BsonDocument)value;
                if (!document.Contains(name))
                {
                    value = new BsonDocument();
                    document.Add(name, value);
                }
                else
                    value = document[name];
            }
            return value.AsBsonDocument;
        }

        public static bool zRemove(this BsonValue value, string name)
        {
            string[] names = name.Split('.');
            //value = value.zGet(names.Take(names.Length - 1));
            BsonElement element = value.zGetElement(names.Take(names.Length - 1));
            //if (value != null && value is BsonDocument)
            if (element != null && element.Value is BsonDocument)
            {
                //BsonDocument document = (BsonDocument)value;
                BsonDocument document = (BsonDocument)element.Value;
                string name2 = names[names.Length - 1];
                if (document.Contains(name2))
                {
                    document.Remove(name2);
                    return true;
                }
            }
            return false;
        }

        //public static T zDeserialize<T>(this BsonDocument document)
        public static T zDeserialize<T>(this BsonValue value)
        {
            if (value == null || !(value is BsonDocument))
                return default(T);
            return BsonSerializer.Deserialize<T>((BsonDocument)value);
        }

        //public static BsonValue zGet(this BsonDocument document, string name)
        //{
        //    if (document == null || !document.Contains(name))
        //        return null;
        //    return document[name];
        //}

        public static BsonDocument zAsBsonDocument(this BsonValue bsonValue)
        {
            return bsonValue == null || bsonValue is BsonNull ? null : bsonValue.AsBsonDocument;
        }

        public static string zAsString(this BsonValue bsonValue)
        {
            return bsonValue == null || bsonValue is BsonNull ? null : bsonValue.AsString;
        }

        public static bool zAsBoolean(this BsonValue bsonValue)
        {
            return bsonValue == null || bsonValue is BsonNull ? false : bsonValue.AsBoolean;
        }

        public static int zAsInt(this BsonValue bsonValue)
        {
            return bsonValue == null || bsonValue is BsonNull ? 0 : bsonValue.AsInt32;
        }

        public static double zAsDouble(this BsonValue bsonValue)
        {
            return bsonValue == null || bsonValue is BsonNull ? 0.0 : bsonValue.BsonType == BsonType.Int32 ? bsonValue.AsInt32 : bsonValue.AsDouble;
        }

        public static DateTime zAsDateTime(this BsonValue bsonValue)
        {
            return bsonValue == null || bsonValue is BsonNull ? DateTime.MinValue : bsonValue.BsonType == BsonType.String ? DateTime.Parse(bsonValue.AsString) : (DateTime)bsonValue.AsBsonDateTime;
        }

        public static DateTime? zAsNullableDateTime(this BsonValue bsonValue)
        {
            return bsonValue == null || bsonValue is BsonNull ? null : bsonValue.BsonType == BsonType.String ? (DateTime?)DateTime.Parse(bsonValue.AsString) : (DateTime?)(DateTime)bsonValue.AsBsonDateTime;
        }

        public static BsonArray zAsBsonArray(this BsonValue bsonValue)
        {
            return bsonValue == null || bsonValue is BsonNull ? null : bsonValue.AsBsonArray;
        }

        public static IEnumerable<string> zAsStrings(this BsonArray bsonArray)
        {
            foreach (BsonValue bsonValue in bsonArray)
            {
                yield return bsonValue.zAsString();
            }
        }

        public static void zSave<T>(this T value, string file)
        {
            //value.zToBsonDocument().zSaveToJsonFile(file);
            //zmongo.SaveToJsonFile(file, value.zToBsonDocument());
            //Trace.WriteLine("zSave() single value");
            zmongo.SaveToJsonFile(file, value);
        }

        public static void zSave<T>(this IEnumerable<T> values, string file)
        {
            //values.zToBsonDocuments().zSaveToJsonFile(file);
            //zmongo.SaveToJsonFile(file, values.zToBsonDocuments());
            //Trace.WriteLine("zSave() enumerable values");
            zmongo.SaveToJsonFile(file, values);
        }

        // IOrderedEnumerable<T> is returned by OrderBy
        // if no zSave extension for IOrderedEnumerable<T> c# compiler call zSave(this T value, string file)
        public static void zSave<T>(this IOrderedEnumerable<T> values, string file)
        {
            //Trace.WriteLine("zSave() ordered enumerable values");
            zmongo.SaveToJsonFile(file, values as IEnumerable<T>);
        }

        //public static void zSave(this BsonValue value, string file)
        //{
        //    zmongo.SaveToJsonFile(file, value);
        //}

        //public static void zSave(this IEnumerable<BsonValue> values, string file)
        //{
        //    zmongo.SaveToJsonFile(file, values);
        //}

        //public static void zSaveToJsonFile(this BsonValue value, string file)
        //{
        //    zmongo.SaveToJsonFile(file, value);
        //}

        //public static void zSaveToJsonFile(this IEnumerable<BsonValue> values, string file)
        //{
        //    zmongo.SaveToJsonFile(file, values);
        //}
    }
}
