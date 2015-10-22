using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using PB_Util;
//using PB_Tools;
using Download.Gesat;

namespace Download.Damien
{
    static partial class w
    {
        public static void Test_Gesat_LoadHeaderFromWeb_02()
        {
            //http://www.reseau-gesat.com/Gesat/
            //http://www.reseau-gesat.com/Gesat/EtablissementList-10-10.html
            _tr.WriteLine("Test_Gesat_LoadHeaderFromWeb_02");
            Gesat.Gesat.Init();
            string url = "http://www.reseau-gesat.com/Gesat/";
            bool reload = false;
            bool loadImage = false;
            Gesat_LoadHeaderFromWeb2 load = new Gesat_LoadHeaderFromWeb2(url, reload, loadImage);
            if (!load.Load())
                return;
            //_rs.View(load.Data);
            load.Data.zView();
        }

        public static void Test_Gesat_LoadHeader_02()
        {
            //http://www.reseau-gesat.com/Gesat/
            //http://www.reseau-gesat.com/Gesat/EtablissementList-10-10.html
            _tr.WriteLine("Test_Gesat_LoadHeader_02");
            Gesat.Gesat.Init();
            string url = "http://www.reseau-gesat.com/Gesat/";
            bool reload = false;
            bool loadImage = false;
            Gesat_LoadHeader2 load = new Gesat_LoadHeader2(url);
            load.Load(reload, loadImage);
            //_rs.View(load.Data);
            load.Data.zView();
        }

        public static void Test_Gesat_LoadHeaderPages_02(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false)
        {
            _tr.WriteLine("Test_Gesat_LoadHeader_02");
            Gesat.Gesat.Init();
            Gesat_LoadHeaderPages2 load = new Gesat_LoadHeaderPages2(startPage, maxPage, reload, loadImage);
            //_rs.View(load);
            load.zView();
        }

        public static void Test_Gesat_LoadCompanyFromWeb_02()
        {
            //http://www.reseau-gesat.com/Gesat/Hauts-de-Seine,92/Bois-Colombes,35494/esat-betty-launay-moulin-vert-competences-et-handicap-92,e1837/
            _tr.WriteLine("Test_Gesat_LoadCompanyFromWeb_02");
            Gesat.Gesat.Init();
            string url = "http://www.reseau-gesat.com/Gesat/Hauts-de-Seine,92/Bois-Colombes,35494/esat-betty-launay-moulin-vert-competences-et-handicap-92,e1837/";
            //string urlFile = @"c:\pib\dev_data\exe\wrun\damien\cache\Gesat\esat-betty-launay-moulin-vert-competences-et-handicap-92,e1837.html";
            bool reload = false;
            bool loadImage = false;
            Gesat_LoadCompanyFromWeb2 load = new Gesat_LoadCompanyFromWeb2(url, null, reload, loadImage);
            load.Load();
            //_rs.View(load.Data);
            load.Data.zView();
        }

        public static void Test_Gesat_loadCompany_02(string url, bool reload = false, bool loadImage = false)
        {
            //http://www.reseau-gesat.com/Gesat/Hauts-de-Seine,92/Bois-Colombes,35494/esat-betty-launay-moulin-vert-competences-et-handicap-92,e1837/
            _tr.WriteLine("Test_Gesat_loadCompany_02");
            Gesat.Gesat.Init();
            Gesat_Company data = Gesat_LoadCompany2.LoadCompany(url, null, reload, loadImage);
            //_rs.View(data);
            data.zView();
        }
    }
}
