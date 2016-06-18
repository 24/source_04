using System.Collections.Generic;

namespace pb.Compiler
{
    public class ResourceCompilerResult
    {
        public string CompiledResourceFile;
        public bool Success = false;
        public List<ResourceCompilerMessage> Messages = new List<ResourceCompilerMessage>();
    }

    public class ResourceCompilerMessage
    {
        public string File;
        public string Message;
    }
}
