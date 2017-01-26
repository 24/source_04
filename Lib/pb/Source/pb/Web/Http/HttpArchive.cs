using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using MongoDB.Bson;
using pb.Data.Mongo;

namespace pb.Web.Http
{
    /************************************************************************************************************************************************
     * 
     * HAR 1.2 Spec http://www.softwareishard.com/blog/har-12-spec/
     * 
     * {
     *     "log": {
     *         "version" : "1.2",
     *         "creator" : {},
     *         "browser" : {},
     *         "pages": [],
     *         "entries": [],
     *         "comment": ""
     *     }
     * }
     * 
     * "creator": {
     *     "name": "Firebug",
     *     "version": "1.6",
     *     "comment": ""
     * }
     * 
     * "browser": {
     *     "name": "Firefox",
     *     "version": "3.6",
     *     "comment": ""
     * }
     * 
     * "pages": [
     *     {
     *         "startedDateTime": "2009-04-16T12:07:25.123+01:00",
     *         "id": "page_0",
     *         "title": "Test Page",
     *         "pageTimings": {...},
     *         "comment": ""
     *     }
     * ]
     * 
     * "pageTimings": {
     *     "onContentLoad": 1720,
     *     "onLoad": 2500,
     *     "comment": ""
     * }
     * 
     * "entries": [
     *     {
     *         "pageref": "page_0",
     *         "startedDateTime": "2009-04-16T12:07:23.596Z",
     *         "time": 50,
     *         "request": {...},
     *         "response": {...},
     *         "cache": {...},
     *         "timings": {},
     *         "serverIPAddress": "10.0.0.1",
     *         "connection": "52492",
     *         "comment": ""
     *     }
     * ]
     * 
     * "request": {
     *     "method": "GET",
     *     "url": "http://www.example.com/path/?param=value",
     *     "httpVersion": "HTTP/1.1",
     *     "cookies": [],
     *     "headers": [],
     *     "queryString" : [],
     *     "postData" : {},
     *     "headersSize" : 150,
     *     "bodySize" : 0,
     *     "comment" : ""
     * }
     * 
     * "response": {
     *     "status": 200,
     *     "statusText": "OK",
     *     "httpVersion": "HTTP/1.1",
     *     "cookies": [],
     *     "headers": [],
     *     "content": {},
     *     "redirectURL": "",
     *     "headersSize" : 160,
     *     "bodySize" : 850,
     *     "comment" : ""
     *  }
     * 
     * "cookies": [
     *     {
     *         "name": "TestCookie",
     *         "value": "Cookie Value",
     *         "path": "/",
     *         "domain": "www.janodvarko.cz",
     *         "expires": "2009-07-24T19:20:30.123+02:00",
     *         "httpOnly": false,
     *         "secure": false,
     *         "comment": ""
     *     }
     * ]
     * 
     * "headers": [
     *     {
     *         "name": "Accept-Encoding",
     *         "value": "gzip,deflate",
     *         "comment": ""
     *     },
     *     {
     *         "name": "Accept-Language",
     *         "value": "en-us,en;q=0.5",
     *         "comment": ""
     *     }
     * ]
     * 
     * "queryString": [
     *     {
     *         "name": "param1",
     *         "value": "value1",
     *         "comment": ""
     *     },
     *     {
     *         "name": "param1",
     *         "value": "value1",
     *         "comment": ""
     *     }
     * ]
     * 
     * "postData": {
     *     "mimeType": "multipart/form-data",
     *     "params": [],
     *     "text" : "plain posted data",
     *     "comment": ""
     * }
     * 
     * "params": [
     *     {
     *         "name": "paramName",
     *         "value": "paramValue",
     *         "fileName": "example.pdf",
     *         "contentType": "application/pdf",
     *         "comment": ""
     *     }
     * ]
     * 
     * "content": {
     *     "size": 33,
     *     "compression": 0,
     *     "mimeType": "text/html; charset=utf-8",
     *     "text": "\n",
     *     "comment": ""
     * }
     * 
     * "content": {
     *     "size": 33,
     *     "compression": 0,
     *     "mimeType": "text/html; charset=utf-8",
     *     "text": "PGh0bWw+PGhlYWQ+PC9oZWFkPjxib2R5Lz48L2h0bWw+XG4=",
     *     "encoding": "base64",
     *     "comment": ""
     * }
     * 
     * "cache": {
     *     "beforeRequest": {},
     *     "afterRequest": {},
     *     "comment": ""
     * }
     * 
     * beforeRequest or afterRequest
     * "beforeRequest": {
     *     "expires": "2009-04-16T15:50:36",
     *     "lastAccess": "2009-16-02T15:50:34",
     *     "eTag": "",
     *     "hitCount": 0,
     *     "comment": ""
     * }
     * 
     * "timings": {
     *     "blocked": 0,
     *     "dns": -1,
     *     "connect": 15,
     *     "send": 20,
     *     "wait": 38,
     *     "receive": 12,
     *     "ssl": -1,
     *     "comment": ""
     * }
     * 
    ************************************************************************************************************************************************/



    public class HttpArchivePage
    {
        public DateTime startedDateTime;
        public string id;
        public string title;
        public double onContentLoadTime;
        public double onLoadTime;

        public HttpArchivePage(BsonDocument document)
        {
            startedDateTime = DateTime.Parse(document["startedDateTime"].AsString);
            id = document.zGet("id").zAsString();
            title = document.zGet("title").zAsString();
            document = document["pageTimings"].AsBsonDocument;
            onContentLoadTime = document.zGet("onContentLoad").zAsDouble();
            onLoadTime = document.zGet("onLoad").zAsDouble();
        }
    }

    public class HttpArchiveEntry
    {
        //public DateTime startedDateTime;
        public DateTime? startedDateTime;
        public double time;
        public HttpArchiveRequest request;
        public HttpArchiveResponse response;
        public HttpArchiveCache cache;
        public HttpArchiveTimings timings;
        public string connection;
        public string pageref;

        public HttpArchiveEntry(BsonDocument document)
        {
            //startedDateTime = DateTime.Parse(document["startedDateTime"].AsString);
            startedDateTime = document.zGet("startedDateTime").zAsNullableDateTime();
            time = document.zGet("time").zAsDouble();
            request = new HttpArchiveRequest(document["request"].AsBsonDocument);
            response = new HttpArchiveResponse(document["response"].AsBsonDocument);
            cache = new HttpArchiveCache(document["cache"].AsBsonDocument);
            timings = new HttpArchiveTimings(document["timings"].AsBsonDocument);
            connection = document.zGet("connection").zAsString();
            pageref = document.zGet("pageref").zAsString();
        }
    }

    public class HttpArchiveRequest
    {
        public string method;
        public string url;
        public string httpVersion;
        public HttpArchiveHeader[] headers;  // duplicate value like Set-Cookie
        public HttpArchiveCookie[] cookies;
        public HttpArchiveQueryString[] queryString;
        public HttpArchivePostData postData;
        public int headersSize;
        public int bodySize;

        public HttpArchiveRequest(BsonDocument document)
        {
            method = document.zGet("method").zAsString();
            url = document.zGet("url").zAsString();
            httpVersion = document.zGet("httpVersion").zAsString();

            List<HttpArchiveHeader> headerList = new List<HttpArchiveHeader>();
            if (document != null)
            {
                foreach (BsonValue value in document["headers"].AsBsonArray)
                    headerList.Add(new HttpArchiveHeader(value.AsBsonDocument));
            }
            headers = headerList.ToArray();

            List<HttpArchiveCookie> cookieList = new List<HttpArchiveCookie>();
            if (document != null)
            {
                foreach (BsonValue value in document["cookies"].AsBsonArray)
                    cookieList.Add(new HttpArchiveCookie(value.AsBsonDocument));
            }
            cookies = cookieList.ToArray();

            List<HttpArchiveQueryString> queryStringList = new List<HttpArchiveQueryString>();
            if (document != null)
            {
                foreach (BsonValue value in document["queryString"].AsBsonArray)
                    queryStringList.Add(new HttpArchiveQueryString(value.AsBsonDocument));
            }
            queryString = queryStringList.ToArray();

            postData = new HttpArchivePostData(document.zGet("postData").zAsBsonDocument());
            headersSize = document.zGet("headersSize").zAsInt();
            bodySize = document.zGet("bodySize").zAsInt();
        }

        public string GetHttpRequestUrl()
        {
            return url;
        }

        public old.HttpRequestParameters_v1 GetHttpRequestParameters()
        {
            old.HttpRequestParameters_v1 requestParameters = new old.HttpRequestParameters_v1();
            requestParameters.method = Http.GetHttpRequestMethod(method);

            foreach (HttpArchiveHeader header in headers)
            {
                switch(header.name.ToLower())
                {
                    case "accept":
                        requestParameters.accept = header.value;
                        break;
                    case "user-agent":
                        requestParameters.userAgent = header.value;
                        break;
                    case "referer":
                        requestParameters.referer = header.value;
                        break;
                    case "host":
                    case "connection":
                    case "if-modified-since":
                    case "accept-encoding":
                        break;
                    //case "cookie":
                    //    break;
                    //case "host":
                    //case "accept-encoding":
                    //case "accept-language":
                    //case "connection":
                    //case "if-modified-since":
                    //case "cache-control":
                    default:
                        requestParameters.headers.Add(header.name, header.value);
                        break;
                }
            }

            Uri uri = new Uri(url);
            foreach (HttpArchiveCookie cookie in cookies)
            {
                Cookie cookie2 = cookie.GetCookie(uri.DnsSafeHost);
                requestParameters.cookies.Add(cookie2);
            }

            //queryString
            //postData
            return requestParameters;
        }
    }

    public class HttpArchiveResponse
    {
        public int status;
        public string statusText;
        public string httpVersion;
        public HttpArchiveHeader[] headers;  // duplicate value like Set-Cookie
        public HttpArchiveCookie[] cookies;
        public HttpArchivePostContent content;
        public string redirectURL;
        public int headersSize;
        public int bodySize;

        public HttpArchiveResponse(BsonDocument document)
        {
            status = document.zGet("status").zAsInt();
            statusText = document.zGet("statusText").zAsString();
            httpVersion = document.zGet("httpVersion").zAsString();

            List<HttpArchiveHeader> headerList = new List<HttpArchiveHeader>();
            if (document != null)
            {
                foreach (BsonValue value in document["headers"].AsBsonArray)
                    headerList.Add(new HttpArchiveHeader(value.AsBsonDocument));
            }
            headers = headerList.ToArray();

            List<HttpArchiveCookie> cookieList = new List<HttpArchiveCookie>();
            if (document != null)
            {
                foreach (BsonValue value in document["cookies"].AsBsonArray)
                    cookieList.Add(new HttpArchiveCookie(value.AsBsonDocument));
            }
            cookies = cookieList.ToArray();

            content = new HttpArchivePostContent(document.zGet("content").zAsBsonDocument());
            redirectURL = document.zGet("redirectURL").zAsString();
            headersSize = document.zGet("headersSize").zAsInt();
            bodySize = document.zGet("bodySize").zAsInt();
        }
    }

    public class HttpArchiveHeader
    {
        public string name;
        public string value;

        public HttpArchiveHeader(BsonDocument document)
        {
            name = document.zGet("name").zAsString();
            value = document.zGet("value").zAsString();
        }
    }

    public class HttpArchiveQueryString
    {
        public string name;
        public string value;

        public HttpArchiveQueryString(BsonDocument document)
        {
            name = document.zGet("name").zAsString();
            value = document.zGet("value").zAsString();
        }
    }

    public class HttpArchiveCookie
    {
        public string name;
        public string value;
        public string path;
        public string domain;
        public DateTime? expires;
        public bool httpOnly;
        public bool secure;

        public HttpArchiveCookie(BsonDocument document)
        {
            name = document.zGet("name").zAsString();
            value = document.zGet("value").zAsString();
            path = document.zGet("path").zAsString();
            domain = document.zGet("domain").zAsString();
            //expires = document.zGet("expires").zAsString();
            expires = document.zGet("expires").zAsNullableDateTime();
            httpOnly = document.zGet("httpOnly").zAsBoolean();
            secure = document.zGet("secure").zAsBoolean();
        }

        public Cookie GetCookie(string defaultDomain)
        {
            Cookie cookie = new Cookie();
            cookie.Name = name;
            cookie.Value = value;

            if (path != null && path != "")
                cookie.Path = path;
            if (domain != null && domain != "")
                cookie.Domain = domain;
            else
                cookie.Domain = defaultDomain;

            cookie.HttpOnly = httpOnly;
            cookie.Secure = secure;
            return cookie;
        }
    }

    public class HttpArchivePostData
    {
        public string mimeType;
        public HttpArchivePostDataParameter[] parameters;
        public string text;

        public HttpArchivePostData(BsonDocument document)
        {
            mimeType = document.zGet("mimeType").zAsString();
            List<HttpArchivePostDataParameter> parameterList = new List<HttpArchivePostDataParameter>();
            if (document != null)
            {
                foreach (BsonValue value in document["params"].AsBsonArray)
                    parameterList.Add(new HttpArchivePostDataParameter(value.AsBsonDocument));
            }
            parameters = parameterList.ToArray();
            text = document.zGet("text").zAsString();
        }
    }

    public class HttpArchivePostDataParameter
    {
        public string name;
        public string value;
        public string fileName;
        public string contentType;

        public HttpArchivePostDataParameter(BsonDocument document)
        {
            name = document.zGet("name").zAsString();
            value = document.zGet("value").zAsString();
            fileName = document.zGet("fileName").zAsString();
            contentType = document.zGet("contentType").zAsString();
        }
    }

    public class HttpArchivePostContent
    {
        public int size;
        public int compression;
        public string mimeType;
        public string text;

        public HttpArchivePostContent(BsonDocument document)
        {
            size = document.zGet("size").zAsInt();
            compression = document.zGet("compression").zAsInt();
            mimeType = document.zGet("mimeType").zAsString();
            text = document.zGet("text").zAsString();
        }
    }

    public class HttpArchiveCache
    {
        public HttpArchiveCacheValue beforeRequest;
        public HttpArchiveCacheValue afterRequest;

        public HttpArchiveCache(BsonDocument document)
        {
            beforeRequest = new HttpArchiveCacheValue(document.zGet("beforeRequest").zAsBsonDocument());
            afterRequest = new HttpArchiveCacheValue(document.zGet("afterRequest").zAsBsonDocument());
        }
    }

    public class HttpArchiveCacheValue
    {
        public DateTime? expires;
        public DateTime? lastAccess;
        public string eTag;
        public int hitCount;

        public HttpArchiveCacheValue(BsonDocument document)
        {
            expires = document.zGet("expires").zAsNullableDateTime();
            lastAccess = document.zGet("lastAccess").zAsNullableDateTime();
            eTag = document.zGet("eTag").zAsString();
            hitCount = document.zGet("hitCount").zAsInt();
        }
    }

    public class HttpArchiveTimings
    {
        public double blocked;
        public double dns;
        public double connect;
        public double send;
        public double wait;
        public double receive;
        public double ssl;

        public HttpArchiveTimings(BsonDocument document)
        {
            blocked = document.zGet("blocked").zAsDouble();
            dns = document.zGet("dns").zAsDouble();
            connect = document.zGet("connect").zAsDouble();
            send = document.zGet("send").zAsDouble();
            wait = document.zGet("wait").zAsDouble();
            receive = document.zGet("receive").zAsDouble();
            ssl = document.zGet("ssl").zAsDouble();
        }
    }

    public class HttpArchive
    {
        private BsonDocument _document = null;

        public HttpArchive(BsonDocument document)
        {
            _document = document["log"].AsBsonDocument;
        }

        public HttpArchive(string file, Encoding encoding = null)
        {
            BsonDocument document = zMongo.ReadFileAs<BsonDocument>(file, encoding);
            _document = document["log"].AsBsonDocument;
        }

        public void Dispose()
        {
        }

        public string Version { get { return _document.zGet("version").zAsString(); } }
        public string Creator { get { var creator = _document.zGet("creator").zAsBsonDocument(); return creator.zGet("name").zAsString() + " version " + creator.zGet("version").zAsString(); } }
        public string Browser { get { var browser = _document.zGet("browser").zAsBsonDocument(); if (browser != null) return browser.zGet("name").zAsString() + " version " + browser.zGet("version").zAsString(); else return null; } }
        public IEnumerable<HttpArchivePage> Pages
        {
            get
            {
                foreach (BsonValue value in _document["pages"].AsBsonArray)
                {
                    yield return new HttpArchivePage(value.AsBsonDocument);
                }
            }
        }

        public IEnumerable<HttpArchiveEntry> Entries
        {
            get
            {
                foreach (BsonValue value in _document["entries"].AsBsonArray)
                {
                    yield return new HttpArchiveEntry(value.AsBsonDocument);
                }
            }
        }
    }
}
