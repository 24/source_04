using System.Text;
using MongoDB.Bson.IO;
using pb.IO;

namespace pb.Data.Mongo.TraceData
{
    public partial class JsonTraceDataWriter
    {
        public JsonTraceDataWriter(string file, FileOption option, Encoding encoding = null, JsonWriterSettings settings = null)
        {
            file = file.zRootPath(zapp.GetAppDirectory());
            if (option == FileOption.IndexedFile)
                file = zfile.GetNewIndexedFileName(zPath.GetDirectoryName(file), zPath.GetFileName(file));
            //_streamWriter = zFile.CreateText(file);
            Open(file, encoding, option != FileOption.RazFile);
            _closeStreamWriter = true;
            InitSettings(settings);
        }
    }
}
