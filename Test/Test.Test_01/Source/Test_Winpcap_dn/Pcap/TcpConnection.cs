using System;
using System.Collections.Generic;
using System.Text;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Transport;

namespace Pib.Pcap
{
    public class TcpAddress : IComparable<TcpAddress>
    {
        private MacAddress _macAddress;
        private IpV4Address _ipAddress;
        private ushort _port;
        //private uint _addressValue;
        private ulong _addressValue;

        //public TcpAddress(IpV4Address address, ushort port)
        public TcpAddress(MacAddress macAddress, IpV4Address ipAddress, ushort port)
        {
            _macAddress = macAddress;
            _ipAddress = ipAddress;
            _port = port;
            _addressValue = ipAddress.ToValue() + ((ulong)port << 32);
        }

        public MacAddress MacAddress { get { return _macAddress; } }
        public IpV4Address IpAddress { get { return _ipAddress; } }
        public ushort Port { get { return _port; } }

        public int CompareTo(TcpAddress other)
        {
            if (_addressValue < other._addressValue)
                return -1;
            if (_addressValue > other._addressValue)
                return 1;
            //if (_port < other._port)
            //    return -1;
            //if (_port > other._port)
            //    return 1;
            return 0;
        }

        public static bool operator <(TcpAddress a1, TcpAddress a2)
        {
            if (a1.CompareTo(a2) < 0)
                return true;
            else
                return false;
        }

        public static bool operator >(TcpAddress a1, TcpAddress a2)
        {
            if (a1.CompareTo(a2) > 0)
                return true;
            else
                return false;
        }

        public static bool operator ==(TcpAddress a1, TcpAddress a2)
        {
            return a1.CompareTo(a2) == 0;
        }

        public static bool operator !=(TcpAddress a1, TcpAddress a2)
        {
            return a1.CompareTo(a2) != 0;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TcpAddress))
                return false;
            TcpAddress address = (TcpAddress)obj;
            return CompareTo(address) == 0;
        }

        public override int GetHashCode()
        {
            return ((_ipAddress.ToValue().GetHashCode() ^ _port.GetHashCode()) as object).GetHashCode();
        }
    }

    public class TcpConnection : IComparable<TcpConnection>
    {
        private int _index = 0;
        private TcpAddress _source;
        private TcpAddress _destination;

        private TcpAddress _address1; // la plus petite entre _source et _destination
        private TcpAddress _address2; // la plus grande entre _source et _destination

        private bool _originalOrder = true;  // si true  : _address1 = _source et _address2 = _destination sinon false
        private string _connectionName = null;


        public TcpConnection(TcpAddress source, TcpAddress destination)
        {
            _source = source;
            _destination = destination;
            SetOrder();
        }

        public TcpConnection(Packet packet)
        {
            if (packet.Ethernet == null)
                throw new Exception("error creating TcpStreamAddress packet is not ethernet");
            EthernetDatagram ethernet = packet.Ethernet;
            //if (ip == null)
            if (ethernet.EtherType != PcapDotNet.Packets.Ethernet.EthernetType.IpV4)
                throw new Exception("error creating TcpStreamAddress packet is not ipv4");
            IpV4Datagram ip = ethernet.IpV4;
            //if (tcp == null)
            if (ip.Protocol != IpV4Protocol.Tcp)
                throw new Exception("error creating TcpStreamAddress packet is not tcp");
            TcpDatagram tcp = ip.Tcp;
            //_source = new TcpAddress(ip.Source, tcp.SourcePort);
            _source = new TcpAddress(ethernet.Source, ip.Source, tcp.SourcePort);
            //_destination = new TcpAddress(ip.Destination, tcp.DestinationPort);
            _destination = new TcpAddress(ethernet.Destination, ip.Destination, tcp.DestinationPort);
            SetOrder();
        }

        private void SetOrder()
        {
            if (_source < _destination)
            {
                _address1 = _source;
                _address2 = _destination;
                _originalOrder = true;
            }
            else
            {
                _address1 = _destination;
                _address2 = _source;
                _originalOrder = false;
            }
        }

        public int Index { get { return _index; } set { _index = value; } }
        public TcpAddress Source { get { return _source; } }
        public TcpAddress Destination { get { return _destination; } }
        public TcpAddress Address1 { get { return _address1; } }
        public TcpAddress Address2 { get { return _address2; } }
        public bool OriginalOrder { get { return _originalOrder; } }

        public string GetConnectionName()
        {
            if (_connectionName == null)
                //_connectionName = string.Format("{0}.{1}-{2}.{3}", _source.IpAddress, _source.Port, _destination.IpAddress, _destination.Port);
                _connectionName = string.Format("{0}.{1:00000}-{2}.{3:00000}", _source.IpAddress.zToFormatedString(), _source.Port, _destination.IpAddress.zToFormatedString(), _destination.Port);
            return _connectionName;
        }

        public TcpDirection GetTcpDirection(TcpConnection tcpConnection)
        {
            if (tcpConnection._source == _source && tcpConnection._destination == _destination)
                return TcpDirection.SourceToDestination;
            else
                return TcpDirection.DestinationToSource;
        }

        public int CompareTo(TcpConnection other)
        {
            if (_address1 < other._address1)
                return -1;
            if (_address1 > other._address1)
                return 1;
            if (_address2 < other._address2)
                return -1;
            if (_address2 > other._address2)
                return 1;
            return 0;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TcpConnection))
                return false;
            TcpConnection address = (TcpConnection)obj;
            return CompareTo(address) == 0;
        }

        public override int GetHashCode()
        {
            //return ((_address1.Address.ToValue().GetHashCode() ^ _address1.Port.GetHashCode()) as object).GetHashCode() ^
            //    ((_address2.Address.ToValue().GetHashCode() ^ _address2.Port.GetHashCode()) as object).GetHashCode();
            return _address1.GetHashCode() ^ _address2.GetHashCode();
        }

        public override string ToString()
        {
            return GetConnectionName();
        }
    }
}
