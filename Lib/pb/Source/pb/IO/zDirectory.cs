using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using Directory = Pri.LongPath.Directory;
using DirectoryInfo = Pri.LongPath.DirectoryInfo;

namespace pb.IO
{
    public static class zDirectory
    {
        public static string GetCurrentDirectory()
        {
            return Directory.GetCurrentDirectory();
        }

        public static void Delete(string path, bool recursive)
        {
            Directory.Delete(path, recursive);
        }

        public static void Delete(string path)
        {
            Directory.Delete(path);
        }

        public static bool Exists(string path)
        {
            return Directory.Exists(path);
        }

        public static IEnumerable<string> EnumerateDirectories(string path)
        {
            return Directory.EnumerateDirectories(path);
        }

        public static IEnumerable<string> EnumerateDirectories(string path, string searchPattern)
        {
            return Directory.EnumerateDirectories(path, searchPattern);
        }

        public static IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption options)
        {
            return Directory.EnumerateDirectories(path, searchPattern, options);
        }

        public static IEnumerable<string> EnumerateFiles(string path)
        {
            return Directory.EnumerateFiles(path);
        }

        public static IEnumerable<string> EnumerateFiles(string path, string searchPattern)
        {
            return Directory.EnumerateFiles(path, searchPattern);
        }

        public static IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption options)
        {
            return Directory.EnumerateFiles(path, searchPattern, options);
        }

        public static IEnumerable<string> EnumerateFileSystemEntries(string path)
        {
            return Directory.EnumerateFileSystemEntries(path);
        }

        public static IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern)
        {
            return Directory.EnumerateFileSystemEntries(path, searchPattern);
        }

        public static IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern, SearchOption options)
        {
            return Directory.EnumerateFileSystemEntries(path, searchPattern, options);
        }

        public static void Move(string sourcePath, string destinationPath)
        {
            Directory.Move(sourcePath, destinationPath);
        }

        public static DirectoryInfo CreateDirectory(string path)
        {
            return Directory.CreateDirectory(path);
        }

        public static DirectoryInfo CreateDirectory(string path, DirectorySecurity directorySecurity)
        {
            return Directory.CreateDirectory(path, directorySecurity);
        }

		public static string[] GetDirectories(string path, string searchPattern, SearchOption searchOption)
        {
            return Directory.GetDirectories(path, searchPattern, searchOption);
        }

        public static string[] GetDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }

        public static string[] GetDirectories(string path, string searchPattern)
        {
            return Directory.GetDirectories(path, searchPattern);
        }

        public static string GetDirectoryRoot(string path)
        {
            return Directory.GetDirectoryRoot(path);
        }

        public static string[] GetFiles(string path)
        {
            return Directory.GetFiles(path);
        }

        public static string[] GetFiles(string path, string searchPattern)
        {
            return Directory.GetFiles(path, searchPattern);
        }

        public static string[] GetFiles(string path, string searchPattern, SearchOption options)
        {
            return Directory.GetFiles(path, searchPattern, options);
        }

        public static string[] GetFileSystemEntries(string path)
        {
            return Directory.GetFileSystemEntries(path);
        }

        public static string[] GetFileSystemEntries(string path, string searchPattern)
        {
            return Directory.GetFileSystemEntries(path, searchPattern);
        }

        public static string[] GetFileSystemEntries(string path, string searchPattern, SearchOption options)
        {
            return Directory.GetFileSystemEntries(path, searchPattern, options);
        }

        public static DirectoryInfo GetParent(string path)
        {
            return Directory.GetParent(path);
        }

        public static void SetCreationTimeUtc(string path, DateTime creationTimeUtc)
        {
            Directory.SetCreationTimeUtc(path, creationTimeUtc);
        }

        public static void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc)
        {
            Directory.SetLastWriteTimeUtc(path, lastWriteTimeUtc);
        }

        public static void SetLastAccessTimeUtc(string path, DateTime lastWriteTimeUtc)
        {
            Directory.SetLastAccessTimeUtc(path, lastWriteTimeUtc);
        }

        public static DirectorySecurity GetAccessControl(string path)
        {
            return Directory.GetAccessControl(path);
        }

        public static DirectorySecurity GetAccessControl(string path, AccessControlSections includeSections)
        {
            return Directory.GetAccessControl(path, includeSections);
        }

        public static DateTime GetCreationTime(string path)
        {
            return Directory.GetCreationTime(path);
        }

		public static DateTime GetCreationTimeUtc(string path)
        {
            return Directory.GetCreationTimeUtc(path);
        }

        public static DateTime GetLastAccessTime(string path)
        {
            return Directory.GetLastAccessTime(path);
        }

        public static DateTime GetLastAccessTimeUtc(string path)
        {
            return Directory.GetLastAccessTimeUtc(path);
        }

        public static DateTime GetLastWriteTime(string path)
        {
            return Directory.GetLastWriteTime(path);
        }

        public static DateTime GetLastWriteTimeUtc(string path)
        {
            return Directory.GetLastWriteTimeUtc(path);
        }

        public static string[] GetLogicalDrives()
        {
            return Directory.GetLogicalDrives();
        }

        public static void SetAccessControl(string path, DirectorySecurity directorySecurity)
        {
            Directory.SetAccessControl(path, directorySecurity);
        }

        public static void SetCreationTime(string path, DateTime creationTime)
        {
            Directory.SetCreationTime(path, creationTime);
        }

        public static void SetLastAccessTime(string path, DateTime lastAccessTime)
        {
            Directory.SetLastAccessTime(path, lastAccessTime);
        }

        public static void SetLastWriteTime(string path, DateTime lastWriteTimeUtc)
        {
            Directory.SetLastWriteTime(path, lastWriteTimeUtc);
        }

        public static void SetCurrentDirectory(string path)
        {
            Directory.SetCurrentDirectory(path);
        }

        public static DirectoryInfo CreateDirectoryInfo(string path)
        {
            return new DirectoryInfo(path);
        }
    }
}
