using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace tv
{
    public class TVPlayInfo
    {
        public string Id;
        public string Type;
        public string Group1;
        public string TvCategory;
        public string FileCategory;
        public string Group2;
        public PlayInfo PlayInfo;
    }

    public static partial class TVPlayListReader
    {
        private static Dictionary<string, string> _tvCategories = null;
        private static Dictionary<string, string> _fileCategories = null;

        public static IEnumerable<TVPlayInfo> Read(string file, bool group = false)
        {
            if (group)
                InitCategories();
            //return PlayListReader.Read(file).Select(playInfo => GetTVPlayInfo(playInfo));
            string group1 = null;
            foreach (PlayInfo playInfo in PlayListReader.Read(file))
            {
                string id;
                string type;
                GetIdFromLink(playInfo.Link, out id, out type);
                string playGroup1 = null;
                string playGroup2 = null;
                string tvCategory = null;
                string fileCategory = null;
                if (group)
                {
                    GetGroupsFromLabel(playInfo.Label, out playGroup1, out playGroup2);
                    if (playGroup1 != null)
                    {
                        group1 = playGroup1;
                        continue;
                    }
                    if (group1 != null && _tvCategories.ContainsKey(group1))
                    {
                        tvCategory = _tvCategories[group1];
                        fileCategory = _fileCategories[tvCategory];
                    }
                    playGroup2 = playGroup2?.ToLower();
                }
                yield return new TVPlayInfo { Id = id, Type = type, Group1 = group1, Group2 = playGroup2, TvCategory = tvCategory, FileCategory = fileCategory, PlayInfo = playInfo };
            }
        }

        //service=739&flavour=sd
        private static Regex _freeboxTvLinkId = new Regex("service=([0-9]+)&flavour=(sd|hd|ld)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        //public static string GetIdFromLink(string  link)
        public static void GetIdFromLink(string link, out string id, out string type)
        {
            id = null;
            type = null;
            if (link != null)
            {
                if (link.StartsWith("http://tv-fan-sat.ovh:"))
                {
                    //http://tv-fan-sat.ovh:25461/live/pbambou/vLKBI80iYs/101.ts
                    Uri uri = new Uri(link);
                    string s = uri.Segments[uri.Segments.Length - 1];
                    int i = s.IndexOf('.');
                    if (i != -1)
                    {
                        if (int.TryParse(s.Substring(0, i), out i))
                        {
                            id = "iptv-" + i.ToString();
                            type = "iptv";
                        }
                    }
                }
                else if (link.StartsWith("rtsp://mafreebox.freebox.fr/"))
                {
                    //rtsp://mafreebox.freebox.fr/fbxtv_pub/stream?namespace=1&service=739&flavour=sd
                    Match match = _freeboxTvLinkId.Match(link);
                    if (match.Success)
                    {
                        id = "free-" + match.Groups[1].Value + "-" + match.Groups[2].Value;
                        type = "free";
                    }
                }
                //if (id == null)
                //    throw new PBException($"unknow link \"{link}\"");
            }
        }

        private static Regex _iptvGroup1 = new Regex(@"^(•●★|\*\*\*|===|▃ ▄)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static Regex _iptvGroup2 = new Regex(@"^([a-z]{2,5}|[a-z]{2,3}-[a-z]{2,3})[_: ]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static void GetGroupsFromLabel(string label, out string group1, out string group2)
        {
            group1 = group2 = null;
            if (label != null)
            {
                // #EXTINF:-1,•●★-BEIN SPORTS AR-★●•
                // #EXTINF:-1,•●★-ART & OSN-★●•
                // #EXTINF:-1,•●★-chaines du Grand Maghreb-★●•
                // #EXTINF:-1,****** Radio TN **********
                // #EXTINF:-1,•●★-ARABIC CHANNELS-★●•
                // #EXTINF:-1,•●★-Islamic-★●•
                // #EXTINF:-1,•●★-CANAL PLAY FR & A LA CARTE-★●•
                // #EXTINF:-1,==========  Chaines   TNT  ==========
                // #EXTINF:-1,======  Canal +  ======
                // #EXTINF:-1,======  CINEMA  ======
                // #EXTINF:-1,======  SPORT  ======
                // #EXTINF:-1,****** Radio FR **********
                // #EXTINF:-1,▃ ▄ ▅ ▆ ▇ █ SFR France █ ▇ ▆ ▅ ▄ ▃
                // #EXTINF:-1,======  CHAINES BELGES  ======
                // #EXTINF:-1,•●★-ITALIA-★●•
                // #EXTINF:-1,****** Radio IT **********
                // #EXTINF:-1,•●★-PORTUGAL-★●•
                // #EXTINF:-1,•●★-DEUTSCHLAND-★●•
                // #EXTINF:-1,•●★- Bulgarie-★●•
                // #EXTINF:-1,•●★-ESPANA-★●•
                // #EXTINF:-1,•●★-TÜRKIYA-★●•
                // #EXTINF:-1,•●★-UNITED KINGDOM-★●•
                // #EXTINF:-1,•●★-ASIA-★●•
                // #EXTINF:-1,•●★-BELG & NL-★●•
                // #EXTINF:-1,•●★-Bouquet Suisse-★●•
                // #EXTINF:-1,•●★-ROMANIA-★●•
                // #EXTINF:-1,•●★-EX YU-★●•
                // #EXTINF:-1,▃ ▄ ▅ ▆ ▇ █ Chaines Musicals █ ▇ ▆ ▅ ▄ ▃
                // #EXTINF:-1,▃ ▄ ▅ ▆ ▇ █ Chaines Kurd █ ▇ ▆ ▅ ▄ ▃
                // #EXTINF:-1,▃ ▄ ▅ ▆ ▇ █ Chaines Africaines  █ ▇ ▆ ▅ ▄ ▃
                // #EXTINF:-1,****** Radio AF **********
                // #EXTINF:-1,==== Chaine Polish =====
                // #EXTINF:-1,▃ ▄ ▅ ▆ ▇ █ Chaine Iran █ ▇ ▆ ▅ ▄ ▃
                // #EXTINF:-1,•●★-XXX ADULT-★●•
                // #EXTINF:-1,==== Film à la Demande (VOD) ====
                // #EXTINF:-1,======= Documentaire =========
                // #EXTINF:-1,===== Serie en Francais ========
                // #EXTINF:-1,===== Film in English ==========
                // #EXTINF:-1,===== Film Francais =========
                // #EXTINF:-1,===== spectacle ======
                // #EXTINF:-1,===== Film arabic ========
                // #EXTINF:-1,===== Film in Findi ==========
                // #EXTINF:-1,====== film adult =======
                // #EXTINF:-1,AR_Bein_Max_1_HD_Africa_Cup_2017
                // #EXTINF:-1,BeIN_Sport_01_HD1
                // #EXTINF:-1,RO Canal 3 HD
                // #EXTINF:-1,KU TV10
                // #EXTINF:-1,XXX Evil Angel.com
                // #EXTINF:-1,EX-YU_Cinestar_Action
                // #EXTINF:-1,AF: VEVO ROCK

                Match match = _iptvGroup1.Match(label);
                if (match.Success)
                    group1 = label;
                else
                {
                    match = _iptvGroup2.Match(label);
                    if (match.Success)
                        group2 = match.Groups[1].Value;
                    //else
                    //    group1 = label;
                }
            }
        }

        public static void InitCategories()
        {
            _tvCategories = new Dictionary<string, string>();

            _tvCategories.Add("======  Canal +  ======", "canal+");
            _tvCategories.Add("======  CINEMA  ======", "cinema");
            _tvCategories.Add("==========  Chaines   TNT  ==========", "tnt");
            _tvCategories.Add("======  SPORT  ======", "sport");

            _tvCategories.Add("•●★-CANAL PLAY FR & A LA CARTE-★●•", "canal play");
            _tvCategories.Add("▃ ▄ ▅ ▆ ▇ █ SFR France █ ▇ ▆ ▅ ▄ ▃", "sfr");
            _tvCategories.Add("===== Film Francais =========", "film");
            _tvCategories.Add("==== Film à la Demande (VOD) ====", "film vod");
            _tvCategories.Add("===== spectacle ======", "spectacle");
            _tvCategories.Add("======= Documentaire =========", "documentaire");
            _tvCategories.Add("===== Serie en Francais ========", "serie");
            _tvCategories.Add("****** Radio FR **********", "radio");
            _tvCategories.Add("▃ ▄ ▅ ▆ ▇ █ Chaines Musicals █ ▇ ▆ ▅ ▄ ▃", "music");

            _tvCategories.Add("•●★-XXX ADULT-★●•", "zzz");
            _tvCategories.Add("====== film adult =======", "zzz film");

            _fileCategories = new Dictionary<string, string>();

            _fileCategories.Add("canal+", "tv");
            _fileCategories.Add("cinema", "tv");
            _fileCategories.Add("tnt", "tv");
            _fileCategories.Add("sport", "tv");
            _fileCategories.Add("canal play", "tv");
            _fileCategories.Add("sfr", "tv");
            _fileCategories.Add("film", "tv");
            _fileCategories.Add("film vod", "tv");
            _fileCategories.Add("spectacle", "tv");
            _fileCategories.Add("documentaire", "tv");
            _fileCategories.Add("serie", "tv");
            _fileCategories.Add("radio", "tv");
            _fileCategories.Add("music", "tv");

            _fileCategories.Add("zzz", "zzz");
            _fileCategories.Add("zzz film", "zzz");
        }
    }
}
