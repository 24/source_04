using System;
using System.Drawing;

namespace pb.Web
{
    public class WebImage
    {
        public WebImage(string url)
        {
            Url = url;
        }

        public string Url;
        public Image Image;
    }
}
