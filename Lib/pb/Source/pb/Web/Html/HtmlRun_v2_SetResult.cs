using pb.Compiler;

namespace pb.Web.Html
{
    static partial class HtmlRun
    {
        public static void Init()
        {
            SetResult += RunSourceCommand.SetResult;
        }

        public static void End()
        {
            SetResult -= RunSourceCommand.SetResult;
        }
    }
}
