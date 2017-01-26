using System;
using System.Collections.Generic;
using System.Linq;

namespace hts.WebData.Unea
{
    public class Unea_Company
    {
        public DateTime? LoadFromWebDate = null;
        //public string urlHeader = null;
        public string UrlDetail1 = null;
        public string UrlDetail2 = null;
        public string HeaderName = null;
        public string Detail1Name = null;
        public string Detail2Name = null;
        public string HeaderLocation = null;
        public string Detail1Location = null;
        public string Detail1Address = null;
        public string Detail2Address = null;
        public string HeaderPhone = null;
        public string Detail1Phone = null;
        public string Detail2Phone = null;
        public string HeaderFax = null;
        public string Detail1Fax = null;
        public string Detail2Fax = null;
        public string HeaderEmail = null;
        public string Detail1Email = null;
        public string Detail2Email = null;
        public string Detail1WebSite = null;
        public string Detail2WebSite = null;
        public string Detail1Presentation = null;
        public string Detail2Presentation = null;
        public string Detail1Clients = null;
        public string Detail2Clients = null;
        public string Detail1Leader = null; // dirigeant
        public string Detail2Leader = null; // dirigeant
        public int? Detail1EmployeNumber = null; // nombre de salarié
        public int? Detail2EmployeNumber = null; // nombre de salarié
        public string Detail1LastYearRevenue = null;  // chiffre d'affaire de l'année écoulée
        public string Detail2LastYearRevenue = null;  // chiffre d'affaire de l'année écoulée
        public string Detail1Certification = null; // certification
        public string Detail2Certification = null; // certification
        public string Detail1Siret = null;
        public string Detail2Siret = null;
        //public SortedDictionary<string, string> HeaderActivities = new SortedDictionary<string, string>();
        //public SortedDictionary<string, string> Detail1Activities = new SortedDictionary<string, string>();
        //public SortedDictionary<string, string> Detail2Activities = new SortedDictionary<string, string>();
        public Unea_Activity[] HeaderActivities = null;
        public Unea_Activity[] Detail1Activities = null;
        public Unea_Activity[] Detail2Activities = null;
        public string[] Activities1 = null;
        public string[] Activities2 = null;
        public string[] Activities3 = null;
        public SortedDictionary<string, string> Detail1Sectors = new SortedDictionary<string, string>();  // Filières Métiers UNEA
        public SortedDictionary<string, string> Detail2Sectors = new SortedDictionary<string, string>();  // Filières Métiers UNEA
        public SortedDictionary<string, Unea_Document> Detail1DownloadDocuments = new SortedDictionary<string, Unea_Document>();
        public SortedDictionary<string, Unea_Document> Detail2DownloadDocuments = new SortedDictionary<string, Unea_Document>();
        public SortedDictionary<string, string> Detail1Photos = new SortedDictionary<string, string>();
        public SortedDictionary<string, string> Detail2Photos = new SortedDictionary<string, string>();
        public List<string> HeaderUnknowInfos = new List<string>();
        public List<string> Detail1UnknowInfos = new List<string>();
        public List<string> Detail2UnknowInfos = new List<string>();
    }

    public class Unea_Company_DuplicateExists
    {
        public bool Detail1Name = false;
        public bool Detail2Name = false;
        public bool Detail1Location = false;
        public bool Detail2Address = false;
        public bool Detail1Phone = false;
        public bool Detail2Phone = false;
        public bool Detail1Fax = false;
        public bool Detail2Fax = false;
        public bool Detail1Email = false;
        public bool Detail2Email = false;
        public bool Detail2WebSite = false;
        public bool Detail2Presentation = false;
        public bool Detail2Clients = false;
        public bool Detail2Leader = false; // falset
        public bool Detail2EmployeNumber = false; // nombre de falseé
        public bool Detail2LastYearRevenue = false;  // chiffre d'affaire de l'année falsee
        public bool Detail2Certification = false; // falsen
        public bool Detail2Siret = false;
        public bool Detail1Activities = false;
        public bool Detail2Activities = false;
        public bool Detail2Sectors = false;
        public bool Detail2DownloadDocuments = false;
        public bool Detail2Photos = false;
        public bool HeaderUnknowInfos = false;
        public bool Detail1UnknowInfos = false;
        public bool Detail2UnknowInfos = false;
    }

    public class Unea_CompanyUniqueValues
    {
        //public SortedDictionary<string, string> Activities = new SortedDictionary<string, string>();
        public SortedDictionary<string, string> Sectors = new SortedDictionary<string, string>();
        public SortedDictionary<string, string> DownloadDocumentsUrl = new SortedDictionary<string, string>();
        public SortedDictionary<string, string> DownloadDocumentsName = new SortedDictionary<string, string>();
        public SortedDictionary<string, string> Photos = new SortedDictionary<string, string>();
        public SortedDictionary<string, string> UnknowInfos = new SortedDictionary<string, string>();
    }

    public static class UneaData
    {
        public static Unea_Company AggregateCompanyData(Unea_Header_v2 header, Unea_Detail_v2 detail)
        {
            Unea_Detail1_v2 detail1 = detail.Detail1;
            Unea_Detail2_v2 detail2 = detail.Detail2;

            Unea_Company company = new Unea_Company();

            //company.urlHeader = header.sourceUrl;
            company.LoadFromWebDate = header.LoadFromWebDate;
            company.UrlDetail1 = header.UrlDetail1;
            company.UrlDetail2 = header.UrlDetail2;
            company.HeaderName = header.Name;
            company.HeaderLocation = header.Location;
            company.HeaderPhone = header.Phone;
            company.HeaderFax = header.Fax;
            company.HeaderEmail = header.Email;
            company.HeaderActivities = header.Activities;
            company.HeaderUnknowInfos = header.UnknowInfos;

            if (detail1 != null)
            {
                company.Detail1Name = detail1.Name;
                company.Detail1Location = detail1.Location;
                company.Detail1Activities = detail1.Activities;
                company.Detail1Sectors = detail1.Sectors;
                company.Detail1Presentation = detail1.Presentation;
                company.Detail1Clients = detail1.Clients;
                company.Detail1Leader = detail1.Leader;
                company.Detail1EmployeNumber = detail1.EmployeNumber;
                company.Detail1LastYearRevenue = detail1.LastYearRevenue;
                company.Detail1Certification = detail1.Certification;
                company.Detail1Siret = detail1.Siret;
                company.Detail1Photos = detail1.Photos;
                company.Detail1DownloadDocuments = detail1.DownloadDocuments;
                company.Detail1Address = detail1.Address;
                company.Detail1Phone = detail1.Phone;
                company.Detail1Fax = detail1.Fax;
                company.Detail1Email = detail1.Email;
                company.Detail1WebSite = detail1.WebSite;
                company.Detail1UnknowInfos = detail1.UnknowInfos;
            }

            if (detail2 != null)
            {
                company.Detail2Name = detail2.Name;
                company.Detail2Presentation = detail2.Presentation;
                company.Detail2Activities = detail2.Activities;
                company.Detail2Sectors = detail2.Sectors;
                company.Detail2DownloadDocuments = detail2.DownloadDocuments;
                company.Detail2Address = detail2.Address;
                company.Detail2Phone = detail2.Phone;
                company.Detail2Fax = detail2.Fax;
                company.Detail2Email = detail2.Email;
                company.Detail2WebSite = detail2.WebSite;
                company.Detail2Leader = detail2.Leader;
                company.Detail2EmployeNumber = detail2.EmployeNumber;
                company.Detail2LastYearRevenue = detail2.LastYearRevenue;
                company.Detail2Siret = detail2.Siret;
                company.Detail2Certification = detail2.Certification;
                company.Detail2Clients = detail2.Clients;
                company.Detail2UnknowInfos = detail2.UnknowInfos;
            }

            return company;
        }

        public static void AggregateDuplicateData(Unea_Company company)
        {
            AggregateDuplicateData(ref company.HeaderName, ref company.Detail1Name, ref company.Detail2Name);
            AggregateDuplicateData(ref company.HeaderLocation, ref company.Detail1Location);
            AggregateDuplicateData(ref company.Detail1Address, ref company.Detail2Address);
            AggregateDuplicateData(ref company.HeaderPhone, ref company.Detail1Phone, ref company.Detail2Phone);
            AggregateDuplicateData(ref company.HeaderFax, ref company.Detail1Fax, ref company.Detail2Fax);
            AggregateDuplicateData(ref company.HeaderEmail, ref company.Detail1Email, ref company.Detail2Email);
            AggregateDuplicateData(ref company.Detail1WebSite, ref company.Detail2WebSite);
            AggregateDuplicateData(ref company.Detail1Presentation, ref company.Detail2Presentation);
            AggregateDuplicateData(ref company.Detail1Clients, ref company.Detail2Clients);
            AggregateDuplicateData(ref company.Detail1Leader, ref company.Detail2Leader);
            AggregateDuplicateData(ref company.Detail1EmployeNumber, ref company.Detail2EmployeNumber);
            AggregateDuplicateData(ref company.Detail1LastYearRevenue, ref company.Detail2LastYearRevenue);
            AggregateDuplicateData(ref company.Detail1Certification, ref company.Detail2Certification);
            AggregateDuplicateData(ref company.Detail1Siret, ref company.Detail2Siret);
            AggregateActivities(company);
            //AggregateDuplicateData(company.Detail1Activities, company.HeaderActivities);
            //AggregateDuplicateData(company.Detail2Activities, company.HeaderActivities, company.Detail1Activities);
            AggregateDuplicateData(company.Detail2Sectors, company.Detail1Sectors);
            AggregateDuplicateData(company.Detail2DownloadDocuments, company.Detail1DownloadDocuments);
            AggregateDuplicateData(company.Detail2Photos, company.Detail1Photos);
        }

        private static void AggregateActivities(Unea_Company company)
        {
            // aggregate HeaderActivities, Detail1Activities, Detail2Activities in Activities1, Activities2, Activities2
            // header contain Level1
            // detail1 contain Level1, Activity
            // detail2 contain Level1 (uppercase), Level2, Activity

            if (company.Detail2Activities == null)
            {
                company.Activities1 = new string[0];
                company.Activities2 = new string[0];
                company.Activities3 = new string[0];
                return;
            }

            SortedDictionary<string, SortedDictionary<string, Unea_Activity>> dictionary = ActivitiesToDictionary(company.Detail2Activities);

            List<string> activities2 = new List<string>();
            if (company.Detail1Activities != null)
            {
                foreach (Unea_Activity activity in company.Detail1Activities)
                {
                    bool found = false;
                    string key1 = activity.Level1.ToLower();
                    if (dictionary.ContainsKey(key1))
                    {
                        SortedDictionary<string, Unea_Activity> dictionary2 = dictionary[key1];
                        string key2 = activity.Activity.ToLower();
                        if (dictionary2.ContainsKey(key2))
                        {
                            found = true;
                            dictionary2[key2].Level1 = activity.Level1;
                        }
                    }
                    if (!found)
                        activities2.Add(activity.Level1 + " : " + activity.Activity);
                }
            }
            company.Activities2 = activities2.ToArray();

            List<string> activities3 = new List<string>();
            if (company.HeaderActivities != null)
            {
                foreach (Unea_Activity activity in company.HeaderActivities)
                {
                    string key1 = activity.Level1.ToLower();
                    if (!dictionary.ContainsKey(key1))
                        activities3.Add(activity.Level1);
                }
            }
            company.Activities3 = activities3.ToArray();

            company.Activities1 = dictionary.Values.SelectMany(dictionary2 => dictionary2.Values).Select(activity => activity.Level1 + " - " + activity.Level2 + " : " + activity.Activity).ToArray();
        }

        private static SortedDictionary<string, SortedDictionary<string, Unea_Activity>> ActivitiesToDictionary(Unea_Activity[] activities)
        {
            SortedDictionary<string, SortedDictionary<string, Unea_Activity>> dictionary = new SortedDictionary<string, SortedDictionary<string, Unea_Activity>>();
            foreach (Unea_Activity activity in activities)
            {
                string key1 = activity.Level1.ToLower();
                if (!dictionary.ContainsKey(key1))
                    dictionary.Add(key1, new SortedDictionary<string, Unea_Activity>());
                SortedDictionary<string, Unea_Activity> dictionary2 = dictionary[key1];
                string key2 = activity.Activity.ToLower();
                if (!dictionary2.ContainsKey(key2))
                    dictionary2.Add(key2, activity);
            }
            return dictionary;
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

        private static void AggregateDuplicateData(ref string text1, ref string text2, ref string text3)
        {
            AggregateDuplicateData(ref text1, ref text2);
            AggregateDuplicateData(ref text1, ref text3);
            AggregateDuplicateData(ref text2, ref text3);
        }

        private static void AggregateDuplicateData(ref int? value1, ref int? value2)
        {
            if (value1 == null)
            {
                if (value2 != null)
                {
                    value1 = value2;
                    value2 = null;
                }
            }
            else if (value2 != null && value1 == value2)
                value2 = null;
        }

        private static void AggregateDuplicateData(SortedDictionary<string, string> activities1, SortedDictionary<string, string> activities2, SortedDictionary<string, string> activities3 = null)
        {
            foreach (string key in activities1.Keys.ToArray())
            {
                if (activities2.ContainsKey(key) || (activities3 != null && activities3.ContainsKey(key)))
                    activities1.Remove(key);
            }
        }

        private static void AggregateDuplicateData(SortedDictionary<string, Unea_Document> downloadDocuments1, SortedDictionary<string, Unea_Document> downloadDocuments2)
        {
            foreach (string key in downloadDocuments1.Keys.ToArray())
            {
                if (downloadDocuments2.ContainsKey(key) && downloadDocuments1[key].Name == downloadDocuments2[key].Name)
                    downloadDocuments1.Remove(key);
            }
        }

        public static void GetCompany_Duplicate(Unea_Company company, Unea_Company_DuplicateExists duplicate)
        {
            if (company.Detail1Name != null)
                duplicate.Detail1Name = true;
            if (company.Detail2Name != null)
                duplicate.Detail2Name = true;
            if (company.Detail1Location != null)
                duplicate.Detail1Location = true;
            if (company.Detail2Address != null)
                duplicate.Detail2Address = true;
            if (company.Detail1Phone != null)
                duplicate.Detail1Phone = true;
            if (company.Detail2Phone != null)
                duplicate.Detail2Phone = true;
            if (company.Detail1Fax != null)
                duplicate.Detail1Fax = true;
            if (company.Detail2Fax != null)
                duplicate.Detail2Fax = true;
            if (company.Detail1Email != null)
                duplicate.Detail1Email = true;
            if (company.Detail2Email != null)
                duplicate.Detail2Email = true;
            if (company.Detail2WebSite != null)
                duplicate.Detail2WebSite = true;
            if (company.Detail2Presentation != null)
                duplicate.Detail2Presentation = true;
            if (company.Detail2Clients != null)
                duplicate.Detail2Clients = true;
            if (company.Detail2Leader != null)
                duplicate.Detail2Leader = true;
            if (company.Detail2EmployeNumber != null)
                duplicate.Detail2EmployeNumber = true;
            if (company.Detail2LastYearRevenue != null)
                duplicate.Detail2LastYearRevenue = true;
            if (company.Detail2Certification != null)
                duplicate.Detail2Certification = true;
            if (company.Detail2Siret != null)
                duplicate.Detail2Siret = true;

            //if (company.Detail1Activities.Count > 0)
            //    duplicate.Detail1Activities = true;
            //if (company.Detail2Activities.Count > 0)
            //    duplicate.Detail2Activities = true;

            if (company.Detail2Sectors.Count > 0)
                duplicate.Detail2Sectors = true;

            if (company.Detail2DownloadDocuments.Count > 0)
                duplicate.Detail2DownloadDocuments = true;

            if (company.Detail2Photos.Count > 0)
                duplicate.Detail2Photos = true;

            if (company.HeaderUnknowInfos.Count > 0)
                duplicate.HeaderUnknowInfos = true;
            if (company.Detail1UnknowInfos.Count > 0)
                duplicate.Detail1UnknowInfos = true;
            if (company.Detail2UnknowInfos.Count > 0)
                duplicate.Detail2UnknowInfos = true;
        }

        public static void AddUniqueValues(Unea_CompanyUniqueValues uniqueValues, Unea_Company company)
        {
            //AddUniqueValues(uniqueValues.Activities, company.HeaderActivities.Keys);
            //AddUniqueValues(uniqueValues.Activities, company.Detail1Activities.Keys);
            //AddUniqueValues(uniqueValues.Activities, company.Detail2Activities.Keys);
            //SortedDictionary<string, SortedDictionary<string, string>>
            AddUniqueValues(uniqueValues.Sectors, company.Detail1Sectors.Keys);
            AddUniqueValues(uniqueValues.Sectors, company.Detail2Sectors.Keys);
            AddUniqueValues(uniqueValues.DownloadDocumentsName, from doc in company.Detail1DownloadDocuments select doc.Value.Name);
            AddUniqueValues(uniqueValues.DownloadDocumentsUrl, from doc in company.Detail1DownloadDocuments select doc.Value.Url);
            AddUniqueValues(uniqueValues.DownloadDocumentsName, from doc in company.Detail2DownloadDocuments select doc.Value.Name);
            AddUniqueValues(uniqueValues.DownloadDocumentsUrl, from doc in company.Detail2DownloadDocuments select doc.Value.Url);
            AddUniqueValues(uniqueValues.Photos, company.Detail1Photos.Keys);
            AddUniqueValues(uniqueValues.Photos, company.Detail2Photos.Keys);
            AddUniqueValues(uniqueValues.UnknowInfos, company.HeaderUnknowInfos);
            AddUniqueValues(uniqueValues.UnknowInfos, company.Detail1UnknowInfos);
            AddUniqueValues(uniqueValues.UnknowInfos, company.Detail2UnknowInfos);
        }

        private static void AddUniqueValues(SortedDictionary<string, string> uniqueValues, IEnumerable<string> values)
        {
            foreach (string value in values)
            {
                if (value != null && !uniqueValues.ContainsKey(value))
                    uniqueValues.Add(value, null);
            }
        }
    }
}
