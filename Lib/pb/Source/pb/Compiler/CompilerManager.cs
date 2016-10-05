using pb.Data.Xml;
using pb.IO;
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
        private static bool _updateAssembly = false;
        private static string _updateSubDirectory = "new";
        private static bool _traceUpdateAssembly = false;

        public static CompilerManager Current { get { return __current; } }  // set { __current = value; }
        public Win32ResourceCompiler Win32ResourceCompiler { get { return _win32ResourceCompiler; } }
        public ResourceCompiler ResourceCompiler { get { return _resourceCompiler; } }
        public Dictionary<string, string> FrameworkDirectories { get { return _frameworkDirectories; } }  // set { _frameworkDirectories = value; }
        public Predicate<CompilerMessage> MessageFilter { get { return _messageFilter; } }
        public static bool UpdateAssembly { get { return _updateAssembly; } set { _updateAssembly = value; } }
        public static string UpdateSubDirectory { get { return _updateSubDirectory; } set { _updateSubDirectory = value; } }
        public static bool TraceUpdateAssembly { get { return _traceUpdateAssembly; } set { _traceUpdateAssembly = value; } }

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

            _updateAssembly = xe.Get("UpdateAssembly").zTryParseAs(false); ;
            _updateSubDirectory = xe.Get("UpdateAssemblySubDirectory", _updateSubDirectory); ;
            _traceUpdateAssembly = xe.Get("TraceUpdateAssembly").zTryParseAs(false);
            //Trace.WriteLine("CompilerManager.Init()   :");
            //Trace.WriteLine("  UpdateAssembly         : {0}", _updateAssembly);
            //Trace.WriteLine("  UpdateSubDirectory     : {0}", _updateSubDirectory);
            //Trace.WriteLine("  TraceUpdateAssembly    : {0}", _traceUpdateAssembly);
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

        public void UpdateAssemblies(IEnumerable<CompilerAssembly> assemblies)
        {
            if (!_updateAssembly)
                return;
            foreach (CompilerAssembly assembly in assemblies)
            {
                if (!assembly.FrameworkAssembly)
                {
                    string assemblyFile = assembly.File;
                    string assemblyDirectory = zPath.GetDirectoryName(assemblyFile);
                    string updateDirectory = zPath.Combine(assemblyDirectory, _updateSubDirectory);
                    string error;
                    if (zUpdateFiles.TryUpdateFile(assemblyFile, updateDirectory, out error))
                    {
                        if (_traceUpdateAssembly)
                            Trace.WriteLine("update assembly \"{0}\" from \"{1}\"", assemblyFile, _updateSubDirectory);
                        zUpdateFiles.UpdateFiles(updateDirectory, assemblyDirectory);
                    }
                    if (error != null)
                    {
                        if (_traceUpdateAssembly)
                        {
                            Trace.WriteLine("unable to update assembly \"{0}\" (\"{1}\")", assemblyFile, error.TrimEnd(' ', '\r', '\n'));
                        }
                    }
                }
            }
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
