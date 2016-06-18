using pb.IO;
using System.Diagnostics;

// Utilisation de Resgen.exe http://msdn.microsoft.com/fr-fr/library/ccec7sz1.aspx
// resgen [parameters] [/compile]filename.extension [outputFilename.extension] [/str:lang[,namespace[,class[,file]]]]

//   /compile
//      Permet de spécifier plusieurs fichiers .resx ou texte à convertir en plusieurs fichiers .resources en une seule opération globale.
//      Si vous omettez cette option, vous ne pouvez spécifier qu'un seul argument de fichier d'entrée.
//      Cette option ne peut pas être utilisée avec l'option /str:.
//   /publicClass
//      Crée un type de ressources fortement typé comme classe public.
//      Cette option est ignorée si l'option /str: n'est pas utilisée.
//   /r:assembly
//      Spécifie que les types doivent être chargés à partir d'un assembly.
//      Si vous spécifiez cette option, un fichier .resx avec une version antérieure d'un type utilisera le type dans un assembly.
//   /str:language[,namespace[,classname[,filename]]]
//      Crée un fichier de classe de ressources fortement typé dans le langage de programmation (cs ou C# pour C#, vb ou visualbasic pour Visual Basic)
//      spécifié dans l'option language.
//      Vous pouvez utiliser l'option namespace pour spécifier l'espace de noms par défaut du projet,
//      l'option classname pour spécifier le nom de la classe générée et l'option filename pour spécifier le nom du fichier de classe.
//      Remarque	Dans le .NET Framework version 2.0, classname et filename sont ignorés si namespace n'est pas spécifié. 
//      Un seul fichier d'entrée est autorisé lorsque l'option /str: est utilisée, afin qu'il ne puisse pas être utilisé avec l'option /compile.
//      Si namespace est spécifié mais que classname ne l'est pas, le nom de la classe est dérivé du nom de fichier de sortie
//      (par exemple, les traits de soulignement sont substitués pour les périodes).Les ressources fortement typées peuvent ne pas fonctionner correctement en conséquence.Pour éviter ce problème, spécifiez à la fois le nom de la classe et le nom du fichier de sortie.
//   /usesourcepath
//      Précise que le répertoire actif du fichier d'entrée sera utilisé pour résoudre des chemins d'accès de fichier relatif.
//
//  commande utilisée :
//    Resgen.exe resource.resx resource.resources /str:cs,PibLink,PibLink_resource
//    Resgen.exe PibLink_resource.resx WRunSource.Class.PibLink.PibLink_resource.resources /str:cs,WRunSource.Class.PibLink,PibLink_resource

namespace pb.Compiler
{
    public class ResourceCompiler
    {
        private string _resourceCompiler = null;
        private string _resourceCompiling = null;
        private ResourceCompilerResult _resourceResults = null;

        public ResourceCompiler(string resourceCompiler)
        {
            _resourceCompiler = resourceCompiler;
        }

        public ResourceCompilerResult Compile(ResourceFile resource, string outputDir)
        {
            if (_resourceCompiler == null)
                throw new PBException("resource compiler is not defined");
            if (!zFile.Exists(_resourceCompiler))
                throw new PBException("resource compiler cannot be found \"{0}\"", _resourceCompiler);
            if (!zFile.Exists(resource.File))
                throw new PBException("resource file cannot be found \"{0}\"", resource.File);

            if (!zDirectory.Exists(outputDir))
                zDirectory.CreateDirectory(outputDir);

            //string resourceFile = resource.File;
            string resourceFilename = zPath.GetFileNameWithoutExtension(resource.File);
            //string nameSpace = null;
            //if (attributes.ContainsKey("namespace"))
            //    nameSpace = attributes["namespace"];
            string compiledResourceFile = resourceFilename + ".resources";
            if (resource.Namespace != null)
                compiledResourceFile = resource.Namespace + "." + compiledResourceFile;
            //"WRunSource.Class.PibLink."
            compiledResourceFile = zpath.PathSetDirectory(compiledResourceFile, outputDir);
            if (zFile.Exists(compiledResourceFile) && zFile.Exists(resource.File))
            {
                var fiResource = zFile.CreateFileInfo(resource.File);
                var fiCompiledResource = zFile.CreateFileInfo(compiledResourceFile);
                if (fiCompiledResource.LastWriteTime > fiResource.LastWriteTime)
                    //return pathCompiledResource;
                    return new ResourceCompilerResult { CompiledResourceFile = compiledResourceFile, Success = true };
            }

            _resourceCompiling = resource.File;
            _resourceResults = new ResourceCompilerResult { CompiledResourceFile = compiledResourceFile };

            //if (_resourceCompiler == null)
            //    throw new PBException("error resource compiler is not defined");
            //if (!zFile.Exists(_resourceCompiler))
            //    throw new PBException("error resource compiler cannot be found {0}", _resourceCompiler);
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = _resourceCompiler;
            //    Resgen.exe PibLink_resource.resx PibLink.PibLink_resource.resources /str:cs,PibLink,PibLink_resource
            processStartInfo.Arguments = resource.File + " " + compiledResourceFile;
            if (resource.Namespace != null)
                processStartInfo.Arguments += " /str:cs," + resource.Namespace + "," + resourceFilename;
            //WriteLine(1, "  {0} {1}", _resourceCompiler, processStartInfo.Arguments);
            Trace.WriteLine("  {0} {1}", _resourceCompiler, processStartInfo.Arguments);
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.WorkingDirectory = zPath.GetDirectoryName(resource.File);

            Process process = new Process();
            process.StartInfo = processStartInfo;
            //process.ErrorDataReceived += new DataReceivedEventHandler(CompileResource_EventErrorDataReceived);
            process.ErrorDataReceived += CompileResource_EventErrorDataReceived;
            process.Start();
            process.BeginErrorReadLine();

            while (!process.HasExited)
            {
            }

            if (process.ExitCode != 0)
            {
                _resourceResults.Messages.Add(new ResourceCompilerMessage { File = resource.File, Message = string.Format("error compiling resource, exit code {0}", process.ExitCode) });
                //_resourceResults.HasError = true;
            }
            else
                _resourceResults.Success = true;

            //return compiledResourceFile;
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
