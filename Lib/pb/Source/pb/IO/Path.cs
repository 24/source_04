using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace pb.IO
{
    public static class zpath
    {
        //public static string PathGetDirectory(string path)
        //{
        //    return zPath.GetDirectoryName(path);
        //}

        //public static string PathGetFileName(string path)
        //{
        //    return zPath.GetFileNameWithoutExtension(path);
        //}

        // test(1).txt  => test.txt
        // test[1].txt  => test.txt
        // test_2.txt  => test.txt

        //public static string PathGetFileNameWithExtension(string path)
        //{
        //    return zPath.GetFileName(path);
        //}

        //public static string PathGetExtension(string path)
        //{
        //    return zPath.GetExtension(path);
        //}

        public static string PathSetFileNameWithoutExtension(string path, string file)
        {
            if (path == null)
                return file;
            if (file == null)
                return path;
            string directory = zPath.GetDirectoryName(path);
            string ext = zPath.GetExtension(path);
            //if (directory != "" && !directory.EndsWith("\\"))
            //    directory += "\\";
            //return directory + file + ext;
            return zPath.Combine(directory, file + ext);
        }

        public static string PathSetFileName(string path, string file)
        {
            if (path == null)
                return file;
            if (file == null)
                return path;
            string directory = zPath.GetDirectoryName(path);
            //if (directory != "" && !directory.EndsWith("\\")) directory += "\\";
            //return directory + file;
            return zPath.Combine(directory, file);
        }

        public static string PathSetExtension(string path, string ext)
        {
            if (path == null || path == "")
                return ext;
            //if (ext == null) return path;
            //string dir = PathGetDir(path);
            //if (dir != "" && !dir.EndsWith("\\"))
            //    dir += "\\";
            //string file = PathGetFile(path);
            //if (ext.Length > 0 && ext[0] != '.') ext = "." + ext;
            return zPath.Combine(zPath.GetDirectoryName(path), zPath.GetFileNameWithoutExtension(path) + ext);
        }

        //public static string PathSetDir(string path, string directory)
        //{
        //    if (path == null) return null;
        //    if (directory == null) return path;
        //    string file = PathGetFileWithExt(path);
        //    if (!zPath.IsPathRooted(directory))
        //        directory = PathMakeRooted(directory, PathGetDir(path));
        //    if (directory != "" && !directory.EndsWith("\\")) directory += "\\";
        //    return directory + file;
        //}

        public static string PathSetDirectory(string file, string directory)
        {
            if (file == null)
                return null;
            string filename = zPath.GetFileName(file);
            if (directory == null)
                return filename;
            return zPath.Combine(directory, filename);
        }

        //public static string SetRootDirectory(string file, string rootDirectory = null)
        //public static string SetRootDirectory(string file, string rootDirectory)
        public static string RootPath(string file, string rootDirectory)
        {
            if (file == null)
                return null;
            if (zPath.IsPathRooted(file) || rootDirectory == null || rootDirectory == "")
                return file;
            //if (rootDirectory == null)
            //    rootDirectory = zapp.GetAppDirectory();
            return zPath.GetFullPath(zPath.Combine(rootDirectory, file));
        }

        //public static string PathAddDirectory(string path, string directory)
        //{
        //    if (path == null) return null;
        //    if (directory == null) return path;
        //    if (directory != "" && !directory.EndsWith("\\")) directory += "\\";
        //    return directory + path;
        //}

        //public static string PathMakeRooted(string path, string directory)
        //{
        //    if (path == null) return null;
        //    if (directory == null) return path;
        //    if (zPath.IsPathRooted(path))
        //        return path;
        //    else
        //        return zPath.GetFullPath(PathAddDirectory(path, directory));
        //}

        //public static string GetPathFile(string file)
        //{
        //    if (!zPath.IsPathRooted(file))
        //        file = zPath.Combine(Directory.GetCurrentDirectory(), file);
        //    return file;
        //}

        //public static string PathGetRelativeDirectory(string path, string baseDirectory)
        //{
        //    string directory = zPath.GetDirectoryName(path);
        //    if (directory.StartsWith(baseDirectory, StringComparison.CurrentCultureIgnoreCase))
        //    {
        //        int l = baseDirectory.Length;
        //        if (!baseDirectory.EndsWith("\\"))
        //        {
        //            if (directory.Length <= l || directory[l] != '\\') return null;
        //            l++;
        //        }
        //        if (directory.Length > l)
        //            return directory.Substring(l, directory.Length - l);
        //    }
        //    return null;
        //}

        //private static Regex __firstDirectory = new Regex(@"^([a-z]\:)?\\?([^\\]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        //public static string PathGetFirstDirectory(string directory)
        //{
        //    if (directory != null)
        //    {
        //        Match match = __firstDirectory.Match(directory);
        //        if (match.Success)
        //            return match.Groups[2].Value;
        //    }
        //    return null;
        //}
    }

    public static class PathExtension
    {
        //public static string zSetRootDirectory(this string file, string rootDirectory = null)
        //public static string zSetRootDirectory(this string file, string rootDirectory)
        public static string zRootPath(this string file, string rootDirectory)
        {
            return zpath.RootPath(file, rootDirectory);
        }
    }
}
