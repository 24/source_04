using System;
using System.IO;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using pb.Data.TraceData;
using System.Text;

namespace pb.Data.Mongo.TraceData
{
    public partial class JsonTraceDataWriter : TraceDataWriter, IDisposable
    {
        private StreamWriter _streamWriter = null;
        private bool _closeStreamWriter = false;
        private JsonWriterSettings _settings = null;

        public JsonTraceDataWriter(string file, JsonWriterSettings settings = null, Encoding encoding = null, bool append = false)
        {
            //_streamWriter = zFile.CreateText(file);
            Open(file, encoding, append);
            InitSettings(settings);
        }

        public JsonTraceDataWriter(StreamWriter streamWriter, JsonWriterSettings settings = null, bool closeStreamWriter = false)
        {
            _streamWriter = streamWriter;
            _closeStreamWriter = closeStreamWriter;
            InitSettings(settings);
        }

        public void Dispose()
        {
            Close();
        }

        private void Open(string file, Encoding encoding, bool append)
        {
            FileMode fileMode;
            if (append)
                fileMode = FileMode.Append;
            else
                fileMode = FileMode.Create;
            FileStream fs = new FileStream(file, fileMode, FileAccess.Write, FileShare.Read);
            if (encoding == null)
                encoding = new UTF8Encoding();  // no bom with new UTF8Encoding(), bom with Encoding.UTF8
            _streamWriter = new StreamWriter(fs, encoding);
            _closeStreamWriter = true;
        }

        public void Close()
        {
            if (_closeStreamWriter && _streamWriter != null)
            {
                _streamWriter.Close();
                _streamWriter = null;
            }
        }

        private void InitSettings(JsonWriterSettings settings)
        {
            if (settings != null)
                _settings = settings;
            else
            {
                _settings = new JsonWriterSettings();
                _settings.Indent = true;
            }
        }

        //public override void Write<T>(T data)
        public override void Write<T>(T data, Exception ex)
        {
            _streamWriter.WriteLine(data.ToJson(_settings));
        }
    }
}
