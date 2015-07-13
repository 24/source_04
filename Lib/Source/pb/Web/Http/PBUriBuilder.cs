using System;
using System.Collections.Specialized;
using System.Web;

namespace pb.Web
{
    public class PBUriBuilder : UriBuilder
    {
        private NameValueCollection _queryValues = null;

        public PBUriBuilder()
        {
        }

        public PBUriBuilder(string url)
            : base(url)
        {
        }

        public NameValueCollection GetQueryValues()
        {
            if (_queryValues == null)
                _queryValues = HttpUtility.ParseQueryString(Query);
            return _queryValues;
        }

        public void AddQueryValue(string name, string value)
        {
            GetQueryValues().Add(name, value);
            Query = _queryValues.ToString();
        }

        public void RemoveQueryValue(string name)
        {
            GetQueryValues().Remove(name);
            Query = _queryValues.ToString();
        }

        public override string ToString()
        {
            bool port80 = false;
            if (Port == 80)
            {
                Port = -1;
                port80 = true;
            }
            string url = base.ToString();
            if (port80)
                Port = 80;
            return url;
        }
    }
}
