using pb.Data.Xml;
using System.Collections.Generic;
using System.Xml.Linq;

namespace pb.Web.Data.Mongo
{
    //public static class WebImageMongoTools
    //{
    //    private static WebImageMongo __webImageMongo = null;

    //    public static void Init(bool test)
    //    {
    //        XElement xe;
    //        if (!test)
    //            xe = XmlConfig.CurrentConfig.GetElement("Image");
    //        else
    //            xe = XmlConfig.CurrentConfig.GetElement("Image_Test");
    //        __webImageMongo = new WebImageMongo(WebImageMongoCacheManager.Create(xe));
    //    }

    //    public static void LoadImages(IEnumerable<WebImage> images, ImageRequest imageRequest, HttpRequestParameters requestParameters = null)
    //    {
    //        foreach (WebImage image in images)
    //        {
    //            //if (__webImageMongo.LoadImage(image, imageRequest, requestParameters))
    //            //    yield return image;
    //            __webImageMongo.LoadImage(image, imageRequest, requestParameters);
    //        }
    //    }
    //}
}
