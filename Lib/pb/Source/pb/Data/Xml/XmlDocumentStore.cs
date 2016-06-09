using System;
using System.Text;
using System.Xml;
using pb.IO;
using pb.Data.old;

namespace pb.Data.Xml
{
    // Error	6	'pb.Data.Xml.XmlDocumentStore<T>' does not implement interface member 'pb.Data.IDocumentStore_v1<T>.DocumentExists(pb.Data.IDocumentRequest_v1<T>)'	C:\pib\dropbox\pbeuz\Dropbox\dev\project\Source\Source_01\Source\PB_Library\pb\Data\Xml\XmlDocumentStore.cs	11	18	Source_01
    public class XmlDocumentStore<T> : IDocumentStore_v1<T>
    {
        private string _nodeName = null;

        public void SetParameters(string nodeName)
        {
            _nodeName = nodeName;
        }

        public bool DocumentExists(IDocumentRequest_v1<T> dataRequest)
        {
            throw new PBException("not implemented");
        }

        //public void LoadDocument(WebDataRequest_v2<T> dataRequest)
        public void LoadDocument(IDocumentRequest_v1<T> dataRequest)
        {
            //if (!dataRequest.DocumentLoadedFromXml)
            if (!dataRequest.DocumentLoaded)
            {
                string file = GetFileDocument();
                if (!zFile.Exists(file))
                    throw new PBException("error impossible to load xml file does'nt exist \"{0}\"", file);
                //////////////////////////////////////////////////////dataRequest.Data = LoadDocument(file, dataRequest.LoadImage);
                //dataRequest.DocumentLoadedFromXml = true;
                dataRequest.DocumentLoaded = true;
            }
        }

        //protected virtual T LoadDocument(string file, bool loadImage = false)
        //{
        //    throw new NotImplementedException();
        //}

        public bool DocumentExists()
        {
            return zFile.Exists(GetFileDocument());
        }

        //public void SaveDocument(WebDataRequest_v2<T> dataRequest)
        public void SaveDocument(IDocumentRequest_v1<T> dataRequest)
        {
            string file = GetFileDocument();
            //////if (!dataRequest.ReloadFromWeb && File.Exists(file))
            //////    return;
            zfile.CreateFileDirectory(file);
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;
            Trace.CurrentTrace.WriteLine("save {0} to \"{1}\"", _nodeName, file);
            using (XmlWriter xw = XmlWriter.Create(file, settings))
            {
                xw.WriteStartElement(_nodeName);
                //////////////////////////////////////////////////////SaveDocument(xw, dataRequest.LoadImage);
                xw.WriteEndElement();
            }
        }

        //protected virtual void SaveDocument(XmlWriter xw, bool saveImage = true)
        //{
        //    throw new NotImplementedException();
        //}

        private string GetFileDocument()
        {
            //if (_xmlFile == null)
            //    _xmlFile = zpath.PathSetExt(GetUrlCachePath(), ".xml");
            //return _xmlFile;
            throw new NotImplementedException();
        }
    }
}
