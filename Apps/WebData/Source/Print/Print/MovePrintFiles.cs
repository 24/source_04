using System.Collections.Generic;
using pb;
using pb.IO;

// todo :
//  - move also .i files ($$todo)
//  - delete empty .i directory
//  - filter file .i
namespace Download.Print
{
    public class MovePrintFiles
    {
        private bool _simulate = false;
        private bool _moveInfoFiles = false;
        private PathPrintFile _pathPrintFile = null;
        private List<string> _moveFiles = null;
        private Dictionary<int, int> _fileNumbers = null;
        private int _lastFileNumber = -1;

        //public bool Simulate { get { return _simulate; } set { _simulate = value; } }
        public PathPrintFile PathPrintFile { get { return _pathPrintFile; } set { _pathPrintFile = value; } }

        public void MoveFiles(IEnumerable<IEnumerable<FileGroup_v2>> filesGroups, string destinationDirectory, bool simulate, bool moveInfoFiles)
        {
            _simulate = simulate;
            _moveInfoFiles = moveInfoFiles;
            _pathPrintFile.DestinationDirectory = destinationDirectory;
            foreach (IEnumerable<FileGroup_v2> files in filesGroups)
            {
                MoveFiles(files);
            }
        }

        private void MoveFiles(IEnumerable<FileGroup_v2> files)
        {
            _moveFiles = new List<string>();
            _fileNumbers = new Dictionary<int, int>();
            _lastFileNumber = -1;

            // rename destination files
            foreach (FileGroup_v2 file in files)
            {
                if (file.DestinationDirectoryFile)
                {
                    if (AddFileNumber(file.Number))
                    {
                        string path = _pathPrintFile.GetPath(file);
                        if (file.File != path)
                            MoveFileToTmp(file.File, path);
                        file.Ok = true;
                    }
                }
            }

            // rename destination files with duplicate number
            foreach (FileGroup_v2 file in files)
            {
                if (file.DestinationDirectoryFile && !file.Ok)
                {
                    file.Number = GetNewFileNumber();
                    MoveFileToTmp(file.File, _pathPrintFile.GetPath(file));
                    file.Ok = true;
                }
            }

            // rename source files
            foreach (FileGroup_v2 file in files)
            {
                if (!file.DestinationDirectoryFile)
                {
                    file.Number = GetNewFileNumber();
                    MoveFileToTmp(file.File, _pathPrintFile.GetPath(file));
                    file.Ok = true;
                }
            }

            RenameTmpFiles();
        }

        private bool AddFileNumber(int number)
        {
            if (!_fileNumbers.ContainsKey(number))
            {
                _fileNumbers.Add(number, number);
                return true;
            }
            else
                return false;
        }

        private int GetNewFileNumber()
        {
            int number = _lastFileNumber + 1;
            while (true)
            {
                if (!_fileNumbers.ContainsKey(number))
                {
                    _fileNumbers.Add(number, number);
                    _lastFileNumber = number;
                    return number;
                }
                number++;
            }
        }

        private void MoveFileToTmp(string sourcePath, string destinationPath)
        {
            // example :
            //   move file              "g:\pib\media\ebook\_dl\_dl_pib\print\03\print\.02_hebdo\Le point\Le point - 2016-05-26 - no 16107.pdf"
            //          to              "g:\pib\media\ebook\print\.02_hebdo\Le point\2016\Le point - 2016-05-26 - no 16107.pdf.tmp"
            //   move info file         "g:\pib\media\ebook\_dl\_dl_pib\print\03\print\.02_hebdo\Le point\.i\Le point - 2016-05-26 - no 16107.pdf.i"
            //          to              "g:\pib\media\ebook\print\.02_hebdo\Le point\2016\.i\Le point - 2016-05-26 - no 16107.pdf.i.tmp"
            _moveFiles.Add(destinationPath);
            string tmpDestinationPath = destinationPath + ".tmp";
            Trace.WriteLine("move file              \"{0}\"", sourcePath);
            Trace.WriteLine("  to                   \"{0}\"", tmpDestinationPath);
            string sourceInfoPath = null;
            string tmpDestinationInfoPath = null;
            bool moveInfoFile = false;
            if (_moveInfoFiles)
            {
                sourceInfoPath = InfoFile.GetInfoFile(sourcePath);
                if (zFile.Exists(sourceInfoPath))
                {
                    moveInfoFile = true;
                    tmpDestinationInfoPath = InfoFile.GetInfoFile(destinationPath) + ".tmp";
                    Trace.WriteLine("move info file         \"{0}\"", sourceInfoPath);
                    Trace.WriteLine("  to                   \"{0}\"", tmpDestinationInfoPath);
                }
            }
            if (!_simulate)
            {
                zfile.CreateFileDirectory(tmpDestinationPath);
                zFile.Move(sourcePath, tmpDestinationPath);
                if (moveInfoFile)
                {
                    zfile.CreateFileDirectory(tmpDestinationInfoPath);
                    zFile.Move(sourceInfoPath, tmpDestinationInfoPath);
                }
            }
        }

        private void RenameTmpFiles()
        {
            // example :
            //   rename tmp file        "g:\pib\media\ebook\print\.02_hebdo\Le point\2016\Le point - 2016-05-26 - no 16107.pdf.tmp"
            //                to        "g:\pib\media\ebook\print\.02_hebdo\Le point\2016\Le point - 2016-05-26 - no 16107.pdf"
            //   rename info tmp file   "g:\pib\media\ebook\print\.02_hebdo\Le point\2016\.i\Le point - 2016-05-26 - no 16107.pdf.i.tmp"
            //                to        "g:\pib\media\ebook\print\.02_hebdo\Le point\2016\.i\Le point - 2016-05-26 - no 16107.pdf.i"
            foreach (string file in _moveFiles)
            {
                Trace.WriteLine("rename tmp file        \"{0}\"", file + ".tmp");
                Trace.WriteLine("  to                   \"{0}\"", file);
                string infoFile = null;
                bool moveInfoFile = false;
                if (_moveInfoFiles)
                {
                    infoFile = InfoFile.GetInfoFile(file);
                    if (zFile.Exists(infoFile + ".tmp"))
                    {
                        moveInfoFile = true;
                        Trace.WriteLine("rename tmp info file   \"{0}\"", infoFile + ".tmp");
                        Trace.WriteLine("  to                   \"{0}\"", infoFile);
                    }
                }
                if (!_simulate)
                {
                    zFile.Move(file + ".tmp", file);
                    if (moveInfoFile)
                    {
                        zFile.Move(infoFile + ".tmp", infoFile);
                    }
                }
            }
        }

        //public void MoveFiles_v1(IEnumerable<FileGroup_v2> files)
        //{
        //    // destinationDirectory : g:\pib\media\ebook\print
        //    // destinationDirectory : g:\pib\media\ebook\book\unsorted\Les fées cuisinent et les lutins dînent
        //    //Trace.WriteLine("_MoveFiles() directory \"{0}\"", directory);
        //    int n = 0;
        //    Dictionary<int, int> fileNumbers = new Dictionary<int, int>();
        //    List<string> moveFiles = new List<string>();
        //    //PrintDirectoryDateStorage directoryDateStorage = null;
        //    //bool first = true;
        //    //string directory2 = null;
        //    foreach (FileGroup_v2 file in files)
        //    {
        //        // file : (book)
        //        //   Directory                : "g:\pib\media\ebook\_dl\_dl_pib\book\10\book\Les fées cuisinent et les lutins dînent"
        //        //   SubDirectory             : "Les fées cuisinent et les lutins dînent"
        //        //   File                     : "g:\pib\media\ebook\_dl\_dl_pib\book\10\book\Les fées cuisinent et les lutins dînent\Les fées cuisinent et les lutins dînent\Fees-Lutins.pdf"
        //        //   BaseFilename             : "Fees-Lutins.pdf"
        //        //   Number                   : 0
        //        //   BadFile                  : false
        //        //   DestinationDirectoryFile : false
        //        //
        //        // file : (print)

        //        // fileGroup.DirectoryGroup.BaseDirectory
        //        // fileGroup.DirectoryInfo.Directory    : g:\\pib\\media\\ebook\\print\\.02_hebdo\\01 net
        //        //                                        g:\\pib\\media\\ebook\\_dl\\_dl_pib\\01\\print\\.02_hebdo\\01 net
        //        // fileGroup.DirectoryInfo.SubDirectory : .02_hebdo\\01 net
        //        // fileGroup.SubDirectory               : 2014
        //        // fileGroup.File                       : g:\\pib\\media\\ebook\\print\\.02_hebdo\\01 net\\2014\\01 net - 2014-10-16 - no 806(1).pdf
        //        // fileGroup.BaseFilename               : 01 net - 2014-10-16 - no 806.pdf
        //        // fileGroup.Number                     : 1

        //        //if (first)
        //        //{
        //        //    first = false;
        //        //    //Trace.WriteLine("_MoveFiles() SubDirectory \"{0}\"", fileGroup.DirectoryInfo.SubDirectory);
        //        //    //directory2 = destinationDirectory;
        //        //    //if (file.SubDirectory != null && useSubdirectory)
        //        //    //    directory2 = zPath.Combine(destinationDirectory, file.SubDirectory);
        //        //    //Trace.WriteLine("_MoveFiles() directory \"{0}\"", directory2);
        //        //    //directoryDateStorage = PrintDirectoryManager.GetDirectoryDateStorage(destinationDirectory);
        //        //}

        //        //////string pathFile = _destinationDirectory;
        //        //////string filename = zPath.GetFileNameWithoutExtension(file.BaseFilename);
        //        //////if (!file.BadFile)
        //        //////{
        //        //////    PrintInfo printInfo = PrintIssue.GetPrintInfo(filename);
        //        //////    //Trace.WriteLine("_MoveFiles() filename \"{0}\" date \"{1}\"", filename, printInfo != null ? printInfo.Date.ToString() : "null");
        //        //////    if (printInfo != null && printInfo.Date != null)
        //        //////    {
        //        //////        //string subDirectory = directoryDateStorage.GetDirectory((Date)printInfo.Date);
        //        //////        string subDirectory = GetPrintDirectoryDateStorage().GetDirectory((Date)printInfo.Date);
        //        //////        if (subDirectory != null)
        //        //////            pathFile = zPath.Combine(pathFile, subDirectory);
        //        //////    }
        //        //////    else if (!file.DestinationDirectoryFile)
        //        //////    {
        //        //////        if (file.SubDirectory != null)
        //        //////            pathFile = zPath.Combine(pathFile, file.SubDirectory);
        //        //////    }
        //        //////    //else if (file.File.StartsWith(destinationDirectory))
        //        //////    else // if (file.DestinationDirectoryFile)
        //        //////    {
        //        //////        // dont move unknow file of destination directory
        //        //////        if (n <= file.Number)
        //        //////            n = file.Number + 1;
        //        //////        continue;
        //        //////        // pas de continue sinon n n'est pas incrémenté
        //        //////        // keep directory file
        //        //////        //file = fileGroup.DirectoryInfo.Directory;
        //        //////    }
        //        //////}
        //        //////else
        //        //////    pathFile = zPath.Combine(pathFile, __badFileDirectory);
        //        //////pathFile = zPath.Combine(pathFile, filename);
        //        //////if (n > 0)
        //        //////    pathFile += string.Format("[{0}]", n);
        //        //////pathFile += zPath.GetExtension(file.BaseFilename);
        //        if (file.File != pathFile)
        //        {
        //            moveFiles.Add(pathFile);
        //            Trace.WriteLine("move file         \"{0}\"", file.File);
        //            Trace.WriteLine("       to         \"{0}\"", pathFile + ".tmp");
        //            if (!_simulate)
        //            {
        //                zfile.CreateFileDirectory(pathFile);
        //                zFile.Move(file.File, pathFile + ".tmp");
        //            }
        //        }
        //        //if (fileGroup.Number != n || fileGroup.DirectoryInfo.Directory != directory)
        //        //{
        //        //    string file = zPath.Combine(directory, fileGroup.DirectoryInfo.SubDirectory, zPath.GetFileNameWithoutExtension(fileGroup.BaseFilename));
        //        //    if (n > 0)
        //        //        file += string.Format("[{0}]", n);
        //        //    file += zPath.GetExtension(fileGroup.BaseFilename);
        //        //    files.Add(file);
        //        //    Trace.WriteLine("move file \"{0}\" to \"{1}\"", fileGroup.File, file + ".tmp");
        //        //    if (!_simulate)
        //        //    {
        //        //        zfile.CreateFileDirectory(file);
        //        //        File.Move(fileGroup.File, file + ".tmp");
        //        //    }
        //        //}
        //        n++;
        //    }

        //    foreach (string file in moveFiles)
        //    {
        //        Trace.WriteLine("rename tmp file   \"{0}\"", file + ".tmp");
        //        Trace.WriteLine("             to   \"{0}\"", file);
        //        if (!_simulate)
        //            zFile.Move(file + ".tmp", file);
        //    }
        //}
    }

    public class PathPrintFile
    {
        private string _badFileDirectory = "_bad";
        private string _destinationDirectory = null;
        private PrintDirectoryDateStorage _directoryDateStorage = null;

        public string BadFileDirectory { get { return _badFileDirectory; } set { _badFileDirectory = value; } }
        public string DestinationDirectory { get { return _destinationDirectory; } set { _destinationDirectory = value; _directoryDateStorage = null; } }

        private PrintDirectoryDateStorage GetPrintDirectoryDateStorage()
        {
            if (_directoryDateStorage == null)
                _directoryDateStorage = PrintDirectoryManager.GetDirectoryDateStorage(_destinationDirectory);
            return _directoryDateStorage;
        }

        public string GetPath(FileGroup_v2 file)
        {
            string pathFile = _destinationDirectory;
            string filename = zPath.GetFileNameWithoutExtension(file.BaseFilename);
            if (!file.BadFile)
            {
                PrintInfo printInfo = PrintIssue.GetPrintInfo(filename);
                //Trace.WriteLine("_MoveFiles() filename \"{0}\" date \"{1}\"", filename, printInfo != null ? printInfo.Date.ToString() : "null");
                if (printInfo != null && printInfo.Date != null)
                {
                    string subDirectory = GetPrintDirectoryDateStorage().GetDirectory((Date)printInfo.Date);
                    if (subDirectory != null)
                        pathFile = zPath.Combine(pathFile, subDirectory);
                }
                else if (!file.DestinationDirectoryFile)
                {
                    if (file.SubDirectory != null)
                        pathFile = zPath.Combine(pathFile, file.SubDirectory);
                }
                //else if (file.File.StartsWith(destinationDirectory))
                //else // if (file.DestinationDirectoryFile)
                //{
                //    // dont move unknow file of destination directory
                //    if (n <= file.Number)
                //        n = file.Number + 1;
                //    continue;
                //    // pas de continue sinon n n'est pas incrémenté
                //    // keep directory file
                //    //file = fileGroup.DirectoryInfo.Directory;
                //}
            }
            else
                pathFile = zPath.Combine(pathFile, _badFileDirectory);
            pathFile = zPath.Combine(pathFile, filename);
            if (file.Number > 0)
                pathFile += string.Format("[{0}]", file.Number);
            pathFile += zPath.GetExtension(file.BaseFilename);
            return pathFile;
        }
    }
}
