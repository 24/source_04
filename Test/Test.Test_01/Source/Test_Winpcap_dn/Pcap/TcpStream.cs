using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PcapDotNet.Core;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Transport;

namespace Pib.Pcap
{
    public enum TcpDirection
    {
        SourceToDestination = 1,
        DestinationToSource
    }

    public class AnalyzeTcpStream
    {
        private static bool _trace = false;
        private string _directory = null;
        private Dictionary<TcpConnection, TcpStream> _tcpStreamList = new Dictionary<TcpConnection, TcpStream>();
        //private List<FileStream> _fileStreams = new List<FileStream>();
        //private List<WriteTcpStreamToFile> _tcpStreamToFileList = new List<WriteTcpStreamToFile>();
        private PPacketManager _ppacketManager = null;

        public static bool Trace { get { return _trace; } set { _trace = value; } }

        public static void Analyze(ReceivePackets receivePackets, string directory)
        {
            AnalyzeTcpStream tcpReconstruct = new AnalyzeTcpStream();
            tcpReconstruct._Analyze(receivePackets, directory);
        }

        private void _Analyze(ReceivePackets receivePackets, string directory)
        {
            _directory = directory;
            _ppacketManager = new PPacketManager();
            try
            {
                receivePackets.Receive(PacketHandle);
            }
            finally
            {
                foreach(TcpStream tcpStream in _tcpStreamList.Values)
                    tcpStream.Dispose();
            }
        }

        //private int _packetIndex = 1;
        private void PacketHandle(PPacket ppacket)
        {
            //if (ppacket.Packet.Ethernet == null || ppacket.Packet.Ethernet.IpV4 == null || ppacket.Packet.Ethernet.IpV4.Tcp == null)
            if (ppacket.Ipv4 == null || ppacket.Tcp == null)
                return;

            //TcpStreamPacket tcpStreamPacket = new TcpStreamPacket(packet);
            //TcpConnection tcpConnection = tcpStreamPacket.Connection;
            //TcpConnection tcpConnection = new TcpConnection(ppacket.Packet);
            TcpConnection tcpConnection = ppacket.GetTcpConnection();
            if (!_tcpStreamList.ContainsKey(tcpConnection))
            {
                TcpStream tcpStream = new TcpStream(tcpConnection);
                _tcpStreamList.Add(tcpConnection, tcpStream);

                //WriteTcpStreamToFile writeTcpStreamToFile = new WriteTcpStreamToFile(tcpStream, _directory);
                //_tcpStreamToFileList.Add(writeTcpStreamToFile);
                //new CreatePacketTcpStream(tcpStream).TcpPacketHandle = (direction, tcpPacket) => { w.PrintPacketHandler2(_ppacketManager.CreatePPacket(tcpPacket), true); };
                //if (_trace)
                //    new TraceTcpStream(tcpStream);

                tcpStream.zWriteTcpStreamToFile(_directory);
                //tcpStream.CreatePacketTcpStream((direction, tcpPacket) => { w.PrintPacketHandler2(_ppacketManager.CreatePPacket(tcpPacket), true); });
                tcpStream.zTcpStreamReassembledPacket((direction, tcpPacket) => { PrintPacket.PrintPacket1(_ppacketManager.CreatePPacket(tcpPacket), true); });
                if (_trace)
                    tcpStream.zTraceTcpStream(_directory);
            }
            //Trace.CurrentTrace.WriteLine("packet no {0} {1}:{2} - {3}:{4}", _packetIndex++, tcpConnection.Source.Address, tcpConnection.Source.Port, tcpConnection.Destination.Address, tcpConnection.Destination.Port);
            //_tcpStreamList[tcpConnection].Add(tcpStreamPacket);
            //_tcpStreamList[tcpConnection].Add(tcpConnection, ppacket);
            _tcpStreamList[tcpConnection].Add(ppacket);
        }
    }

    public class TcpStreamReassembledPacket : IDisposable
    {
        private TcpConnection _tcpConnection = null;
        private MemoryStream _streamRequest = null;
        private DateTime _requestTimestamp = DateTime.MinValue;
        private MemoryStream _streamReply = null;
        private DateTime _replyTimestamp = DateTime.MinValue;
        public Action<TcpDirection, Packet> PacketHandle = null;

        public TcpStreamReassembledPacket(TcpStream tcpStream)
        {
            _tcpConnection = tcpStream.TcpConnection;
            tcpStream.WriteData += WriteData;
        }

        public void Dispose()
        {
            Close();
        }

        public void Close()
        {
            CloseStreamRequest();
            CloseStreamReply();
        }

        public void CloseStreamRequest()
        {
            if (_streamRequest != null)
            {
                _streamRequest.Close();
                _streamRequest = null;
                _requestTimestamp = DateTime.MinValue;
            }
        }

        public void CloseStreamReply()
        {
            if (_streamReply != null)
            {
                _streamReply.Close();
                _streamReply = null;
                _replyTimestamp = DateTime.MinValue;
            }
        }

        private void CreateRequestPacket()
        {
            if (_streamRequest != null)
            {
                _streamRequest.Close();
                byte[] data = _streamRequest.ToArray();
                _streamRequest = null;
                EthernetLayer ethernetLayer = new EthernetLayer { Source = _tcpConnection.Source.MacAddress, Destination = _tcpConnection.Destination.MacAddress };
                IpV4Layer ipV4Layer = new IpV4Layer
                {
                    Source = _tcpConnection.Source.IpAddress,
                    CurrentDestination = _tcpConnection.Destination.IpAddress,
                    Fragmentation = IpV4Fragmentation.None,
                    //HeaderChecksum = null, // Will be filled automatically.
                    Identification = 123,
                    Options = IpV4Options.None,
                    //Protocol = null, // Will be filled automatically.
                    Ttl = 100,
                    TypeOfService = 0
                };
                TcpLayer tcpLayer = new TcpLayer()
                {
                    SourcePort = _tcpConnection.Source.Port,
                    DestinationPort = _tcpConnection.Destination.Port,
                    //Checksum = null, // Will be filled automatically.
                    SequenceNumber = 100,
                    AcknowledgmentNumber = 50,
                    ControlBits = TcpControlBits.Acknowledgment,
                    Window = 100,
                    UrgentPointer = 0,
                    Options = TcpOptions.None
                };
                PayloadLayer payloadLayer = new PayloadLayer { Data = new Datagram(data) };
                PacketBuilder packetBuilder = new PacketBuilder(ethernetLayer, ipV4Layer, tcpLayer, payloadLayer);
                if (PacketHandle != null)
                    PacketHandle(TcpDirection.SourceToDestination, packetBuilder.Build(_requestTimestamp));
                _requestTimestamp = DateTime.MinValue;
            }
        }

        private void CreateRequestReply()
        {
            if (_streamReply != null)
            {
                _streamReply.Close();
                byte[] data = _streamReply.ToArray();
                _streamReply = null;
                EthernetLayer ethernetLayer = new EthernetLayer { Source = _tcpConnection.Destination.MacAddress, Destination = _tcpConnection.Source.MacAddress };
                IpV4Layer ipV4Layer = new IpV4Layer
                {
                    Source = _tcpConnection.Destination.IpAddress,
                    CurrentDestination = _tcpConnection.Source.IpAddress,
                    Fragmentation = IpV4Fragmentation.None,
                    //HeaderChecksum = null, // Will be filled automatically.
                    Identification = 123,
                    Options = IpV4Options.None,
                    //Protocol = null, // Will be filled automatically.
                    Ttl = 100,
                    TypeOfService = 0
                };
                TcpLayer tcpLayer = new TcpLayer()
                {
                    SourcePort = _tcpConnection.Destination.Port,
                    DestinationPort = _tcpConnection.Source.Port,
                    //Checksum = null, // Will be filled automatically.
                    SequenceNumber = 100,
                    AcknowledgmentNumber = 50,
                    ControlBits = TcpControlBits.Acknowledgment,
                    Window = 100,
                    UrgentPointer = 0,
                    Options = TcpOptions.None
                };
                PayloadLayer payloadLayer = new PayloadLayer { Data = new Datagram(data) };
                PacketBuilder packetBuilder = new PacketBuilder(ethernetLayer, ipV4Layer, tcpLayer, payloadLayer);
                if (PacketHandle != null)
                    PacketHandle(TcpDirection.DestinationToSource, packetBuilder.Build(_replyTimestamp));
                _replyTimestamp = DateTime.MinValue;
            }
        }

        private void WriteData(TcpDirection direction, TcpStreamOperation operation, byte[] data, TcpStreamWriteDataInfo info)
        {
            if (direction == TcpDirection.SourceToDestination)
            {
                if (operation == TcpStreamOperation.Data)
                {
                    if (_streamRequest == null)
                    {
                        _streamRequest = new MemoryStream();
                        _requestTimestamp = info.PPacket.Packet.Timestamp;
                    }
                    _streamRequest.Write(data, 0, data.Length);
                }
                else if (operation == TcpStreamOperation.EndOfData)
                {
                    CreateRequestPacket();
                    CloseStreamRequest();
                }
            }
            else
            {
                if (operation == TcpStreamOperation.Data)
                {
                    if (_streamReply == null)
                    {
                        _streamReply = new MemoryStream();
                        _replyTimestamp = info.PPacket.Packet.Timestamp;
                    }
                    _streamReply.Write(data, 0, data.Length);
                }
                else if (operation == TcpStreamOperation.EndOfData)
                {
                    CreateRequestReply();
                    CloseStreamReply();
                }
            }
        }
    }

    public class TcpFragment
    {
        public uint sequenceNumber = 0;
        public uint length = 0;
        public uint dataLength = 0;
        public byte[] data = null;
        public PPacket ppacket = null;
        public TcpFragment next = null;
    }

    public enum TcpStreamOperation
    {
        Data = 1,
        EndOfData
    }

    public class TcpStream : IDisposable
    {
        private TcpConnection _tcpConnection = null;
        private TcpMonoStream _streamSourceToDestination = null;
        private bool _dataSourceToDestination = false;
        private TcpMonoStream _streamDestinationToSource = null;
        private bool _dataDestinationToSource = false;
        public Action<TcpDirection, TcpStreamOperation, byte[], TcpStreamWriteDataInfo> WriteData;
        public Action OnDispose;

        public TcpStream(TcpConnection tcpConnection)
        {
            _tcpConnection = tcpConnection;
            _streamSourceToDestination = new TcpMonoStream();
            _streamSourceToDestination.WriteData = WriteDataSourceToDestination;
            _streamDestinationToSource = new TcpMonoStream();
            _streamDestinationToSource.WriteData = WriteDataDestinationToSource;
        }

        public void Dispose()
        {
            WriteEndOfDataSourceToDestination();
            WriteEndOfDataDestinationToSource();
        }

        public TcpConnection TcpConnection { get { return _tcpConnection; } }

        //public void Add(TcpStreamPacket tcpStreamPacket)
        //public void Add(TcpConnection tcpConnection, PPacket ppacket)
        public void Add(PPacket ppacket)
        {
            TcpConnection tcpConnection = ppacket.GetTcpConnection();
            if (tcpConnection.Source == _tcpConnection.Source && tcpConnection.Destination == _tcpConnection.Destination)
                _streamSourceToDestination.Add(ppacket);
            else
                _streamDestinationToSource.Add(ppacket);
        }

        private void WriteDataSourceToDestination(byte[] data, TcpStreamWriteDataInfo info)
        {
            WriteEndOfDataDestinationToSource();
            if (WriteData != null)
                WriteData(TcpDirection.SourceToDestination, TcpStreamOperation.Data, data, info);
            _dataSourceToDestination = true;
        }

        private void WriteEndOfDataSourceToDestination()
        {
            if (_dataSourceToDestination)
            {
                if (WriteData != null)
                    WriteData(TcpDirection.SourceToDestination, TcpStreamOperation.EndOfData, null, null);
                _dataSourceToDestination = false;
            }
        }

        private void WriteDataDestinationToSource(byte[] data, TcpStreamWriteDataInfo info)
        {
            WriteEndOfDataSourceToDestination();
            if (WriteData != null)
                WriteData(TcpDirection.DestinationToSource, TcpStreamOperation.Data, data, info);
            _dataDestinationToSource = true;
        }

        private void WriteEndOfDataDestinationToSource()
        {
            if (_dataDestinationToSource)
            {
                if (WriteData != null)
                    WriteData(TcpDirection.DestinationToSource, TcpStreamOperation.EndOfData, null, null);
                _dataDestinationToSource = false;
            }
        }
    }

    public class TcpStreamWriteDataInfo
    {
        public PPacket PPacket;
        public string Message;
        public uint NextSequenceNumber;
    }

    public class TcpMonoStream
    {
        private PPacket _ppacket = null;
        private Packet _packet = null;
        private bool _first = true;
        private TcpFragment _fragment = null; // holds linked list of the session data
        private uint _sequenceNumber; // holds the last sequence number
        private bool _emptyTcpStream = true;
        private uint _bytesWritten = 0;
        private bool _incompleteTcpStream = false;
        public Action<byte[], TcpStreamWriteDataInfo> WriteData;

        public bool IncompleteStream { get { return _incompleteTcpStream; } }
        public bool EmptyStream { get { return _emptyTcpStream; } }

        public void Add(PPacket ppacket)
        {
            _ppacket = ppacket;
            Add(ppacket.Packet);
        }

        public void Add(Packet packet)
        {
            _packet = packet;
            TcpDatagram tcp = packet.Ethernet.IpV4.Tcp;

            // if the paylod length is zero bail out
            uint length = (uint)tcp.PayloadLength;
            if (length == 0)
                return;

            //uint dataLength
            byte[] data = tcp.Payload.ToArray();

            uint dataLength = (uint)data.Length;
            if (dataLength < length)
                _incompleteTcpStream = true;

            uint sequenceNumber = tcp.SequenceNumber;

            // now that we have filed away the srcs, lets get the sequence number stuff figured out
            if (_first)
            {
                // this is the first time we have seen this src's sequence number
                _sequenceNumber = sequenceNumber + length;
                if (tcp.IsSynchronize)
                    _sequenceNumber++;
                if (!(!tcp.IsPush && dataLength == 1 && length == 1 && data[0] == 0))
                    writeData(data, _ppacket, "first packet");
                _first = false;
                return;
            }

            // if we are here, we have already seen this src, let's try and figure out if this packet is in the right place
            if (sequenceNumber < _sequenceNumber)
            {
                // this sequence number seems dated, but check the end to make sure it has no more info than we have already seen
                uint newSequenceNumber = sequenceNumber + length;
                if (newSequenceNumber > _sequenceNumber)
                {
                    // this one has more than we have seen. let's get the payload that we have not seen.

                    uint new_len = _sequenceNumber - sequenceNumber;

                    if (dataLength <= new_len)
                    {
                        data = null;
                        dataLength = 0;
                        _incompleteTcpStream = true;
                    }
                    else
                    {
                        dataLength -= new_len;
                        byte[] tmpData = new byte[dataLength];
                        for (uint i = 0; i < dataLength; i++)
                            tmpData[i] = data[i + new_len];
                        data = tmpData;
                    }
                    sequenceNumber = _sequenceNumber;
                    length = newSequenceNumber - _sequenceNumber;

                    // this will now appear to be right on time :)
                }
            }

            if (sequenceNumber == _sequenceNumber)
            {
                // right on time
                _sequenceNumber += length;
                if (tcp.IsSynchronize)
                    _sequenceNumber++;
                if (data != null)
                    writeData(data, _ppacket, "next packet");

                // done with the packet, see if it caused a fragment to fit
                while (checkFragments())
                    ;
            }
            else
            {
                // out of order packet
                if (dataLength > 0 && sequenceNumber > _sequenceNumber)
                {
                    TcpFragment tmp_frag = new TcpFragment();
                    tmp_frag.data = data;
                    tmp_frag.sequenceNumber = sequenceNumber;
                    tmp_frag.length = length;
                    tmp_frag.dataLength = dataLength;
                    tmp_frag.ppacket = _ppacket;

                    if (_fragment != null)
                        tmp_frag.next = _fragment;
                    else
                        tmp_frag.next = null;
                    _fragment = tmp_frag;
                }
            }
        }

        private void writeData(byte[] data, PPacket ppacket, string message)
        {
            // ignore empty packets
            if (data.Length == 0)
                return;

            if (WriteData != null)
                WriteData(data, new TcpStreamWriteDataInfo() { PPacket = ppacket, Message = message, NextSequenceNumber = _sequenceNumber });

            _bytesWritten += (uint)data.Length;
            _emptyTcpStream = false;
        }

        // here we search through all the frag we have collected to see if one fits
        private bool checkFragments()
        {
            TcpFragment prev = null;
            TcpFragment current = _fragment;
            while (current != null)
            {
                if (current.sequenceNumber == _sequenceNumber)
                {
                    // this fragment fits the stream
                    _sequenceNumber += current.length;
                    if (current.data != null)
                        writeData(current.data, current.ppacket, "next packet from fragments");
                    //_sequenceNumber += current.length;
                    if (prev != null)
                        prev.next = current.next;
                    else
                        _fragment = current.next;
                    current.data = null;
                    current = null;
                    return true;
                }
                prev = current;
                current = current.next;
            }
            return false;
        }
    }
}
