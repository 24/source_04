using System;
using System.Drawing;
using System.Linq;
using Download.Print;
using pb.Web;

namespace Print
{
    public interface IPost : IPostToDownload
    {
        DateTime GetLoadFromWebDate();
        string GetOriginalTitle();
        string GetPostAuthor();
        DateTime? GetPostCreationDate();
        WebImage[] GetImages();
        void SetImages(WebImage[] images);       // used in GoldenDdl_LoadPostDetail.RefreshDocumentsStore()
    }

    public class CompactPost
    {
        public int Id;
        public DateTime LoadFromWebDate;
        public DateTime? CreationDate;
        public PrintType PrintType;
        public string Title;
        public string Url;
        public Image[] Images;
        public string[] DownloadLinks;

        public static CompactPost Create(IPost post)
        {
            return new CompactPost
            {
                Id = post.GetKey(),
                LoadFromWebDate = post.GetLoadFromWebDate(),
                CreationDate = post.GetPostCreationDate(),
                PrintType = post.GetPrintType(),
                Title = post.GetTitle(),
                Url = post.GetDataHttpRequest().Url,
                Images = (from image in post.GetImages() select image.Image).ToArray(),
                DownloadLinks = post.GetDownloadLinks()
            };
        }
    }
}
