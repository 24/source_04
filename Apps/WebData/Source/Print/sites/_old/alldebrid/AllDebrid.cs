using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using pb;
using pb.IO;

//namespace Print.download
namespace Download.Print
{
    class AllDebrid
    {
        private const string _urlService = "http://www.alldebrid.com/service.php?pseudo=la_beuze&password=zzzz&view=1&link=";
        private static pb.old.HtmlXmlReader _hxr = pb.old.HtmlXmlReader.CurrentHtmlXmlReader;
        private static ITrace _tr = Trace.CurrentTrace;
        private static string _downloadDir = null;
        private bool _invalidLink = false;

        public static string DownloadDir { get { return _downloadDir; } set { _downloadDir = value; } }
        public bool InvalidLink { get { return _invalidLink; } }

        public bool Donwload(string url)
        {
            _tr.WriteLine("download url \"{0}\" into dir \"{1}\"", url, _downloadDir);
            string downloadUrl = GetDonwloadUrl(url);
            if (downloadUrl == null)
                return false;
            string file = GetFilename(downloadUrl);
            if (file == null || file == "")
            {
                _tr.WriteLine("error unknow filename in url \"{0}\"", downloadUrl);
                return false;
            }
            file = GetDownloadPath(file);
            _hxr.Save(downloadUrl, file);
            return true;
        }

        private string GetDownloadPath(string file)
        {
            if (_downloadDir == null)
                throw new PBException("error undefined download directory");
            //string newDir = zfile.GetNewIndexedDirectory(Path.Combine(_downloadDir, "file"));
            string newDir = zdir.GetNewIndexedDirectory(_downloadDir) + "_file";
            return Path.Combine(newDir, file);
        }

        private string GetDonwloadUrl(string url)
        {
            _hxr.Load(_urlService + url);
            // check _hxr.http.ContentType == text/html
            // Invalid link : "1,;,http://bayfiles.net/file/Rofb/NxsNgh/zzfigzz.pdf : <span style='color:#a00;'>Invalid link</span>,;,0"
            string downloadUrl = _hxr.http.TextResult;
            if (downloadUrl.Contains("Invalid link"))
            {
                _tr.WriteLine("error invalid link");
                _invalidLink = true;
                return null;
            }
            return downloadUrl;
        }

        private string GetFilename(string downloadUrl)
        {
            //http://s10.alldebrid.com/dl/379jqr8eeb/BonneJourneeAvous.17.08.2013.zip
            int i = downloadUrl.LastIndexOf('/');
            if (i == -1)
                return null;
            return downloadUrl.Substring(i + 1);
        }
    }
}
