using pb.Data.Mongo;
using pb.IO;

namespace pb.Data.OpenXml.Test
{
    public static class Test_OpenXml_Serialize
    {
        public static void Test_01(string file, bool jsonIndent = false)
        {
            string outputFile = zPath.Combine(zPath.GetDirectoryName(file), zPath.GetFileNameWithoutExtension(file) + "_out" + zPath.GetExtension(file));
            //zmongo.ReadFileAs<OXmlElement>(file).zSave(outputFile);
            zMongo.BsonRead<OXmlElement>(file).zSave(outputFile, jsonIndent);
        }
    }
}
