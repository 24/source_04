using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Ionic.Zip;
using Ionic.Zlib;
using EnterpriseDT.Net.Ftp;
using pb.Data.Xml;
using pb;
using pb.IO;

namespace Pib
{
    public class Backup : PB_Library.ITask
    {
        private string gTaskName = null;
        private bool gAbortTask = false;
        private bool gSuspendTask = false;
        //private Trace gTaskTrace = null;
        private ITrace gTaskTrace = null;
        private Progress gTaskProgress = null;
        private Progress gTaskProgressDetail = null;
        public event PB_Library.TaskEventHandler TaskEnded = null;

        private bool gbErrorGeneratedByFtpStop = false;
        XmlConfig gConfig = null;
        private long glFtpByteToTransfer;

        public Backup()
        {
            gTaskName = "Backup";
            gTaskTrace = Trace.CurrentTrace;
            gConfig = new XmlConfig();
            //string sPath = gConfig.Get("Backup/Log");
            //if (sPath != null)
            //    gTaskTrace.SetLogFile(sPath, LogOptions.IndexedFile);
            gTaskTrace.SetWriter(gConfig.Get("Backup/Log"), gConfig.Get("Backup/Log/@option").zTextDeserialize(FileOption.None));

            gTaskProgress = new Progress();
            gTaskProgressDetail = new Progress();
        }

        #region TaskName
        public string TaskName
        {
            get { return gTaskName; }
        }
        #endregion

        public ITrace TaskTrace
        {
            get { return gTaskTrace; }
            set { gTaskTrace = value; }
        }

        #region TaskProgress
        public Progress TaskProgress
        {
            get { return gTaskProgress; }
        }
        #endregion

        #region ProgressDetail
        public Progress TaskProgressDetail
        {
            get { return gTaskProgressDetail; }
        }
        #endregion

        #region AbortTask
        public void AbortTask()
        {
            gAbortTask = true;
        }
        #endregion

        #region SuspendTask
        public void SuspendTask()
        {
            gSuspendTask = true;
        }
        #endregion

        #region ResumeTask
        public void ResumeTask()
        {
            gSuspendTask = false;
        }
        #endregion

        #region Execute
        public void ExecuteTask()
        {
            try
            {
                DateTime t1 = DateTime.Now;
                gTaskTrace.WriteLine("Backup start : {0:HH:mm:ss}", DateTime.Now);

                List<string> zipFiles = Zip();
                if (gAbortTask) goto Stop;

                DateTime t2 = DateTime.Now;
                TimeSpan time = t2.Subtract(t1);
                gTaskTrace.WriteLine("Zip          : duration {0:00}:{1:00}:{2:00}", time.Hours, time.Minutes, time.Seconds);

                Ftp(zipFiles);
                if (gAbortTask) goto Stop;

                DateTime t3 = DateTime.Now;
                time = t3.Subtract(t2);
                gTaskTrace.WriteLine("Ftp          : duration {0:00}:{1:00}:{2:00}", time.Hours, time.Minutes, time.Seconds);

                time = t3.Subtract(t1);
                gTaskTrace.WriteLine("Backup       : duration {0:00}:{1:00}:{2:00} - start time {3:HH:mm:ss}", time.Hours, time.Minutes, time.Seconds, t3);
                gTaskTrace.WriteLine("Backup end   : {0:00}:{1:00}:{2:00}", time.Hours, time.Minutes, time.Seconds);
                gTaskTrace.WriteLine();

                return;

            Stop:
                //gTaskTrace.WriteLine("Backup       : process aborted");
                gTaskTrace.WriteLine("Backup       : process aborted");
                gTaskTrace.WriteLine();
            }
            catch (Exception ex)
            {
                //string sError = cError.GetErrorMessage(ex, false, true);
                string sError = Error.GetErrorMessage(ex, false, true);
                //gTaskTrace.WriteLine(sError);
                gTaskTrace.WriteLine(sError);
            }
            finally
            {
                try
                {
                    if (TaskEnded != null) TaskEnded(this);
                }
                catch (Exception ex)
                {
                    string sError = Error.GetErrorMessage(ex, false, true);
                    //gTaskTrace.WriteLine(sError);
                    gTaskTrace.WriteLine(sError);
                }
            }
        }
        #endregion

        #region Zip
        public List<string> Zip()
        {
            int nbZip = 0;
            IEnumerable<XElement> zips = gConfig.GetElements("Backup/Zip");
            foreach (XElement xeZip in zips)
            {
                nbZip += xeZip.zXPathValues("Source/SourceDirectory").Count();
                nbZip += xeZip.zXPathValues("Source/SourceFile").Count();
            }


            List<string> zipFiles = new List<string>();
            int iZip = 0;
            foreach (XElement xeZip in zips)
            {
                if (gAbortTask) break;
                CompressionLevel compressionLevel = GetCompressionLevel(xeZip.zXPathValue("CompressionLevel"));
                string sZipDir = xeZip.zXPathValue("ZipDirectory");
                foreach (string sourceDir in xeZip.zXPathValues("Source/SourceDirectory"))
                {
                    if (gAbortTask) break;
                    gTaskProgress.SetProgressText("Zip " + sourceDir);
                    gTaskProgress.SetProgress(++iZip, nbZip);

                    if (!Directory.Exists(sourceDir))
                    {
                        //cTrace.Trace("Zip          : directory does'nt exist {0}", sourceDir);
                        gTaskTrace.WriteLine("Zip          : directory does'nt exist {0}", sourceDir);
                        continue;
                    }
                    DateTime t3 = DateTime.Now;
                    using (ZipFile zip = new ZipFile())
                    {
                        zip.SaveProgress += new EventHandler<SaveProgressEventArgs>(ZipSaveProgress);
                        //zip.CompressionLevel = CompressionLevel.BestSpeed;
                        zip.CompressionLevel = compressionLevel;
                        zip.AddDirectory(sourceDir, "");
                        string sPath = zpath.PathSetDirectory(Path.GetFileNameWithoutExtension(sourceDir) + ".zip", sZipDir);
                        string sDir = Path.GetDirectoryName(sPath);
                        if (!Directory.Exists(sDir)) Directory.CreateDirectory(sDir);
                        zip.Save(sPath);
                        if (gAbortTask) break;
                        zipFiles.Add(sPath);
                        int nbFiles = zip.zNbFile();
                        //int nbDir = zip.zNbDirectory();
                        long compressedSize = zip.zCompressedSize();
                        long uncompressedSize = zip.zUncompressedSize();
                        double ratio = zip.zCompressionRatio();
                        TimeSpan t4 = DateTime.Now.Subtract(t3);
                        gTaskTrace.WriteLine("Zip          : {0,-40} - ratio {1,4:0.0} - nb files {2,5} - size {3,8} / {4,8} - {5:00}:{6:00}:{7:00}", sourceDir, ratio, nbFiles, GetSizeString(compressedSize), GetSizeString(uncompressedSize), t4.Hours, t4.Minutes, t4.Seconds);
                    }
                }

                foreach (string sourceFile in xeZip.zXPathValues("Source/SourceFile"))
                {
                    if (gAbortTask) break;
                    gTaskProgress.SetProgressText("Zip " + sourceFile);
                    gTaskProgress.SetProgress(++iZip, nbZip);

                    if (!File.Exists(sourceFile))
                    {
                        //cTrace.Trace("Zip          : file does'nt exist {0}", sourceFile);
                        gTaskTrace.WriteLine("Zip          : file does'nt exist {0}", sourceFile);
                        continue;
                    }
                    DateTime t3 = DateTime.Now;
                    using (ZipFile zip = new ZipFile())
                    {
                        zip.SaveProgress += new EventHandler<SaveProgressEventArgs>(ZipSaveProgress);
                        //zip.CompressionLevel = CompressionLevel.BestSpeed;
                        zip.CompressionLevel = compressionLevel;
                        zip.AddFile(sourceFile, "");
                        string sPath = zpath.PathSetDirectory(Path.GetFileNameWithoutExtension(sourceFile) + ".zip", sZipDir);
                        string sDir = Path.GetDirectoryName(sPath);
                        if (!Directory.Exists(sDir)) Directory.CreateDirectory(sDir);
                        zip.Save(sPath);
                        if (gAbortTask) break;
                        zipFiles.Add(sPath);
                        int nbFiles = zip.zNbFile();
                        //int nbDir = zip.zNbDirectory();
                        long compressedSize = zip.zCompressedSize();
                        long uncompressedSize = zip.zUncompressedSize();
                        double ratio = zip.zCompressionRatio();
                        TimeSpan t4 = DateTime.Now.Subtract(t3);
                        gTaskTrace.WriteLine("Zip          : {0,-40} - ratio {1,4:0.0} - nb files {2,5} - size {3,8} / {4,8} - {5:00}:{6:00}:{7:00}", sourceFile, ratio, nbFiles, GetSizeString(compressedSize), GetSizeString(uncompressedSize), t4.Hours, t4.Minutes, t4.Seconds);
                    }
                }
            }
            return zipFiles;
        }
        #endregion

        #region GetCompressionLevel
        public static CompressionLevel GetCompressionLevel(string compressionLevel)
        {
            if (compressionLevel == null) return CompressionLevel.Default;
            switch (compressionLevel.ToLower())
            {
                case "bestcompression": return CompressionLevel.BestCompression;
                case "bestspeed": return CompressionLevel.BestSpeed;
                case "default": return CompressionLevel.Default;
                case "level0": return CompressionLevel.Level0;
                case "level1": return CompressionLevel.Level1;
                case "level2": return CompressionLevel.Level2;
                case "level3": return CompressionLevel.Level3;
                case "level4": return CompressionLevel.LevelL4;
                case "level5": return CompressionLevel.Level5;
                case "level6": return CompressionLevel.Level6;
                case "level7": return CompressionLevel.Level7;
                case "level8": return CompressionLevel.Level8;
                case "level9": return CompressionLevel.Level9;
                case "none": return CompressionLevel.None;
            }
            return CompressionLevel.Default;
        }
        #endregion

        #region Ftp
        public void Ftp(List<string> zipFiles)
        {
            IEnumerable<XElement> ftps = gConfig.GetElements("Backup/Ftp");
            foreach (XElement xeFtp in ftps)
            {
                if (gAbortTask) break;
                string ftpServer = xeFtp.zAttribValue("server");
                if (ftpServer == null)
                {
                    //cTrace.Trace("Ftp          : server is'nt defined");
                    gTaskTrace.WriteLine("Ftp          : server is'nt defined");
                    continue;
                }
                string ftpUser = xeFtp.zAttribValue("user");
                if (ftpUser == null)
                {
                    //cTrace.Trace("Ftp          : user is'nt defined");
                    gTaskTrace.WriteLine("Ftp          : user is'nt defined");
                    continue;
                }
                string ftpPassword = xeFtp.zAttribValue("password");
                string ftpDirectory = xeFtp.zAttribValue("directory");

                string dir = "";
                if (ftpDirectory != null) dir = " directory " + ftpDirectory;
                gTaskTrace.WriteLine("Ftp          : connect to server {0}{1}", ftpServer, dir);

                FTPClient ftp = new FTPClient();
                try
                {
                    ftp.RemoteHost = ftpServer;
                    ftp.Connect();
                    ftp.Login(ftpUser, ftpPassword);
                    if (ftpDirectory != null) ftp.ChDir(ftpDirectory);
                    ftp.TransferType = FTPTransferType.BINARY;
                    ftp.BytesTransferred += new BytesTransferredHandler(FtpBytesTransferred);


                    int iFile = 0;
                    foreach (string zipFile in zipFiles)
                    {
                        if (gAbortTask) break;
                        gTaskProgress.SetProgressText("Ftp copy file " + zipFile);
                        gTaskProgress.SetProgress(++iFile, zipFiles.Count);
                        gTaskTrace.WriteLine("Ftp          : copy file {0}", zipFile);

                        if (!File.Exists(zipFile))
                        {
                            //cTrace.Trace("Ftp          : file does'nt exist {0}", zipFile);
                            gTaskTrace.WriteLine("Ftp          : file does'nt exist {0}", zipFile);
                            continue;
                        }
                        string remoteFile = Path.GetFileName(zipFile);
                        glFtpByteToTransfer = new FileInfo(zipFile).Length;
                        gTaskProgressDetail.SetProgressText("FTP transfer file {0}", remoteFile);

                        try
                        {
                            ftp.Put(zipFile, remoteFile);
                        }
                        catch
                        {
                            if (!gbErrorGeneratedByFtpStop) throw;
                            //string sError = cError.GetErrorMessage(ex, false, true);
                            //gTrace.Trace(sError);
                            break;
                        }

                        gTaskProgressDetail.SetProgress(glFtpByteToTransfer, glFtpByteToTransfer);
                    }

                    ftp.BytesTransferred -= new BytesTransferredHandler(FtpBytesTransferred);
                }
                finally
                {
                    if (!gbErrorGeneratedByFtpStop)
                        ftp.Quit();
                    else
                        gbErrorGeneratedByFtpStop = false;
                }
            }
        }
        #endregion

        #region GetSizeString
        private string GetSizeString(long size)
        {
            double dSize = size;
            if (dSize < 1000) return dSize.ToString() + " o ";
            dSize /= 1024;
            if (dSize < 1000) return dSize.ToString("0.0") + " Ko";
            dSize /= 1024;
            if (dSize < 1000) return dSize.ToString("0.0") + " Mo";
            dSize /= 1024;
            return dSize.ToString("0.0") + " Go";
        }
        #endregion

        #region ZipSaveProgress
        private void ZipSaveProgress(object sender, SaveProgressEventArgs e)
        {
            if (gAbortTask) e.Cancel = true;
            string file = null;
            ZipEntry ze = e.CurrentEntry;
            if (ze != null) file = "Zip " + ze.FileName;
            gTaskProgressDetail.SetProgressText(file);
            gTaskProgressDetail.SetProgress(e.BytesTransferred, e.TotalBytesToTransfer);
        }
        #endregion

        #region FtpBytesTransferred
        private void FtpBytesTransferred(object sender, BytesTransferredEventArgs e)
        {
            if (gAbortTask)
            {
                FTPClient ftp = (FTPClient)sender;
                gbErrorGeneratedByFtpStop = true;
                ftp.QuitImmediately();
            }
            gTaskProgressDetail.SetProgress(e.ByteCount, glFtpByteToTransfer);
        }
        #endregion
    }

    public static class GlobalExtension
    {
        public static int zNbFile(this ZipFile zip)
        {
            return (from f in zip.Entries where !f.IsDirectory select f).Count();
        }

        public static int zNbDirectory(this ZipFile zip)
        {
            return (from f in zip.Entries where f.IsDirectory select f).Count();
        }

        public static long zCompressedSize(this ZipFile zip)
        {
            return (from f in zip.Entries select f.CompressedSize).Sum();
        }

        public static long zUncompressedSize(this ZipFile zip)
        {
            return (from f in zip.Entries select f.UncompressedSize).Sum();
        }

        public static double zCompressionRatio(this ZipFile zip)
        {
            return (double)zip.zUncompressedSize() / (double)zip.zCompressedSize();
        }
    }
}
