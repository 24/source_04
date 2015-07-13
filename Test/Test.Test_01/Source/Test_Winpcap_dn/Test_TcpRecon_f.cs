using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Http;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Transport;
using pb;
using Pib.Pcap;

namespace Test_Winpcap_dn
{
    static partial class w
    {
        public static void Test_AnalyzeTcpStream_01(string deviceName = null, string dataDir = "dump")
        {
            Trace.WriteLine("Test_AnalyzeTcpStream_01");
            AnalyzeTcpStream.Trace = true;
            AnalyzeTcpStream.Analyze(CreateReceivePackets(deviceName), GetPath(dataDir));
        }

        public static void Test_TcpRecon_TcpRecon_01(string dumpFile)
        {
            Trace.WriteLine("Test_TcpRecon_TcpRecon_01");
            dumpFile = GetPath(dumpFile);
            //Tamir_IPLib_SharpPcap.Program.ReconSingleFileSharpPcap(dumpFile);
            TcpRecon.Program.Main(new string[] { dumpFile });
        }

        public static void Test_TcpRecon_LibNids_01(string dumpFile)
        {
            Trace.WriteLine("Test_TcpRecon_LibNids_01");
            dumpFile = GetPath(dumpFile);
            //Tamir_IPLib_SharpPcap.Program.ReconSingleFileLibNids(dumpFile);
            TcpRecon.Program.Main(new string[] { dumpFile, "-nids" });
        }

        public static void Test_TcpRecon_PcapDotNet_01(string dumpFile)
        {
            Trace.WriteLine("Test_TcpRecon_PcapDotNet_01");
            dumpFile = GetPath(dumpFile);
            TcpRecon_PcapDotNet.TcpReconstruct.ReconstructSingleFileSharpPcap(dumpFile);
        }

        public static void Test_SortedTcpStreamList_01(string deviceName)
        {
            SortedTcpStreamList sortedTcpStreamList = new SortedTcpStreamList();
            ReceivePackets receivePackets = CreateReceivePackets(deviceName);
            receivePackets.Receive(sortedTcpStreamList.Add);
            Trace.WriteLine("{0} packet captured", sortedTcpStreamList.TotalPacketsCount);
            Trace.WriteLine("{0} packet selectionned", sortedTcpStreamList.SelectionnedPacketsCount);
            //var q = from p in tcpAnalyze.Packets where p.PPacket.Tcp != null orderby p.StreamNumber, p.PPacket.PacketNumber select p;
            //var q = from p in sortedTcpStreamList.TcpStreamPacketsList from p2 in p.Value orderby p.Key.Index, p2.PacketNumber select new { TcpConnection = p.Key, PPacket = p2 };
            var q = from ppacketList in sortedTcpStreamList.TcpStreamPacketsList.Values from ppacket in ppacketList orderby ppacket.GetTcpConnection().Index, ppacket.PacketNumber select ppacket;
            foreach (PPacket ppacket in q)
                //PrintSortedTcpStreamPacket(p.TcpConnection, p.PPacket);
                PrintSortedTcpStreamPacket(ppacket);
        }

        //private static int _LastTcpConnectionIndex = 0;
        private static TcpConnection _currentTcpConnection = null;
        //private static void PrintSortedTcpStreamPacket(IndexedTcpConnection tcpConnection, PPacket ppacket)
        private static void PrintSortedTcpStreamPacket(PPacket ppacket)
        {
            TcpConnection tcpConnection = ppacket.GetTcpConnection();
            if (_currentTcpConnection == null || tcpConnection.Index != _currentTcpConnection.Index)
            {
                Trace.WriteLine();
                Trace.WriteLine("group packet    time      source                 dir   destination           protocol flags                data   seq        next seq   ack        window urg       data");
                //_LastStreamNumber = tcpConnection.Index;
                _currentTcpConnection = tcpConnection;
            }
            //TcpDirection direction = TcpDirection.SourceToDestination;
            //string s = GetTcpStreamPacketString1(tcpConnection, ppacket, _currentTcpConnection.GetTcpDirection(tcpConnection), true);
            string s = GetTcpStreamPacketString1(ppacket, _currentTcpConnection.GetTcpDirection(tcpConnection), true);
            //string s2 = null;
            //foreach (Pib.Pcap.Test.TcpPacketError error in streamPacket.Errors)
            //    s2 = s2.zAddValue(streamPacket.GetErrorMessage(error));

            //if (s2 != null)
            //    s2 = " " + s2;
            //Trace.WriteLine("{0,-162}{1}{2}", s, ppacket.GetTcpDescription(), s2);
            Trace.WriteLine(s);
        }

        //public static string GetTcpStreamPacketString1(IndexedTcpConnection tcpConnection, PPacket ppacket, TcpDirection direction, bool printData)
        public static string GetTcpStreamPacketString1(PPacket ppacket, TcpDirection direction, bool printData)
        {
            IpV4Datagram ip = ppacket.Ipv4;
            TcpDatagram tcp = ppacket.Tcp;
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
            //if (direction != null)
            //{
                if (direction == TcpDirection.SourceToDestination)
                    dir = "-->   ";
                else
                {
                    dir = "<--   ";
                    string addr = saddr;
                    saddr = daddr;
                    daddr = addr;
                }
            //}
            //sb.Append(string.Format("{0,5}  {1,5}  {2,10:0.000000}  {3,-21}  {4}{5,-21} {6,-7}", packet.gGroupNumber, packet.PacketNumber, packet.RelativeTime.TotalSeconds, saddr, dir, daddr, packet.ProtocolCode));
            sb.Append(string.Format("{0,5}  {1,5}  {2,10:0.000000}  {3,-21}  {4}{5,-21} {6,-7}",
                ppacket.GetTcpConnection().Index, ppacket.PacketNumber, ppacket.RelativeTime.TotalSeconds, saddr, dir, daddr, ppacket.IpProtocolCode));
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
                sb.Append(string.Format("  {0,-20} {1} 0x{2} {3} {4} 0x{5}{6}", ppacket.GetTcpFlagsString(), dataLength, tcp.SequenceNumber.zToHex(), next_seq, ack_seq, tcp.Window.zToHex(), urg_ptr));
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

        public static void Test_AnalyzeTcpStreamTest_01(string deviceName)
        {
            Trace.WriteLine("Test_AnalyzeTcpStreamTest_01");
            ReceivePackets receivePackets = CreateReceivePackets(deviceName);
            Pib.Pcap.Test.AnalyzeTcpStreamTest tcpAnalyze = new Pib.Pcap.Test.AnalyzeTcpStreamTest();
            receivePackets.Receive(packet => tcpAnalyze.Add(packet));
            Trace.WriteLine("{0} packet captured", tcpAnalyze.Packets.Count);
            var q = from p in tcpAnalyze.Packets where p.PPacket.Tcp != null orderby p.StreamNumber, p.PPacket.PacketNumber select p;
            Trace.WriteLine("{0} packet selectionned", q.Count());
            foreach (Pib.Pcap.Test.TcpStreamPacket p in q)
                PrintStreamPacket(p);
        }

        public static void Test_AnalyzeTcpStreamTest_ReadPcap_01(string file)
        {
            Trace.WriteLine("Test_AnalyzeTcpStreamTest_ReadPcap_01");
            Trace.WriteLine("pcap file \"{0}\"", file);
            Pib.Pcap.Test.AnalyzeTcpStreamTest tcpAnalyze = new Pib.Pcap.Test.AnalyzeTcpStreamTest();
            using (PcapFileReader pcapFileReader = new PcapFileReader(GetPath(file)))
            {
                PPacketManager ppacketManager = new PPacketManager();
                foreach (PcapPacket packet in pcapFileReader)
                {
                    Packet packet2 = new Packet(packet.Data, packet.Timestamp, DataLinkKind.Ethernet);
                    tcpAnalyze.Add(ppacketManager.CreatePPacket(packet2));
                }
            }

            Trace.WriteLine("{0} packet captured", tcpAnalyze.Packets.Count);
            //var q = from p in gPacketList where p.TCPHeader != null orderby p.gGroupNumber, p.PacketNumber select p;
            var q = from p in tcpAnalyze.Packets where p.PPacket.Tcp != null orderby p.StreamNumber, p.PPacket.PacketNumber select p;
            Trace.WriteLine("{0} packet selectionned", q.Count());
            foreach (Pib.Pcap.Test.TcpStreamPacket p in q)
                PrintStreamPacket(p);
        }
    }
}
