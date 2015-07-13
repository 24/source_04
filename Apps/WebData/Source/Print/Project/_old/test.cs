
Trace.WriteLine("toto");

HttpRun.Load(@"c:\pib\dropbox\pbeuz\Dropbox\dev\project\Source\Source_01\Source\Download\Print\download\download_project.xml");
HtmlRun.Select("//Source");
HtmlRun.Select("//Source", "@value");
HttpRun.GetXDocument().Root.zElements("Project/Source").zView();


HttpRun.Load("http://www.vosbooks.net/74722-revues-magazines/top-sante-n294-mars-2015.html");
HtmlRun.Select("//div[@class='entry']//a", "@href");
