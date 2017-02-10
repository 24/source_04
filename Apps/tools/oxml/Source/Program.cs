using pb;
using pb.IO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace oxml
{
    // _selectDocument = false;               // -f:d   word/document.xml
    // _selectStyle = false;                  // -f:s   word/styles.xml
    // _selectStyleWE = false;                // -f:se  word/stylesWithEffects.xml
    // _selectSetting = false;                // -f:sg  word/settings.xml
    // _selectWebSetting = false;             // -f:ws  word/webSettings.xml
    // _selectHeader = false;                 // -f:h   word/header1.xml ...
    // _selectFooter = false;                 // -f:f   word/footer1.xml ...
    // _selectFooterNote = false;             // -f:fn  word/footnotes.xml
    // _selectEndNote = false;                // -f:en  word/endnotes.xml
    // _selectFontTable = false;              // -f:ft  word/fontTable.xml
    // _selectTheme = false;                  // -f:th  word/theme/theme1.xml
    // _selectDocumentRels = false;           // -f:rd  word/_rels/document.xml.rels
    // _selectContentType = false;            // -f:ct  [Content_Types].xml
    // _selectRels = false;                   // -f:re  _rels/.rels
    // _selectAppDocProps = false;            // -f:ap  docProps/app.xml
    // _selectCoreDocProps = false;           // -f:cp  docProps/core.xml

    public enum DocxFileType
    {
        Unknow = 0,
        Document,                             // -f:d   word/document.xml
        Style,                                // -f:s   word/styles.xml
        StyleWE,                              // -f:se  word/stylesWithEffects.xml
        Setting,                              // -f:sg  word/settings.xml
        WebSetting,                           // -f:ws  word/webSettings.xml
        Header,                               // -f:h   word/header1.xml ...
        Footer,                               // -f:f   word/footer1.xml ...
        FooterNote,                           // -f:fn  word/footnotes.xml
        EndNote,                              // -f:en  word/endnotes.xml
        FontTable,                            // -f:ft  word/fontTable.xml
        Theme,                                // -f:th  word/theme/theme1.xml
        DocumentRels,                         // -f:rd  word/_rels/document.xml.rels
        ContentType,                          // -f:ct  [Content_Types].xml
        Rels,                                 // -f:re  _rels/.rels
        AppDocProps,                          // -f:ap  docProps/app.xml
        CoreDocProps                          // -f:cp  docProps/core.xml
    }

    public class SelectFile
    {
        public DocxFileType Type;
        public string File;
    }

    static class Program
    {
        private static bool _help = false;
        private static bool _generate = false;                     // -z generate docx
        private static bool _extract = false;                      // -x extract docx files
        private static string _file = null;                        // docx file
        private static string _directory = null;                   // source directory to generate docx
        //private static bool _renameFiles = false;
        //private static bool _selectFiles = false;
        //private static DocxFileType _docxFileType;
        private static List<SelectFile> _selectFiles = new List<SelectFile>();
        //private static bool _selectDocument = false;               // -f:d   word/document.xml
        //private static bool _selectStyle = false;                  // -f:s   word/styles.xml
        //private static bool _selectStyleWE = false;                // -f:se  word/stylesWithEffects.xml
        //private static bool _selectSetting = false;                // -f:sg  word/settings.xml
        //private static bool _selectWebSetting = false;             // -f:ws  word/webSettings.xml
        //private static bool _selectHeader = false;                 // -f:h   word/header1.xml ...
        //private static bool _selectFooter = false;                 // -f:f   word/footer1.xml ...
        //private static bool _selectFooterNote = false;             // -f:fn  word/footnotes.xml
        //private static bool _selectEndNote = false;                // -f:en  word/endnotes.xml
        //private static bool _selectFontTable = false;              // -f:ft  word/fontTable.xml
        //private static bool _selectTheme = false;                  // -f:th  word/theme/theme1.xml
        //private static bool _selectDocumentRels = false;           // -f:rd  word/_rels/document.xml.rels
        //private static bool _selectContentType = false;            // -f:ct  [Content_Types].xml
        //private static bool _selectRels = false;                   // -f:re  _rels/.rels
        //private static bool _selectAppDocProps = false;            // -f:ap  docProps/app.xml
        //private static bool _selectCoreDocProps = false;           // -f:cp  docProps/core.xml
        //private static IEnumerable<string> _selectedFiles = null;

        static void Main(string[] args)
        {
            //Trace.CurrentTrace.AddOnWrite("console", text => Console.Write(text));
            TraceManager.Current.AddTrace(Trace.Current);
            TraceManager.Current.SetViewer(text => Console.Write(text));
            try
            {
                //if (!ManageParameters(args))
                //    Help();
                //else
                if (ManageParameters(args))
                {
                    if (_extract)
                    {
                        //new OXml().Uncompress(_file, selectedFiles: _selectedFiles, renameFiles: _renameFiles);
                        if (_selectFiles.Count > 0)
                        {
                            string dir = _directory != null ? $"to directory \"{_directory}\"" : "to current directory";
                            Trace.WriteLine($"extract files from \"{_file}\" {dir} :");
                            //Trace.WriteLine($"count {_selectFiles.Count} \"{_selectFiles[0].File}\"");
                            new OXml().Uncompress(_file, _directory, _selectFiles.Select(selectFile =>
                            {
                                string file = selectFile.File;
                                if (file == null)
                                    file = GetSpecificDocxFile(selectFile.Type);
                                string compressedFile = GetDocxFile(selectFile.Type);
                                Trace.WriteLine($"  \"{compressedFile}\" as \"{file}\"");
                                //return selectFile.File;
                                return new CompressFile { CompressedFile = GetDocxFile(selectFile.Type), File = file };
                            }));
                        }
                        else
                        {
                            if (_directory == null)
                                _directory = _file + ".zip";
                            Trace.WriteLine($"extract all files from \"{_file}\" to \"{_directory}\"");
                            new OXml().Uncompress(_file, _directory);
                        }
                    }
                    else if (_generate)
                    {
                        if (_directory != null)
                        {
                            Trace.WriteLine($"generate (update) \"{_file}\" from directory \"{_directory}\"");
                            new OXml().Compress(_file, _directory);
                        }
                        else // if (_selectFiles.Count > 0)
                        {
                            Trace.WriteLine($"generate (update) \"{_file}\" from files :");
                            new OXml().Compress(_file, GetCompressFiles(_selectFiles));
                        }
                    }
                    else if (_help)
                        Help();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteError(ex);
            }
        }

        private static IEnumerable<CompressFile> GetCompressFiles(IEnumerable<SelectFile> selectFiles)
        {
            // used to Compress list of file
            foreach (SelectFile selectFile in selectFiles)
            {
                string compressedFile = GetDocxFile(selectFile.Type);
                string file = selectFile.File;
                if (file == null)
                    file = GetSpecificDocxFile(selectFile.Type);
                Trace.WriteLine($"  \"{file}\" as \"{compressedFile}\"");
                yield return new CompressFile { File = file, CompressedFile = compressedFile };
            }
        }

        private static bool ManageParameters(string[] args)
        {
            if (args.Length == 0)
            {
                _help = true;
                return true;
            }
            foreach (string arg in args)
            {
                if (arg.StartsWith("-"))
                {
                    //if (_docxFileType != DocxFileType.Unknow)
                    //{
                    //    //string file = GetDocxFile(_docxFileType);
                    //    //if (_extract)
                    //    //    file = zPath.GetFileName(file);
                    //    _selectFiles.Add(new SelectFile { Type = _docxFileType });
                    //    _docxFileType = DocxFileType.Unknow;
                    //}
                    if (!SetOption(arg))
                        return false;
                }
                else
                {
                    //if (_docxFileType != DocxFileType.Unknow)
                    //{
                    //    _selectFiles.Add(new SelectFile { Type = _docxFileType, File = arg });
                    //    _docxFileType = DocxFileType.Unknow;
                    //}
                    //else if (_file == null)
                    if (_file == null)
                        _file = arg;
                    else if (_directory == null && (_extract || (_generate && _selectFiles.Count == 0)))
                        _directory = arg;
                    else
                    {
                        Trace.WriteLine($"syntax error \"{arg}\"");
                        return false;
                    }
                }
            }

            //if (_docxFileType != DocxFileType.Unknow)
            //{
            //    _selectFiles.Add(new SelectFile { Type = _docxFileType });
            //    _docxFileType = DocxFileType.Unknow;
            //}

            if (!_extract && !_generate)
            {
                Trace.WriteLine($"error missing -x or -z option");
                return false;
            }
            if (_file == null)
            {
                Trace.WriteLine("no file specified");
                return false;
            }

            if (_generate)
            {
                if (_directory == null && _selectFiles.Count == 0)
                {
                    Trace.WriteLine($"error missing directory or files to generate docx");
                    return false;
                }
                if (_directory != null && _selectFiles.Count > 0)
                {
                    Trace.WriteLine($"error choose directory or files to generate docx");
                    return false;
                }
            }

            return true;
        }

        //private static bool ManageParameters_v1(string[] args)
        //{
        //    if (args.Length == 0)
        //    {
        //        _help = true;
        //        return true;
        //    }
        //    foreach (string arg in args)
        //    {
        //        if (arg.StartsWith("-"))
        //        {
        //            if (_docxFileType != DocxFileType.Unknow)
        //            {
        //                //string file = GetDocxFile(_docxFileType);
        //                //if (_extract)
        //                //    file = zPath.GetFileName(file);
        //                _selectFiles.Add(new SelectFile { Type = _docxFileType });
        //                _docxFileType = DocxFileType.Unknow;
        //            }
        //            if (!SetOption(arg))
        //                return false;
        //        }
        //        else
        //        {
        //            if (_docxFileType != DocxFileType.Unknow)
        //            {
        //                _selectFiles.Add(new SelectFile { Type = _docxFileType, File = arg });
        //                _docxFileType = DocxFileType.Unknow;
        //            }
        //            else if (_file == null)
        //                _file = arg;
        //            else if (_directory == null && (_extract || (_generate && _selectFiles.Count == 0)))
        //                _directory = arg;
        //            else
        //            {
        //                Trace.WriteLine($"syntax error \"{arg}\"");
        //                return false;
        //            }
        //        }
        //    }

        //    if (_docxFileType != DocxFileType.Unknow)
        //    {
        //        //string file = GetDocxFile(_docxFileType);
        //        //if (_extract)
        //        //    file = zPath.GetFileName(file);
        //        _selectFiles.Add(new SelectFile { Type = _docxFileType });
        //        _docxFileType = DocxFileType.Unknow;
        //    }

        //    if (!_extract && !_generate)
        //    {
        //        Trace.WriteLine($"error missing -x or -z option");
        //        return false;
        //    }
        //    if (_file == null)
        //    {
        //        Trace.WriteLine("no file specified");
        //        return false;
        //    }

        //    if (_generate)
        //    {
        //        if (_directory == null && _selectFiles.Count == 0)
        //        {
        //            Trace.WriteLine($"error missing directory or files to generate docx");
        //            return false;
        //        }
        //        if (_directory != null && _selectFiles.Count > 0)
        //        {
        //            Trace.WriteLine($"error choose directory or files to generate docx");
        //            return false;
        //        }
        //    }

        //    return true;
        //}

        private static bool SetOption(string option)
        {
            option = option.ToLower();
            // -x extract docx files
            if (option == "-x")
            {
                if (_generate)
                {
                    Trace.WriteLine($"error -x and -z cannot be used at same time");
                    return false;
                }
                _extract = true;
            }
            // -z generate docx
            else if (option == "-z")
            {
                if (_extract)
                {
                    Trace.WriteLine($"error -x and -z cannot be used at same time");
                    return false;
                }
                _generate = true;
            }
            // -f:name[=file]
            else if (option.StartsWith("-f:"))
            {
                int i = option.IndexOf('=');
                string fileType;
                if (i == -1)
                    fileType = option.Substring(3);
                else
                    fileType = option.Substring(3, i - 3);
                DocxFileType docxFileType = GetDocxFileType(fileType);
                //_docxFileType = GetDocxFileType(option.Substring(3));
                if (docxFileType == DocxFileType.Unknow)
                {
                    Trace.WriteLine($"unknow value for option \"{option}\"");
                    return false;
                }
                string file = null;
                if (i != -1)
                    file = option.Substring(i + 1);
                _selectFiles.Add(new SelectFile { Type = docxFileType, File = file });
            }
            else
            {
                Trace.WriteLine($"unknow option \"{option}\"");
                return false;
            }
            return true;
        }

        private static DocxFileType GetDocxFileType(string option)
        {
            switch (option)
            {
                case "d":
                    return DocxFileType.Document;
                case "s":
                    return DocxFileType.Style;
                case "se":
                    return DocxFileType.StyleWE;
                case "sg":
                    return DocxFileType.Setting;
                case "ws":
                    return DocxFileType.WebSetting;
                case "h":
                    return DocxFileType.Header;
                case "f":
                    return DocxFileType.Footer;
                case "fn":
                    return DocxFileType.FooterNote;
                case "en":
                    return DocxFileType.EndNote;
                case "ft":
                    return DocxFileType.FontTable;
                case "th":
                    return DocxFileType.Theme;
                case "rd":
                    return DocxFileType.DocumentRels;
                case "ct":
                    return DocxFileType.ContentType;
                case "re":
                    return DocxFileType.Rels;
                case "ap":
                    return DocxFileType.AppDocProps;
                case "cp":
                    return DocxFileType.CoreDocProps;
                default:
                    return DocxFileType.Unknow;
            }
        }

        // add docx file name to docx file
        private static string GetSpecificDocxFile(DocxFileType docxFileType)
        {
            return zPath.GetFileNameWithoutExtension(_file) + "." + zPath.GetFileName(GetDocxFile(docxFileType));
        }

        private static string GetDocxFile(DocxFileType docxFileType)
        {
            switch (docxFileType)
            {
                case DocxFileType.Document:
                    return "word/document.xml";
                case DocxFileType.Style:
                    return "word/styles.xml";
                case DocxFileType.StyleWE:
                    return "word/stylesWithEffects.xml";
                case DocxFileType.Setting:
                    return "word/settings.xml";
                case DocxFileType.WebSetting:
                    return "word/webSettings.xml";
                case DocxFileType.Header:
                    return "word/header1.xml";
                case DocxFileType.Footer:
                    return "word/footer1.xml";
                case DocxFileType.FooterNote:
                    return "word/footnotes.xml";
                case DocxFileType.EndNote:
                    return "word/endnotes.xml";
                case DocxFileType.FontTable:
                    return "word/fontTable.xml";
                case DocxFileType.Theme:
                    return "word/theme/theme1.xml";
                case DocxFileType.DocumentRels:
                    return "word/document.xml.rels";
                case DocxFileType.ContentType:
                    return "[Content_Types].xml";
                case DocxFileType.Rels:
                    return ".rels";
                case DocxFileType.AppDocProps:
                    return "docProps/app.xml";
                case DocxFileType.CoreDocProps:
                    return "docProps/core.xml";
                default:
                    throw new PBException($"unknow DocxFileType {docxFileType}");
            }
        }

        private static void Help()
        {
            // -x extract
            // -z zip
            // -z docx_file directory
            // -z docx_file -f:d file1 -f:s file2 ...
            Trace.WriteLine("oxml.exe extract content of docx or generate docx");
            Trace.WriteLine("oxml -x [-f:name ...] docx_file [directory]");
            Trace.WriteLine("  extract docx files");
            Trace.WriteLine("oxml -z docx_file directory");
            Trace.WriteLine("  generate docx or update existing docx from directory");
            Trace.WriteLine("oxml -z docx_file -f:name file1 -f:name file2 ...");
            Trace.WriteLine("  generate docx or update existing docx from files");
            Trace.WriteLine("-f:name :");
            Trace.WriteLine("  -f:d[=file]   word/document.xml");
            Trace.WriteLine("  -f:s[=file]   word/styles.xml");
            Trace.WriteLine("  -f:se[=file]  word/stylesWithEffects.xml");
            Trace.WriteLine("  -f:st[=file]  word/settings.xml");
            Trace.WriteLine("  -f:ws[=file]  word/webSettings.xml");
            Trace.WriteLine("  -f:h[=file]   word/header1.xml ...");
            Trace.WriteLine("  -f:f[=file]   word/footer1.xml ...");
            Trace.WriteLine("  -f:fn[=file]  word/footnotes.xml");
            Trace.WriteLine("  -f:en[=file]  word/endnotes.xml");
            Trace.WriteLine("  -f:ft[=file]  word/fontTable.xml");
            Trace.WriteLine("  -f:th[=file]  word/theme/theme1.xml");
            Trace.WriteLine("  -f:rd[=file]  word/_rels/document.xml.rels");
            Trace.WriteLine("  -f:ct[=file]  [Content_Types].xml");
            Trace.WriteLine("  -f:re[=file]  _rels/.rels");
            Trace.WriteLine("  -f:ap[=file]  docProps/app.xml");
            Trace.WriteLine("  -f:cp[=file]  docProps/core.xml");
        }

        //private static IEnumerable<string> GetSelectedFiles()
        //{
        //    if (_selectDocument)
        //        yield return "word/document.xml";
        //    if (_selectStyle)
        //        yield return "word/styles.xml";
        //    if (_selectStyleWE)
        //        yield return "word/stylesWithEffects.xml";
        //    if (_selectSetting)
        //        yield return "word/settings.xml";
        //    if (_selectWebSetting)
        //        yield return "word/webSettings.xml";
        //    if (_selectStyleWE)
        //        yield return "word/stylesWithEffects.xml";
        //    if (_selectHeader)
        //    {
        //        yield return "word/header1.xml";
        //        yield return "word/header2.xml";
        //        yield return "word/header3.xml";
        //    }
        //    if (_selectFooter)
        //    {
        //        yield return "word/footer1.xml";
        //        yield return "word/footer2.xml";
        //        yield return "word/footer3.xml";
        //    }
        //    if (_selectFooterNote)
        //        yield return "word/footnotes.xml";
        //    if (_selectEndNote)
        //        yield return "word/endnotes.xml";
        //    if (_selectFontTable)
        //        yield return "word/fontTable.xml";
        //    if (_selectTheme)
        //        yield return "word/theme/theme1.xml";
        //    if (_selectDocumentRels)
        //        yield return "word/_rels/document.xml.rels";
        //    if (_selectContentType)
        //        yield return "[Content_Types].xml";
        //    if (_selectRels)
        //        yield return "_rels/.rels";
        //    if (_selectAppDocProps)
        //        yield return "docProps/app.xml";
        //    if (_selectCoreDocProps)
        //        yield return "docProps/core.xml";
        //}
    }
}

//foreach (char c in option.Substring(3))
//{
//switch (option.Substring(3))
//{
//    case "d":
//        _selectDocument = true;
//        break;
//    case "s":
//        _selectStyle = true;
//        break;
//    case "se":
//        _selectStyleWE = true;
//        break;
//    case "sg":
//        _selectSetting = true;
//        break;
//    case "ws":
//        _selectWebSetting = true;
//        break;
//    case "h":
//        _selectHeader = true;
//        break;
//    case "f":
//        _selectFooter = true;
//        break;
//    case "fn":
//        _selectFooterNote = true;
//        break;
//    case "en":
//        _selectEndNote = true;
//        break;
//    case "ft":
//        _selectFontTable = true;
//        break;
//    case "th":
//        _selectTheme = true;
//        break;
//    case "rd":
//        _selectDocumentRels = true;
//        break;
//    case "ct":
//        _selectContentType = true;
//        break;
//    case "re":
//        _selectRels = true;
//        break;
//    case "ap":
//        _selectAppDocProps = true;
//        break;
//    case "cp":
//        _selectCoreDocProps = true;
//        break;
//    default:
//        Trace.WriteLine($"unknow value for option -f: \"{c}\"");
//        return false;
//}
//}
