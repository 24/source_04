using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using pb.Compiler;
using pb.IO;

namespace Test.Test_cpp
{
    public class SpecializedCppError
    {
        public CppError error;
        public string code;
    }

    public class SpecializedCppError_C2440 : SpecializedCppError
    {
        public string conversion;
        public string convertFrom;
        public string convertTo;
    }

    public class SpecializedCppError_C2664 : SpecializedCppError
    {
        public string function;
        public string argument;
        public string convertFrom;
        public string convertTo;
    }

    public class CppError
    {
        public bool error;
        public int index;
        public string message;
        public string source;
        public int line;
        public int column;
        public string project;
    }

    public static class Test_cpp_err_f
    {
        public static void Test_cpp_err_01(string cppErrFile)
        {
            // err line :
            //   Warning or Error, index, message, source path, line, column, project
            // message
            //   warning MSB8028: The intermediate directory (Debug\) contains files shared from another project (megasync.vcxproj).  This can lead to incorrect clean and rebuild behavior.
            //   error C2440: '=' : cannot convert from 'void *' to 'Parse *'
            //   error C2440: '=' : cannot convert from 'ExprList::ExprList_item *' to 'resolveCompoundOrderBy::ExprList_item *'
            //   error C2440: 'initializing' : cannot convert from 'void *' to 'FileChunk *'
            //   error C2664: 'int sqlite3VdbeAddOp4(Vdbe *,int,int,int,int,const char *,int)' : cannot convert argument 6 from 'void *' to 'const char *'

            var query = zfile.ReadLines(cppErrFile).Select(line => GetSpecializedCppError(GetCppError(line)));
            query.zView();
        }

        public static void Test_cpp_err_patch_01(string source, string patchedSource, string cppErrFile)
        {
            var query = zfile.ReadLines(cppErrFile).Select(line => GetSpecializedCppError(GetCppError(line)));
            Test_patch_01(source, patchedSource, query);
        }

        public static void Test_patch_01(string source, string patchedSource, IEnumerable<SpecializedCppError> errors)
        {
            //using (StreamWriter sw = File.CreateText(patchedSource))
            //string comment = "// new modif $$pb";
            using (StreamWriter sw = zfile.CreateCreateText(patchedSource, encoding: Encoding.Default))
            {
                IEnumerator<SpecializedCppError> errorsEnum = errors.GetEnumerator();
                SpecializedCppError error;
                if (errorsEnum.MoveNext())
                    error = errorsEnum.Current;
                else
                    error = null;
                int lineNumber = 1;
                int newLineNumber = 1;
                foreach (string line in zfile.ReadLines(source))
                {
                    if (error != null && error.error.line == lineNumber)
                    {
                        if (error is SpecializedCppError_C2440)
                        {
                            SpecializedCppError_C2440 error2 = (SpecializedCppError_C2440)error;
                            newLineNumber += 3;
                            //sw.WriteLine("// modif $$pb error line {0} => {1} cannot convert from '{2}' to '{3}'", lineNumber, newLineNumber, error2.convertFrom, error2.convertTo);
                            sw.WriteLine("// modif $$pb error '{0}' cannot convert from '{1}' to '{2}'", error2.conversion, error2.convertFrom, error2.convertTo);
                            sw.Write("//");
                            sw.WriteLine(line);
                            sw.WriteLine("// ({0})", error2.convertTo);
                        }
                        else  if (error is SpecializedCppError_C2664)
                        {
                            SpecializedCppError_C2664 error2 = (SpecializedCppError_C2664)error;
                            newLineNumber += 3;
                            sw.WriteLine("// modif $$pb error cannot convert argument {0} of {1} from '{2}' to '{3}'", error2.argument, error2.function, error2.convertFrom, error2.convertTo);
                            sw.Write("//");
                            sw.WriteLine(line);
                            sw.WriteLine("// ({0})", error2.convertTo);
                        }
                        if (errorsEnum.MoveNext())
                            error = errorsEnum.Current;
                        else
                            error = null;
                    }
                    sw.WriteLine(line);
                    lineNumber++;
                    newLineNumber++;
                }
            }
        }

        private static Regex __cppErrorCode = new Regex(@"^(?:error) (C[0-9]+):", RegexOptions.Compiled | RegexOptions.IgnoreCase);  // error C2440:
        private static Regex __cppErrorC2440 = new Regex(@"'([^']+)' : cannot convert from '([^']+)' to '([^']+)'$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static Regex __cppErrorC2664Function = new Regex(@"([^\s\(]+)\(", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static Regex __cppErrorC2664ArgumentConvertFromTo = new Regex(@"cannot convert argument ([0-9]+) from '([^']+)' to '([^']+)'$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static SpecializedCppError GetSpecializedCppError(CppError error)
        {
            //   error C2440: '=' : cannot convert from 'void *' to 'Parse *'
            //   error C2440: '=' : cannot convert from 'ExprList::ExprList_item *' to 'resolveCompoundOrderBy::ExprList_item *'
            //   error C2440: 'initializing' : cannot convert from 'void *' to 'FileChunk *'
            //   error C2664: 'int sqlite3VdbeAddOp4(Vdbe *,int,int,int,int,const char *,int)' : cannot convert argument 6 from 'void *' to 'const char *'
            Match match = __cppErrorCode.Match(error.message);
            string code = null;
            if (match.Success)
            {
                string function = null;
                string argument = null;
                string conversion = null;
                string convertFrom = null;
                string convertTo = null;
                code = match.Groups[1].Value;
                switch (code)
                {
                    case "C2440":
                        match = __cppErrorC2440.Match(error.message);
                        if (match.Success)
                        {
                            conversion = match.Groups[1].Value;
                            convertFrom = match.Groups[2].Value;
                            convertTo = match.Groups[3].Value;
                        }
                        return new SpecializedCppError_C2440 { error = error, code = code, conversion = conversion, convertFrom = convertFrom, convertTo = convertTo };
                    case "C2664":
                        match = __cppErrorC2664Function.Match(error.message);
                        if (match.Success)
                            function = match.Groups[1].Value;
                        match = __cppErrorC2664ArgumentConvertFromTo.Match(error.message);
                        if (match.Success)
                        {
                            argument = match.Groups[1].Value;
                            convertFrom = match.Groups[2].Value;
                            convertTo = match.Groups[3].Value;
                        }
                        return new SpecializedCppError_C2664 { error = error, code = code, function = function, argument = argument, convertFrom = convertFrom, convertTo = convertTo };
                }

            }
            return new SpecializedCppError { error = error, code = code };
        }

        public static CppError GetCppError(string line)
        {
            CppError error = new CppError();
            string[] values = line.Split('\t');
            int l = values.Length;
            int i = 0;
            if (l > i && values[i++] == "Error")
                error.error = true;
            if (l > i)
            {
                int j;
                if (int.TryParse(values[i++], out j))
                    error.index = j;
            }
            if (l > i)
                error.message = values[i++];
            if (l > i)
                error.source = values[i++];
            if (l > i)
            {
                int j;
                if (int.TryParse(values[i++], out j))
                    error.line = j;
            }
            if (l > i)
            {
                int j;
                if (int.TryParse(values[i++], out j))
                    error.column = j;
            }
            if (l > i)
                error.project = values[i++];
            return error;
        }
    }
}
