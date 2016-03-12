
Test_iTextSharp_f3.Test_iTextSharp_01();
Test_iTextSharp_f3.Test_ReadPdf_01(@"c:\pib\_dl\RI Martin Beuzart.pdf");
Test_iTextSharp_f3.Test_ReadPdf_01(@"c:\pib\_dl\RI Martin Beuzart_2.pdf");

Test_01();
Test_03();
Test_04();
Test_05();
Test_06();
Test_07();
Test_02();
Test_02("tata");
Test_02("tata", 22);
Test_02(a: 22);
Test_02(s: "tata");
Test_Regex_01();
Test_GetPdfText_01(@"c:\pib\dev_data\exe\pdf\Le_monde-2012-12-06-no21113.pdf");
Test_GetPdfText_02(@"c:\pib\media\print\Le monde\_quotidien\Le monde - 2012-12\Le monde - 2012-12-06 - no 21113.pdf");
Test_GetPdfText_03(@"c:\pib\media\print\Le monde\_quotidien\Le monde - 2012-12\Le monde - 2012-12-06 - no 21113.pdf");
Test_GetPdfText_04(@"c:\pib\media\print\Le monde\_quotidien\Le monde - 2012-12\Le monde - 2012-12-06 - no 21113.pdf");
Test_GetPdfText_04(@"c:\pib\media\print\.01_quotidien\Le monde\Le monde - 2013-05-21 - no 21254 _quo.pdf");
Test_GetPdfText_05(@"c:\pib\media\print\Le monde\_quotidien\Le monde - 2012-12\Le monde - 2012-12-06 - no 21113.pdf");
Test_GetPdfText_05(@"c:\pib\media\print\.01_quotidien\Le monde\Le monde - 2013-05-21 - no 21254 _quo.pdf");
Test_GetPdfText_06(@"c:\pib\media\print\Le monde\_quotidien\Le monde - 2012-12\Le monde - 2012-12-06 - no 21113.pdf");
Test_GetPdfText_07(@"c:\pib\media\print\Le monde\_quotidien\Le monde - 2012-12\Le monde - 2012-12-06 - no 21113.pdf");
Test_GetPdfText_07(@"c:\pib\dev_data\exe\pdf\Le_monde-2012-12-06-no21113.pdf");
Test_GetPdfText_08(@"c:\pib\dev_data\exe\pdf\Le_monde-2012-12-06-no21113.pdf");
Test_PdfExportDeflatedStream_02(@"c:\pib\dev_data\exe\pdf\Le_monde-2012-12-06-no21113.pdf", 59, @"c:\pib\dev_data\exe\pdf\Resources_page_1_TPL1_object_59.txt");
Test_PdfExportDeflatedStream_02(@"c:\pib\dev_data\exe\pdf\Le_monde-2012-12-06-no21113_new_test_05.pdf");
Test_PdfExportDeflatedStream(@"c:\pib\dev_data\exe\pdf\Le_monde-2012-12-06-no21113.pdf", 59, @"c:\pib\dev_data\exe\pdf\stream_page_1_TPL1_object_59.txt");
Test_PdfImportStream_01(@"c:\pib\dev_data\exe\pdf\Le_monde-2012-12-06-no21113_new.pdf", @"c:\pib\dev_data\exe\pdf\Le_monde-2012-12-06-no21113_new_test_99.pdf", @"c:\pib\dev_data\exe\pdf\stream_page_1_TPL1_object_59_99.txt");
Test_PdfUpdateStream_01(@"c:\pib\dev_data\exe\pdf\Le_monde-2012-12-06-no21113_new_test_99.pdf", 59, @"c:\pib\dev_data\exe\pdf\stream_page_1_TPL1_object_59_99.txt");

_wr.View(Directory.GetFiles("."));
_wr.View(Directory.GetFiles(".", "*.cs"));
_wr.View(Directory.GetFiles(".\\*.cs"));

