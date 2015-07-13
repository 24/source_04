using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using pb.Data.Xml;

namespace pb.old
{
    public class TextVariables
    {
        private static Regex _rgVariables = new Regex(@"\$([_a-z][_a-z0-9]*)\$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private Dictionary<string, string> _variables;
        private bool _removeUnknowVariable = false;
        private bool _findVariable = false;

        public TextVariables(Dictionary<string, string> variables)
        {
            _variables = variables;
        }

        public bool FindVariable { get { return _findVariable; } }

        public string SetTextVariables(string text, bool removeUnknowVariable = false)
        {
            _removeUnknowVariable = removeUnknowVariable;
            _findVariable = false;
            return _rgVariables.Replace(text, new MatchEvaluator(MatchEval));
        }

        private string MatchEval(Match match)
        {
            string name = match.Groups[1].Value.ToLower();
            if (_variables.ContainsKey(name))
            {
                _findVariable = true;
                return _variables[name];
            }
            else if (_removeUnknowVariable)
                return "";
            else
                return match.Value;
        }

        public static string SetTextVariables(string text, Dictionary<string, string> variables, bool removeUnknowVariable = false)
        {
            TextVariables textVariables = new TextVariables(variables);
            return textVariables.SetTextVariables(text, removeUnknowVariable);
        }
    }

    public class XmlTextVariables
    {
        public static Dictionary<string, string> GetXmlTextVariables(XElement xe)
        {
            Dictionary<string, string> variables = new Dictionary<string,string>();
            // remplacement de TextValue par XmlVariable le 20/09/2013
            foreach (XElement xe2 in xe.zXPathElements("//XmlVariable"))
            {
                string name = xe2.zAttribValue("name");
                string value = xe2.zAttribValue("value");
                if (!variables.ContainsKey(name))
                    variables.Add(name, value);
            }
            return variables;
        }

        public static void SetDictionaryTextVariables(Dictionary<string, string> variables)
        {
            bool again = true;
            TextVariables textVariables = new TextVariables(variables);
            while (again)
            {
                again = false;
                foreach (KeyValuePair<string, string> variable in variables)
                {
                    string newValue = textVariables.SetTextVariables(variable.Value);
                    if (textVariables.FindVariable)
                    {
                        variables[variable.Key] = newValue;
                        again = true;
                    }
                }
            }
        }

        public static void SetXmlAttributesTextVariables(XElement xe, Dictionary<string, string> variables)
        {
            TextVariables textVariables = new TextVariables(variables);
            foreach (XElement xe2 in xe.DescendantsAndSelf())
            {
                foreach (XAttribute xa in xe2.Attributes())
                {
                    xa.Value = textVariables.SetTextVariables(xa.Value);
                }
            }
        }
    }

    public static partial class GlobalExtension
    {
        public static string zSetTextVariables(this string text, Dictionary<string, string> variables, bool removeUnknowVariable = false)
        {
            return TextVariables.SetTextVariables(text, variables, removeUnknowVariable);
        }
    }
}
