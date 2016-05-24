using System;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Web;
using pb.IO;
using pb.Data;

// todo :
//   - test QueueDownloadFile
//   - make QueueDownloadFilessssss

namespace Download.Print
{
    public static partial class WebData
    {
        public static void QueueDownloadFiles(string[] fileLinks, string directory = null, string parameters = null)
        {
            //NamedValues<ZValue> parameters2 = NamedValues.ParseValues(parameters);
            NamedValues<ZValue> parameters2 = ParseParameters(parameters);
            MongoCollectionManager<QueueDownloadFile> mongoQueueDownloadFileManager = DownloadAutomateManagerCreator.CreateMongoQueueDownloadFileManager(GetDownloadAutomateManagerConfig(GetTestValue(parameters2)));
            foreach (string fileLink in fileLinks)
            {
                QueueDownloadFile(mongoQueueDownloadFileManager, new string[] { fileLink }, directory: directory);
            }
        }

        // add an item to mongoQueueDownloadFile
        public static void QueueDownloadFile(string[] filePartLinks, string directory = null, string filename = null, string parameters = null)
        {
            //NamedValues<ZValue> parameters2 = NamedValues.ParseValues(parameters);
            NamedValues<ZValue> parameters2 = ParseParameters(parameters);
            MongoCollectionManager<QueueDownloadFile> mongoQueueDownloadFileManager = DownloadAutomateManagerCreator.CreateMongoQueueDownloadFileManager(GetDownloadAutomateManagerConfig(GetTestValue(parameters2)));
            QueueDownloadFile(mongoQueueDownloadFileManager, filePartLinks, directory, filename);
        }

        //public static void QueueDownloadFile(string[] filePartLinks, string file = null)
        public static void QueueDownloadFile(MongoCollectionManager<QueueDownloadFile> mongoQueueDownloadFileManager, string[] filePartLinks, string directory = null, string filename = null)
        {
            //ServerKey key = null;

            //key = new ServerKey { };

            //Uri uri = new Uri(url);
            //string file = uri.Segments[uri.Segments.Length - 1];
            //IRequestDownloadLinks downloadLinks = null;
            //DownloadItemLink[] downloadItemLinks = DownloadItemLink.CreateDownloadItemLinkArray(downloadLinks);

            //string[] downloadLinks2 = null;
            //PostDownloadLinks postDownloadLinks = PostDownloadLinks.Create(downloadLinks2);
            //DownloadItemLink[] downloadItemLinks2 = DownloadItemLink.CreateDownloadItemLinkArray(postDownloadLinks);

            DownloadItemLink itemLink = DownloadItemLink.CreateDownloadItemLink(filePartLinks);

            // Key (ServerKey) of QueueDownloadFile is null, no save to _mongoDownloadedFileManager in DownloadManager
            QueueDownloadFile downloadFile = new QueueDownloadFile
            {
                DownloadItemLinks = new DownloadItemLink[] { itemLink },
                RequestTime = DateTime.Now,
                Directory = directory,
                Filename = filename
                //Directory = file != null ? zPath.GetDirectoryName(file) : null,
                //Filename = file != null ? zPath.GetFileName(file) : null
            };
            downloadFile.Id = mongoQueueDownloadFileManager.GetNewId();
            mongoQueueDownloadFileManager.Save(downloadFile.Id, downloadFile);
        }

        public static void DownloadFile(string url, string directory = null, bool startNow = false, string parameters = null)
        {
            //NamedValues<ZValue> parameters2 = NamedValues.ParseValues(parameters);
            NamedValues<ZValue> parameters2 = ParseParameters(parameters);
            DownloadManagerClientBase downloadManagerClient = DownloadAutomateManagerCreator.CreateDownloadManagerClient(GetDownloadAutomateManagerConfig(GetTestValue(parameters2)));
            Debrider debrider = DownloadAutomateManagerCreator.CreateDebrider(XmlConfig.CurrentConfig);
            url = debrider.DebridLink(url);
            Uri uri = new Uri(url);
            string file = uri.Segments[uri.Segments.Length - 1];
            if (directory != null)
                file = zPath.Combine(directory, file);
            downloadManagerClient.AddDownload(url, file, startNow: startNow);
        }
    }
}
