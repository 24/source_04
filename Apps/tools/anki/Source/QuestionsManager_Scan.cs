using pb;
using pb.Data;
using pb.Data.Pdf;
using pb.Data.Xml;
using pb.IO;
using pb.Text;
using pb.Web.Http;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace anki
{
    public partial class QuestionsManager
    {
        public async Task Scan(bool imageScan = false, string range = null, bool simulate = false)
        {
            QuestionsParameters parameters = GetParameters();
            if (parameters == null)
            {
                Trace.WriteLine($"parameters are not defined");
                return;
            }
            if (parameters.PageRange == null)
            {
                Trace.WriteLine($"page range is not defined");
                return;
            }
            if (parameters.PageColumn == 1 && !imageScan)
                await ScanPdf(parameters, range, simulate);
            else
            {
                if (!parameters.ImagesExtracted)
                    ExtractImagesFromPdf(parameters);
                await ScanImages(parameters, range, simulate);
            }
        }

        private async Task ScanPdf(QuestionsParameters parameters, string range, bool simulate = false)
        {
            string pdfFile = GetPdfFile();
            if (!zFile.Exists(pdfFile))
            {
                Trace.WriteLine($"pdf file not found \"{pdfFile}\"");
                return;
            }
            if (range == null)
                range = parameters.PageRange;
            Trace.WriteLine($"scan pdf \"{pdfFile}\" range \"{range}\"");
            //OcrWebService ocrWebService = CreateOcrWebService();
            ScanManager scanManager = CreateScanManager();
            //OcrRequest request = CreateOcrRequest();
            //request.DocumentFile = pdfFile;
            //request.PageRange = range;
            if (!simulate)
            {
                await scanManager.ScanPdf(pdfFile, range, zPath.Combine(GetScanDirectory(), "scan"));
                //OcrResult<OcrProcessDocumentResponse> response = await ocrWebService.ProcessDocument(request);
                //if (response.Success)
                //{
                //    Trace.WriteLine($"scan ok {response.Data.ProcessedPages} pages - remainder {response.Data.AvailablePages} pages");
                //    string directory = GetScanDirectory();
                //    zdir.CreateDirectory(directory);
                //    string scanFile = zPath.Combine(directory, "scan");
                //    Trace.WriteLine($"save scan to \"{scanFile}\"");
                //    await ocrWebService.DownloadResultDocuments(response.Data, scanFile);
                //}
                //else
                //    Trace.WriteLine($"scan error code {response.StatusCode}");
            }
            else
            {
                Trace.WriteLine($"simulate scan pdf \"{pdfFile}\" range \"{range}\"");
            }
        }

        private async Task ScanImages(QuestionsParameters parameters, string range, bool simulate = false)
        {
            string directory = GetImagesDirectory();
            if (!zDirectory.Exists(directory))
            {
                Trace.WriteLine($"images directory not found \"{directory}\"");
                return;
            }
            if (range == null)
                range = parameters.PageRange;
            Trace.WriteLine($"scan images \"{directory}\" range \"{range}\"");
            //OcrWebService ocrWebService = CreateOcrWebService();
            ScanManager scanManager = CreateScanManager();
            //OcrRequest request = CreateOcrRequest();
            foreach (int page in zstr.EnumRange(range))
            {
                string imageBaseFile = zPath.Combine(directory, $"page-{page:000}");
                string imageFile = FindImageFile(imageBaseFile);
                if (!zFile.Exists(imageFile))
                {
                    Trace.WriteLine($"image not found \"{imageBaseFile}\"");
                    return;
                }
                //request.DocumentFile = imageFile;
                //request.Zone = GetScanZone(parameters, imageFile);
                //if (!simulate)
                //{
                //    Trace.WriteLine($"scan image \"{imageFile}\"");
                //    await scanManager.ScanImage(imageFile, zPath.Combine(GetScanDirectory(), $"scan-page-{page:000}"));
                //    //OcrResult<OcrProcessDocumentResponse> response = await ocrWebService.ProcessDocument(request);
                //    //if (response.Success)
                //    //{
                //    //    string scanDirectory = GetScanDirectory();
                //    //    zdir.CreateDirectory(scanDirectory);
                //    //    string scanFile = zPath.Combine(scanDirectory, $"scan-page-{page:000}");
                //    //    Trace.WriteLine($"save scan to \"{scanFile}\"");
                //    //    await ocrWebService.DownloadResultDocuments(response.Data, scanFile);
                //    //}
                //}
                //else
                //{
                //    //Trace.WriteLine("OcrRequest :");
                //    //request.zTraceJson();
                //    Trace.WriteLine($"simulate scan image \"{imageFile}\"");
                //}
                foreach (ImageColumn imageColumn in GetScanImages(parameters, imageFile))
                {
                    string scanFile = zPath.Combine(GetScanDirectory(), $"scan-page-{page:000}");
                    if (imageColumn.Column != 0)
                        scanFile += $"-{imageColumn.Column:00}";
                    if (!simulate)
                    {
                        Trace.WriteLine($"scan image \"{imageColumn.ImageFile}\" to \"{scanFile}\"");
                        await scanManager.ScanImage(imageColumn.ImageFile, scanFile);
                    }
                    else
                        Trace.WriteLine($"simulate scan image \"{imageColumn.ImageFile}\" to \"{scanFile}\"");
                }
            }
        }

        private IEnumerable<ImageColumn> GetScanImages(QuestionsParameters parameters, string imageFile)
        {
            if (parameters.PageColumn == 1)
                yield return new ImageColumn { ImageFile = imageFile, Column = 0 };
            else if (parameters.PageColumn == 2)
            {
                string imageColumnFile1 = GetIndexedImageFile(imageFile, 1);
                string imageColumnFile2 = GetIndexedImageFile(imageFile, 2);
                if (!zFile.Exists(imageColumnFile1) || !zFile.Exists(imageColumnFile2))
                {
                    CreateImagesColumn(imageFile, parameters.PageRotate, imageColumnFile1, imageColumnFile2);
                }

                if (_joinImagesColumn)
                {
                    string joinedImageFile = GetIndexedImageFile(imageFile, 3);
                    if (!zFile.Exists(joinedImageFile))
                    {
                        CreateJoinedImageColumn(imageColumnFile1, imageColumnFile2, joinedImageFile);
                    }
                    yield return new ImageColumn { ImageFile = joinedImageFile, Column = 0 };
                }
                else
                {
                    yield return new ImageColumn { ImageFile = imageColumnFile1, Column = 1 };
                    yield return new ImageColumn { ImageFile = imageColumnFile2, Column = 2 };
                }
            }
            else
                throw new PBException($"wrong page column {parameters.PageColumn}");
        }

        private void CreateJoinedImageColumn(string imageColumnFile1, string imageColumnFile2, string joinedImageFile)
        {
            Trace.WriteLine($"create joined image column \"{joinedImageFile}\"");
            using (Image image1 = Image.FromFile(imageColumnFile1))
            using (Image image2 = Image.FromFile(imageColumnFile2))
            {
                zimg.JoinBottom(image1, image2).Save(joinedImageFile, image1.RawFormat);
            }
        }

        private void CreateImagesColumn(string imageFile, PageRotate pageRotate, string imageColumnFile1, string imageColumnFile2)
        {
            using (Image image = Image.FromFile(imageFile))
            {
                int width = image.Width;
                int height = image.Height;
                Rectangle column1;
                Rectangle column2;
                RotateFlipType? rotate = null;
                if (pageRotate == PageRotate.NoRotate || pageRotate == PageRotate.Rotate180)
                {
                    column1 = new Rectangle(0, 0, width / 2, height);
                    column2 = new Rectangle(width / 2, 0, width - (width / 2), height);
                    if (pageRotate == PageRotate.Rotate180)
                        rotate = RotateFlipType.Rotate180FlipNone;
                }
                else // if (parameters.PageRotate == PageRotate.Rotate90 || parameters.PageRotate == PageRotate.Rotate270)
                {
                    column1 = new Rectangle(0, height / 2, width, height - (height / 2));
                    column2 = new Rectangle(0, 0, width, height / 2);
                    if (pageRotate == PageRotate.Rotate90)
                        rotate = RotateFlipType.Rotate90FlipNone;
                    else
                        rotate = RotateFlipType.Rotate270FlipNone;
                }

                Trace.WriteLine($"create image column \"{imageColumnFile1}\"");
                using (Bitmap bitmap = zimg.Crop(image, column1))
                {
                    if (rotate != null)
                        bitmap.RotateFlip((RotateFlipType)rotate);
                    bitmap.Save(imageColumnFile1, image.RawFormat);
                }

                Trace.WriteLine($"create image column \"{imageColumnFile2}\"");
                using (Bitmap bitmap = zimg.Crop(image, column2))
                {
                    if (rotate != null)
                        bitmap.RotateFlip((RotateFlipType)rotate);
                    bitmap.Save(imageColumnFile2, image.RawFormat);
                }
            }
        }

        private static string GetIndexedImageFile(string imageFile, int index)
        {
            return zPath.Combine(zPath.GetDirectoryName(imageFile), zPath.GetFileNameWithoutExtension(imageFile) + $"-{index:00}" + zPath.GetExtension(imageFile));
        }

        public void ExtractImagesFromPdf()
        {
            QuestionsParameters parameters = GetParameters();
            if (parameters == null)
            {
                Trace.WriteLine($"parameters are not defined");
                return;
            }
            if (parameters.PageRange == null)
            {
                Trace.WriteLine($"page range is not defined");
                return;
            }
            ExtractImagesFromPdf(parameters);
        }

        private void ExtractImagesFromPdf(QuestionsParameters parameters)
        {
            string pdfFile = GetPdfFile();
            //if (range == null)
            //    range = parameters.PageRange;
            Trace.WriteLine($"extract images from pdf \"{pdfFile}\" range \"{parameters.PageRange}\"");
            string directory = GetImagesDirectory();
            zdir.CreateDirectory(directory);
            //zfile.DeleteFiles(directory, "*.jpeg");
            DeleteImageFiles(directory);
            //iText.ExtractImages(pdfFile,
            //    (image, page, imageIndex) => image.Save(zPath.Combine(directory, $"page-{page:000}{(imageIndex != 1 ? $"-{imageIndex:00}" : "")}.jpeg"), ImageFormat.Jpeg),
            //    range: range);

            foreach (PdfImage image in iText7.EnumImages(pdfFile, parameters.PageRange))
            {
                iText7.SaveImage(image.Image, zPath.Combine(directory, $"page-{image.PageNumber:000}{(image.ImageNumber != 1 ? $"-{image.ImageNumber:00}" : "")}"));
            }
            parameters.ImagesExtracted = true;
            SetParameters(parameters);
        }

        private static string FindImageFile(string file)
        {
            foreach (string extension in _imageExtensions)
            {
                string file2 = file + extension;
                if (zFile.Exists(file2))
                    return file2;
            }
            return null;
        }

        private static void DeleteImageFiles(string directory)
        {
            foreach (string extension in _imageExtensions)
                zfile.DeleteFiles(directory, "*" + extension);
        }

        //private static string GetScanZone(QuestionsParameters parameters, string imageFile)
        //{
        //    // format: "top:left:height:width,...", example "zone=0:0:100:100,50:50:50:50"
        //    if (parameters.PageColumn == 1)
        //        return null;
        //    else if (parameters.PageColumn == 2)
        //    {
        //        int width;
        //        int height;
        //        zimg.GetImageWidthHeight(imageFile, out width, out height);
        //        string zone;
        //        switch (parameters.PageRotate)
        //        {
        //            case PageRotate.NoRotate:
        //            case PageRotate.Rotate180:
        //                int width2 = width / 2;
        //                zone = $"0:0:{width2}:{height},{width2}:0:{width - width2}:{height}";
        //                break;
        //            case PageRotate.Rotate90:
        //            case PageRotate.Rotate270:
        //                int height2 = height / 2;
        //                zone = $"0:0:{width}:{height2},0:{height2}:{width}:{height - height2}";
        //                break;
        //            default:
        //                throw new PBException($"unknow page rotation {parameters.PageRotate}");
        //        }
        //        return zone;
        //    }
        //    else
        //        throw new PBException($"can't create scan zone for {parameters.PageColumn} columns");
        //}

        //private OcrRequest CreateOcrRequest()
        //{
        //    return new OcrRequest { Language = "french,english", OutputFormat = "txt" };
        //}

        //private OcrWebService CreateOcrWebService()
        //{
        //    XmlConfig config = XmlConfig.CurrentConfig;
        //    XmlConfig ocrWebServiceConfig = config.GetConfig("OcrWebServiceConfig");
        //    OcrWebService ocrWebService = new OcrWebService(ocrWebServiceConfig.GetExplicit("UserName"), ocrWebServiceConfig.GetExplicit("LicenseCode"), _timeout);
        //    //ocrWebService.UserName = ocrWebServiceConfig.GetExplicit("UserName");
        //    //ocrWebService.LicenseCode = ocrWebServiceConfig.GetExplicit("LicenseCode");
        //    string cacheDirectory = config.Get("OcrWebServiceCacheDirectory");
        //    if (cacheDirectory != null)
        //    {
        //        UrlCache urlCache = new UrlCache(cacheDirectory);
        //        urlCache.UrlFileNameType = UrlFileNameType.Host | UrlFileNameType.Path;
        //        if (config.Get("OcrWebServiceCacheDirectory/@option")?.ToLower() == "indexedfile")
        //            urlCache.IndexedFile = true;
        //        ocrWebService.HttpManager.SetCacheManager(urlCache);
        //    }
        //    return ocrWebService;
        //}

        private ScanManager CreateScanManager()
        {
            XmlConfig config = XmlConfig.CurrentConfig;
            XmlConfig ocrWebServiceConfig = config.GetConfig("OcrWebServiceConfig");
            UrlCache urlCache = null;
            string cacheDirectory = config.Get("OcrWebServiceCacheDirectory");
            if (cacheDirectory != null)
            {
                urlCache = new UrlCache(cacheDirectory);
                urlCache.UrlFileNameType = UrlFileNameType.Host | UrlFileNameType.Path;
                if (config.Get("OcrWebServiceCacheDirectory/@option")?.ToLower() == "indexedfile")
                    urlCache.IndexedFile = true;
            }
            ScanManager scanManager = new ScanManager(ocrWebServiceConfig.GetExplicit("UserName"), ocrWebServiceConfig.GetExplicit("LicenseCode"), urlCache);
            scanManager.Language = "french,english";
            scanManager.OutputFormat = "txt";
            return scanManager;
        }

        private Regex _pageNumberScanFile = new Regex("-page-[0-9]{3}.txt$", RegexOptions.Compiled);
        public void RenameAndCopyScanFiles(bool simulate = false)
        {
            string directory = GetScanDirectory();
            if (!zDirectory.Exists(directory))
                throw new PBException($"scan directory not found (\"{directory}\")");
            //string name = zPath.GetFileName(_directory);
            string name = "scan";
            foreach (string file in zDirectory.EnumerateFiles(directory))
            {
                string filename = zPath.GetFileName(file);
                //Match match1 = _numericHeaderScanFile.Match(filename);
                //if (match1.Success)
                //{
                //Trace.WriteLine($"file \"{filename}\"");
                Match match2 = _pageNumberScanFile.Match(filename);
                if (match2.Success)
                {
                    //string file2 = zPath.Combine(directory, match1.zReplace(filename, ""));
                    string file2 = zPath.Combine(directory, name + match2.Value);

                    //if (file == file2)
                    //    continue;

                    if (file != file2)
                    {
                        if (zFile.Exists(file2))
                            throw new PBException($"can't rename file \"{zPath.GetFileName(file)}\" file already exists \"{zPath.GetFileName(file2)}\"");
                        Trace.WriteLine($"{(simulate ? "simulate " : "")}rename file \"{zPath.GetFileName(file)}\" to \"{zPath.GetFileName(file2)}\"");
                        if (!simulate)
                            zFile.Move(file, file2);
                    }

                    string file3 = zPath.Combine(directory, zPath.GetFileNameWithoutExtension(file2) + "_02" + zPath.GetExtension(file2));

                    // check if file_02 exists
                    string file4 = zPath.Combine(directory, zPath.GetFileNameWithoutExtension(file) + "_02" + zPath.GetExtension(file));
                    if (zFile.Exists(file4))
                    {
                        if (file4 != file3)
                        {
                            Trace.WriteLine($"{(simulate ? "simulate " : "")}rename file \"{zPath.GetFileName(file4)}\" to \"{zPath.GetFileName(file3)}\"");
                            if (!simulate)
                                zFile.Move(file4, file3);
                        }
                    }
                    else
                    {
                        // copy file to *_02.txt
                        if (zFile.Exists(file3))
                            throw new PBException($"can't copy file \"{zPath.GetFileName(file2)}\" file already exists \"{zPath.GetFileName(file3)}\"");
                        Trace.WriteLine($"{(simulate ? "simulate " : "")}copy file \"{zPath.GetFileName(file2)}\" to \"{zPath.GetFileName(file3)}\"");
                        if (!simulate)
                            zFile.Copy(file2, file3);
                    }
                }
                //else
                //    Trace.WriteLine("page number not found");
                //}
            }
        }

        private string GetImagesDirectory()
        {
            return zPath.Combine(_directory, @"data\images");
        }
    }
}
