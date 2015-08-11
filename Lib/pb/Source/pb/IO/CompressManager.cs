using System;
using System.Collections.Generic;
using System.IO;
using SharpCompress.Common;
using SharpCompress.Reader;

namespace pb.IO
{
    [Flags]
    public enum UncompressOptions
    {
        None                          = 0x0000,
        ExtractFullPath               = 0x0001,
        OverrideExistingFile          = 0x0002,
        RenameExistingFile            = 0x0004,
        UncompressNestedCompressFiles = 0x0008,
        DeleteNestedCompressFiles     = 0x0010
    }

    public class UncompressFileResult
    {
        public string File;
        public bool Error;
    }

    public class UncompressResult
    {
        public string[] UncompressFiles;
        //public string[] CompressFiles;
        public UncompressFileResult[] UncompressFileResults;
    }

    public class CompressManager
    {
        // error when uncompress some zip file (bonus6.zip)
        // System.IO.DirectoryNotFoundException occurred Could not find a part of the path 'c:\...\bonus6\Encyclopedie Junior-Dot Com-8 pdf\'.
        //private CompressBaseManager _zipManager = null;
        private CompressBaseManager _sharpCompressManager = null;

        public CompressManager()
        {
            //_zipManager = new ZipManager();
            _sharpCompressManager = new SharpCompressManager();
        }

        public UncompressResult Uncompress(string file, string directory = null, UncompressOptions options = UncompressOptions.None)
        {
            bool extractFullPath = (options & UncompressOptions.ExtractFullPath) == UncompressOptions.ExtractFullPath;
            bool overrideExistingFile = (options & UncompressOptions.OverrideExistingFile) == UncompressOptions.OverrideExistingFile;
            bool renameExistingFile = (options & UncompressOptions.RenameExistingFile) == UncompressOptions.RenameExistingFile;
            bool uncompressNestedCompressFiles = (options & UncompressOptions.UncompressNestedCompressFiles) == UncompressOptions.UncompressNestedCompressFiles;
            bool deleteNestedCompressFiles = (options & UncompressOptions.DeleteNestedCompressFiles) == UncompressOptions.DeleteNestedCompressFiles;

            UncompressBaseOptions uncompressBaseOptions = UncompressBaseOptions.None;
            if (extractFullPath)
                uncompressBaseOptions |= UncompressBaseOptions.ExtractFullPath;
            if (overrideExistingFile)
                uncompressBaseOptions |= UncompressBaseOptions.OverrideExistingFile;
            if (renameExistingFile)
                uncompressBaseOptions |= UncompressBaseOptions.RenameExistingFile;

            List<string> allUncompressFiles = new List<string>();
            //List<string> allCompressFiles = new List<string>();
            List<UncompressFileResult> uncompressFileResults = new List<UncompressFileResult>();
            List<string> compressFiles = new List<string>();
            compressFiles.Add(file);
            bool nestedCompressFile = false;

            do
            {
                List<string> newCompressFiles = new List<string>();
                foreach (string compressFile in compressFiles)
                {
                    if (directory == null)
                    {
                        directory = zpath.PathSetExtension(compressFile, "");
                        if (zDirectory.Exists(directory))
                            directory = zdir.GetNewDirectory(directory);
                    }

                    CompressBaseManager compressBaseManager = GetCompressBaseManager(compressFile);

                    string[] uncompressFiles = null;
                    UncompressFileResult uncompressFileResult = new UncompressFileResult();
                    uncompressFileResult.File = compressFile;
                    try
                    {
                        uncompressFiles = compressBaseManager.Uncompress(compressFile, directory, uncompressBaseOptions);
                        uncompressFileResult.Error = false;

                        if (nestedCompressFile && deleteNestedCompressFiles)
                            zFile.Delete(compressFile);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine("error uncompress file \"{0}\"", file);
                        Trace.WriteLine(ex.Message);
                        uncompressFileResult.Error = true;
                    }

                    uncompressFileResults.Add(uncompressFileResult);

                    if (uncompressFiles != null)
                    {
                        foreach (string uncompressFile in uncompressFiles)
                        {
                            if (uncompressNestedCompressFiles && IsCompressFile(uncompressFile))
                            {
                                newCompressFiles.Add(uncompressFile);
                                //if (!deleteNestedCompressFiles)
                                //    allCompressFiles.Add(uncompressFile);
                            }
                            else
                                allUncompressFiles.Add(uncompressFile);
                        }
                    }
                    directory = null;
                }
                compressFiles = newCompressFiles;
                nestedCompressFile = true;
            } while (compressFiles.Count > 0);
            //return new UncompressResult { UncompressFiles = allUncompressFiles.ToArray(), CompressFiles = allCompressFiles.ToArray() };
            return new UncompressResult { UncompressFiles = allUncompressFiles.ToArray(), UncompressFileResults = uncompressFileResults.ToArray() };
        }

        public CompressBaseManager GetCompressBaseManager(string file)
        {
            switch (zPath.GetExtension(file).ToLower())
            {
                //case ".zip":
                //    return _zipManager;
                case ".zip":
                case ".rar":
                case ".tar":
                case ".gz":
                case ".7z":
                    return _sharpCompressManager;
                default:
                    throw new PBException("error no CompressBaseManager for file type \"{0}\"", zPath.GetExtension(file));
            }
        }

        public static bool IsCompressFile(string file)
        {
            switch (zPath.GetExtension(file).ToLower())
            {
                case ".zip":
                case ".rar":
                case ".tar":
                case ".gz":
                case ".7z":
                    return true;
                default:
                    return false;
            }
        }
    }
}
