using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Text;
using File = Pri.LongPath.File;
using FileInfo = Pri.LongPath.FileInfo;

namespace pb.IO
{
    public static class zFile
    {
        public static StreamReader OpenText(string path)
        {
            return File.OpenText(path);
        }

        public static StreamWriter CreateText(string path)
        {
            return File.CreateText(path);
        }

        public static void Copy(string sourcePath, string destinationPath)
        {
            File.Copy(sourcePath, destinationPath);
        }

        public static void Copy(string sourcePath, string destinationPath, bool overwrite)
        {
            File.Copy(sourcePath, destinationPath, overwrite);
        }

        public static FileStream Create(string path)
        {
            return File.Create(path);
        }

        public static FileStream Create(string path, int bufferSize)
        {
            return File.Create(path, bufferSize);
        }

        public static FileStream Create(string path, int bufferSize, FileOptions options)
        {
            return File.Create(path, bufferSize, options);
        }

        public static FileStream Create(string path, int bufferSize, FileOptions options, FileSecurity fileSecurity)
        {
            return File.Create(path, bufferSize, options, fileSecurity);
        }

        public static void Delete(string path)
        {
            File.Delete(path);
        }

        public static void Decrypt(string path)
        {
            File.Decrypt(path);
        }

        public static void Encrypt(string path)
        {
            File.Encrypt(path);
        }

        public static bool Exists(string path)
        {
            return File.Exists(path);
        }

        public static FileStream Open(string path, FileMode mode)
        {
            return File.Open(path, mode);
        }

        public static FileStream Open(string path, FileMode mode, FileAccess access)
        {
            return File.Open(path, mode, access);
        }

        public static FileStream Open(string path, FileMode mode, FileAccess access, FileShare share)
        {
            return File.Open(path, mode, access, share);
        }

        public static FileStream Open(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize)
        {
            return File.Open(path, mode, access, share, bufferSize);
        }

        public static void SetCreationTime(string path, DateTime creationTime)
        {
            File.SetCreationTime(path, creationTime);
        }

        public static void SetCreationTimeUtc(string path, DateTime creationTimeUtc)
        {
            File.SetCreationTimeUtc(path, creationTimeUtc);
        }

        public static DateTime GetCreationTime(string path)
        {
            return File.GetCreationTime(path);
        }

        public static DateTime GetCreationTimeUtc(string path)
        {
            return File.GetCreationTimeUtc(path);
        }

        public static void SetLastWriteTime(string path, DateTime lastWriteTime)
        {
            File.SetLastWriteTime(path, lastWriteTime);
        }

        public static void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc)
        {
            File.SetLastWriteTimeUtc(path, lastWriteTimeUtc);
        }

        public static DateTime GetLastWriteTime(string path)
        {
            return File.GetLastWriteTime(path);
        }

        public static DateTime GetLastWriteTimeUtc(string path)
        {
            return File.GetLastWriteTimeUtc(path);
        }

        public static void SetLastAccessTime(string path, DateTime lastAccessTime)
        {
            File.SetLastAccessTime(path, lastAccessTime);
        }

        public static void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc)
        {
            File.SetLastAccessTimeUtc(path, lastAccessTimeUtc);
        }

        public static DateTime GetLastAccessTime(string path)
        {
            return File.GetLastAccessTime(path);
        }

        public static DateTime GetLastAccessTimeUtc(string path)
        {
            return File.GetLastAccessTimeUtc(path);
        }

        public static FileAttributes GetAttributes(string path)
        {
            return File.GetAttributes(path);
        }

        public static void SetAttributes(string path, FileAttributes fileAttributes)
        {
            File.SetAttributes(path, fileAttributes);
        }

        public static FileStream OpenRead(string path)
        {
            return File.OpenRead(path);
        }

        public static FileStream OpenWrite(string path)
        {
            return File.OpenWrite(path);
        }

        public static string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        public static string ReadAllText(string path, Encoding encoding)
        {
            return File.ReadAllText(path, encoding);
        }

        public static void WriteAllText(string path, string contents)
        {
            File.WriteAllText(path, contents);
        }

        public static void WriteAllText(string path, string contents, Encoding encoding)
        {
            File.WriteAllText(path, contents, encoding);
        }

        public static byte[] ReadAllBytes(string path)
        {
            return File.ReadAllBytes(path);
        }

        public static void WriteAllBytes(string path, byte[] bytes)
        {
            File.WriteAllBytes(path, bytes);
        }

        public static string[] ReadAllLines(string path)
        {
            return File.ReadAllLines(path);
        }

        public static string[] ReadAllLines(string path, Encoding encoding)
        {
            return File.ReadAllLines(path, encoding);
        }

        public static IEnumerable<string> ReadLines(string path)
        {
            return File.ReadLines(path);
        }

        public static IEnumerable<string> ReadLines(string path, Encoding encoding)
        {
            return File.ReadLines(path, encoding);
        }

        public static void WriteAllLines(string path, string[] contents)
        {
            File.WriteAllLines(path, contents);
        }

        public static void WriteAllLines(string path, string[] contents, Encoding encoding)
        {
            File.WriteAllLines(path, contents, encoding);
        }

        public static void WriteAllLines(string path, IEnumerable<string> contents)
        {
            File.WriteAllLines(path, contents);
        }

        public static void WriteAllLines(string path, IEnumerable<string> contents, Encoding encoding)
        {
            File.WriteAllLines(path, contents, encoding);
        }

        public static void AppendAllText(string path, string contents)
        {
            File.AppendAllText(path, contents);
        }

        public static void AppendAllText(string path, string contents, Encoding encoding)
        {
            File.AppendAllText(path, contents, encoding);
        }

        public static void AppendAllLines(string path, IEnumerable<string> contents)
        {
            File.AppendAllLines(path, contents);
        }

        public static void AppendAllLines(string path, IEnumerable<string> contents, Encoding encoding)
        {
            File.AppendAllLines(path, contents, encoding);
        }

        public static void Move(string sourcePath, string destinationPath)
        {
            File.Move(sourcePath, destinationPath);
        }

        public static void Replace(string sourcePath, string destinationPath, string destinationBackupPath)
        {
            File.Replace(sourcePath, destinationPath, destinationBackupPath);
        }

        public static void Replace(string sourcePath, string destinationPath, string destinationBackupPath, bool ignoreMetadataErrors)
        {
            File.Replace(sourcePath, destinationPath, destinationBackupPath, ignoreMetadataErrors);
        }

        public static void SetAccessControl(string path, FileSecurity fileSecurity)
        {
            File.SetAccessControl(path, fileSecurity);
        }

        public static FileSecurity GetAccessControl(string path)
        {
            return File.GetAccessControl(path);
        }

        public static FileSecurity GetAccessControl(string path, AccessControlSections includeSections)
        {
            return File.GetAccessControl(path, includeSections);
        }

        public static FileInfo CreateFileInfo(string path)
        {
            return new FileInfo(path);
        }
    }
}
