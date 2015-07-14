using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Pib
{
    // a supprimer
    class zzzPib_Rapidshare
    {
        private static PB_Library.Rapidshare gRapidshare = null;

        #region InitRapidshare
        public static void InitRapidshare()
        {
            if (gRapidshare == null)
                gRapidshare = GetRapidshare();
        }
        #endregion

        #region GetRapidshare
        public static PB_Library.Rapidshare GetRapidshare()
        {
            PB_Library.Rapidshare rapidshare = new PB_Library.Rapidshare();
            rapidshare.Login = XmlConfig.CurrentConfig.Get("Login");
            rapidshare.Password = XmlConfig.CurrentConfig.Get("Password");
            rapidshare.DownloadDirectory = XmlConfig.CurrentConfig.Get("DownloadDirectory");
            //rapidshare.UnzipDirectory = XmlConfig.CurrentConfig.Get("UnzipDirectory");
            //rapidshare.TaskDirectory = XmlConfig.CurrentConfig.Get("Rapidshare/TaskDirectory");
            //rapidshare.ArchiveTaskDirectory = XmlConfig.CurrentConfig.Get("ArchiveTaskDirectory");
            //rapidshare.TraceDirectory = wr.TraceDir;
            return rapidshare;
        }
        #endregion

        #region RunTaskThread
        public static void RunTaskThread()
        {
            InitRapidshare();
            //gRapidshare.RunTaskThread();
        }
        #endregion

        #region AbortTask
        public static void AbortTask()
        {
            if (gRapidshare != null) gRapidshare.AbortTask();
        }
        #endregion

        #region AddTask_FilesTubeDownload_FromClipboard
        public static void AddTask_FilesTubeDownload_FromClipboard()
        {
            InitRapidshare();
            //gRapidshare.AddTask_FilesTubeDownload_FromClipboard();
        }
        #endregion

        #region AddTask_RapidshareDownload_FromClipboard
        public static void AddTask_RapidshareDownload_FromClipboard()
        {
            InitRapidshare();
            //gRapidshare.AddTask_RapidshareDownload_FromClipboard();
        }
        #endregion

        #region //GetClipboardText
        //public static string[] GetClipboardText()
        //{
        //    if (!Clipboard.ContainsText())
        //    {
        //        cTrace.Trace("Clipboard doesn't contain text");
        //        return null;
        //    }
        //    return cu.Split(Clipboard.GetText(), SplitOption.All);
        //}
        #endregion

        #region //GenerateXml_FilesTubeDownload(string fileXml, int nbDownloadFileToSkip, params string[] urls)
        //public static void GenerateXml_FilesTubeDownload(string fileXml, int nbDownloadFileToSkip, params string[] urls)
        //{
        //    string pathXml;
        //    if (fileXml == null)
        //        fileXml = "Rapidshare_" + GetUrlFilename(urls[0]) + ".xml";
        //    if (!Path.IsPathRooted(fileXml))
        //    {
        //        string generateDirectory = XmlConfig.CurrentConfig.GetExplicit("Rapidshare/GenerateTaskDirectory");
        //        pathXml = Path.Combine(generateDirectory, fileXml);
        //    }
        //    else
        //        pathXml = fileXml;
        //    PB_Library.Rapidshare.GenerateXml_FilesTubeDownload(pathXml, nbDownloadFileToSkip, urls);
        //}
        #endregion

        #region //GenerateXml_RapidshareDownload(string fileXml, int nbDownloadFileToSkip, params string[] urls)
        //public static void GenerateXml_RapidshareDownload(string fileXml, int nbDownloadFileToSkip, params string[] urls)
        //{
        //    string pathXml;
        //    if (fileXml == null)
        //        fileXml = "Rapidshare_" + GetUrlFilename(urls[0]) + ".xml";
        //    if (!Path.IsPathRooted(fileXml))
        //    {
        //        string generateDirectory = XmlConfig.CurrentConfig.GetExplicit("Rapidshare/GenerateTaskDirectory");
        //        pathXml = Path.Combine(generateDirectory, fileXml);
        //    }
        //    else
        //        pathXml = fileXml;
        //    PB_Library.Rapidshare.GenerateXml_RapidshareDownload(pathXml, nbDownloadFileToSkip, urls);
        //}
        #endregion

        #region //GetUrlFilename
        //public static string GetUrlFilename(string url)
        //{
        //    Uri uri = new Uri(url);
        //    return Path.GetFileNameWithoutExtension(uri.Segments[uri.Segments.Length - 1]);
        //}
        #endregion
    }
}
