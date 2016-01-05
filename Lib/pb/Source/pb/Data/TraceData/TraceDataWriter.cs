namespace pb.Data.TraceData
{
    public abstract class TraceDataWriter
    {
        public abstract void Write<T>(T data);
    }
}
