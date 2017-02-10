using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using pb.Data;
using pb.Data.Mongo;
using pb.Text;

namespace Download.Print
{
    public class PrintTextValues
    {
        public string[] description;
        //public string language = null;
        //public string size = null;
        //public int? nbPages = null;
        public NamedValues<ZValue> infos = new NamedValues<ZValue>(useLowercaseKey: true);
    }

    public class PrintTextValues_v1
    {
        public string[] description;
        public string language = null;
        public string size = null;
        public int? nbPages = null;
        public NamedValues<ZValue> infos = new NamedValues<ZValue>(useLowercaseKey: true);
    }

    public class PrintTitleInfos
    {
        public bool foundInfo = false;
        public string title = null;
        public NamedValues<ZValue> infos = null;
    }

    public class PrintTextValuesManager : IDisposable
    {
        private static bool __trace = false;
        //private static PrintTextValuesManager __currentPrintTextValuesManager = null;
        private RegexValuesList _textInfoRegexList = null;
        private Func<string, string> _trim = null;
        private int _maxNameLength = 30;
        private string _exportDataFile = null;
        //private BsonWriter _exportDataWriter = null;
        private PBBsonWriter _exportDataWriter = null;
        //private bool _extractValuesFromText = true;

        public PrintTextValuesManager(RegexValuesList textInfoRegexList, Func<string, string> trim = null)
        {
            _textInfoRegexList = textInfoRegexList;
            _trim = trim;
        }

        public void Dispose()
        {
            CloseExportDataFile();
        }

        public static bool Trace { get { return __trace; } set { __trace = value; } }
        //public static PrintTextValuesManager CurrentPrintTextValuesManager { get { return __currentPrintTextValuesManager; } set { __currentPrintTextValuesManager = value; } }
        //public bool ExtractValuesFromText { get { return _extractValuesFromText; } set { _extractValuesFromText = value; } }

        public void SetExportDataFile(string file)
        {
            _exportDataFile = file;
            OpenExportDataFile();
        }

        public void OpenExportDataFile()
        {
            CloseExportDataFile();
            if (_exportDataFile != null)
            {
                _exportDataWriter = PBBsonWriter.Open(_exportDataFile, append: true);
            }
        }

        public void CloseExportDataFile()
        {
            if (_exportDataWriter != null)
            {
                _exportDataWriter.Close();
                _exportDataWriter = null;
            }
        }

        public PrintTextValues GetTextValues(IEnumerable<string> texts, string title, bool extractValuesFromText = true)
        {
            // read : title
            // modify : infos, description, language, size, nbPages

            PrintTextValues textValues = new PrintTextValues();
            List<string> description = new List<string>();

            _exportDataWriter.zWriteStartDocument();
            _exportDataWriter.zWrite("Title", title);
            _exportDataWriter.zWriteStartArray("Texts");

            string name = null;
            foreach (string s in texts)
            {
                _exportDataWriter.zWrite(s);
                if (__trace)
                    pb.Trace.WriteLine("SetTextValues : \"{0}\"", s);

                string name2;
                string text2;
                // Editeur : Presse fr
                bool textValues2 = false;
                if (extractValuesFromText)
                {
                    textValues2 = ExtractTextValues2(s, out name2, out text2);

                    if (s == "\r\n" || textValues2)
                    {
                        if (name != null)
                        {
                            if (__trace)
                                pb.Trace.WriteLine("SetTextValues SetValue : \"{0}\" = null", name);
                            textValues.infos.SetValue(name, null);
                        }
                        name = null;

                        if (textValues2)
                        {
                            if (__trace)
                                pb.Trace.WriteLine("SetTextValues SetValue : \"{0}\" = \"{1}\"", name2, text2);
                            if (text2 != null)
                                textValues.infos.SetValue(name2, new ZString(text2));
                            else
                                name = name2;
                        }
                    }
                }

                if (!textValues2)
                {
                    // PDF | 116 pages | 53 Mb | French
                    string s2 = ExtractTextValues(textValues.infos, s);
                    s2 = Trim(s2);
                    if (__trace)
                        pb.Trace.WriteLine("SetTextValues ExtractTextValues : \"{0}\" - \"{1}\"", s, s2);

                    if (s2 != "" && s2 != title)
                    {
                        if (s2.StartsWith(title))
                            s2 = Trim(s2.Substring(title.Length));

                        if (name != null)
                        {
                            if (__trace)
                                pb.Trace.WriteLine("SetTextValues SetValue : \"{0}\" = \"{1}\"", name, s2);
                            textValues.infos.SetValue(name, new ZString(s2));
                            name = null;
                        }
                        else
                        {
                            NamedValues<ZValue> values = _textInfoRegexList.ExtractTextValues(ref s2);
                            textValues.infos.SetValues(values);

                            if (s2 != "")
                            {
                                if (__trace)
                                    pb.Trace.WriteLine("SetTextValues description.Add : \"{0}\"", s2);
                                description.Add(s2);
                            }
                        }
                    }
                }
            }
            if (name != null)
            {
                if (__trace)
                    pb.Trace.WriteLine("SetTextValues SetValue : \"{0}\" = null", name);
                textValues.infos.SetValue(name, null);
            }

            textValues.description = description.ToArray();

            _exportDataWriter.zWriteEndArray();
            _exportDataWriter.zWriteName("PrintTextValues");
            //_exportDataWriter.zWriteStartDocument("PrintTextValues");
            _exportDataWriter.zSerialize(textValues);
            //_exportDataWriter.zWriteEndDocument();
            _exportDataWriter.zWriteEndDocument();
            _exportDataWriter.zWriteName("fake");    // ??? pour éviter l'erreur : WriteString can only be called when State is Value or Initial, not when State is Name (System.InvalidOperationException)
            _exportDataWriter.zWriteLine();
            return textValues;
        }

        public PrintTextValues_v1 GetTextValues_v1(IEnumerable<string> texts, string title)
        {
            // read : title
            // modify : infos, description, language, size, nbPages

            PrintTextValues_v1 textValues = new PrintTextValues_v1();
            List<string> description = new List<string>();

            _exportDataWriter.zWriteStartDocument();
            _exportDataWriter.zWrite("Title", title);
            _exportDataWriter.zWriteStartArray("Texts");

            string name = null;
            string text = null;
            string name2 = null;
            string text2 = null;
            foreach (string s in texts)
            {
                // PDF | 116 pages | 53 Mb | French
                _exportDataWriter.zWrite(s);
                if (__trace)
                    pb.Trace.WriteLine("SetTextValues : \"{0}\"", s);

                // Editeur : Presse fr
                bool textValues2 = ExtractTextValues2(s, out name2, out text2);
                if (textValues2)
                {
                    if (__trace)
                        pb.Trace.WriteLine("SetTextValues SetValue : \"{0}\" = \"{1}\"", name2, text2);
                    textValues.infos.SetValue(name2, new ZString(text2));
                }

                if (s == "\r\n" || textValues2)
                {
                    if (name != null)
                    {
                        if (__trace)
                            pb.Trace.WriteLine("SetTextValues SetValue : \"{0}\" = \"{1}\"", name, text);
                        textValues.infos.SetValue(name, new ZString(text));
                    }
                    else if (text != null)
                    {
                        if (__trace)
                            pb.Trace.WriteLine("SetTextValues description.Add : \"{0}\"", text);
                        description.Add(text);
                    }
                    name = null;
                    text = null;
                }
                else
                {
                    string s2 = s.Trim();
                    if (s2.EndsWith(":"))
                    {
                        string s3 = s2.Substring(0, s2.Length - 1).Trim();
                        if (s3 != "")
                        {
                            if (name != null)
                            {
                                if (__trace)
                                    pb.Trace.WriteLine("SetTextValues SetValue : \"{0}\" = \"{1}\"", name, text);
                                textValues.infos.SetValue(name, new ZString(text));
                            }
                            else if (text != null)
                            {
                                if (__trace)
                                    pb.Trace.WriteLine("SetTextValues description.Add : \"{0}\"", text);
                                description.Add(text);
                            }
                            name = s3;
                            text = null;
                            //foundName = true;
                            if (__trace)
                                pb.Trace.WriteLine("SetTextValues Set name \"{0}\"", name);
                        }
                        else
                        {
                            if (__trace)
                                pb.Trace.WriteLine("SetTextValues Nothing to do");
                        }
                    }
                    else
                    {
                        s2 = ExtractTextValues(textValues.infos, s);
                        s2 = Trim(s2);
                        if (__trace)
                            pb.Trace.WriteLine("SetTextValues ExtractTextValues : \"{0}\" - \"{1}\"", s, s2);
                        if (textValues.infos.ContainsKey("language"))
                        {
                            textValues.language = (string)textValues.infos["language"];
                            textValues.infos.Remove("language");
                        }
                        else if (textValues.infos.ContainsKey("size"))
                        {
                            textValues.size = (string)textValues.infos["size"];
                            textValues.infos.Remove("size");
                        }
                        else if (textValues.infos.ContainsKey("page_nb"))
                        {
                            textValues.nbPages = int.Parse((string)textValues.infos["page_nb"]);
                            textValues.infos.Remove("page_nb");
                        }

                        if (s2 != "" && s2 != title)
                        {
                            if (text == null)
                                text = s2;
                            else
                                text += " " + s2;
                            if (__trace)
                                pb.Trace.WriteLine("SetTextValues Add string to text : \"{0}\"", text);
                        }
                        else
                        {
                            if (__trace)
                                pb.Trace.WriteLine("SetTextValues dont Add string to text");
                        }
                    }
                }
            }
            if (text != null)
            {
                if (name != null)
                {
                    if (__trace)
                        pb.Trace.WriteLine("SetTextValues SetValue : \"{0}\" = \"{1}\"", name, text);
                    textValues.infos.SetValue(name, new ZString(text));
                }
                else
                {
                    if (__trace)
                        pb.Trace.WriteLine("SetTextValues description.Add : \"{0}\"", text);
                    description.Add(text);
                }
            }
            textValues.description = description.ToArray();

            _exportDataWriter.zWriteEndArray();
            _exportDataWriter.zWriteName("PrintTextValues");
            //_exportDataWriter.zWriteStartDocument("PrintTextValues");
            _exportDataWriter.zSerialize(textValues);
            //_exportDataWriter.zWriteEndDocument();
            _exportDataWriter.zWriteEndDocument();
            _exportDataWriter.zWriteName("fake");    // ??? pour éviter l'erreur : WriteString can only be called when State is Value or Initial, not when State is Name (System.InvalidOperationException)
            _exportDataWriter.zWriteLine();
            return textValues;
        }

        private bool ExtractTextValues2(string s, out string name, out string value)
        {
            // Editeur : Presse fr
            // Date de sortie : 2014
            // Hébergeur : Multi
            // pas de | : ISBN: 2300028441 | 221 pages | PDF
            name = null;
            value = null;
            if (s.IndexOf('|') != -1)
                return false;
            int i = s.IndexOf(':');
            if (i == -1 || i > _maxNameLength)
                return false;
            name = Trim(s.Substring(0, i));
            value = Trim(s.Substring(i + 1));
            if (value == "")
                value = null;
            return true;
        }

        private string ExtractTextValues(NamedValues<ZValue> infos, string s)
        {
            // French | PDF | 107 MB -*- French | PDF |  22 Pages | 7 MB -*- PDF | 116 pages | 205/148 pages | 53 Mb | French -*- Micro Application | 2010 | ISBN: 2300028441 | 221 pages | PDF
            // pb : |I|N|F|O|S|, |S|Y|N|O|P|S|I|S|, |T|E|L|E|C|H|A|R|G|E|R|
            // example http://www.telechargement-plus.com/e-book-magazines/bande-dessines/136846-season-one-100-marvel-syrie-en-cours-10-tomes-comicmulti.html
            if (s.Contains('|'))
            {
                //Trace.CurrentTrace.WriteLine("info \"{0}\"", s);
                foreach (string s2 in zsplit.Split(s, '|', true))
                {
                    string s3 = s2;
                    NamedValues<ZValue> values = _textInfoRegexList.ExtractTextValues(ref s3);
                    infos.SetValues(values);
                    //s3 = s3.Trim();
                    s3 = Trim(s3);
                    if (s3 != "")
                    {
                        string name;
                        string value;
                        if (ExtractTextValues2(s3, out name, out value))
                            infos.SetValue(name, value);
                        else
                            infos.SetValue(s3, null);
                    }
                }
                return "";
            }
            else
            //{
            //    NamedValues<ZValue> values = _textInfoRegexList.ExtractTextValues(ref s);
            //    infos.SetValues(values);
            //    return s;
            //}
            return s;
        }

        private static Regex __rgTitleInfo = new Regex(@"\[([^\]]+)\]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public PrintTitleInfos ExtractTitleInfos(string title)
        {
            // LES JOURNAUX -  MERCREDI 29 / 30 OCTOBRE 2014 & + [PDF][Lien Direct]
            // extract [PDF][Lien Direct]
            //string title = post.title;
            if (title == null)
                return new PrintTitleInfos { foundInfo = false };
            NamedValues<ZValue> infos = new NamedValues<ZValue>();
            bool foundInfo = false;
            while (true)
            {
                Match match = __rgTitleInfo.Match(title);
                if (!match.Success)
                    break;
                foundInfo = true;
                infos.SetValue(match.Groups[1].Value, null);
                title = match.zReplace(title, "_");
            }
            if (foundInfo)
            {
                title = Trim(title);
                return new PrintTitleInfos { foundInfo = true, title = title, infos = infos };
            }
            else
                return new PrintTitleInfos { foundInfo = false };
        }

        private string Trim(string s)
        {
            if (_trim != null)
                s = _trim(s);
            return s;
        }
    }
}
