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
        public static void Test_ReseauGesat_loadHeader_01(int startPage = 1, int maxPage = 1, bool loadImage = false)
        {
            GesatInit();
            ReseauGesat_LoadHeaderFromWebPagesOld1 load = new ReseauGesat_LoadHeaderFromWebPagesOld1(startPage, maxPage, loadImage);
            //_rs.View(load);
            load.zView();
        }

        public static void Test_ReseauGesat_loadCompanyList_01(bool reload = false, int startPage = 1, int maxPage = 1, bool loadImage = false)
        {
            _tr.WriteLine("Test_ReseauGesat_loadCompanyFromWeb_02");
            GesatInit();
            //_rs.View(from header in new ReseauGesat_LoadHeaderFromWebPagesOld1(startPage: startPage, maxPage: maxPage) select Gesat_LoadCompany.LoadCompany(header.url, header, reload, loadImage));
            (from header in new ReseauGesat_LoadHeaderFromWebPagesOld1(startPage: startPage, maxPage: maxPage) select Gesat_LoadCompany.LoadCompany(header.url, header, reload, loadImage)).zView();
        }

        public static void Test_ReseauGesat_loadCompanyListFromWeb_01(int startPage = 1, int maxPage = 1, bool loadImage = false)
        {
            _tr.WriteLine("Test_ReseauGesat_loadCompanyListFromWeb_01");
            GesatInit();
            //_rs.View(from header in new ReseauGesat_LoadHeaderFromWebPagesOld1(startPage: startPage, maxPage: maxPage) select Gesat_LoadCompanyFromWeb.LoadCompany(header.url, header, null, false, loadImage));
            (from header in new ReseauGesat_LoadHeaderFromWebPagesOld1(startPage: startPage, maxPage: maxPage) select Gesat_LoadCompanyFromWeb.LoadCompany(header.url, header, null, false, loadImage)).zView();
        }

        //public static void Test_ReseauGesat_ExportXmlCompanyListFromWeb_01(int startPage = 1, int maxPage = 1, bool loadImage = false)
        //{
        //    _tr.WriteLine("Test_ReseauGesat_ExportXmlCompanyListFromWeb_01");
        //    GesatInit();
        //    Test_ExportXmlGesat_CompanyList_01(from header in new ReseauGesat_LoadHeaderFromWebPagesOld1(startPage: startPage, maxPage: maxPage) select Gesat_LoadCompanyFromWeb.LoadCompany(header.url, header, null, false, loadImage));
        //}

        //public static void Test_ReseauGesat_ExportTextCompanyListFromWeb_01(int startPage = 1, int maxPage = 1, bool loadImage = false)
        //{
        //    _tr.WriteLine("Test_ReseauGesat_ExportTextCompanyListFromWeb_01");
        //    GesatInit();
        //    Test_ExportTextGesat_CompanyList_01(from header in new ReseauGesat_LoadHeaderFromWebPagesOld1(startPage: startPage, maxPage: maxPage) select Gesat_LoadCompanyFromWeb.LoadCompany(header.url, header, null, false, loadImage));
        //}
    }
}
