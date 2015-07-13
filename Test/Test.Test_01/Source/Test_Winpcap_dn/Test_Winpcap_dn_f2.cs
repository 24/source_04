using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PcapDotNet.Core;
using PcapDotNet.Core.Extensions;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.Http;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Transport;
using pb;
using pb.Text;
using Pib.Pcap;
//using Pib.Pcap.Test;

namespace Test_Winpcap_dn
{
    static partial class w
    {
        public static void Test_GetHashCode_01()
        {
            //packet no 3 192.168.0.1:52581 - 173.194.66.94:443
            //packet no 4 173.194.66.94:443 - 192.168.0.1:52581
            TcpConnection tcpConnection1 = new TcpConnection(new TcpAddress(new MacAddress("00:07:CB:C1:35:5D"), new IpV4Address("192.168.0.1"), 52581),
                new TcpAddress(new MacAddress("48:5B:39:C0:45:48"), new IpV4Address("173.194.66.94"), 443));
            TcpConnection tcpConnection2 = new TcpConnection(new TcpAddress(new MacAddress("48:5B:39:C0:45:48"), new IpV4Address("173.194.66.94"), 443),
                new TcpAddress(new MacAddress("00:07:CB:C1:35:5D"), new IpV4Address("192.168.0.1"), 52581));
        }

        public static void Test_PrintPackets_01(string deviceName = null, bool detail = false)
        {
            _tr.WriteLine("Test_PrintPackets_01");

            //string dumpFile = @"dump\dump.pcap";
            //string dumpFile = @"dump\dump.pcapng";
            //dumpFile = GetPath(dumpFile);
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
                    //using (BerkeleyPacketFilter filter = __communicator.CreateFilter("ip and udp"))
                    //{
                    //    __communicator.SetFilter(filter);
                    //}

                    _tr.WriteLine("Listening on " + device.Description + "...");

                    //__communicator.ReceivePackets(0, PrintPacketHandler1);
                    //__communicator.zReceivePackets(0, PrintPacketHandler2);

                    PPacketManager ppacketManager = new PPacketManager();
                    PrintPacketHandler2Header();
                    //__communicator.ReceivePackets(0, packet => PrintPacketHandler2(ppacketManager.CreatePPacket(packet), detail));
                    __communicator.ReceivePackets(0, packet => PrintPacket.PrintPacket1(ppacketManager.CreatePPacket(packet), detail));
                }
            }
            //catch (Exception ex)
            //{
            //    _tr.WriteLine(ex.Message);
            //}
            finally
            {
                //_rs.OnAbortExecution -= new OnAbortEvent(OnAbortExecution);
            }
        }

        public static void Test_DumpPacketsToFile_04(string dumpFile, string deviceName = null, bool detail = false, string filter = null)
        {
            Trace.WriteLine("Test_DumpPacketsToFile_04");
            Trace.WriteLine("dump to file \"{0}\"", dumpFile);
            //string filter = "not tcp portrange 52000-53000 and not net 0.0.0.0 mask 255.0.0.0";
            ReceivePackets receivePackets = CreateReceivePackets(deviceName, dumpFile, filter);
            receivePackets.Receive(PrintIpAdressList);
        }

        //public static void Test_DumpPacketsToFile_03(string dumpFile, string deviceName = null, bool detail = false)
        //{
        //    // from project SavingPacketsToADumpFile
        //    _tr.WriteLine("Test_DumpPacketsToFile_03");

        //    string filter = "";
        //    //string filter = "not tcp portrange 52000-53000 and not net 0.0.0.0 mask 255.0.0.0";

        //    //string dumpFile = @"dump\dump.pcap";
        //    dumpFile = GetPath(dumpFile);
        //    _tr.WriteLine("dump to file \"{0}\"", dumpFile);

        //    PacketDevice device = SelectDevice(deviceName);
        //    if (device == null)
        //        return;
        //    __communicator = null;
        //    //_rs.OnAbortExecution += new OnAbortEvent(OnAbortExecution);
        //    _rs.OnAbortExecution = OnAbortExecution;
        //    try
        //    {
        //        using (__communicator = device.Open(65536, PacketDeviceOpenAttributes.Promiscuous, 1000))
        //        {
        //            using (PacketDumpFile dump = __communicator.OpenDump(dumpFile))
        //            {
        //                _tr.WriteLine("Listening on " + device.Description + "...");
        //                __communicator.SetFilter(filter);

        //                //PPacketManager ppacketManager = new PPacketManager();
        //                __communicator.ReceivePackets(0, packet =>
        //                    {
        //                        dump.Dump(packet);
        //                        //PrintPacketHandler2(ppacketManager.CreatePPacket(packet), detail);
        //                        PrintIpAdressList(packet);
        //                    });
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _tr.WriteLine(ex.Message);
        //    }
        //    finally
        //    {
        //        //_rs.OnAbortExecution -= new OnAbortEvent(OnAbortExecution);
        //    }
        //}

        private static void PrintPacketHandler1(Packet packet)
        {
            _tr.Write("{0:yyyy-MM-dd HH:mm:ss.fff}", packet.Timestamp);

            EthernetDatagram ethernet = packet.Ethernet;
            if (ethernet != null)
            {
                _tr.WriteLine(" eth : src {0} dst {1} type {2,-10} pl {3,5}", ethernet.Source, ethernet.Destination, ethernet.EtherType, ethernet.PayloadLength);
                IpV4Datagram ipv4 = ethernet.IpV4;
                if (ipv4 != null)
                    _tr.WriteLine(" ipv4 : src {0} dst {1} type {2,-10} pl {3,5}", ipv4.Source, ethernet.Destination, ethernet.EtherType, ethernet.PayloadLength);
                //IpV4Datagram ip = packet.Ethernet.IpV4;
                //UdpDatagram udp = ip.Udp;

                // print ip addresses and udp ports
                //_tr.Write(ip.Source + ":" + udp.SourcePort + " -> " + ip.Destination + ":" + udp.DestinationPort);
            }
            else
                _tr.WriteLine(" not ethernet");
        }

        private static void PrintPacketHandler2Header()
        {
            //*************    0      3    0.465083 192.168.0.1     52581 173.194.66.94     443 TCP         ACK                  0x0001 0x4B2091F9 0x4B2091FA 0x4695DD86 0x401A       .
            _tr.WriteLine("group     no    time     source           port destination      port protocal    flags                length sequence   next seq   ack number window urgent");
        }

        private static int _LastStreamNumber = 0;
        private static void PrintStreamPacket(Pib.Pcap.Test.TcpStreamPacket streamPacket)
        {
            if (streamPacket.StreamNumber != _LastStreamNumber)
            {
                Trace.WriteLine();
                Trace.WriteLine("group packet    time      source                 dir   destination           protocol flags                data   seq        next seq   ack        window urg       data");
                _LastStreamNumber = streamPacket.StreamNumber;
            }
            string s = GetStreamPacketString1(streamPacket, streamPacket.Direction, true);
            string s2 = null;
            foreach (Pib.Pcap.Test.TcpPacketError error in streamPacket.Errors)
                s2 = s2.zAddValue(streamPacket.GetErrorMessage(error));

            if (s2 != null)
                s2 = " " + s2;
            Trace.WriteLine("{0,-162}{1}{2}", s, streamPacket.PacketDescription, s2);
        }

        public static string GetStreamPacketString1(Pib.Pcap.Test.TcpStreamPacket streamPacket, TcpDirection? direction, bool printData)
        {
            IpV4Datagram ip = streamPacket.PPacket.Ipv4;
            TcpDatagram tcp = streamPacket.PPacket.Tcp;
            string saddr = null;
            string daddr = null;
            string mf = null;
            string offset = null;
            string id = null;
            if (ip != null)
            {
                saddr = ip.Source.ToString();
                daddr = ip.Destination.ToString();
                if (tcp != null)
                {
                    saddr += ":" + tcp.SourcePort.ToString();
                    daddr += ":" + tcp.DestinationPort.ToString();
                }
                //mf = ip.MoreFragment.ToString();
                mf = (ip.Fragmentation.Options == IpV4FragmentationOptions.MoreFragments).ToString();
                offset = ip.Fragmentation.Offset.zToHex();
                id = ip.Identification.zToHex();
            }
            StringBuilder sb = new StringBuilder();
            //direction
            string dir = null;
            if (direction != null)
            {
                if ((TcpDirection)direction == TcpDirection.SourceToDestination)
                    dir = "-->   ";
                else
                {
                    dir = "<--   ";
                    string addr = saddr;
                    saddr = daddr;
                    daddr = addr;
                }
            }
            //sb.Append(string.Format("{0,5}  {1,5}  {2,10:0.000000}  {3,-21}  {4}{5,-21} {6,-7}", packet.gGroupNumber, packet.PacketNumber, packet.RelativeTime.TotalSeconds, saddr, dir, daddr, packet.ProtocolCode));
            sb.Append(string.Format("{0,5}  {1,5}  {2,10:0.000000}  {3,-21}  {4}{5,-21} {6,-7}",
                streamPacket.StreamNumber, streamPacket.PPacket.PacketNumber, streamPacket.PPacket.RelativeTime.TotalSeconds, saddr, dir, daddr, streamPacket.PPacket.IpProtocolCode));
            string align = "";
            if (tcp != null)
            {
                string next_seq;
                if (tcp.PayloadLength > 0)
                    next_seq = "0x" + (tcp.SequenceNumber + (uint)tcp.PayloadLength).zToHex();
                else if (tcp.IsSynchronize)
                    next_seq = "0x" + (tcp.SequenceNumber + 1).zToHex();
                else
                    next_seq = "          ";
                string dataLength;
                if (tcp.PayloadLength > 0)
                    dataLength = "0x" + ((ushort)tcp.PayloadLength).zToHex();
                else
                    dataLength = "      ";
                string ack_seq;
                if (tcp.AcknowledgmentNumber != 0)
                    ack_seq = "0x" + tcp.AcknowledgmentNumber.zToHex();
                else
                    ack_seq = "          ";
                string urg_ptr = null;
                if (tcp.UrgentPointer != 0)
                    urg_ptr = " 0x" + tcp.UrgentPointer.zToHex();
                else
                    align += "       ";
                //TCPFlags(tcp)
                //tcp.GetFlagsString()
                sb.Append(string.Format("  {0,-20} {1} 0x{2} {3} {4} 0x{5}{6}", streamPacket.PPacket.GetTcpFlagsString(), dataLength, tcp.SequenceNumber.zToHex(), next_seq, ack_seq, tcp.Window.zToHex(), urg_ptr));
            }
            else
                align += "                                                                                    ";
            sb.Append(align + "   ");
            HttpDatagram http = tcp.Http;
            if (http != null)
            {
                sb.Append("http ");
                if (http.IsRequest)
                    sb.Append("request");
                else if (http.IsResponse)
                    sb.Append("reply  ");
                else
                    sb.Append("????   ");
                //sb.Append(http.Version.ToString());
            }
            else
                sb.Append("                      ");
            if (printData && tcp != null && tcp.PayloadLength > 0)
            {
                //sb.Append(align + "   ");
                //byte[] data = tcp.Payload;
                int i = 0;
                //int maxDataChar = 100;
                int maxDataChar = 50;
                foreach (byte b in tcp.Payload)
                {
                    if (++i > maxDataChar)
                        break;
                    sb.Append(" ");
                    sb.Append(b.zToHex());
                }
                sb.Append("  ");
                i = 0;
                foreach (byte b in tcp.Payload)
                {
                    if (++i > maxDataChar)
                        break;
                    if (b >= 32 && b <= 126)
                        sb.Append((char)b);
                    else
                        sb.Append('.');
                }
            }
            return sb.ToString();
        }

        private static Dictionary<uint, string> _ipAdressList = new Dictionary<uint, string>();
        private static Dictionary<TcpConnection, string> _tcpStreamList = new Dictionary<TcpConnection, string>();
        //private static void PrintIpAdressList(Packet packet)
        private static void PrintIpAdressList(PPacket ppacket)
        {
            Packet packet = ppacket.Packet;
            //if (packet.Ethernet == null)
            //    return;
            //IpV4Datagram ip = packet.Ethernet.IpV4;
            IpV4Datagram ip = ppacket.Ipv4;
            if (ip == null)
                return;
            uint ipAdress = ip.Source.ToValue();
            if (!_ipAdressList.ContainsKey(ipAdress))
            {
                _ipAdressList.Add(ipAdress, null);
                Trace.WriteLine("new ip adress {0}", ip.Source.ToString());
            }
            ipAdress = ip.Destination.ToValue();
            if (!_ipAdressList.ContainsKey(ipAdress))
            {
                _ipAdressList.Add(ipAdress, null);
                Trace.WriteLine("new ip adress {0}", ip.Destination.ToString());
            }
            //TcpDatagram tcp = ip.Tcp;
            TcpDatagram tcp = ppacket.Tcp;
            if (tcp == null)
                return;
            TcpConnection tcpConnection = new TcpConnection(packet);
            if (!_tcpStreamList.ContainsKey(tcpConnection))
            {
                _tcpStreamList.Add(tcpConnection, null);
                //_tr.WriteLine("new tcp stream {0}:{1} {2}:{3}", tcpConnection.Source.IpAddress, tcpConnection.Source.Port,
                //    tcpConnection.Destination.IpAddress, tcpConnection.Destination.Port);
                Trace.WriteLine("new tcp stream {0}", tcpConnection.GetConnectionName());
            }
        }
    }
}
