using pb;
using pb.Data.Mongo;
using pb.IO;
using System.Linq;

namespace anki.Test
{
    public static class Test_ColumnText
    {
        public static void Test_ColumnText_01(string directory, int limit = 0)
        {
            // c:\pib\drive\google\valentin\UE5\UE5-01-anatomie\data\scan\test\
            int minSpaceBeforeColumn = 4;
            ColumnTextManager columnTextManager = new ColumnTextManager(minSpaceBeforeColumn);
            var files = zDirectory.EnumerateFiles(directory, "*.txt");
            if (limit != 0)
                files = files.Take(limit);
            foreach (string file in files)
            {
                //columnTextManager.Test_GetColumnInfos(zFile.ReadAllLines(file)).zSave(file + ".json");
                //columnTextManager.Test_GetColumnInfos(zFile.ReadAllLines(file)).GroupBy();
                // c# Linq GroupBy with count
                Trace.WriteLine($"file \"{zPath.GetFileName(file)}\"");
                ColumnTextResult result = columnTextManager.FindColumn(zFile.ReadAllLines(file));
                result.zTraceJson();
            }
            //System.Drawing.ImageConverter
        }
    }
}
