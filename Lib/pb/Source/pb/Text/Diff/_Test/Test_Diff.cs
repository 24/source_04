using pb.Data.Mongo;

namespace pb.Text.Diff.Test
{
    public static class Test_Diff
    {
        public static void Test_01(string s1, string s2, bool trimSpace = false, bool ignoreSpace = false, bool ignoreCase = false)
        {
            Trace.WriteLine($"compare \"{s1}\" and \"{s2}\"");
            Diff.Item[] items = Diff.DiffText(s1, s2, trimSpace, ignoreSpace, ignoreCase);
            Trace.WriteLine($"items.Length \"{items.Length}\"");
            items.zTraceJson();
        }
    }
}
