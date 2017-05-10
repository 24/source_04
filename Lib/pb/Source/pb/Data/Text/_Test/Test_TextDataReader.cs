using pb.Data.Xml;
using pb.IO;
using pb.Text;
using System.Collections.Generic;

namespace pb.Data.Text.Test
{
    public static class Test_TextDataReader
    {
        public static IEnumerable<TextData> Test_Anki_01(string directory, int limit = 0)
        {
            TextDataReader textDataReader = new TextDataReader(CreateAnkiRegexValuesList(), GetScanFiles(directory, limit));
            return textDataReader.Read();
        }

        public static RegexValuesList CreateAnkiRegexValuesList()
        {
            string configFile = @"c:\pib\drive\google\dev\.net\Apps\tools\anki\anki.config.xml";
            XmlConfig config = new XmlConfig(configFile);
            RegexValuesList regexValuesList = new RegexValuesList(config.GetElements("QuestionInfos/QuestionInfo"), compileRegex: true);
            //regexValuesList.Add(config.GetElements("ResponseInfos/ResponseInfo"), compileRegex: true);
            return regexValuesList;
        }

        // copy from QuestionsManager.cs
        private static IEnumerable<string> GetScanFiles(string directory, int limit = 0)
        {
            // files :
            //   scan_02.txt
            //   scan.txt
            //   scan-page-001_02.txt
            //   scan-page-001.txt
            //   scan-page-001-01_02.txt
            //   scan-page-001-01.txt
            //   scan-page-001-02_02.txt
            //   scan-page-001-02.txt

            if (!zDirectory.Exists(directory))
                throw new PBException($"scan directory not found (\"{directory}\")");
            int index = 1;
            int count = 0;
            bool foundOne = false;
            string file = null;
            file = zPath.Combine(directory, "scan.txt");
            if (zFile.Exists(file))
            {
                yield return GetLastFileNumber(file);
            }
            else
            {
                while (true)
                {
                    if (!foundOne && index == 100)
                        throw new PBException($"scan files not found (\"{file}\")");

                    bool found = false;

                    // first column file : scan-page-001-01.txt
                    file = zPath.Combine(directory, $"scan-page-{index:000}-01.txt");
                    if (zFile.Exists(file))
                    {
                        foundOne = true;
                        found = true;
                        yield return GetLastFileNumber(file);
                        count++;

                        // second column file : scan-page-001-01.txt
                        file = zPath.Combine(directory, $"scan-page-{index:000}-02.txt");
                        if (zFile.Exists(file) && (limit == 0 || count < limit))
                        {
                            yield return GetLastFileNumber(file);
                            count++;
                        }
                    }
                    else
                    {

                        file = zPath.Combine(directory, $"scan-page-{index:000}.txt");

                        //Trace.WriteLine(file);
                        if (zFile.Exists(file))
                        {
                            foundOne = true;
                            found = true;
                            yield return GetLastFileNumber(file);
                            count++;
                        }
                    }

                    if ((!found && foundOne) || (limit != 0 && count >= limit))
                        break;
                    index++;
                }
            }
        }

        // copy from QuestionsManager.cs
        private static string GetLastFileNumber(string file)
        {
            string lastFile = file;
            string directory = zPath.GetDirectoryName(file);
            string filename = zPath.GetFileNameWithoutExtension(file);
            string ext = zPath.GetExtension(file);
            int index = 2;
            while (true)
            {
                string file2 = zPath.Combine(directory, filename + $"_{index:00}" + ext);
                if (!zFile.Exists(file2))
                    break;
                lastFile = file2;
                index++;
            }
            return lastFile;
        }
    }
}
