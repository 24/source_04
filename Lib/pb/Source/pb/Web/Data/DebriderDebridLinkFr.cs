using MongoDB.Bson;
using pb.Data.Mongo;

namespace pb.Web.Data
{
    public class DebriderDebridLinkFr : Debrider
    {
        private DebridLinkFr _debridLinkFr = null;

        public DebridLinkFr DebridLinkFr { get { return _debridLinkFr; } set { _debridLinkFr = value; } }

        public override string DebridLink(string link)
        {
            if (__trace)
                pb.Trace.Write("DebriderDebridLinkFr.DebridLink() : \"{0}\"", link);

            if (_debridLinkFr == null)
                throw new PBException("DebridLinkFr is null");
            BsonDocument doc = _debridLinkFr.DownloaderAdd(link);
            if (doc.zGet("result").zAsString() == "OK")
                return doc.zGet("value.downloadLink").zAsString();
            else
                return null;
        }

        public override int GetLinkRate(string link)
        {
            return DefaultGetLinkRate(link);
        }

        public static int DefaultGetLinkRate(string link)
        {
            //return DownloadFileServerInfo.GetLinkRate(DownloadFileServerInfo.GetServerNameFromLink(link));
            return DebridLinkFr.GetLinkRate(DownloadFileServerInfo.GetServerNameFromLink(link));
        }
    }
}
