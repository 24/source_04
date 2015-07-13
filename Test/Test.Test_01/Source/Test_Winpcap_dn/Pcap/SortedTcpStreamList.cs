using System;
using System.Collections.Generic;
using System.Text;
using PcapDotNet.Packets;

namespace Pib.Pcap
{
    //public class IndexedTcpConnection : TcpConnection
    //{
    //    public IndexedTcpConnection(TcpAddress source, TcpAddress destination) : base(source, destination)
    //    {
    //    }

    //    public IndexedTcpConnection(Packet packet) : base (packet)
    //    {
    //    }

    //    public int Index { get; set; }
    //}

    public class SortedTcpStreamList
    {
        private int _totalPacketsCount = 0;
        private int _selectionnedPacketsCount = 0;
        private int _tcpConnectionIndex = 0;
        //private SortedList<IndexedTcpConnection, List<PPacket>> _tcpStreamPacketsList = new SortedList<IndexedTcpConnection, List<PPacket>>();
        private SortedList<TcpConnection, List<PPacket>> _tcpStreamPacketsList = new SortedList<TcpConnection, List<PPacket>>();

        public int TotalPacketsCount { get { return _totalPacketsCount; } }
        public int SelectionnedPacketsCount { get { return _selectionnedPacketsCount; } }
        public SortedList<TcpConnection, List<PPacket>> TcpStreamPacketsList { get { return _tcpStreamPacketsList; } }

        public void Add(PPacket ppacket)
        {
            _totalPacketsCount++;
            if (ppacket.Tcp == null)
                return;
            _selectionnedPacketsCount++;
            //IndexedTcpConnection tcpConnection = new IndexedTcpConnection(ppacket.Packet);
            TcpConnection tcpConnection = ppacket.GetTcpConnection();
            int i = _tcpStreamPacketsList.IndexOfKey(tcpConnection);
            List<PPacket> ppacketList;
            if (i == -1)
            {
                tcpConnection.Index = ++_tcpConnectionIndex;
                ppacketList = new List<PPacket>();
                _tcpStreamPacketsList.Add(tcpConnection, ppacketList);
            }
            else
            {
                tcpConnection.Index = _tcpStreamPacketsList.Keys[i].Index;
                ppacketList = _tcpStreamPacketsList.Values[i];
            }
            ppacketList.Add(ppacket);
        }
    }
}
