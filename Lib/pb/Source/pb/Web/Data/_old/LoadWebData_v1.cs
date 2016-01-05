using System;
using System.Text;
using System.Xml;
using MongoDB.Bson;
using MongoDB.Driver;
using pb.IO;
using pb.Web.old;

namespace pb.Web.old
{
    public abstract class LoadWebData_v1<T>
    {
        private string _url = null;
        private HttpRequestParameters_v1 _requestParameters = null;
        private object _key = null;
        //private string _imageFile = null;
        private bool _documentLoadedFromWeb = false;

        private bool _useXml = false;
        //private string _xmlFile = null;
        private bool _documentLoadedFromXml = false;

        private bool _useMongo = false;
        private string _mongoServer = null;
        private string _mongoDatabase = null;
        private string _mongoCollectionName = null;
        private MongoCollection _mongoCollection = null;
        private bool _documentLoadedFromMongo = false;

        //protected string _imageCacheDirectory = "image";

        private T _data;

        protected LoadWebData_v1(string url, HttpRequestParameters_v1 requestParameters = null)
        {
            _url = url;
            _requestParameters = requestParameters;
        }

        public string Url { get { return _url; } }
        public HttpRequestParameters_v1 RequestParameters { get { return _requestParameters; } }
        public T Data { get { return _data; } }

        protected void SetXmlParameters(bool useXml)
        {
            _useXml = useXml;
        }

        protected void SetMongoParameters(bool useMongo, string mongoServer, string mongoDatabase, string mongoCollectionName)
        {
            _useMongo = useMongo;
            _mongoServer = mongoServer;
            _mongoDatabase = mongoDatabase;
            _mongoCollectionName = mongoCollectionName;
        }

        protected abstract string GetName();

        public void Load(bool reload = false, bool loadImage = false)
        {
            if (reload || !DocumentExists())
                SaveDocument(reload, loadImage);
            LoadDocument(reload, loadImage);
        }

        private bool DocumentExists()
        {
            if (_useXml)
            {
                return DocumentXmlExists();
            }
            else if (_useMongo)
            {
                return DocumentMongoExists();
            }
            else
                //throw new Exception("unknow document read write");
                return false;
        }

        private void LoadDocument(bool reload = false, bool loadImage = false)
        {
            if (_useXml)
            {
                _LoadDocumentFromXml(loadImage);
            }
            else if (_useMongo)
            {
                _LoadDocumentFromMongo(loadImage);
            }
            //else
            //    throw new Exception("unknow document read write");
            else
            {
                _LoadDocumentFromWeb(reload, loadImage);
            }
        }

        private void SaveDocument(bool reload = false, bool saveImage = true)
        {
            if (_useXml)
            {
                _SaveDocumentToXml(reload: reload, saveImage: saveImage);
            }
            else if (_useMongo)
            {
                _SaveDocumentToMongo(reload: reload, saveImage: saveImage);
            }
            //else
            //    throw new Exception("unknow document read write");
        }

        private void _LoadDocumentFromWeb(bool reload = false, bool loadImage = false)
        {
            if (!_documentLoadedFromWeb)
            {
                _data = LoadDocumentFromWeb(reload, loadImage);
                _documentLoadedFromWeb = true;
            }
        }

        protected abstract T LoadDocumentFromWeb(bool reload = false, bool loadImage = false);

        private void _LoadDocumentFromXml(bool loadImage = false)
        {
            if (!_documentLoadedFromXml)
            {
                string file = GetFileDocumentXml();
                if (!zFile.Exists(file))
                    throw new PBException("error impossible to load xml file does'nt exist \"{0}\"", file);
                _data = LoadDocumentFromXml(file, loadImage);
                _documentLoadedFromXml = true;
            }
        }

        //protected abstract T LoadDocumentFromXml(string file, bool loadImage = false);
        protected virtual T LoadDocumentFromXml(string file, bool loadImage = false)
        {
            throw new NotImplementedException();
        }

        private bool DocumentXmlExists()
        {
            return zFile.Exists(GetFileDocumentXml());
        }

        private void _SaveDocumentToXml(bool reload = false, bool saveImage = true)
        {
            string file = GetFileDocumentXml();
            if (!reload && zFile.Exists(file))
                return;
            _LoadDocumentFromWeb(reload);
            zfile.CreateFileDirectory(file);
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;
            string name = GetName();
            Trace.CurrentTrace.WriteLine("save {0} to \"{1}\"", name, file);
            using (XmlWriter xw = XmlWriter.Create(file, settings))
            {
                xw.WriteStartElement(name);
                SaveDocumentToXml(xw, saveImage);
                xw.WriteEndElement();
            }
        }

        //protected abstract void SaveDocumentToXml(XmlWriter xw, bool saveImage = true);
        protected virtual void SaveDocumentToXml(XmlWriter xw, bool saveImage = true)
        {
            throw new NotImplementedException();
        }

        private string GetFileDocumentXml()
        {
            //if (_xmlFile == null)
            //    _xmlFile = zpath.PathSetExt(GetUrlCachePath(), ".xml");
            //return _xmlFile;
            throw new NotImplementedException();
        }

        private MongoCollection GetMongoCollection()
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

        private QueryDocument GetMongoQueryDocument()
        {
            return new QueryDocument { { "_id", GetDocumentBsonKey() } };
        }

        private bool DocumentMongoExists()
        {
            return GetMongoCollection().Count(GetMongoQueryDocument()) == 1;
        }

        private void _LoadDocumentFromMongo(bool loadImage = false)
        {
            if (!_documentLoadedFromMongo)
            {
                BsonDocument doc = (BsonDocument)GetMongoCollection().FindOneByIdAs(typeof(BsonDocument), GetDocumentBsonKey());
                if (doc == null)
                    throw new PBException("error mongo document not found id \"{0}\" collection \"{1}\" database \"{2}\" server \"{3}\"", _GetDocumentKey(), _mongoCollectionName, _mongoDatabase, _mongoServer);
                _data = LoadDocumentFromMongo(doc, loadImage);
                _documentLoadedFromMongo = true;
            }
        }

        //protected abstract T LoadDocumentFromMongo(BsonDocument doc, bool loadImage = false);
        protected virtual T LoadDocumentFromMongo(BsonDocument doc, bool loadImage = false)
        {
            throw new NotImplementedException();
        }

        private void _SaveDocumentToMongo(bool reload = false, bool saveImage = true)
        {
            _LoadDocumentFromWeb(reload);
            BsonDocument doc = new BsonDocument();
            doc.Add("_id", GetDocumentBsonKey());
            SaveDocumentToMongo(doc, saveImage);
            GetMongoCollection().Save(doc);
        }

        //protected abstract void SaveDocumentToMongo(BsonDocument doc, bool saveImage = true);
        protected virtual void SaveDocumentToMongo(BsonDocument doc, bool saveImage = true)
        {
            throw new NotImplementedException();
        }

        private BsonValue GetDocumentBsonKey()
        {
            return BsonValue.Create(_GetDocumentKey());
        }

        private object _GetDocumentKey()
        {
            if (_key == null)
                _key = GetDocumentKey();
            return _key;
        }

        //protected abstract object GetDocumentKey();
        protected virtual object GetDocumentKey()
        {
            throw new NotImplementedException();
        }

        //protected string GetImageFile()
        //{
        //    throw new NotImplementedException();
        //    //if (_imageFile == null)
        //    //    _imageFile = Path.Combine(_imageCacheDirectory, Path.GetFileNameWithoutExtension(GetUrlCachePath()));
        //    //return _imageFile;
        //}
    }
}
