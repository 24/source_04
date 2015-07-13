using System;
using System.Drawing;
using System.IO;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using pb.Data;
using pb.Data.Mongo;
using pb.Web.old;

namespace pb.Web
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

        public WebImageMongoCacheManager(string server, string database, string collectionName, string directory)
        {
            _server = server;
            _database = database;
            _collectionName = collectionName;
            _urlCache = new UrlCache(directory);
            _urlCache.UrlFileNameType = UrlFileNameType.Host | UrlFileNameType.Path;
            _urlCache.GetUrlSubDirectoryFunction = httpRequest => zurl.GetDomain(httpRequest.Url);
        }

        public override ImageCache GetImageCache(string url, HttpRequestParameters requestParameters = null)
        {
            return new ImageMongoCache(this, url, requestParameters);
        }

        public override Image LoadImage(string url, HttpRequestParameters requestParameters = null)
        {
            MongoImage mongoImage = LoadMongoImage(url, requestParameters);
            if (mongoImage.Image == null)
                LoadImage(mongoImage);
            return mongoImage.Image;
        }

        public void LoadImage(MongoImage mongoImage)
        {
            mongoImage.Image = zimg.LoadFromFile(Path.Combine(_urlCache.CacheDirectory, mongoImage.File));
        }

        public MongoImage LoadMongoImage(string url, HttpRequestParameters requestParameters = null)
        {
            BsonDocument document = GetCollection().zFindOneById<BsonDocument>(BsonValue.Create(url));
            if (document != null)
                return BsonSerializer.Deserialize<MongoImage>(document);
            else
                return CreateMongoImage(url, requestParameters);
        }

        protected MongoImage CreateMongoImage(string url, HttpRequestParameters requestParameters = null)
        {
            HttpRequest httpRequest = new HttpRequest { Url = url };
            string file = _urlCache.GetUrlSubPath(httpRequest);
            string path = Path.Combine(_urlCache.CacheDirectory, file);
            if (!File.Exists(path))
                HttpManager.CurrentHttpManager.LoadToFile(httpRequest, path, requestParameters);
            Image image = null;
            if (File.Exists(path))
            {
                try
                {
                    image = zimg.LoadFromFile(path);
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
    }

    public class ImageMongoCache : ImageCache
    {
        protected MongoImage _mongoImage = null;

        public ImageMongoCache(WebImageCacheManager imageUrlCacheManager, string url, HttpRequestParameters requestParameters = null)
            : base(imageUrlCacheManager, url, requestParameters)
        {
        }

        public MongoImage MongoImage
        {
            get
            {
                if (_mongoImage == null)
                    _mongoImage = ((WebImageMongoCacheManager)_webImageCacheManager).LoadMongoImage(_url, _requestParameters);
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
