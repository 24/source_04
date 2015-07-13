RunSource.CurrentRunSource.Compile_Project(@"..\..\runsource\runsource_dll\irunsource_project.xml");
RunSource.CurrentRunSource.Compile_Project(@"..\..\runsource\runsource_dll\runsource_dll_project.xml");
RunSource.CurrentRunSource.Compile_Project(@"..\..\runsource\runsource_domain\runsourced32_project.xml");

_tr.WriteLine("toto");

AnalyzeTcpStream.Trace = true;
Test_AnalyzeTcpStream_01(@"dump\dump.pcap", @"dump");
Test_AnalyzeTcpStream_01(@"dump\dump_handeco_01\dump_handeco_01.pcap", @"dump\dump_handeco_01");
Test_SortedTcpStreamList_01(@"dump\dump01\dump01.pcap");

Test_AnalyzeTcpStreamTest_01(@"dump\dump01\dump01.pcap");
Test_AnalyzeTcpStreamTest_ReadPcap_01(@"dump\dump01\dump01.pcap");
Test_TcpRecon_TcpRecon_01(@"dump\dump01\dump01.pcap");
Test_TcpRecon_LibNids_01(@"dump\dump01\dump01.pcap");
Test_TcpRecon_PcapDotNet_01(@"dump\dump01\dump01.pcap");

// filter = "not tcp portrange 52000-53000 and not net 0.0.0.0 mask 255.0.0.0";
Test_DumpPacketsToFile_04(@"dump\dump.pcap", detail: false);
Test_DumpPacketsToFile_04(@"dump\dump.pcap", detail: false, filter: "net 62.210.154.101"); // www.unea.fr
Test_DumpPacketsToFile_04(@"dump\dump.pcap", detail: false, filter: "net 87.98.177.160");  // www.handeco.org
Test_AnalyzeTcpStream_01(@"dump\dump.pcap", @"dump");



Test_PrintPackets_01(@"dump\dump01.pcap", detail: false);
Test_PrintPackets_01(@"dump\dump01.pcap", detail: true);
Test_PrintPacketsDetail_02(@"dump\dump01.pcap");
Test_PrintPacketsDetail_01(@"dump\dump01.pcap");
Test_DumpPacketsToFile_03(@"dump\dump01.pcap", detail: false);
Test_DumpPacketsToFile_03(@"dump\dump02.pcap", detail: false);
Test_DumpPacketsToFile_03(@"dump\dump03.pcap", detail: false);
Test_DumpPacketsToFile_04(@"dump\dump03.pcap", detail: false);

Test_PrintPackets_01(@"dump\dump.pcap", detail: false);
Test_PrintPackets_01(@"dump\dump.pcap", detail: true);
Test_PrintPackets_01();


Test_GetDevicesList_01();
Test_CapturingThePackets_01();
Test_CapturingThePacketsWithoutTheCallback_01();
Test_DumpPacketsToFile_01();
Test_DumpPacketsToFile_02();
Test_ReadPacketsFromDumpFile_01();
Test_NetworkStatistics_01();
Test_InterpretingThePackets_01();

Test_ForceAbort_01();
Test_Abort_01();
Test_01();
_tr.WriteLine("toto");
Test_BinaryToStruct.w.Test_BinaryToStruct_01();
Test_ReadPcap_01(@"dump\dump01.pcap");
Test_ReadPcap_02(@"dump\dump01.pcap", detail: false);
Test_ReadPcap_02(@"dump\dump01.pcap", detail: true);
Test_PrintPackets_01(@"dump\dump01.pcap", detail: false);
Test_PrintPackets_01(@"dump\dump01.pcap", detail: true);
Test_TcpAnalyze_02(@"dump\dump01.pcap");
Test_ReadPcap_PrintPacketsDetail_01(@"dump\dump01.pcap");
_tr.WriteLine("{0}", 3 / 2);
Test_SendPacket_01();

byte add1 = (byte)(add & 0xFFFFFF00);
uint add = 0x01020304;
_tr.WriteLine("{0}", (byte)(add & 0x000000FF));
uint add = 0x01020304;
_tr.WriteLine("{0}", (byte)((add & 0x0000FF00) >> 8));
_tr.WriteLine("{0}", new PcapDotNet.Packets.IpV4.IpV4Address("1.2.3.4").ToValue().zToHex());
_tr.WriteLine(new PcapDotNet.Packets.IpV4.IpV4Address("1.2.3.4").zToFormatedString());
