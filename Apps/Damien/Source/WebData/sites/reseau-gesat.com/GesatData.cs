using pb;
using System;
using System.Collections.Generic;
using System.Linq;

namespace hts.WebData.Gesat
{
    public class Gesat_Company
    {
        public DateTime? LoadFromWebDate = null;
        public string UrlDetail = null;

        public string HeaderName = null;
        public string DetailName = null;
        public string HeaderType = null;
        public string DetailType = null;
        public string HeaderLocation = null;
        public string DetailLocation = null;
        public string HeaderPhone = null;
        public string DetailPhone = null;
        public string[] HeaderInfos = null;
        //public string[] DetailInfos = null;

        public string City = null;
        public string Department = null;
        public string Description = null;
        public string Address = null;
        public string Fax = null;
        public string Email = null;
        public string WebSite = null;
        public string[] Activities = null;
    }

    public static class GesatData
    {
        public static Gesat_Company AggregateCompanyData(Gesat_Header_v2 header, Gesat_Detail_v2 detail)
        {
            Gesat_Company company = new Gesat_Company();

            company.LoadFromWebDate = header.LoadFromWebDate;
            company.UrlDetail = header.UrlDetail;
            company.HeaderName = header.Name;
            company.HeaderType = header.Type;
            company.HeaderLocation = header.Location;
            company.HeaderPhone = header.Phone;
            company.HeaderInfos = header.Infos;

            if (detail != null)
            {
                company.DetailName = detail.Name;
                company.DetailType = detail.Type;
                company.DetailLocation = detail.Location;
                company.DetailPhone = detail.Phone;
                //company.DetailInfos = detail.Infos;

                company.City = detail.City;
                company.Department = detail.Department;
                company.Description = detail.Description;
                company.Address = detail.Address;
                company.Fax = detail.Fax;
                company.Email = detail.Email;
                company.WebSite = detail.WebSite;
                company.Activities = detail.Activities;
            }

            return company;
        }

        public static void AggregateDuplicateData(Gesat_Company company)
        {
            AggregateDuplicateData(ref company.HeaderName, ref company.DetailName);
            AggregateDuplicateData(ref company.HeaderType, ref company.DetailType);
            AggregateDuplicateData(ref company.HeaderLocation, ref company.DetailLocation);
            AggregateDuplicateData(ref company.HeaderPhone, ref company.DetailPhone);
            AggregateInfos(company);
        }

        private static void AggregateInfos(Gesat_Company company)
        {
            Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
            foreach (string info in company.HeaderInfos)
            {
                if (!dictionary1.ContainsKey(info))
                    dictionary1.Add(info, null);
                else
                    Trace.WriteLine($"warning duplicate info \"{info}\"");
            }
            //Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
            //foreach (string info in company.DetailInfos)
            //{
            //    if (!dictionary1.ContainsKey(info))
            //    {
            //        if (!dictionary2.ContainsKey(info))
            //            dictionary2.Add(info, null);
            //        else
            //            Trace.WriteLine($"warning duplicate info \"{info}\"");
            //    }
            //}
            company.HeaderInfos = dictionary1.Keys.ToArray();
            //company.DetailInfos = dictionary2.Keys.ToArray();
        }

        private static void AggregateDuplicateData(ref string text1, ref string text2)
        {
            if (text1 == null)
            {
                if (text2 != null)
                {
                    text1 = text2;
                    text2 = null;
                }
            }
            else if ((text2 != null && string.Equals(text1, text2, StringComparison.InvariantCultureIgnoreCase)))
                text2 = null;
        }
    }
}
