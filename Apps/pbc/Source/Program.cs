using pb;
using pb.Compiler;
using pb.Data.Xml;
using pb.IO;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace pbc
{
    class Program
    {
        private static bool __help = false;
        private static int __traceLevel = 1;
        private static string __projectFile = null;
        private static Dictionary<string, string> _shortcuts = null;

        static void Main(string[] args)
        {
            try
            {
                Trace.CurrentTrace.AddOnWrite("console", text => Console.Write(text));
                if (!GetArguments(args))
                {
                    HelpUsage();
                    return;
                }
                if (__help)
                {
                    HelpTrace();
                    return;
                }
                //Trace.WriteLine($"args.Length {args.Length}");
                ProjectCompiler.TraceLevel = __traceLevel;

                //ReadShortcuts();
                //if (_shortcuts.ContainsKey(__projectFile))
                //    __projectFile = _shortcuts[__projectFile];
                string projectFile = GetProjectFromShortcut(__projectFile);
                if (projectFile == null)
                {
                    projectFile = __projectFile;
                    if (!zFile.Exists(projectFile) && !zPath.IsPathRooted(projectFile))
                        projectFile = zPath.Combine(XmlConfig.CurrentConfig.GetExplicit("ProjectDirectory"), __projectFile);
                }

                Compile.CompileFile(projectFile);
            }
            catch (Exception ex)
            {
                Trace.CurrentTrace.WriteError(ex);
            }
        }

        private static bool GetArguments(string[] args)
        {
            bool ret = true;
            if (args.Length == 0)
            {
                Trace.WriteLine("missing project file");
                ret = false;
            }
            else
            {
                string projectFile = args[0];
                if (args[0] == "--help")
                {
                    __help = true;
                    projectFile = null;
                }
                else if (args[0] == "--trace")
                {
                    __traceLevel = 2;
                    if (args.Length == 1)
                    {
                        Trace.WriteLine("missing project file");
                        ret = false;
                    }
                    else
                        projectFile = args[1];
                }
                if (ret)
                    __projectFile = projectFile;
            }
            return ret;
        }

        private static void HelpUsage()
        {
            // 0 no message, 1 default messages, 2 detailled messaged
            Trace.WriteLine("usage :");
            Trace.WriteLine("  pbc.exe [--trace] project");
            Trace.WriteLine("  pbc.exe --help");
            Trace.WriteLine(@"example : pbc.exe --trace c:\...\test.project.xml");
        }

        private static void HelpTrace()
        {
            ReadShortcuts();
            Trace.WriteLine("shortcuts :");
            foreach (KeyValuePair<string, string> value in _shortcuts)
                Trace.WriteLine("  shortcut \"{0}\" project \"{1}\"", value.Key, value.Value);
        }

        private static void ReadShortcuts()
        {
            _shortcuts = new Dictionary<string, string>();
            foreach (XElement xe in XmlConfig.CurrentConfig.GetElements("Shortcuts/Shortcut"))
            {
                _shortcuts.Add(xe.zExplicitAttribValue("shortcut").ToLowerInvariant(), xe.zExplicitAttribValue("project"));
            }
        }

        private static string GetProjectFromShortcut(string shortcut)
        {
            ReadShortcuts();
            shortcut = shortcut.ToLowerInvariant();
            if (_shortcuts.ContainsKey(shortcut))
                return _shortcuts[shortcut];
            else
                return null;
        }
    }
}

