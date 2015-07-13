Test_01();

Test_Uri_01("http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/detail.asp?id=3982");
Test_Uri_01(@"c:\pib\dev_data\exe\wrun\damien\cache\Unea\detail_company1\union-nationale-entreprises-adaptees_annuaire-unea_71_71_annuaire_detail.asp_id=3982.asp");
Test_Uri_01("file:///c:/pib/dev_data/exe/wrun/damien/cache/Unea/detail_company1/union-nationale-entreprises-adaptees_annuaire-unea_71_71_annuaire_detail.asp_id=3982.asp");

Test_WebRequest_01("http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/detail.asp?id=3982");
Test_WebRequest_01(@"c:\pib\dev_data\exe\wrun\damien\cache\Unea\detail_company1\union-nationale-entreprises-adaptees_annuaire-unea_71_71_annuaire_detail.asp_id=3982.asp");

Test_WebRequest_02("http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/detail.asp?id=3982");
Test_WebRequest_02(@"c:\pib\dev_data\exe\wrun\damien\cache\Unea\detail_company1\union-nationale-entreprises-adaptees_annuaire-unea_71_71_annuaire_detail.asp_id=3982.asp");

