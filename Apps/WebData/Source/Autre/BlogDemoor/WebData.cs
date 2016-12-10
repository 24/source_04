using pb.Data;
using pb.Data.Xml;
using pb.IO;
using System.Xml.Linq;

namespace WebData.BlogDemoor
{
    public static class WebData
    {
        public static BlogDemoor CreateDataManager(string parameters = null)
        {
            NamedValues<ZValue> parameters2 = ParseParameters(parameters);
            bool test = WebData.GetTestValue(parameters2);
            //InitLoadImage(test);
            return BlogDemoor.Create(test);
        }

        public static BlogDemoor_v4 CreateDataManager_v4(string parameters = null)
        {
            NamedValues<ZValue> parameters2 = ParseParameters(parameters);
            bool test = WebData.GetTestValue(parameters2);
            return BlogDemoor_v4.Create(test);
        }

        public static void Backup(string parameters = null)
        {
            Backup backup = new Backup();

            BlogDemoor blogDemoor = CreateDataManager(parameters);

            XElement xe = XmlConfig.CurrentConfig.XDocument.Root;
            backup.TempBackupDirectory = xe.zXPathValue("MongoBackupTmpDirectory");  // TempBackupDirectory
            backup.BackupDirectory = xe.zXPathValue("MongoBackupDirectory");         // BackupDirectory
            backup.ZipFilename = xe.zXPathValue("ZipFilename", "BlogDemoor");
            backup.Add(dir => blogDemoor.Backup(dir));
            backup.DoBackup();
        }

        //private static void InitLoadImage(bool test)
        //{
        //    XElement xe;
        //    if (!test)
        //        xe = XmlConfig.CurrentConfig.GetElement("Image");
        //    else
        //        xe = XmlConfig.CurrentConfig.GetElement("Image_Test");
        //    WebImageMongoManager.Create(xe);
        //}

        public static NamedValues<ZValue> ParseParameters(string parameters)
        {
            return ParseNamedValues.ParseValues(parameters, useLowercaseKey: true);
        }

        public static bool GetTestValue(NamedValues<ZValue> parameters)
        {
            if (parameters.ContainsKey("test"))
                return (bool)parameters["test"];
            return false;
        }
    }
}
