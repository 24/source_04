using System;
using System.IO;
using System.Xml.Linq;
using pb.Data.Xml;
using pb.IO;

namespace pb.Compiler
{
    public static class ICompilerExtension
    {
        public static void zSetCompilerParameters(this ICompiler compiler, XElement xe)
        {
            if (xe == null)
                return;

            compiler.OutputDir = xe.zXPathValue("OutputDir", compiler.OutputDir);
            compiler.OutputAssembly = xe.zXPathValue("Output", compiler.OutputAssembly);
            string ext = zPath.GetExtension(compiler.OutputAssembly);
            if (ext != null)
            {
                if (ext.ToLower() == ".exe")
                    compiler.GenerateExecutable = true;
                else if (ext.ToLower() == ".dll")
                    compiler.GenerateExecutable = false;
            }
            compiler.GenerateExecutable = xe.zXPathValue("GenerateExecutable").zTryParseAs(compiler.GenerateExecutable);
            compiler.GenerateInMemory = xe.zXPathValue("GenerateInMemory").zTryParseAs(compiler.GenerateInMemory);
            compiler.DebugInformation = xe.zXPathValue("DebugInformation").zTryParseAs(compiler.DebugInformation);
            compiler.WarningLevel = xe.zXPathValue("WarningLevel").zTryParseAs<int>(compiler.WarningLevel);
            compiler.AddCompilerOptions(xe.zXPathValues("CompilerOptions"));
            string keyfile = xe.zXPathValue("KeyFile");
            if (keyfile != null)
                compiler.AddCompilerOption("/keyfile:\"" + keyfile.zRootPath(compiler.DefaultDir) + "\"");
            string target = xe.zXPathValue("Target");
            if (target != null)
                compiler.AddCompilerOption("/target:" + target);
            string icon = xe.zXPathValue("Icon");
            if (icon != null)
                compiler.AddCompilerOption("/win32icon:" + icon.zRootPath(compiler.DefaultDir));
            compiler.AddSources(xe.Elements("Source"));
            compiler.AddFiles(xe.zXPathElements("File"));  // compiler.DefaultDir
            compiler.AddAssemblies(xe.zXPathElements("Assembly"));
            compiler.AddLocalAssemblies(xe.zXPathElements("LocalAssembly"));

            compiler.Language = xe.zXPathValue("Language", compiler.Language);
            foreach (XElement xe2 in xe.zXPathElements("ProviderOption"))
                compiler.SetProviderOption(xe2.zAttribValue("name"), xe2.zAttribValue("value"));
            compiler.ResourceCompiler = xe.zXPathValue("ResourceCompiler", compiler.ResourceCompiler);

            compiler.AddCopyOutputDirectories(xe.zXPathValues("CopyOutput"));
        }

        public static void zSetCompilerParameters(this ICompiler compiler, XmlConfigElement xe)
        {
            if (xe == null)
                return;

            compiler.OutputDir = xe.Get("OutputDir", compiler.OutputDir);
            compiler.OutputAssembly = xe.Get("Output", compiler.OutputAssembly);
            string ext = zPath.GetExtension(compiler.OutputAssembly);
            if (ext != null)
            {
                if (ext.ToLower() == ".exe")
                    compiler.GenerateExecutable = true;
                else if (ext.ToLower() == ".dll")
                    compiler.GenerateExecutable = false;
            }
            compiler.GenerateExecutable = xe.Get("GenerateExecutable").zTryParseAs(compiler.GenerateExecutable);
            compiler.GenerateInMemory = xe.Get("GenerateInMemory").zTryParseAs(compiler.GenerateInMemory);
            compiler.DebugInformation = xe.Get("DebugInformation").zTryParseAs(compiler.DebugInformation);
            compiler.WarningLevel = xe.Get("WarningLevel").zTryParseAs<int>(compiler.WarningLevel);
            compiler.AddCompilerOptions(xe.GetValues("CompilerOptions"));
            string keyfile = xe.Get("KeyFile");
            if (keyfile != null)
                compiler.AddCompilerOption("/keyfile:\"" + keyfile.zRootPath(compiler.DefaultDir) + "\"");
            string target = xe.Get("Target");
            if (target != null)
                compiler.AddCompilerOption("/target:" + target);
            string icon = xe.Get("Icon");
            if (icon != null)
                compiler.AddCompilerOption("/win32icon:" + icon.zRootPath(compiler.DefaultDir));
            compiler.AddSources(xe.GetElements("Source"));
            compiler.AddFiles(xe.GetElements("File"));  // compiler.DefaultDir
            compiler.AddAssemblies(xe.GetElements("Assembly"));
            compiler.AddLocalAssemblies(xe.GetElements("LocalAssembly"));

            compiler.Language = xe.Get("Language", compiler.Language);
            foreach (XElement xe2 in xe.GetElements("ProviderOption"))
                compiler.SetProviderOption(xe2.zAttribValue("name"), xe2.zAttribValue("value"));
            compiler.ResourceCompiler = xe.Get("ResourceCompiler", compiler.ResourceCompiler);

            compiler.AddCopyOutputDirectories(xe.GetValues("CopyOutput"));
        }
    }
}
