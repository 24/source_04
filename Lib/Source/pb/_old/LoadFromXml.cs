using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using pb;
using pb.IO;

namespace pb.old
{
    public abstract class LoadFromXml<T> : IEnumerable<T>, IEnumerator<T>
    {
        protected string _file;
        protected bool _loadImage = false;
        protected string _fileDirectory = null;
        protected XmlReader _xmlReader = null;

        public LoadFromXml(string file, bool loadImage = false)
        {
            _file = file;
            _loadImage = loadImage;
            Load();
        }

        public void Dispose()
        {
            if (_xmlReader != null)
            {
                _xmlReader.Close();
                _xmlReader = null;
            }
        }

        protected void Load()
        {
            _fileDirectory = zPath.GetDirectoryName(_file);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreWhitespace = true;
            _xmlReader = XmlReader.Create(_file, settings);

            while (!_xmlReader.EOF)
            {
                _xmlReader.Read();
                if (_xmlReader.NodeType == XmlNodeType.Element)
                    break;
            }

            _Load();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this;
        }

        object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }

        public bool MoveNext()
        {
            if (_MoveNext())
                return true;
            else
            {
                _xmlReader.Close();
                _xmlReader = null;
                return false;
            }
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        protected static void WriteLine(string msg, params object[] prm)
        {
            Trace.CurrentTrace.WriteLine(msg, prm);
        }

        public abstract T Current { get; }
        protected abstract void _Load();
        protected abstract bool _MoveNext();
    }
}
