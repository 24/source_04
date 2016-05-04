using System.Collections.Generic;

namespace runsourced
{
    public class RunSourceRestartParameters
    {
        public string SourceFile;
        public int SelectionStart;
        //public int SelectionLength;
        public int SelectionEnd;
    }

    public delegate void UpdateRunsourceFilesEvent(Dictionary<string, List<string>> projectFiles);
    public delegate void SetRestartRunsourceEvent(RunSourceRestartParameters runSourceParameters);
}
