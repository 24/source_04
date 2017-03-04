using pb.Data.Xml;
using pb.Text;

namespace anki
{
    public class QuestionResponseFiles
    {
        public string BaseDirectory;
        public string[] QuestionFiles;
        public string ResponseFile;
    }

    public static class QuestionRun
    {
        public static RegexValuesList GetQuestionRegexValuesList()
        {
            return new RegexValuesList(XmlConfig.CurrentConfig.GetElements("QuestionInfos/QuestionInfo"), compileRegex: true);
        }

        public static RegexValuesList GetResponseRegexValuesList()
        {
            return new RegexValuesList(XmlConfig.CurrentConfig.GetElements("ResponseInfos/ResponseInfo"), compileRegex: true);
        }

        //public static void WriteAnkiFile(string ankiFile, string questionFile, string responseFile)
        //{
        //    AnkiWriter.Write(ankiFile, QuestionReader.Read(questionFile, GetQuestionRegexValuesList()), ResponseReader.Read(responseFile, GetResponseRegexValuesList()));
        //}

        //public static void WriteAnkiCard(string name)
        //{
        //    XElement xe = XmlConfig.CurrentConfig.GetElement($"CardGames/CardGame[@name = \"{name}\"]");
        //    if (xe == null)
        //        throw new PBException($"card not found \"{name}\"");
        //    string directory = xe.zXPathExplicitValue("Directory");
        //    AnkiWriter.Write(zPath.Combine(directory, xe.zXPathExplicitValue("AnkiFile")),
        //        QuestionReader.Read(xe.zXPathValues("QuestionFile").Select(file => zPath.Combine(directory, file)), GetQuestionRegexValuesList()),
        //        ResponseReader.Read(zPath.Combine(directory, xe.zXPathExplicitValue("ResponseFile")), GetResponseRegexValuesList()));
        //}

        public static QuestionsManager CreateQuestionsManager(string directory)
        {
            return new QuestionsManager(XmlConfig.CurrentConfig.GetExplicit("CardDirectory"), directory, GetQuestionRegexValuesList(), GetResponseRegexValuesList());
        }

        //public static void Exec(string file)
        //{
        //    // file : "UE3\UE3 - Activité électrique cardiaque"
        //    string baseDirectory = XmlConfig.CurrentConfig.GetExplicit("CardDirectory");
        //    string directory = zPath.Combine(baseDirectory, file);
        //    string filename = zPath.GetFileName(file);
        //    QuestionResponseFiles questionResponseFiles = GetQuestionResponseFiles(directory, filename, baseDirectory);
        //    //TraceScanFiles(scanFiles);
        //    Response[] responses = ResponseReader.Read(questionResponseFiles.ResponseFile, GetResponseRegexValuesList()).ToArray();
        //    string responseFile = zPath.Combine(directory, filename + ".response.txt");
        //    ExportResponse(responseFile, responses);
        //    string ankiFile = zPath.Combine(directory, filename + ".anki.txt");
        //    try
        //    {
        //        //AnkiWriter.Write(ankiFile, QuestionReader.Read(scanFiles.QuestionFiles, GetQuestionRegexValuesList()), ResponseReader.Read(scanFiles.ResponseFile, GetResponseRegexValuesList()));
        //        //AnkiWriter.Write(ankiFile, QuestionResponses.GetQuestionResponses(QuestionReader.Read(questionResponseFiles.QuestionFiles, GetQuestionRegexValuesList()), ResponseReader.Read(questionResponseFiles.ResponseFile, GetResponseRegexValuesList())));

        //        pb.Trace.WriteLine($"generate anki file \"{ankiFile}\"");
        //        AnkiWriter.Write(ankiFile, GetQuestionResponses(questionResponseFiles)
        //            .Select(questionResponse => new AnkiQuestion { Question = questionResponse.GetHtml(questionNumber: true), Response = questionResponse.Response.GetFormatedResponse() }));

        //        //string htmlDirectory = zPath.Combine(directory, "html");
        //        //pb.Trace.WriteLine($"generate html files in \"{htmlDirectory}\"");
        //        // .Select(questionResponse => questionResponse.GetHtml(questionNumber: true, questionDiv: true, response: true, newLine: true))
        //        //QuestionResponses.CreateHtmlFiles(GetQuestionResponses(questionResponseFiles), htmlDirectory, GetHtmlHeader(), GetHtmlFooter());

        //        string dataDirectory = zPath.Combine(directory, "data");
        //        pb.Trace.WriteLine($"save question to \"{dataDirectory}\"");
        //        QuestionResponses.CreateQuestionFiles(GetQuestionResponses(questionResponseFiles), dataDirectory);
        //    }
        //    catch (PBFileException ex)
        //    {
        //        pb.Trace.WriteLine(ex.Message);
        //        OpenFile(GetModifiableFile(ex.File), ex.Line, ex.Column);
        //    }
        //}

        //private static string GetHtmlHeader()
        //{
        //    // warning text from xml CDATA has unix new line type \n
        //    return XmlConfig.CurrentConfig.GetExplicit("QuestionHtml/Header").Trim().Replace("\n", "\r\n");
        //}

        //private static string GetHtmlFooter()
        //{
        //    return XmlConfig.CurrentConfig.GetExplicit("QuestionHtml/Footer").Trim().Replace("\n", "\r\n");
        //}

        //IEnumerable<string> htmls
        //private static void CreateHtmlFiles(IEnumerable<QuestionResponse> questionResponses, string directory, string htmlHeader, string htmlFooter)
        //{
        //    zdir.CreateDirectory(directory);
        //    int index = 1;
        //    //foreach (string html in htmls)
        //    foreach (QuestionResponse questionResponse in questionResponses)
        //    {
        //        string html = questionResponse.GetHtml(questionNumber: true, questionDiv: true, response: true, newLine: true);
        //        string file = zPath.Combine(directory, $"question_{index:00}.html");
        //        using (StreamWriter sw = new StreamWriter(zFile.Create(file)))
        //        {
        //            sw.WriteLine(htmlHeader);
        //            sw.Write(html);
        //            sw.WriteLine(htmlFooter);
        //        }
        //        index++;
        //    }
        //}

        //private static IEnumerable<QuestionResponse> GetQuestionResponses(QuestionResponseFiles questionResponseFiles)
        //{
        //    // , string baseDirectory = null
        //    return QuestionResponses.GetQuestionResponses(QuestionReader.Read(questionResponseFiles.QuestionFiles, GetQuestionRegexValuesList(), questionResponseFiles.BaseDirectory),
        //        ResponseReader.Read(questionResponseFiles.ResponseFile, GetResponseRegexValuesList()));
        //}

        //public static string GetModifiableFile(string file)
        //{
        //    //string filename = zPath.GetFileNameWithoutExtension(file);
        //    //if (!filename.EndsWith("_02"))
        //    //{
        //    //    string file2 = zPath.Combine(zPath.GetDirectoryName(file), filename + "_02" + zPath.GetExtension(file));
        //    //    if (zFile.Exists(file2))
        //    //        throw new PBException($"file already exist \"{file2}\"");
        //    //    zFile.Copy(file, file2);
        //    //    file = file2;
        //    //}
        //    if (FileNumber.GetFileNumber(file).Number == 0)
        //        file = CreateNewFileNumber(file);
        //    return file;
        //}

        //private static string CreateNewFileNumber(string file)
        //{
        //    string directory = zPath.GetDirectoryName(file);
        //    string filename = zPath.GetFileNameWithoutExtension(file);
        //    string ext = zPath.GetExtension(file);
        //    int index = 2;
        //    string file2;
        //    while (true)
        //    {
        //        file2 = zPath.Combine(directory, filename + $"_{index:00}" + ext);
        //        if (!zFile.Exists(file2))
        //            break;
        //        index++;
        //    }
        //    zFile.Copy(file, file2);
        //    return file2;
        //}

        //public static void OpenFile(string file, int? line = null, int? column = null)
        //{
        //    //-cursor line:column
        //    //Process
        //    string option = "";
        //    if (line != null)
        //    {
        //        if (column == null)
        //            column = 1;
        //        option = $" -cursor {line}:{column}";
        //    }
        //    ProcessStartInfo startInfo = new ProcessStartInfo(@"c:\Program Files (x86)\EditPlus 3\editplus.exe", $"\"{file}\"{option}");
        //    Process.Start(startInfo);
        //}

        //public static void TraceScanFiles(QuestionResponseFiles scanFiles)
        //{
        //    pb.Trace.WriteLine($"responseFile : \"{scanFiles.ResponseFile}\"");
        //    foreach (string questionFile in scanFiles.QuestionFiles)
        //        pb.Trace.WriteLine($"questionFile : \"{questionFile}\"");
        //}

        //public static QuestionResponseFiles GetQuestionResponseFiles(string directory, string filename, string baseDirectory)
        //{
        //    QuestionResponseFiles questionResponseFiles = new QuestionResponseFiles();
        //    questionResponseFiles.BaseDirectory = baseDirectory;
        //    string[] files = GetScanFiles(directory, filename).ToArray();
        //    questionResponseFiles.QuestionFiles = new string[files.Length - 1];
        //    Array.Copy(files, questionResponseFiles.QuestionFiles, files.Length - 1);
        //    questionResponseFiles.ResponseFile = files[files.Length - 1];
        //    return questionResponseFiles;
        //}

        //private static IEnumerable<string> GetScanFiles(string directory, string filename)
        //{
        //    directory = zPath.Combine(directory, "scan");
        //    int index = 1;
        //    bool foundOne = false;
        //    while (true)
        //    {
        //        if (!foundOne && index == 100)
        //            throw new PBException("scan files not found");
        //        string file = zPath.Combine(directory, filename + $"-page-{index:000}.txt");
        //        //string file2 = zPath.Combine(directory, filename + $"-page-{index:000}_02.txt");
        //        if (zFile.Exists(file))
        //        {
        //            foundOne = true;
        //            //if (zFile.Exists(file2))
        //            //    yield return file2;
        //            //else
        //            //    yield return file;
        //            yield return GetLastFileNumber(file);
        //        }
        //        else if (foundOne)
        //            break;
        //        index++;
        //    }
        //}

        //private static string GetLastFileNumber(string file)
        //{
        //    string lastFile = file;
        //    string directory = zPath.GetDirectoryName(file);
        //    string filename = zPath.GetFileNameWithoutExtension(file);
        //    string ext = zPath.GetExtension(file);
        //    int index = 2;
        //    while (true)
        //    {
        //        string file2 = zPath.Combine(directory, filename + $"_{index:00}" + ext);
        //        if (!zFile.Exists(file2))
        //            break;
        //        lastFile = file2;
        //        index++;
        //    }
        //    return lastFile;
        //}

        //public static void ReadResponse(string name)
        //{
        //    XElement xe = XmlConfig.CurrentConfig.GetElement($"CardGames/CardGame[@name = \"{name}\"]");
        //    if (xe == null)
        //        throw new PBException($"card not found \"{name}\"");
        //    string directory = xe.zXPathExplicitValue("Directory");
        //    ResponseReader.Read(zPath.Combine(directory, xe.zXPathExplicitValue("ResponseFile")), GetResponseRegexValuesList()).zSave(zPath.Combine(directory, "response.json"), jsonIndent: true);
        //}

        //public static void ExportResponse(string file, Response[] responses, int yearWidth = 12)
        //{
        //    // 2012
        //    // Q102: ABCD
        //    pb.Trace.WriteLine($"export responses to \"{file}\"");

        //    Dictionary<int, int> years = new Dictionary<int, int>();
        //    List<IEnumerator<Response>> yearResponses = new List<IEnumerator<Response>>();
        //    int index = 0;
        //    //Trace.WriteLine($"{responses.Length} responses");
        //    foreach (Response response in responses)
        //    {
        //        //Trace.WriteLine($"{response.Year} - Q{response.QuestionNumber}: {response.Responses}");
        //        if (!years.ContainsKey(response.Year))
        //        {
        //            years.Add(response.Year, index++);
        //            yearResponses.Add(responses.Where(response2 => response2.Year == response.Year).GetEnumerator());
        //        }
        //    }


        //    // new FileStream(file, FileMode.OpenOrCreate)
        //    using (StreamWriter sw = new StreamWriter(zFile.Create(file)))
        //    {
        //        StringBuilder sb = new StringBuilder();

        //        bool first = true;
        //        foreach (int year in years.Keys)
        //        {
        //            if (!first)
        //                sb.Append(new string(' ', yearWidth - 5));
        //            sb.Append($" {year}");
        //            first = false;
        //        }
        //        sw.WriteLine();
        //        sw.WriteLine(sb.ToString());
        //        sw.WriteLine();

        //        sb.Clear();
        //        int lastIndex = -1;
        //        int l = 0;
        //        //foreach (Response response in responses)
        //        while (true)
        //        {
        //            bool found = false;
        //            foreach (IEnumerator<Response> yearResponse in yearResponses)
        //            {
        //                if (!yearResponse.MoveNext())
        //                    continue;

        //                found = true;
        //                Response response = yearResponse.Current;
        //                //Trace.WriteLine($"{response.Year} - Q{response.QuestionNumber}: {response.Responses}");
        //                index = years[response.Year];
        //                if (index <= lastIndex)
        //                {
        //                    sw.WriteLine(sb.ToString());
        //                    sw.WriteLine();
        //                    sb.Clear();
        //                    lastIndex = -1;
        //                    l = 0;
        //                }
        //                if (index != lastIndex - 1 || l > 0)
        //                    sb.Append(new string(' ', (index - lastIndex - 1) * yearWidth + l));
        //                string text = $" Q{response.QuestionNumber}: {response.Responses}";
        //                l = yearWidth - text.Length;
        //                sb.Append(text);
        //                lastIndex = index;
        //            }
        //            if (!found)
        //                break;
        //            if (sb.Length > 0)
        //            sw.WriteLine(sb.ToString());
        //            sw.WriteLine();
        //            sb.Clear();
        //            lastIndex = -1;
        //            l = 0;
        //        }
        //    }
        //}

        //public static void Test_02()
        //{
        //    pb.Trace.WriteLine("QuestionHtml/Header :");
        //    pb.Trace.WriteLine(XmlConfig.CurrentConfig.Get("QuestionHtml/Header")?.Trim()?.Replace("\n", "\r\n"));
        //}

        //public static void Test_01()
        //{
        //    //XElement xe = XmlConfig.CurrentConfig.GetElement("QuestionHtml/Header");
        //    //pb.Trace.WriteLine("QuestionHtml/Header");
        //    //pb.Trace.WriteLine(xe.Value);

        //    XDocument xd = XDocument.Load(@"c:\pib\drive\google\dev\project\.net\Apps\tools\anki\anki.config.xml", LoadOptions.PreserveWhitespace);
        //    //XDocument xd = XDocument.Load(@"c:\pib\drive\google\dev\project\.net\Apps\tools\anki\anki.config.xml");

        //    //System.Xml.XmlReaderSettings settings = new System.Xml.XmlReaderSettings();
        //    //XDocument xd;
        //    //using (System.Xml.XmlReader xr = System.Xml.XmlReader.Create(@"c:\pib\drive\google\dev\project\.net\Apps\tools\anki\anki.config.xml", settings))
        //    //    xd = XDocument.Load(xr, LoadOptions.PreserveWhitespace);

        //    pb.Trace.WriteLine("element QuestionHtml/Header");
        //    XElement xe = xd.Root.zXPathElement("QuestionHtml/Header");
        //    pb.Trace.WriteLine($"childs : nb {xe.Nodes().Count()}");
        //    foreach (XNode node in xe.Nodes())
        //    {
        //        pb.Trace.WriteLine($"node : {node.NodeType}");
        //        //if (node.NodeType == System.Xml.XmlNodeType.CDATA)
        //        //{
        //        //    XCData cdata = (XCData)node;
        //        //    string s = cdata.Value.Trim().Replace("\n", "\r\n");
        //        //    pb.Trace.WriteLine("XCData.Value");
        //        //    pb.Trace.WriteLine($"\"{s}\"");
        //        //    zFile.WriteAllText(@"c:\pib\_dl\test_01.txt", s);
        //        //    break;
        //        //}
        //    }
        //    //string s2 = xe.Value.Trim().Replace("\n", "\r\n");
        //    //pb.Trace.WriteLine("XElement.Value");
        //    //pb.Trace.WriteLine($"\"{s2}\"");
        //    //zFile.WriteAllText(@"c:\pib\_dl\test_02.txt", s2);
        //}
    }
}
