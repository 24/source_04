using System;
using System.Collections;
using System.IO;
using System.Text;
using pb.IO;

namespace pb.Data.Xml
{
    [Serializable]
    public class XmlParameters_v1
    {

        #region variable
        private string gsPath = null;
        private bool gbParametersLoaded = false;
        private SortedList gslPrm = null;
        public delegate void BeforeSaveEvent();
        public BeforeSaveEvent BeforeSave = null;
        #endregion

        public XmlParameters_v1(string name)
        {
            // file c:\Users\Pierre\AppData\Local\[AssemblyCompany]\[name]\
            // file example : c:\Users\Pierre\AppData\Local\Pierre Beuzart\Download\
            gsPath = GetPath(name);
            gslPrm = new SortedList();
        }

        public string Path
        {
            get { return gsPath; }
        }

        public void Set(object oKey, object oValue)
        {
            Load();
            if (gslPrm.ContainsKey(oKey))
                gslPrm[oKey] = oValue;
            else
                gslPrm.Add(oKey, oValue);
        }

        public void AddToList(object oKey, object oValue)
        {
            Load();
            if (!gslPrm.ContainsKey(oKey))
                gslPrm.Add(oKey, new ArrayList());
            ArrayList list = (ArrayList)gslPrm[oKey];
            list.Add(oValue);
        }

        public void AddToList(object oKey, object[] oValues)
        {
            Load();
            if (!gslPrm.ContainsKey(oKey))
                gslPrm.Add(oKey, new ArrayList());
            ArrayList list = (ArrayList)gslPrm[oKey];

            foreach (object o in oValues)
                list.Add(o);
        }

        public object Get(object oKey)
        {
            Load();
            if (!gslPrm.ContainsKey(oKey)) return null;
            return gslPrm[oKey];
        }

        public class Parameter
        {
            public object oKey;
            public object oValue;
            public object[] oList;
        }

        public void Save()
        {
            //Trace.WriteLine("XmlParameter : save parameters to \"{0}\"", gsPath);
            if (BeforeSave != null)
                BeforeSave();
            Parameter[] prm = SortedListToArray(gslPrm);
            System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(Parameter[]), new Type[] { typeof(object[]) });
            string sDir = zPath.GetDirectoryName(gsPath);
            zDirectory.CreateDirectory(sDir);
            StreamWriter sw = new StreamWriter(gsPath, false, Encoding.Default);
            try
            {
                xs.Serialize(sw, prm);
            }
            finally
            {
                sw.Close();
            }
        }

        public void Load()
        {
            if (gbParametersLoaded) return;

            //Trace.WriteLine("XmlParameter : load parameters from \"{0}\"", gsPath);
            System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(Parameter[]));
            if (!zFile.Exists(gsPath))
            {
                gslPrm = new SortedList();
                return;
            }
            StreamReader sr = new StreamReader(gsPath, Encoding.Default);
            Parameter[] prm;
            try
            {
                prm = (Parameter[])xs.Deserialize(sr);
            }
            finally
            {
                sr.Close();
            }
            gslPrm = ArrayToSortedList(prm);
            gbParametersLoaded = true;
        }

        private static SortedList ArrayToSortedList(Parameter[] prms)
        {
            SortedList sl;

            sl = new SortedList();
            foreach (Parameter prm in prms)
            {
                if (prm.oList != null)
                    sl.Add(prm.oKey, prm.oList);
                else
                    sl.Add(prm.oKey, prm.oValue);
            }
            return sl;
        }

        private static Parameter[] SortedListToArray(SortedList sl)
        {
            object o;
            Parameter[] prm;

            prm = new Parameter[sl.Count];
            for (int i = 0; i < sl.Count; i++)
            {
                prm[i] = new Parameter();
                prm[i].oKey = sl.GetKey(i);
                o = sl.GetByIndex(i);
                //if (o is SortedList)
                //	o = SortedListToArray((SortedList)o);
                if (o is ArrayList)
                    prm[i].oList = ArrayListToArray((ArrayList)o);
                else
                    prm[i].oValue = o;
            }
            return prm;
        }

        private static object[] ArrayListToArray(ArrayList al)
        {
            int i;
            object[] oArray;

            oArray = new object[al.Count];
            i = 0;
            foreach (object o in al)
            {
                oArray[i++] = o;
            }
            return oArray;
        }

        private string GetPath(string sPath)
        {
            string sDir;

            if (!zPath.IsPathRooted(sPath))
            {
                string sPath2 = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                //string company = app.GetApplicationCompany();
                //if (company != null) sPath2 += "\\" + company;
                //string product = app.GetApplicationProduct();
                //if (product != null) sPath2 += "\\" + product;
                string company = zapp.GetEntryAssemblyCompany();
                string product = zapp.GetEntryAssemblyProduct();
                if (company == null || product == null)
                {
                    company = zapp.GetExecutingAssemblyCompany();
                    product = zapp.GetExecutingAssemblyProduct();
                }
                if (company != null) sPath2 += "\\" + company;
                if (product != null) sPath2 += "\\" + product;
                sPath = sPath2 + "\\" + sPath;
            }
            if (!zPath.HasExtension(sPath)) sPath += ".xml";
            sDir = zPath.GetDirectoryName(sPath);
            zDirectory.CreateDirectory(sDir);
            return sPath;
        }
    }
}
