using System.Drawing;
using MongoDB.Bson.Serialization.Attributes;

namespace pb.Web
{
    //[BsonIgnoreExtraElements]
    public class WebImage
    {
        public WebImage(string url)
        {
            Url = url;
        }

        [BsonId]
        public string Url;
        public string File;
        [BsonIgnore]
        public Image Image;
        public int? Width;
        public int? Heigth;
    }
}
