using System;
using System.Drawing;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using pb.Data;
using pb.Data.Mongo;
using pb.IO;
using System.Xml.Linq;
using pb.Data.Xml;
using pb.Web.Http;

namespace pb.Web.Data
{
    //[BsonIgnoreExtraElements]
    public class MongoImage
    {
        [BsonId]
        public string Url;
        public string File;
        public int Width;
        public int Height;
        public string Type;
        public string Category;
        [BsonIgnore]
        public Image Image;
    }

    public class WebImageMongoCacheManager : WebImageCacheManager
    {
        protected string _server = null;
        protected string _database = null;
        protected string _collectionName = null;
        protected MongoCollection _collection = null;
        protected UrlCache _urlCache;
        //protected bool _exportRequest = false;

        //public WebImageMongoCacheManager(string server, string database, string collectionName, string directory)
        //{
        //    _server = server;
        //    _database = database;
        //    _collectionName = collectionName;
        //    _urlCache = new UrlCache(directory);
        //    _urlCache.UrlFileNameType = UrlFileNameType.Host | UrlFileNameType.Path;
        //    _urlCache.GetUrlSubDirectoryFunction = httpRequest => zurl.GetDomain(httpRequest.Url);
        //}

        public WebImageMongoCacheManager(string server, string database, string collectionName, UrlCache urlCache)
        {
            _server = server;
            _database = database;
            _collectionName = collectionName;
            _urlCache = urlCache;
        }

        public override ImageCache GetImageCache(string url, HttpRequestParameters requestParameters = null, bool refreshImage = false)
        {
            return new ImageMongoCache(this, url, requestParameters);
        }

        public override Image LoadImage(string url, HttpRequestParameters requestParameters = null, bool refreshImage = false)
        {
            MongoImage mongoImage = LoadMongoImage(url, requestParameters, refreshImage);
            if (mongoImage.Image == null)
                LoadImage(mongoImage);
            return mongoImage.Image;
        }

        public void LoadImage(MongoImage mongoImage)
        {
            mongoImage.Image = zimg.LoadBitmapFromFile(zPath.Combine(_urlCache.CacheDirectory, mongoImage.File));
        }

        public MongoImage LoadMongoImage(string url, HttpRequestParameters requestParameters = null, bool refreshImage = false)
        {
            if (!refreshImage)
            {
                BsonDocument document = GetCollection().zFindOneById<BsonDocument>(BsonValue.Create(url));
                if (document != null)
                {
                    //return BsonSerializer.Deserialize<MongoImage>(document);
                    MongoImage mongoImage = BsonSerializer.Deserialize<MongoImage>(document);
                    if (mongoImage.Width != 0 && mongoImage.Height != 0)
                        return mongoImage;
                }
            }
            return CreateMongoImage(url, requestParameters);
        }

        protected MongoImage CreateMongoImage(string url, HttpRequestParameters requestParameters = null)
        {
            HttpRequest httpRequest = new HttpRequest { Url = url };
            string file = _urlCache.GetUrlSubPath(httpRequest);
            string path = zPath.Combine(_urlCache.CacheDirectory, file);
            if (!zFile.Exists(path))
                HttpManager.CurrentHttpManager.LoadToFile(httpRequest, path, _urlCache.SaveRequest, requestParameters);
            Image image = null;
            if (zFile.Exists(path))
            {
                try
                {
                    image = zimg.LoadBitmapFromFile(path);
                }
                catch (Exception exception)
                {
                    Trace.WriteLine("error unable to load image url \"{0}\" to \"{1}\" (WebImageMongoCacheManager.CreateMongoImage())", url, path);
                    Trace.Write("error : ");
                    Trace.WriteLine(exception.Message);
                }
            }
            else
            {
                Trace.WriteLine("error unable to load image url \"{0}\" to \"{1}\" (WebImageMongoCacheManager.CreateMongoImage())", url, path);
            }

            MongoImage mongoImage = new MongoImage();
            mongoImage.Url = url;
            mongoImage.File = file;
            mongoImage.Width = image != null ? image.Width : 0;
            mongoImage.Height = image != null ? image.Height : 0;
            mongoImage.Image = image;

            GetCollection().zUpdate(new QueryDocument { { "_id", BsonValue.Create(url) } }, new UpdateDocument { { "$set", mongoImage.ToBsonDocument() } }, UpdateFlags.Upsert);

            return mongoImage;
        }

        protected MongoCollection GetCollection()
        {
            if (_collection == null)
            {
                if (_server == null)
                    throw new PBException("error mongo server is'nt defined");
                if (_database == null)
                    throw new PBException("error mongo database is'nt defined");
                if (_collectionName == null)
                    throw new PBException("error mongo collection is'nt defined");
                _collection = new MongoClient(_server).GetServer().GetDatabase(_database).GetCollection(_collectionName);
            }
            return _collection;
        }

        public static WebImageMongoCacheManager Create(XElement xe = null)
        {
            //if (xe != null && xe.zXPathValue("UseUrlCache").zTryParseAs(false))
            //    return new WebImageMongoCacheManager(xe.zXPathValue("MongoServer"), xe.zXPathValue("MongoDatabase"), xe.zXPathValue("MongoCollection"), xe.zXPathValue("CacheDirectory"));
            //else
            //    return null;
            UrlCache urlCache = UrlCache.Create(xe);
            if (urlCache != null)
            {
                urlCache.GetUrlSubDirectory = httpRequest => zurl.GetDomain(httpRequest.Url);
                return new WebImageMongoCacheManager(xe.zXPathExplicitValue("MongoServer"), xe.zXPathExplicitValue("MongoDatabase"), xe.zXPathExplicitValue("MongoCollection"), urlCache);
            }
            else
                return null;
        }
    }

    public class ImageMongoCache : ImageCache
    {
        protected MongoImage _mongoImage = null;

        public ImageMongoCache(WebImageCacheManager imageUrlCacheManager, string url, HttpRequestParameters requestParameters = null, bool refreshImage = false)
            : base(imageUrlCacheManager, url, requestParameters, refreshImage)
        {
        }

        public MongoImage MongoImage
        {
            get
            {
                if (_mongoImage == null)
                    _mongoImage = ((WebImageMongoCacheManager)_webImageCacheManager).LoadMongoImage(_url, _requestParameters, _refreshImage);
                return _mongoImage;
            }
        }

        public override Image Image
        {
            get
            {
                if (MongoImage.Image == null)
                    ((WebImageMongoCacheManager)_webImageCacheManager).LoadImage(_mongoImage);
                return MongoImage.Image;
            }
        }
        public override int Width { get { return MongoImage.Width; } }
        public override int Height { get { return MongoImage.Height; } }
    }
}
