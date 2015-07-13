toto
Winpcap_Capture_TCP_Analyze       (@"dump\dump01.pcap", @"dump\dump01.pcap.TCP_Analyze_PrintPacket1.txt", "PrintPacket1");


Winpcap_Dump(@"\Device\NPF_{BF8A52CB-F023-4F24-AA7E-958A8D1F3069}", @"dump\dump.pcapng", null, "PrintPacket2");
Winpcap_Capture(@"dump\dump.pcapng",           null, "PrintPacket1");
Winpcap_Capture(@"dump\dump.pcap",             null, "PrintPacket1");
Winpcap_Capture(@"dump\dump.pcap",             null, "PrintPacket2");
Winpcap_Capture_TCP_Analyze       (@"dump\dump.pcapng", @"dump\dump.pcapng.08.TCP_Analyze_PrintPacket1.txt", "PrintPacket1");
Winpcap_Capture_TCP_Analyze       (@"c:\Data\Http\Pcap\dump_01\dump_01.pcapng", @"c:\Data\Http\Pcap\dump_01.pcapng.08.TCP_Analyze_PrintPacket1.txt", "PrintPacket1");
Winpcap_Capture(@"\Device\NPF_{BF8A52CB-F023-4F24-AA7E-958A8D1F3069}", null, "PrintPacket1");
Winpcap_Capture(@"\Device\NPF_{BF8A52CB-F023-4F24-AA7E-958A8D1F3069}", null, "PrintPacket2");
Winpcap_Capture(@"dump\dump_01\dump_01.pcapng",           null, "PrintPacket1");
Winpcap_Capture(@"c:\Data\Http\Pcap\dump_01\dump_01.pcapng",           null, "PrintPacket2");
Winpcap_Capture(@"c:\Data\Http\Pcap\dump_01\dump_01.pcapng", @"c:\Data\Http\Pcap\dump_01.pcapng.02.PrintPacket2.txt",             "PrintPacket2");
Winpcap_Capture(@"c:\Data\Http\Pcap\dump_01\dump_01.pcapng",           null, "PrintPacket_Detailled");


Test_Winpcap_FindAllDevs_01();
Winpcap_Capture                   (@"c:\Data\Http\Pcap\dump_01\dump_01.pcapng", @"c:\Data\Http\Pcap\dump_01.pcapng.01.sniffer.txt",                  "PrintPacket_Sniffer");
Winpcap_Capture                   (@"c:\Data\Http\Pcap\dump_01\dump_01.pcapng", @"c:\Data\Http\Pcap\dump_01.pcapng.02.PrintPacket1.txt",             "PrintPacket1");
Winpcap_Capture_Sorted            (@"c:\Data\Http\Pcap\dump_01\dump_01.pcapng", @"c:\Data\Http\Pcap\dump_01.pcapng.03.SortedPrintPacket1.txt",       "PrintPacket1");
Winpcap_Capture                   (@"c:\Data\Http\Pcap\dump_01\dump_01.pcapng", @"c:\Data\Http\Pcap\dump_01.pcapng.04.ip.detailled.txt",             "PrintPacket_IPDetailled");
Winpcap_Capture                   (@"c:\Data\Http\Pcap\dump_01\dump_01.pcapng", @"c:\Data\Http\Pcap\dump_01.pcapng.05.tcp.detailled.txt",            "PrintPacket_TCPDetailled");
Winpcap_Capture                   (@"c:\Data\Http\Pcap\dump_01\dump_01.pcapng", @"c:\Data\Http\Pcap\dump_01.pcapng.06.detailled.txt",                "PrintPacket_Detailled");
Winpcap_Capture                   (@"c:\Data\Http\Pcap\dump_01\dump_01.pcapng", @"c:\Data\Http\Pcap\dump_01.pcapng.07.wireshark.txt",                "PrintPacket_Wireshark");
Winpcap_Capture_TCP_Analyze       (@"c:\Data\Http\Pcap\dump_01\dump_01.pcapng", @"c:\Data\Http\Pcap\dump_01.pcapng.08.TCP_Analyze_PrintPacket1.txt", "PrintPacket1");
Winpcap_Capture_To_Xml            (@"c:\Data\Http\Pcap\dump_01\dump_01.pcapng", @"c:\Data\Http\Pcap\dump_01.pcapng.09.ip.xml",                       "ExportXml_IP");
Winpcap_Capture_To_Xml            (@"c:\Data\Http\Pcap\dump_01\dump_01.pcapng", @"c:\Data\Http\Pcap\dump_01.pcapng.10.ip.detail.xml",                "ExportXml_IP_Detail");
Winpcap_Capture_To_Xml            (@"c:\Data\Http\Pcap\dump_01\dump_01.pcapng", @"c:\Data\Http\Pcap\dump_01.pcapng.11.ip.tcp.xml",                   "ExportXml_IP_TCP");
Winpcap_Capture_To_Xml            (@"c:\Data\Http\Pcap\dump_01\dump_01.pcapng", @"c:\Data\Http\Pcap\dump_01.pcapng.12.ip.tcp.detail.xml",            "ExportXml_IP_TCP_Detail");
Winpcap_Capture_TCP_Analyze_To_Xml(@"c:\Data\Http\Pcap\dump_01\dump_01.pcapng", @"c:\Data\Http\Pcap\dump_01.pcapng.13.stream.xml");

Test_Winpcap_Capture_02(@"c:\Data\Http\Pcap\dump.pcapng");
Test_Winpcap_Capture_02(@"\Device\NPF_{8C984C67-71B9-44BA-BB64-87E026241D6C}");

Test_Winpcap_Dump_01(@"c:\Data\Http\Pcap\dump.pcapng");
Test_Winpcap_CreateSourceString_01(pcap_cs.CaptureSourceType.LocalInterface, @"\Device\NPF_{8C984C67-71B9-44BA-BB64-87E026241D6C}");
Test_Winpcap_CreateSourceString_01(pcap_cs.CaptureSourceType.File, @"c:\_Data\_Http\Pcap\dump_01.pcap");
Test_Winpcap_CreateSourceString_02(wpcap.CaptureSourceType.LocalInterface, @"\Device\NPF_{8C984C67-71B9-44BA-BB64-87E026241D6C}");

Test_ReadStructList_01(@"c:\Logiciel\ToolsNetwork\Wireshark\Source\struct_list.txt");
Test_ReadStructList_02(@"c:\Logiciel\ToolsNetwork\Wireshark\Source\struct_list.txt");
Test_ReadStructList_03(@"c:\Logiciel\ToolsNetwork\Wireshark\Source\struct_list.txt");
Test_01();

TestBitField_03(0x1F, 0xFF, null);
TestBitField_03(0xFF, 0xFF, null);
TestBitField_03(0x00, 0x00, 0x07);
TestBitField_03(null, null, 0xFF);
TestBitField_04(0x1F, 0xFF, null);
TestBitField_04(0xFF, 0xFF, null);
TestBitField_04(0x00, 0x00, 0x07);
TestBitField_04(null, null, 0xFF);
TestBitField_05(0x1F, 0xFF, null);
TestBitField_05(0xFF, 0xFF, null);
TestBitField_05(0x00, 0x00, 0x07);
TestBitField_05(null, null, 0xFF);

Test_Wireshark_FilterIP_01(@"c:\_Data\_Http\Pcap\dump_01\dump_01.pcapng.wireshark_3.txt");
Test_PacketGroup();

_tr.WriteLine("toto");
