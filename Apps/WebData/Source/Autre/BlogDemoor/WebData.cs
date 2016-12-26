using pb.Data;
using pb.Data.Xml;
using pb.IO;
using System.Xml.Linq;

namespace WebData.BlogDemoor
{
    public static partial class WebData
    {
        public static BlogDemoor_v4 CreateDataManager_v4(string parameters = null)
        {
            NamedValues<ZValue> parameters2 = ParseParameters(parameters);
            bool test = WebData.GetTestValue(parameters2);
            return BlogDemoor_v4.Create(test);
        }

        public static void Backup(string parameters = null)
        {
            Backup backup = new Backup();

            //BlogDemoor_v3 blogDemoor = CreateDataManager_v3(parameters);
            BlogDemoor_v4 blogDemoor = CreateDataManager_v4(parameters);

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
