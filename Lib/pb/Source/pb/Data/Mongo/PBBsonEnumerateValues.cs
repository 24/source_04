using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace pb.Data.Mongo
{
    public class PBBsonNamedValue
    {
        public string Name;
        public BsonValue Value;
    }

    public class PBBsonEnumerateValues : IEnumerable<PBBsonNamedValue>, IEnumerator<PBBsonNamedValue>
    {
        private PBBsonReaderWithBookmark _reader;
        private string _currentName;
        private PBBsonNamedValue _currentValue;

        public PBBsonEnumerateValues(PBBsonReaderWithBookmark reader, string name = null)
        {
            _reader = reader;
            _currentName = name;
        }

        public IEnumerator<PBBsonNamedValue> GetEnumerator()
        {
            return this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this;
        }

        public PBBsonNamedValue Current
        {
            get { return _currentValue; }
        }

        public void Dispose()
        {
        }

        object System.Collections.IEnumerator.Current
        {
            get { return _currentValue; }
        }

        public bool MoveNext()
        {
            while (_reader.Read())
            {
                switch (_reader.Type)
                {
                    case PBBsonReaderType.Document:
                    case PBBsonReaderType.Array:
                        _currentName = AddToName(_currentName, _reader.Name);
                        break;
                    case PBBsonReaderType.EndOfDocument:
                    case PBBsonReaderType.EndOfArray:
                        _currentName = RemoveLastName(_currentName);
                        break;
                    case PBBsonReaderType.Value:
                        _currentValue = new PBBsonNamedValue { Name = AddToName(_currentName, _reader.Name), Value = _reader.Value };
                        return true;
                }
            }
            return false;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        private static string AddToName(string name, string newName)
        {
            if (newName != null)
            {
                if (name != null)
                    name += ".";
                name += newName;
            }
            return name;
        }

        private static string RemoveLastName(string name)
        {
            if (name != null)
            {
                int i = name.LastIndexOf('.');
                if (i != -1)
                    name = name.Substring(0, i);
                else
                    name = null;
            }
            return name;
        }
    }
}
