using System;
using System.Collections.Generic;
using System.Drawing;
using pb.Data;
using pb.Web.Http.old;

namespace pb.old
{
    public static class Http_v2
    {
        private static HtmlXmlReader _hxr = null;
        //private static int _loadXmlRetryTimeout = 180;
        private static int _loadRepeatIfError = 5;
        //private static ITrace _tr = Trace.CurrentTrace;

        static Http_v2()
        {
            _hxr = HtmlXmlReader.CurrentHtmlXmlReader;
            //_hxr.LoadXmlRetryTimeout = _loadXmlRetryTimeout;
            _hxr.LoadRepeatIfError = _loadRepeatIfError;
        }

        public static HtmlXmlReader HtmlReader { get { return _hxr; } }
        //public static CookieContainer Cookies { get { return _hxr.Cookies; } }

        public static bool LoadUrl(string url, HttpRequestParameters_v1 requestParameters = null)
        {
            try
            {
                _hxr.Load(url, requestParameters);
                //WriteLine("request headers :");
                //WriteHeaders(_hxr.http.Request.Headers);
                //WriteLine("response headers :");
                //WriteHeaders(_hxr.http.Response.Headers);
                return true;
            }
            catch (Exception ex)
            {

                //Load("http://www.frboard.com/magazines-et-journaux/441873-multi-les-journaux-mardi-13-aout-2013-pdf-lien-direct.html");
                //15/08/2013 12:00:32 Error : A connection attempt failed because the connected party did not properly respond after a period of time, or established connection failed because connected host has failed to respond 5.199.168.178:80 (System.Net.Sockets.SocketException)
                //Unable to connect to the remote server (System.Net.WebException)
                //----------------------
                //   at System.Net.Sockets.Socket.DoConnect(EndPoint endPointSnapshot, SocketAddress socketAddress)
                //   at System.Net.ServicePoint.ConnectSocketInternal(Boolean connectFailure, Socket s4, Socket s6, Socket& socket, IPAddress& address, ConnectSocketState state, IAsyncResult asyncResult, Int32 timeout, Exception& exception)
                //----------------------
                //   at System.Net.HttpWebRequest.GetResponse()
                //   at pb.old.Http.OpenWebRequest() in c:\pib\dropbox\pbeuz\Dropbox\dev\project\Source\Source_01\Source\PB_Tools\\Http_Html.cs:line 911
                //   at pb.old.Http.Open() in c:\pib\dropbox\pbeuz\Dropbox\dev\project\Source\Source_01\Source\PB_Tools\\Http_Html.cs:line 780
                //   at pb.old.Http.Load() in c:\pib\dropbox\pbeuz\Dropbox\dev\project\Source\Source_01\Source\PB_Tools\\Http_Html.cs:line 503
                //   at pb.old.HtmlXmlReader.Load(String sUrl) in c:\pib\dropbox\pbeuz\Dropbox\dev\project\Source\Source_01\Source\PB_Tools\\HtmlXmlReader.cs:line 426
                //   at Print.download.w.Test_frboard_02()
                //   at Print.download.w.Run()

                //Trace.WriteLine("Error : \"{0}\" ({1})", ex.Message, ex.GetType().ToString());
                Trace.CurrentTrace.WriteError(ex);
                return false;
            }
        }

        public static bool LoadToFile(string url, string file, HttpRequestParameters_v1 requestParameters = null)
        {
            try
            {
                _hxr.LoadToFile(url, file, requestParameters);
                return true;
            }
            catch (Exception ex)
            {
                //WriteLine("Error : \"{0}\" ({1})", ex.Message, ex.GetType().ToString());
                Trace.CurrentTrace.WriteError(ex);
                return false;
            }
        }

        //public static void LoadImageFromWeb(ImageHtml image, HttpRequestParameters requestParameters = null)
        //{
        //    if (image.Source != null)
        //        image.Image = LoadImageFromWeb(image.Source, requestParameters);
        //}

        // List<ImageHtml> images
        public static void LoadImageFromWeb(IEnumerable<ImageHtml> images, HttpRequestParameters_v1 requestParameters = null)
        {
            foreach (ImageHtml image in images)
            {
                if (image.Source != null)
                {
                    if (image.Image == null)
                        image.Image = LoadImageFromWeb(image.Source, requestParameters);
                    if (image.Image != null)
                    {
                        image.ImageWidth = image.Image.Width;
                        image.ImageHeight = image.Image.Height;
                    }
                }
            }
        }

        public static Image LoadImageFromWeb(string url, HttpRequestParameters_v1 requestParameters = null)
        {
            try
            {
                Image image = _hxr.LoadImage(url, requestParameters);
                //if (image.Height > 200)
                //    image = image.zResize(height: 200);
                return image;
            }
            catch (Exception ex)
            {
                //WriteLine("Error : \"{0}\" ({1})", ex.Message, ex.GetType().ToString());
                Trace.CurrentTrace.WriteError(ex);
                return null;
            }
        }

        public static Image LoadImageFromFile(string file)
        {
            try
            {
                Image image = zimg.LoadBitmapFromFile(file);
                if (image.Height > 200)
                    image = image.zResize(height: 200);
                return image;
            }
            catch (Exception ex)
            {
                //WriteLine("Error : \"{0}\" ({1})", ex.Message, ex.GetType().ToString());
                Trace.CurrentTrace.WriteError(ex);
                return null;
            }
        }

        //public static void WriteHeaders(WebHeaderCollection headers)
        //{
        //    foreach (string key in headers.AllKeys)
        //        WriteLine("  {0} = \"{1}\"", key, headers[key]);
        //}

        //public static void WriteLine(string msg, params object[] prm)
        //{
        //    _tr.WriteLine(msg, prm);
        //}
    }
}
