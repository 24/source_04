using System.Linq;
using Print.old;

namespace Download.Print.old
{
    public static class DownloadPrint_v1
    {
        public static void LoadImages(IPost post)
        {
            post.SetImages(DownloadPrint.LoadImages(post.GetImages()).ToArray());
        }
    }
}
