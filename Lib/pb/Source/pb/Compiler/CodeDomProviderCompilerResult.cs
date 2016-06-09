using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;

namespace pb.Compiler
{
    public class CodeDomProviderCompilerResult : ICompilerResult
    {
        private CompilerResults _result;

        public CodeDomProviderCompilerResult(CompilerResults result)
        {
            _result = result;
        }

        public bool Success { get { return !_result.Errors.HasErrors; } }
        public int MessagesCount { get { return _result.Errors.Count; } }

        public Assembly LoadAssembly()
        {
            return _result.CompiledAssembly;
        }

        public string GetAssemblyFile()
        {
            return _result.PathToAssembly;
        }

        public bool HasErrors()
        {
            return _result.Errors.HasErrors;
        }

        public bool HasWarnings()
        {
            return _result.Errors.HasWarnings;
        }

        public IEnumerable<CompilerMessage> GetMessages(Predicate<CompilerMessage> messageFilter = null)
        {
            foreach (CompilerError error in _result.Errors)
            {
                //yield return new CompilerMessage { Id = error.ErrorNumber, Message = error.ErrorText, IsWarning = error.IsWarning, FileName = error.FileName, Column = error.Column, Line = error.Line };
                //messageFilter
                CompilerMessage message = new CompilerMessage { Id = error.ErrorNumber, Message = error.ErrorText, Severity = error.IsWarning ? CompilerMessageSeverity.Warning : CompilerMessageSeverity.Error, FileName = error.FileName, Column = error.Column, Line = error.Line };
                if (messageFilter != null && !messageFilter(message))
                    continue;
                yield return message;
            }
        }
    }
}
