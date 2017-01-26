using pb.Compiler;

namespace pb.Web.Html
{
    //public static class RunSourceHtmlRun
    static partial class HtmlRun_v1
    {
        public static void Init()
        {
            //SetResult += RunSource.CurrentRunSource.SetResult;
            SetResult += RunSourceCommand.SetResult;
        }

        public static void End()
        {
            //SetResult -= RunSource.CurrentRunSource.SetResult;
            SetResult -= RunSourceCommand.SetResult;
        }
    }
}
