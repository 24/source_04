using System;
using System.Collections.Generic;
using System.IO;
using pb.IO;

namespace pb.Compiler
{
    public class GenerateCSharpCodeResult
    {
        public string SourceFile = null;            // "c:\pib\prog\tools\runsource\exe\run\RunSource_00001.cs"
        public string NameSpace = null;             // namespace from project
        public string TypeName = null;              // "_RunCode" - "w"
        public string RunMethodName = null;         // "Run"

        public string GetFullRunMethodName()
        {
            string typeName;
            if (NameSpace != null)
                typeName = NameSpace + "." + TypeName;
            else
                typeName = TypeName;
            return typeName + "." + RunMethodName;
        }
    }

    public class GenerateCSharpCode
    {
        private string _sourceFile = null;            // "c:\pib\prog\tools\runsource\exe\run\RunSource_00001.cs"
        private Dictionary<string, string> _usings = new Dictionary<string, string>();

        private string _nameSpace = null;             // namespace from project
        private string _runTypeName = null;           // "_RunCode" - "w"
        private string _runMethodName = null;         // "Run"

        public GenerateCSharpCode(string sourceFile)
        {
            if (zPath.GetExtension(sourceFile).ToLower() != ".cs")
                sourceFile = zpath.PathSetExtension(sourceFile, ".cs");
            _sourceFile = sourceFile;
        }


        /// <summary>example "c:\pib\prog\tools\runsource\exe\run\RunSource_00001"</summary>
        public string SourceFile { get { return _sourceFile; } }
        // test if (value != null) pour simplifier l'utilisation à partir d'un fichier de config
        public string NameSpace { get { return _nameSpace; } set { if (value != null) _nameSpace = value; } }
        public string RunTypeName { get { return _runTypeName; } set { if (value != null) _runTypeName = value; } }
        public string RunMethodName { get { return _runMethodName; } set { if (value != null) _runMethodName = value; } }

        public void AddUsings(IEnumerable<string> usings)
        {
            foreach (string name in usings)
            {
                // $$todo vérifier s'il faut mettre un warning si using est déjà dans le distionnaire
                if (!_usings.ContainsKey(name))
                    _usings.Add(name, name);
            }
        }

        public GenerateCSharpCodeResult GenerateCode(string code)
        {
            string file = _sourceFile;
            using (StreamWriter sw = zFile.CreateText(file))
            {
                CSharpCodeWriter codeWriter = new CSharpCodeWriter(sw);

                // using
                codeWriter.WriteUsings(_usings.Keys);

                // open namespace
                codeWriter.OpenNameSpace(_nameSpace);

                // open class
                // public static partial class ...
                codeWriter.OpenClass(_runTypeName, ClassOptions.Public | ClassOptions.Static | ClassOptions.Partial);

                // open function
                // public static void Run()
                if (_runMethodName == null)
                    throw new PBException("run method name is null, can't open method");
                codeWriter.WriteLine("public static void {0}()", _runMethodName);
                codeWriter.WriteLine("{");
                codeWriter.WriteLine(code);
                codeWriter.WriteLine("}");

                // close class
                codeWriter.CloseClass();

                // close namespace
                codeWriter.CloseNameSpace();
            }
            return new GenerateCSharpCodeResult { SourceFile = _sourceFile, NameSpace = _nameSpace, TypeName = _runTypeName, RunMethodName = _runMethodName };
        }
    }
}
