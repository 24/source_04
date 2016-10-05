using System;
using System.Drawing;
using System.Net;
using pb.Data;
using pb.IO;

namespace pb.Web
{
    public partial class HttpManager
    {
        private static HttpManager __currentHttpManager = new HttpManager();
        private bool _traceException = false;
        private int _loadRepeatIfError = 1;
        private int _loadRetryTimeout = 10;                                // timeout in seconds, 0 = no timeout, -1 = endless timeout
        private bool _exportResult = false;
        private string _exportDirectory = null;

        public static HttpManager CurrentHttpManager { get { return __currentHttpManager; } }

        public bool TraceException { get { return _traceException; } set { _traceException = value; } }
        public int LoadRepeatIfError { get { return _loadRepeatIfError; } set { _loadRepeatIfError = value; } }
        public int LoadRetryTimeout { get { return _loadRetryTimeout; } set { _loadRetryTimeout = value; } }
        public bool ExportResult { get { return _exportResult; } set { _exportResult = value; } }
        public string ExportDirectory { get { return _exportDirectory; } set { _exportDirectory = value; } }

        public Http Load(HttpRequest httpRequest, HttpRequestParameters requestParameters = null, string exportFile = null, bool setExportFileExtension = false)
        {
            try
            {
                for (int i = 0; i < _loadRepeatIfError - 1; i++)
                {
                    try
                    {
                        return _Load(httpRequest, requestParameters, exportFile, setExportFileExtension);
                    }
                    catch (Exception ex)
                    {
                        if (ex is WebException)
                        {
                            WebException wex = (WebException)ex;
                            // WebExceptionStatus : ConnectFailure, PipelineFailure, ProtocolError, ReceiveFailure, SendFailure, ServerProtocolViolation, Timeout, UnknownError
                            // $$pb modif le 27/01/2015 WebExceptionStatus.NameResolutionFailure  ex : "The remote name could not be resolved: 'pixhost.me'"
                            if (wex.Status == WebExceptionStatus.ProtocolError || wex.Status == WebExceptionStatus.NameResolutionFailure)
                                throw;
                        }
                        if (ex is ProtocolViolationException)
                            throw;
                        Trace.WriteLine(1, "Error : \"{0}\" ({1})", ex.Message, ex.GetType().ToString());
                    }
                }
                return _Load(httpRequest, requestParameters, exportFile, setExportFileExtension);
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

                if (_traceException)
                    Trace.CurrentTrace.WriteError(ex);
                else
                    throw;
                return null;
            }
        }

        private Http _Load(HttpRequest httpRequest, HttpRequestParameters requestParameters = null, string exportFile = null, bool setExportFileExtension = false)
        {
            Trace.WriteLine(1, "Load \"{0}\" ({1}){2}", httpRequest.Url, httpRequest.Method, exportFile != null ? "(\"" + exportFile + "\")" : null);
            Http http = CreateHttp(httpRequest, requestParameters, exportFile, setExportFileExtension);

            http.LoadAsText();

            return http;
        }

        public bool LoadToFile(HttpRequest httpRequest, string file, bool exportRequest = false, HttpRequestParameters requestParameters = null)
        {
            try
            {
                for (int i = 0; i < _loadRepeatIfError - 1; i++)
                {
                    try
                    {
                        return _LoadToFile(httpRequest, file, exportRequest, requestParameters);
                    }
                    catch (Exception ex)
                    {
                        if (ex is WebException)
                        {
                            WebException wex = (WebException)ex;
                            // WebExceptionStatus : ConnectFailure, PipelineFailure, ProtocolError, ReceiveFailure, SendFailure, ServerProtocolViolation, Timeout, UnknownError
                            // $$pb modif le 27/01/2015 WebExceptionStatus.NameResolutionFailure  ex : "The remote name could not be resolved: 'pixhost.me'"
                            if (wex.Status == WebExceptionStatus.ProtocolError || wex.Status == WebExceptionStatus.NameResolutionFailure)
                                throw;
                        }
                        Trace.WriteLine(1, "Error : \"{0}\" ({1})", ex.Message, ex.GetType().ToString());
                    }
                }
                return _LoadToFile(httpRequest, file, exportRequest, requestParameters);
            }
            catch (Exception ex)
            {
                if (_traceException)
                    Trace.WriteLine("Error : \"{0}\" ({1})", ex.Message, ex.GetType().ToString());
                else
                    throw;
                return false;
            }
        }

        private bool _LoadToFile(HttpRequest httpRequest, string file, bool exportRequest = false, HttpRequestParameters requestParameters = null)
        {
            Trace.WriteLine(1, "LoadToFile(\"{0}\", \"{1}\");", httpRequest.Url, file);
            Http http = CreateHttp(httpRequest, requestParameters);

            return http.LoadToFile(file, exportRequest);
        }

        public Image LoadImage(HttpRequest httpRequest, HttpRequestParameters requestParameters = null)
        {
            try
            {
                for (int i = 0; i < _loadRepeatIfError - 1; i++)
                {
                    try
                    {
                        return _LoadImage(httpRequest, requestParameters);
                    }
                    catch (Exception ex)
                    {
                        if (!ex.GetType().FullName.StartsWith("System.Net."))
                            throw;
                        if (ex is WebException)
                        {
                            WebException wex = (WebException)ex;
                            // WebExceptionStatus : ConnectFailure, PipelineFailure, ProtocolError, ReceiveFailure, SendFailure, ServerProtocolViolation, Timeout, UnknownError
                            // $$pb modif le 27/01/2015 WebExceptionStatus.NameResolutionFailure  ex : "The remote name could not be resolved: 'pixhost.me'"
                            if (wex.Status == WebExceptionStatus.ProtocolError || wex.Status == WebExceptionStatus.NameResolutionFailure)
                                throw;
                        }
                        Trace.WriteLine(1, "Error : \"{0}\" ({1})", ex.Message, ex.GetType().ToString());
                    }
                }
                return _LoadImage(httpRequest, requestParameters);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error : \"{0}\" ({1})", ex.Message, ex.GetType().ToString());
                return null;
            }
        }

        private Image _LoadImage(HttpRequest httpRequest, HttpRequestParameters requestParameters = null)
        {
            Trace.WriteLine(1, "LoadImage(\"{0}\");", httpRequest.Url);
            if (httpRequest.Url.StartsWith("http://"))
            {
                return CreateHttp(httpRequest, requestParameters).LoadImage();
            }
            else
            {
                try
                {
                    return zimg.LoadFromFile(httpRequest.Url);
                }
                catch (Exception ex)
                {
                    pb.Trace.WriteLine("Error : \"{0}\" ({1})", ex.Message, ex.GetType().ToString());
                    return null;
                }
            }
        }

        public Http CreateHttp(HttpRequest httpRequest, HttpRequestParameters requestParameters = null, string exportFile = null, bool setExportFileExtension = false)
        {
            Http http = new Http(httpRequest, requestParameters);
            //http.HttpRetry += new Http.fnHttpRetry(LoadRetryEvent);
            http.LoadRetryTimeout = _loadRetryTimeout;
            if (exportFile != null)
            {
                http.ExportFile = exportFile;
                http.SetExportFileExtension = setExportFileExtension;
            }
            else if (_exportResult && _exportDirectory != null)
            {
                http.ExportFile = GetNewHttpFileName(httpRequest);
                http.SetExportFileExtension = true;
            }
            return http;
        }

        private string GetNewHttpFileName(HttpRequest httpRequest, string ext = null)
        {
            return zfile.GetNewIndexedFileName(_exportDirectory) + "_" + zurl.UrlToFileName(httpRequest.Url, UrlFileNameType.Host | UrlFileNameType.Path | UrlFileNameType.Ext | UrlFileNameType.Query, ext);
        }
    }
}
