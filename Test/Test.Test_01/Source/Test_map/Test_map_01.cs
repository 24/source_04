using System.Collections.Generic;
using MongoDB.Bson;
using pb;
using pb.Data.Mongo;
using System.Data;
using System.Linq;
using pb.Data;
using pb.IO;
using System.IO;

namespace Test.Test_map
{
    public static class Test_map_01
    {
        public static void CreateTopojsonData(string topojsonFile, string topojsonDataFile)
        {
            using (FileStream fs = zFile.OpenWrite(topojsonDataFile))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    DataTable dt = BsonDocumentsToDataTable_v2.ToDataTable(Test_map_01.TransformTopojson(zmongo.BsonRead<BsonDocument>(topojsonFile).First()));
                    TraceDataTable.Trace(dt, sw);
                    sw.WriteLine();
                    string[] columns = new string[] { "type", "properties.scalerank", "properties.featurecla", "properties.labelrank", "properties.adm0_dif", "properties.level", "properties.type", "properties.su_dif", "properties.brk_diff", "properties.mapcolor7", "properties.mapcolor8", "properties.mapcolor9", "properties.mapcolor13" };
                    TraceDataTable.Trace(DataTableDistinct.GetDistinctValues(dt, columns), sw);
                }
            }

        }

        public static IEnumerable<BsonDocument> TransformTopojson(BsonDocument doc)
        {
            BsonValue geometries = doc.zGet("objects.data.geometries");
            if (geometries == null)
            {
                //Trace.WriteLine("objects.data.geometries not found");
                //yield break;
                throw new PBException("objects.data.geometries not found");
            }
            if (!geometries.IsBsonArray)
            {
                //Trace.WriteLine("objects.data.geometries is not an array");
                //yield break;
                throw new PBException("objects.data.geometries is not an array");
            }

            //Dictionary<int, int> scalerank = new Dictionary<int, int>();
            //Dictionary<string, string> featurecla = new Dictionary<string, string>();
            //Dictionary<int, int> labelrank = new Dictionary<int, int>();
            //Dictionary<int, int> adm0_dif = new Dictionary<int, int>();
            //Dictionary<int, int> level = new Dictionary<int, int>();
            //Dictionary<string, string> type = new Dictionary<string, string>();
            //Dictionary<int, int> su_dif = new Dictionary<int, int>();
            //Dictionary<int, int> brk_diff = new Dictionary<int, int>();
            //Dictionary<int, int> mapcolor7 = new Dictionary<int, int>();
            //Dictionary<int, int> mapcolor8 = new Dictionary<int, int>();
            //Dictionary<int, int> mapcolor9 = new Dictionary<int, int>();
            //Dictionary<int, int> mapcolor13 = new Dictionary<int, int>();

            int count = 0;
            foreach (BsonValue geometry in geometries.AsBsonArray)
            {
                if (!geometry.IsBsonDocument)
                {
                    Trace.WriteLine("geometry is not a document");
                    continue;
                }
                BsonValue properties = geometry.zGet("properties");
                //if (properties.IsBsonDocument)
                //{
                //    BsonDocument propertiesDoc = properties.AsBsonDocument;
                //    int i = propertiesDoc.zGet("scalerank").zAsInt();
                //    scalerank[i] = i;
                //    string text = propertiesDoc.zGet("featurecla").zAsString();
                //    featurecla[text] = text;
                //    i = propertiesDoc.zGet("labelrank").zAsInt();
                //    labelrank[i] = i;
                //    i = propertiesDoc.zGet("adm0_dif").zAsInt();
                //    adm0_dif[i] = i;
                //    i = propertiesDoc.zGet("level").zAsInt();
                //    level[i] = i;
                //    text = propertiesDoc.zGet("type").zAsString();
                //    type[text] = text;
                //    i = propertiesDoc.zGet("su_dif").zAsInt();
                //    su_dif[i] = i;
                //    i = propertiesDoc.zGet("brk_diff").zAsInt();
                //    brk_diff[i] = i;
                //    i = propertiesDoc.zGet("mapcolor7").zAsInt();
                //    mapcolor7[i] = i;
                //    i = propertiesDoc.zGet("mapcolor8").zAsInt();
                //    mapcolor8[i] = i;
                //    i = propertiesDoc.zGet("mapcolor9").zAsInt();
                //    mapcolor9[i] = i;
                //    i = propertiesDoc.zGet("mapcolor13").zAsInt();
                //    mapcolor13[i] = i;
                //}
                yield return new BsonDocument { { "type", geometry .zGet("type") }, { "properties", properties } };
                count++;
            }
            //Trace.WriteLine($"{count} geometries");
            //Trace.WriteLine($"scalerank  {scalerank.Count} : {scalerank.Keys.zToStringValues()}");
            //Trace.WriteLine($"featurecla {featurecla.Count} : {featurecla.Keys.zToStringValues()}");
            //Trace.WriteLine($"labelrank  {labelrank.Count} : {labelrank.Keys.zToStringValues()}");
            //Trace.WriteLine($"adm0_dif   {adm0_dif.Count} : {adm0_dif.Keys.zToStringValues()}");
            //Trace.WriteLine($"level      {level.Count} : {level.Keys.zToStringValues()}");
            //Trace.WriteLine($"type       {type.Count} : {type.Keys.zToStringValues()}");
            //Trace.WriteLine($"su_dif     {su_dif.Count} : {su_dif.Keys.zToStringValues()}");
            //Trace.WriteLine($"brk_diff   {brk_diff.Count} : {brk_diff.Keys.zToStringValues()}");
            //Trace.WriteLine($"mapcolor7  {mapcolor7.Count} : {mapcolor7.Keys.zToStringValues()}");
            //Trace.WriteLine($"mapcolor8  {mapcolor8.Count} : {mapcolor8.Keys.zToStringValues()}");
            //Trace.WriteLine($"mapcolor9  {mapcolor9.Count} : {mapcolor9.Keys.zToStringValues()}");
            //Trace.WriteLine($"mapcolor13 {mapcolor13.Count} : {mapcolor13.Keys.zToStringValues()}");
        }
    }
}
