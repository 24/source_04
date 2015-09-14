using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace pb.Compiler
{
    [Flags]
    public enum ClassOptions
    {
        None                 = 0x00000000,
        //  access modifiers : public, protected internal, protected, internal, or private
        AccessType           = 0x00000007,
        Public               = 0x00000001,
        Private              = 0x00000002,
        Protected            = 0x00000003,
        Internal             = 0x00000004,
        ProtectedInternal    = 0x00000005,
        // static
        Static               = 0x00000008,
        // partial
        Partial              = 0x00000010
    }

    public class CSharpCodeWriter
    {
        private TextWriter _tw = null;

        public CSharpCodeWriter()
        {
            _tw = new StringWriter();
        }

        public CSharpCodeWriter(TextWriter tw)
        {
            _tw = tw;
        }

        public void WriteLine()
        {
            _tw.WriteLine();
        }

        public void WriteLine(string format, params object[] prm)
        {
            if (prm.Length > 0)
                _tw.WriteLine(format, prm);
            else
                _tw.WriteLine(format);
        }

        public void WriteUsings(IEnumerable<string> names)
        {
            foreach (string name in names)
            {
                _tw.WriteLine("using {0};", name);
            }
        }

        public void OpenNameSpace(string name)
        {
            if (name == null)
                throw new PBException("namespace is null, can't open namespace");
            _tw.WriteLine("namespace {0}", name);
            _tw.WriteLine("{");
        }

        public void CloseNameSpace()
        {
            _tw.WriteLine("}");
        }

        public void OpenClass(string name, ClassOptions options = ClassOptions.None)
        {
            if (name == null)
                throw new PBException("class name is null, can't open class");
            string access = GetAccessString(options);
            if (access != null)
                _tw.Write("{0} ", access);
            if ((options & ClassOptions.Static) == ClassOptions.Static)
                _tw.Write("static ");
            if ((options & ClassOptions.Partial) == ClassOptions.Partial)
                _tw.Write("partial ");
            _tw.WriteLine("class {0}", name);
            _tw.WriteLine("{");
        }

        public void CloseClass()
        {
            _tw.WriteLine("}");
        }

        public override string ToString()
        {
            if (_tw is StringWriter)
                return ((StringWriter)_tw).ToString();
            else
                throw new PBException("TextWriter of GenerateCode is not a StringWriter, impossible to generate string");
        }

        private static string GetAccessString(ClassOptions options)
        {
            ClassOptions access = options & ClassOptions.AccessType;
            switch (access)
            {
                case ClassOptions.Public:
                    return "public";
                case ClassOptions.Private:
                    return "private";
                case ClassOptions.Protected:
                    return "protected";
                case ClassOptions.Internal:
                    return "internal";
                case ClassOptions.ProtectedInternal:
                    return "protected internal";
            }
            return null;
        }
    }
}
