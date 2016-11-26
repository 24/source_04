using pb;
using pb.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace oxml
{
    public class OXml
    {
        private CompressManager _compressManager = null;

        public OXml()
        {
            _compressManager = new CompressManager(new ZipManager());
        }

        public void Uncompress(string docxFile, string directory, bool desactivateXmlFormat = false)
        {
            // - case 1 : extract all files in directory with directory structure
            if (zDirectory.Exists(directory))
                directory = zdir.GetNewDirectory(directory);
            //Action<string> doAfterUncompress = null;
            //if (!desactivateXmlFormat)
            //    doAfterUncompress = FormatXmlFile;
            IEnumerable<string> files = _compressManager.Uncompress(docxFile, directory, uncompressOptions: UncompressOptions.ExtractFullPath);
            if (!desactivateXmlFormat)
                FormatXmlFiles(files);
        }

        public void Uncompress(string docxFile, string directory, IEnumerable<string> selectFiles, bool desactivateXmlFormat = false)
        {
            //Action<string> doAfterUncompress = null;
            //if (!desactivateXmlFormat)
            //    doAfterUncompress = FormatXmlFile;
            IEnumerable<string> files = _compressManager.Uncompress(docxFile, directory, selectFiles, UncompressOptions.RenameExistingFile);
            if (!desactivateXmlFormat)
                FormatXmlFiles(files);
        }

        // Uncompress and rename files : extract CompressFile.CompressedFile and rename to CompressFile.File, only filename of CompressFile.File is used no path
        public void Uncompress(string docxFile, string directory, IEnumerable<CompressFile> selectFiles, bool desactivateXmlFormat = false)
        {
            IEnumerable<string> files = _compressManager.Uncompress(docxFile, directory, selectFiles, UncompressOptions.RenameExistingFile);
            if (!desactivateXmlFormat)
                FormatXmlFiles(files);
        }

        //public void Uncompress(string docxFile, string directory = null, IEnumerable<string> selectedFiles = null, bool renameFiles = false, bool desactivateXmlFormat = false)
        //{
        //    if (directory == null)
        //        directory = docxFile + ".zip";
        //    if (zDirectory.Exists(directory))
        //        directory = zdir.GetNewDirectory(directory);
        //    //UncompressResult result = _compressManager.UncompressMultiple(docxFile, directory, selectedFiles, UncompressOptions.ExtractFullPath, ".zip");
        //    IEnumerable<string> files = _compressManager.Uncompress(docxFile, directory, selectedFiles, UncompressOptions.ExtractFullPath);
        //    if (!desactivateXmlFormat)
        //        FormatXmlFiles(files);
        //    if (renameFiles)
        //    {
        //        RenameFiles(docxFile, files);
        //        zdir.DeleteEmptyDirectory(directory, recurse: true);
        //    }
        //}

        public void Compress(string docxFile, string directory)
        {
            if (!zPath.IsPathRooted(directory))
                directory = zPath.Combine(zDirectory.GetCurrentDirectory(), directory);
            //FileInfo
            //EnumDirectoryInfo
            //DirectoryInfo
            _compressManager.Compress(docxFile, zDirectory.EnumerateFiles(directory, "*.*", options: SearchOption.AllDirectories), FileMode.OpenOrCreate, CompressOptions.StorePartialPath, directory);
        }

        public void Compress(string docxFile, IEnumerable<CompressFile> files)
        {
            _compressManager.Compress(docxFile, files, FileMode.OpenOrCreate, CompressOptions.StorePartialPath);
        }

        private static void FormatXmlFiles(IEnumerable<string> files)
        {
            foreach (string file in files)
            {
                //Trace.WriteLine($"format xml file \"{file}\"");
                string ext = zPath.GetExtension(file).ToLower();
                if (ext == ".rels" || ext == ".xml")
                    XDocument.Load(file).Save(file);
            }
        }

        private static void FormatXmlFile(string file)
        {
            //Trace.WriteLine($"format xml file \"{file}\"");
            string ext = zPath.GetExtension(file).ToLower();
            if (ext == ".rels" || ext == ".xml")
                XDocument.Load(file).Save(file);
        }

        //private static void RenameFiles(string docxFile, IEnumerable<string> files)
        //{
        //    string baseFile = zPath.Combine(zPath.GetDirectoryName(docxFile), zPath.GetFileNameWithoutExtension(docxFile)) + ".";
        //    foreach (string file in files)
        //    {
        //        string newFile = baseFile + zPath.GetFileName(file);
        //        if (zFile.Exists(newFile))
        //            throw new PBException($"file already exist \"{newFile}\"");
        //        zFile.Move(file, newFile);
        //    }
        //}
    }
}
