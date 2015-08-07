using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Xml;
using pb;
using pb.Compiler;
using pb.Data.Xml;
using pb.IO;
using pb.Text;
using wpcap_dn;

namespace Test_Winpcap
{
    static partial class w
    {
        private static ITrace _tr = Trace.CurrentTrace;
        private static RunSource _rs = RunSource.CurrentRunSource;
        private static string _dataDir = null;

        //private static TimeSpan gTime0;
        private static bool gReceivePacketEof;
        private static StreamWriter gWriter = null;
        private static XmlWriter gXmlWriter = null;
        private static List<EthernetPacket> gPacketList;
        private static TCP_Analyze gTCPAnalyze;
        private static SortedList<PacketGroup, int> gPacketGroup;
        private static int gLastGroupNumber;

        public static void Init()
        {
            //_rs.InitConfig("Test_Winpcap");
            XmlConfig.CurrentConfig = new XmlConfig("Test_Winpcap");
            _dataDir = XmlConfig.CurrentConfig.Get("DataDir");
        }

        public static string GetPath(string file)
        {
            if (file == null)
                return null;
            return zPath.Combine(_dataDir, file);
        }

        public static void Test_01()
        {
            ushort u1 = 0xB5EE;
            short u2 = IPAddress.HostToNetworkOrder((short)u1);
            ushort u3 = (ushort)u2;
            _tr.WriteLine("ushort u1 = {0}", u1);
            _tr.WriteLine("short  u2 = {0}", u2);
            _tr.WriteLine("ushort u3 = {0}", u3);
        }

        public static void TestBitField_01()
        {
            TestBitField1 t = new TestBitField1();
            t.FragmentOffset = 0x1FFF;
            t.MoreFragment = 0;
            t.DontFragment = 0;
            t.Reserved = 0;
            byte[] bytes = t.GetBitFieldStructByte();
            _tr.WriteLine("Reserved       bit 0    = {0}", t.Reserved.zToHex());
            _tr.WriteLine("DontFragment   bit 1    = {0}", t.DontFragment.zToHex());
            _tr.WriteLine("MoreFragment   bit 2    = {0}", t.MoreFragment.zToHex());
            _tr.WriteLine("FragmentOffset bit 3-15 = {0}", t.FragmentOffset.zToHex());
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes) sb.Append(" " + b.zToHex());
            _tr.WriteLine("struct BitField byte    ={0}", sb.ToString());
        }

        public static void TestBitField_02()
        {
            TestBitField2 t = new TestBitField2();
            t.FragmentOffset1 = 0xFFFF;
            t.FragmentOffset2 = 0xFFFF;
            t.MoreFragment = 0;
            t.DontFragment = 0;
            t.Reserved = 0;
            byte[] bytes = t.GetBitFieldStructByte();
            _tr.WriteLine("Reserved        bit 0    = {0}", t.Reserved.zToHex());
            _tr.WriteLine("DontFragment    bit 1    = {0}", t.DontFragment.zToHex());
            _tr.WriteLine("MoreFragment    bit 2    = {0}", t.MoreFragment.zToHex());
            _tr.WriteLine("FragmentOffset1 bit 3-7  = {0}", t.FragmentOffset1.zToHex());
            _tr.WriteLine("FragmentOffset2 bit 8-15 = {0}", t.FragmentOffset2.zToHex());
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes) sb.Append(" " + b.zToHex());
            _tr.WriteLine("struct BitField byte     ={0}", sb.ToString());
        }

        public static void TestBitField_03(byte? fragmentOffset1, byte? fragmentOffset2, byte? reserved)
        {
            TestBitField3 t = new TestBitField3();
            if (fragmentOffset1 != null) t.FragmentOffset1 = (byte)fragmentOffset1;
            if (fragmentOffset2 != null) t.FragmentOffset2 = (byte)fragmentOffset2;
            if (reserved != null) t.Reserved = (byte)reserved;
            _tr.WriteLine("FragmentOffset2 bit 0-7   = {0}", fragmentOffset2.zToHex());
            _tr.WriteLine("FragmentOffset1 bit 8-12  = {0}", fragmentOffset1.zToHex());
            _tr.WriteLine("Reserved        bit 12-15 = {0}", reserved.zToHex());
            byte[] bytes = t.Get_TestBitField3_Struct_Byte();
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes) sb.Append(" " + b.zToHex());
            _tr.WriteLine("struct BitField byte     ={0}", sb.ToString());
        }

        public static void TestBitField_04(byte? fragmentOffset1, byte? fragmentOffset2, byte? reserved)
        {
            TestBitField3 t = new TestBitField3();
            if (fragmentOffset1 != null) t.FragmentOffset1 = (byte)fragmentOffset1;
            if (fragmentOffset2 != null) t.FragmentOffset2 = (byte)fragmentOffset2;
            if (reserved != null) t.Reserved = (byte)reserved;
            _tr.WriteLine("FragmentOffset2 bit 0-7   = {0}", fragmentOffset2.zToHex());
            _tr.WriteLine("FragmentOffset1 bit 8-12  = {0}", fragmentOffset1.zToHex());
            _tr.WriteLine("Reserved        bit 12-15 = {0}", reserved.zToHex());
            byte[] bytes = t.Get_TestBitField4_Struct_Byte();
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes) sb.Append(" " + b.zToHex());
            _tr.WriteLine("struct BitField byte     ={0}", sb.ToString());
            _tr.WriteLine("FragmentOffset           = {0}", t.Get_TestBitField4_FragmentOffset().zToHex());
        }

        public static void TestBitField_05(byte? fragmentOffset1, byte? fragmentOffset2, byte? reserved)
        {
            TestBitField3 t = new TestBitField3();
            if (fragmentOffset1 != null) t.FragmentOffset1 = (byte)fragmentOffset1;
            if (fragmentOffset2 != null) t.FragmentOffset2 = (byte)fragmentOffset2;
            if (reserved != null) t.Reserved = (byte)reserved;
            _tr.WriteLine("FragmentOffset2 bit 0-7   = {0}", fragmentOffset2.zToHex());
            _tr.WriteLine("FragmentOffset1 bit 8-12  = {0}", fragmentOffset1.zToHex());
            _tr.WriteLine("Reserved        bit 12-15 = {0}", reserved.zToHex());
            byte[] bytes = t.Get_TestBitField5_Struct_Byte();
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes) sb.Append(" " + b.zToHex());
            _tr.WriteLine("struct BitField byte     ={0}", sb.ToString());
            _tr.WriteLine("FragmentOffset           = {0}", t.Get_TestBitField5_FragmentOffset().zToHex());
        }

        public static void Test_Wireshark_FilterIP_01(string file)
        {
            FileStream fs = null;
            FileStream fs2 = null;
            StreamReader sr = null;
            StreamWriter sw = null;

            try
            {
                string file2 = zpath.PathSetFileName(file, zPath.GetFileNameWithoutExtension(file) + "_filtered");
                fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                fs2 = new FileStream(file2, FileMode.Create, FileAccess.Write, FileShare.Read);
                sr = new StreamReader(fs);
                sw = new StreamWriter(fs2);
                bool keepGroup = false;
                while (!sr.EndOfStream)
                {
                    string s = sr.ReadLine();
                    if (keepGroup)
                    {
                        if (!s.StartsWith("    "))
                            keepGroup = false;
                    }
                    if (!keepGroup && (s.StartsWith("Internet Protocol") || s.StartsWith("Transmission Control Protocol")))
                        keepGroup = true;
                    if (keepGroup)
                    {
                        // supprimer : [Stream index: 2] et [SEQ/ACK analysis]
                        if (!s.StartsWith("    [Stream index:") && !s.StartsWith("    [SEQ/ACK analysis]"))
                            sw.WriteLine(s);
                    }

                }
            }
            finally
            {
                if (sw != null) sw.Close();
                if (sr != null) sr.Close();
                if (fs != null) fs.Close();
                if (fs2 != null) fs2.Close();
            }
        }

        public static void Test_Winpcap_CreateSourceString_01(CaptureSourceType type, string name)
        {
            string host = null;
            string port = null;
            string source = wpcap.CreateSourceString(type, host, port, name);
            _tr.WriteLine("SourceString type {0}, host {1}, port {2}, name {3} : {4}", type, host, port, name, source);
        }

        public static void Test_Winpcap_FindAllDevs_01()
        {
            Device[] devices = wpcap.FindAllDevs();
            _tr.WriteLine("{0} device(s)", devices.Length);
            foreach (Device device in devices)
            {
                _tr.WriteLine("Device :");
                _tr.WriteLine("  Name        : {0}", device.Name);
                _tr.WriteLine("  Description : {0}", device.Description);
                _tr.WriteLine("  Address     : {0}", device.Address);
                _tr.WriteLine("  Netmask     : {0}", device.Netmask);
            }
        }

        public static PacketReceiveEventHandler GetPacketReceiveEventHandler(string printType)
        {
            switch (printType.ToLower())
            {
                case "printpacket_sniffer":
                    return new PacketReceiveEventHandler(PrintPacket_Sniffer);
                case "printpacket_detailled":
                    return new PacketReceiveEventHandler(PrintPacket_Detailled);
                case "printpacket_ipdetailled":
                    return new PacketReceiveEventHandler(PrintPacket_IPDetailled);
                case "printpacket_wireshark":
                    return new PacketReceiveEventHandler(PrintPacket_Wireshark);
                case "printpacket_tcpdetailled":
                    return new PacketReceiveEventHandler(PrintPacket_TCPDetailled);
                case "printpacket1":
                    return new PacketReceiveEventHandler(PrintPacket1);
                case "printpacket2":
                    return new PacketReceiveEventHandler(PrintPacket2);
                case "exportxml_ip":
                    return new PacketReceiveEventHandler(ExportXml_IP);
                case "exportxml_ip_detail":
                    return new PacketReceiveEventHandler(ExportXml_IP_Detail);
                case "exportxml_ip_tcp":
                    return new PacketReceiveEventHandler(ExportXml_IP_TCP);
                case "exportxml_ip_tcp_detail":
                    return new PacketReceiveEventHandler(ExportXml_IP_TCP_Detail);
                default:
                    _tr.WriteLine("Unknow printType : {0}", printType);
                    return null;
            }
        }

        public delegate void PacketPrintHandler(EthernetPacket packet);
        public static PacketPrintHandler GetPacketPrintHandler(string printType)
        {
            switch (printType.ToLower())
            {
                case "printpacket1":
                    return new PacketPrintHandler(PrintPacket1);
                default:
                    _tr.WriteLine("Unknow printType : {0}", printType);
                    return null;
            }
        }

        public delegate void PacketPrintHandler2(TCP_StreamPacket packet);
        public static PacketPrintHandler2 GetPacketPrintHandler2(string printType)
        {
            switch (printType.ToLower())
            {
                case "printpacket1":
                    return new PacketPrintHandler2(PrintPacket1);
                default:
                    _tr.WriteLine("Unknow printType : {0}", printType);
                    return null;
            }
        }

        private static void OpenTextWriter(string path)
        {
            CloseTextWriter();
            if (path == null) return;
            path = GetPath(path);
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
            gWriter = new StreamWriter(fs);
        }

        private static void CloseTextWriter()
        {
            if (gWriter != null)
            {
                gWriter.Close();
                gWriter = null;
            }
        }

        private static void Print()
        {
            //if (gWriter == null) return;
            if (gWriter != null)
                gWriter.WriteLine();
            else
                _tr.WriteLine();
        }

        private static void Print(string format, params object[] prm)
        {
            //if (gWriter == null) return;
            if (gWriter != null)
            {
                if (prm.Length == 0)
                    gWriter.WriteLine(format);
                else
                    gWriter.WriteLine(format, prm);
            }
            else
            {
                if (prm.Length == 0)
                    _tr.WriteLine(format);
                else
                    _tr.WriteLine(format, prm);
            }
        }

        public static void Winpcap_Capture(string device, string path, string printType)
        {
            gLastGroupNumber = 0;
            OpenTextWriter(path);
            try
            {
                Winpcap_Capture(device, GetPacketReceiveEventHandler(printType));
            }
            finally
            {
                CloseTextWriter();
            }
        }

        public static void Winpcap_Capture(string device, PacketReceiveEventHandler packetReceived)
        {
            PcapDevice pcapDevice = new PcapDevice();
            // \Device\NPF_{8C984C67-71B9-44BA-BB64-87E026241D6C} -> NVIDIA nForce MCP Networking Adapter Driver (Microsoft's Packet Scheduler)
            //file://c:\_Data\_Http\Pcap\dump_01.pcap
            //string file = @"c:\_Data\_Http\Pcap\dump_01.pcap";
            CaptureSourceType type = CaptureSourceType.LocalInterface;
            if (!device.StartsWith(@"\Device", StringComparison.CurrentCultureIgnoreCase))
            {
                device = GetPath(device);
                type = CaptureSourceType.File;
            }
            string source = wpcap.CreateSourceString(type, null, null, device);
            _tr.WriteLine("Open Winpcap device : {0}", source);
            pcapDevice.Open(source, 65536, 1, 0);
            try
            {
                pcapDevice.SetMinToCopy(100);
                gReceivePacketEof = false;
                pcapDevice.PacketReceived += packetReceived;
                _tr.WriteLine("Capturing. . .");
                pcapDevice.StartListen();
                while (!_rs.IsExecutionAborted() && !gReceivePacketEof && pcapDevice.IsListening)
                {
                    Thread.Sleep(100);
                }
                _tr.WriteLine("Close capture");
                if (pcapDevice.ListenException != null) throw new PBException(pcapDevice.ListenException, "error during Listen()");
            }
            finally
            {
                pcapDevice.PacketReceived -= packetReceived;
                pcapDevice.Close();
            }
        }

        public static void Winpcap_Capture_To_Xml(string device, string path, string printType)
        {
            gLastGroupNumber = 0;
            Winpcap_Capture_To_Xml(device, path, GetPacketReceiveEventHandler(printType));

        }

        public static void Winpcap_Capture_To_Xml(string device, string path, PacketReceiveEventHandler packetReceived)
        {
            PcapDevice pcapDevice = new PcapDevice();
            // \Device\NPF_{8C984C67-71B9-44BA-BB64-87E026241D6C} -> NVIDIA nForce MCP Networking Adapter Driver (Microsoft's Packet Scheduler)
            //file://c:\_Data\_Http\Pcap\dump_01.pcap
            //string file = @"c:\_Data\_Http\Pcap\dump_01.pcap";
            CaptureSourceType type = CaptureSourceType.LocalInterface;
            if (!device.StartsWith(@"\Device", StringComparison.CurrentCultureIgnoreCase)) type = CaptureSourceType.File;
            string source = wpcap.CreateSourceString(type, null, null, device);
            _tr.WriteLine("Open Winpcap device : {0}", source);
            pcapDevice.Open(source, 65536, 1, 0);

            gXmlWriter = null;

            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = ("    ");
                gXmlWriter = XmlWriter.Create(path, settings);
                gXmlWriter.WriteStartElement("Ethernet");

                pcapDevice.SetMinToCopy(100);
                gReceivePacketEof = false;
                pcapDevice.PacketReceived += packetReceived;
                _tr.WriteLine("Capturing. . .");
                pcapDevice.StartListen();
                while (!_rs.IsExecutionAborted() && !gReceivePacketEof && pcapDevice.IsListening)
                {
                    Thread.Sleep(100);
                }
                gXmlWriter.WriteEndElement();
                if (pcapDevice.ListenException != null) throw new PBException(pcapDevice.ListenException, "error during Listen()");
            }
            finally
            {
                _tr.WriteLine("Close capture");
                pcapDevice.PacketReceived -= packetReceived;
                pcapDevice.Close();

                if (gXmlWriter != null)
                {
                    gXmlWriter.Close();
                    gXmlWriter = null;
                }
            }
        }

        public static void Winpcap_Capture_Sorted(string device, string path, string printType)
        {
            gPacketList = new List<EthernetPacket>();
            gPacketGroup = new SortedList<PacketGroup, int>();
            gLastGroupNumber = 0;
            Winpcap_Capture(device, new PacketReceiveEventHandler(AddPacketToList));
            _tr.WriteLine("{0} packet captured", gPacketList.Count);
            //var q = from p in gPacketList where p.TCPHeader != null orderby p.TCPHeader.source, p.TCPHeader.dest select p;
            var q = from p in gPacketList where p.TCP != null orderby p.gGroupNumber, p.PacketNumber select p;
            _tr.WriteLine("{0} packet selectionned", q.Count());
            PacketPrintHandler packetHandler = GetPacketPrintHandler(printType);
            OpenTextWriter(path);
            try
            {
                foreach (EthernetPacket p in q)
                    packetHandler(p);
            }
            finally
            {
                CloseTextWriter();
            }
        }

        public static void Winpcap_Capture_TCP_Analyze(string device, string path, string printType)
        {
            gTCPAnalyze = new TCP_Analyze();
            Winpcap_Capture(device, new PacketReceiveEventHandler(AddPacketToTCPPacket));
            _tr.WriteLine("{0} packet captured", gTCPAnalyze.Packets.Count);
            //var q = from p in gPacketList where p.TCPHeader != null orderby p.gGroupNumber, p.PacketNumber select p;
            var q = from p in gTCPAnalyze.Packets where p.Packet.TCP != null orderby p.Address.StreamNumber, p.Packet.PacketNumber select p;
            _tr.WriteLine("{0} packet selectionned", q.Count());
            PacketPrintHandler2 printHandler = GetPacketPrintHandler2(printType);
            OpenTextWriter(path);
            try
            {
                foreach (TCP_StreamPacket p in q)
                    printHandler(p);
            }
            finally
            {
                CloseTextWriter();
            }
        }

        public static void Winpcap_Capture_TCP_Analyze_To_Xml(string device, string path)
        {
            gTCPAnalyze = new TCP_Analyze();
            Winpcap_Capture(device, new PacketReceiveEventHandler(AddPacketToTCPPacket));
            _tr.WriteLine("{0} packet captured", gTCPAnalyze.Packets.Count);
            gTCPAnalyze.XmlExportPackets(path);
        }

        public static void Test_Winpcap_Dump_01(string filename)
        {
            PcapDevice pcapDevice = new PcapDevice();
            //((Device)devlist[cbAdapters.SelectedIndex]).Name
            //string device = @"\Device\NPF_{8C984C67-71B9-44BA-BB64-87E026241D6C}";
            string device = @"\Device\NPF_{BF8A52CB-F023-4F24-AA7E-958A8D1F3069}";
            _tr.WriteLine("Open Winpcap device : {0}", device);
            pcapDevice.Open(device, 65536, 1, 0);
            pcapDevice.SetMinToCopy(100);
            pcapDevice.StartDump(filename);
            //gPacketCount = 0;
            pcapDevice.PacketReceived += new PacketReceiveEventHandler(PrintPacket_Sniffer);
            pcapDevice.StartListen();
            _tr.WriteLine("Dump to file {0}", filename);
            while (!_rs.IsExecutionAborted())
            {
                Thread.Sleep(100);
            }
            pcapDevice.PacketReceived -= new PacketReceiveEventHandler(PrintPacket_Sniffer);
            pcapDevice.Close();
        }

        public static void Winpcap_Dump(string device, string filename, string path, string printType)
        {
            gLastGroupNumber = 0;
            OpenTextWriter(path);
            try
            {
                Winpcap_Dump(device, filename, GetPacketReceiveEventHandler(printType));
            }
            finally
            {
                CloseTextWriter();
            }
        }

        public static void Winpcap_Dump(string device, string filename, PacketReceiveEventHandler packetReceived)
        {
            PcapDevice pcapDevice = new PcapDevice();
            CaptureSourceType type = CaptureSourceType.LocalInterface;
            if (!device.StartsWith(@"\Device", StringComparison.CurrentCultureIgnoreCase))
            {
                device = GetPath(device);
                type = CaptureSourceType.File;
            }
            string source = wpcap.CreateSourceString(type, null, null, device);
            _tr.WriteLine("Open Winpcap device : {0}", source);
            pcapDevice.Open(source, 65536, 1, 0);
            try
            {
                pcapDevice.SetMinToCopy(100);
                filename = GetPath(filename);
                _tr.WriteLine("Dump to file {0}", filename);
                pcapDevice.StartDump(filename);
                gReceivePacketEof = false;
                pcapDevice.PacketReceived += packetReceived;
                _tr.WriteLine("Capturing. . .");
                pcapDevice.StartListen();
                while (!_rs.IsExecutionAborted() && !gReceivePacketEof && pcapDevice.IsListening)
                {
                    Thread.Sleep(100);
                }
                _tr.WriteLine("Close capture");
                if (pcapDevice.ListenException != null) throw new PBException(pcapDevice.ListenException, "error during Listen()");
            }
            finally
            {
                pcapDevice.PacketReceived -= packetReceived;
                pcapDevice.Close();
            }
        }

        public static void PrintPacket_Detailled(PcapDevice sender, EthernetPacket packet)
        {
            //if (packet.PacketNumber > 10)
            //{
            //    if (!gReceivePacketEof)
            //    {
            //        gReceivePacketEof = true;
            //        _tr.WriteLine("Stop after packet 10 ****************************");
            //    }
            //    return;
            //}
            if (packet.State == PcapState.Eof)
            {
                gReceivePacketEof = true;
                _tr.WriteLine("Packet Eof ****************************");
                return;
            }
            // ************************************************************************************************************************
            // BUG BUG BUG BUG BUG BUG BUG BUG BUG BUG BUG BUG
            // Print("{0,5} {1:0.000000} {3}", packet.PacketNumber, packet.RelativeTime.TotalSeconds, packet.ProtocolCode);
            // pb on ne voit pas ds le message d'erreur la pile d'ou vient l'erreur
            // ************************************************************************************************************************
            Print("{0,5} {1,10:0.000000} {2}", packet.PacketNumber, packet.RelativeTime.TotalSeconds, packet.ProtocolCode);
            Print_IPDetailled(packet);
            Print_TCPDetailled(packet);
        }

        public static void PrintPacket_IPDetailled(PcapDevice sender, EthernetPacket packet)
        {
            //if (packet.PacketNumber > 10)
            //{
            //    if (!gReceivePacketEof)
            //    {
            //        gReceivePacketEof = true;
            //        Print("Stop after packet 10 ****************************");
            //    }
            //    return;
            //}
            if (packet.State == PcapState.Eof)
            {
                gReceivePacketEof = true;
                _tr.WriteLine("Packet Eof ****************************");
                return;
            }
            // ************************************************************************************************************************
            // BUG BUG BUG BUG BUG BUG BUG BUG BUG BUG BUG BUG
            // Print("{0,5} {1:0.000000} {3}", packet.PacketNumber, packet.RelativeTime.TotalSeconds, packet.ProtocolCode);
            // pb on ne voit pas ds le message d'erreur la pile d'ou vient l'erreur
            // ************************************************************************************************************************
            Print("{0,5} {1,10:0.000000} {2}", packet.PacketNumber, packet.RelativeTime.TotalSeconds, packet.ProtocolCode);
            Print_IPDetailled(packet);
        }

        public static void Print_IPDetailled(EthernetPacket packet)
        {
            IP ip = packet.IP;
            if (ip == null) return;
            byte[] ipByte = ip.HeaderBytes;
            Print("                                      Bytes    Bits   Length        Value             Header bytes");
            Print("      IP Version                      0-1      0-3     4 bits       0x{0}              {1} {2}", ip.Version.zToHex(), ipByte[0].zToHex(), ipByte[1].zToHex());
            Print("      IP HeaderLength                 0-1      4-7     4 bits       0x{0}              {1} {2}", ip.WordHeaderLength.zToHex(), ipByte[0].zToHex(), ipByte[1].zToHex());
            Print("      IP DSCP                         0-1      8-13    6 bits       0x{0}              {1} {2}", ip.DSCP.zToHex(), ipByte[0].zToHex(), ipByte[1].zToHex());
            Print("      IP ECN                          0-1      14-15   2 bits       0x{0}              {1} {2}", ip.ECN.zToHex(), ipByte[0].zToHex(), ipByte[1].zToHex());
            Print("      IP TotalLength                  2-3      -       2 bytes      0x{0}            {1} {2}", ip.TotalLength.zToHex(), ipByte[2].zToHex(), ipByte[3].zToHex());
            Print("      IP ID                           4-5      -       2 bytes      0x{0}            {1} {2}", ip.ID.zToHex(), ipByte[4].zToHex(), ipByte[5].zToHex());
            Print("      IP Reserved                     6-7      0       1 bit        0x0{0}              {1} {2}", ip.Reserved.zToString01(), ipByte[6].zToHex(), ipByte[7].zToHex());
            Print("      IP DontFragment                 6-7      1       1 bit        0x0{0}              {1} {2}", ip.DontFragment.zToString01(), ipByte[6].zToHex(), ipByte[7].zToHex());
            Print("      IP MoreFragment                 6-7      2       1 bit        0x0{0}              {1} {2}", ip.MoreFragment.zToString01(), ipByte[6].zToHex(), ipByte[7].zToHex());
            Print("      IP FragmentOffset               6-7      3-15   13 bits       0x{0}            {1} {2}", ip.FragmentOffset.zToHex(), ipByte[6].zToHex(), ipByte[7].zToHex());
            Print("      IP TimeToLive                   8        -       1 byte       0x{0}              {1}", ip.TimeToLive.zToHex(), ipByte[8].zToHex());
            Print("      IP Protocol                     9        -       1 byte       0x{0}              {1}", ip.ProtocolValue.zToHex(), ipByte[9].zToHex());
            Print("      IP Checksum                     10-11    -       2 bytes      0x{0}            {1} {2}", ip.Checksum.zToHex(), ipByte[10].zToHex(), ipByte[11].zToHex());
            StringBuilder sb = new StringBuilder();
            foreach (byte a in ip.SourceAddress.GetAddressBytes()) sb.Append(a.zToHex());
            Print("      IP Source                       12-15    -       4 bytes      0x{0}        {1} {2} {3} {4}", sb.ToString(), ipByte[12].zToHex(), ipByte[13].zToHex(), ipByte[14].zToHex(), ipByte[15].zToHex());
            sb = new StringBuilder();
            foreach (byte a in ip.DestinationAddress.GetAddressBytes()) sb.Append(a.zToHex());
            Print("      IP Destination                  16-19    -       4 bytes      0x{0}        {1} {2} {3} {4}", sb.ToString(), ipByte[16].zToHex(), ipByte[17].zToHex(), ipByte[18].zToHex(), ipByte[19].zToHex());
            Print();
            PrintData(ipByte, ipByte.Length, "      ip header  : ", true, false);
            Print();
        }

        public static void PrintPacket_TCPDetailled(PcapDevice sender, EthernetPacket packet)
        {
            //if (packet.PacketNumber > 10)
            //{
            //    if (!gReceivePacketEof)
            //    {
            //        gReceivePacketEof = true;
            //        Print("Stop after packet 10 ****************************");
            //    }
            //    return;
            //}
            if (packet.State == PcapState.Eof)
            {
                gReceivePacketEof = true;
                _tr.WriteLine("Packet Eof ****************************");
                return;
            }
            // ************************************************************************************************************************
            // BUG BUG BUG BUG BUG BUG BUG BUG BUG BUG BUG BUG
            // Print("{0,5} {1:0.000000} {3}", packet.PacketNumber, packet.RelativeTime.TotalSeconds, packet.ProtocolCode);
            // pb on ne voit pas ds le message d'erreur la pile d'ou vient l'erreur
            // ************************************************************************************************************************
            Print("{0,5} {1,10:0.000000} {2}", packet.PacketNumber, packet.RelativeTime.TotalSeconds, packet.ProtocolCode);
            Print_TCPDetailled(packet);
        }

        public static void Print_TCPDetailled(EthernetPacket packet)
        {
            TCP tcp = packet.TCP;
            if (tcp == null) return;
            byte[] tcpByte = tcp.HeaderBytes;
            Print("                                      Bytes    Bits   Length        Value             Header bytes");
            Print("      TCP Source port                 0-1      -       2 bytes      0x{0}            {1} {2}", tcp.SourcePort.zToHex(), tcpByte[0].zToHex(), tcpByte[1].zToHex());
            Print("      TCP Destination port            2-3      -       2 bytes      0x{0}            {1} {2}", tcp.DestinationPort.zToHex(), tcpByte[2].zToHex(), tcpByte[3].zToHex());
            Print("      TCP Sequence number             4-7      -       4 bytes      0x{0}        {1} {2} {3} {4}", tcp.Sequence.zToHex(), tcpByte[4].zToHex(), tcpByte[5].zToHex(), tcpByte[6].zToHex(), tcpByte[7].zToHex());
            Print("      TCP Acknowledgment number       8-11     -       4 bytes      0x{0}        {1} {2} {3} {4}", tcp.AckSequence.zToHex(), tcpByte[8].zToHex(), tcpByte[9].zToHex(), tcpByte[10].zToHex(), tcpByte[11].zToHex());
            Print("      TCP Data offset                 12-13    0-3     4 bits       0x{0}              {1} {2}", tcp.DataOffset.zToHex(), tcpByte[12].zToHex(), tcpByte[13].zToHex());
            Print("      TCP Reserved                    12-13    4-7     4 bits       0x{0}              {1} {2}", tcp.Reserved.zToHex(), tcpByte[12].zToHex(), tcpByte[13].zToHex());
            Print("      TCP Flags                       12-13    8-15    1 byte       0x{0}              {1} {2}", tcp.Flags.zToHex(), tcpByte[12].zToHex(), tcpByte[13].zToHex());
            Print("      TCP CWR                         12-13    8       1 bit        0x0{0}              {1} {2}", tcp.CongestionWindowReduced.zToString01(), tcpByte[12].zToHex(), tcpByte[13].zToHex());
            Print("      TCP ECE                         12-13    9       1 bit        0x0{0}              {1} {2}", tcp.ECN_Echo.zToString01(), tcpByte[12].zToHex(), tcpByte[13].zToHex());
            Print("      TCP URG                         12-13    10      1 bit        0x0{0}              {1} {2}", tcp.Urgent.zToString01(), tcpByte[12].zToHex(), tcpByte[13].zToHex());
            Print("      TCP ACK                         12-13    11      1 bit        0x0{0}              {1} {2}", tcp.Ack.zToString01(), tcpByte[12].zToHex(), tcpByte[13].zToHex());
            Print("      TCP PSH                         12-13    12      1 bit        0x0{0}              {1} {2}", tcp.Push.zToString01(), tcpByte[12].zToHex(), tcpByte[13].zToHex());
            Print("      TCP RST                         12-13    13      1 bit        0x0{0}              {1} {2}", tcp.Reset.zToString01(), tcpByte[12].zToHex(), tcpByte[13].zToHex());
            Print("      TCP SYN                         12-13    14      1 bit        0x0{0}              {1} {2}", tcp.Synchronize.zToString01(), tcpByte[12].zToHex(), tcpByte[13].zToHex());
            Print("      TCP FIN                         12-13    15      1 bit        0x0{0}              {1} {2}", tcp.Finish.zToString01(), tcpByte[12].zToHex(), tcpByte[13].zToHex());
            Print("      TCP Window Size                 14-15    -       2 bytes      0x{0}            {1} {2}", tcp.Window.zToHex(), tcpByte[14].zToHex(), tcpByte[15].zToHex());
            Print("      TCP Checksum                    16-17    -       2 bytes      0x{0}            {1} {2}", tcp.Checksum.zToHex(), tcpByte[16].zToHex(), tcpByte[17].zToHex());
            Print("      TCP Urgent pointer              18-19    -       2 bytes      0x{0}            {1} {2}", tcp.UrgentPointer.zToHex(), tcpByte[18].zToHex(), tcpByte[19].zToHex());
            if (tcp.OptionsLength > 0)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < tcp.OptionsLength; i++) sb.Append(" " + tcpByte[i + 20].zToHex());
                string nbBytes = string.Format("{0,2} bytes", tcp.OptionsLength);
                Print("      TCP Options                     20-{0}    -      {1}                       {2}", tcp.OptionsLength + 19, nbBytes, sb.ToString());
            }
            else
                Print("      TCP Options                     -        -       -");

            Print();
            PrintData(tcpByte, tcpByte.Length, "      tcp header : ", true, false);
            Print();
        }

        public static void PrintPacket_Wireshark(PcapDevice sender, EthernetPacket packet)
        {
            if (packet.State == PcapState.Eof)
            {
                gReceivePacketEof = true;
                _tr.WriteLine("Packet Eof ****************************");
                return;
            }
            //**************************************************************************************************
            //Internet Protocol, Src: 192.168.0.2 (192.168.0.2), Dst: 212.27.40.241 (212.27.40.241)
            //    Version: 4
            //    Header length: 20 bytes
            //    Differentiated Services Field: 0x00 (DSCP 0x00: Default; ECN: 0x00)
            //        0000 00.. = Differentiated Services Codepoint: Default (0x00)
            //        .... ..0. = ECN-Capable Transport (ECT): 0
            //        .... ...0 = ECN-CE: 0
            //    Total Length: 59
            //    Identification: 0xeeb5 (61109)
            //    Flags: 0x00
            //        0... .... = Reserved bit: Not set
            //        .0.. .... = Don't fragment: Not set
            //        ..0. .... = More fragments: Not set
            //    Fragment offset: 0
            //    Time to live: 128
            //    Protocol: UDP (17)
            //    Header checksum: 0x8e45 [correct]
            //        [Good: True]
            //        [Bad: False]
            //    Source: 192.168.0.2 (192.168.0.2)
            //    Destination: 212.27.40.241 (212.27.40.241)
            //**************************************************************************************************

            //**************************************************************************************************
            //No.     Time        Source                Destination           Protocol Info
            //      1 0.000000    192.168.0.2           212.27.40.241         DNS      Standard query A www.google.fr
            //
            //Frame 1: 73 bytes on wire (584 bits), 73 bytes captured (584 bits)
            //Ethernet II, Src: AsustekC_bc:cf:c9 (00:13:d4:bc:cf:c9), Dst: FreeboxS_c1:35:5d (00:07:cb:c1:35:5d)
            //Internet Protocol, Src: 192.168.0.2 (192.168.0.2), Dst: 209.85.229.100 (209.85.229.100)
            //    Version: 4
            //    Header length: 20 bytes
            //    Differentiated Services Field: 0x00 (DSCP 0x00: Default; ECN: 0x00)
            //    Total Length: 48
            //    Identification: 0xeeb7 (61111)
            //    Flags: 0x02 (Don't Fragment)
            //        0... .... = Reserved bit: Not set
            //        .1.. .... = Don't fragment: Set
            //        ..0. .... = More fragments: Not set
                //    Fragment offset: 0
            //    Time to live: 128
            //    Protocol: TCP (6)
            //    Header checksum: 0x94ab [correct]
            //    Source: 192.168.0.2 (192.168.0.2)
            //    Destination: 209.85.229.100 (209.85.229.100)
            //Transmission Control Protocol, Src Port: chromagrafx (1373), Dst Port: http (80), Seq: 0, Len: 0
            //    Source port: chromagrafx (1373)
            //    Destination port: http (80)
            //    [Stream index: 2]
            //    Sequence number: 0    (relative sequence number)
            //    Acknowledgement number: 1    (relative ack number)
            //    Header length: 28 bytes
            //    Flags: 0x02 (SYN)
            //    Flags: 0x10 (ACK)
            //    Flags: 0x11 (FIN, ACK)
            //    Flags: 0x18 (PSH, ACK)
            //    Flags: 0x12 (SYN, ACK)
            //        0... .... = Congestion Window Reduced (CWR): Not set
            //        .0.. .... = ECN-Echo: Not set
            //        ..0. .... = Urgent: Not set
            //        ...1 .... = Acknowledgement: Set
            //        .... 0... = Push: Not set
            //        .... .0.. = Reset: Not set
            //        .... ..1. = Syn: Set
            //        .... ...0 = Fin: Not set
            //    Window size: 65535
            //    Checksum: 0xff8c [validation disabled]
            //    Options: (8 bytes)
            //**************************************************************************************************

            IP ip = packet.IP;
            //if (ip == null) return;
            //byte[] ipByte = ip.HeaderBytes;
            //Print("No.     Time        Source                Destination           Protocol Info");
            //string source = "";
            //string destination = "";
            //if (ip != null)
            //{
            //    source = ip.Source.ToString();
            //    destination = ip.Destination.ToString();
            //}
            //Print("{0,7} {1:0.000000}    {2,-15}       {3,-15}       {4}", packet.PacketNumber, packet.RelativeTime.TotalSeconds, source, destination, packet.ProtocolCode);
            //Print();
            //Print("Frame 1: {0} bytes on wire ({1} bits), {0} bytes captured ({1} bits)", packet.PacketHeader.len, packet.PacketHeader.len * 8);
            //Print("Ethernet II, Src:                                        Dst:                                      ");
            if (ip != null)
            {
                Print("Internet Protocol, Src: {0} ({0}), Dst: {1} ({1})", ip.SourceAddress.ToString(), ip.DestinationAddress.ToString());
                Print("    Version: {0}", ip.Version);
                Print("    Header length: {0} bytes", ip.WordHeaderLength * 4);
                Print("    Differentiated Services Field: 0x{0} (DSCP 0x{1}: Default; ECN: 0x{2})", ip.DifferentiatedServices.zToHex(), ip.DSCP.zToHex(), ip.ECN.zToHex());
                Print("    Total Length: {0}", ip.TotalLength);
                Print("    Identification: 0x{0} ({1})", ip.ID.zToHex().ToLower(), ip.ID);
                string flag = "";
                if (ip.DontFragment)
                    flag = " (Don't Fragment)";
                else if (ip.MoreFragment)
                    flag = " (More Fragment)";
                Print("    Flags: 0x{0}{1}", ip.Flags.zToHex(), flag);
                Print("        {0}... .... = Reserved bit: {1}", ip.Reserved.zToString01(), SetOrNotSet(ip.Reserved));
                Print("        .{0}.. .... = Don't fragment: {1}", ip.DontFragment.zToString01(), SetOrNotSet(ip.DontFragment));
                Print("        ..{0}. .... = More fragments: {1}", ip.MoreFragment.zToString01(), SetOrNotSet(ip.MoreFragment));
                Print("    Fragment offset: {0}", ip.FragmentOffset);
                Print("    Time to live: {0}", ip.TimeToLive);
                Print("    Protocol: {0} ({1})", ip.ProtocolCode, ip.ProtocolValue);
                Print("    Header checksum: 0x{0} [correct]", ip.Checksum.zToHex().ToLower());
                Print("    Source: {0} ({0})", ip.SourceAddress.ToString());
                Print("    Destination: {0} ({0})", ip.DestinationAddress.ToString());
            }
            TCP tcp = packet.TCP;
            if (tcp != null)
            {
                string srcPort = PortName(tcp.SourcePort);
                string dstPort = PortName(tcp.DestinationPort);
                // problème :
                //       Sequence number: tcp.seq
                //       Acknowledgement number:
                //       Stream index:
                //       [SEQ/ACK analysis]
                //       [Next sequence number: 633    (relative sequence number)]


                Print("Transmission Control Protocol, Src Port: {0} ({1}), Dst Port: {2} ({3}), Seq: {4}, Len: {5}", srcPort, tcp.SourcePort, dstPort, tcp.DestinationPort, tcp.Sequence, tcp.DataLength);
                Print("    Source port: {0} ({1})", srcPort, tcp.SourcePort);
                Print("    Destination port: {0} ({1})", dstPort, tcp.DestinationPort);
                Print("    [Stream index: ?]");
                Print("    Sequence number: {0}    (relative sequence number)", tcp.Sequence);
                if (tcp.AckSequence != 0) Print("    Acknowledgement number: {0}    (relative ack number)", tcp.AckSequence);
                Print("    Header length: {0} bytes", tcp.DataOffset * 4);
                //string flags = TCPFlags(tcp);
                string flags = tcp.GetFlagsString();
                if (flags != "") flags = " (" + flags + ")";
                Print("    Flags: 0x{0}{1}", tcp.Flags.zToHex(), flags);
                Print("        {0}... .... = Congestion Window Reduced (CWR): {1}", tcp.CongestionWindowReduced.zToString01(), SetOrNotSet(tcp.CongestionWindowReduced));
                Print("        .{0}.. .... = ECN-Echo: {1}", tcp.ECN_Echo.zToString01(), SetOrNotSet(tcp.ECN_Echo));
                Print("        ..{0}. .... = Urgent: {1}", tcp.Urgent.zToString01(), SetOrNotSet(tcp.Urgent));
                Print("        ...{0} .... = Acknowledgement: {1}", tcp.Ack.zToString01(), SetOrNotSet(tcp.Ack));
                Print("        .... {0}... = Push: {1}", tcp.Push.zToString01(), SetOrNotSet(tcp.Push));
                Print("        .... .{0}.. = Reset: {1}", tcp.Reset.zToString01(), SetOrNotSet(tcp.Reset));
                Print("        .... ..{0}. = Syn: {1}", tcp.Synchronize.zToString01(), SetOrNotSet(tcp.Synchronize));
                Print("        .... ...{0} = Fin: {1}", tcp.Finish.zToString01(), SetOrNotSet(tcp.Finish));
                Print("    Window size: {0}", tcp.Window);
                Print("    Checksum: 0x{0} [validation disabled]", tcp.Checksum.zToHex().ToLower());
                if (tcp.OptionsLength > 0) Print("    Options: ({0} bytes)", tcp.OptionsLength);
            }
            //Print();
        }

        public static void ExportXml_IP(PcapDevice sender, EthernetPacket packet)
        {
            //ExportXml_Ethernet_Begin(gXmlWriter, packet);
            //if (packet.IP != null) packet.IP.ExportXml(gXmlWriter, false, false);
            //ExportXml_Ethernet_End(gXmlWriter);
            packet.ExportXml(gXmlWriter, ExportXmlOptions.IP);
        }

        public static void ExportXml_IP_Detail(PcapDevice sender, EthernetPacket packet)
        {
            //ExportXml_Ethernet_Begin(gXmlWriter, packet);
            //if (packet.IP != null) packet.IP.ExportXml(gXmlWriter, true, true);
            //ExportXml_Ethernet_End(gXmlWriter);
            packet.ExportXml(gXmlWriter, ExportXmlOptions.IP_Detailled);
        }

        public static void ExportXml_IP_TCP(PcapDevice sender, EthernetPacket packet)
        {
            //ExportXml_Ethernet_Begin(gXmlWriter, packet);
            //if (packet.IP != null) packet.IP.ExportXml(gXmlWriter, false, false);
            //if (packet.TCP != null) packet.TCP.ExportXml(gXmlWriter, false, false);
            //ExportXml_Ethernet_End(gXmlWriter);
            packet.ExportXml(gXmlWriter, ExportXmlOptions.IP | ExportXmlOptions.TCP);
        }

        public static void ExportXml_IP_TCP_Detail(PcapDevice sender, EthernetPacket packet)
        {
            //ExportXml_Ethernet_Begin(gXmlWriter, packet);
            //if (packet.IP != null) packet.IP.ExportXml(gXmlWriter, false, false);
            //if (packet.TCP != null) packet.TCP.ExportXml(gXmlWriter, true, true);
            //ExportXml_Ethernet_End(gXmlWriter);
            packet.ExportXml(gXmlWriter, ExportXmlOptions.IP | ExportXmlOptions.TCP_Detailled);
        }

        //public static void ExportXml_Ethernet_Begin(XmlWriter writer, EthernetPacket packet)
        //{
        //    writer.WriteStartElement("Packet");
        //    writer.WriteAttributeString("number", packet.PacketNumber.ToString());
        //    writer.WriteAttributeString("time", packet.RelativeTime.TotalSeconds.ToString("0.000000"));
        //    writer.WriteAttributeString("protocol", packet.ProtocolCode);
        //}

        //public static void ExportXml_Ethernet_End(XmlWriter writer)
        //{
        //    writer.WriteEndElement();
        //}

        private static string PortName(ushort port)
        {
            switch (port)
            {
                case 80: return "http";
                case 1373: return "chromagrafx";
                case 1375: return "bytex";
                case 1377: return "cichlid";
                case 1379: return "dbreporter";
                case 1381: return "apple-licman";
                default: return "????";
            }
        }

        private static string SetOrNotSet(bool flag)
        {
            if (!flag)
                return "Not set";
            else
                return "Set";
        }

        public class PacketGroup : IComparable<PacketGroup>
        {
            public IPAddress Address1 = null;
            public ushort Port1 = 0;
            public IPAddress Address2 = null;
            public ushort Port2 = 0;

            public byte[] gAddress1Bytes = null;
            public byte[] gAddress2Bytes = null;

            public PacketGroup()
            {
            }

            public PacketGroup(EthernetPacket packet)
            {
                IPAddress address1 = packet.IP.SourceAddress;
                IPAddress address2 = packet.IP.DestinationAddress;
                byte[] address1Bytes = address1.GetAddressBytes();
                byte[] address2Bytes = address2.GetAddressBytes();
                ushort port1 = packet.TCP.SourcePort;
                ushort port2 = packet.TCP.DestinationPort;

                if (AddressBytesCompare(address1Bytes, port1, address2Bytes, port2) <= 0)
                {
                    Address1 = address1;
                    Address2 = address2;
                    gAddress1Bytes = address1Bytes;
                    gAddress2Bytes = address2Bytes;
                    Port1 = port1;
                    Port2 = port2;
                }
                else
                {
                    Address1 = address2;
                    Address2 = address1;
                    gAddress1Bytes = address2Bytes;
                    gAddress2Bytes = address1Bytes;
                    Port1 = port2;
                    Port2 = port1;
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

            public int CompareTo(PacketGroup other)
            {
                int r = AddressBytesCompare(gAddress1Bytes, Port1, other.gAddress1Bytes, other.Port1);
                if (r != 0) return r;
                return AddressBytesCompare(gAddress2Bytes, Port2, other.gAddress2Bytes, other.Port2);
            }
        }

        public static void Test_PacketGroup()
        {
            gPacketGroup = new SortedList<PacketGroup, int>();
            PacketGroup group = new PacketGroup();
            group.Address1 = new IPAddress(new byte[] { 209, 85, 229, 100 });
            group.gAddress1Bytes = group.Address1.GetAddressBytes();
            group.Port1 = 80;
            group.Address2 = new IPAddress(new byte[] { 192, 168, 0, 2 });
            group.gAddress2Bytes = group.Address2.GetAddressBytes();
            group.Port2 = 1373;
            _tr.WriteLine("PacketGroupNumber : {0}", GetPacketGroupNumber(group));
            _tr.WriteLine("PacketGroupNumber : {0}", GetPacketGroupNumber(group));
        }

        public static int GetPacketGroupNumber(PacketGroup group)
        {
            int i = gPacketGroup.IndexOfKey(group);
            if (i != -1)
                return gPacketGroup.Values[i];
            else
            {
                gPacketGroup.Add(group, gPacketGroup.Count + 1);
                return gPacketGroup.Count;
            }
        }

        public static void SetPacketGroupNumber(EthernetPacket packet)
        {
            if (packet.TCP == null)
            {
                packet.gGroupNumber = 0;
                return;
            }
            PacketGroup group = new PacketGroup(packet);
            packet.gGroupNumber = GetPacketGroupNumber(group);
        }

        public static void AddPacketToList(PcapDevice sender, EthernetPacket packet)
        {
            if (packet.State == PcapState.Eof)
            {
                gReceivePacketEof = true;
                _tr.WriteLine("Packet Eof ****************************");
                return;
            }
            SetPacketGroupNumber(packet);
            gPacketList.Add(packet);
            //_tr.WriteLine("AddPacketToList : {0}", packet.PacketNumber);
        }

        public static void AddPacketToTCPPacket(PcapDevice sender, EthernetPacket packet)
        {
            if (packet.State == PcapState.Eof)
            {
                gReceivePacketEof = true;
                _tr.WriteLine("Packet Eof ****************************");
                return;
            }
            //_tr.WriteLine("AddPacketToTCPPacket : {0}", packet.PacketNumber);
            gTCPAnalyze.Add(packet);
        }

        public static void PrintPacket1(PcapDevice sender, EthernetPacket packet)
        {
            if (packet.State == PcapState.Eof)
            {
                gReceivePacketEof = true;
                _tr.WriteLine("Packet Eof ****************************");
                return;
            }
            PrintPacket1(packet);
        }

        public static void PrintPacket1(EthernetPacket packet)
        {
            PrintPacket1(packet, null);
        }

        public static void PrintPacket1(TCP_StreamPacket streamPacket)
        {
            if (streamPacket.Packet.gGroupNumber != gLastGroupNumber)
            {
                Print();
                Print(GetPacketHeader1(streamPacket.Direction));
                gLastGroupNumber = streamPacket.Packet.gGroupNumber;
            }
            string s = GetPacketString1(streamPacket.Packet, streamPacket.Direction, false);
            string s2 = null;
            foreach (TCP_PacketError error in streamPacket.Errors)
                s2 = s2.zAddValue(streamPacket.GetErrorMessage(error));

            if (s2 != null) s2 = " " + s2;
            Print("{0,-162}{1}{2}", s, streamPacket.PacketDescription, s2);
        }

        public static void PrintPacket1(EthernetPacket packet, TCP_Direction? direction)
        {
            if (packet.gGroupNumber != gLastGroupNumber)
            {
                Print();
                Print(GetPacketHeader1(direction));
                gLastGroupNumber = packet.gGroupNumber;
            }
            Print(GetPacketString1(packet, direction, true));
        }

        public static void PrintPacket2(PcapDevice sender, EthernetPacket packet)
        {
            if (packet.State == PcapState.Eof)
            {
                gReceivePacketEof = true;
                _tr.WriteLine("Packet Eof ****************************");
                return;
            }
            PrintPacket2(packet);
        }

        public static void PrintPacket2(EthernetPacket packet)
        {
            PrintPacket2(packet, null);
        }

        public static void PrintPacket2(EthernetPacket packet, TCP_Direction? direction)
        {
            //if (packet.gGroupNumber != gLastGroupNumber)
            //{
            //    Print();
            //    Print(GetPacketHeader1(direction));
            //    gLastGroupNumber = packet.gGroupNumber;
            //}
            Print(GetPacketString2(packet, direction, true));
        }

        public static string GetPacketHeader1(TCP_Direction? direction)
        {
            if (direction == null)
                return "group packet    time      source                 destination           protocol flags                data   seq        next seq   ack        window urg       data";
            else
                return "group packet    time      source                 dir   destination           protocol flags                data   seq        next seq   ack        window urg       data";
        }

        public static string GetPacketString1(EthernetPacket packet, TCP_Direction? direction, bool printData)
        {
            IP ip = packet.IP;
            TCP tcp = packet.TCP;
            string saddr = null;
            string daddr = null;
            string mf = null;
            string offset = null;
            string id = null;
            if (ip != null)
            {
                saddr = ip.SourceAddress.ToString();
                daddr = ip.DestinationAddress.ToString();
                if (tcp != null)
                {
                    saddr += ":" + tcp.SourcePort.ToString();
                    daddr += ":" + tcp.DestinationPort.ToString();
                }
                mf = ip.MoreFragment.ToString();
                offset = ip.FragmentOffset.zToHex();
                id = ip.ID.zToHex();
            }
            StringBuilder sb = new StringBuilder();
            //direction
            string dir = null;
            if (direction != null)
            {
                if ((TCP_Direction)direction == TCP_Direction.SourceToDestination)
                    dir = "-->   ";
                else
                {
                    dir = "<--   ";
                    string addr = saddr;
                    saddr = daddr;
                    daddr = addr;
                }
            }
            sb.Append(string.Format("{0,5}  {1,5}  {2,10:0.000000}  {3,-21}  {4}{5,-21} {6,-7}", packet.gGroupNumber, packet.PacketNumber, packet.RelativeTime.TotalSeconds, saddr, dir, daddr, packet.ProtocolCode));
            string align = "";
            if (tcp != null)
            {
                string next_seq;
                if (tcp.DataLength > 0)
                    next_seq = "0x" + (tcp.Sequence + tcp.DataLength).zToHex();
                else if (tcp.Synchronize)
                    next_seq = "0x" + (tcp.Sequence + 1).zToHex();
                else
                    next_seq = "          ";
                string dataLength;
                if (tcp.DataLength > 0)
                    dataLength = "0x" + tcp.DataLength.zToHex();
                else
                    dataLength = "      ";
                string ack_seq;
                if (tcp.AckSequence != 0)
                    ack_seq = "0x" + tcp.AckSequence.zToHex();
                else
                    ack_seq = "          ";
                string urg_ptr = null;
                if (tcp.UrgentPointer != 0)
                    urg_ptr = " 0x" + tcp.UrgentPointer.zToHex();
                else
                    align += "       ";
                //TCPFlags(tcp)
                sb.Append(string.Format("  {0,-20} {1} 0x{2} {3} {4} 0x{5}{6}", tcp.GetFlagsString(), dataLength, tcp.Sequence.zToHex(), next_seq, ack_seq, tcp.Window.zToHex(), urg_ptr));
            }
            else
                align += "                                                                                    ";
            if (printData && tcp != null && tcp.DataLength > 0)
            {
                sb.Append(align + "    ");
                byte[] data = tcp.DataBytes;
                int i = 0;
                //int maxDataChar = 100;
                int maxDataChar = 50;
                foreach (byte b in data)
                {
                    if (++i > maxDataChar) break;
                    if (b >= 32 && b <= 126)
                        sb.Append((char)b);
                    else
                        sb.Append('.');
                }
            }
            return sb.ToString();
        }

        public static string GetPacketString2(EthernetPacket packet, TCP_Direction? direction, bool printData)
        {
            IP ip = packet.IP;
            TCP tcp = packet.TCP;
            string saddr = null;
            string daddr = null;
            string mf = null;
            string offset = null;
            string id = null;
            if (ip != null)
            {
                saddr = ip.SourceAddress.ToString();
                daddr = ip.DestinationAddress.ToString();
                if (tcp != null)
                {
                    saddr += ":" + tcp.SourcePort.ToString();
                    daddr += ":" + tcp.DestinationPort.ToString();
                }
                mf = ip.MoreFragment.ToString();
                offset = ip.FragmentOffset.zToHex();
                id = ip.ID.zToHex();
            }
            StringBuilder sb = new StringBuilder();
            //direction
            string dir = null;
            if (direction != null)
            {
                if ((TCP_Direction)direction == TCP_Direction.SourceToDestination)
                    dir = "-->   ";
                else
                {
                    dir = "<--   ";
                    string addr = saddr;
                    saddr = daddr;
                    daddr = addr;
                }
            }
            sb.Append(string.Format("{0,5}  {1,5}  {2,10:0.000000}  {3,-21}  {4}{5,-21} {6,-7}", packet.gGroupNumber, packet.PacketNumber, packet.RelativeTime.TotalSeconds, saddr, dir, daddr, packet.ProtocolCode));
            string align = "";
            if (tcp != null)
            {
                //string next_seq;
                //if (tcp.DataLength > 0)
                //    next_seq = "0x" + (tcp.Sequence + tcp.DataLength).zToHex();
                //else if (tcp.Synchronize)
                //    next_seq = "0x" + (tcp.Sequence + 1).zToHex();
                //else
                //    next_seq = "          ";
                string dataLength;
                if (tcp.DataLength > 0)
                    dataLength = "0x" + tcp.DataLength.zToHex();
                else
                    dataLength = "      ";
                //string ack_seq;
                //if (tcp.AckSequence != 0)
                //    ack_seq = "0x" + tcp.AckSequence.zToHex();
                //else
                //    ack_seq = "          ";
                //string urg_ptr = null;
                //if (tcp.UrgentPointer != 0)
                //    urg_ptr = " 0x" + tcp.UrgentPointer.zToHex();
                //else
                //    align += "       ";
                //sb.Append(string.Format("  {0,-20} {1} 0x{2} {3} {4} 0x{5}{6}", tcp.GetFlagsString(), dataLength, tcp.Sequence.zToHex(), next_seq, ack_seq, tcp.Window.zToHex(), urg_ptr));
                sb.Append(string.Format("  {0,-20} {1}", tcp.GetFlagsString(), dataLength));
            }
            else
                align += "                                                                                    ";
            if (printData && tcp != null && tcp.DataLength > 0)
            {
                sb.Append(align + "    ");
                byte[] data = tcp.DataBytes;
                int i = 0;
                //int maxDataChar = 100;
                int maxDataChar = 50;
                foreach (byte b in data)
                {
                    if (++i > maxDataChar) break;
                    if (b >= 32 && b <= 126)
                        sb.Append((char)b);
                    else
                        sb.Append('.');
                }
            }
            return sb.ToString();
        }

        public static void PrintPacket_Sniffer(PcapDevice sender, EthernetPacket packet)
        {
            if (packet.State == PcapState.Eof)
            {
                gReceivePacketEof = true;
                _tr.WriteLine("Packet Eof ****************************");
                return;
            }
            if (packet == null)
            {
                Print("Packet null ****************************");
                return;
            }
            if (packet.State != PcapState.Success)
            {
                Print("Packet {0} ****************************", packet.State.ToString());
                return;
            }
            Print("Packet length: {0}", packet.PacketHeader.len);
            Print("Received at ..... (null)");
            EthernetHeader eth = packet.EthernetHeader;
            Print("-------------------[ETH HEADER]-----------");
            Print("| {0}:{1}:{2}:{3}:{4}:{5} -> {6}:{7}:{8}:{9}:{10}:{11} |",
                eth.SourceMACAddress[0].zToHex(), eth.SourceMACAddress[1].zToHex(), eth.SourceMACAddress[2].zToHex(), eth.SourceMACAddress[3].zToHex(), eth.SourceMACAddress[4].zToHex(), eth.SourceMACAddress[5].zToHex(),
                eth.DestinationMACAddress[0].zToHex(), eth.DestinationMACAddress[1].zToHex(), eth.DestinationMACAddress[2].zToHex(), eth.DestinationMACAddress[3].zToHex(), eth.DestinationMACAddress[4].zToHex(), eth.DestinationMACAddress[5].zToHex());
            Print("------------------------------------------");
            Print("Protocol: 0x{0}", packet.EtherTypeValue.zToHex());
            switch (packet.EtherTypeValue)
            {
                case 0x0800: Print("Internet Protocol, Version 4 (IPv4)");
                    PrintIPHeader1(packet);
                    break;
                case 0x0806: Print("Address Resolution Protocol (ARP)");
                    break;
                case 0x8035: Print("Reverse Address Resolution Protocol (RARP)");
                    break;
                case 0x809B: Print("AppleTalk (Ethertalk)");
                    break;
                case 0x80F3: Print("AppleTalk Address Resolution Protocol (AARP)");
                    break;
                case 0x8100: Print("IEEE 802.1Q-tagged frame");
                    break;
                case 0x8137: Print("Novell IPX (alt)");
                    break;
                case 0x8138: Print("Novell");
                    break;
                case 0x86DD: Print("Internet Protocol, Version 6 (IPv6)");
                    PrintIPHeader1(packet);
                    break;
                case 0x8847: Print("MPLS unicast");
                    break;
                case 0x8848: Print("MPLS multicast");
                    break;
                case 0x8863: Print("PPPoE Discovery Stage");
                    break;
                case 0x8864: Print("PPPoE Session Stage");
                    break;
                case 0x88A2: Print("ATA over Ethernet");
                    break;
                default: Print("Unknow packet");
                    break;
            }

            Print();
            Print();
        }

        public static void PrintIPHeader1(EthernetPacket packet)
        {
            IP ipHeader = packet.IP;
            if (ipHeader == null)
            {
                Print("IP null ****************************");
                return;
            }
            Print("[ IP : {0} -> {1} ]", ipHeader.SourceAddress.ToString(), ipHeader.DestinationAddress.ToString());
            Print("--------------------[IP HEADER]----------------------");
            Print("| IP header length    : {0} * 32 bits = {1} bytes      |", ipHeader.WordHeaderLength, ipHeader.ByteHeaderLength);
            Print("| IP version      : {0:00000} | Type of service : {1:00000} |", ipHeader.Version, ipHeader.DSCP);
            Print("| Total length    : {0:00000} | Identification  : {1:00000} |", ipHeader.TotalLength, ipHeader.ID);
            Print("| Time to live    : {0:00000} | Protocol        : {1:00000} |", ipHeader.TimeToLive, ipHeader.ProtocolValue);
            Print("| Checksum            : {0:00000}                       |", (ushort)IPAddress.NetworkToHostOrder((short)ipHeader.Checksum));
            Print("-----------------------------------------------------");


            switch (ipHeader.ProtocolValue)
            {
                case 1:
                    Print("Internet Control Message Protocol (ICMP)");
                    break;
                case 6:
                    Print("Transmission Control Protocol (TCP)");
                    PrintTCPHeader1(packet.TCP);
                    PrintData(packet.TCP.DataBytes, 16, null, false, true);
                    break;
                case 17:
                    Print("User Datagram Protocol (UDP)");
                    break;
                default:
                    Print("Unknow");
                    break;
            }
        }

        public static void PrintTCPHeader1(TCP tcp)
        {
            Print("[ Port : {0:00000} -> {1:00000} ]", tcp.SourcePort, tcp.DestinationPort);
            Print("--------------------[TCP HEADER]---------------------");
            Print("| TCP Header length   : {0} * 32 bits = {1} bytes      |", tcp.DataOffset, tcp.DataOffset * 4);
            Print("| URG (URGENT)      = {0} | ACK (ACKNOWLEDGE) = {1}     |", tcp.Urgent.zToString01(), tcp.Ack.zToString01());
            Print("| PSH (PUSH)        = {0} | RST (RESET)       = {1}     |", tcp.Push.zToString01(), tcp.Reset.zToString01());
            Print("| SYN (SYNCHRONIZE) = {0} | FIN (FINISH)      = {1}     |", tcp.Synchronize.zToString01(), tcp.Finish.zToString01());
            Print("-----------------------------------------------------");
        }

        public static void PrintData(Byte[] values, int nbLineValues, string beginLine, bool viewValuesIndex, bool printChar)
        {
            if (values == null || values.Length == 0)
            {
                Print();
                return;
            }

            StringBuilder sb;

            if (viewValuesIndex)
            {
                sb = new StringBuilder();
                if (beginLine != null)
                {
                    for (int i = 0; i < beginLine.Length; i++) sb.Append(' ');
                }
                for (int i = 0; i < nbLineValues; i++) sb.AppendFormat("{0:00} ", i);
                Print(sb.ToString());
            }

            sb = new StringBuilder();
            if (beginLine != null) sb.Append(beginLine);
            int size = values.Length;
            int j;
            int k = 0;

            for (j = 0; j < size; j++)
            {
                if (k == nbLineValues)
                {
                    if (printChar)
                    {
                        j -= nbLineValues;
                        sb.Append(" ");
                        for (k = 0; k < nbLineValues; k++, j++)
                        {
                            Byte value = values[j];
                            if (value > 31 && value < 127)
                                sb.Append((char)value);
                            else
                                sb.Append(".");
                        }
                    }
                    j--;
                    k = 0;
                    Print(sb.ToString());
                    sb = new StringBuilder();
                    if (beginLine != null) sb.Append(beginLine);
                }
                else
                {
                    sb.AppendFormat("{0} ", values[j].zToHex());
                    k++;
                }
            }
            if (printChar)
            {

                j -= k;
                while (k != 0)
                {
                    Byte value = values[j];
                    if (value > 31 && value < 127)
                        sb.Append((char)value);
                    else
                        sb.Append(".");
                    k--;
                    j++;
                }
            }
            Print(sb.ToString());
        }
    }

    public static partial class GlobalExtension
    {
        public static string zToString01(this bool v)
        {
            if (v)
                return "1";
            else
                return "0";
        }
    }

}
