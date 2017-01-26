using pb.Data.Mongo;
using pb.Data.OpenXml;
using pb.IO;
using System.Collections.Generic;

namespace WebData.BlogDemoor.Test
{
    public static class Test_HtmlToOXmlDoc_v2
    {
        public static void CreateDocx(string file, string footerText, int limit = 0, int skip = 0, OXmlDocOptions options = OXmlDocOptions.None, string parameters = null, string patchesFile = null)
        {
            //BlogDemoorOXmlDoc.CreateDocx(WebData.CreateDataManager_v4(parameters), file, footerText, limit, skip, options);
            CreateBlogDemoorOXmlDoc(parameters, patchesFile).CreateDocx(file, footerText, limit, skip, options);
        }

        public static void CreateDocx(string file, string footerText, IEnumerable<int> detailIds, OXmlDocOptions options = OXmlDocOptions.None, string parameters = null, string patchesFile = null)
        {
            //BlogDemoorOXmlDoc.CreateDocx(WebData.CreateDataManager_v4(parameters), file, footerText, detailIds, options);
            CreateBlogDemoorOXmlDoc(parameters, patchesFile).CreateDocx(file, footerText, detailIds, options);
        }

        public static void CreateAllDocx(string directory, string footerText, OXmlDocOptions options = OXmlDocOptions.None, string parameters = null, string patchesFile = null)
        {
            //BlogDemoor_v4 dataManager = WebData.CreateDataManager_v4(parameters);
            BlogDemoorOXmlDoc blogDemoorOXmlDoc = CreateBlogDemoorOXmlDoc(parameters, patchesFile);
            int limit = 40;
            int skip = 0;
            for (int i = 1; i < 5; i++)
            {
                string file = zPath.Combine(directory, $"voyage_{i.ToString().PadLeft(2, '0')}.docx");
                if (i == 4)
                    limit = 0;
                //BlogDemoorOXmlDoc.CreateDocx(dataManager, file, footerText, limit, skip, options);
                blogDemoorOXmlDoc.CreateDocx(file, footerText, limit, skip, options);
                skip += limit;
            }
        }

        public static void OXmlToDocx(string file)
        {
            OXmlDoc.Create(file + ".docx", zMongo.BsonRead<OXmlElement>(file));
        }

        public static void TracePages(int limit = 0, int skip = 0, string parameters = null)
        {
            BlogDemoorOXmlDoc blogDemoorOXmlDoc = CreateBlogDemoorOXmlDoc(parameters);
            blogDemoorOXmlDoc.TracePages(limit, skip);
        }

        public static BlogDemoorOXmlDoc CreateBlogDemoorOXmlDoc(string parameters = null, string patchesFile = null)
        {
            return new BlogDemoorOXmlDoc(WebData.CreateDataManager_v4(parameters), patchesFile);
        }
    }
}
