using System.Collections.Generic;

namespace pb.Data.TraceData
{
    public class TraceDataRegistry
    {
        private static TraceDataRegistry __currentTraceDataRegistry = new TraceDataRegistry();
        private Dictionary<string, ITraceData> _traceDataRegistry = new Dictionary<string, ITraceData>();

        public static TraceDataRegistry CurrentTraceDataRegistry { get { return __currentTraceDataRegistry; } }

        public void Register(string name, ITraceData traceData)
        {
            if (_traceDataRegistry.ContainsKey(name))
                //throw new PBException("can't register TraceData, TraceData \"{0}\" already exist", name);
                _traceDataRegistry.Remove(name);
            _traceDataRegistry.Add(name, traceData);
        }

        public void Unregister(string name)
        {
            //if (!_traceDataRegistry.ContainsKey(name))
            //    throw new PBException("can't unregister TraceData, TraceData \"{0}\" dont exist", name);
            if (_traceDataRegistry.ContainsKey(name))
                _traceDataRegistry.Remove(name);
        }

        public void Activate(string name, TraceData traceData = null)
        {
            if (!_traceDataRegistry.ContainsKey(name))
                throw new PBException("can't activate TraceData, TraceData \"{0}\" dont exist", name);
            if (traceData == null)
                traceData = TraceData.CurrentTraceData;
            _traceDataRegistry[name].ActivateTraceData(traceData);
        }

        public void Desactivate(string name)
        {
            if (!_traceDataRegistry.ContainsKey(name))
                throw new PBException("can't deactivate TraceData, TraceData \"{0}\" dont exist", name);
            _traceDataRegistry[name].DesactivateTraceData();
        }
    }
}
