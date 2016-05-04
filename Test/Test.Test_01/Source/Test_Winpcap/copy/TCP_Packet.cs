#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using wpcap_dn;
#endregion

namespace Test_Winpcap
{
    public class TCP_Analyze
    {
        public List<TCP_StreamPacket> Packets = new List<TCP_StreamPacket>();
        public SortedList<TCP_StreamAddress, TCP_Stream> Streams = new SortedList<TCP_StreamAddress, TCP_Stream>();

        public void Add(EthernetPacket packet)
        {
            if (packet.TCPHeader == null) return;
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
    }

    #region enum TCP_Direction
    public enum TCP_Direction
    {
        SourceToDestination,
        DestinationToSource
    }
    #endregion

    #region enum TCP_PacketType
    [Flags]
    public enum TCP_PacketType
    {
        Unknow                      = 0x0000,
        OpenConnection1             = 0x0001,      // --> SYN seq = x
        OpenConnection2             = 0x0002,      // <-- SYN seq = y, ACK x + 1
        OpenConnection3             = 0x0003,      // --> ACK y + 1
        CloseSourceConnection1      = 0x0004,     // --> FIN seq = x
        CloseSourceConnection2      = 0x0005,     // <-- ACK x + 1
        CloseDestinationConnection1 = 0x0005,     // <-- FIN seq = y, ACK x + 1
        CloseDestinationConnection2 = 0x0006,     // <-- ACK y + 1
        Data                        = 0x0010,
        Push                        = 0x0020,
        Ack                         = 0x0040
    }
    #endregion

    #region //class TCP
    //public static class TCP
    //{
    //}
    #endregion

    public class DataSegment
    {
        public long Position;
        public uint Length;
    }

    #region class TCP_Data
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
    #endregion

    public class TCP_StreamPacket
    {
        public TCP_Direction Direction;
        public TCP_StreamAddress Address;
        public TCP_PacketType PacketType;
        public EthernetPacket Packet;
    }

    #region class TCP_Stream
    public class TCP_Stream
    {
        #region variable
        public TCP_StreamAddress Address = null;
        public TCP_PacketType OpenState = TCP_PacketType.Unknow;
        public TCP_PacketType CloseSourceState = TCP_PacketType.Unknow;
        public TCP_PacketType CloseDestinationState = TCP_PacketType.Unknow;
        public bool ConnectionOpened = false;
        public uint InitialSourceSequence = 0;
        public uint InitialDestinationSequence = 0;
        public uint CurrentSourceSequence = 0;
        public uint CurrentDestinationSequence = 0;
        public TCP_PacketType LastPacketType = TCP_PacketType.Unknow;
        public TCP_Data CurrentSourceData = null;
        public TCP_Data CurrentDestinationData = null;
        public List<TCP_Data> DataList = new List<TCP_Data>();
        public List<TCP_StreamPacket> Packets = new List<TCP_StreamPacket>();
        #endregion

        #region constructor
        public TCP_Stream(TCP_StreamPacket streamPacket)
        {
            Address = streamPacket.Address;
            _Add(streamPacket);
        }
        #endregion

        #region Add
        public void Add(TCP_StreamPacket streamPacket)
        {
            _Add(streamPacket);
        }
        #endregion

        #region _Add
        private void _Add(TCP_StreamPacket streamPacket)
        {
            if (streamPacket.Address.OriginalOrder == Address.OriginalOrder)
                streamPacket.Direction = TCP_Direction.SourceToDestination;
            else
                streamPacket.Direction = TCP_Direction.DestinationToSource;
            Packets.Add(streamPacket);
            TCPHeader tcp = streamPacket.Packet.TCPHeader;
            if (tcp.syn == 1)
            {
                // ****** OpenConnection1 SYN seq = x ******
                if (tcp.ack == 0)
                {
                    streamPacket.PacketType = TCP_PacketType.OpenConnection1;
                    OpenState = TCP_PacketType.OpenConnection1;
                    InitialSourceSequence = CurrentSourceSequence = tcp.seq;
                    Message("TCP open connection step 1 : SYN seq = x (0x{0})", h(tcp.seq));
                    if (streamPacket.Direction != TCP_Direction.SourceToDestination)
                        Message("  warning wrong direction");
                }
                // ****** OpenConnection2 SYN seq = y, ACK x + 1 ******
                else
                {
                    if (streamPacket.Direction != TCP_Direction.DestinationToSource)
                    {
                        Message("TCP open connection step 2 : error wrong direction (SYN seq = y, ACK x + 1)");
                        streamPacket.PacketType = TCP_PacketType.Unknow;
                    }
                    else
                    {
                        streamPacket.PacketType = TCP_PacketType.OpenConnection2;
                        OpenState = TCP_PacketType.OpenConnection2;
                        InitialDestinationSequence = CurrentDestinationSequence = tcp.seq;
                        Message("TCP open connection step 2 : SYN seq = y (0x{0}), ACK x + 1 (0x{1})", h(tcp.seq), h(tcp.ack_seq));
                        if (tcp.ack_seq != CurrentSourceSequence + 1)
                            Message("  warning ack (0x{0}) != source seq + 1 (0x{1})", h(tcp.ack_seq), h(CurrentSourceSequence));
                        if (LastPacketType != TCP_PacketType.OpenConnection1)
                            Message("  warning message should be just after open connection step 1");
                        CurrentSourceSequence++;
                    }
                }
            }
            else if (tcp.fin == 1)
            {
                // ****** CloseSourceConnection1 FIN seq = x ******
                if (streamPacket.Direction == TCP_Direction.SourceToDestination)
                {
                    CloseSourceState = TCP_PacketType.CloseSourceConnection1;
                    Message("TCP close source connection step 1 : FIN seq = x (0x{0})", h(tcp.seq));
                    if (tcp.seq != CurrentSourceSequence)
                        Message("TCP close source connection step 1 : warning seq (0x{0}) != source seq (0x{1}) (FIN seq = x)", h(tcp.seq), h(CurrentSourceSequence));
                    if (tcp.ack != 0)
                        Message("TCP close source connection step 1 : warning ack should not be set (FIN seq = x)");
                }
                // ****** CloseDestinationConnection1 FIN seq = y, ACK x + 1 ******
                else // streamPacket.Direction == TCP_Direction.DestinationToSource
                {
                    CloseDestinationState = TCP_PacketType.CloseDestinationConnection1;
                    Message("TCP close destination connection step 1 : FIN seq = y (0x{0}), ACK x + 1 (0x{1})", h(tcp.seq), h(tcp.ack_seq));
                    if (tcp.seq != CurrentDestinationSequence)
                        Message("TCP close destination connection step 1 : warning seq (0x{0}) != destination seq (0x{1}) (FIN seq = y, ACK x + 1)", h(tcp.seq), h(CurrentDestinationSequence));
                    if (tcp.ack != 1)
                        Message("TCP close destination connection step 1 : warning ack should be set (FIN seq = y, ACK x + 1)");
                    else if (tcp.ack_seq != CurrentSourceSequence + 1)
                        Message("TCP close destination connection step 1 : warning ack (0x{0}) != source seq + 1 (0x{1}) (FIN seq = y, ACK x + 1)", h(tcp.ack_seq), h(CurrentSourceSequence));
                }
            }
            else if (tcp.ack == 1)
            {
                // ****** OpenConnection3 ACK y + 1 ******
                if (LastPacketType == TCP_PacketType.OpenConnection2 && streamPacket.Direction == TCP_Direction.SourceToDestination)
                {
                    streamPacket.PacketType = TCP_PacketType.OpenConnection3;
                    OpenState = TCP_PacketType.OpenConnection3;
                    ConnectionOpened = true;
                    CurrentDestinationSequence++;
                    Message("TCP open connection step 3 : ACK y + 1 (0x{0})", h(tcp.ack_seq));

                    if (tcp.ack_seq != InitialDestinationSequence + 1)
                        Message("TCP open connection step 3 : warning ack (0x{0}) != destination seq + 1 (0x{1}) (ACK y + 1)", h(tcp.ack_seq), h(CurrentDestinationSequence));
                }
                // ****** CloseSourceConnection2 ACK x + 1 ******
                else if (CloseSourceState = TCP_PacketType.CloseSourceConnection1 && streamPacket.Direction == TCP_Direction.DestinationToSource)
                {
                    Message("TCP close source connection step 2 : ACK x + 1 (0x{0})", h(tcp.ack_seq));
                    if (tcp.ack_seq != CurrentSourceSequence + 1)
                        Message("TCP close source connection step 2 : warning ack (0x{0}) != source seq + 1 (0x{1}) (ACK x + 1)", h(tcp.ack_seq), h(CurrentSourceSequence));
                }
                // ****** CloseDestinationConnection2 ACK y + 1 ******
                else if (CloseDestinationState = TCP_PacketType.CloseDestinationConnection1 && streamPacket.Direction == TCP_Direction.SourceToDestination)
                {
                    Message("TCP close destination connection step 2 : ACK y + 1 (0x{0})", h(tcp.ack_seq));
                    if (tcp.ack_seq != CurrentDestinationSequence + 1)
                        Message("TCP close destination connection step 2 : warning ack (0x{0}) != destination seq + 1 (0x{1}) (ACK y + 1)", h(tcp.ack_seq), h(CurrentDestinationSequence));
                }
            }
            LastPacketType = streamPacket.PacketType;
        }
        #endregion

        #region Message
        private void Message(string message, params object[] prm)
        {
            if (prm.Length > 0)
                message = string.Format(message, prm);
        }
        #endregion
    }
    #endregion

    #region TCP_Address
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
    #endregion

    #region class TCP_StreamAddress
    public class TCP_StreamAddress : IComparable<TCP_StreamAddress>
    {
        //public IPAddress SourceAddress = null;
        //public ushort SourcePort = 0;
        //public IPAddress DestinationAddress = null;
        //public ushort DestinationPort = 0;

        //public byte[] gSourceAddressBytes = null;
        //public byte[] gDestinationAddressBytes = null;

        //public IPAddress Address1 = null;
        //public ushort Port1 = 0;
        //public IPAddress Address2 = null;
        //public ushort Port2 = 0;

        //public byte[] gAddress1Bytes = null;
        //public byte[] gAddress2Bytes = null;

        public TCP_Address SourceAddress = null;
        public TCP_Address DestinationAddress = null;

        public TCP_Address Address1 = null; // la plus petite entre SourceAddress et DestinationAddress
        public TCP_Address Address2 = null; // la plus grande entre SourceAddress et DestinationAddress

        public bool OriginalOrder = true;  // si true  : Address1 = SourceAddress et Address2 = DestinationAddress
                                           // si false : Address1 = DestinationAddress  et Address2 = SourceAddress

        public int StreamNumber = -1;

        //public TCP_StreamAddress()
        //{
        //}

        public TCP_StreamAddress(EthernetPacket packet)
        {
            SourceAddress = new TCP_Address(packet.IPHeader.Source, packet.TCPHeader.source);
            DestinationAddress = new TCP_Address(packet.IPHeader.Destination, packet.TCPHeader.dest);
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


            //SourceAddress = packet.IPHeader.Source;
            //SourcePort = packet.TCPHeader.source;
            //DestinationAddress = packet.IPHeader.Destination;
            //DestinationPort = packet.TCPHeader.dest;

            //gSourceAddressBytes = SourceAddress.GetAddressBytes();
            //gDestinationAddressBytes = DestinationAddress.GetAddressBytes();

            //IPAddress address1 = packet.IPHeader.Source;
            //IPAddress address2 = packet.IPHeader.Destination;
            //byte[] address1Bytes = address1.GetAddressBytes();
            //byte[] address2Bytes = address2.GetAddressBytes();
            //ushort port1 = packet.TCPHeader.source;
            //ushort port2 = packet.TCPHeader.dest;

            //if (AddressBytesCompare(address1Bytes, port1, address2Bytes, port2) <= 0)
            //{
            //    Address1 = address1;
            //    Address2 = address2;
            //    gAddress1Bytes = address1Bytes;
            //    gAddress2Bytes = address2Bytes;
            //    Port1 = port1;
            //    Port2 = port2;
            //}
            //else
            //{
            //    Address1 = address2;
            //    Address2 = address1;
            //    gAddress1Bytes = address2Bytes;
            //    gAddress2Bytes = address1Bytes;
            //    Port1 = port2;
            //    Port2 = port1;
            //}
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
            //if (gSourceAddressBytes.Length < other.gSourceAddressBytes.Length)
            //    return -1;
            //if (gSourceAddressBytes.Length > other.gSourceAddressBytes.Length)
            //    return 1;
            //for (int i = 0; i < gSourceAddressBytes.Length; i++)
            //{
            //    if (gSourceAddressBytes[i] < other.gSourceAddressBytes[i])
            //        return -1;
            //    if (gSourceAddressBytes[i] > other.gSourceAddressBytes[i])
            //        return 1;
            //}
            //if (gDestinationAddressBytes.Length < other.gDestinationAddressBytes.Length)
            //    return -1;
            //if (gDestinationAddressBytes.Length > other.gDestinationAddressBytes.Length)
            //    return 1;
            //for (int i = 0; i < gDestinationAddressBytes.Length; i++)
            //{
            //    if (gDestinationAddressBytes[i] < other.gDestinationAddressBytes[i])
            //        return -1;
            //    if (gDestinationAddressBytes[i] > other.gDestinationAddressBytes[i])
            //        return 1;
            //}
            //if (SourcePort < other.SourcePort)
            //    return -1;
            //if (SourcePort > other.SourcePort)
            //    return 1;
            //if (DestinationPort < other.DestinationPort)
            //    return -1;
            //if (DestinationPort > other.DestinationPort)
            //    return 1;
            //return 0;

            //if (gAddress1Bytes.Length < other.gAddress1Bytes.Length)
            //    return -1;
            //if (gAddress1Bytes.Length > other.gAddress1Bytes.Length)
            //    return 1;
            //for (int i = 0; i < gAddress1Bytes.Length; i++)
            //{
            //    if (gAddress1Bytes[i] < other.gAddress1Bytes[i])
            //        return -1;
            //    if (gAddress1Bytes[i] > other.gAddress1Bytes[i])
            //        return 1;
            //}
            //if (gAddress2Bytes.Length < other.gAddress2Bytes.Length)
            //    return -1;
            //if (gAddress2Bytes.Length > other.gAddress2Bytes.Length)
            //    return 1;
            //for (int i = 0; i < gAddress2Bytes.Length; i++)
            //{
            //    if (gAddress2Bytes[i] < other.gAddress2Bytes[i])
            //        return -1;
            //    if (gAddress2Bytes[i] > other.gAddress2Bytes[i])
            //        return 1;
            //}
            //if (Port1 < other.Port1)
            //    return -1;
            //if (Port1 > other.Port1)
            //    return 1;
            //if (Port2 < other.Port2)
            //    return -1;
            //if (Port2 > other.Port2)
            //    return 1;
            //return 0;
            //int r = AddressBytesCompare(gAddress1Bytes, Port1, other.gAddress1Bytes, other.Port1);
            //if (r != 0) return r;
            //return AddressBytesCompare(gAddress2Bytes, Port2, other.gAddress2Bytes, other.Port2);
        }
    }
    #endregion
}
