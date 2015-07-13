using MongoDB.Bson;
using MongoDB.Bson.IO;

namespace pb.Data.Mongo
{
    public enum PBBsonReaderType
    {
        Value = 1,
        Document,
        EndOfDocument,
        Array,
        EndOfArray
    }

    public class PBBsonReaderBookmark
    {
        public BsonReaderBookmark Bookmark;
        public PBBsonReaderType Type;
        public string Name;
        public BsonType BsonType;
        public BsonValue Value;
        public int Indent;
        public string IndentString;
    }

    public class PBBsonReader
    {
        private static int __traceLevel = 0;  // 0 no trace, 1 trace compact, 2 trace detail
        private BsonReader _reader;
        private PBBsonReaderType _type;
        private string _name;
        private BsonType _bsonType;
        private BsonValue _value;
        private int _indent = 0;
        private string _indentString = null;

        public PBBsonReader(BsonReader reader)
        {
            _reader = reader;
        }

        public static int TraceLevel { get { return __traceLevel; } set { __traceLevel = value; } }

        public PBBsonReaderType Type { get { return _type; } }
        public string Name { get { return _name; } }
        public BsonValue Value { get { return _value; } }

        public PBBsonReader Clone()
        {
            PBBsonReader clone = new PBBsonReader(_reader.Clone());
            clone._type = _type;
            clone._name = _name;
            clone._bsonType = _bsonType;
            clone._value = _value;
            clone._indent = _indent;
            clone._indentString = _indentString;
            return clone;
        }

        public PBBsonReaderBookmark GetBookmark()
        {
            return new PBBsonReaderBookmark { Bookmark = _reader.GetBookmark(), Type = _type, Name = _name, BsonType = _bsonType, Value = _value, Indent = _indent, IndentString = _indentString };
        }

        public void ReturnToBookmark(PBBsonReaderBookmark bookmark)
        {
            _reader.ReturnToBookmark(bookmark.Bookmark);
            _type = bookmark.Type;
            _name = bookmark.Name;
            _bsonType = bookmark.BsonType;
            _value = bookmark.Value;
            _indent = bookmark.Indent;
            _indentString = bookmark.IndentString;
        }

        public bool Read()
        {
            do
            {
                WriteLine(2, "state {0}", _reader.State);
                switch (_reader.State)
                {
                    case BsonReaderState.Done:
                        return false;
                    case BsonReaderState.Initial:
                    case BsonReaderState.Type:
                        _name = null;
                        _value = null;
                        _bsonType = _reader.ReadBsonType();
                        WriteLine(2, "ReadBsonType type {0}", _bsonType);
                        break;
                    case BsonReaderState.EndOfArray:
                        WriteLine(2, "ReadEndArray");
                        //_indent -= 2;
                        Indent(-2);
                        //WriteLine(1, "{0}]", "".PadRight(_indent));
                        WriteLine(1, "{0}]", _indentString);
                        _reader.ReadEndArray();
                        _type = PBBsonReaderType.EndOfArray;
                        break;
                    case BsonReaderState.EndOfDocument:
                        WriteLine(2, "ReadEndDocument");
                        //_indent -= 2;
                        Indent(-2);
                        //WriteLine(1, "{0}}}", "".PadRight(_indent));
                        WriteLine(1, "{0}}}", _indentString);
                        _reader.ReadEndDocument();
                        _type = PBBsonReaderType.EndOfDocument;
                        break;
                    case BsonReaderState.Name:
                        _name = _reader.ReadName();
                        WriteLine(2, "ReadName : name {0}", _name);
                        //Trace.Write("{0}{1}: ", "".PadRight(_indent), _name);
                        break;
                    case BsonReaderState.Value:
                        //Write(1, "{0}", "".PadRight(_indent));
                        Write(1, "{0}", _indentString);
                        if (_name != null)
                            Write(1, "{0}: ", _name);
                        ReadValue();
                        if (_type == PBBsonReaderType.Value)
                        {
                            WriteLine(2, "ReadValue : value {0} ({1})", _value, _value.BsonType);
                            WriteLine(1, "{0} ({1})", _value, _value.BsonType);
                        }
                        break;
                }
            } while (_reader.State != BsonReaderState.Type && _reader.State != BsonReaderState.Done);
            return true;
        }

        private void ReadValue()
        {
            //bool value = true;
            _type = PBBsonReaderType.Value;
            switch (_bsonType)
            {
                case BsonType.Document:
                    WriteLine(2, "ReadStartDocument");
                    WriteLine(1, "{");
                    //_indent += 2;
                    Indent(2);
                    _reader.ReadStartDocument();
                    _type = PBBsonReaderType.Document;
                    //value = false;
                    break;
                case BsonType.Array:
                    WriteLine(2, "ReadStartArray");
                    WriteLine(1, "[");
                    //_indent += 2;
                    Indent(2);
                    _reader.ReadStartArray();
                    _type = PBBsonReaderType.Array;
                    //value = false;
                    break;
                case BsonType.Binary:
                    _value = BsonValue.Create(_reader.ReadBytes());
                    break;
                case BsonType.Boolean:
                    _value = BsonValue.Create(_reader.ReadBoolean());
                    break;
                case BsonType.DateTime:
                    _value = BsonValue.Create(_reader.ReadDateTime());
                    break;
                case BsonType.Double:
                    _value = BsonValue.Create(_reader.ReadDouble());
                    break;
                case BsonType.Int32:
                    _value = BsonValue.Create(_reader.ReadInt32());
                    break;
                case BsonType.Int64:
                    _value = BsonValue.Create(_reader.ReadInt64());
                    break;
                case BsonType.JavaScript:
                    _value = BsonValue.Create(_reader.ReadJavaScript());
                    break;
                case BsonType.JavaScriptWithScope:
                    _value = BsonValue.Create(_reader.ReadJavaScriptWithScope());
                    break;
                case BsonType.MaxKey:
                    _reader.ReadMaxKey();
                    _value = BsonMaxKey.Value;
                    break;
                case BsonType.MinKey:
                    _reader.ReadMinKey();
                    _value = BsonMinKey.Value;
                    break;
                case BsonType.Null:
                    _reader.ReadNull();
                    _value = BsonNull.Value;
                    break;
                case BsonType.ObjectId:
                    _value = BsonValue.Create(_reader.ReadObjectId());
                    break;
                case BsonType.RegularExpression:
                    _value = BsonValue.Create(_reader.ReadRegularExpression());
                    break;
                case BsonType.String:
                    _value = BsonValue.Create(_reader.ReadString());
                    break;
                case BsonType.Symbol:
                    _value = BsonValue.Create(_reader.ReadSymbol());
                    break;
                case BsonType.Timestamp:
                    _value = BsonValue.Create(_reader.ReadTimestamp());
                    break;
                case BsonType.Undefined:
                    _reader.ReadUndefined();
                    _value = BsonUndefined.Value;
                    break;
                default:
                    throw new PBException("error unable to read value type {0}", _bsonType);
            }
            //return value;
        }

        private void Indent(int indent)
        {
            _indent += indent;
            _indentString = new string(' ', _indent);
        }

        private void Write(int traceLevel, string message, params object[] prm)
        {
            if (traceLevel == __traceLevel)
            {
                Trace.Write(message, prm);
            }
        }

        private void WriteLine(int traceLevel, string message, params object[] prm)
        {
            if (traceLevel == __traceLevel)
            {
                Trace.WriteLine(message, prm);
            }
        }
    }
}
