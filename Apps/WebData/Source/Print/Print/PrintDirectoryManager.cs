using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using pb;
using pb.IO;

namespace Download.Print
{
    public enum PrintDirectoryDateStorageType
    {
        NoSubDirectory = 1,
        YearSubDirectory,
        MonthSubDirectory,
        DaySubDirectory
    }

    //public class SubDirectoryInfo
    //{
    //    public string Directory;
    //    public string SubDirectory;
    //    public PrintSubDirectoryType SubDirectoryType;
    //}

    public class PrintDirectoryDateStorageLevel
    {
        public PrintDirectoryDateStorageType Type;
        public string Prefix;

        public string GetDirectory(Date date)
        {
            string format = null;
            switch (Type)
            {
                case PrintDirectoryDateStorageType.YearSubDirectory:
                    format = "yyyy";
                    break;
                case PrintDirectoryDateStorageType.MonthSubDirectory:
                    format = "yyyy-MM";
                    break;
                case PrintDirectoryDateStorageType.DaySubDirectory:
                    format = "yyyy-MM-dd";
                    break;
            }
            if (format != null)
                return Prefix + date.ToString(format);
            else
                return null;
        }
    }

    public class PrintDirectoryDateStorage
    {
        public PrintDirectoryDateStorageLevel DirectoryLevel1;
        public PrintDirectoryDateStorageLevel DirectoryLevel2;

        public string GetDirectory(Date date)
        {
            string directory1 = DirectoryLevel1.GetDirectory(date);
            string directory2 = DirectoryLevel2.GetDirectory(date);
            if (directory2 != null)
            {
                if (directory1 != null)
                    return Path.Combine(directory1, directory2);
                else
                    return directory2;
            }
            else
                return directory1;
        }
    }

    public class PrintDirectoryInfo
    {
        public string Directory;                             // "g:\\pib\\media\\print\\.01_quotidien\\Journaux"
        public string SubDirectory;                          // ".01_quotidien\\Journaux"
        public int Level;                                    // 1
        //public PrintSubDirectoryType SubDirectory1Type;      // MonthSubDirectory
        //public PrintSubDirectoryType SubDirectory2Type;      // DaySubDirectory
        //public string SubDirectory2Prefix;                   // "Journaux - "
        //public PrintSubDirectoryInfo SubDirectory1Type;
        //public PrintSubDirectoryInfo SubDirectory2Type;
        //public bool HasSubDirectory;
    }

    public class PrintFileInfo
    {
        public PrintDirectoryInfo DirectoryInfo;
        public string File;
        public string BaseFilename;
        public int Number;
    }

    public class PrintDirectoryManager
    {
        private string[] _directories;        // ".01_quotidien", ".02_hebdo", ..., from print_list2.xml
        //private int _level = 1;

        public PrintDirectoryManager(string[] directories)
        {
            _directories = directories;
        }

        public Dictionary<string, List<PrintDirectoryInfo>> GetDirectoryGroups(string[] directories)
        {
            Dictionary<string, List<PrintDirectoryInfo>> directoryGroups = new Dictionary<string, List<PrintDirectoryInfo>>();
            foreach (string directory in directories)
            {
                // directory : "g:\pib\media\print", "c:\pib\_dl\_pib\dl\print"
                directoryGroups.zAddKeyList(GetDirectories(directory), dir => dir.SubDirectory);
            }
            return directoryGroups;
        }

        public IEnumerable<PrintDirectoryInfo> GetDirectories(string directory)
        {
            foreach (string directory2 in _directories)
            {
                foreach (EnumDirectoryInfo directoryInfo in zdir.EnumerateDirectoriesInfo(Path.Combine(directory, directory2), minLevel: 1, maxLevel: 1))
                {
                    PrintDirectoryInfo printDirectoryInfo = new PrintDirectoryInfo();
                    printDirectoryInfo.Directory = directoryInfo.Directory;
                    printDirectoryInfo.SubDirectory = Path.Combine(directory2, directoryInfo.SubDirectory);
                    printDirectoryInfo.Level = directoryInfo.Level;
                    //printDirectoryInfo.SubDirectory1Type = GetSubDirectoryType(printDirectoryInfo.Directory);
                    //GetSubDirectoryType(printDirectoryInfo);
                    yield return printDirectoryInfo;
                }
            }
        }

        public static PrintDirectoryDateStorage GetDirectoryDateStorage(string directory)
        {
            PrintDirectoryDateStorage directoryDateStorage = new PrintDirectoryDateStorage
                {
                    DirectoryLevel1 = new PrintDirectoryDateStorageLevel { Type = PrintDirectoryDateStorageType.NoSubDirectory },
                    DirectoryLevel2 = new PrintDirectoryDateStorageLevel { Type = PrintDirectoryDateStorageType.NoSubDirectory }
                };
            foreach (EnumDirectoryInfo directoryInfo in zdir.EnumerateDirectoriesInfo(directory, minLevel: 1, maxLevel: 1))
            {
                // priority DaySubDirectory, MonthSubDirectory, YearSubDirectory
                PrintDirectoryDateStorageLevel directoryDateStorageLevel = GetDirectoryDateStorageLevel(directoryInfo.SubDirectory);
                if (directoryDateStorageLevel.Type > directoryDateStorage.DirectoryLevel1.Type)
                {
                    directoryDateStorage.DirectoryLevel1 = directoryDateStorageLevel;
                    directoryDateStorage.DirectoryLevel2 = new PrintDirectoryDateStorageLevel { Type = PrintDirectoryDateStorageType.NoSubDirectory };
                }
                if (directoryDateStorageLevel.Type == directoryDateStorage.DirectoryLevel1.Type && directoryDateStorageLevel.Type != PrintDirectoryDateStorageType.NoSubDirectory)
                {
                    foreach (EnumDirectoryInfo directoryInfo2 in zdir.EnumerateDirectoriesInfo(directoryInfo.Directory, minLevel: 1, maxLevel: 1))
                    {
                        directoryDateStorageLevel = GetDirectoryDateStorageLevel(directoryInfo2.SubDirectory);
                        if (directoryDateStorageLevel.Type > directoryDateStorage.DirectoryLevel2.Type)
                            directoryDateStorage.DirectoryLevel2 = directoryDateStorageLevel;
                    }
                }
            }
            //printDirectoryInfo.SubDirectory1Type = printSubDirectory1Info;
            //printDirectoryInfo.SubDirectory2Type = printSubDirectory2Info;
            return directoryDateStorage;
        }

        //private static Regex __rgSubDirectoryType = new Regex("^(([0-9]{4})|([0-9]{4}-[0-9]{2}))$", RegexOptions.Compiled);
        private static Regex __rgDirectoryDateStorageLevel = new Regex("^(([0-9]{4})|([0-9]{4}-[0-9]{2})|(.*)([0-9]{4}-[0-9]{2}-[0-9]{2}))$", RegexOptions.Compiled);
        public static PrintDirectoryDateStorageLevel GetDirectoryDateStorageLevel(string subDirectory)
        {
            Match match = __rgDirectoryDateStorageLevel.Match(subDirectory);
            PrintDirectoryDateStorageType directoryDateStorageType = PrintDirectoryDateStorageType.NoSubDirectory;
            string prefix = null;
            if (match.Success)
            {
                if (match.Groups[2].Value != "")
                    directoryDateStorageType = PrintDirectoryDateStorageType.YearSubDirectory;
                else if (match.Groups[3].Value != "")
                    directoryDateStorageType = PrintDirectoryDateStorageType.MonthSubDirectory;
                else if (match.Groups[5].Value != "")
                {
                    if (match.Groups[4].Value != "")
                        prefix = match.Groups[4].Value;
                    directoryDateStorageType = PrintDirectoryDateStorageType.DaySubDirectory;
                }
            }
            return new PrintDirectoryDateStorageLevel { Type = directoryDateStorageType, Prefix = prefix };
        }

        //public static PrintSubDirectoryType GetSubDirectoryType(string directory)
        //{
        //    PrintSubDirectoryType type = PrintSubDirectoryType.NoSubDirectory;
        //    foreach (SubDirectoryInfo subDirectoryInfo in EnumerateSubDirectoryType(directory))
        //    {
        //        // priority DaySubDirectory, MonthSubDirectory, YearSubDirectory
        //        if (subDirectoryInfo.SubDirectoryType == PrintSubDirectoryType.YearSubDirectory)
        //        {
        //            if (type != PrintSubDirectoryType.MonthSubDirectory)
        //                type = PrintSubDirectoryType.YearSubDirectory;
        //        }
        //        else if (subDirectoryInfo.SubDirectoryType == PrintSubDirectoryType.MonthSubDirectory)
        //        {
        //            type = PrintSubDirectoryType.MonthSubDirectory;
        //        }
        //    }
        //    return type;
        //}

        //private static IEnumerable<SubDirectoryInfo> EnumerateSubDirectoryType(string directory)
        //{
        //    foreach (EnumDirectoryInfo directoryInfo in zdir.EnumerateDirectoriesInfo(directory, minLevel: 1, maxLevel: 1))
        //    {
        //        SubDirectoryInfo subDirectoryInfo = new SubDirectoryInfo();
        //        subDirectoryInfo.Directory = directoryInfo.Directory;
        //        subDirectoryInfo.SubDirectory = directoryInfo.SubDirectory;
        //        Match match = __rgSubDirectoryType.Match(directoryInfo.SubDirectory);
        //        if (match.Success)
        //        {
        //            if (match.Groups[2].Value != "")
        //                subDirectoryInfo.SubDirectoryType = PrintSubDirectoryType.YearSubDirectory;
        //            else if (match.Groups[3].Value != "")
        //                subDirectoryInfo.SubDirectoryType = PrintSubDirectoryType.MonthSubDirectory;
        //        }
        //        else
        //            subDirectoryInfo.SubDirectoryType = PrintSubDirectoryType.NoSubDirectory;
        //        yield return subDirectoryInfo;
        //    }
        //}

        private static Regex __rgSubDirectoryPartOfFilename = new Regex("[0-9]{4}-[0-9]{2}-[0-9]{2}$", RegexOptions.Compiled);
        public static bool IsSubdirectoryPartOfFilename(string subdirectory)
        {
            // Journaux - 2014-10-12
            // Médiapart - 2012-05-10
            return __rgSubDirectoryPartOfFilename.IsMatch(subdirectory);
        }

        public Dictionary<string, List<PrintFileInfo>> GetFileGroups(IEnumerable<PrintDirectoryInfo> directories)
        {
            //public class PrintFileInfo
            //{
            //    public PrintDirectoryInfo DirectoryInfo;
            //    public string File;
            //    public string BaseFilename;
            //    public int Number;
            //}
            Dictionary<string, List<PrintFileInfo>> fileGroups = new Dictionary<string, List<PrintFileInfo>>();
            foreach (PrintDirectoryInfo directoryInfo in directories)
            {
                var query = zdir.EnumerateFilesInfo(directoryInfo.Directory, followDirectoryTree: dir => { });
            }
            return fileGroups;
        }

        //public static void GetSubDirectoryType(PrintDirectoryInfo printDirectoryInfo)
        //{
        //    PrintSubDirectoryType type = PrintSubDirectoryType.NoSubDirectory;
        //    bool hasSubDirectory = false;
        //    bool hasSubDirectory2 = false;
        //    foreach (string subDirectory in Directory.EnumerateDirectories(printDirectoryInfo.Directory))
        //    {
        //        Match match = __rgSubDirectoryType.Match(Path.GetFileName(subDirectory));
        //        if (match.Success)
        //        {
        //            if (match.Groups[2].Value != "")
        //            {
        //                if (type != PrintSubDirectoryType.MonthSubDirectory)
        //                    type = PrintSubDirectoryType.YearSubDirectory;
        //                if (Directory.EnumerateDirectories(subDirectory).FirstOrDefault() != null)
        //                    hasSubDirectory = true;
        //            }
        //            else if (match.Groups[3].Value != "")
        //            {
        //                type = PrintSubDirectoryType.MonthSubDirectory;
        //                if (Directory.EnumerateDirectories(subDirectory).FirstOrDefault() != null)
        //                    hasSubDirectory = true;
        //            }
        //        }
        //        else
        //            hasSubDirectory2 = true;
        //    }
        //    if (type == PrintSubDirectoryType.NoSubDirectory)
        //        hasSubDirectory = hasSubDirectory2;
        //    printDirectoryInfo.SubDirectoryType = type;
        //    printDirectoryInfo.HasSubDirectory = hasSubDirectory;
        //}
    }
}
