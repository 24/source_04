using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using pb;
using pb.Data.Mongo;
using pb.IO;
using Print;

namespace Download.Print
{
    public class FileGroup
    {
        public EnumDirectoryInfo DirectoryInfo;
        public string SubDirectory;
        public string File;
        public string BaseFilename;
        public int Number;
        public bool BadFile;
        //public PrintInfo PrintInfo;
    }

    public class DirectoryGroup
    {
        public string BaseDirectory;
        public string Directory;
        public string SubDirectory;
    }

    public class FileGroup_v1
    {
        public DirectoryGroup DirectoryGroup;
        public string File;
        public string BaseFilename;
        public int Number;
    }

    public class PrintFileManager
    {
        private static string __badFileDirectory = "_bad";
        private bool _simulate = false;
        private bool _moveFiles = false;
        private UncompressManager _uncompressManager = null;

        public PrintFileManager(UncompressManager uncompressManager)
        {
            _uncompressManager = uncompressManager;
        }

        public bool Simulate { get { return _simulate; } set { _simulate = value; } }
        public bool MoveFiles { get { return _moveFiles; } set { _moveFiles = value; } }

        public void ManageDirectoryGroups(IEnumerable<IEnumerable<EnumDirectoryInfo>> directoryGroups, string directory)
        {
            foreach (IEnumerable<EnumDirectoryInfo> directoryGroup in directoryGroups)
            {
                ManageDirectoryGroup(directoryGroup, directory);
            }
        }

        public void ManageDirectoryGroup(IEnumerable<EnumDirectoryInfo> directoryGroup, string directory, string bonusDirectory = null)
        {
            // print example
            //   directoryGroup :
            //     - "Directory" : "g:\\pib\\media\\ebook\\_test\\print\\.02_hebdo\\L'express", "SubDirectory" : ".02_hebdo\\L'express", "Level" : 1
            //     - "Directory" : "g:\\pib\\media\\ebook\\_test\\_dl\\print\\01\\print\\.02_hebdo\\L'express", "SubDirectory" : ".02_hebdo\\L'express", "Level" : 1
            //     - "Directory" : "g:\\pib\\media\\ebook\\_test\\_dl\\print\\02\\print\\.02_hebdo\\L'express", "SubDirectory" : ".02_hebdo\\L'express", "Level" : 1
            //   directory : "g:\\pib\\media\\ebook\\_test\\print"
            // book example
            //   directoryGroup :
            //     - "Directory" : "g:\\pib\\media\\ebook\\_test\\book\\Comment faire les fromages", "SubDirectory" : "Comment faire les fromages", "Level" : 1
            //     - "Directory" : "g:\\pib\\media\\ebook\\_test\\_dl\\book\\01\\book\\Comment faire les fromages", "SubDirectory" : "Comment faire les fromages", "Level" : 1
            //     - "Directory" : "g:\\pib\\media\\ebook\\_test\\_dl\\book\\02\\book\\Comment faire les fromages", "SubDirectory" : "Comment faire les fromages", "Level" : 1
            //   directory : "g:\\pib\\media\\ebook\\_test\\book"


            // 1) delete empty directory
            if (!_simulate)
                directoryGroup.zForEach(dir => zdir.DeleteEmptyDirectory(dir.Directory, deleteOnlySubdirectory: true));

            // 2) uncompress .zip .rar (recursive)
            directoryGroup.zForEach(dir => UncompressDirectoryFiles(dir.Directory));

            // 3) pdf control

            Dictionary<string, List<FileGroup>> fileGroups;

            // 4) bonus directories
            if (bonusDirectory != null)
            {
                IEnumerable<EnumDirectoryInfo> bonusDirectoryGroup = GetBonusDirectories(bonusDirectory, directoryGroup);
                fileGroups = GetFileGroups(bonusDirectoryGroup);

                // delete duplicate files, duplicate directories
                DeleteDuplicateFiles(fileGroups.Values);

                // move and rename files
                if (_moveFiles)
                    _MoveFiles(fileGroups.Values, bonusDirectory, useSubdirectory: false);
            }

            fileGroups = GetFileGroups(directoryGroup);

            // 5) delete duplicate files, duplicate directories
            DeleteDuplicateFiles(fileGroups.Values);

            // 6) move and rename files
            if (_moveFiles && directory != null)
                _MoveFiles(fileGroups.Values, directory, useSubdirectory: true);

            // 7) delete empty directory
            if (!_simulate)
                // deleteOnlySubdirectory: true
                directoryGroup.zForEach(dir => zdir.DeleteEmptyDirectory(dir.Directory, deleteOnlySubdirectory: false));
        }

        public void ManageDirectory_v1(int level, params string[] directories)
        {
            Dictionary<string, List<DirectoryGroup>> directoryGroups = GetDirectoryGroups_v1(directories, level);
            ManageDirectory_v1(directoryGroups, directories[0]);
        }

        public void ManageDirectory_v1(Dictionary<string, List<DirectoryGroup>> directoryGroups, string directory)
        {
            //Dictionary<string, List<DirectoryGroup>> directoryGroups = GetDirectoryGroups(directories, level);

            //Trace.WriteLine(directoryGroups.zToJson());

            foreach (List<DirectoryGroup> directoryGroup in directoryGroups.Values)
            {
                // 1) delete empty directory
                if (!_simulate)
                    directoryGroup.zForEach(dir => zdir.DeleteEmptyDirectory(dir.Directory, deleteOnlySubdirectory: true));

                // 2) uncompress .zip .rar (recursive)
                directoryGroup.zForEach(dir => UncompressDirectoryFiles(dir.Directory));

                // 3) pdf control

                Dictionary<string, List<FileGroup_v1>> fileGroups = GetFileGroups_v1(directoryGroup);

                // 4) delete duplicate files, duplicate directories
                DeleteDuplicateFiles_v1(fileGroups);

                // 5) rename files
                if (_moveFiles)
                    _MoveFiles_v1(fileGroups, directory);

                // 5) delete empty directory
                if (!_simulate)
                    directoryGroup.zForEach(dir => zdir.DeleteEmptyDirectory(dir.Directory, deleteOnlySubdirectory: true));
            }

        }

        private Dictionary<string, List<DirectoryGroup>> GetDirectoryGroups_v1(string[] directories, int level)
        {
            Dictionary<string, List<DirectoryGroup>> directoryGroups = new Dictionary<string, List<DirectoryGroup>>();
            foreach (string directory in directories)
            {
                int l = directory.Length;
                var query = from dir in zdir.EnumerateDirectories(directory, minLevel: level, maxLevel: level)
                            select
                                new DirectoryGroup
                                {
                                    BaseDirectory = directory,
                                    Directory = dir,
                                    SubDirectory = dir.Substring(l + 1)
                                };
                directoryGroups.zKeyListAdd(query, dir => dir.SubDirectory);
            }
            return directoryGroups;
        }

        private void UncompressDirectoryFiles(string directory)
        {
            //DirectoryInfo directoryInfo = new DirectoryInfo(directory);
            //foreach (FileInfo fileInfo in directoryInfo.EnumerateFiles("*.*", SearchOption.AllDirectories))
            foreach (var fileInfo in zDirectory.CreateDirectoryInfo(directory).EnumerateFiles("*.*", SearchOption.AllDirectories))
            {
                if (CompressManager.IsCompressFile(fileInfo.Name))
                {
                    Trace.WriteLine("uncompress \"{0}\"", fileInfo.FullName);
                    if (!_simulate)
                        _uncompressManager.Uncompress(fileInfo.FullName);
                }
            }
        }

        public static IEnumerable<EnumDirectoryInfo> GetBonusDirectories(string bonusDirectory, IEnumerable<EnumDirectoryInfo> directoryGroup)
        {
            yield return new EnumDirectoryInfo { Directory = bonusDirectory, Level = 1 };
            foreach (EnumDirectoryInfo directory in directoryGroup)
            {
                foreach (EnumDirectoryInfo directory2 in GetBonusDirectories(directory.Directory))
                {
                    yield return directory2;
                }
            }
        }

        // bonus, bonus2, BBonus
        private static Regex __bonusDirectory = new Regex("^b?bonus[0-9]*$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        public static IEnumerable<EnumDirectoryInfo> GetBonusDirectories(string directory)
        {
            return zdir.EnumerateDirectoriesInfo(directory,
                directoryFilter: directoryInfo => __bonusDirectory.IsMatch(zPath.GetFileName(directoryInfo.SubDirectory)) ? new EnumDirectoryFilter { Select = true, RecurseSubDirectory = false } : new EnumDirectoryFilter { Select = false, RecurseSubDirectory = true });
        }

        public static Dictionary<string, List<FileGroup>> GetFileGroups(IEnumerable<EnumDirectoryInfo> directories)
        {
            Dictionary<string, List<FileGroup>> fileGroups = new Dictionary<string, List<FileGroup>>(StringComparer.InvariantCultureIgnoreCase);
            foreach (EnumDirectoryInfo directoryInfo in directories)
            {
                //Trace.WriteLine("get files from \"{0}\"", directoryInfo.Directory);
                var query = zdir.EnumerateFilesInfo(directoryInfo.Directory, followDirectoryTree: dir => { })
                .Select(
                    file =>
                    {
                        FilenameNumberInfo filenameNumberInfo = FilenameNumberInfo.GetFilenameNumberInfo(file.File);
                        string baseFilename = filenameNumberInfo.BaseFilename;
                        bool badFile = false;
                        if (file.SubDirectory == __badFileDirectory)
                        {
                            baseFilename = __badFileDirectory + "\\" + baseFilename;
                            badFile = true;
                        }
                        return new FileGroup
                        {
                            DirectoryInfo = directoryInfo,
                            SubDirectory = file.SubDirectory,
                            File = file.File,
                            BaseFilename = baseFilename,
                            Number = filenameNumberInfo.Number,
                            BadFile = badFile
                            //PrintInfo = PrintIssue.GetPrintInfo(zPath.GetFileNameWithoutExtension(filenameNumberInfo.BaseFilename))
                        };
                    }
                    );
                fileGroups.zKeyListAdd(query, fileGroup => fileGroup.BaseFilename);
            }
            return fileGroups;
        }

        private Dictionary<string, List<FileGroup_v1>> GetFileGroups_v1(IEnumerable<DirectoryGroup> directories)
        {
            Dictionary<string, List<FileGroup_v1>> fileGroups = new Dictionary<string, List<FileGroup_v1>>();
            foreach (DirectoryGroup directoryGroup in directories)
            {
                var query = zDirectory.EnumerateFiles(directoryGroup.Directory, "*.*", SearchOption.AllDirectories).Select(
                    file =>
                    {
                        //FilenameNumberInfo filenameNumberInfo = zpath.PathGetFilenameNumberInfo(file);
                        FilenameNumberInfo filenameNumberInfo = FilenameNumberInfo.GetFilenameNumberInfo(file);
                        return new FileGroup_v1
                        {
                            DirectoryGroup = directoryGroup,
                            File = file,
                            BaseFilename = filenameNumberInfo.BaseFilename,
                            Number = filenameNumberInfo.Number
                        };
                    }
                    );
                fileGroups.zKeyListAdd(query, fileGroup => fileGroup.BaseFilename);
            }
            return fileGroups;
        }

        //private void DeleteDuplicateFiles(Dictionary<string, List<FileGroup>> fileGroups)
        private void DeleteDuplicateFiles(IEnumerable<List<FileGroup>> fileGroups)
        {
            foreach (List<FileGroup> fileGroup in fileGroups)
            {
                if (fileGroup.Count > 1)
                {
                    DeleteDuplicateFiles(fileGroup);
                }
            }
        }

        private void DeleteDuplicateFiles(List<FileGroup> fileGroup)
        {
            for (int i1 = 0; i1 < fileGroup.Count - 1; )
            {
                FileGroup fileGroup1 = fileGroup[i1];
                bool file1Deleted = false;
                for (int i2 = i1 + 1; i2 < fileGroup.Count; )
                {
                    FileGroup fileGroup2 = fileGroup[i2];
                    bool file2Deleted = false;
                    if (_simulate)
                    {
                        Trace.WriteLine("compare file      \"{0}\"", fileGroup1.File);
                        Trace.WriteLine("        with      \"{0}\"", fileGroup2.File);
                    }
                    else
                    {
                        if (zfile.AreFileEqual(fileGroup1.File, fileGroup2.File))
                        {
                            string file1, file2;
                            if (fileGroup1.Number <= fileGroup2.Number)
                            {
                                file2Deleted = true;
                                file1 = fileGroup2.File;
                                file2 = fileGroup1.File;
                            }
                            else
                            {
                                file1Deleted = true;
                                file1 = fileGroup1.File;
                                file2 = fileGroup2.File;
                            }
                            Trace.WriteLine("delete file       \"{0}\"", file1);
                            Trace.WriteLine("   identical to   \"{0}\"", file2);
                            zFile.Delete(file1);
                        }
                    }
                    if (file1Deleted)
                        break;
                    if (file2Deleted)
                        fileGroup.RemoveAt(i2);
                    else
                        i2++;
                }
                if (file1Deleted)
                    fileGroup.RemoveAt(i1);
                else
                    i1++;
            }
        }

        private void DeleteDuplicateFiles_v1(Dictionary<string, List<FileGroup_v1>> fileGroups)
        {
            foreach (List<FileGroup_v1> fileGroup in fileGroups.Values)
            {
                if (fileGroup.Count > 1)
                {
                    DeleteDuplicateFiles_v1(fileGroup);
                }
            }
        }

        private void DeleteDuplicateFiles_v1(List<FileGroup_v1> fileGroup)
        {
            for (int i1 = 0; i1 < fileGroup.Count - 1; )
            {
                FileGroup_v1 fileGroup1 = fileGroup[i1];
                bool file1Deleted = false;
                for (int i2 = i1 + 1; i2 < fileGroup.Count; )
                {
                    FileGroup_v1 fileGroup2 = fileGroup[i2];
                    bool file2Deleted = false;
                    if (_simulate)
                        Trace.WriteLine("compare file \"{0}\" with \"{1}\"", fileGroup1.File, fileGroup2.File);
                    else
                    {
                        if (zfile.AreFileEqual(fileGroup1.File, fileGroup2.File))
                        {
                            //int n1 = zpath.PathGetFilenameNumber(file1);
                            //int n2 = zpath.PathGetFilenameNumber(file2);
                            string file1, file2;
                            if (fileGroup1.Number <= fileGroup2.Number)
                            {
                                file2Deleted = true;
                                file1 = fileGroup2.File;
                                file2 = fileGroup1.File;
                            }
                            else
                            {
                                file1Deleted = true;
                                file1 = fileGroup1.File;
                                file2 = fileGroup2.File;
                            }
                            Trace.WriteLine("delete file \"{0}\" identical to \"{0}\"", file1, file2);
                            zFile.Delete(file1);
                        }
                    }
                    if (file1Deleted)
                        break;
                    if (file2Deleted)
                        fileGroup.RemoveAt(i2);
                    else
                        i2++;
                }
                if (file1Deleted)
                    fileGroup.RemoveAt(i1);
                else
                    i1++;
            }
        }

        private void _MoveFiles(IEnumerable<IEnumerable<FileGroup>> fileGroups, string directory, bool useSubdirectory = true)
        {
            foreach (IEnumerable<FileGroup> fileGroup in fileGroups)
            {
                _MoveFiles(fileGroup, directory, useSubdirectory);
            }
        }

        private void _MoveFiles(IEnumerable<FileGroup> fileGroups, string directory, bool useSubdirectory = true)
        {
            // directory : g:\\pib\\media\\ebook\\print
            //Trace.WriteLine("_MoveFiles() directory \"{0}\"", directory);
            int n = 0;
            List<string> files = new List<string>();
            PrintDirectoryDateStorage directoryDateStorage = null;
            bool first = true;
            string directory2 = null;
            foreach (FileGroup fileGroup in fileGroups)
            {
                // fileGroup.DirectoryGroup.BaseDirectory
                // fileGroup.DirectoryInfo.Directory    : g:\\pib\\media\\ebook\\print\\.02_hebdo\\01 net
                //                                        g:\\pib\\media\\ebook\\_dl\\_dl_pib\\01\\print\\.02_hebdo\\01 net
                // fileGroup.DirectoryInfo.SubDirectory : .02_hebdo\\01 net
                // fileGroup.SubDirectory               : 2014
                // fileGroup.File                       : g:\\pib\\media\\ebook\\print\\.02_hebdo\\01 net\\2014\\01 net - 2014-10-16 - no 806(1).pdf
                // fileGroup.BaseFilename               : 01 net - 2014-10-16 - no 806.pdf
                // fileGroup.Number                     : 1

                if (first)
                {
                    first = false;
                    //Trace.WriteLine("_MoveFiles() SubDirectory \"{0}\"", fileGroup.DirectoryInfo.SubDirectory);
                    directory2 = directory;
                    if (fileGroup.DirectoryInfo.SubDirectory != null && useSubdirectory)
                        directory2 = zPath.Combine(directory, fileGroup.DirectoryInfo.SubDirectory);
                    //Trace.WriteLine("_MoveFiles() directory \"{0}\"", directory2);
                    directoryDateStorage = PrintDirectoryManager.GetDirectoryDateStorage(directory2);
                }

                string file = directory2;
                string filename = zPath.GetFileNameWithoutExtension(fileGroup.BaseFilename);
                if (!fileGroup.BadFile)
                {
                    PrintInfo printInfo = PrintIssue.GetPrintInfo(filename);
                    //Trace.WriteLine("_MoveFiles() filename \"{0}\" date \"{1}\"", filename, printInfo != null ? printInfo.Date.ToString() : "null");
                    if (printInfo != null && printInfo.Date != null)
                    {
                        string subDirectory = directoryDateStorage.GetDirectory((Date)printInfo.Date);
                        if (subDirectory != null)
                            file = zPath.Combine(file, subDirectory);
                    }
                    else if (fileGroup.File.StartsWith(directory))
                    {
                        // dont move unknow file of destination directory
                        if (n <= fileGroup.Number)
                            n = fileGroup.Number + 1;
                        continue;
                        // pas de continue sinon n n'est pas incrémenté
                        // keep directory file
                        //file = fileGroup.DirectoryInfo.Directory;
                    }
                }
                else
                    file = zPath.Combine(file, __badFileDirectory);
                file = zPath.Combine(file, filename);
                if (n > 0)
                    file += string.Format("[{0}]", n);
                file += zPath.GetExtension(fileGroup.BaseFilename);
                if (fileGroup.File != file)
                {
                    files.Add(file);
                    Trace.WriteLine("move file         \"{0}\"", fileGroup.File);
                    Trace.WriteLine("       to         \"{0}\"", file + ".tmp");
                    if (!_simulate)
                    {
                        zfile.CreateFileDirectory(file);
                        zFile.Move(fileGroup.File, file + ".tmp");
                    }
                }
                //if (fileGroup.Number != n || fileGroup.DirectoryInfo.Directory != directory)
                //{
                //    string file = zPath.Combine(directory, fileGroup.DirectoryInfo.SubDirectory, zPath.GetFileNameWithoutExtension(fileGroup.BaseFilename));
                //    if (n > 0)
                //        file += string.Format("[{0}]", n);
                //    file += zPath.GetExtension(fileGroup.BaseFilename);
                //    files.Add(file);
                //    Trace.WriteLine("move file \"{0}\" to \"{1}\"", fileGroup.File, file + ".tmp");
                //    if (!_simulate)
                //    {
                //        zfile.CreateFileDirectory(file);
                //        File.Move(fileGroup.File, file + ".tmp");
                //    }
                //}
                n++;
            }

            foreach (string file in files)
            {
                Trace.WriteLine("rename tmp file   \"{0}\"", file + ".tmp");
                Trace.WriteLine("             to   \"{0}\"", file);
                if (!_simulate)
                    zFile.Move(file + ".tmp", file);
            }
        }

        private void _MoveFiles_v1(Dictionary<string, List<FileGroup_v1>> fileGroups, string directory)
        {
            foreach (List<FileGroup_v1> fileGroup in fileGroups.Values)
            {
                _MoveFiles_v1(fileGroup, directory);
            }
        }

        private void _MoveFiles_v1(List<FileGroup_v1> fileGroups, string directory)
        {
            int n = 0;
            List<string> files = new List<string>();
            foreach (FileGroup_v1 fileGroup in fileGroups)
            {
                if (fileGroup.Number != n || fileGroup.DirectoryGroup.BaseDirectory != directory)
                {
                    //string extension = zPath.GetExtension(fileGroup.BaseFilename);
                    string file = zPath.Combine(directory, fileGroup.DirectoryGroup.SubDirectory, zPath.GetFileNameWithoutExtension(fileGroup.BaseFilename));
                    if (n > 0)
                        file += string.Format("[{0}]", n);
                    file += zPath.GetExtension(fileGroup.BaseFilename);
                    files.Add(file);
                    Trace.WriteLine("move file \"{0}\" to \"{1}\"", fileGroup.File, file + ".tmp");
                    if (!_simulate)
                    {
                        zfile.CreateFileDirectory(file);
                        zFile.Move(fileGroup.File, file + ".tmp");
                    }
                }
                n++;
            }

            foreach (string file in files)
            {
                Trace.WriteLine("move file \"{0}\" to \"{1}\"", file + ".tmp", file);
                if (!_simulate)
                    zFile.Move(file + ".tmp", file);
            }
        }

        //public void ManageDirectory_old(int level, params string[] directories)
        //{
        //    Dictionary<string, List<string>> directoryGroups = new Dictionary<string, List<string>>();
        //    foreach (string directory in directories)
        //    {
        //        directoryGroups.zAddKeyList(zdir.EnumerateDirectories(directory, minLevel: level, maxLevel: level), dir => zPath.GetFileName(dir));
        //    }

        //    //Trace.WriteLine(directoryGroups.zToJson());

        //    foreach (List<string> directoryGroup in directoryGroups.Values)
        //    {
        //        // 1) delete empty directory
        //        directoryGroup.zForEach(directory => zdir.DeleteEmptyDirectory(directory, deleteOnlySubdirectory: true));

        //        // 2) uncompress .zip .rar (recursive)
        //        directoryGroup.zForEach(directory => UncompressDirectoryFiles(directory));

        //        // 3) pdf control

        //        Dictionary<string, List<string>> fileGroups = GetFileGroups(directories);

        //        // 4) delete duplicate files, duplicate directories
        //        DeleteDuplicateFiles(fileGroups);

        //        // 5) rename files
        //        RenameFiles(fileGroups);

        //        // 5) delete empty directory
        //        directoryGroup.zForEach(directory => zdir.DeleteEmptyDirectory(directory, deleteOnlySubdirectory: true));
        //    }
        //}

        //public void ManageDirectory(string directory, int level)
        //{
        //    foreach (string directory2 in zdir.EnumerateDirectories(directory, minLevel: level, maxLevel: level))
        //    {
        //        ManageDirectory(directory2);
        //    }
        //}

        //private void ManageDirectory(string directory)
        //{
        //    // manage print directory ("c:\pib\_dl\_pib\dl\rapide-ddl.com\print\.01_quotidien\Journaux")
        //    // 1) delete empty directory
        //    // 2) uncompress .zip .rar (recursive)
        //    // 3) pdf control
        //    // 4) delete duplicate files, duplicate directories
        //    // 5) delete empty directory

        //    // 1) delete empty directory
        //    if (zdir.DeleteEmptyDirectory(directory, deleteOnlySubdirectory: false))
        //        return;

        //    // 2) uncompress .zip .rar (recursive)
        //    UncompressDirectoryFiles(directory);

        //    // 3) pdf control

        //    // 4) delete duplicate files, duplicate directories
        //    DeleteDuplicateDirectoryFiles(directory);

        //    // 5) delete empty directory
        //    zdir.DeleteEmptyDirectory(directory, deleteOnlySubdirectory: false);
        //}
    }
}
