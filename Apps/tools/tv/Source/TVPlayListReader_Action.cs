using pb;
using pb.Data.Xml;
using pb.IO;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace tv
{
    public static partial class TVPlayListReader
    {
        // control que les chaines dont group2 est "fr" ou "box" ont KnowGroup1 non nul
        public static IEnumerable<TVPlayInfo> ControlGroupFrance(string file)
        {
            //InitFilter();
            //Filter_AddTvFrance();
            //Filter_AddRadioFrance();
            //Filter_AddTvMusicFrance();
            //foreach (TVPlayInfo playInfo in Read(file, group: true))
            //{
            //    //if (playInfo.Group2 != null && _group2Filter.ContainsKey(playInfo.Group2) && (playInfo.Group1 == null || !_group1Filter.ContainsKey(playInfo.Group1)))
            //    if (FilterGroup2(playInfo.Group2) && (playInfo.Group1 == null || !FilterGroup1(playInfo.Group1)))
            //        yield return playInfo;
            //}
            return Read(file, group: true).Where(playInfo => (playInfo.Group2 == "fr" || playInfo.Group2 == "box") && playInfo.TvCategory == null);
        }

        // control que toutes les chaines de selectFile existent encore dans newFile
        public static IEnumerable<TVPlayInfo> ControlIptv(string selectFile, string newFile)
        {
            //InitFilter();
            //Filter_AddTvFrance();
            Dictionary<string, TVPlayInfo> _newTv = new Dictionary<string, TVPlayInfo>();
            foreach (TVPlayInfo playInfo in Read(newFile, group: true))
            {
                if (playInfo.Id == null || playInfo.TvCategory == null)
                    continue;
                //if (!Filter(playInfo.Group1, playInfo.Group2))
                //    continue;
                if (!_newTv.ContainsKey(playInfo.Id))
                    _newTv.Add(playInfo.Id, playInfo);
                else
                    Trace.WriteLine($"warning duplicate tv key \"{playInfo.Id}\" label \"{playInfo.PlayInfo.Label}\"");
            }
            foreach (TVPlayInfo playInfo in Read(selectFile, group: false))
            {
                if (playInfo.Type == "free")
                    continue;

                if (playInfo.Id == null)
                {
                    if (playInfo.PlayInfo.Link != null)
                        Trace.WriteLine($"warning key is null label \"{playInfo.PlayInfo.Label}\" link \"{playInfo.PlayInfo.Link}\"");
                    continue;
                }
                if (!_newTv.ContainsKey(playInfo.Id))
                    yield return playInfo;
            }
        }

        public static void SavePlayList(string file, IEnumerable<TVPlayInfo> playInfos)
        {
            string tvCategory = null;
            using (StreamWriter sw = new StreamWriter(zFile.Create(file)))
            {
                foreach (TVPlayInfo playInfo in playInfos)
                {
                    if (playInfo.TvCategory != tvCategory)
                    {
                        sw.WriteLine("# " + playInfo.Group1);
                        tvCategory = playInfo.TvCategory;
                    }
                    sw.WriteLine(playInfo.PlayInfo.Comment);
                    sw.WriteLine(playInfo.PlayInfo.Link);
                }
            }
        }

        public static void UpdateNewTvPlayList(string newSelectPlayList, string selectPlayList, string newPlayList, string fileCategory)
        {
            // newSelectPlayList : "new.m3u"
            // selectPlayList    : "tv_channels_pbambou_01_03_02.m3u"
            // newPlayList       : "source\tv_channels_pbambou_08.m3u"
            // fileCategory      : "tv"
            newSelectPlayList = newSelectPlayList.zRootPath(GetPlayListDirectory());
            string newSelectPlayList2 = zfile.GetNewFilename(newSelectPlayList);
            SavePlayList(newSelectPlayList2, GetNewTvPlayList(selectPlayList, newSelectPlayList, newPlayList, fileCategory));
        }

        public static IEnumerable<TVPlayInfo> GetNewTvPlayList(string selectPlayList, string newSelectPlayList, string newPlayList, string fileCategory)
        {
            // selectPlayList    : "tv_channels_pbambou_01_03_02.m3u"
            // newPlayList       : "source\tv_channels_pbambou_08.m3u"
            // newSelectPlayList : "new.m3u"
            // fileCategory      : "tv"

            //InitFilter();
            //Filter_AddTvFrance();

            //string s;
            //if (!XmlConfig.CurrentConfig.XDocument.Root.zTryGetVariableValue(selectPlayList, out s, traceError: true))
            //    yield break;
            //selectPlayList = s;
            //if (!XmlConfig.CurrentConfig.XDocument.Root.zTryGetVariableValue(newSelectPlayList, out s, traceError: true))
            //    yield break;
            //newSelectPlayList = s;
            //if (!XmlConfig.CurrentConfig.XDocument.Root.zTryGetVariableValue(newPlayList, out s, traceError: true))
            //    yield break;
            //newPlayList = s;

            selectPlayList = selectPlayList.zRootPath(GetPlayListDirectory());
            newSelectPlayList = newSelectPlayList.zRootPath(GetPlayListDirectory());
            newPlayList = newPlayList.zRootPath(GetPlayListDirectory());

            Dictionary<string, TVPlayInfo> selectTv = new Dictionary<string, TVPlayInfo>();
            AddToDictionary(selectTv, Read(selectPlayList, group: false));
            if (zFile.Exists(newSelectPlayList))
                AddToDictionary(selectTv, Read(newSelectPlayList, group: false));

            foreach (TVPlayInfo playInfo in Read(newPlayList, group: true))
            {
                if (playInfo.Id == null)
                    continue;
                //if (!Filter(playInfo.Group1, playInfo.Group2))
                //    continue;
                if (playInfo.FileCategory == fileCategory && !selectTv.ContainsKey(playInfo.Id))
                    yield return playInfo;
            }
        }

        public static string GetPlayListDirectory()
        {
            return XmlConfig.CurrentConfig.GetExplicit("PlayListDirectory");
        }

        public static void AddToDictionary(Dictionary<string, TVPlayInfo> selectTv, IEnumerable<TVPlayInfo> tvPlayList)
        {
            foreach (TVPlayInfo playInfo in tvPlayList)
            {
                if (playInfo.Id == null)
                {
                    if (playInfo.PlayInfo.Link != null)
                        Trace.WriteLine($"warning key is null label \"{playInfo.PlayInfo.Label}\" link \"{playInfo.PlayInfo.Link}\"");
                    continue;
                }
                if (!selectTv.ContainsKey(playInfo.Id))
                    selectTv.Add(playInfo.Id, playInfo);
                else
                    Trace.WriteLine($"warning duplicate tv key \"{playInfo.Id}\" label \"{playInfo.PlayInfo.Label}\"");
            }
        }
    }
}
