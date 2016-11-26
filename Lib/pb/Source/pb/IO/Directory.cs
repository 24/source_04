using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DirectoryInfo = Pri.LongPath.DirectoryInfo;

namespace pb.IO
{
    public enum SortOption
    {
        Ascending = 1,
        Descending
    }

    public class EnumDirectory
    {
        public DirectoryInfo directoryInfo;
        public IEnumerator<DirectoryInfo> directoryEnum;

        public EnumDirectory(string directory, string pattern = null)
        {
            this.directoryInfo = new DirectoryInfo(directory);
            if (pattern == null)
                pattern = "*.*";
            this.directoryEnum = directoryInfo.EnumerateDirectories(pattern).GetEnumerator();
        }

        public EnumDirectory(DirectoryInfo directoryInfo, string pattern = null)
        {
            this.directoryInfo = directoryInfo;
            if (pattern == null)
                pattern = "*.*";
            this.directoryEnum = directoryInfo.EnumerateDirectories(pattern).GetEnumerator();
        }
    }

    public class EnumDirectoryInfo
    {
        public string Directory;              // path complet avec le numéro (1) ou [1]
        public string SubDirectory;           // avec le numéro (1) ou [1]
        public string Name;                   // sans le numéro (1) ou [1]
        public int Number;                    // (1) ou [1]
        public int Level;
    }

    public class EnumDirectoryFilter
    {
        public bool Select;
        public bool RecurseSubDirectory;
    }

    public class EnumFileInfo
    {
        public string File;
        public string SubDirectory;
        public int Level;
    }

    public static class zdir
    {
        public static IEnumerable<string> Files(string directory, string pattern = "*.*", SearchOption searchOption = SearchOption.TopDirectoryOnly, SortOption sortOption = SortOption.Ascending)
        {
            var q = zDirectory.EnumerateFiles(directory, pattern, searchOption);
            if (sortOption == SortOption.Ascending)
                return q;
            else
                return q.OrderByDescending(file => file);
        }

        public static void CreateDirectory(string dir)
        {
            if (!zDirectory.Exists(dir))
                zDirectory.CreateDirectory(dir);
        }

        public static string GetNewDirectory(string directory)
        {
            if (zDirectory.Exists(directory))
            {
                for (int i = 1; ; i++)
                {
                    //string newDirectory = zPath.Combine(directory, string.Format("{0}[{1}]", directory, i));
                    string newDirectory = string.Format("{0}[{1}]", directory, i);
                    if (!zDirectory.Exists(newDirectory))
                    {
                        directory = newDirectory;
                        break;
                    }
                }

            }
            return directory;
        }

        public static bool IsDirectoryEmpty(string directory)
        {
            return IsDirectoryEmpty(new DirectoryInfo(directory));
        }

        public static bool IsDirectoryEmpty(DirectoryInfo directoryInfo)
        {
            if (directoryInfo.EnumerateDirectories().FirstOrDefault() != null)
                return false;
            if (directoryInfo.EnumerateFiles().FirstOrDefault() != null)
                return false;
            return true;
        }

        //public static IEnumerable<string> EnumerateDirectories(string directory, string pattern = null, int minLevel = 0, int maxLevel = 0)
        //{
        //    if (!Directory.Exists(directory))
        //        yield break;
        //    Stack<EnumDirectory> directoryStack = new Stack<EnumDirectory>();
        //    EnumDirectory enumDirectory = new EnumDirectory(directory, pattern);
        //    int level = 1;
        //    while (true)
        //    {
        //        if (enumDirectory.directoryEnum.MoveNext())
        //        {
        //            if (minLevel == 0 || level >= minLevel)
        //                yield return enumDirectory.directoryEnum.Current.FullName;
        //            if (maxLevel == 0 || level < maxLevel)
        //            {
        //                level++;
        //                directoryStack.Push(enumDirectory);
        //                enumDirectory = new EnumDirectory(enumDirectory.directoryEnum.Current, pattern);
        //            }
        //        }
        //        else
        //        {
        //            if (directoryStack.Count == 0)
        //                break;
        //            enumDirectory = directoryStack.Pop();
        //            level--;
        //        }
        //    }
        //}

        public static IEnumerable<string> EnumerateDirectories(string directory, string pattern = null, int minLevel = 0, int maxLevel = 0)
        {
            Func<EnumDirectoryInfo, EnumDirectoryFilter> directoryFilter =
                dirInfo =>
                    new EnumDirectoryFilter
                    {
                        Select = (minLevel == 0 || dirInfo.Level >= minLevel) && (maxLevel == 0 || dirInfo.Level <= maxLevel),
                        RecurseSubDirectory = maxLevel == 0 || dirInfo.Level < maxLevel
                    };
            return from dir in EnumerateDirectoriesInfo(directory, pattern, new Func<EnumDirectoryInfo, EnumDirectoryFilter>[] { directoryFilter }) select dir.Directory;
        }

        public static IEnumerable<EnumDirectoryInfo> EnumerateDirectoriesInfo(string directory, int minLevel, int maxLevel = 0, string pattern = null,
            Func<EnumDirectoryInfo, EnumDirectoryFilter> directoryFilter = null, bool getSubDirectoryNumber = false)
        {
            Func<EnumDirectoryInfo, EnumDirectoryFilter> directoryFilter2 =
                dirInfo =>
                {
                    EnumDirectoryFilter result = new EnumDirectoryFilter
                    {
                        Select = (minLevel == 0 || dirInfo.Level >= minLevel) && (maxLevel == 0 || dirInfo.Level <= maxLevel),
                        RecurseSubDirectory = maxLevel == 0 || dirInfo.Level < maxLevel
                    };
                    if (directoryFilter != null && (result.Select || result.RecurseSubDirectory))
                    {
                        EnumDirectoryFilter result2 = directoryFilter(dirInfo);
                        result.Select = result.Select & result2.Select;
                        result.RecurseSubDirectory = result.RecurseSubDirectory & result2.RecurseSubDirectory;
                    }
                    return result;
                };

            return EnumerateDirectoriesInfo(directory, pattern, directoryFilters: new Func<EnumDirectoryInfo, EnumDirectoryFilter>[] { directoryFilter2 }, getSubDirectoryNumber: getSubDirectoryNumber);
        }

        // followDirectoryTree : followDirectoryTree est appelé quand on entre dans un répertoire et quand on en sort
        //                       à l'entrée EnumDirectoryInfo contient les info du répertoire
        //                       à la sortie EnumDirectoryInfo contient uniquement le Level
        // followDirectoryTree exemple EnumerateDirectoriesInfo(@"c:\toto");
        //   entré dans tata EnumDirectoryInfo = Directory: "c:\toto\tata" SubDirectory: "tata" Level: 1
        //   entré dans tutu EnumDirectoryInfo = Directory: "c:\toto\tata\tutu" SubDirectory: "tutu" Level: 2
        //   sortie de tutu EnumDirectoryInfo = Directory: null SubDirectory: null Level: 2
        //   sortie de tata EnumDirectoryInfo = Directory: null SubDirectory: null Level: 1
        public static IEnumerable<EnumDirectoryInfo> EnumerateDirectoriesInfo(string directory, string pattern = null,
            //Func<EnumDirectoryInfo, EnumDirectoryFilter> directoryFilter = null,
            IEnumerable<Func<EnumDirectoryInfo, EnumDirectoryFilter>> directoryFilters = null,
            //Action<EnumDirectoryInfo> followDirectoryTree = null,
            IEnumerable<Action<EnumDirectoryInfo>> followDirectoryTrees = null,
            bool getSubDirectoryNumber = false)
        {
            if (!zDirectory.Exists(directory))
                yield break;
            Stack<EnumDirectory> directoryStack = new Stack<EnumDirectory>();
            EnumDirectory enumDirectory = new EnumDirectory(directory, pattern);
            EnumDirectoryFilter enumDirectoryFilter = new EnumDirectoryFilter { Select = true, RecurseSubDirectory = true };
            int l = directory.Length + 1;
            int level = 1;
            while (true)
            {
                if (enumDirectory.directoryEnum.MoveNext())
                {
                    DirectoryInfo directoryInfo = enumDirectory.directoryEnum.Current;
                    EnumDirectoryInfo enumDirectoryInfo = new EnumDirectoryInfo
                        { Directory = directoryInfo.FullName, SubDirectory = directoryInfo.FullName.Substring(l), Name = directoryInfo.Name, Level = level };

                    if (getSubDirectoryNumber)
                    {
                        FilenameNumberInfo directoryNumberInfo = FilenameNumberInfo.GetFilenameNumberInfo(enumDirectoryInfo.Name);
                        enumDirectoryInfo.Name = directoryNumberInfo.BasePath;
                        enumDirectoryInfo.Number = directoryNumberInfo.Number;
                    }

                    //if (directoryFilter != null)
                    //    enumDirectoryFilter = directoryFilter(enumDirectoryInfo);
                    if (directoryFilters != null)
                        enumDirectoryFilter = EvaluateDirectoryFilters(directoryFilters, enumDirectoryInfo);

                    if (enumDirectoryFilter.Select)
                    {
                        //if (followDirectoryTree != null)
                        //    followDirectoryTree(enumDirectoryInfo);
                        if (followDirectoryTrees != null)
                            FollowDirectoryTrees(followDirectoryTrees, enumDirectoryInfo);

                        //if (getSubDirectoryNumber)
                        //{
                        //    FilenameNumberInfo directoryNumberInfo = FilenameNumberInfo.GetFilenameNumberInfo(enumDirectoryInfo.SubDirectory);
                        //    //enumDirectoryInfo.SubDirectory = directoryNumberInfo.BaseFilename;
                        //    enumDirectoryInfo.SubDirectory = directoryNumberInfo.BasePath;
                        //    enumDirectoryInfo.Number = directoryNumberInfo.Number;
                        //}
                        yield return enumDirectoryInfo;

                        //if (!enumDirectoryFilter.RecurseSubDirectory && followDirectoryTree != null)
                        //    followDirectoryTree(new EnumDirectoryInfo { Level = level });
                        if (!enumDirectoryFilter.RecurseSubDirectory && followDirectoryTrees != null)
                            FollowDirectoryTrees(followDirectoryTrees, new EnumDirectoryInfo { Level = level });
                    }

                    if (enumDirectoryFilter.RecurseSubDirectory)
                    {
                        //if (!enumDirectoryFilter.Select && followDirectoryTree != null)
                        //    followDirectoryTree(enumDirectoryInfo);
                        if (!enumDirectoryFilter.Select && followDirectoryTrees != null)
                            FollowDirectoryTrees(followDirectoryTrees, enumDirectoryInfo);

                        directoryStack.Push(enumDirectory);
                        enumDirectory = new EnumDirectory(enumDirectory.directoryEnum.Current, pattern);
                        level++;
                    }
                }
                else
                {
                    if (directoryStack.Count == 0)
                        break;
                    enumDirectory = directoryStack.Pop();
                    level--;

                    //if (followDirectoryTree != null)
                    //    followDirectoryTree(new EnumDirectoryInfo { Level = level });
                    if (followDirectoryTrees != null)
                        FollowDirectoryTrees(followDirectoryTrees, new EnumDirectoryInfo { Level = level });
                }
            }
        }

        private static EnumDirectoryFilter EvaluateDirectoryFilters(IEnumerable<Func<EnumDirectoryInfo, EnumDirectoryFilter>> directoryFilters, EnumDirectoryInfo enumDirectoryInfo)
        {
            bool select = true;
            bool recurseSubDirectory = true;
            foreach (Func<EnumDirectoryInfo, EnumDirectoryFilter> directoryFilter in directoryFilters)
            {
                EnumDirectoryFilter enumDirectoryFilter = directoryFilter(enumDirectoryInfo);
                if (select)
                    select = enumDirectoryFilter.Select;
                if (recurseSubDirectory)
                    recurseSubDirectory = enumDirectoryFilter.RecurseSubDirectory;
                if (!select && !recurseSubDirectory)
                    break;
            }
            return new EnumDirectoryFilter { Select = select, RecurseSubDirectory = recurseSubDirectory };
        }

        private static void FollowDirectoryTrees(IEnumerable<Action<EnumDirectoryInfo>> followDirectoryTrees, EnumDirectoryInfo enumDirectoryInfo)
        {
            foreach (Action<EnumDirectoryInfo> followDirectoryTree in followDirectoryTrees)
                followDirectoryTree(enumDirectoryInfo);
        }

        // Func<EnumDirectoryInfo, EnumDirectoryFilter> directoryFilter
        // Action<EnumDirectoryInfo> followDirectoryTree
        public static IEnumerable<EnumFileInfo> EnumerateFilesInfo(string directory, string pattern = null,
            IEnumerable<Func<EnumDirectoryInfo, EnumDirectoryFilter>> directoryFilters = null,
            IEnumerable<Action<EnumDirectoryInfo>> followDirectoryTrees = null)
        {
            if (!zDirectory.Exists(directory))
                yield break;

            if (pattern == null)
                pattern = "*.*";

            foreach (string file in zDirectory.EnumerateFiles(directory, pattern))
                yield return new EnumFileInfo { File = file, SubDirectory = "", Level = 0 };

            foreach (EnumDirectoryInfo directoryInfo in EnumerateDirectoriesInfo(directory, null, directoryFilters, followDirectoryTrees))
            {
                foreach (string file in zDirectory.EnumerateFiles(directoryInfo.Directory, pattern))
                    yield return new EnumFileInfo { File = file, SubDirectory = directoryInfo.SubDirectory, Level = directoryInfo.Level };
            }
        }

        // return true if directory is deleted otherwise false
        //public static bool DeleteEmptyDirectory(string directory, bool deleteOnlySubdirectory = true)
        public static bool DeleteEmptyDirectory(string directory, bool recurse = false)
        {
            if (!zDirectory.Exists(directory))
                return false;
            Stack<EnumDirectory> directoryStack = new Stack<EnumDirectory>();
            EnumDirectory enumDirectory = new EnumDirectory(directory);
            while (true)
            {
                if (enumDirectory.directoryEnum.MoveNext())
                {
                    directoryStack.Push(enumDirectory);
                    enumDirectory = new EnumDirectory(enumDirectory.directoryEnum.Current);
                }
                else
                {
                    if (directoryStack.Count == 0)
                    {
                        //if (!deleteOnlySubdirectory && IsDirectoryEmpty(enumDirectory.directoryInfo))
                        if (recurse && IsDirectoryEmpty(enumDirectory.directoryInfo))
                        {
                            enumDirectory.directoryInfo.Delete();
                            return true;
                        }
                        else
                            return false;
                    }
                    if (IsDirectoryEmpty(enumDirectory.directoryInfo))
                        enumDirectory.directoryInfo.Delete();
                    enumDirectory = directoryStack.Pop();
                }
            }
        }

        public static string GetNewIndexedDirectory(string dir, string dirname = null, int indexLength = 0, TextIndexOption option = TextIndexOption.NumberBefore)
        {
            if (dir == null)
                return null;

            int maxIndexLength;
            int index = GetLastDirectoryIndex(dir, dirname, option, out maxIndexLength) + 1;
            if (indexLength == 0)
                indexLength = maxIndexLength;
            if (indexLength == 0)
                indexLength = 4;
            string indexedDirectory = index.ToString();
            int l = indexedDirectory.Length;
            if (l < indexLength)
            {
                indexedDirectory = new string('0', indexLength - l) + indexedDirectory;
            }
            return zPath.Combine(dir, indexedDirectory + dirname);
        }

        //public static int GetLastDirectoryIndex(string dir, string dirname, out int maxIndexLength)
        public static int GetLastDirectoryIndex(string dir, string dirname, TextIndexOption option, out int maxIndexLength)
        {
            maxIndexLength = 0;
            if (!zDirectory.Exists(dir))
                return 0;
            //return Directory.EnumerateDirectories(dir).Select(d => zPath.GetFileName(d)).zGetLastTextIndex(dirname, out maxIndexLength);
            return zfile.GetLastTextIndex(zDirectory.EnumerateDirectories(dir).Select(d => zPath.GetFileName(d)), dirname, option, out maxIndexLength);
        }
    }
}
