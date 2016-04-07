using pb.Web;

namespace pb.Compiler
{
    public static class RunSourceHtmlRun
    {
        public static void Init()
        {
            HtmlRun.SetResult += RunSource.CurrentRunSource.SetResult;
        }

        public static void End()
        {
            HtmlRun.SetResult -= RunSource.CurrentRunSource.SetResult;
        }
    }
}
