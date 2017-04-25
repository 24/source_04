namespace pb.Compiler
{
    public partial class RemoteRunSource
    {
        private string _runSourceVSProjectManagerClassName = "pb.Compiler.RunSourceVSProjectManager";

        public IRunSourceVSProjectManager CreateRunSourceVSProjectManager()
        {
            return (IRunSourceVSProjectManager)GetAppDomain().CreateInstanceFromAndUnwrap(_runsourceDllFilename, _runSourceVSProjectManagerClassName);
        }
    }
}
