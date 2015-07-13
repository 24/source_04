using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using PcapDotNet.Core;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Transport;
using pb;
//using Pib.Pcap;

namespace Pib.Pcap.Test
{
    // SortedTcpStreamList
    public class AnalyzeTcpStreamTest
    {
        private List<TcpStreamPacket> _packets = new List<TcpStreamPacket>();
        private SortedList<TcpConnection, TcpStream0> _streams = new SortedList<TcpConnection, TcpStream0>();

        public List<TcpStreamPacket> Packets { get { return _packets; } }
        public SortedList<TcpConnection, TcpStream0> Streams { get { return _streams; } }

        public void Add(PPacket packet)
        {
            if (packet.Tcp == null)
                return;
            TcpStreamPacket streamPacket = new TcpStreamPacket(packet);
            int i = _streams.IndexOfKey(streamPacket.Connection);
            TcpStream0 stream;
            if (i == -1)
            {
                stream = new TcpStream0();
                stream.StreamNumber = _streams.Count + 1;
                _streams.Add(streamPacket.Connection, stream);
            }
            else
            {
                stream = _streams.Values[i];
            }
            streamPacket.StreamNumber = stream.StreamNumber;
            stream.Add(streamPacket);
            _packets.Add(streamPacket);
        }

        public static void Analyze(PacketDevice device)
        {
            AnalyzeTcpStreamTest tcpAnalyze = new AnalyzeTcpStreamTest();

            //__communicator = null;
            //_rs.OnAbortExecution += new OnAbortEvent(OnAbortExecution);
            try
            {
                using (PacketCommunicator communicator = device.Open(65536, PacketDeviceOpenAttributes.Promiscuous, 1000))
                {
                    PPacketManager ppacketManager = new PPacketManager();
                    communicator.ReceivePackets(0, packet => tcpAnalyze.Add(ppacketManager.CreatePPacket(packet)));
                }
            }
            finally
            {
                //_rs.OnAbortExecution -= new OnAbortEvent(OnAbortExecution);
            }
        }
    }

    public class TcpStream0
    {
        private TcpConnection _address = null;
        private TcpStreamState _streamState = new TcpStreamState();
        private TcpData _currentSourceData = null;
        private TcpData _currentDestinationData = null;
        private List<TcpData> _dataList = new List<TcpData>();
        private List<TcpStreamPacket> _packets = new List<TcpStreamPacket>();
        private int _streamNumber = -1;

        public int StreamNumber { get { return _streamNumber; } set { _streamNumber = value; } }

        public void Add(TcpStreamPacket streamPacket)
        {
            if (_address == null)
                _address = streamPacket.Connection;
            if (streamPacket.Connection.OriginalOrder == _address.OriginalOrder)
                streamPacket.Direction = TcpDirection.SourceToDestination;
            else
                streamPacket.Direction = TcpDirection.DestinationToSource;
            _packets.Add(streamPacket);
            AnalyzePacket(streamPacket);
        }

        private void AnalyzePacket(TcpStreamPacket streamPacket)
        {
            //TCP tcp = streamPacket.Packet.TCP;
            TcpDatagram tcp = streamPacket.Packet.Ethernet.IpV4.Tcp;
            streamPacket.PacketType = TcpPacketType.Unknow;
            bool OpenConnectionPacketType = false;
            bool CloseConnectionPacketType = false;
            //if (tcp.Synchronize)
            if (tcp.IsSynchronize)
            {
                OpenConnectionPacketType = true;
                // ****** OpenConnection1 SYN seq = x ******
                //if (!tcp.Ack)
                if (!tcp.IsAcknowledgment)
                {
                    streamPacket.PacketType = TcpPacketType.OpenConnection1;
                    _streamState.OpenState = TcpPacketType.OpenConnection1;
                    _streamState.InitialSourceSequence = _streamState.CurrentSourceSequence = tcp.SequenceNumber;
                    //streamPacket.Message("TCP open 1 : SYN seq = x (0x{0})", tcp.seq.zToHex());
                    if (streamPacket.Direction != TcpDirection.SourceToDestination)
                        //streamPacket.Message("error wrong direction");
                        streamPacket.AddError(TcpPacketError.ErrorBadDirection);
                }
                // ****** OpenConnection2 SYN seq = y, ACK x + 1 ******
                else
                {
                    if (streamPacket.Direction == TcpDirection.DestinationToSource)
                    {
                        streamPacket.PacketType = TcpPacketType.OpenConnection2;
                        _streamState.OpenState = TcpPacketType.OpenConnection2;
                        _streamState.InitialDestinationSequence = _streamState.CurrentDestinationSequence = tcp.SequenceNumber;
                        //streamPacket.Message("TCP open 2 : SYN seq = y (0x{0}), ACK x + 1 (0x{1})", tcp.seq.zToHex(), tcp.ack_seq.zToHex());
                        _streamState.CurrentSourceSequence++;
                        if (tcp.AcknowledgmentNumber != _streamState.CurrentSourceSequence)
                            //streamPacket.Message("warning ack (0x{0}) != source seq + 1 (0x{1})", tcp.ack_seq.zToHex(), CurrentSourceSequence.zToHex());
                            streamPacket.AddError(TcpPacketError.WarningOpenConnection2BadAck);
                        if (_streamState.LastPacketType != TcpPacketType.OpenConnection1)
                            //streamPacket.Message("warning message should be just after open connection step 1");
                            streamPacket.AddError(TcpPacketError.WarningOpenConnection2BadMessagePosition);
                    }
                    else
                        //streamPacket.Message("error wrong direction, TCP open 2, SYN seq = y (0x{0}), ACK x + 1 (0x{1})", tcp.seq.zToHex(), tcp.ack_seq.zToHex());
                        streamPacket.AddError(TcpPacketError.ErrorOpenConnection2BadDirection);
                }
            }
            //else if (tcp.Finish)
            else if (tcp.IsFin)
            {
                CloseConnectionPacketType = true;
                // ****** CloseSourceConnection1 FIN seq = x ******
                if (streamPacket.Direction == TcpDirection.SourceToDestination)
                {
                    streamPacket.PacketType = TcpPacketType.CloseSourceConnection1;
                    _streamState.CloseSourceState = TcpPacketType.CloseSourceConnection1;
                    //streamPacket.Message("TCP close source connection step 1 : FIN seq = x (0x{0})", tcp.seq.zToHex());
                    if (tcp.SequenceNumber != _streamState.CurrentSourceSequence)
                        //streamPacket.Message("warning seq (0x{0}) != source seq (0x{1})", tcp.seq.zToHex(), CurrentSourceSequence.zToHex());
                        streamPacket.AddError(TcpPacketError.WarningCloseSourceConnection1BadSeq);
                    //if (tcp.Ack)
                    if (tcp.IsAcknowledgment)
                        //streamPacket.Message("warning ack should not be set");
                        streamPacket.AddError(TcpPacketError.WarningBadFlagAck);
                }
                // ****** CloseDestinationConnection1 FIN seq = y, ACK x + 1 ******
                else // streamPacket.Direction == TcpDirection.DestinationToSource
                {
                    streamPacket.PacketType = TcpPacketType.CloseDestinationConnection1;
                    _streamState.CloseDestinationState = TcpPacketType.CloseDestinationConnection1;
                    //streamPacket.Message("TCP close destination connection step 1 : FIN seq = y (0x{0}), ACK x + 1 (0x{1})", tcp.seq.zToHex(), tcp.ack_seq.zToHex());
                    if (tcp.SequenceNumber != _streamState.CurrentDestinationSequence)
                        //streamPacket.Message("warning seq (0x{0}) != destination seq (0x{1})", tcp.seq.zToHex(), CurrentDestinationSequence.zToHex());
                        streamPacket.AddError(TcpPacketError.WarningCloseDestinationConnection1BadSeq);
                    //if (!tcp.Ack)
                    if (!tcp.IsAcknowledgment)
                        //streamPacket.Message("warning ack should be set");
                        streamPacket.AddError(TcpPacketError.WarningBadFlagAck);
                    else if (tcp.AcknowledgmentNumber != _streamState.CurrentSourceSequence + 1)
                        //streamPacket.Message("warning ack (0x{0}) != source seq + 1 (0x{1})", tcp.ack_seq.zToHex(), CurrentSourceSequence.zToHex());
                        streamPacket.AddError(TcpPacketError.WarningCloseDestinationConnection1BadAck);
                }
            }
            //else if (tcp.Ack)
            else if (tcp.IsAcknowledgment)
            {
                // ****** OpenConnection3 ACK y + 1 ******
                if (_streamState.LastPacketType == TcpPacketType.OpenConnection2 && streamPacket.Direction == TcpDirection.SourceToDestination)
                {
                    OpenConnectionPacketType = true;
                    streamPacket.PacketType = TcpPacketType.OpenConnection3;
                    _streamState.OpenState = TcpPacketType.OpenConnection3;
                    _streamState.ConnectionOpened = true;
                    _streamState.CurrentDestinationSequence++;
                    //streamPacket.Message("TCP open 3 : ACK y + 1 (0x{0})", tcp.ack_seq.zToHex());
                    if (tcp.AcknowledgmentNumber != _streamState.InitialDestinationSequence + 1)
                        //streamPacket.Message("warning ack (0x{0}) != destination seq + 1 (0x{1})", tcp.ack_seq.zToHex(), CurrentDestinationSequence.zToHex());
                        streamPacket.AddError(TcpPacketError.WarningOpenConnection3BadAck);
                }
                // ****** CloseSourceConnection2 ACK x + 1 ******
                else if (_streamState.CloseSourceState == TcpPacketType.CloseSourceConnection1 && streamPacket.Direction == TcpDirection.DestinationToSource)
                {
                    CloseConnectionPacketType = true;
                    streamPacket.PacketType = TcpPacketType.CloseSourceConnection2;
                    _streamState.CloseSourceState = TcpPacketType.CloseSourceConnection2;
                    //streamPacket.Message("TCP close source connection step 2 : ACK x + 1 (0x{0})", tcp.ack_seq.zToHex());
                    if (tcp.AcknowledgmentNumber != _streamState.CurrentSourceSequence + 1)
                        //streamPacket.Message("warning ack (0x{0}) != source seq + 1 (0x{1})", tcp.ack_seq.zToHex(), CurrentSourceSequence.zToHex());
                        streamPacket.AddError(TcpPacketError.WarningCloseSourceConnection2BadAck);
                }
                // ****** CloseDestinationConnection2 ACK y + 1 ******
                else if (_streamState.CloseDestinationState == TcpPacketType.CloseDestinationConnection1 && streamPacket.Direction == TcpDirection.SourceToDestination)
                {
                    CloseConnectionPacketType = true;
                    streamPacket.PacketType = TcpPacketType.CloseDestinationConnection2;
                    _streamState.CloseDestinationState = TcpPacketType.CloseDestinationConnection2;
                    //streamPacket.Message("TCP close destination connection step 2 : ACK y + 1 (0x{0})", tcp.ack_seq.zToHex());
                    if (tcp.AcknowledgmentNumber != _streamState.CurrentDestinationSequence + 1)
                        //streamPacket.Message("warning ack (0x{0}) != destination seq + 1 (0x{1}) (ACK y + 1)", tcp.ack_seq.zToHex(), CurrentDestinationSequence.zToHex());
                        streamPacket.AddError(TcpPacketError.WarningCloseDestinationConnection2BadAck);
                }
                else
                    streamPacket.PacketType |= TcpPacketType.Ack;
            }

            //if (OpenConnectionPacketType || CloseConnectionPacketType)
            //{
            //    if (tcp.DataLength > 0)
            //    {
            //        if (tcp.psh == 1)
            //            streamPacket.Message("error data with push option cannot be send with syn flag");
            //        else
            //            streamPacket.Message("error data with cannot be send with syn flag");
            //    }
            //    if (tcp.psh == 1)
            //        streamPacket.Message("error push option without data cannot be send with syn flag");
            //}

            //if (tcp.Push)
            if (tcp.IsPush)
                streamPacket.PacketType |= TcpPacketType.Push;
            if (tcp.PayloadLength > 0)
                streamPacket.PacketType |= TcpPacketType.Data;
            _streamState.LastPacketType = streamPacket.PacketType;
            streamPacket.StreamState = _streamState;
        }
    }

    public class TcpData
    {
        private TcpDirection _direction;
        private List<byte> _data = new List<byte>();
        private uint _initialSequence;
        private uint _currentSequence;
        private long _sequencePosition;
        private long _firstMissingBytePosition;
        private long _lastBytePosition;
        private SortedList<long, DataSegment> _dataSegments = new SortedList<long, DataSegment>();
        private List<TcpStreamPacket> _dataPackets = new List<TcpStreamPacket>();

        //public TcpData(Packet packet)
        //{
        //}

        public void Add(Packet packet)
        {
        }
    }

    [Flags]
    public enum TcpPacketType
    {
        Unknow = 0x0000,
        OpenConnection1 = 0x0001,     // --> SYN seq = x
        OpenConnection2 = 0x0002,     // <-- SYN seq = y, ACK x + 1
        OpenConnection3 = 0x0003,     // --> ACK y + 1
        CloseSourceConnection1 = 0x0004,     // --> FIN seq = x
        CloseSourceConnection2 = 0x0005,     // <-- ACK x + 1
        CloseDestinationConnection1 = 0x0006,     // <-- FIN seq = y, ACK x + 1
        CloseDestinationConnection2 = 0x0007,     // <-- ACK y + 1
        Data = 0x0010,
        Push = 0x0020,
        Ack = 0x0040
    }

    public class TcpStreamState
    {
        public TcpPacketType OpenState = TcpPacketType.Unknow;
        public TcpPacketType CloseSourceState = TcpPacketType.Unknow;
        public TcpPacketType CloseDestinationState = TcpPacketType.Unknow;
        public bool ConnectionOpened = false;
        public uint InitialSourceSequence = 0;
        public uint InitialDestinationSequence = 0;
        public uint CurrentSourceSequence = 0;
        public uint CurrentDestinationSequence = 0;
        public TcpPacketType LastPacketType = TcpPacketType.Unknow;
    }

    public enum TcpPacketError
    {
        ErrorBadDirection = 1001,
        WarningBadFlagAck = 1002,
        WarningOpenConnection2BadAck = 2001,
        WarningOpenConnection2BadMessagePosition = 2002,
        ErrorOpenConnection2BadDirection = 2003,
        WarningOpenConnection3BadAck = 2011,
        WarningCloseSourceConnection1BadSeq = 2051,
        WarningCloseSourceConnection2BadAck = 2061,
        WarningCloseDestinationConnection1BadSeq = 2071,
        WarningCloseDestinationConnection1BadAck = 2072,
        WarningCloseDestinationConnection2BadAck = 2081
    }

    public class TcpStreamPacket
    {
        private TcpDirection _direction;
        private TcpConnection _connection;
        private TcpPacketType _packetType = TcpPacketType.Unknow;
        private Packet _packet;
        private PPacket _ppacket;
        private TcpStreamState _streamState = null;
        private List<TcpPacketError> _errors = new List<TcpPacketError>();
        private int _streamNumber = -1;

        public TcpStreamPacket(Packet packet)
        {
            _direction = TcpDirection.SourceToDestination;
            _connection = new TcpConnection(packet);
            _packet = packet;
        }

        public TcpStreamPacket(PPacket ppacket)
        {
            _direction = TcpDirection.SourceToDestination;
            //_connection = new TcpConnection(packet.Packet);
            _connection = ppacket.GetTcpConnection();
            _ppacket = ppacket;
            _packet = ppacket.Packet;
        }

        public TcpDirection Direction { get { return _direction; } set { _direction = value; } }
        public TcpConnection Connection { get { return _connection; } }
        public TcpPacketType PacketType { get { return _packetType; } set { _packetType = value; } }
        public Packet Packet { get { return _packet; } }
        public PPacket PPacket { get { return _ppacket; } }
        public TcpStreamState StreamState { get { return _streamState; } set { _streamState = value; } }
        public List<TcpPacketError> Errors { get { return _errors; } }
        public string PacketDescription
        {
            get
            {
                string s = null;
                //TCP tcp = Packet.TCP;
                //TcpDatagram tcp = _packet.IpV4.Tcp;
                //TcpDatagram tcp = _ppacket.Tcp;
                TcpDatagram tcp = _packet.Ethernet.IpV4.Tcp;
                switch (_packetType)
                {
                    case TcpPacketType.OpenConnection1:
                        s = string.Format("TCP open 1 : SYN seq = x (0x{0})", tcp.SequenceNumber.zToHex());
                        break;
                    case TcpPacketType.OpenConnection2:
                        s = string.Format("TCP open 2 : SYN seq = y (0x{0}), ACK x + 1 (0x{1})", tcp.SequenceNumber.zToHex(), tcp.AcknowledgmentNumber.zToHex());
                        break;
                    case TcpPacketType.OpenConnection3:
                        s = string.Format("TCP open 3 : ACK y + 1 (0x{0})", tcp.AcknowledgmentNumber.zToHex());
                        break;
                    case TcpPacketType.CloseSourceConnection1:
                        s = string.Format("TCP close source 1 : FIN seq = x (0x{0})", tcp.SequenceNumber.zToHex());
                        break;
                    case TcpPacketType.CloseSourceConnection2:
                        s = string.Format("TCP close source 2 : ACK x + 1 (0x{0})", tcp.AcknowledgmentNumber.zToHex());
                        break;
                    case TcpPacketType.CloseDestinationConnection1:
                        s = string.Format("TCP close destination 1 : FIN seq = y (0x{0}), ACK x + 1 (0x{1})", tcp.SequenceNumber.zToHex(), tcp.AcknowledgmentNumber.zToHex());
                        break;
                    case TcpPacketType.CloseDestinationConnection2:
                        s = string.Format("TCP close destination 2 : ACK y + 1 (0x{0})", tcp.AcknowledgmentNumber.zToHex());
                        break;
                }
                //TCP_PacketType.Ack;
                //TCP_PacketType.Push;
                //TCP_PacketType.Data;
                return s;
            }
        }
        public int StreamNumber { get { return _streamNumber; } set { _streamNumber = value; } }

        //public bool IsTcp()
        //{
        //    if (_packet.Ethernet != null && _packet.Ethernet.IpV4 != null && _packet.Ethernet.IpV4.Tcp != null)
        //        return true;
        //    else
        //        return false;
        //}

        public void AddError(TcpPacketError error)
        {
            _errors.Add(error);
        }

        public void XmlExport(XmlWriter writer)
        {
            writer.WriteStartElement("Packet");
            //writer.WriteAttributeString("Number", _packet.PacketNumber.ToString());
            //writer.WriteAttributeString("Time", _packet.RelativeTime.TotalSeconds.ToString("000000"));
            //string source = _packet.IpV4.Source.ToString() + ":" + _packet.IpV4.Tcp.SourcePort.ToString();
            //string source = _ppacket.Ipv4.Source.ToString() + ":" + _ppacket.Tcp.SourcePort.ToString();
            IpV4Datagram ip = _packet.Ethernet.IpV4;
            TcpDatagram tcp = ip.Tcp;
            string source = ip.Source.ToString() + ":" + tcp.SourcePort.ToString();
            //string destination = _packet.IpV4.Destination.ToString() + ":" + _packet.IpV4.Tcp.DestinationPort.ToString();
            string destination = ip.Destination.ToString() + ":" + tcp.DestinationPort.ToString();
            if (Direction == TcpDirection.DestinationToSource)
            {
                string s = source;
                source = destination;
                destination = s;
            }
            writer.WriteAttributeString("Source", source);
            writer.WriteAttributeString("Dir", TcpDirection.SourceToDestination.ToString());
            writer.WriteAttributeString("Destination", destination);
            //writer.WriteAttributeString("Protocol", _packet.ProtocolCode);
            //writer.WriteAttributeString("Flags", _packet.TCP.GetFlagsString());
            writer.WriteAttributeString("Data", "0x" + tcp.PayloadLength.zToHex());
            writer.WriteAttributeString("Sequence", "0x" + tcp.SequenceNumber.zToHex());
            writer.WriteAttributeString("AckSequence", "0x" + tcp.AcknowledgmentNumber.zToHex());
            writer.WriteAttributeString("Window", "0x" + tcp.Window.zToHex());
            writer.WriteAttributeString("UrgentPointer", "0x" + tcp.UrgentPointer.zToHex());
            writer.WriteAttributeString("Message", PacketDescription);
            foreach (TcpPacketError error in _errors)
            {
                writer.WriteStartElement("Error");
                writer.WriteAttributeString("Error", GetErrorMessage(error));
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        public void XmlExportFormated(XmlWriter writer)
        {
            string s;

            writer.WriteStartElement("Packet");
            //writer.WriteAttributeString("Number", string.Format("{0,4}", _packet.PacketNumber));
            //writer.WriteAttributeString("Time", _packet.RelativeTime.TotalSeconds.ToString("000000"));
            IpV4Datagram ip = _packet.Ethernet.IpV4;
            TcpDatagram tcp = ip.Tcp;
            string source = ip.Source.ToString() + ":" + tcp.SourcePort.ToString();
            string destination = ip.Destination.ToString() + ":" + tcp.DestinationPort.ToString();
            string dir;
            if (Direction == TcpDirection.SourceToDestination)
                dir = "--»";
            else
            {
                dir = "«--";
                s = source;
                source = destination;
                destination = s;
            }
            writer.WriteAttributeString("Source", source);
            writer.WriteAttributeString("Dir", dir);
            writer.WriteAttributeString("Destination", destination);
            //writer.WriteAttributeString("Protocol", _packet.ProtocolCode);
            //writer.WriteAttributeString("Flags", string.Format("{0,-20}", _packet.IpV4.Tcp.GetFlagsString()));
            if (tcp.PayloadLength > 0) s = "0x" + tcp.PayloadLength.zToHex(); else s = "      ";
            writer.WriteAttributeString("Data", s);
            if (tcp.SequenceNumber != 0) s = "0x" + tcp.SequenceNumber.zToHex(); else s = "          ";
            writer.WriteAttributeString("Sequence", s);
            if (tcp.AcknowledgmentNumber != 0) s = "0x" + tcp.AcknowledgmentNumber.zToHex(); else s = "          ";
            writer.WriteAttributeString("AckSequence", s);
            if (tcp.Window != 0) s = "0x" + tcp.Window.zToHex(); else s = "      ";
            writer.WriteAttributeString("Window", s);
            if (tcp.UrgentPointer != 0) s = "0x" + tcp.UrgentPointer.zToHex(); else s = "      ";
            writer.WriteAttributeString("UrgentPointer", s);
            writer.WriteAttributeString("Message", PacketDescription);
            foreach (TcpPacketError error in _errors)
            {
                writer.WriteStartElement("Error");
                writer.WriteAttributeString("Error", GetErrorMessage(error));
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        public string GetErrorMessage(TcpPacketError error)
        {
            //TCP tcp = Packet.TCP;
            TcpDatagram tcp = _packet.Ethernet.IpV4.Tcp;
            switch (error)
            {
                case TcpPacketError.ErrorBadDirection:
                    return "error wrong direction";
                case TcpPacketError.WarningBadFlagAck:
                    return "warning ack should not be set";
                case TcpPacketError.WarningOpenConnection2BadAck:
                    return string.Format("warning ack (0x{0}) != source seq + 1 (0x{1})", tcp.AcknowledgmentNumber.zToHex(), (StreamState.CurrentSourceSequence - 1).zToHex());
                case TcpPacketError.WarningOpenConnection2BadMessagePosition:
                    return "warning message should be just after open connection step 1";
                case TcpPacketError.ErrorOpenConnection2BadDirection:
                    return string.Format("error wrong direction, TCP open 2, SYN seq = y (0x{0}), ACK x + 1 (0x{1})", tcp.SequenceNumber.zToHex(), tcp.AcknowledgmentNumber.zToHex());
                case TcpPacketError.WarningOpenConnection3BadAck:
                    return string.Format("warning ack (0x{0}) != destination seq + 1 (0x{1})", tcp.AcknowledgmentNumber.zToHex(), StreamState.CurrentDestinationSequence.zToHex());
                case TcpPacketError.WarningCloseSourceConnection1BadSeq:
                    return string.Format("warning seq (0x{0}) != source seq (0x{1})", tcp.SequenceNumber.zToHex(), StreamState.CurrentSourceSequence.zToHex());
                case TcpPacketError.WarningCloseSourceConnection2BadAck:
                    return string.Format("warning ack (0x{0}) != source seq + 1 (0x{1})", tcp.AcknowledgmentNumber.zToHex(), StreamState.CurrentSourceSequence.zToHex());
                case TcpPacketError.WarningCloseDestinationConnection1BadSeq:
                    return string.Format("warning seq (0x{0}) != destination seq (0x{1})", tcp.SequenceNumber.zToHex(), StreamState.CurrentDestinationSequence.zToHex());
                case TcpPacketError.WarningCloseDestinationConnection1BadAck:
                    return string.Format("warning ack (0x{0}) != source seq + 1 (0x{1})", tcp.AcknowledgmentNumber.zToHex(), StreamState.CurrentSourceSequence.zToHex());
                case TcpPacketError.WarningCloseDestinationConnection2BadAck:
                    return string.Format("warning ack (0x{0}) != destination seq + 1 (0x{1}) (ACK y + 1)", tcp.AcknowledgmentNumber.zToHex(), StreamState.CurrentDestinationSequence.zToHex());
                default:
                    return null;
            }
        }
    }
}
