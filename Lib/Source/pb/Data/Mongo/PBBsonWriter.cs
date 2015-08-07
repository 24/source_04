using System;
using System.IO;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using pb.IO;

namespace pb.Data.Mongo
{
    // IDisposable 
    public class PBBsonWriter : JsonWriter
    {
        //private BsonWriter _bsonWriter = null;
        //private StreamWriter _streamWriter = null;
        private TextWriter _writer = null;

        public PBBsonWriter(TextWriter writer, JsonWriterSettings settings)
            : base(writer, settings)
        {
            _writer = writer;
        }

        //public void Dispose()
        //{
        //    Close();
        //}

        //public void Close()
        //{
        //    //if (_bsonWriter != null)
        //    //{
        //    //    _bsonWriter.Close();
        //    //    _bsonWriter = null;
        //    //}
        //    if (_streamWriter != null)
        //    {
        //        _streamWriter.Close();
        //        _streamWriter = null;
        //    }
        //}

        public void WriteLine()
        {
            _writer.WriteLine();
        }

        //public static JsonWriter Open(string file, bool append = false)
        public static PBBsonWriter Open(string file, bool append = false)
        {
            zfile.CreateFileDirectory(file);
            FileMode mode;
            if (zFile.Exists(file))
            {
                if (append)
                    mode = FileMode.Append;
                else
                    mode = FileMode.Truncate;
            }
            else
                mode = FileMode.Create;
            StreamWriter streamWriter = new StreamWriter(zFile.Open(file, mode, FileAccess.Write, FileShare.Read));
            JsonWriterSettings jsonSettings = new JsonWriterSettings();
            jsonSettings.Indent = true;
            jsonSettings.OutputMode = JsonOutputMode.Shell;
            jsonSettings.CloseOutput = true;
            //return new JsonWriter(streamWriter, jsonSettings);
            return new PBBsonWriter(streamWriter, jsonSettings);
        }
    }

    public static partial class GlobalExtension
    {
        public static void zWriteLine(this PBBsonWriter bsonWriter)
        {
            if (bsonWriter == null)
                return;
            bsonWriter.WriteLine();
        }

        public static void zWriteName(this BsonWriter bsonWriter, string name)
        {
            if (bsonWriter == null)
                return;
            bsonWriter.WriteName(name);
        }

        public static void zWriteStartDocument(this BsonWriter bsonWriter)
        {
            if (bsonWriter == null)
                return;
            bsonWriter.WriteStartDocument();
        }

        public static void zWriteStartDocument(this BsonWriter bsonWriter, string name)
        {
            if (bsonWriter == null)
                return;
            bsonWriter.WriteStartDocument(name);
        }

        public static void zWriteEndDocument(this BsonWriter bsonWriter)
        {
            if (bsonWriter == null)
                return;
            bsonWriter.WriteEndDocument();
        }

        public static void zWriteStartArray(this BsonWriter bsonWriter)
        {
            if (bsonWriter == null)
                return;
            bsonWriter.WriteStartArray();
        }

        public static void zWriteStartArray(this BsonWriter bsonWriter, string name)
        {
            if (bsonWriter == null)
                return;
            bsonWriter.WriteStartArray(name);
        }

        public static void zWriteEndArray(this BsonWriter bsonWriter)
        {
            if (bsonWriter == null)
                return;
            bsonWriter.WriteEndArray();
        }

        public static void zWrite<T>(this BsonWriter bsonWriter, string name, T value)
        {
            if (bsonWriter == null)
                return;
            bsonWriter.WriteName(name);
            bsonWriter.zWrite(value);
        }

        public static void zWrite<T>(this BsonWriter bsonWriter, T value)
        {
            // implementation for the GetTypeCode method : protected virtual TypeCode GetTypeCodeImpl()
            // enum TypeCode : Empty = 0, Object = 1, DBNull = 2, Boolean = 3, Char = 4, SByte = 5, Byte = 6, Int16 = 7, UInt16 = 8, Int32 = 9, UInt32 = 10, Int64 = 11, UInt64 = 12, Single = 13, Double = 14, Decimal = 15, DateTime = 16, String = 18

            // (bool)value : error cannot convert T to bool
            // value as bool : error the as operator must be used with a reference type or nullable type
            // bool b = __refvalue(__makeref(value), bool);
            // Generic Parse Method without Boxing http://stackoverflow.com/questions/390286/generic-parse-method-without-boxing
            // UnCommon C# keywords - A Look http://www.codeproject.com/Articles/38695/UnCommon-C-keywords-A-Look#refv

            if (bsonWriter == null)
                return;

            Type type = typeof(T);
            if (value == null)
                bsonWriter.WriteNull();
            // nullable type int?, long?, ...
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = Nullable.GetUnderlyingType(type);
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Boolean:
                        bsonWriter.WriteBoolean((bool) __refvalue(__makeref(value), bool?));
                        break;
                    case TypeCode.DateTime:
                        bsonWriter.WriteDateTime(BsonUtils.ToMillisecondsSinceEpoch((DateTime) __refvalue(__makeref(value), DateTime?)));
                        break;
                    case TypeCode.Double:
                        bsonWriter.WriteDouble((double) __refvalue(__makeref(value), double?));
                        break;
                    case TypeCode.Int32:
                        bsonWriter.WriteInt32((int) __refvalue(__makeref(value), int?));
                        break;
                    case TypeCode.Int64:
                        bsonWriter.WriteInt64((long) __refvalue(__makeref(value), long?));
                        break;
                    default:
                        if (type == typeof(Date))
                            bsonWriter.WriteDateTime(BsonUtils.ToMillisecondsSinceEpoch(((Date) __refvalue(__makeref(value), Date?)).DateTime));
                        else
                            throw new PBException("unable to write type {0} with BsonWriter", type.zGetTypeName());
                        break;
                }
            }
            else
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Boolean:
                        bsonWriter.WriteBoolean( __refvalue(__makeref(value), bool));
                        break;
                    case TypeCode.DateTime:
                        bsonWriter.WriteDateTime(BsonUtils.ToMillisecondsSinceEpoch( __refvalue(__makeref(value), DateTime)));
                        break;
                    case TypeCode.Double:
                        bsonWriter.WriteDouble( __refvalue(__makeref(value), double));
                        break;
                    case TypeCode.Int32:
                        bsonWriter.WriteInt32( __refvalue(__makeref(value), int));
                        break;
                    case TypeCode.Int64:
                        bsonWriter.WriteInt64( __refvalue(__makeref(value), long));
                        break;
                    case TypeCode.String:
                        bsonWriter.WriteString(value as string);
                        break;
                    default:
                        if (type == typeof(byte[]))
                            bsonWriter.WriteBytes(value as byte[]);
                        else if (type == typeof(Date))
                            bsonWriter.WriteDateTime(BsonUtils.ToMillisecondsSinceEpoch( __refvalue(__makeref(value), Date).DateTime));
                        else
                            throw new PBException("unable to write type {0} with BsonWriter", type.zGetTypeName());
                        break;
                }
            }
        }

        public static void zSerialize(this BsonWriter bsonWriter, object value, Type nominalType = null)
        {
            if (bsonWriter == null)
                return;
            if (nominalType == null)
                nominalType = value.GetType();
            BsonSerializer.Serialize(bsonWriter, nominalType, value);
        }

        //public static void zWriteString(this BsonWriter bsonWriter, string value)
        //{
        //    if (value != null)
        //        bsonWriter.WriteString(value);
        //    else
        //        bsonWriter.WriteNull();
        //}

        //public static void zWriteString(this BsonWriter bsonWriter, string name, string value)
        //{
        //    if (value != null)
        //        bsonWriter.WriteString(name, value);
        //    else
        //        bsonWriter.WriteNull(name);
        //}
    }

}
