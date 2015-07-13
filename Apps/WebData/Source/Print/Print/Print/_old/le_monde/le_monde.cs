Test_RenameFilesQuotidien();
Test_RenameFilesOther();
Test_ControlLeMondePrintNumber();

Test_01();
Test_02();
Test_Regex_01();
Test_GetLeMondePrintNumber("2008-08-31"); // ok  19781
Test_GetLeMondePrintNumber("2010-03-05"); // ok  20252
Test_GetLeMondePrintNumber("2011-04-12"); // ok  20596
Test_GetLeMondePrintNumber("2011-04-30"); // ok  20612 à vérifier samedi
Test_GetLeMondePrintNumber("2011-05-01"); // ok  20613 à vérifier dimanche
Test_GetLeMondePrintNumber("2011-05-03"); // ok  20614 à vérifier mardi
Test_GetLeMondePrintNumber("2011-05-06"); // ok  20617 à vérifier
Test_GetLeMondePrintNumber("2011-05-08"); // ok  20619 à vérifier
Test_GetLeMondePrintNumber("2011-05-13"); // ok  20623 à vérifier
Test_GetLeMondePrintNumber("2011-07-31"); // ok  20691
Test_GetLeMondePrintNumber("2011-08-31"); // ok  20717
Test_GetLeMondePrintNumber("2012-01-19"); // ok  20838
Test_GetLeMondePrintNumber("2012-02-25"); // ok  20870
Test_GetLeMondePrintNumber("2012-04-11"); // ok  20909
Test_GetLeMondePrintNumber("2012-04-18"); // ok  20915
Test_GetLeMondePrintNumber("2012-04-22"); // ok  20919
Test_GetLeMondePrintNumber("2012-04-29"); // ok  20925
Test_GetLeMondePrintNumber("2012-05-02"); // ok  20926
Test_GetLeMondePrintNumber("2012-05-11"); // ok  20934
Test_GetLeMondePrintNumber("2012-05-29"); // ok  20949
Test_GetLeMondePrintNumber("2012-07-10"); // ok  20985
Test_GetLeMondePrintNumber("2012-07-19"); // ok  20993
Test_GetLeMondePrintNumber("2012-09-11"); // ok  21039

