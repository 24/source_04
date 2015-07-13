using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PcapDotNet.Core;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.IpV4;
using pb;
using Pib.Pcap;

namespace Test_Winpcap_dn
{
    static partial class w
    {
        public static void Test_PrintPacketsDetail_02(string deviceName = null)
        {
            Trace.WriteLine("Test_PrintPacketsDetail_02");
            ReceivePackets receivePackets = CreateReceivePackets(deviceName);
            //receivePackets.Receive(PrintPacketDetail.PrintPacketDetailHandler, 5);
            //receivePackets.Receive(PrintPacketDetail.PrintPacketDetailHandler);
            int i = 0;
            receivePackets.Receive(ppacket =>
                {
                    IpV4Datagram ip = ppacket.Packet.Ethernet.IpV4;
                    if (ip.Tcp == null || ip.Tcp.Http.Version == null)
                        return;
                    if (++i > 5)
                        return;
                    PrintPacketDetail.PrintPacketDetailHandler(ppacket);
                });
        }

        public static void Test_PrintPacketsDetail_01(string deviceName = null)
        {
            _tr.WriteLine("Test_PrintPacketsDetail_01");

            PacketDevice device = SelectDevice(deviceName);
            if (device == null)
                return;

            __communicator = null;
            //_rs.OnAbortExecution += new OnAbortEvent(OnAbortExecution);
            _rs.OnAbortExecution = OnAbortExecution;
            try
            {
                using (__communicator = device.Open(65536, PacketDeviceOpenAttributes.Promiscuous, 1000))
                {
                    _tr.WriteLine("Listening on " + device.Description + "...");

                    PPacketManager ppacketManager = new PPacketManager();
                    //PrintPacketDetail print = new PrintPacketDetail();
                    //__communicator.ReceivePackets(5, packet => PrintPacketDetailHandlerOld(ppacketManager.CreatePPacket(packet)));
                    __communicator.ReceivePackets(5, packet => PrintPacketDetail.PrintPacketDetailHandler(ppacketManager.CreatePPacket(packet)));
                }
            }
            finally
            {
                //_rs.OnAbortExecution -= new OnAbortEvent(OnAbortExecution);
            }
        }

        private static void PrintPacketDetailHandlerOld(PPacket ppacket)
        {
            EthernetDatagram ethernet = ppacket.Packet.Ethernet;
            if (ethernet == null)
            {
                _tr.WriteLine("---------------------------------------------[not ethernet packet]---------------------------------------------");
                _tr.WriteLine("---------------------------------------------------------------------------------------------------------------");
                _tr.WriteLine();
                return;
            }
            _tr.WriteLine("----------------------------------[ethernet type II frame (64 to 1518 bytes)]----------------------------------");
            _tr.WriteLine("| dest MAC address   src MAC address    ether type  data (46-1500 bytes)   length  CRC checksum  total length |");
            _tr.WriteLine("| XX:XX:XX:XX:XX:XX  XX:XX:XX:XX:XX:XX  XX XX       XX XX XX XX XX XX ...          XX XX XX XX                |");
            _tr.WriteLine("---------------------------------------------------------------------------------------------------------------");
            StringBuilder sb = new StringBuilder();
            sb.Append("|");
            byte[] buffer = ppacket.Packet.Buffer;
            int i = 0;
            foreach (byte b in buffer)
            {
                if (i == 6 || i == 12)
                    sb.Append(" ");
                else if (i == 14)
                    sb.Append("      ");
                else if (i == 20)
                {
                    sb.Append(" ... ");
                    sb.AppendFormat(" {0,6} ", ethernet.PayloadLength);
                    break;
                }
                sb.Append(" ");
                sb.Append(b.zToHex());
                i++;
            }
            //for (i = buffer.Length - 4; i < buffer.Length; i++)
            //{
            //    byte b = buffer[i];
            //    sb.Append(" ");
            //    sb.Append(b.zToHex());
            //}
            sb.Append("            ");
            //  total length
            sb.AppendFormat("         {0,6} |", ppacket.Packet.Count);
            _tr.WriteLine(sb.ToString());
            _tr.WriteLine("---------------------------------------------------------------------------------------------------------------");
            sb.Clear();
            sb.AppendFormat("| {0}  {1}  {2}       ", ethernet.Destination, ethernet.Source, ((ushort)ethernet.EtherType).zToHex());
            i = 0;
            foreach (byte b in ethernet.Payload)
            {
                if (i++ == 6)
                    break;
                sb.Append(" ");
                sb.Append(b.zToHex());
            }
            sb.Append(" ... ");
            sb.AppendFormat(" {0,6} ", ethernet.PayloadLength);
            if (ethernet.FrameCheckSequence != null)
            {
                foreach (byte b in ethernet.FrameCheckSequence)
                {
                    sb.Append(" ");
                    sb.Append(b.zToHex());
                }
            }
            else
                sb.Append("            ");
            sb.AppendFormat("         {0,6} |", ppacket.Packet.Count);
            _tr.WriteLine(sb.ToString());
            _tr.WriteLine("|                                       {0,-25}                                             |", ppacket.EthernetTypeCode);
            _tr.WriteLine("---------------------------------------------------------------------------------------------------------------");
            IpV4Datagram ip = ethernet.IpV4;
            if (ip == null)
            {
                _tr.WriteLine("-----------------------------------------------[not ipv4 packet]-----------------------------------------------");
                _tr.WriteLine("---------------------------------------------------------------------------------------------------------------");
                _tr.WriteLine();
                return;
            }
            buffer = ip.ToArray();
            _tr.WriteLine("------------------------------------------[ipv4 header]-------------------------------------------");
            _tr.WriteLine("| 0  1  2  3  4  5  6  7  8  9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30 31 |");
            _tr.WriteLine("|  version  |    ihl    |      dscp       | ecn |                  total length                  |");
            _tr.WriteLine("|                identification                 | flags  |            fragment offset            |");
            _tr.WriteLine("|     time to live      |       protocol        |                header checksum                 |");
            _tr.WriteLine("|                                       source ip address                                        |");
            _tr.WriteLine("|                                     destination ip address                                     |");
            _tr.WriteLine("|                                      options (if ihl > 5)                                      |");
            _tr.WriteLine("--------------------------------------------------------------------------------------------------");
            sb.Clear();
            for (i = 0; i < 4; i++)
                sb.AppendFormat("|          {0}           ", buffer[i].zToHex());
            sb.Append(" |");
            _tr.WriteLine(sb.ToString());
            sb.Clear();
            for (i = 4; i < 8; i++)
                sb.AppendFormat("|          {0}           ", buffer[i].zToHex());
            sb.Append(" |");
            _tr.WriteLine(sb.ToString());
            sb.Clear();
            for (i = 8; i < 12; i++)
                sb.AppendFormat("|          {0}           ", buffer[i].zToHex());
            sb.Append(" |");
            _tr.WriteLine(sb.ToString());
            sb.Clear();
            for (i = 12; i < 16; i++)
                sb.AppendFormat("|          {0}           ", buffer[i].zToHex());
            sb.Append(" |");
            _tr.WriteLine(sb.ToString());
            sb.Clear();
            for (i = 16; i < 20; i++)
                sb.AppendFormat("|          {0}           ", buffer[i].zToHex());
            sb.Append(" |");
            _tr.WriteLine(sb.ToString());
            _tr.WriteLine("--------------------------------------------------------------------------------------------------");
            //_tr.WriteLine("|  version  |    ihl    |      dscp       | ecn |                  total length                  |");
            //_tr.WriteLine("|     4     |    15     |      dscp       | ecn |                     12345                      |");
            _tr.WriteLine("|     {0}     |    {1,2}     |      0x{2}       |     |                     {3,5}                      |",
                ip.Version, ip.HeaderLength / 4, ip.TypeOfService.zToHex(), ip.TotalLength);
            //_tr.WriteLine("|                identification                 | flags  |            fragment offset            |");
            //_tr.WriteLine("|                    0x0000                     | flags  |                0x0000                 |");
            _tr.WriteLine("|                    0x{0}                     |        |                0x{1}                 |", ip.Identification.zToHex(), ip.Fragmentation.Offset.zToHex());
            //_tr.WriteLine("|     time to live      |       protocol        |                header checksum                 |");
            //_tr.WriteLine("|         0x00          |         0x00          |                    0x0000                      |");
            _tr.WriteLine("|         0x{0}          |         0x{1}          |                    0x{2}                      |",
                ip.Ttl.zToHex(), ((byte)ip.Protocol).zToHex(), ip.HeaderChecksum.zToHex());
            //_tr.WriteLine("|                                       source ip address                                        |");
            //_tr.WriteLine("|                                        123.123.123.123                                         |");
            _tr.WriteLine("|                                        {0,-15}                                         |", ip.Source);
            _tr.WriteLine("|                                        {0,-15}                                         |", ip.Destination);
            _tr.WriteLine("-------------------------------------------[ipv4 data]--------------------------------------------");
            //             | 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ...  |
            sb.Clear();
            sb.Append("|");
            i = 0;
            foreach (byte b in ip.Payload)
            {
                if (++i == 31)
                    break;
                sb.Append(" ");
                sb.Append(b.zToHex());
            }
            if (i != 31)
            {
                for (; i < 30; i++)
                    sb.Append("   ");
                sb.Append("      |");
            }
            else
                sb.Append(" ...  |");
            _tr.WriteLine(sb.ToString());
            _tr.WriteLine("--------------------------------------------------------------------------------------------------");
            _tr.WriteLine();







            ////*************    0      3    0.465083 192.168.0.1     52581 173.194.66.94     443 TCP         ACK                  0x0001 0x4B2091F9 0x4B2091FA 0x4695DD86 0x401A       .
            ////_tr.WriteLine("group     no    time     source           port destination      port protocal    flags                length sequence   next seq   ack number window urgent");
            //StringBuilder sb = new StringBuilder();
            ////_tr.Write("{0,5}  {1,5}  {2,10:0.000000}", ppacket.GroupNumber, ppacket.PacketNumber, ppacket.RelativeTime.TotalSeconds);
            //sb.AppendFormat("{0,5}  {1,5}  {2,10:0.000000}", ppacket.GroupNumber, ppacket.PacketNumber, ppacket.RelativeTime.TotalSeconds);
            //sb.AppendFormat(" {0,-15} {1,5} {2,-15} {3,5} {4,-10}", ppacket.Source, ppacket.SourcePort, ppacket.Destination, ppacket.DestinationPort, ppacket.ProtocolCode);
            //if (detail)
            //{
            //    TcpDatagram tcp = ppacket.Tcp;
            //    if (tcp != null && ppacket.Ipv4.Protocol == IpV4Protocol.Tcp)
            //    {
            //        sb.AppendFormat("  {0,-20}", ppacket.GetTcpFlagsString());
            //        sb.AppendFormat(" {0,6}", tcp.PayloadLength > 0 ? "0x" + ((short)tcp.PayloadLength).zToHex() : null);

            //        sb.AppendFormat(" 0x{0} {1,10} {2,10} {3,6} {4,6}", ppacket.Tcp.SequenceNumber.zToHex(), tcp.NextSequenceNumber != tcp.SequenceNumber ? "0x" + tcp.NextSequenceNumber.zToHex() : null,
            //            tcp.AcknowledgmentNumber != 0 ? "0x" + tcp.AcknowledgmentNumber.zToHex() : null, "0x" + tcp.Window.zToHex(),
            //            tcp.UrgentPointer != 0 ? "0x" + tcp.UrgentPointer.zToHex() : null);

            //        int i = 0;
            //        int maxDataChar = 50;
            //        foreach (byte b in tcp.Payload)
            //        {
            //            if (++i > maxDataChar)
            //                break;
            //            if (b >= 32 && b <= 126)
            //                sb.Append(((char)b).ToString());
            //            else
            //                sb.Append(".");
            //        }


            //    }
            //    else if (ppacket.Packet.Ethernet == null)
            //        sb.Append(" not ethernet");
            //}
            //_tr.WriteLine(sb.ToString());
        }
    }
}
