using MongoDB.Bson;
using pb;
using pb.Data;
using pb.Data.Xml;
using pb.Text;
using pb.Web.Data;
using pb.Web.Data.Mongo;
using pb.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace hts.WebData.Gesat
{
    public class Gesat_Header_v2 : IHeaderData
    {
        public string SourceUrl;
        public DateTime? LoadFromWebDate;
        //public string Title;
        public string UrlDetail;

        public string Name = null;
        public string Type = null;
        public string Location = null;
        public string Phone = null;
        public string[] Infos = null;

        public HttpRequest GetHttpRequestDetail()
        {
            return new HttpRequest { Url = UrlDetail };
        }
    }

    public class Gesat_HeaderDataPages : DataPages<Gesat_Header_v2>, IKeyData
    {
        public int Id;

        public BsonValue GetKey()
        {
            return Id;
        }
    }

    public class Gesat_Detail_v2 : IKeyData
    {
        public string Id;
        public string SourceUrl;
        public DateTime LoadFromWebDate;

        public string Name = null;
        public string Type = null;
        public string Location = null;
        public string Phone = null;
        //public string[] Infos = null;

        //public string HeaderName = null;
        //public string HeaderPhone = null;
        public string City = null;
        public string Department = null;
        public string Description = null;
        public string Address = null;
        public string Fax = null;
        public string Email = null;
        public string WebSite = null;
        public string[] Activities = null;

        // IKeyData
        public BsonValue GetKey()
        {
            return Id;
        }
    }

    public class Gesat_v2 : WebHeaderDetailMongoManagerBase_v2<Gesat_Header_v2, Gesat_Detail_v2>
    {
        private static string _configName = "Gesat";
        private static Gesat_v2 _current = null;
        private static string _urlMainPage = "http://www.reseau-gesat.com/Gesat/";
        private static Func<string, string> _trimFunc = text => text?.Trim();
        private static Regex _removeChars = new Regex("[\r\n\t]+", RegexOptions.Compiled);
        private static Func<string, string> _removeCharsFunc = text => text != null ? _removeChars.Replace(text, "") : null;

        public static string ConfigName { get { return _configName; } }
        public static Gesat_v2 Current { get { return _current; } }

        public static Gesat_v2 Create(bool test)
        //public static Gesat_v2 Create(XElement xe)
        {
            if (test)
                Trace.WriteLine("{0} init for test", _configName);
            XElement xe = GetConfigElement(test);

            _current = new Gesat_v2();
            _current.HeaderPageNominalType = typeof(Gesat_HeaderDataPages);
            _current.Create(xe);
            return _current;
        }

        public static XElement GetConfigElement(bool test = false)
        {
            string configName;
            if (!test)
                configName = _configName;
            else
                configName = _configName + "_Test";
            return XmlConfig.CurrentConfig.GetElement(configName);
        }

        // header get data, from WebHeaderDetailMongoManagerBase_v2<THeaderData, TDetailData>
        protected override IEnumDataPages<Gesat_Header_v2> GetHeaderPageData(HttpResult<string> httpResult)
        {
            XXElement xeSource = httpResult.zGetXDocument().zXXElement();
            string url = httpResult.Http.HttpRequest.Url;
            Gesat_HeaderDataPages data = new Gesat_HeaderDataPages();
            data.SourceUrl = url;
            data.LoadFromWebDate = httpResult.Http.RequestTime;
            data.Id = GetPageKey(httpResult.Http.HttpRequest);

            // <div class="PAGENAVIGLIST">
            // <a href="/Gesat/EtablissementList-10-10.html" title="page suivante">&gt;</a>&nbsp;
            data.UrlNextPage = zurl.GetUrl(url, xeSource.XPathValue("//div[@class='PAGENAVIGLIST']//a[@title='page suivante']/@href"));

            // <div class="ETABLISSEMENT STAR-1 ODD"> <div class="ETABLISSEMENT STAR-0 ODD"> <div class="ETABLISSEMENT STAR-1 EVEN">
            IEnumerable<XXElement> xeHeaders = xeSource.XPathElements("//div[starts-with(@class, 'ETABLISSEMENT STAR-')]");

            List<Gesat_Header_v2> headers = new List<Gesat_Header_v2>();
            foreach (XXElement xeHeader in xeHeaders)
            {
                Gesat_Header_v2 header = new Gesat_Header_v2();
                header.SourceUrl = url;
                header.LoadFromWebDate = DateTime.Now;

                //<span class="NOM"><a title="ESAT BETTY LAUNAY-MOULIN VERT" href="/Gesat/Hauts-de-Seine,92/Bois-Colombes,35494/esat-betty-launay-moulin-vert-competences-et-handicap-92,e1837/">ESAT BETTY LAUNAY-MOULIN VERT</a></span>
                XXElement xe = xeHeader.XPathElement(".//span[@class='NOM']//a");
                if (xe != null)
                {
                    header.UrlDetail = zurl.GetUrl(url, xe.ExplicitXPathValue("@href"));
                    header.Name = _trimFunc(xe.ExplicitXPathValue(".//text()"));
                }
                //<span class="VILLE">E.S.A.T.<br />Bois-Colombes (92)</span>
                xe = xeHeader.XPathElement(".//span[@class='VILLE']");
                if (xe != null)
                {
                    IEnumerator<string> texts = xe.DescendantTexts().GetEnumerator();
                    if (texts.MoveNext())
                        header.Type = texts.Current.Trim();
                    else
                        Trace.WriteLine("error companyType not found");
                    if (texts.MoveNext())
                        header.Location = texts.Current.Trim();
                    else
                        Trace.WriteLine("error companyLocation not found");

                }
                // <span class="TELEPHONE">01 47 86 11 48</span>
                header.Phone = _trimFunc(xeHeader.ExplicitXPathValue(".//span[@class='TELEPHONE']//text()"));
                //<img info_bulle="Signataire de la charte Ethique et Valeurs" border="0" alt="/images/bullesGesat/pictoCharte.png" src="/images/bullesGesat/pictoCharte.png" style=" border: 0;" />
                //<img info_bulle="Lauréat des trophées HandiResponsables 2013" border="0" alt="/images/bullesGesat/LAURIERS-OR-2013.png" src="/images/bullesGesat/LAURIERS-OR-2013.png" style=" border: 0;" />
                header.Infos = xeHeader.XPathValues(".//img/@info_bulle").Select(_trimFunc).ToArray();
                //_header.SetInfo(xeHeader.XPathValues(".//img/@info_bulle"));

                headers.Add(header);
            }
            data.Data = headers.ToArray();
            return data;
        }

        // header get key, from WebHeaderDetailMongoManagerBase_v2<THeaderData, TDetailData>
        protected override BsonValue GetHeaderKey(HttpRequest httpRequest)
        {
            return GetPageKey(httpRequest);
        }

        protected static int _headersNumberByPage = 10;
        protected static Regex _pageKeyRegex = new Regex("etablissementlist-([0-9]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static int GetPageKey(HttpRequest httpRequest)
        {
            // page 1 : http://www.reseau-gesat.com/Gesat/
            // page 2 : http://www.reseau-gesat.com/Gesat/EtablissementList-10-10.html
            string url = httpRequest.Url;
            if (url == _urlMainPage)
                return 1;
            Uri uri = new Uri(url);
            string s = uri.Segments[uri.Segments.Length - 1];
            Match match = _pageKeyRegex.Match(s);
            if (!match.Success)
                throw new PBException($"page key not found in url \"{url}\"");
            return int.Parse(match.Groups[1].Value) / _headersNumberByPage + 1;
        }

        // header get url page, from WebHeaderDetailMongoManagerBase_v2<THeaderData, TDetailData>
        protected override HttpRequest GetHttpRequestPage(int page)
        {
            // page 1 : http://www.reseau-gesat.com/Gesat/
            // page 2 : http://www.reseau-gesat.com/Gesat/EtablissementList-10-10.html
            if (page < 1)
                throw new PBException($"wrong page number {page}");
            string url;
            if (page == 1)
                url = _urlMainPage;
            else
                url = _urlMainPage + $"EtablissementList-{(page - 1) * _headersNumberByPage}-10.html";
            return new HttpRequest { Url = url };
        }

        private static Regex _detailCacheSubDirectory = new Regex(",([0-9]+)", RegexOptions.Compiled);
        // detail cache get sub-directory, from WebHeaderDetailMongoManagerBase_v2<THeaderData, TDetailData>
        protected override string GetDetailCacheUrlSubDirectory(HttpRequest httpRequest)
        {
            // http://www.reseau-gesat.com/Gesat/Hauts-de-Seine,92/Bois-Colombes,35494/esat-betty-launay-moulin-vert-competences-et-handicap-92,e1837/
            // http://www.reseau-gesat.com/Gesat/activ-adis,e1239/
            // http://www.reseau-gesat.com/Gesat/esat-l-hospitalite-du-travail-adcp,e1518/
            // http://www.reseau-gesat.com/Gesat/esat-les-pierides-geac62,e1216/
            // Scheme "http" Host "www.reseau-gesat.com" AbsolutePath "/Gesat/Hauts-de-Seine,92/Bois-Colombes,35494/esat-betty-launay-moulin-vert-competences-et-handicap-92,e1837/" Query "" Segments /, Gesat/, Hauts-de-Seine,92/, Bois-Colombes,35494/, esat-betty-launay-moulin-vert-competences-et-handicap-92,e1837/
            // SubDirectory : 92
            Uri uri = new Uri(httpRequest.Url);
            if (uri.Segments.Length >= 3)
            {
                Match match = _detailCacheSubDirectory.Match(uri.Segments[2]);
                if (match.Success)
                    return match.Groups[1].Value;
                else
                    return "other";
            }
            throw new PBException($"wrong detail url \"{httpRequest.Url}\"");
        }

        // detail get data, from WebHeaderDetailMongoManagerBase_v2<THeaderData, TDetailData>
        protected override Gesat_Detail_v2 GetDetailData(HttpResult<string> httpResult)
        {
            XXElement xeSource = httpResult.zGetXDocument().zXXElement();

            Gesat_Detail_v2 data = new Gesat_Detail_v2();
            data.SourceUrl = httpResult.Http.HttpRequest.Url;
            data.LoadFromWebDate = httpResult.Http.RequestTime;
            data.Id = _GetDetailKey(httpResult.Http.HttpRequest);

            _GetDetailData(xeSource, data);

            return data;
        }

        protected void _GetDetailData(XXElement xeSource, Gesat_Detail_v2 data)
        {
            //if (_header != null)
            //{
            //    data.Name = _header.name;
            //    data.Type = _header.type;
            //    data.Location = _header.location;
            //    data.Phone = _header.phone;
            //    data.Infos = _header.infos;
            //}

            // <div class="PAGES" id="content">
            XXElement xe = xeSource.XPathElement(".//div[@id='content']");

            // <h1><span>ESAT BETTY LAUNAY-MOULIN VERT >></span><br />Coordonnées & activités</h1>
            data.Name = _trimFunc(xe.XPathValue(".//h1//text()")?.Trim('>'));
            //if (!s.Equals(data.name, StringComparison.InvariantCultureIgnoreCase))
            //{
            //    data.HeaderName = data.Name;
            //    data.Name = s;
            //}

            // <div class="BLOC B100 ACCROCHE">
            // <div class="CONTENU-BLOC">Cet E.S.A.T. est ouvert depuis 1989 et accueille 55 personnes reconnues travailleurs handicapés.  Il est situé dans la ville de 
            // <a href="/Gesat/Hauts-de-Seine,92/Bois-Colombes,35494/" title="Bois-Colombes // Les ESAT et EA de la ville">Bois-Colombes</a> (
            // <a href="/Gesat/Hauts-de-Seine,92/" title="Hauts-de-Seine // Les ESAT et EA du département">Hauts-de-Seine</a>)
            // </div></div>
            //data.Descryption = xe.XPathConcatText(".//div[@class='BLOC B100 ACCROCHE']//text()", resultFunc: _trimFunc);
            //data.descryption = data.descryption.Replace("\r", "");
            //data.descryption = data.descryption.Replace("\n", "");
            //data.descryption = data.descryption.Replace("\t", "");
            data.Description = _removeCharsFunc(xe.XPathElement(".//div[@class='BLOC B100 ACCROCHE']").DescendantTexts().Select(_trimFunc).zConcatStrings());


            //data.city = xe.XPathValue(".//div[@class='BLOC B100 ACCROCHE']//a[1]//text()", _trimFunc1);
            data.City = _trimFunc(xe.XPathValue(".//div[@class='BLOC B100 ACCROCHE']//a[1]//text()"));
            //data.department = xe.XPathValue(".//div[@class='BLOC B100 ACCROCHE']//a[2]//text()", _trimFunc1);
            data.Department = _trimFunc(xe.XPathValue(".//div[@class='BLOC B100 ACCROCHE']//a[2]//text()"));

            // <div class="ADRESSE">78, RUE RASPAIL<br />92270  Bois-Colombes</div>
            //data.address = xe.XPathConcatText(".//div[@class='ADRESSE']//text()", " ", itemFunc: _trimFunc1);
            //data.address = data.address.Replace("\r", "");
            //data.address = data.address.Replace("\n", "");
            //data.address = data.address.Replace("\t", "");
            data.Address = _removeCharsFunc(xe.XPathElement(".//div[@class='ADRESSE']").DescendantTexts().Select(_trimFunc).zConcatStrings());

            // <div class="TEL">01 47 86 11 48</div>
            //s = xe.XPathValue(".//div[@class='TEL']//text()", _trimFunc1);
            data.Phone = _trimFunc(xe.XPathValue(".//div[@class='TEL']//text()"));
            //if (!s.Equals(data.phone, StringComparison.InvariantCultureIgnoreCase))
            //{
            //    data.headerPhone = data.phone;
            //    data.phone = s;
            //}

            // <div class="FAX">01 47 82 42 64</div>
            data.Fax = _trimFunc(xe.XPathValue(".//div[@class='FAX']//text()"));

            // <div class="EMAIL">production.launay<img border="0" alt="arobase.png" src="/images/bulles/arobase.png" style=" border: 0;" />lemoulinvert.org</div>
            //data.Email = xe.XPathConcatText(".//div[@class='EMAIL']//text()", "@", itemFunc: _trimFunc1);
            data.Email = xe.XPathElement(".//div[@class='EMAIL']").DescendantTexts().Select(_trimFunc).zConcatStrings("@");

            // <div class="WWW"><a href="http://www.esat-b-launay.com" target="_blank">www.esat-b-launay.com</a></div>
            data.WebSite = _trimFunc(xe.XPathValue(".//div[@class='WWW']//a/@href"));

            // <div class="BLOC-FICHE BLOC-ACTIVITES">
            // <dl><dt>Conditionnement, travaux &agrave; fa&ccedil;on</dt></dl>
            // <dl><dt>Assemblage, montage</dt></dl>
            // <dl><dt>Mise sous pli, mailing, routage</dt></dl>
            // <dl><dt>Toutes activit&eacute;s en entreprise </dt></dl>
            // <dl><dt>Num&eacute;risation, saisie informatique</dt></dl>
            // <dl><dt>Remplissage, ensachage, flaconnage</dt></dl>
            // <dl><dt>Etiquetage, codage, badges</dt></dl>
            // <dl><dt>Secr&eacute;tariat, travaux administratifs</dt></dl>
            // <dl><dt>Artisanats divers</dt></dl>
            // </div>
            data.Activities = xe.XPathValues(".//div[@class='BLOC-FICHE BLOC-ACTIVITES']//dl//text()").Select(_trimFunc).ToArray();
        }

        // detail get key, from WebHeaderDetailMongoManagerBase_v2<THeaderData, TDetailData>
        protected override BsonValue GetDetailKey(HttpRequest httpRequest)
        {
            return _GetDetailKey(httpRequest);
        }

        private static string _GetDetailKey(HttpRequest httpRequest)
        {
            // http://www.reseau-gesat.com/Gesat/Hauts-de-Seine,92/Bois-Colombes,35494/esat-betty-launay-moulin-vert-competences-et-handicap-92,e1837/
            // key : Hauts-de-Seine,92/Bois-Colombes,35494/esat-betty-launay-moulin-vert-competences-et-handicap-92,e1837
            // AbsolutePath "/Gesat/Hauts-de-Seine,92/Bois-Colombes,35494/esat-betty-launay-moulin-vert-competences-et-handicap-92,e1837/"
            string absolutePath = new Uri(httpRequest.Url).AbsolutePath;
            if (!absolutePath.StartsWith("/Gesat/"))
                throw new PBException("key not found in url \"{0}\"", httpRequest.Url);
            return absolutePath.Substring(7, absolutePath.Length - 8);
        }
    }
}
