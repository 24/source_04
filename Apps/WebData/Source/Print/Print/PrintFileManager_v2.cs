using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using pb;
using pb.IO;
using pb.Text;

// todo :
//  - concat .i files when delete pdf ($$todo)
namespace Download.Print
{
    public enum FileFilter
    {
        AllFiles = 0,
        BonusFiles,
        NotBonusFiles
    }

    public class FileGroup_v2
    {
        public string Directory;
        public string SubDirectory;
        public string File;
        public string BaseFilename;
        public int Number;
        public bool BadFile;
        //public bool SourceDirectoryFile;
        public bool DestinationDirectoryFile;
        public bool Ok;
    }

    public class BonusDirectoryInfo
    {
        public bool IsBonusDirectory;
        public string Directory;
    }

    public class DailyPrintFile
    {
        public string File;
        public Date Date;
        public string Directory;
    }

    //public class RenamePrintFile1
    //{
    //    public bool RenameFile;
    //    public string Title;
    //    public Date ExpectedDate;
    //    public string File;
    //    public string Error;
    //    public FindPrintInfo FindPrint;
    //}

    public class RenamePrintFile
    {
        public bool RenameFile;
        public Date ExpectedDate;
        public string File;
        public string Title;
        public string SourceFile;
        public string DestinationFile;
        //public bool FindPrint;

        public string FormatedTitle;
        public Date? Date;
        public DateType DateType = DateType.Unknow;
        public string Name;
        //public string Title;
        //public string Directory;
        public int? Number;
        public bool Special = false;
        public string SpecialText;
        public string Label;
        public string RemainText;
        public string PrintName;
        public string Error;
    }

    public class PrintFileManager_v2
    {
        private static string __badFileDirectory = "_bad";
        private bool _simulate = false;
        private bool _moveFiles = false;
        private bool _moveInfoFiles = false;
        private UncompressQueueManager _uncompressManager = null;
        private RegexValuesList _bonusDirectories = null;
        private MovePrintFiles _movePrintFiles = null;

        public PrintFileManager_v2()
        {
            PathPrintFile pathPrintFile = new PathPrintFile();
            pathPrintFile.BadFileDirectory = __badFileDirectory;
            _movePrintFiles = new MovePrintFiles();
            _movePrintFiles.PathPrintFile = pathPrintFile;
        }

        //public PrintFileManager_v2(UncompressManager uncompressManager)
        //{
        //    _uncompressManager = uncompressManager;
        //}

        public bool Simulate { get { return _simulate; } set { _simulate = value; } }
        public bool MoveFiles { get { return _moveFiles; } set { _moveFiles = value; } }
        public bool MoveInfoFiles { get { return _moveInfoFiles; } set { _moveInfoFiles = value; } }
        public UncompressQueueManager UncompressManager { get { return _uncompressManager; } set { _uncompressManager = value; } }
        public RegexValuesList BonusDirectories { get { return _bonusDirectories; } set { _bonusDirectories = value; } }

        // book example :
        //   sourceDirectory      : g:\pib\media\ebook\_dl\_dl_pib\book\10\book\Les météores
        //   destinationDirectory : g:\pib\media\ebook\book\unsorted_verified\Les météores
        //   bonusDirectory       : g:\pib\media\ebook\book\bonus
        public void ManageDirectory(string sourceDirectory, string destinationDirectory, string bonusDirectory = null)
        {
            // 1) delete empty directory
            if (!_simulate)
                //zdir.DeleteEmptyDirectory(sourceDirectory, deleteOnlySubdirectory: true);
                zdir.DeleteEmptyDirectory(sourceDirectory, recurse: false);

            // 2) uncompress .zip .rar (recursive)
            if (!_simulate)
                UncompressDirectoryFiles(sourceDirectory);

            // 3) pdf control
            // todo

            Dictionary<string, List<FileGroup_v2>> filesGroups;

            // 4) bonus directories
            if (bonusDirectory != null)
            {
                //IEnumerable<EnumDirectoryInfo> bonusDirectoryGroup = GetBonusDirectories(bonusDirectory, directoryGroup);
                //IEnumerable<EnumDirectoryInfo> directories = new EnumDirectoryInfo[] { new EnumDirectoryInfo { Directory = bonusDirectory } }.Concat(GetBonusDirectories(sourceDirectory));
                //fileGroups = GetFileGroups(directories);
                filesGroups = CreateFileGroups();
                //filesGroups.zAddSourceDirectoryFiles(GetBonusFiles(sourceDirectory));
                filesGroups.zAddSourceDirectoryFiles(GetFiles(sourceDirectory, FileFilter.BonusFiles));
                //filesGroups.zAddDestinationDirectoryFiles(GetNotBonusFiles(bonusDirectory));
                filesGroups.zAddDestinationDirectoryFiles(GetFiles(bonusDirectory, FileFilter.AllFiles));

                //"bonus files".zTrace();
                //filesGroups.zToJson().zTrace();
                //"".zTrace();

                // delete duplicate files, duplicate directories
                DeleteDuplicateFiles(filesGroups.Values);

                // move and rename files
                if (_moveFiles)
                    //_MoveFiles_v1(filesGroups.Values, bonusDirectory);
                    _movePrintFiles.MoveFiles(filesGroups.Values, bonusDirectory, _simulate, _moveInfoFiles);
            }

            filesGroups = CreateFileGroups();
            //filesGroups.zAddSourceDirectoryFiles(GetNotBonusFiles(sourceDirectory));
            filesGroups.zAddSourceDirectoryFiles(GetFiles(sourceDirectory, FileFilter.NotBonusFiles));
            //filesGroups.zAddDestinationDirectoryFiles(GetNotBonusFiles(destinationDirectory));
            filesGroups.zAddDestinationDirectoryFiles(GetFiles(destinationDirectory, FileFilter.AllFiles));

            //"not bonus files".zTrace();
            //filesGroups.zToJson().zTrace();
            //"".zTrace();

            // 5) delete duplicate files, duplicate directories
            DeleteDuplicateFiles(filesGroups.Values);

            // 6) move and rename files
            if (_moveFiles)
                //_MoveFiles_v1(filesGroups.Values, destinationDirectory);
                _movePrintFiles.MoveFiles(filesGroups.Values, destinationDirectory, _simulate, _moveInfoFiles);

            // 7) delete empty directory
            if (!_simulate)
            {
                //zdir.DeleteEmptyDirectory(zPath.Combine(sourceDirectory, InfoFile.InfoDirectory) , deleteOnlySubdirectory: false);
                zdir.DeleteEmptyDirectory(zPath.Combine(sourceDirectory, InfoFile.InfoDirectory), recurse: true);
                //zdir.DeleteEmptyDirectory(sourceDirectory, deleteOnlySubdirectory: false);
                zdir.DeleteEmptyDirectory(sourceDirectory, recurse: true);
            }
        }

        private static Regex __dailyPrintDirectory = new Regex("Journaux - ([0-9]{4})-([0-9]{2})-([0-9]{2})", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static IEnumerable<DailyPrintFile> GetDailyPrintFiles(string sourceDirectory)
        {
            Date date = Date.MinValue;
            Func<EnumDirectoryInfo, EnumDirectoryFilter> directoryFilter = dir =>
            {
                Match match = __dailyPrintDirectory.Match(FilenameNumberInfo.GetFilenameWithoutNumber(zPath.GetFileName(dir.SubDirectory)));
                if (match.Success)
                {
                    int year = int.Parse(match.Groups[1].Value);
                    int month = int.Parse(match.Groups[2].Value);
                    int day = int.Parse(match.Groups[3].Value);
                    if (zdate.IsDateValid(year, month, day))
                    {
                        date = new Date(year, month, day);
                        return new EnumDirectoryFilter { Select = true, RecurseSubDirectory = false };
                    }

                }
                return new EnumDirectoryFilter { Select = false, RecurseSubDirectory = true };
            };
            foreach (EnumDirectoryInfo dir in zdir.EnumerateDirectoriesInfo(sourceDirectory, directoryFilters: new Func<EnumDirectoryInfo, EnumDirectoryFilter>[] { directoryFilter }))
            {
                foreach (EnumFileInfo fileInfo in zdir.EnumerateFilesInfo(dir.Directory))
                {
                    yield return new DailyPrintFile { File = fileInfo.File, Date = date, Directory = dir.Directory };
                }
            }
        }

        //public static IEnumerable<RenamePrintFile> RenameDailyPrintFiles(IEnumerable<DailyPrintFile> files, FindPrintManager findPrintManager, string destinationDirectory, bool simulate = true)
        public static IEnumerable<RenamePrintFile> GetRenameDailyPrintFilesInfos(IEnumerable<DailyPrintFile> files, FindPrintManager findPrintManager)
        {
            //string lastDirectory = null;
            foreach (DailyPrintFile file in files)
            {
                //if (file.Directory != lastDirectory)
                //{
                //    // remove empty directories
                //    if (!simulate && lastDirectory != null)
                //        zdir.DeleteEmptyDirectory(lastDirectory, deleteOnlySubdirectory: false);
                //    lastDirectory = file.Directory;
                //}
                //yield return RenamePrintFileI(findPrintManager, file.File, file.Date, destinationDirectory, simulate);
                yield return GetRenamePrintFileInfo(findPrintManager, file.File, file.Date);
            }

            // remove empty directories
            //if (!simulate && lastDirectory != null)
            //    zdir.DeleteEmptyDirectory(lastDirectory, deleteOnlySubdirectory: false);
        }

        //public static void RenameDailyPrintFiles_old(FindPrintManager findPrintManager, string sourceDirectory, string destinationDirectory, bool simulate = true)
        //{
        //    Date date = Date.MinValue;
        //    Func<EnumDirectoryInfo, EnumDirectoryFilter> directoryFilter = dir =>
        //        {
        //            Match match = __dailyPrintDirectory.Match(FilenameNumberInfo.GetFilenameWithoutNumber(zPath.GetFileName(dir.SubDirectory)));
        //            if (match.Success)
        //            {
        //                int year = int.Parse(match.Groups[1].Value);
        //                int month = int.Parse(match.Groups[2].Value);
        //                int day = int.Parse(match.Groups[3].Value);
        //                if (zdate.IsDateValid(year, month, day))
        //                {
        //                    date = new Date(year, month, day);
        //                    return new EnumDirectoryFilter { Select = true, RecurseSubDirectory = false };
        //                }

        //            }
        //            return new EnumDirectoryFilter { Select = false, RecurseSubDirectory = true };
        //        };
        //    foreach (EnumDirectoryInfo dir in zdir.EnumerateDirectoriesInfo(sourceDirectory, directoryFilter: directoryFilter))
        //    {
        //        RenamePrintFiles_old(findPrintManager, dir.Directory, date, destinationDirectory, simulate);
        //        //Trace.WriteLine("rename print files from \"{0}\" {1}", dir.Directory, date);
        //    }
        //}

        //private static void RenamePrintFiles_old(FindPrintManager findPrintManager, string sourceDirectory, Date date, string destinationDirectory, bool simulate)
        //{
        //    Trace.WriteLine("rename print files from \"{0}\"", sourceDirectory);
        //    foreach (EnumFileInfo fileInfo in zdir.EnumerateFilesInfo(sourceDirectory))
        //    {
        //        RenamePrintFile(findPrintManager, fileInfo.File, date, destinationDirectory, simulate);
        //    }
        //    Trace.WriteLine();

        //    // remove empty directories
        //    if (!simulate)
        //        zdir.DeleteEmptyDirectory(sourceDirectory, deleteOnlySubdirectory: false);
        //}

        public static string RenamePrintFile(string sourceFile, string destinationFile)
        {
            string destinationFile2 = null;
            try
            {
                //newfile = zPath.Combine(destinationDirectory, renamePrintFile.File);
                destinationFile2 = zfile.GetNewFilename(destinationFile);
                zfile.CreateFileDirectory(destinationFile2);
                //zFile.Move(file, newfile);
                zFile.Move(sourceFile, destinationFile2);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("  error print \"{0}\"", sourceFile);
                Trace.WriteLine(Error.GetErrorMessage(ex, true, false));
                Trace.WriteLine();
            }
            return destinationFile2;
        }

        //private static RenamePrintFile RenamePrintFile(FindPrintManager findPrintManager, string file, Date date, string destinationDirectory, bool simulate)
        private static RenamePrintFile GetRenamePrintFileInfo(FindPrintManager findPrintManager, string file, Date date)
        {
            RenamePrintFile renamePrintFile = new RenamePrintFile();
            //try
            //{
                renamePrintFile.SourceFile = file;
                renamePrintFile.Title = zPath.GetFileNameWithoutExtension(file);
                renamePrintFile.ExpectedDate = date;
                FindPrintInfo findPrint = findPrintManager.Find(renamePrintFile.Title, PrintType.Print, expectedDate: date);
                if (findPrint.DayTitle != null)
                    renamePrintFile.FormatedTitle = findPrint.DayTitle;
                else
                    renamePrintFile.FormatedTitle = findPrint.titleInfo.FormatedTitle;
                renamePrintFile.Name = findPrint.name;
                renamePrintFile.Date = findPrint.date;
                renamePrintFile.DateType = findPrint.dateType;
                renamePrintFile.Number = findPrint.number;
                renamePrintFile.Special = findPrint.special;
                renamePrintFile.SpecialText = findPrint.specialText;
                renamePrintFile.Label = findPrint.label;
                renamePrintFile.RemainText = findPrint.remainText;
                if (findPrint.print != null)
                    renamePrintFile.PrintName = findPrint.print.Name;

                if (!findPrint.found)
                {
                    renamePrintFile.Error = string.Format("unknow print \"{0}\" date {1} formated title \"{2}\"", zPath.GetFileName(file), findPrint.date, findPrint.titleInfo.FormatedTitle);
                }
                else if (findPrint.date == null)
                {
                    renamePrintFile.Error = string.Format("date not found \"{0}\"", zPath.GetFileName(file));
                }

                if (findPrint.found && findPrint.date != null)
                {
                    int dateGap = findPrint.date.Value.Subtract(date).Days;
                    if (dateGap < -findPrintManager.GapDayBefore || dateGap > findPrintManager.GapDayAfter)
                    {
                        renamePrintFile.Error = string.Format("wrong date found {0} - \"{1}\"", findPrint.date, renamePrintFile.Title);
                    }
                    else
                    {
                        renamePrintFile.RenameFile = true;
                        renamePrintFile.File = findPrint.file + zPath.GetExtension(file);
                        //string newfile = zPath.Combine(destinationDirectory, findPrint.file) + zPath.GetExtension(file);
                        //newfile = zfile.GetNewFilename(newfile);
                        //if (!simulate)
                        //{
                        //    zfile.CreateFileDirectory(newfile);
                        //    zFile.Move(file, newfile);
                        //}
                    }
                }
            //}
            //catch (Exception ex)
            //{
            //    Trace.WriteLine("  error print \"{0}\"", zPath.GetFileName(file));
            //    Trace.WriteLine(Error.GetErrorMessage(ex, true, false));
            //    Trace.WriteLine();
            //}
            return renamePrintFile;
        }

        private static Dictionary<string, List<FileGroup_v2>> CreateFileGroups()
        {
            return new Dictionary<string, List<FileGroup_v2>>(StringComparer.InvariantCultureIgnoreCase);
        }

        //public static void zAddSourceDirectoryFiles(this Dictionary<string, List<FileGroup_v2>> fileGroups, string directory)
        //{
        //    fileGroups.zKeyListAdd(GetFiles(directory).Select(fileGroup => { fileGroup.SourceDirectoryFile = true; return fileGroup; }), fileGroup => fileGroup.BaseFilename);
        //}

        public IEnumerable<FileGroup_v2> GetNotBonusFiles(string directory)
        {
            Func<EnumDirectoryInfo, EnumDirectoryFilter> directoryFilter =
                dir =>
                {
                    if (GetBonusDirectoryInfo(dir.SubDirectory).IsBonusDirectory)
                        return new EnumDirectoryFilter { Select = false, RecurseSubDirectory = false };
                    else
                        return new EnumDirectoryFilter { Select = true, RecurseSubDirectory = true };
                };

            IEnumerable< EnumFileInfo> files = zdir.EnumerateFilesInfo(directory, directoryFilters: new Func<EnumDirectoryInfo, EnumDirectoryFilter>[] { directoryFilter });
            foreach (EnumFileInfo file in files)
            {
                FilenameNumberInfo filenameNumberInfo = FilenameNumberInfo.GetFilenameNumberInfo(file.File);
                string baseFilename = filenameNumberInfo.BaseFilename;
                bool badFile = false;
                if (file.SubDirectory == __badFileDirectory)
                {
                    baseFilename = __badFileDirectory + "\\" + baseFilename;
                    badFile = true;
                }
                yield return new FileGroup_v2
                {
                    Directory = directory,
                    SubDirectory = file.SubDirectory,
                    File = file.File,
                    BaseFilename = baseFilename,
                    Number = filenameNumberInfo.Number,
                    BadFile = badFile
                    //PrintInfo = PrintIssue.GetPrintInfo(zPath.GetFileNameWithoutExtension(filenameNumberInfo.BaseFilename))
                };
            }
        }

        public IEnumerable<FileGroup_v2> GetBonusFiles(string directory)
        {
            int bonusDirectoryLevel = 0;
            int bonusSubDirectoryLength = 0;
            string bonusSubDirectory = null;

            Action<EnumDirectoryInfo> followDirectoryTree =
                dir =>
                {
                    if (bonusDirectoryLevel == 0)
                    {
                        //if (dir.SubDirectory != null && IsBonusDirectory(dir.SubDirectory))
                        if (dir.SubDirectory != null)
                        {
                            BonusDirectoryInfo bonusDirectoryInfo = GetBonusDirectoryInfo(dir.SubDirectory);
                            if (bonusDirectoryInfo.IsBonusDirectory)
                            {
                                bonusDirectoryLevel = dir.Level;
                                bonusSubDirectoryLength = dir.SubDirectory.Length + 1;
                                bonusSubDirectory = bonusDirectoryInfo.Directory;
                            }
                        }
                    }
                    else if (dir.SubDirectory == null && dir.Level == bonusDirectoryLevel)
                    {
                        bonusDirectoryLevel = 0;
                        bonusSubDirectoryLength = 0;
                        bonusSubDirectory = null;
                    }
                };

            IEnumerable<EnumFileInfo> files = zdir.EnumerateFilesInfo(directory, followDirectoryTrees: new Action<EnumDirectoryInfo>[] { followDirectoryTree });
            foreach (EnumFileInfo file in files)
            {
                if (bonusDirectoryLevel != 0)
                {
                    FilenameNumberInfo filenameNumberInfo = FilenameNumberInfo.GetFilenameNumberInfo(file.File);
                    string subDirectory = file.SubDirectory.Length > bonusSubDirectoryLength ? file.SubDirectory.Substring(bonusSubDirectoryLength) : null;
                    if (bonusSubDirectory != null)
                    {
                        if (subDirectory != null)
                            subDirectory = zPath.Combine(bonusSubDirectory, subDirectory);
                        else
                            subDirectory = bonusSubDirectory;
                    }
                    yield return new FileGroup_v2
                    {
                        Directory = directory,
                        SubDirectory = subDirectory,
                        File = file.File,
                        BaseFilename = filenameNumberInfo.BaseFilename,
                        Number = filenameNumberInfo.Number
                        //PrintInfo = PrintIssue.GetPrintInfo(zPath.GetFileNameWithoutExtension(filenameNumberInfo.BaseFilename))
                    };
                }
            }
        }

        public EnumDirectoryFilter InfoDirectoryFilter(EnumDirectoryInfo directory)
        {
            if (directory.SubDirectory == InfoFile.InfoDirectory)
                return new EnumDirectoryFilter { Select = false, RecurseSubDirectory = false };
            else
                return new EnumDirectoryFilter { Select = true, RecurseSubDirectory = true };
        }

        public EnumDirectoryFilter NotBonusDirectoryFilter(EnumDirectoryInfo directory)
        {
            if (GetBonusDirectoryInfo(directory.SubDirectory).IsBonusDirectory)
                return new EnumDirectoryFilter { Select = false, RecurseSubDirectory = false };
            else
                return new EnumDirectoryFilter { Select = true, RecurseSubDirectory = true };
        }

        public IEnumerable<FileGroup_v2> GetFiles(string directory, FileFilter fileFilter)
        {
            // not bonus files
            List<Func<EnumDirectoryInfo, EnumDirectoryFilter>> directoryFilters = new List<Func<EnumDirectoryInfo, EnumDirectoryFilter>>();
            directoryFilters.Add(InfoDirectoryFilter);
            if (fileFilter == FileFilter.NotBonusFiles)
                directoryFilters.Add(NotBonusDirectoryFilter);

            //if (fileFilter == FileFilter.NotBonusFiles)
            //{
            //    // skip bonus directories
            //    directoryFilter = dir =>
            //        {
            //            if (GetBonusDirectoryInfo(dir.SubDirectory).IsBonusDirectory)
            //                return new EnumDirectoryFilter { Select = false, RecurseSubDirectory = false };
            //            else
            //                return new EnumDirectoryFilter { Select = true, RecurseSubDirectory = true };
            //        };
            //}

            // bonus files
            int bonusDirectoryLevel = 0;
            int bonusSubDirectoryLength = 0;
            string bonusSubDirectory = null;
            Action<EnumDirectoryInfo> followDirectoryTree = null;
            if (fileFilter == FileFilter.BonusFiles)
            {
                followDirectoryTree = dir =>
                    {
                        if (bonusDirectoryLevel == 0)
                        {
                            if (dir.SubDirectory != null)
                            {
                                BonusDirectoryInfo bonusDirectoryInfo = GetBonusDirectoryInfo(dir.SubDirectory);
                                if (bonusDirectoryInfo.IsBonusDirectory)
                                {
                                    bonusDirectoryLevel = dir.Level;
                                    bonusSubDirectoryLength = dir.SubDirectory.Length + 1;
                                    bonusSubDirectory = bonusDirectoryInfo.Directory;
                                }
                            }
                        }
                        else if (dir.SubDirectory == null && dir.Level == bonusDirectoryLevel)
                        {
                            bonusDirectoryLevel = 0;
                            bonusSubDirectoryLength = 0;
                            bonusSubDirectory = null;
                        }
                    };
            }

            IEnumerable<EnumFileInfo> files = zdir.EnumerateFilesInfo(directory,
                //directoryFilters: directoryFilter != null ? new Func<EnumDirectoryInfo, EnumDirectoryFilter>[] { directoryFilter } : null,
                directoryFilters: directoryFilters,
                followDirectoryTrees: followDirectoryTree != null ? new Action<EnumDirectoryInfo>[] { followDirectoryTree } : null);
            foreach (EnumFileInfo file in files)
            {
                FilenameNumberInfo filenameNumberInfo = FilenameNumberInfo.GetFilenameNumberInfo(file.File);
                string baseFilename = filenameNumberInfo.BaseFilename;
                bool badFile = false;
                if (file.SubDirectory == __badFileDirectory)
                {
                    baseFilename = __badFileDirectory + "\\" + baseFilename;
                    badFile = true;
                }
                string subDirectory = file.SubDirectory;
                if (fileFilter == FileFilter.BonusFiles)
                {
                    if (bonusDirectoryLevel == 0)
                        continue;
                    subDirectory = file.SubDirectory.Length > bonusSubDirectoryLength ? file.SubDirectory.Substring(bonusSubDirectoryLength) : null;
                    if (bonusSubDirectory != null)
                    {
                        if (subDirectory != null)
                            subDirectory = zPath.Combine(bonusSubDirectory, subDirectory);
                        else
                            subDirectory = bonusSubDirectory;
                    }
                }
                //switch (fileFilter)
                //{
                //    case FileFilter.NotBonusFiles:
                //        break;
                //    case FileFilter.BonusFiles:
                //        break;
                //    case FileFilter.AllFiles:
                //        break;
                //}
                yield return new FileGroup_v2
                {
                    Directory = directory,
                    SubDirectory = subDirectory,
                    File = file.File,
                    BaseFilename = baseFilename,
                    Number = filenameNumberInfo.Number,
                    BadFile = badFile
                    //PrintInfo = PrintIssue.GetPrintInfo(zPath.GetFileNameWithoutExtension(filenameNumberInfo.BaseFilename))
                };
            }
        }

        // bonus, bonus2, BBonus
        private static Regex __bonusDirectory = new Regex("^b?bonus[0-9]*$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        private static bool IsBonusDirectory(string directory)
        {
            return __bonusDirectory.IsMatch(zPath.GetFileName(directory));
        }

        private BonusDirectoryInfo GetBonusDirectoryInfo(string directory)
        {
            FindText findText = _bonusDirectories.Find(zPath.GetFileName(directory));
            if (findText.Found)
            {
                string bonusDirectory = findText.matchValues.GetAttribute("directory");
                return new BonusDirectoryInfo { IsBonusDirectory = true, Directory = bonusDirectory };
            }
            else
                return new BonusDirectoryInfo { IsBonusDirectory = false };
        }

        private void UncompressDirectoryFiles(string directory)
        {
            if (_simulate)
                return;
            _uncompressManager.UncompressDirectoryFiles(directory, file => { Trace.WriteLine("uncompress \"{0}\"", file); });
        }

        private void DeleteDuplicateFiles(IEnumerable<List<FileGroup_v2>> fileGroups)
        {
            foreach (List<FileGroup_v2> fileGroup in fileGroups)
            {
                if (fileGroup.Count > 1)
                {
                    DeleteDuplicateFiles(fileGroup);
                }
            }
        }

        private void DeleteDuplicateFiles(List<FileGroup_v2> files)
        {
            for (int i1 = 0; i1 < files.Count - 1;)
            {
                FileGroup_v2 file1 = files[i1];
                bool deleteFile1 = false;
                for (int i2 = i1 + 1; i2 < files.Count;)
                {
                    FileGroup_v2 file2 = files[i2];
                    bool deleteFile2 = false;
                    if (_simulate)
                    {
                        Trace.WriteLine("compare file           \"{0}\"", file1.File);
                        //Trace.WriteLine("        with           \"{0}\"", file2.File);
                        Trace.WriteLine("  with                 \"{0}\"", file2.File);
                        if (_moveInfoFiles)
                            Trace.WriteLine("  concat info files");
                    }
                    else
                    {
                        if (zfile.AreFileEqual(file1.File, file2.File))
                        {
                            if (!file1.DestinationDirectoryFile && file2.DestinationDirectoryFile)
                                deleteFile1 = true;
                            else if (file1.DestinationDirectoryFile && !file2.DestinationDirectoryFile)
                                deleteFile2 = true;
                            else if (file1.Number <= file2.Number)
                            {
                                deleteFile2 = true;
                                //pathFile1 = file2.File;
                                //pathFile2 = file1.File;
                            }
                            else
                            {
                                deleteFile1 = true;
                                //pathFile1 = file1.File;
                                //pathFile2 = file2.File;
                            }
                            string pathFile1, pathFile2;
                            if (deleteFile1)
                            {
                                pathFile1 = file1.File;
                                pathFile2 = file2.File;
                            }
                            else
                            {
                                pathFile1 = file2.File;
                                pathFile2 = file1.File;
                            }
                            Trace.WriteLine("delete file            \"{0}\"", pathFile1);
                            Trace.WriteLine("  identical to         \"{0}\"", pathFile2);
                            zFile.Delete(pathFile1);
                            if (_moveInfoFiles)
                            {
                                string pathInfoFile1 = InfoFile.GetInfoFile(pathFile1);
                                string pathInfoFile2 = InfoFile.GetInfoFile(pathFile2);
                                if (zFile.Exists(pathInfoFile1))
                                {
                                    if (zFile.Exists(pathInfoFile2))
                                        Trace.WriteLine("  append info file     \"{0}\" to \"{0}\"", pathInfoFile1, pathInfoFile2);
                                    else
                                        Trace.WriteLine("  move info file       \"{0}\" to \"{0}\"", pathInfoFile1, pathInfoFile2);
                                }
                                else
                                {
                                    if (zFile.Exists(pathInfoFile2))
                                        Trace.WriteLine("  no info file to append");
                                    else
                                        Trace.WriteLine("  no info file");
                                }
                                // append pathInfoFile1 to pathInfoFile2
                                InfoFile.ConcatFiles(pathInfoFile2, pathInfoFile1);
                            }
                        }
                    }
                    if (deleteFile1)
                        break;
                    if (deleteFile2)
                        files.RemoveAt(i2);
                    else
                        i2++;
                }
                if (deleteFile1)
                    files.RemoveAt(i1);
                else
                    i1++;
            }
        }

        private void _MoveFiles_v1(IEnumerable<IEnumerable<FileGroup_v2>> filesGroups, string destinationDirectory)
        {
            foreach (IEnumerable<FileGroup_v2> files in filesGroups)
            {
                _MoveFiles_v1(files, destinationDirectory);
            }
        }

        // book example :
        //   destinationDirectory : g:\pib\media\ebook\book\unsorted_verified\Les météores
        //                        : g:\pib\media\ebook\book\bonus
        private void _MoveFiles_v1(IEnumerable<FileGroup_v2> files, string destinationDirectory)
        {
            // directory : g:\\pib\\media\\ebook\\print
            //Trace.WriteLine("_MoveFiles() directory \"{0}\"", directory);
            int n = 0;
            //Dictionary<int, int> fileNumbers = GetDestinationFileNumbers(files);
            List<string> moveFiles = new List<string>();
            PrintDirectoryDateStorage directoryDateStorage = null;
            bool first = true;
            string directory2 = null;
            foreach (FileGroup_v2 file in files)
            {
                // file : (book)
                //   Directory                : "g:\pib\media\ebook\_dl\_dl_pib\book\10\book\Les fées cuisinent et les lutins dînent"
                //   SubDirectory             : "Les fées cuisinent et les lutins dînent"
                //   File                     : "g:\pib\media\ebook\_dl\_dl_pib\book\10\book\Les fées cuisinent et les lutins dînent\Les fées cuisinent et les lutins dînent\Fees-Lutins.pdf"
                //   BaseFilename             : "Fees-Lutins.pdf"
                //   Number                   : 0
                //   BadFile                  : false
                //   DestinationDirectoryFile : false
                //
                // file : (print)

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
                    directory2 = destinationDirectory;
                    //if (file.SubDirectory != null && useSubdirectory)
                    //    directory2 = zPath.Combine(destinationDirectory, file.SubDirectory);
                    //Trace.WriteLine("_MoveFiles() directory \"{0}\"", directory2);
                    directoryDateStorage = PrintDirectoryManager.GetDirectoryDateStorage(directory2);
                }

                string pathFile = directory2;
                string filename = zPath.GetFileNameWithoutExtension(file.BaseFilename);
                if (!file.BadFile)
                {
                    PrintInfo printInfo = PrintIssue.GetPrintInfo(filename);
                    //Trace.WriteLine("_MoveFiles() filename \"{0}\" date \"{1}\"", filename, printInfo != null ? printInfo.Date.ToString() : "null");
                    if (printInfo != null && printInfo.Date != null)
                    {
                        string subDirectory = directoryDateStorage.GetDirectory((Date)printInfo.Date);
                        if (subDirectory != null)
                            pathFile = zPath.Combine(pathFile, subDirectory);
                    }
                    else if (!file.DestinationDirectoryFile)
                    {
                        if (file.SubDirectory != null)
                            pathFile = zPath.Combine(pathFile, file.SubDirectory);
                    }
                    //else if (file.File.StartsWith(destinationDirectory))
                    else // if (file.DestinationDirectoryFile)
                    {
                        // dont move unknow file of destination directory
                        if (n <= file.Number)
                            n = file.Number + 1;
                        continue;
                        // pas de continue sinon n n'est pas incrémenté
                        // keep directory file
                        //file = fileGroup.DirectoryInfo.Directory;
                    }
                }
                else
                    pathFile = zPath.Combine(pathFile, __badFileDirectory);
                pathFile = zPath.Combine(pathFile, filename);
                if (n > 0)
                    pathFile += string.Format("[{0}]", n);
                pathFile += zPath.GetExtension(file.BaseFilename);
                if (file.File != pathFile)
                {
                    moveFiles.Add(pathFile);
                    Trace.WriteLine("move file         \"{0}\"", file.File);
                    Trace.WriteLine("       to         \"{0}\"", pathFile + ".tmp");
                    if (!_simulate)
                    {
                        zfile.CreateFileDirectory(pathFile);
                        zFile.Move(file.File, pathFile + ".tmp");
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

            foreach (string file in moveFiles)
            {
                Trace.WriteLine("rename tmp file   \"{0}\"", file + ".tmp");
                Trace.WriteLine("             to   \"{0}\"", file);
                if (!_simulate)
                    zFile.Move(file + ".tmp", file);
            }
        }

        //private static Dictionary<int, int> GetDestinationFileNumbers(IEnumerable<FileGroup_v2> files)
        //{
        //    Dictionary<int, int> fileNumbers = new Dictionary<int,int>();
        //    foreach (FileGroup_v2 file in files)
        //    {
        //        if (file.DestinationDirectoryFile)
        //        {

        //        }

        //    }
        //    return fileNumbers;
        //}
    }

    public static partial class GlobalExtension
    {
        //public static Dictionary<TKey, List<TData>> zToKeyList<TKey, TData>(this IEnumerable<TData> dataList, Func<TData, TKey> getKey)
        //{
        //    Dictionary<TKey, List<TData>> dictionary = new Dictionary<TKey, List<TData>>();
        //    dictionary.zAddKeyList(dataList, getKey);
        //    return dictionary;
        //}

        public static void zAddSourceDirectoryFiles(this Dictionary<string, List<FileGroup_v2>> fileGroups, IEnumerable<FileGroup_v2> files)
        {
            //fileGroups.zKeyListAdd(PrintFileManager_v2.GetFiles(directory).Select(fileGroup => { fileGroup.SourceDirectoryFile = true; return fileGroup; }), fileGroup => fileGroup.BaseFilename);
            fileGroups.zKeyListAdd(files, fileGroup => fileGroup.BaseFilename);
        }

        public static void zAddDestinationDirectoryFiles(this Dictionary<string, List<FileGroup_v2>> fileGroups, IEnumerable<FileGroup_v2> files)
        {
            //fileGroups.zKeyListAddToExistingKey(PrintFileManager_v2.GetFiles(directory), fileGroup => fileGroup.BaseFilename);
            fileGroups.zKeyListAddToExistingKey(files.zAction(fileGroup => { fileGroup.DestinationDirectoryFile = true; }), fileGroup => fileGroup.BaseFilename);
        }

        public static void zKeyListAdd<TKey, TData>(this Dictionary<TKey, List<TData>> dictionary, IEnumerable<TData> dataList, Func<TData, TKey> getKey)
        {
            if (dictionary == null || dataList == null)
                return;
            foreach (TData data in dataList)
            {
                TKey key = getKey(data);
                if (!dictionary.ContainsKey(key))
                    dictionary.Add(key, new List<TData>());
                dictionary[key].Add(data);
            }
        }

        public static void zKeyListAddToExistingKey<TKey, TData>(this Dictionary<TKey, List<TData>> dictionary, IEnumerable<TData> dataList, Func<TData, TKey> getKey)
        {
            if (dictionary == null || dataList == null)
                return;
            foreach (TData data in dataList)
            {
                TKey key = getKey(data);
                if (dictionary.ContainsKey(key))
                    dictionary[key].Add(data);
            }
        }

        // string destinationDirectory, bool simulate = true
        public static IEnumerable<RenamePrintFile> zGetRenameDailyPrintFilesInfos(this IEnumerable<DailyPrintFile> files, FindPrintManager findPrintManager)
        {
            return PrintFileManager_v2.GetRenameDailyPrintFilesInfos(files, findPrintManager);
        }
    }
}
