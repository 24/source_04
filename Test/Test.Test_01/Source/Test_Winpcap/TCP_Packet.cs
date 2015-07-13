using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using wpcap_dn;
using pb;

namespace Test_Winpcap
{
    public class TCP_Analyze
    {
        public List<TCP_StreamPacket> Packets = new List<TCP_StreamPacket>();
        public SortedList<TCP_StreamAddress, TCP_Stream> Streams = new SortedList<TCP_StreamAddress, TCP_Stream>();

        public void Add(EthernetPacket packet)
        {
            if (packet.TCP == null) return;
            TCP_StreamAddress address = new TCP_StreamAddress(packet);
            TCP_StreamPacket streamPacket = new TCP_StreamPacket();
            streamPacket.Direction = TCP_Direction.SourceToDestination;
            streamPacket.Address = address;
            streamPacket.Packet = packet;
            int i = Streams.IndexOfKey(address);
            TCP_Stream stream;
            if (i == -1)
            {
                address.StreamNumber = Streams.Count;
                stream = new TCP_Stream(streamPacket);
                Streams.Add(address, stream);
            }
            else
            {
                stream = Streams.Values[i];
                address.StreamNumber = stream.Address.StreamNumber;
            }
            stream.Add(streamPacket);
            Packets.Add(streamPacket);
            packet.gGroupNumber = address.StreamNumber + 1;
        }

        public void XmlExportPackets(string path)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = ("    ");
            using (XmlWriter writer = XmlWriter.Create(path, settings))
            {
                writer.WriteStartElement("TCP");
                var q = from p in Packets where p.Packet.TCP != null orderby p.Address.StreamNumber, p.Packet.PacketNumber select p;
                int lastStreamNumber = -1;
                foreach (TCP_StreamPacket packet in q)
                {
                    if (packet.Packet.gGroupNumber != lastStreamNumber)
                    {
                        if (lastStreamNumber != -1)
                            writer.WriteEndElement();

                        writer.WriteStartElement("Stream");
                        writer.WriteAttributeString("Stream", packet.Packet.gGroupNumber.ToString());
                        lastStreamNumber = packet.Packet.gGroupNumber;
                    }
                    packet.XmlExport(writer);
                }
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }
    }

    public enum TCP_Direction
    {
        SourceToDestination,
        DestinationToSource
    }

    [Flags]
    public enum TCP_PacketType
    {
        Unknow                      = 0x0000,
        OpenConnection1             = 0x0001,     // --> SYN seq = x
        OpenConnection2             = 0x0002,     // <-- SYN seq = y, ACK x + 1
        OpenConnection3             = 0x0003,     // --> ACK y + 1
        CloseSourceConnection1      = 0x0004,     // --> FIN seq = x
        CloseSourceConnection2      = 0x0005,     // <-- ACK x + 1
        CloseDestinationConnection1 = 0x0006,     // <-- FIN seq = y, ACK x + 1
        CloseDestinationConnection2 = 0x0007,     // <-- ACK y + 1
        Data                        = 0x0010,
        Push                        = 0x0020,
        Ack                         = 0x0040
    }

    public enum TCP_PacketError
    {
        ErrorBadDirection                          = 1001,
        WarningBadFlagAck                          = 1002,
        WarningOpenConnection2BadAck               = 2001,
        WarningOpenConnection2BadMessagePosition   = 2002,
        ErrorOpenConnection2BadDirection           = 2003,
        WarningOpenConnection3BadAck               = 2011,
        WarningCloseSourceConnection1BadSeq        = 2051,
        WarningCloseSourceConnection2BadAck        = 2061,
        WarningCloseDestinationConnection1BadSeq   = 2071,
        WarningCloseDestinationConnection1BadAck   = 2072,
        WarningCloseDestinationConnection2BadAck   = 2081
    }

    public class DataSegment
    {
        public long Position;
        public uint Length;
    }

    public class TCP_Data
    {
        public TCP_Direction Direction;
        public List<byte> Data = new List<byte>();
        public uint InitialSequence;
        public uint CurrentSequence;
        public long SequencePosition;
        public long FirstMissingBytePosition;
        public long LastBytePosition;
        public SortedList<long, DataSegment> DataSegments = new SortedList<long, DataSegment>();
        public List<TCP_StreamPacket> DataPackets = new List<TCP_StreamPacket>();

        public TCP_Data(EthernetPacket Packet)
        {
        }

        public void Add(EthernetPacket Packet)
        {
        }
    }

    public class TCP_StreamPacket
    {
        public TCP_Direction Direction;
        public TCP_StreamAddress Address;
        public TCP_PacketType PacketType;
        public EthernetPacket Packet;
        public TCP_StreamState StreamState;
        //public List<string> Messages = new List<string>();
        public List<TCP_PacketError> Errors = new List<TCP_PacketError>();

        public string PacketDescription
        {
            get
            {
                string s = null;
                TCP tcp = Packet.TCP;
                switch (PacketType)
                {
                    case TCP_PacketType.OpenConnection1:
                        s = string.Format("TCP open 1 : SYN seq = x (0x{0})", tcp.Sequence.zToHex());
                        break;
                    case TCP_PacketType.OpenConnection2:
                        s = string.Format("TCP open 2 : SYN seq = y (0x{0}), ACK x + 1 (0x{1})", tcp.Sequence.zToHex(), tcp.AckSequence.zToHex());
                        break;
                    case TCP_PacketType.OpenConnection3:
                        s = string.Format("TCP open 3 : ACK y + 1 (0x{0})", tcp.AckSequence.zToHex());
                        break;
                    case TCP_PacketType.CloseSourceConnection1:
                        s = string.Format("TCP close source 1 : FIN seq = x (0x{0})", tcp.Sequence.zToHex());
                        break;
                    case TCP_PacketType.CloseSourceConnection2:
                        s = string.Format("TCP close source 2 : ACK x + 1 (0x{0})", tcp.AckSequence.zToHex());
                        break;
                    case TCP_PacketType.CloseDestinationConnection1:
                        s = string.Format("TCP close destination 1 : FIN seq = y (0x{0}), ACK x + 1 (0x{1})", tcp.Sequence.zToHex(), tcp.AckSequence.zToHex());
                        break;
                    case TCP_PacketType.CloseDestinationConnection2:
                        s = string.Format("TCP close destination 2 : ACK y + 1 (0x{0})", tcp.AckSequence.zToHex());
                        break;
                }
                //TCP_PacketType.Ack;
                //TCP_PacketType.Push;
                //TCP_PacketType.Data;
                return s;
            }
        }

        public void XmlExport(XmlWriter writer)
        {
            writer.WriteStartElement("Packet");
            writer.WriteAttributeString("Number", Packet.PacketNumber.ToString());
            writer.WriteAttributeString("Time", Packet.RelativeTime.TotalSeconds.ToString("000000"));
            string source = Packet.IP.SourceAddress.ToString() + ":" + Packet.TCP.SourcePort.ToString();
            string destination = Packet.IP.DestinationAddress.ToString() + ":" + Packet.TCP.DestinationPort.ToString();
            if (Direction == TCP_Direction.DestinationToSource)
            {
                string s = source;
                source = destination;
                destination = s;
            }
            writer.WriteAttributeString("Source", source);
            writer.WriteAttributeString("Dir", TCP_Direction.SourceToDestination.ToString());
            writer.WriteAttributeString("Destination", destination);
            writer.WriteAttributeString("Protocol", Packet.ProtocolCode);
            writer.WriteAttributeString("Flags", Packet.TCP.GetFlagsString());
            writer.WriteAttributeString("Data", "0x" + Packet.TCP.DataLength.zToHex());
            writer.WriteAttributeString("Sequence", "0x" + Packet.TCP.Sequence.zToHex());
            writer.WriteAttributeString("AckSequence", "0x" + Packet.TCP.AckSequence.zToHex());
            writer.WriteAttributeString("Window", "0x" + Packet.TCP.Window.zToHex());
            writer.WriteAttributeString("UrgentPointer", "0x" + Packet.TCP.UrgentPointer.zToHex());
            writer.WriteAttributeString("Message", PacketDescription);
            foreach (TCP_PacketError error in Errors)
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
            writer.WriteAttributeString("Number", string.Format("{0,4}", Packet.PacketNumber));
            writer.WriteAttributeString("Time", Packet.RelativeTime.TotalSeconds.ToString("000000"));
            string source = Packet.IP.SourceAddress.ToString() + ":" + Packet.TCP.SourcePort.ToString();
            string destination = Packet.IP.DestinationAddress.ToString() + ":" + Packet.TCP.DestinationPort.ToString();
            string dir;
            if (Direction == TCP_Direction.SourceToDestination)
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
            writer.WriteAttributeString("Protocol", Packet.ProtocolCode);
            writer.WriteAttributeString("Flags", string.Format("{0,-20}", Packet.TCP.GetFlagsString()));
            if (Packet.TCP.DataLength > 0) s = "0x" + Packet.TCP.DataLength.zToHex(); else s = "      ";
            writer.WriteAttributeString("Data", s);
            if (Packet.TCP.Sequence != 0) s = "0x" + Packet.TCP.Sequence.zToHex(); else s = "          ";
            writer.WriteAttributeString("Sequence", s);
            if (Packet.TCP.AckSequence != 0) s = "0x" + Packet.TCP.AckSequence.zToHex(); else s = "          ";
            writer.WriteAttributeString("AckSequence", s);
            if (Packet.TCP.Window != 0) s = "0x" + Packet.TCP.Window.zToHex(); else s = "      ";
            writer.WriteAttributeString("Window", s);
            if (Packet.TCP.UrgentPointer != 0) s = "0x" + Packet.TCP.UrgentPointer.zToHex(); else s = "      ";
            writer.WriteAttributeString("UrgentPointer", s);
            writer.WriteAttributeString("Message", PacketDescription);
            foreach (TCP_PacketError error in Errors)
            {
                writer.WriteStartElement("Error");
                writer.WriteAttributeString("Error", GetErrorMessage(error));
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        public void AddError(TCP_PacketError error)
        {
            Errors.Add(error);
        }

        public string GetErrorMessage(TCP_PacketError error)
        {
            TCP tcp = Packet.TCP;
            switch (error)
            {
                case TCP_PacketError.ErrorBadDirection:
                    return "error wrong direction";
                case TCP_PacketError.WarningBadFlagAck:
                    return "warning ack should not be set";
                case TCP_PacketError.WarningOpenConnection2BadAck:
                    return string.Format("warning ack (0x{0}) != source seq + 1 (0x{1})", tcp.AckSequence.zToHex(), (StreamState.CurrentSourceSequence - 1).zToHex());
                case TCP_PacketError.WarningOpenConnection2BadMessagePosition:
                    return "warning message should be just after open connection step 1";
                case TCP_PacketError.ErrorOpenConnection2BadDirection:
                    return string.Format("error wrong direction, TCP open 2, SYN seq = y (0x{0}), ACK x + 1 (0x{1})", tcp.Sequence.zToHex(), tcp.AckSequence.zToHex());
                case TCP_PacketError.WarningOpenConnection3BadAck:
                    return string.Format("warning ack (0x{0}) != destination seq + 1 (0x{1})", tcp.AckSequence.zToHex(), StreamState.CurrentDestinationSequence.zToHex());
                case TCP_PacketError.WarningCloseSourceConnection1BadSeq:
                    return string.Format("warning seq (0x{0}) != source seq (0x{1})", tcp.Sequence.zToHex(), StreamState.CurrentSourceSequence.zToHex());
                case TCP_PacketError.WarningCloseSourceConnection2BadAck:
                    return string.Format("warning ack (0x{0}) != source seq + 1 (0x{1})", tcp.AckSequence.zToHex(), StreamState.CurrentSourceSequence.zToHex());
                case TCP_PacketError.WarningCloseDestinationConnection1BadSeq:
                    return string.Format("warning seq (0x{0}) != destination seq (0x{1})", tcp.Sequence.zToHex(), StreamState.CurrentDestinationSequence.zToHex());
                case TCP_PacketError.WarningCloseDestinationConnection1BadAck:
                    return string.Format("warning ack (0x{0}) != source seq + 1 (0x{1})", tcp.AckSequence.zToHex(), StreamState.CurrentSourceSequence.zToHex());
                case TCP_PacketError.WarningCloseDestinationConnection2BadAck:
                    return string.Format("warning ack (0x{0}) != destination seq + 1 (0x{1}) (ACK y + 1)", tcp.AckSequence.zToHex(), StreamState.CurrentDestinationSequence.zToHex());
                default:
                    return null;
            }
        }
    }

    public class TCP_StreamState
    {
        public TCP_PacketType OpenState = TCP_PacketType.Unknow;
        public TCP_PacketType CloseSourceState = TCP_PacketType.Unknow;
        public TCP_PacketType CloseDestinationState = TCP_PacketType.Unknow;
        public bool ConnectionOpened = false;
        public uint InitialSourceSequence = 0;
        public uint InitialDestinationSequence = 0;
        public uint CurrentSourceSequence = 0;
        public uint CurrentDestinationSequence = 0;
        public TCP_PacketType LastPacketType = TCP_PacketType.Unknow;
    }

    public class TCP_Stream
    {
        public TCP_StreamAddress Address = null;
        //public TCP_PacketType OpenState = TCP_PacketType.Unknow;
        //public TCP_PacketType CloseSourceState = TCP_PacketType.Unknow;
        //public TCP_PacketType CloseDestinationState = TCP_PacketType.Unknow;
        //public bool ConnectionOpened = false;
        //public uint InitialSourceSequence = 0;
        //public uint InitialDestinationSequence = 0;
        //public uint CurrentSourceSequence = 0;
        //public uint CurrentDestinationSequence = 0;
        //public TCP_PacketType LastPacketType = TCP_PacketType.Unknow;
        public TCP_StreamState StreamState = new TCP_StreamState();
        public TCP_Data CurrentSourceData = null;
        public TCP_Data CurrentDestinationData = null;
        public List<TCP_Data> DataList = new List<TCP_Data>();
        public List<TCP_StreamPacket> Packets = new List<TCP_StreamPacket>();

        public TCP_Stream(TCP_StreamPacket streamPacket)
        {
            Address = streamPacket.Address;
            //_Add(streamPacket);
        }

        public void Add(TCP_StreamPacket streamPacket)
        {
            _Add(streamPacket);
        }

        private void _Add(TCP_StreamPacket streamPacket)
        {
            if (streamPacket.Address.OriginalOrder == Address.OriginalOrder)
                streamPacket.Direction = TCP_Direction.SourceToDestination;
            else
                streamPacket.Direction = TCP_Direction.DestinationToSource;
            Packets.Add(streamPacket);
            AnalyzePacket(streamPacket);
        }

        private void AnalyzePacket(TCP_StreamPacket streamPacket)
        {
            TCP tcp = streamPacket.Packet.TCP;
            streamPacket.PacketType = TCP_PacketType.Unknow;
            bool OpenConnectionPacketType = false;
            bool CloseConnectionPacketType = false;
            if (tcp.Synchronize)
            {
                OpenConnectionPacketType = true;
                // ****** OpenConnection1 SYN seq = x ******
                if (!tcp.Ack)
                {
                    streamPacket.PacketType = TCP_PacketType.OpenConnection1;
                    StreamState.OpenState = TCP_PacketType.OpenConnection1;
                    StreamState.InitialSourceSequence = StreamState.CurrentSourceSequence = tcp.Sequence;
                    //streamPacket.Message("TCP open 1 : SYN seq = x (0x{0})", tcp.seq.zToHex());
                    if (streamPacket.Direction != TCP_Direction.SourceToDestination)
                        //streamPacket.Message("error wrong direction");
                        streamPacket.AddError(TCP_PacketError.ErrorBadDirection);
                }
                // ****** OpenConnection2 SYN seq = y, ACK x + 1 ******
                else
                {
                    if (streamPacket.Direction == TCP_Direction.DestinationToSource)
                    {
                        streamPacket.PacketType = TCP_PacketType.OpenConnection2;
                        StreamState.OpenState = TCP_PacketType.OpenConnection2;
                        StreamState.InitialDestinationSequence = StreamState.CurrentDestinationSequence = tcp.Sequence;
                        //streamPacket.Message("TCP open 2 : SYN seq = y (0x{0}), ACK x + 1 (0x{1})", tcp.seq.zToHex(), tcp.ack_seq.zToHex());
                        StreamState.CurrentSourceSequence++;
                        if (tcp.AckSequence != StreamState.CurrentSourceSequence)
                            //streamPacket.Message("warning ack (0x{0}) != source seq + 1 (0x{1})", tcp.ack_seq.zToHex(), CurrentSourceSequence.zToHex());
                            streamPacket.AddError(TCP_PacketError.WarningOpenConnection2BadAck);
                        if (StreamState.LastPacketType != TCP_PacketType.OpenConnection1)
                            //streamPacket.Message("warning message should be just after open connection step 1");
                            streamPacket.AddError(TCP_PacketError.WarningOpenConnection2BadMessagePosition);
                    }
                    else
                        //streamPacket.Message("error wrong direction, TCP open 2, SYN seq = y (0x{0}), ACK x + 1 (0x{1})", tcp.seq.zToHex(), tcp.ack_seq.zToHex());
                        streamPacket.AddError(TCP_PacketError.ErrorOpenConnection2BadDirection);
                }
            }
            else if (tcp.Finish)
            {
                CloseConnectionPacketType = true;
                // ****** CloseSourceConnection1 FIN seq = x ******
                if (streamPacket.Direction == TCP_Direction.SourceToDestination)
                {
                    streamPacket.PacketType = TCP_PacketType.CloseSourceConnection1;
                    StreamState.CloseSourceState = TCP_PacketType.CloseSourceConnection1;
                    //streamPacket.Message("TCP close source connection step 1 : FIN seq = x (0x{0})", tcp.seq.zToHex());
                    if (tcp.Sequence != StreamState.CurrentSourceSequence)
                        //streamPacket.Message("warning seq (0x{0}) != source seq (0x{1})", tcp.seq.zToHex(), CurrentSourceSequence.zToHex());
                        streamPacket.AddError(TCP_PacketError.WarningCloseSourceConnection1BadSeq);
                    if (tcp.Ack)
                        //streamPacket.Message("warning ack should not be set");
                        streamPacket.AddError(TCP_PacketError.WarningBadFlagAck);
                }
                // ****** CloseDestinationConnection1 FIN seq = y, ACK x + 1 ******
                else // streamPacket.Direction == TCP_Direction.DestinationToSource
                {
                    streamPacket.PacketType = TCP_PacketType.CloseDestinationConnection1;
                    StreamState.CloseDestinationState = TCP_PacketType.CloseDestinationConnection1;
                    //streamPacket.Message("TCP close destination connection step 1 : FIN seq = y (0x{0}), ACK x + 1 (0x{1})", tcp.seq.zToHex(), tcp.ack_seq.zToHex());
                    if (tcp.Sequence != StreamState.CurrentDestinationSequence)
                        //streamPacket.Message("warning seq (0x{0}) != destination seq (0x{1})", tcp.seq.zToHex(), CurrentDestinationSequence.zToHex());
                        streamPacket.AddError(TCP_PacketError.WarningCloseDestinationConnection1BadSeq);
                    if (!tcp.Ack)
                        //streamPacket.Message("warning ack should be set");
                        streamPacket.AddError(TCP_PacketError.WarningBadFlagAck);
                    else if (tcp.AckSequence != StreamState.CurrentSourceSequence + 1)
                        //streamPacket.Message("warning ack (0x{0}) != source seq + 1 (0x{1})", tcp.ack_seq.zToHex(), CurrentSourceSequence.zToHex());
                        streamPacket.AddError(TCP_PacketError.WarningCloseDestinationConnection1BadAck);
                }
            }
            else if (tcp.Ack)
            {
                // ****** OpenConnection3 ACK y + 1 ******
                if (StreamState.LastPacketType == TCP_PacketType.OpenConnection2 && streamPacket.Direction == TCP_Direction.SourceToDestination)
                {
                    OpenConnectionPacketType = true;
                    streamPacket.PacketType = TCP_PacketType.OpenConnection3;
                    StreamState.OpenState = TCP_PacketType.OpenConnection3;
                    StreamState.ConnectionOpened = true;
                    StreamState.CurrentDestinationSequence++;
                    //streamPacket.Message("TCP open 3 : ACK y + 1 (0x{0})", tcp.ack_seq.zToHex());
                    if (tcp.AckSequence != StreamState.InitialDestinationSequence + 1)
                        //streamPacket.Message("warning ack (0x{0}) != destination seq + 1 (0x{1})", tcp.ack_seq.zToHex(), CurrentDestinationSequence.zToHex());
                        streamPacket.AddError(TCP_PacketError.WarningOpenConnection3BadAck);
                }
                // ****** CloseSourceConnection2 ACK x + 1 ******
                else if (StreamState.CloseSourceState == TCP_PacketType.CloseSourceConnection1 && streamPacket.Direction == TCP_Direction.DestinationToSource)
                {
                    CloseConnectionPacketType = true;
                    streamPacket.PacketType = TCP_PacketType.CloseSourceConnection2;
                    StreamState.CloseSourceState = TCP_PacketType.CloseSourceConnection2;
                    //streamPacket.Message("TCP close source connection step 2 : ACK x + 1 (0x{0})", tcp.ack_seq.zToHex());
                    if (tcp.AckSequence != StreamState.CurrentSourceSequence + 1)
                        //streamPacket.Message("warning ack (0x{0}) != source seq + 1 (0x{1})", tcp.ack_seq.zToHex(), CurrentSourceSequence.zToHex());
                        streamPacket.AddError(TCP_PacketError.WarningCloseSourceConnection2BadAck);
                }
                // ****** CloseDestinationConnection2 ACK y + 1 ******
                else if (StreamState.CloseDestinationState == TCP_PacketType.CloseDestinationConnection1 && streamPacket.Direction == TCP_Direction.SourceToDestination)
                {
                    CloseConnectionPacketType = true;
                    streamPacket.PacketType = TCP_PacketType.CloseDestinationConnection2;
                    StreamState.CloseDestinationState = TCP_PacketType.CloseDestinationConnection2;
                    //streamPacket.Message("TCP close destination connection step 2 : ACK y + 1 (0x{0})", tcp.ack_seq.zToHex());
                    if (tcp.AckSequence != StreamState.CurrentDestinationSequence + 1)
                        //streamPacket.Message("warning ack (0x{0}) != destination seq + 1 (0x{1}) (ACK y + 1)", tcp.ack_seq.zToHex(), CurrentDestinationSequence.zToHex());
                        streamPacket.AddError(TCP_PacketError.WarningCloseDestinationConnection2BadAck);
                }
                else
                    streamPacket.PacketType |= TCP_PacketType.Ack;
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

            if (tcp.Push)
                streamPacket.PacketType |= TCP_PacketType.Push;
            if (tcp.DataLength > 0)
                streamPacket.PacketType |= TCP_PacketType.Data;
            StreamState.LastPacketType = streamPacket.PacketType;
            streamPacket.StreamState = StreamState;
        }
    }

    public class TCP_Address : IComparable<TCP_Address>
    {
        public IPAddress Address = null;
        public ushort Port = 0;
        public byte[] AddressBytes = null;

        public TCP_Address(IPAddress address, ushort port)
        {
            Address = address;
            Port = port;
            AddressBytes = address.GetAddressBytes();
        }

        public static bool operator <(TCP_Address a1, TCP_Address a2)
        {
            if (a1.CompareTo(a2) < 0)
                return true;
            else
                return false;
        }

        public static bool operator >(TCP_Address a1, TCP_Address a2)
        {
            if (a1.CompareTo(a2) > 0)
                return true;
            else
                return false;
        }

        public int CompareTo(TCP_Address other)
        {
            if (AddressBytes.Length < other.AddressBytes.Length)
                return -1;
            if (AddressBytes.Length > other.AddressBytes.Length)
                return 1;
            for (int i = 0; i < AddressBytes.Length; i++)
            {
                if (AddressBytes[i] < other.AddressBytes[i])
                    return -1;
                if (AddressBytes[i] > other.AddressBytes[i])
                    return 1;
            }
            if (Port < other.Port)
                return -1;
            if (Port > other.Port)
                return 1;
            return 0;
        }
    }

    public class TCP_StreamAddress : IComparable<TCP_StreamAddress>
    {
        public TCP_Address SourceAddress = null;
        public TCP_Address DestinationAddress = null;

        public TCP_Address Address1 = null; // la plus petite entre SourceAddress et DestinationAddress
        public TCP_Address Address2 = null; // la plus grande entre SourceAddress et DestinationAddress

        public bool OriginalOrder = true;  // si true  : Address1 = SourceAddress et Address2 = DestinationAddress
                                           // si false : Address1 = DestinationAddress  et Address2 = SourceAddress

        public int StreamNumber = -1;

        public TCP_StreamAddress(EthernetPacket packet)
        {
            SourceAddress = new TCP_Address(packet.IP.SourceAddress, packet.TCP.SourcePort);
            DestinationAddress = new TCP_Address(packet.IP.DestinationAddress, packet.TCP.DestinationPort);
            if (SourceAddress < DestinationAddress)
            {
                Address1 = SourceAddress;
                Address2 = DestinationAddress;
                OriginalOrder = true;
            }
            else
            {
                Address1 = DestinationAddress;
                Address2 = SourceAddress;
                OriginalOrder = false;
            }
        }

        public static int AddressBytesCompare(byte[] address1Bytes, ushort port1, byte[] address2Bytes, ushort port2)
        {
            if (address1Bytes.Length < address2Bytes.Length)
                return -1;
            if (address1Bytes.Length > address2Bytes.Length)
                return 1;
            for (int i = 0; i < address1Bytes.Length; i++)
            {
                if (address1Bytes[i] < address2Bytes[i])
                    return -1;
                if (address1Bytes[i] > address2Bytes[i])
                    return 1;
            }
            if (port1 < port2)
                return -1;
            if (port1 > port2)
                return 1;
            return 0;
        }

        public int CompareTo(TCP_StreamAddress other)
        {
            if (Address1 < other.Address1)
                return -1;
            if (Address1 > other.Address1)
                return 1;
            if (Address2 < other.Address2)
                return -1;
            if (Address2 > other.Address2)
                return 1;
            return 0;
        }
    }
}
