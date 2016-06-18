using pb.Data.Xml;
using pb.Text;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace pb.Compiler
{
    public class CompilerManager
    {
        private static CompilerManager __current = new CompilerManager();
        private Dictionary<string, Func<ICompiler>> _compilers = new Dictionary<string, Func<ICompiler>>();
        private Win32ResourceCompiler _win32ResourceCompiler = null;
        private ResourceCompiler _resourceCompiler = null;
        private Dictionary<string, string> _frameworkDirectories = new Dictionary<string, string>();
        private Predicate<CompilerMessage> _messageFilter = null;

        public static CompilerManager Current { get { return __current; } }  // set { __current = value; }
        public Win32ResourceCompiler Win32ResourceCompiler { get { return _win32ResourceCompiler; } }
        public ResourceCompiler ResourceCompiler { get { return _resourceCompiler; } }
        public Dictionary<string, string> FrameworkDirectories { get { return _frameworkDirectories; } }  // set { _frameworkDirectories = value; }
        public Predicate<CompilerMessage> MessageFilter { get { return _messageFilter; } }

        public void Init(XmlConfigElement xe)
        {
            foreach (XElement xe2 in xe.GetElements("Frameworks/Framework"))
            {
                _frameworkDirectories.Add(xe2.zExplicitAttribValue("version"), xe2.zExplicitAttribValue("directory"));
            }
            _win32ResourceCompiler = new Win32ResourceCompiler(xe.Get("Win32ResourceCompiler"));
            _resourceCompiler = new ResourceCompiler(xe.Get("ResourceCompiler"));

            //string disableMessages = xe.Get("DisableCompilerMessages");
            //if (disableMessages != null)
            //{
            //    Dictionary<string, string> disableMessagesDictionary = new Dictionary<string, string>();
            //    foreach (string messageId in zsplit.Split(disableMessages, ',', true))
            //        disableMessagesDictionary.Add(messageId, messageId);
            //    _messageFilter = compilerMessage => !disableMessagesDictionary.ContainsKey(compilerMessage.Id);
            //}
            _messageFilter = GetMessageFilter(xe.Get("DisableCompilerMessages"));
        }

        public void AddCompiler(string language, Func<ICompiler> compiler)
        {
            language = language.ToLowerInvariant();
            _compilers[language] = compiler;
        }

        public ICompiler GetCompiler(string language)
        {
            Func<ICompiler> compiler = null;
            if (_compilers.TryGetValue(language.ToLowerInvariant(), out compiler))
                return compiler();
            else
                throw new PBException("unknow compiler \"{0}\"", language);

        }

        public static Predicate<CompilerMessage> GetMessageFilter(string messagesIds)
        {
            if (messagesIds != null)
            {
                Dictionary<string, string> disableMessagesDictionary = new Dictionary<string, string>();
                foreach (string messageId in zsplit.Split(messagesIds, ',', true))
                    disableMessagesDictionary.Add(messageId, messageId);
                return compilerMessage => !disableMessagesDictionary.ContainsKey(compilerMessage.Id);
            }
            else
                return null;
        }
    }
}
