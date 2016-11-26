using System;
using System.Collections.Generic;

namespace pb.IO
{
    [Flags]
    public enum UncompressMultipleOptions
    {
        None                            = 0x0000,
        //ExtractFullPath                 = 0x0001,
        //OverrideExistingFile            = 0x0002,
        //RenameExistingFile              = 0x0004,
        UncompressNestedCompressFiles   = 0x0008,
        DeleteNestedCompressFiles       = 0x0010
    }

    public class UncompressFileResult
    {
        public string File;
        public bool Error;
    }

    public class UncompressMultipleResult
    {
        //public string[] UncompressFiles;
        public IEnumerable<string> UncompressFiles;
        //public string[] CompressFiles;
        //public UncompressFileResult[] UncompressFileResults;
        public IEnumerable<UncompressFileResult> UncompressFileResults;
    }

    partial class CompressManager
    {
        // Uncompress()
        // string compressFileType = null
        public UncompressMultipleResult UncompressMultiple(string file, string directory = null, IEnumerable<string> selectedFiles = null, UncompressOptions uncompressOptions = UncompressOptions.None,
            UncompressMultipleOptions multipleOptions = UncompressMultipleOptions.None)
        {
            //bool extractFullPath = (multipleOptions & UncompressMultipleOptions.ExtractFullPath) == UncompressMultipleOptions.ExtractFullPath;
            //bool overrideExistingFile = (multipleOptions & UncompressMultipleOptions.OverrideExistingFile) == UncompressMultipleOptions.OverrideExistingFile;
            //bool renameExistingFile = (multipleOptions & UncompressMultipleOptions.RenameExistingFile) == UncompressMultipleOptions.RenameExistingFile;
            bool uncompressNestedCompressFiles = (multipleOptions & UncompressMultipleOptions.UncompressNestedCompressFiles) == UncompressMultipleOptions.UncompressNestedCompressFiles;
            bool deleteNestedCompressFiles = (multipleOptions & UncompressMultipleOptions.DeleteNestedCompressFiles) == UncompressMultipleOptions.DeleteNestedCompressFiles;

            //UncompressBaseOptions uncompressBaseOptions = UncompressBaseOptions.None;
            //if (extractFullPath)
            //    uncompressBaseOptions |= UncompressBaseOptions.ExtractFullPath;
            //if (overrideExistingFile)
            //    uncompressBaseOptions |= UncompressBaseOptions.OverrideExistingFile;
            //if (renameExistingFile)
            //    uncompressBaseOptions |= UncompressBaseOptions.RenameExistingFile;

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
                        directory = zpath.PathSetExtension(compressFile, "");
                    if (zDirectory.Exists(directory))
                        directory = zdir.GetNewDirectory(directory);

                    //CompressBaseManager compressBaseManager = GetCompressBaseManager(compressFile);
                    //if (compressFileType == null)
                    //    compressFileType = zPath.GetExtension(compressFile);
                    //CompressBaseManager compressBaseManager = GetCompressBaseManager(compressFileType);
                    //compressFileType = null;

                    //string[] uncompressFiles = null;
                    IEnumerable<string> uncompressFiles = null;
                    UncompressFileResult uncompressFileResult = new UncompressFileResult();
                    uncompressFileResult.File = compressFile;
                    try
                    {
                        // uncompressBaseOptions
                        uncompressFiles = _compressManager.Uncompress(compressFile, directory, selectedFiles, uncompressOptions);
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
            //return new UncompressResult { UncompressFiles = allUncompressFiles.ToArray(), UncompressFileResults = uncompressFileResults.ToArray() };
            return new UncompressMultipleResult { UncompressFiles = allUncompressFiles, UncompressFileResults = uncompressFileResults };
        }

        //private CompressBaseManager GetCompressBaseManager(string file)
        //private CompressBaseManager GetCompressBaseManager(string fileExtension)
        //{
        //    //switch (zPath.GetExtension(file).ToLower())
        //    switch (fileExtension.ToLower())
        //    {
        //        //case ".zip":
        //        //    return _zipManager;
        //        case ".zip":
        //        case ".rar":
        //        case ".tar":
        //        case ".gz":
        //        case ".7z":
        //            return _sharpCompressManager;
        //        default:
        //            throw new PBException("error no CompressBaseManager for file type \"{0}\"", fileExtension);
        //    }
        //}
    }
}
