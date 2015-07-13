using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Transport;
using pb;

namespace Pib.Pcap
{
    public static class PrintPacket
    {
        public static void PrintPacket1(PPacket ppacket, bool detail = false, ITrace trace = null)
        {
            if (trace == null)
                trace = Trace.CurrentTrace;
            //*************    0      3    0.465083 192.168.0.1     52581 173.194.66.94     443 TCP         ACK                  0x0001 0x4B2091F9 0x4B2091FA 0x4695DD86 0x401A       .
            //_tr.WriteLine("group     no    time     source           port destination      port protocal    flags                length sequence   next seq   ack number window urgent");
            StringBuilder sb = new StringBuilder();
            //_tr.Write("{0,5}  {1,5}  {2,10:0.000000}", ppacket.GroupNumber, ppacket.PacketNumber, ppacket.RelativeTime.TotalSeconds);
            sb.AppendFormat("{0,5}  {1,5}  {2,10:0.000000}", ppacket.GroupNumber, ppacket.PacketNumber, ppacket.RelativeTime.TotalSeconds);
            sb.AppendFormat(" {0,-15} {1,5} {2,-15} {3,5} {4,-10}", ppacket.Source, ppacket.SourcePort, ppacket.Destination, ppacket.DestinationPort, ppacket.IpProtocolCode);
            if (detail)
            {
                TcpDatagram tcp = ppacket.Tcp;
                if (tcp != null && ppacket.Ipv4.Protocol == IpV4Protocol.Tcp)
                {
                    sb.AppendFormat("  {0,-20}", ppacket.GetTcpFlagsString());
                    sb.AppendFormat(" {0,6}", tcp.PayloadLength > 0 ? "0x" + ((short)tcp.PayloadLength).zToHex() : null);

                    sb.AppendFormat(" 0x{0} {1,10} {2,10} {3,6} {4,6}", ppacket.Tcp.SequenceNumber.zToHex(), tcp.NextSequenceNumber != tcp.SequenceNumber ? "0x" + tcp.NextSequenceNumber.zToHex() : null,
                        tcp.AcknowledgmentNumber != 0 ? "0x" + tcp.AcknowledgmentNumber.zToHex() : null, "0x" + tcp.Window.zToHex(),
                        tcp.UrgentPointer != 0 ? "0x" + tcp.UrgentPointer.zToHex() : null);

                    int i = 0;
                    int maxDataChar = 50;
                    foreach (byte b in tcp.Payload)
                    {
                        if (++i > maxDataChar)
                            break;
                        if (b >= 32 && b <= 126)
                            sb.Append(((char)b).ToString());
                        else
                            sb.Append(".");
                    }


                }
                else if (ppacket.Packet.Ethernet == null)
                    sb.Append(" not ethernet");
            }
            trace.WriteLine(sb.ToString());
        }
    }
}
