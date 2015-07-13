using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace pb.IO
{
    [Flags]
    public enum CopyFileOptions
    {
        None                        = 0x0000,
        Overwrite                   = 0x0001,
        CopyIfDestinationIsReadOnly = 0x0002,
        OverwriteReadOnly           = Overwrite | CopyIfDestinationIsReadOnly,
        CopyOnlyIfNewer             = 0x0004
        //CopyCreationTime            = 0x0008,
        //CopyLastAccessTime          = 0x0010,
        //CopyLastWriteTime           = 0x0020,
        //CopyAllFileTime             = CopyCreationTime | CopyLastAccessTime | CopyLastWriteTime,
        //CopyAttributes              = 0x0040,
        //CopyAllInfos                = CopyAllFileTime | CopyAttributes
    }

    public enum TextIndexOption
    {
        NumberBefore = 1,
        NumberAfter
    }

    public static class zfile
    {
        private static Regex __rgGetNewIndexedFileName = new Regex(@"{0.*}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        //private static Regex __rgBadFilenameChars = new Regex(@"(/|\?|%|&|\\|:)+", RegexOptions.Compiled);
        // new bad char : "
        private static Regex __rgBadFilenameChars = new Regex("(/|\\?|%|&|\\\\|:|\")+", RegexOptions.Compiled);

        public static string ReplaceBadFilenameChars(string file, string replaceBy = "")
        {
            return __rgBadFilenameChars.Replace(file, replaceBy);
        }

        public static bool CopyFile(string sourceFile, string destinationFile, CopyFileOptions options = CopyFileOptions.None)
        {
            if (!File.Exists(sourceFile))
                return false;
            if (File.Exists(destinationFile))
            {
                if ((options & CopyFileOptions.Overwrite) != CopyFileOptions.Overwrite)
                    return false;
                if ((options & CopyFileOptions.CopyOnlyIfNewer) == CopyFileOptions.CopyOnlyIfNewer)
                {
                    FileInfo sourceFileInfo = new FileInfo(sourceFile);
                    FileInfo destinationFileInfo = new FileInfo(destinationFile);
                    if (destinationFileInfo.LastWriteTimeUtc >= sourceFileInfo.LastWriteTimeUtc)
                        return false;
                }
                bool removeReadOnlyAttribute = false;
                if ((options & CopyFileOptions.CopyIfDestinationIsReadOnly) == CopyFileOptions.CopyIfDestinationIsReadOnly)
                    removeReadOnlyAttribute = true;
                DeleteFile(destinationFile, removeReadOnlyAttribute);
            }
            else
                CreateFileDirectory(destinationFile);
            File.Copy(sourceFile, destinationFile);
            return true;
        }

        public static string CopyFileToDirectory(string sourceFile, string destinationDirectory, string destinationFilename = null, CopyFileOptions options = CopyFileOptions.None)
        {
            string destinationFile;
            if (destinationFilename == null)
                destinationFile = zpath.PathSetDirectory(sourceFile, destinationDirectory);
            // vérifier si ce cas est utilisé
            //else if (destinationFilename.EndsWith("\\"))
            //    destinationFile = Path.Combine(destinationDirectory, destinationFilename + Path.GetFileName(sourceFile));
            else
                destinationFile = Path.Combine(destinationDirectory, destinationFilename);
            if (CopyFile(sourceFile, destinationFile, options))
                return destinationFile;
            else
                return null;
        }

        public static string CopyFileToDirectory(string file, string destinationDir, string destinationFile = null, bool overwrite = false, bool copyOnlyIfNewer = false)
        {
            if (!Directory.Exists(destinationDir))
                Directory.CreateDirectory(destinationDir);
            if (File.Exists(file))
            {
                string file2;
                if (destinationFile == null)
                    file2 = zpath.PathSetDirectory(file, destinationDir);
                else if (destinationFile.EndsWith("\\"))
                    file2 = Path.Combine(destinationDir, destinationFile + Path.GetFileName(file));
                else
                    file2 = Path.Combine(destinationDir, destinationFile);
                if (overwrite && File.Exists(file2))
                {
                    if (copyOnlyIfNewer)
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        FileInfo fileInfo2 = new FileInfo(file2);
                        if (fileInfo2.LastWriteTimeUtc >= fileInfo.LastWriteTimeUtc)
                            return null;
                    }
                    //FileAttributes attribs = File.GetAttributes(file2);
                    //if ((attribs & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    //    File.SetAttributes(file2, attribs & ~FileAttributes.ReadOnly);
                    RemoveFileReadOnlyAttribute(file2);
                }
                File.Copy(file, file2, overwrite);
                return file2;
            }
            return null;
        }

        public static void RemoveFileReadOnlyAttribute(string file)
        {
            FileAttributes attribs = File.GetAttributes(file);
            if ((attribs & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                File.SetAttributes(file, attribs & ~FileAttributes.ReadOnly);
        }

        public static bool FilesEquals(string path1, string path2)
        {
            FileInfo fi1 = new FileInfo(path1);
            FileInfo fi2 = new FileInfo(path2);
            if (fi1.Length != fi2.Length)
                return false;
            BinaryReader br1 = null;
            BinaryReader br2 = null;
            int bufferLen = 4096;
            byte[] buffer1 = new byte[bufferLen];
            byte[] buffer2 = new byte[bufferLen];
            try
            {
                br1 = new BinaryReader(new FileStream(path1, FileMode.Open, FileAccess.Read, FileShare.Read));
                br2 = new BinaryReader(new FileStream(path2, FileMode.Open, FileAccess.Read, FileShare.Read));
                while (true)
                {
                    int n1 = br1.Read(buffer1, 0, bufferLen);
                    int n2 = br2.Read(buffer2, 0, bufferLen);
                    if (n1 == 0 && n2 == 0)
                        break;
                    if (n1 != n2)
                        throw new PBException("error comparing files \"{0}\" \"{1}\"", path1, path2);
                    if (!((IStructuralEquatable)buffer1).Equals(buffer2, StructuralComparisons.StructuralEqualityComparer))
                        return false;
                }
            }
            finally
            {
                if (br1 != null)
                    br1.Close();
                if (br2 != null)
                    br2.Close();
            }
            return true;
        }

        public static void DeleteFile(string file, bool removeReadOnlyAttribute = true)
        {
            if (File.Exists(file))
            {
                if (removeReadOnlyAttribute)
                    RemoveFileReadOnlyAttribute(file);
                File.Delete(file);
            }
        }

        public static void DeleteFiles(string pathWithPattern, bool removeReadOnlyAttribute = true)
        {
            string dir = Path.GetDirectoryName(pathWithPattern);
            string searchPattern = Path.GetFileName(pathWithPattern);
            DeleteFiles(dir, searchPattern, removeReadOnlyAttribute);
        }

        public static void DeleteFiles(string dir, string searchPattern, bool removeReadOnlyAttribute = true)
        {
            DeleteFiles(dir, searchPattern, true, removeReadOnlyAttribute);
        }

        public static void DeleteFiles(string dir, string searchPattern, bool throwError, bool removeReadOnlyAttribute = true)
        {
            if (!Directory.Exists(dir))
                return;
            string[] files = Directory.GetFiles(dir, searchPattern);
            foreach (string file in files)
            {
                try
                {
                    //File.Delete(file);
                    DeleteFile(file, removeReadOnlyAttribute);
                }
                catch
                {
                    if (throwError)
                        throw;
                }
            }
        }

        public static void DeleteDirectoryFiles(string dir)
        {
            DeleteFiles(dir, "*.*");
        }

        public static void RenameFile(string file, string newFile, bool overwrite = false)
        {
            if (File.Exists(file))
            {
                if (overwrite && File.Exists(newFile))
                {
                    RemoveFileReadOnlyAttribute(newFile);
                    File.Delete(newFile);
                }
                File.Move(file, newFile);
            }
        }

        public static void MoveFile(string path, string dir, bool overwrite = false)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            if (File.Exists(path))
            {
                string path2 = zpath.PathSetDirectory(path, dir);
                if (overwrite && File.Exists(path2))
                {
                    RemoveFileReadOnlyAttribute(path2);
                    File.Delete(path2);
                }
                File.Move(path, path2);
            }
        }

        public static void MoveFiles(IEnumerable<string> files, string dir, bool overwrite = false)
        {
            foreach (string file in files)
                MoveFile(file, dir, overwrite);
        }

        public static void MoveFiles(string path, string dir, bool overwrite = false)
        {
            //if (!Directory.Exists(dir))
            //    Directory.CreateDirectory(dir);
            foreach (string file in DirectoryGetFiles(path))
                MoveFile(file, dir, overwrite);
        }

        //public static string ReadFile(string sPath, int iOffset, int iCount)
        //{
        //    char[] cBuff;
        //    string s;
        //    FileStream fs;
        //    StreamReader sr;

        //    fs = new FileStream(sPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        //    if (iCount == -1) { iCount = (int)fs.Length - iOffset; }
        //    cBuff = new char[iCount];
        //    fs.Seek(iOffset, SeekOrigin.Begin);
        //    sr = new StreamReader(fs, Encoding.Default);
        //    try
        //    {
        //        sr.Read(cBuff, 0, iCount);
        //        s = new string(cBuff);
        //        return s;
        //    }
        //    finally
        //    {
        //        sr.Close();
        //    }
        //}

        public static string ReadAllText(string file, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            return File.ReadAllText(file, encoding);
        }

        public static byte[] ReadAllBytes(string file)
        {
            return File.ReadAllBytes(file);
        }

        //public static string ReadFile(string file)
        //{
        //    if (file == null) return null;
        //    FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        //    StreamReader sr = new StreamReader(fs, Encoding.Default);
        //    try
        //    {
        //        string s = sr.ReadToEnd();
        //        return s;
        //    }
        //    finally
        //    {
        //        sr.Close();
        //    }
        //}

        public static IEnumerable<string> ReadLines(string file, Encoding encoding = null)
        {
            if (file != null)
            {
                if (encoding == null)
                    encoding = Encoding.UTF8;
                return File.ReadLines(file, encoding);
                //FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                //StreamReader sr = new StreamReader(fs, encoding);
                //try
                //{
                //    while (!sr.EndOfStream)
                //    {
                //        yield return sr.ReadLine();

                //    }
                //}
                //finally
                //{
                //    sr.Close();
                //}
            }
            else
                return new string[0];
        }

        //private static Regex _rgSplitLines = new Regex("\r?\n", RegexOptions.Compiled);
        public static string[] ReadAllLines(string file, Encoding encoding = null)
        {
            //if (file == null) return null;
            //FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            //StreamReader sr = new StreamReader(fs, Encoding.Default);
            //try
            //{
            //    string s = sr.ReadToEnd();
            //    return _rgSplitLines.Split(s);
            //}
            //finally
            //{
            //    sr.Close();
            //}
            if (encoding == null)
                encoding = Encoding.UTF8;
            return File.ReadAllLines(file, encoding);
        }

        //public static void WriteFile(string sPath, string s)
        //{
        //    WriteFile(sPath, s, false);
        //}

        public static void WriteFile(string file, string text, bool append = false, Encoding encoding = null)
        {
            string dir = Path.GetDirectoryName(file);
            if (dir != "" && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            if (encoding == null)
                encoding = Encoding.UTF8;
            FileMode fm;
            if (append) fm = FileMode.Append; else fm = FileMode.Create;
            FileStream fs = new FileStream(file, fm, FileAccess.Write, FileShare.Read);
            StreamWriter sw = new StreamWriter(fs, encoding);
            try
            {
                sw.Write(text);
            }
            finally
            {
                sw.Close();
            }
        }

        public static void WriteFile(string file, byte[] bytes)
        {
            CreateFileDirectory(file);
            FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Read);
            BinaryWriter bw = new BinaryWriter(fs, Encoding.Default);
            try
            {
                bw.Write(bytes);
            }
            finally
            {
                bw.Close();
            }
        }

        public static string StreamReadText(StreamReader stream)
        {
            StringBuilder sb = new StringBuilder(10000);
            int iChar;
            while ((iChar = stream.Read()) != -1)
                sb.Append((char)iChar);
            return sb.ToString();
        }

        public static void StreamWrite(Stream source, Stream destination)
        {
            int bufferLength = 32000;
            byte[] buffer = new byte[bufferLength];
            while (true)
            {
                int readLength = source.Read(buffer, 0, bufferLength);
                if (readLength == 0) break;
                destination.Write(buffer, 0, readLength);
            }
        }

        public static string GetNewFilename(string path)
        {
            if (File.Exists(path))
            {
                string filename = Path.GetFileNameWithoutExtension(path);
                string ext = Path.GetExtension(path);
                string directory = Path.GetDirectoryName(path);
                for (int i = 1; ; i++)
                {
                    path = Path.Combine(directory, string.Format("{0}[{1}]{2}", filename, i, ext));
                    if (!File.Exists(path))
                        break;
                }

            }
            return path;
        }

        public static string GetNewDirectory(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
            {
                string directoryName = Path.GetFileName(directoryPath);
                string parentDirectoryPath = Path.GetDirectoryName(directoryPath);
                for (int i = 1; ; i++)
                {
                    directoryPath = Path.Combine(parentDirectoryPath, string.Format("{0}[{1}]", directoryName, i));
                    if (!Directory.Exists(directoryPath))
                        break;
                }

            }
            Directory.CreateDirectory(directoryPath);
            return directoryPath;
        }

        //public static string GetNewIndexedFileName(string path)
        //{
        //    int index; string fileMask;
        //    return GetNewIndexedFileName(path, out index, out fileMask);
        //}

        //public static string GetNewIndexedFileName(string path, out int index, out string fileMask)
        //{
        //    index = 0;
        //    fileMask = null;
        //    if (path == null) return null;

        //    // Path.GetDirectoryName(path);
        //    // exception générée et capturée par le debugger mais qui n'apparait pas à l'exécution
        //    // System.NotSupportedException "The given path's format is not supported."
        //    // path = "C:\\pib\\prog\\tools\\runsource\\run\\RunSource_{0:00000}.*"
        //    string dir = Path.GetDirectoryName(path);
        //    int l = dir.Length + 1;
        //    string file = path.Substring(l, path.Length - l);

        //    string ext = null;
        //    int i = file.LastIndexOf('.');
        //    if (i != -1)
        //    {
        //        ext = file.Substring(i, file.Length - i);
        //        file = file.Substring(0, i);
        //    }

        //    if (!__rgGetNewIndexedFileName.IsMatch(file)) file = "{0:0000}_" + file;
        //    index = GetLastFileNameIndex(dir, file, ext) + 1;

        //    fileMask = file;
        //    if (ext != null) fileMask += ext;
        //    fileMask = Path.Combine(dir, fileMask);
        //    return string.Format(fileMask, index);
        //}

        //public static string GetNewIndexedDirectory(string path)
        //{
        //    if (path == null) return null;

        //    string dir = Path.GetDirectoryName(path);
        //    int l = dir.Length + 1;
        //    string dirname = path.Substring(l, path.Length - l);

        //    if (!__rgGetNewIndexedFileName.IsMatch(dirname))
        //        dirname = "{0:0000}_" + dirname;
        //    int iIndex = GetLastDirectoryIndex(dir, dirname) + 1;
        //    return Path.Combine(dir, string.Format(dirname, iIndex));
        //}

        //public static int GetLastFileNameIndex(string dir, string file, string ext)
        //{
        //    if (!Directory.Exists(dir))
        //    {
        //        Directory.CreateDirectory(dir);
        //        return 0;
        //    }
        //    string sRegex;
        //    string sFileSearch;
        //    Match match = __rgGetNewIndexedFileName.Match(file);
        //    if (!match.Success)
        //    {
        //        sFileSearch = "*_*" + file;
        //        sRegex = "^([0-9]+)_" + file;
        //    }
        //    else
        //    {
        //        sRegex = "^" + match.zReplace(file, "([0-9]+)");
        //        sFileSearch = match.zReplace(file, "*");
        //        if (!sFileSearch.EndsWith("*")) sFileSearch += "*";
        //    }
        //    if (ext != null && ext != "")
        //        sFileSearch += ext;
        //    else
        //        sFileSearch += ".*";
        //    Regex rx = new Regex(sRegex, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        //    string[] sFiles = Directory.GetFiles(dir, sFileSearch);
        //    int iMax = 0;
        //    foreach (string sFile in sFiles)
        //    {
        //        match = rx.Match(Path.GetFileName(sFile));
        //        if (!match.Success) continue;
        //        int iIndex = int.Parse(match.Groups[1].Value);
        //        if (iMax < iIndex) iMax = iIndex;
        //    }
        //    return iMax;
        //}

        //public static int GetLastDirectoryIndex(string dir, string dirname)
        //{
        //    if (!Directory.Exists(dir))
        //    {
        //        Directory.CreateDirectory(dir);
        //        return 0;
        //    }
        //    string sRegex;
        //    string sDirSearch;
        //    Match match = __rgGetNewIndexedFileName.Match(dirname);
        //    if (!match.Success)
        //    {
        //        sDirSearch = "*_*" + dirname;
        //        sRegex = "^([0-9]+)_" + dirname;
        //    }
        //    else
        //    {
        //        sRegex = "^" + match.zReplace(dirname, "([0-9]+)");
        //        sDirSearch = match.zReplace(dirname, "*");
        //        if (!sDirSearch.EndsWith("*")) sDirSearch += "*";
        //    }

        //    Regex rx = new Regex(sRegex, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        //    string[] sFiles = Directory.GetDirectories(dir, sDirSearch);
        //    int iMax = 0;
        //    foreach (string sFile in sFiles)
        //    {
        //        match = rx.Match(Path.GetFileName(sFile));
        //        if (!match.Success) continue;
        //        int iIndex = int.Parse(match.Groups[1].Value);
        //        if (iMax < iIndex) iMax = iIndex;
        //    }
        //    return iMax;
        //}

        public static string GetNewIndexedFileName(string dir, string filename = null, int indexLength = 0, TextIndexOption option = TextIndexOption.NumberBefore)
        {
            if (dir == null)
                return null;

            int maxIndexLength;
            int index = GetLastFileNameIndex(dir, filename, option, out maxIndexLength) + 1;
            if (indexLength == 0)
                indexLength = maxIndexLength;
            if (indexLength == 0)
                indexLength = 4;
            string indexedFile = index.ToString();
            int l = indexedFile.Length;
            if (l < indexLength)
                indexedFile = new string('0', indexLength - l) + indexedFile;
            return Path.Combine(dir, indexedFile + filename);
        }

        public static int GetLastFileNameIndex(string dir, string filename = null, TextIndexOption option = TextIndexOption.NumberBefore)
        {
            int maxIndexLength;
            return GetLastFileNameIndex(dir, filename, option, out maxIndexLength);
        }

        //public static int GetLastFileNameIndex(string dir, string filename, out int maxIndexLength)
        public static int GetLastFileNameIndex(string dir, string filename, TextIndexOption option, out int maxIndexLength)
        {
            maxIndexLength = 0;
            if (!Directory.Exists(dir))
                return 0;
            //return Directory.EnumerateFiles(dir).Select(d => Path.GetFileName(d)).zGetLastTextIndex(filename, out maxIndexLength);
            //return Directory.EnumerateFiles(dir).Select(d => Path.GetFileName(d)).zGetLastTextIndex(filename, option, out maxIndexLength);
            return GetLastTextIndex(Directory.EnumerateFiles(dir).Select(d => Path.GetFileName(d)), filename, option, out maxIndexLength);
        }

        private static Regex __textIndexNumberBefore = new Regex("(?<number>[0-9]+)(?<name>.*)$", RegexOptions.Compiled);
        private static Regex __textIndexNumberAfter = new Regex("^(?<name>.*)(?<number>[0-9]+)", RegexOptions.Compiled);
        /// <summary></summary>
        /// <param name="texts"></param>
        /// <param name="name"></param>
        /// <param name="maxIndexLength">ex : "000123" maxIndexLength = 6</param>
        /// <returns></returns>
        //public static int GetLastTextIndex(IEnumerable<string> texts, string name, out int maxIndexLength)
        public static int GetLastTextIndex(IEnumerable<string> texts, string name, TextIndexOption option, out int maxIndexLength)
        {
            if (name != null)
                name = name.ToLower();
            Regex regex;
            if (option == TextIndexOption.NumberAfter)
                regex = __textIndexNumberAfter;
            else // if (option == TextIndexOption.NumberBefore)
                regex = __textIndexNumberBefore;
            int lastIndex = 0;
            maxIndexLength = 0;
            foreach (string text in texts)
            {
                Match match = regex.Match(text);
                if (match.Success)
                {
                    //if (name != null && match.Groups[2].Value.ToLower() != name)
                    if (name != null && match.Groups["name"].Value.ToLower() != name)
                        continue;
                    //int index = int.Parse(match.Groups[1].Value);
                    int index = int.Parse(match.Groups["number"].Value);
                    if (index > lastIndex)
                        lastIndex = index;
                    int indexLength = match.Groups[1].Value.Length;
                    if (indexLength > maxIndexLength)
                        maxIndexLength = indexLength;
                }
            }
            return lastIndex;
        }

        public static string[] DirectoryGetFiles(string sPath)
        {
            return Directory.GetFiles(Path.GetDirectoryName(sPath), Path.GetFileName(sPath));
        }

        public static string[] DirectoryGetFiles(string sDir, params string[] sPatterns)
        {
            string[][] sFiles1 = new string[sPatterns.Length][];
            int n = 0;
            for (int i = 0; i < sPatterns.Length; i++)
            {
                sFiles1[i] = Directory.GetFiles(sDir, sPatterns[i]);
                n += sFiles1[i].Length;
            }
            string[] sFiles = new string[n];
            int i2 = 0;
            for (int i = 0; i < sPatterns.Length; i++)
            {
                sFiles1[i].CopyTo(sFiles, i2);
                i2 += sFiles1[i].Length;
            }
            return sFiles;
        }

        public static string GetUriPath(string sPath)
        {
            UriBuilder u = new UriBuilder();
            u.Scheme = Uri.UriSchemeFile;
            u.Path = sPath;
            return u.Uri.AbsoluteUri;
        }

        public static void CreateFileDirectory(string path)
        {
            string dir = Path.GetDirectoryName(path);
            if (dir != "" && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        public static byte[] ReadBinaryFile(string file)
        {
            if (file == null) return null;
            FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            BinaryReader br = new BinaryReader(fs, Encoding.Default);
            try
            {
                return br.ReadBytes((int)fs.Length);
            }
            finally
            {
                br.Close();
            }
        }

        public static StreamWriter CreateCreateText(string file, bool createNew = false, Encoding encoding = null)
        {
            FileMode fileMode;
            if (createNew)
                fileMode = FileMode.CreateNew;
            else
                fileMode = FileMode.Create;
            if (encoding == null)
                encoding = Encoding.UTF8;
            return new StreamWriter(new FileStream(file, fileMode, FileAccess.Write, FileShare.Read), encoding);
        }

        public static bool AreFileEqual(string file1, string file2)
        {
            int len = 10000;
            FileInfo fi1 = new FileInfo(file1);
            FileInfo fi2 = new FileInfo(file2);
            if (fi1.Length != fi2.Length)
                return false;
            FileStream fs1 = null;
            FileStream fs2 = null;
            BinaryReader br1 = null;
            BinaryReader br2 = null;
            try
            {
                fs1 = File.OpenRead(file1);
                br1 = new BinaryReader(fs1);
                fs2 = File.OpenRead(file2);
                br2 = new BinaryReader(fs2);
                while (true)
                {
                    byte[] byte1 = br1.ReadBytes(len);
                    byte[] byte2 = br2.ReadBytes(len);
                    if (!StructuralComparisons.StructuralEqualityComparer.Equals(byte1, byte2))
                        return false;
                    if (byte1.Length == 0)
                        return true;
                }
            }
            finally
            {
                if (br1 != null)
                    br1.Close();
                if (br2 != null)
                    br2.Close();
                if (fs1 != null)
                    fs1.Close();
                if (fs2 != null)
                    fs2.Close();
            }

        }
    }

    //public static partial class GlobalExtension
    //{
    //    //public static int zGetLastTextIndex(this IEnumerable<string> texts, string name, out int maxIndexLength)
    //    public static int zGetLastTextIndex(this IEnumerable<string> texts, string name, TextIndexOption option, out int maxIndexLength)
    //    {
    //        //return zfile.GetLastTextIndex(texts, name, out maxIndexLength);
    //        return zfile.GetLastTextIndex(texts, name, option, out maxIndexLength);
    //    }
    //}
}
