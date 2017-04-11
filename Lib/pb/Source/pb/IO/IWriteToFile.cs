namespace pb.IO
{
    // used in TraceManager
    public interface IWriteToFile
    {
        string File { get; }
        void Close();
        void Write(string msg);
        //void Write(string msg, params object[] prm);
        //void WriteLine();
        //void WriteLine(string msg);
        //void WriteLine(string msg, params object[] prm);
    }
}
