using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using pb;
using pb.IO;
using pb.Web;
using pb.Web.Http;

// *********************************************************** deprecated use LoadWebData or LoadWebDataPages ***********************************************************

namespace pb.old
{
    public abstract class HttpLoad
    {
        protected string _url = null;
        protected object _key = null;
        protected string _fileDirectory = null;
        protected string _urlFile = null;
        protected string _imageFile = null;
        protected bool _documentLoadedFromWeb = false;

        protected bool _documentXml = false;
        protected bool _documentLoadedFromXml = false;
        protected string _xmlFile = null;

        protected bool _documentMongoDb = false;
        protected string _mongoServer = null;
        protected string _mongoDatabase = null;
        protected string _mongoCollectionName = null;
        protected MongoCollection _mongoCollection = null;

        protected bool _useUrlCache = false;
        protected string _cacheDirectory = null;
        protected string _imageCacheDirectory = "image";

        protected HttpLoad(string url)
        {
            _url = url;
        }

        public void Load(bool reload = false, bool loadImage = false)
        {
            if (reload || !DocumentExists())
                DocumentSave(reload, loadImage);
            LoadDocument(reload, loadImage);
        }

        protected virtual bool DocumentExists()
        {
            if (_documentXml)
            {
                return DocumentXmlExists();
            }
            else if (_documentMongoDb)
            {
                return DocumentMongoExists();
            }
            else
                //throw new Exception("unknow document read write");
                return false;
        }

        protected virtual void DocumentSave(bool reload = false, bool saveImage = true)
        {
            if (_documentXml)
            {
                SaveDocumentToXml(reload: reload, saveImage: saveImage);
            }
            else if (_documentMongoDb)
            {
                SaveDocumentToMongo(reload: reload, saveImage: saveImage);
            }
            //else
            //    throw new Exception("unknow document read write");
        }

        protected virtual void LoadDocument(bool reload = false, bool loadImage = false)
        {
            if (_documentXml)
            {
                LoadDocumentFromXml(loadImage);
            }
            else if (_documentMongoDb)
            {
                LoadDocumentFromMongo(loadImage);
            }
            //else
            //    throw new Exception("unknow document read write");
            else
            {
                LoadDocumentFromWeb(reload, loadImage);
            }
        }

        public void LoadDocumentFromWeb(bool reload = false, bool loadImage = false)
        {
            if (!_documentLoadedFromWeb)
            {
                string file = null;
                if (_useUrlCache)
                    file = GetUrlCachePath();
                _LoadDocumentFromWeb(file, reload, loadImage);
                _documentLoadedFromWeb = true;
            }
        }

        public virtual void LoadDocumentFromXml(bool loadImage = false)
        {
            if (!_documentLoadedFromXml)
            {
                string file = GetFileDocumentXml();
                if (!zFile.Exists(file))
                    throw new PBException("error impossible to load xml file does'nt exist \"{0}\"", file);
                _LoadDocumentFromXml(file, loadImage);
                _documentLoadedFromXml = true;
            }
        }

        protected virtual bool DocumentXmlExists()
        {
            return zFile.Exists(GetFileDocumentXml());
        }

        public virtual void SaveDocumentToXml(bool reload = false, bool saveImage = true)
        {
            string file = GetFileDocumentXml();
            if (!reload && zFile.Exists(file))
                return;
            LoadDocumentFromWeb(reload);
            zfile.CreateFileDirectory(file);
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;
            string name = GetName();
            Trace.CurrentTrace.WriteLine("save {0} to \"{1}\"", name, file);
            using (XmlWriter xw = XmlWriter.Create(file, settings))
            {
                xw.WriteStartElement(name);
                _SaveDocumentToXml(xw, saveImage);
                xw.WriteEndElement();
            }
        }

        public virtual string GetFileDocumentXml()
        {
            if (_xmlFile == null)
                _xmlFile = zpath.PathSetExtension(GetUrlCachePath(), ".xml");
            return _xmlFile;
        }

        protected virtual MongoCollection GetCollectionMongoDocument()
        {
            if (_mongoCollection == null)
            {
                if (_mongoServer == null)
                    throw new PBException("error mongo server is'nt defined");
                if (_mongoDatabase == null)
                    throw new PBException("error mongo database is'nt defined");
                if (_mongoCollectionName == null)
                    throw new PBException("error mongo collection is'nt defined");
                _mongoCollection = new MongoClient(_mongoServer).GetServer().GetDatabase(_mongoDatabase).GetCollection(_mongoCollectionName);
            }
            return _mongoCollection;
        }

        protected virtual QueryDocument GetQueryMongoDocument()
        {
            return new QueryDocument { { "_id", GetDocumentBsonKey() } };
        }

        protected virtual bool DocumentMongoExists()
        {
            return GetCollectionMongoDocument().Count(GetQueryMongoDocument()) == 1;
        }

        protected virtual void SaveDocumentToMongo(bool reload = false, bool saveImage = true)
        {
            LoadDocumentFromWeb(reload);
            BsonDocument doc = new BsonDocument();
            doc.Add("_id", GetDocumentBsonKey());
            _SaveDocumentToMongo(doc, saveImage);
            GetCollectionMongoDocument().Save(doc);
        }

        public virtual void LoadDocumentFromMongo(bool loadImage = false)
        {
            BsonDocument doc = (BsonDocument)GetCollectionMongoDocument().FindOneByIdAs(typeof(BsonDocument), GetDocumentBsonKey());
            if (doc == null)
                throw new PBException("error mongo document not found id \"{0}\" collection \"{1}\" database \"{2}\" server \"{3}\"", GetDocumentKey(), _mongoCollectionName, _mongoDatabase, _mongoServer);
            _LoadDocumentFromMongo(doc, loadImage);
        }

        public BsonValue GetDocumentBsonKey()
        {
            return BsonValue.Create(GetDocumentKey());
        }

        public object GetDocumentKey()
        {
            if (_key == null)
                _key = _GetDocumentKey();
            return _key;
        }

        public string GetCacheFileDirectory()
        {
            if (_fileDirectory == null)
            {
                _fileDirectory = _GetCacheFileDirectory();
            }
            return _fileDirectory;
        }

        public virtual string _GetCacheFileDirectory()
        {
            return _cacheDirectory;
        }

        public string GetUrlCachePath()
        {
            if (_urlFile == null)
            {
                _urlFile = zPath.Combine(GetCacheFileDirectory(), GetUrlCacheFilename());
                //if (!zPath.HasExtension(_urlFile))
                //    _urlFile += ".html";
            }
            return _urlFile;
        }

        protected virtual string GetUrlCacheFilename()
        {
            return zurl.UrlToFileName(_url, UrlFileNameType.Path);  // UrlFileNameType.FileName
        }

        public string GetImageFile()
        {
            if (_imageFile == null)
                _imageFile = zPath.Combine(_imageCacheDirectory, zPath.GetFileNameWithoutExtension(GetUrlCachePath()));
            return _imageFile;
        }

        //public virtual string GetUrlNextPage()
        //{
        //    return null;
        //}

        public abstract string GetName();
        public abstract object _GetDocumentKey();
        protected abstract void _LoadDocumentFromWeb(string file = null, bool reload = false, bool loadImage = false);
        protected abstract void _LoadDocumentFromXml(string file, bool loadImage = false);
        protected abstract void _SaveDocumentToXml(XmlWriter xw, bool saveImage = true);
        protected abstract void _LoadDocumentFromMongo(BsonDocument doc, bool loadImage = false);
        protected abstract void _SaveDocumentToMongo(BsonDocument doc, bool saveImage = true);
    }
}
