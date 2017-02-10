using pb.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace tv
{
    public class PlayInfo
    {
        public string Comment;
        public bool ExtInf;
        public int Number;
        public string Label;
        public string Link;
    }

    public class PlayListReader
    {
        private string _file = null;

        public PlayListReader(string file)
        {
            _file = file;
        }

        public IEnumerable<PlayInfo> Read()
        {
            string comment = null;
            foreach (string line in zFile.ReadLines(_file))
            {
                if (line == "")
                    continue;
                // #EXTM3U
                // #EXTINF:-1,FR_ANIMAUX_HD
                // http://tv-fan-sat.ovh:25461/live/pbambou/vLKBI80iYs/114.ts
                if (line.StartsWith("#"))
                {
                    if (comment != null)
                    {
                        PlayInfo playInfo = new PlayInfo();
                        SetComment(playInfo, comment);
                        yield return playInfo;
                    }
                    comment = line;
                }
                else
                {
                    PlayInfo playInfo = new PlayInfo { Link = line };
                    SetComment(playInfo, comment);
                    comment = null;
                    yield return playInfo;
                }
            }
        }

        private static Regex _commentExtInf = new Regex("^#EXTINF:(?:(-?[0-9]+),)?(.*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static void SetComment(PlayInfo playInfo, string comment)
        {
            if (comment == null)
                return;
            playInfo.Comment = comment;
            Match match = _commentExtInf.Match(comment);
            if (match.Success)
            {
                playInfo.ExtInf = true;
                playInfo.Number = int.Parse(match.Groups[1].Value);
                playInfo.Label = match.Groups[2].Value;
            }
        }

        public static IEnumerable<PlayInfo> Read(string file)
        {
            return new PlayListReader(file).Read();
        }
    }
}
