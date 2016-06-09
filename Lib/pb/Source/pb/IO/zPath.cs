using Path = Pri.LongPath.Path;
//using Directory = Pri.LongPath.Directory;
//using DirectoryInfo = Pri.LongPath.DirectoryInfo;
//using File = Pri.LongPath.File;

namespace pb.IO
{
    public static class zPath
    {
        public static string Combine(string path1, string path2)
        {
            if (path1 == null)
                return path2;
            if (path2 == null)
                return path1;
            return Path.Combine(path1, path2);
        }

        public static string Combine(string path1, string path2, string path3)
        {
            return Path.Combine(path1, path2, path3);
        }

        public static string Combine(string path1, string path2, string path3, string path4)
        {
            return Path.Combine(path1, path2, path3, path4);
        }

        public static string Combine(params string[] paths)
        {
            return Path.Combine(paths);
        }

        public static bool IsPathRooted(string path)
        {
            return Path.IsPathRooted(path);
        }

        public static string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        public static string GetFullPath(string path)
        {
            return Path.GetFullPath(path);
        }

        public static string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
        }

        public static string GetPathRoot(string path)
        {
            return Path.GetPathRoot(path);
        }

        public static string GetExtension(string path)
        {
            return Path.GetExtension(path);
        }

        public static bool HasExtension(string path)
        {
            return Path.HasExtension(path);
        }

        public static string GetFileNameWithoutExtension(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        public static string ChangeExtension(string filename, string extension)
        {
            return Path.ChangeExtension(filename, extension);
        }
    }
}
