using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Http;
using PcapDotNet.Packets.Transport;
using pb;

namespace Pib.Pcap
{
    public class EnumerableWithCounter<T> : IEnumerable<T>, IEnumerator<T>
    {
        private IEnumerator<T> _enumerator = null;
        private int _index = -1;
        private bool _endOfData = false;

        public EnumerableWithCounter(IEnumerable<T> enumerable, int index = -1)
        {
            _enumerator = enumerable.GetEnumerator();
            _index = index;
        }

        public void Dispose()
        {
        }

        public int Index { get { return _index; } }
        public bool EndOfData { get { return _endOfData; } }

        public IEnumerator<T> GetEnumerator()
        {
            return this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this;
        }

        public T Current
        {
            get { return _enumerator.Current; }
        }

        object System.Collections.IEnumerator.Current
        {
            get { return _enumerator.Current; }
        }

        public bool MoveNext()
        {
            if (_enumerator.MoveNext())
            {
                _index++;
                return true;
            }
            else
            {
                _endOfData = true;
                return false;
            }
        }

        public void Reset()
        {
            _enumerator.Reset();
        }
    }

    public static partial class GlobalExtension
    {
        public static EnumerableWithCounter<T> zAsEnumerableWithCounter<T>(this IEnumerable<T> enumerable, int index = -1)
        {
            return new EnumerableWithCounter<T>(enumerable, index);
        }
    }

    public class PrintPacketDetail
    {
        private ushort _dataLength;
        private StringBuilder _sb = new StringBuilder();
        EnumerableWithCounter<byte> _dataEnum = null;
        private const int _maxDataPerLine1 = 8;
        private const int _maxDataPerLine2 = 16;

        public void _PrintPacketDetailHandler(PPacket ppacket)
        {
            //try
            //{
                //------------------------------------------------------------------------------------------------------------------------------
                //| packet no 5                      | relative time 0.745022                                            | packet length 55    |
                //-------------------------------------------------------[ethernet frame]-------------------------------------------------------
                //| 0x0000 | 00 07 CB C1 35 5D       | destination MAC address         00:07:CB:C1:35:5D                 | header length 14    |
                //| 0x0006 | 48 5B 39 C0 45 48       | source MAC address              48:5B:39:C0:45:48                 | data length   41    |
                //| 0x000C | 08 00                   | ether type                      0x0800  IPv4                      |                     |
                //---------------------------------------------------------[ipv4 frame]---------------------------------------------------------
                //| 0x000E | 45                      | version                         4                                 | header length 20    |
                //|        |                         | internet header length (ihl)    5                                 | data length   21    |
                //| 0x000F | 00                      | dscp                            0x00                              |                     |
                //|        |                         | ecn                                                               |                     |
                //| 0x0010 | 00 29                   | total length                    41                                |                     |
                //| 0x0012 | 4F F9                   | identification                  0x4FF9                            |                     |
                //| 0x0014 | 40 00                   | flags                                                             |                     |
                //|        |                         | fragment offset                 0x0000                            |                     |
                //| 0x0016 | 80                      | time to live                    0x80                              |                     |
                //| 0x0017 | 06                      | protocol                        0x06 TCP                          |                     |
                //| 0x0018 | 1A 47                   | header checksum                 0x1A47                            |                     |
                //| 0x001A | C0 A8 00 01             | source ip address               192.168.0.1                       |                     |
                //| 0x001E | AD C2 22 23             | destination ip address          173.194.34.35                     |                     |
                //|        |                         | options (if ihl > 5)                                              |                     |
                //| 0x0022 | CD 67 01 BB E9 DF 53 5D | data                                                              |                     |
                //| 0x002A | 91 1F 42 06 50 10 40 3D |                                                                   |                     |
                //| 0x0032 | FF 81 00 00 00          |                                                                   |                     |
                //------------------------------------------------------------------------------------------------------------------------------

                //if (ip.Tcp != null && ip.Tcp.Http.Version == null)
                //    return;

                PrintPacketInfos(ppacket);
                _dataLength = (ushort)ppacket.Packet.Count;
                _dataEnum = ppacket.Packet.zAsEnumerableWithCounter();
                PrintEthernet(ppacket);

                EthernetDatagram ethernet = ppacket.Packet.Ethernet;
                if (ethernet.EtherType == EthernetType.IpV4)
                {
                    PrintIpV4(ppacket);
                    IpV4Datagram ip = ethernet.IpV4;
                    if (ip.Protocol == IpV4Protocol.Tcp)
                    {
                        PrintTcp(ppacket);
                        if (ip.Tcp.Http.Version != null)
                            PrintHttp(ppacket);
                    }
                }
                WriteSeparation();
                Trace.WriteLine();


            //}
            //finally
            //{
            //    WriteSeparation();
            //    Trace.WriteLine();
            //}
        }

        private void PrintPacketInfos(PPacket ppacket)
        {
            WriteSeparation();
            WriteHeaderValues("packet no " + ppacket.PacketNumber.ToString(), string.Format("relative time {0:0.000000}", ppacket.RelativeTime.TotalSeconds), "packet length", ppacket.Packet.Count);
        }

        private void PrintEthernet(PPacket ppacket)
        {
            EthernetDatagram ethernet = ppacket.Packet.Ethernet;
            if (ethernet == null)
                return;
            _dataLength -= 14;
            WriteTitle("[ethernet frame]");
            WriteValues(6, "destination MAC address", ethernet.Destination, "header length", 14);
            //WriteValues(6, "source MAC address", ethernet.Source, "data length", _dataLength);
            WriteValues(6, "source MAC address", ethernet.Source);
            WriteValues(2, "ether type", "0x" + ((ushort)ethernet.EtherType).zToHex() + "  " + ppacket.EthernetTypeCode);
            WriteTitle("[ethernet data]");
            WritePayloadData(48, ethernet.Payload.zAsEnumerableWithCounter(_dataEnum.Index));
        }

        private void PrintIpV4(PPacket ppacket)
        {
            //EthernetDatagram ethernet = ppacket.Packet.Ethernet;
            IpV4Datagram ip = ppacket.Packet.Ethernet.IpV4;
            //if (ethernet.EtherType != EthernetType.IpV4)
            //    return;
            _dataLength -= (ushort)ip.HeaderLength;

            WriteTitle("[ipv4 frame]");
            WriteValues(1, "version", ip.Version, "header length", ip.HeaderLength);
            //WriteValues(0, "internet header length (ihl)", ip.HeaderLength / 4, "data length", _dataLength);
            WriteValues(0, "internet header length (ihl)", ip.HeaderLength / 4);
            WriteValues(1, "dscp", "0x" + ip.TypeOfService.zToHex());
            WriteValues(0, "ecn");
            WriteValues(2, "total length", ip.TotalLength);
            WriteValues(2, "identification", "0x" + ip.Identification.zToHex());
            WriteValues(2, "flags");
            WriteValues(0, "fragment offset", "0x" + ip.Fragmentation.Offset.zToHex());
            WriteValues(1, "time to live", "0x" + ip.Ttl.zToHex());
            WriteValues(1, "protocol", "0x" + ((byte)ip.Protocol).zToHex() + " " + ppacket.IpProtocolCode);
            WriteValues(2, "header checksum", "0x" + ip.HeaderChecksum.zToHex());
            WriteValues(4, "source ip address", ip.Source);
            WriteValues(4, "destination ip address", ip.Destination);
            WriteValues(ip.HeaderLength - 20, "options (if ihl > 5)");
            WriteTitle("[ipv4 data]");
            WritePayloadData(48, ip.Payload.zAsEnumerableWithCounter(_dataEnum.Index));
        }

        private void PrintHttp(PPacket ppacket)
        {
            HttpDatagram http = ppacket.Packet.Ethernet.IpV4.Tcp.Http;
            //ReadOnlyCollection<HttpDatagram> httpCollection = ppacket.Packet.Ethernet.IpV4.Tcp.HttpCollection;
            WriteTitle("[http]");
            WriteValues(0, "version", http.Version != null ? http.Version.ToString() : "null");
            WriteValues(0, "is request", http.IsRequest);
            WriteValues(0, "is response", http.IsResponse);
            WriteValues(0, "header", http.Header != null ? http.Header.ToString() : "null");
        }

        private void PrintTcp(PPacket ppacket)
        {
            //IpV4Datagram ip = ppacket.Packet.Ethernet.IpV4;
            TcpDatagram tcp = ppacket.Packet.Ethernet.IpV4.Tcp;
            //if (ip.Protocol != IpV4Protocol.Tcp)
            //    return;
            _dataLength -= (ushort)tcp.RealHeaderLength;

            WriteTitle("[tcp frame]");
            WriteValues(2, "source port", tcp.SourcePort, "header length", tcp.RealHeaderLength);
            //WriteValues(2, "destination port", tcp.DestinationPort, "data length", _dataLength);
            WriteValues(2, "destination port", tcp.DestinationPort);
            WriteValues(4, "sequence number", "0x" + tcp.SequenceNumber.zToHex());
            WriteValues(4, "acknowledgment number", "0x" + tcp.AcknowledgmentNumber.zToHex());
            WriteValues(2, "data offset", "0x" + ((byte)tcp.HeaderLength).zToHex());
            WriteValues(0, "ecn", tcp.IsExplicitCongestionNotificationEcho);
            WriteValues(0, "cwr", tcp.IsCongestionWindowReduced);
            WriteValues(0, "ece", tcp.IsExplicitCongestionNotificationEcho);
            WriteValues(0, "urg", tcp.IsUrgent);
            WriteValues(0, "ack", tcp.IsAcknowledgment);
            WriteValues(0, "psh", tcp.IsPush);
            WriteValues(0, "rst", tcp.IsReset);
            WriteValues(0, "syn", tcp.IsSynchronize);
            WriteValues(0, "fin", tcp.IsFin);
            WriteValues(2, "window size", "0x" + tcp.Window.zToHex());
            WriteValues(2, "checksum", "0x" + tcp.Checksum.zToHex());
            WriteValues(2, "urgent pointer", "0x" + tcp.UrgentPointer.zToHex());
            int optionLength = tcp.RealHeaderLength - 20;
            int n = Math.Min(8, optionLength);
            WriteValues(n, "options", null, "options length", optionLength);
            optionLength -= n;
            while (optionLength > 0)
            {
                n = Math.Min(8, optionLength);
                WriteValues(n);
                optionLength -= n;
            }
            WriteTitle("[tcp data]");
            WritePayloadData(48, tcp.Payload.zAsEnumerableWithCounter(_dataEnum.Index));
        }

        private void WriteTitle(string title)
        {
            Trace.WriteLine(title.zPadCenter(126, '-'));
        }

        private void WriteSeparation()
        {
            Trace.WriteLine("".PadRight(126, '-'));
        }

        private void WriteHeaderValues(string label1, string label2, string label3, object value3)
        {
            _sb.Append("| ");
            _sb.Append(label1.PadRight(33));
            _sb.Append("| ");
            _sb.Append(label2.PadRight(66));
            _sb.Append("| ");
            _sb.Append(label3.PadRight(15));
            _sb.Append(value3.ToString().PadRight(5));
            _sb.Append("|");
            WriteLine();
        }

        private void WriteValues(int nbData, string label1 = null, object value1 = null, string label2 = null, object value2 = null)
        {
            WriteData(nbData);
            if (label1 != null)
                _sb.Append(label1.PadRight(32));
            else
                _sb.Append("".PadRight(32));
            if (value1 != null)
                _sb.Append(value1.ToString().PadRight(34));
            else
                _sb.Append("".PadRight(34));
            if (label2 != null)
            {
                _sb.Append("| ");
                _sb.Append(label2.PadRight(15));
                if (value2 != null)
                    _sb.Append(value2.ToString().PadRight(5));
                else
                    _sb.Append("     ");
                _sb.Append("|");
            }
            else
                _sb.Append("|                     |");
            WriteLine();
        }

        private void WriteLine()
        {
            Trace.WriteLine(_sb.ToString());
            _sb.Clear();
        }

        private void WriteData(int nb)
        {
            // | 0x0000 | 00 07 CB C1 35 5D       | 
            if (nb == 0)
            {
                _sb.Append("|        |                         | ");
                //return false;
                return;
            }

            int i = 0;
            while (_dataEnum.MoveNext())
            {
                if (i == 0)
                    _sb.AppendFormat("| 0x{0} |", ((ushort)_dataEnum.Index).zToHex());
                _sb.AppendFormat(" {0}", _dataEnum.Current.zToHex());
                if (++i == nb)
                    break;
            }
            if (i == 0)
                _sb.AppendFormat("|        |");
            while (i < _maxDataPerLine1)
            {
                _sb.Append("   ");
                i++;
            }
            _sb.Append(" | ");
        }

        private void WritePayloadData_old(int nb, EnumerableWithCounter<byte> dataEnum = null)
        {
            //|        |                         | data                                                              |                     |
            //| 0x0022 | CD 67 01 BB E9 DF 53 5D | data                                                              |                     |
            //| 0x002A | 91 1F 42 06 50 10 40 3D |                                                                   |                     |
            //| 0x0032 | FF 81 00 00 00          |                                                                   |                     |
            if (dataEnum == null)
                dataEnum = _dataEnum;

            // | 0x0000 | 00 07 CB C1 35 5D       | 
            //if (nb == 0)
            //{
            //    _sb.Append("|        |                         | data                                                              |                     |");
            //    WriteLine();
            //    return;
            //}

            StringBuilder sb = new StringBuilder();
            bool firstLine = true;

            while (nb > 0)
            {
                int n = Math.Min(8, nb);
                int i = 0;
                sb.Append("\"");
                while (dataEnum.MoveNext())
                {
                    if (i == 0)
                        _sb.AppendFormat("| 0x{0} |", ((ushort)dataEnum.Index).zToHex());
                    byte b = dataEnum.Current;
                    _sb.AppendFormat(" {0}", b.zToHex());
                    if (b >= 32)
                        sb.Append((char)b);
                    else
                        sb.Append(".");
                    if (++i == n)
                        break;
                }
                sb.Append("\"");
                if (i == 0)
                {
                    _sb.AppendFormat("|        |");
                    sb.Clear();
                    sb.Append("  ");
                }
                while (i < _maxDataPerLine2)
                {
                    _sb.Append("   ");
                    sb.Append(" ");
                    i++;
                }
                _sb.Append(" | ");
                if (firstLine)
                    _sb.Append("data  ");
                else
                    _sb.Append("      ");
                _sb.Append(sb.ToString());
                _sb.Append("                                                  |                     |");
                WriteLine();
                sb.Clear();
                nb -= n;
                firstLine = false;
                if (dataEnum.EndOfData)
                    break;
            }
        }

        private void WritePayloadData(int nb, EnumerableWithCounter<byte> dataEnum = null)
        {
            //|        |                         | data                                                              |                     |
            //| 0x0022 | CD 67 01 BB E9 DF 53 5D | data                                                              |                     |
            //| 0x002A | 91 1F 42 06 50 10 40 3D |                                                                   |                     |
            //| 0x0032 | FF 81 00 00 00          |                                                                   |                     |


            //| 0x000E | 00 01 08 00 06 04 00 02 00 01 08 00 06 04 00 02 | "................"                        | data length    48   |
            //| 0x000E | 00 01 08 00 06 04 00 02 00 01 08 00 06 04 00 02 | "................"                        |                     |
            //| 0x000E | 00 01 08 00 06 04 00 02 00 01 08 00 06 04 00 02 | "................"                        |                     |


            if (dataEnum == null)
                dataEnum = _dataEnum;

            StringBuilder sb = new StringBuilder();
            bool firstLine = true;

            while (nb > 0)
            {
                int n = Math.Min(_maxDataPerLine2, nb);
                int i = 0;
                sb.Append("\"");
                while (dataEnum.MoveNext())
                {
                    if (i == 0)
                        _sb.AppendFormat("| 0x{0} |", ((ushort)dataEnum.Index).zToHex());
                    byte b = dataEnum.Current;
                    _sb.AppendFormat(" {0}", b.zToHex());
                    if (b >= 32)
                        sb.Append((char)b);
                    else
                        sb.Append(".");
                    if (++i == n)
                        break;
                }
                sb.Append("\"");
                if (i == 0)
                {
                    _sb.AppendFormat("|        |");
                    sb.Clear();
                    sb.Append("  ");
                }
                while (i < _maxDataPerLine2)
                {
                    _sb.Append("   ");
                    sb.Append(" ");
                    i++;
                }
                _sb.Append(" | ");
                //if (firstLine)
                //    _sb.Append("data  ");
                //else
                //    _sb.Append("      ");
                _sb.Append(sb.ToString());
                //_sb.Append("                                                  |                     |");




                //_sb.Append("                        |                     |");
                _sb.Append("                        ");
                //_sb.Append("|                     |");


                if (firstLine)
                {
                    _sb.Append("| ");
                    _sb.Append("data length".PadRight(15));
                    _sb.Append(_dataLength.ToString().PadRight(5));
                    _sb.Append("|");
                }
                else
                    _sb.Append("|                     |");





                WriteLine();
                sb.Clear();
                nb -= n;
                firstLine = false;
                if (dataEnum.EndOfData)
                    break;
            }
        }

        public static void PrintPacketDetailHandler(PPacket ppacket)
        {
            new PrintPacketDetail()._PrintPacketDetailHandler(ppacket);
        }
    }
}
