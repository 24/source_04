using System.Collections.Generic;

namespace tv
{
    public static partial class TVPlayListReader
    {
        // tv fr :
        //   Group1 : "•●★-CANAL PLAY FR & A LA CARTE-★●•" "==========  Chaines   TNT  ==========" "======  Canal +  ======" "======  CINEMA  ======" "======  SPORT  ======" "▃ ▄ ▅ ▆ ▇ █ SFR France █ ▇ ▆ ▅ ▄ ▃"
        //   Group2 : "FR" "box"
        // radio fr : Group1 "****** Radio FR **********" Group2 "FR"
        // film : Group1 "==== Film à la Demande (VOD) ====" "===== Film Francais ========="
        // spectacle : Group1 "===== spectacle ======"
        // docu : Group1 "======= Documentaire ========="
        // serie : Group1 "===== Serie en Francais ========"
        // music : Group1 "▃ ▄ ▅ ▆ ▇ █ Chaines Musicals █ ▇ ▆ ▅ ▄ ▃"
        // tv xxx : Group1 "•●★-XXX ADULT-★●•" "====== film adult ======="

        private static Dictionary<string, string> _group1Filter = null;
        private static Dictionary<string, string> _group2Filter = null;
        public static void InitFilter()
        {
            _group1Filter = new Dictionary<string, string>();
            _group2Filter = new Dictionary<string, string>();
        }

        public static void AddGroup1Filter(string group1)
        {
            _group1Filter.Add(group1.ToLower(), null);
        }

        public static void AddGroup2Filter(string group2)
        {
            group2 = group2.ToLower();
            if (!_group2Filter.ContainsKey(group2))
                _group2Filter.Add(group2, null);
        }

        public static void InitFilter_TvFrance()
        {
            InitFilter();
            Filter_AddTvFrance();
        }

        public static void InitFilter_RadioFrance()
        {
            InitFilter();
            Filter_AddRadioFrance();
        }

        public static void Filter_AddFilmFrance()
        {
            AddGroup1Filter("===== Film Francais =========");
            AddGroup1Filter("==== Film à la Demande (VOD) ====");
            AddGroup1Filter("===== spectacle ======");
            AddGroup1Filter("======= Documentaire =========");
            AddGroup1Filter("===== Serie en Francais ========");
        }

        public static void Filter_AddTvFrance()
        {
            AddGroup1Filter("•●★-CANAL PLAY FR & A LA CARTE-★●•");
            AddGroup1Filter("==========  Chaines   TNT  ==========");
            AddGroup1Filter("======  Canal +  ======");
            AddGroup1Filter("======  CINEMA  ======");
            AddGroup1Filter("======  SPORT  ======");
            AddGroup1Filter("▃ ▄ ▅ ▆ ▇ █ SFR France █ ▇ ▆ ▅ ▄ ▃");

            AddGroup2Filter("FR");
            AddGroup2Filter("box");
        }

        public static void Filter_AddRadioFrance()
        {
            AddGroup1Filter("****** Radio FR **********");
            AddGroup2Filter("FR");
        }

        public static void Filter_AddTvMusicFrance()
        {
            AddGroup1Filter("▃ ▄ ▅ ▆ ▇ █ Chaines Musicals █ ▇ ▆ ▅ ▄ ▃");
            AddGroup2Filter("FR");
        }

        public static bool Filter(string group1, string group2)
        {
            //if (playInfo.Group1 != null && _group1Filter.ContainsKey(playInfo.Group1.ToLower()) && playInfo.Group2 != null && _group2Filter.ContainsKey(playInfo.Group2.ToLower()))
            if (FilterGroup1(group1) && FilterGroup2(group2))
                return true;
            else
                return false;
        }

        public static bool FilterGroup1(string group1)
        {
            if (group1 != null && _group1Filter.ContainsKey(group1.ToLower()))
                return true;
            else
                return false;
        }

        public static bool FilterGroup2(string group2)
        {
            if (group2 != null && _group2Filter.ContainsKey(group2.ToLower()))
                return true;
            else
                return false;
        }
    }
}
