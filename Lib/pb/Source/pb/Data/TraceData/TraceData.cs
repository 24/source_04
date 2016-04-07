using System;
using System.Collections.Generic;

namespace pb.Data.TraceData
{
    public class TraceData
    {
        private static TraceData __currentTraceData = new TraceData();
        private Dictionary<string, TraceDataWriter> _writers = new Dictionary<string, TraceDataWriter>();

        public static TraceData CurrentTraceData { get { return __currentTraceData; } }

        public void AddWriter(string name, TraceDataWriter writer)
        {
            if (_writers.ContainsKey(name))
                throw new PBException("can't add writer, writer \"{0}\" already exist", name);
            _writers.Add(name, writer);
        }

        public void RemoveWriter(string name)
        {
            if (!_writers.ContainsKey(name))
                throw new PBException("can't remove writer, writer \"{0}\" don't exist", name);
            _writers.Remove(name);
        }

        public void Trace<T>(T data, Exception ex = null)
        {
            foreach (TraceDataWriter writer in _writers.Values)
                writer.Write(data, ex);
        }
    }
}
