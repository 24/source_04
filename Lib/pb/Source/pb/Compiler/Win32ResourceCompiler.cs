using pb.IO;
using System.Diagnostics;

// todo :
//   ok - controler que le fichier .rc n'est pas en utf8+bom
//   - ou re-générer le fichier .rc sans bom

// doc :
//   - attention si le fichier .rc est en utf8+bom le .exe n'aura pas d'icone


namespace pb.Compiler
{
    //public class Win32ResourceCompilerResult
    //{
    //    public string CompiledResourceFile;
    //    public bool Success = false;
    //    public List<ResourceCompilerMessage> Messages = new List<ResourceCompilerMessage>();
    //}

    public class Win32ResourceCompiler
    {
        private string _win32ResourceCompiler = null;
        private string _resourceCompiling = null;
        private ResourceCompilerResult _resourceResults = null;

        public Win32ResourceCompiler(string win32ResourceCompiler)
        {
            _win32ResourceCompiler = win32ResourceCompiler;
        }

        public ResourceCompilerResult Compile(string resourceFile, string outputDir)
        {
            if (_win32ResourceCompiler == null)
                throw new PBException("win32 resource compiler is not defined");
            if (!zFile.Exists(_win32ResourceCompiler))
                throw new PBException("resource compiler cannot be found \"{0}\"", _win32ResourceCompiler);
            if (!zFile.Exists(resourceFile))
                throw new PBException("resource file cannot be found \"{0}\"", resourceFile);
            // attention si le fichier .rc est en utf8+bom le .exe n'aura pas d'icone
            if (zfile.IsBom(resourceFile))
                throw new PBException("resource file encoding is utf8 + bom \"{0}\"", resourceFile);

            if (!zDirectory.Exists(outputDir))
                zDirectory.CreateDirectory(outputDir);

            string resourceFilename = zPath.GetFileNameWithoutExtension(resourceFile);
            string compiledResourceFile = resourceFilename + ".res";
            compiledResourceFile = zpath.PathSetDirectory(compiledResourceFile, outputDir);
            if (zFile.Exists(compiledResourceFile) && zFile.Exists(resourceFile))
            {
                var fiResource = zFile.CreateFileInfo(resourceFile);
                var fiCompiledResource = zFile.CreateFileInfo(compiledResourceFile);
                if (fiCompiledResource.LastWriteTime > fiResource.LastWriteTime)
                    return new ResourceCompilerResult { CompiledResourceFile = compiledResourceFile, Success = true };
            }

            _resourceCompiling = resourceFile;
            _resourceResults = new ResourceCompilerResult { CompiledResourceFile = compiledResourceFile };

            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = _win32ResourceCompiler;
            //    Resgen.exe PibLink_resource.resx PibLink.PibLink_resource.resources /str:cs,PibLink,PibLink_resource
            //    rc.exe /fo test.res test.rc
            processStartInfo.Arguments = "/fo \"" + compiledResourceFile + "\" \"" + resourceFile + "\"";
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.WorkingDirectory = zPath.GetDirectoryName(resourceFile);
            Trace.WriteLine("  \"{0}\" {1}", _win32ResourceCompiler, processStartInfo.Arguments);
            //Trace.WriteLine("  WorkingDirectory \"{0}\"", processStartInfo.WorkingDirectory);

            Process process = new Process();
            process.StartInfo = processStartInfo;
            process.ErrorDataReceived += CompileResource_EventErrorDataReceived;
            process.Start();
            process.BeginErrorReadLine();

            while (!process.HasExited)
            {
            }

            if (process.ExitCode != 0)
            {
                _resourceResults.Messages.Add(new ResourceCompilerMessage { File = resourceFile, Message = string.Format("error compiling win32 resource, exit code {0}", process.ExitCode) });
            }
            else
                _resourceResults.Success = true;

            return _resourceResults;
        }

        private void CompileResource_EventErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
                return;
            string message = e.Data.Trim();
            if (message != "")
                _resourceResults.Messages.Add(new ResourceCompilerMessage { File = _resourceCompiling, Message = message });
        }
    }
}
