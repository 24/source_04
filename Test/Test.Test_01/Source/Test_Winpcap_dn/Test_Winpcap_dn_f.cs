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
using PcapDotNet.Core;
using PcapDotNet.Core.Extensions;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Transport;
using Pib.Pcap;

// Ethernet frame : http://en.wikipedia.org/wiki/Ethernet_frame
// Ethernet II framing : http://en.wikipedia.org/wiki/Ethernet_II_framing
// EtherType : http://en.wikipedia.org/wiki/EtherType
// IPv4 : http://en.wikipedia.org/wiki/IPv4
// IPv6 : http://en.wikipedia.org/wiki/IPv6
// IPv6 packet : http://en.wikipedia.org/wiki/IPv6_packet
// ip (struct) : http://en.wikipedia.org/wiki/Ip_(struct)
// List of IP protocol numbers : http://en.wikipedia.org/wiki/List_of_IP_protocol_numbers
// Transmission Control Protocol : http://en.wikipedia.org/wiki/Transmission_Control_Protocol
// IP Message Reassembly Process : http://www.tcpipguide.com/free/t_IPMessageReassemblyProcess.htm
// How to reassemble split TCP Packets : http://www.wireshark.org/docs/wsdg_html_chunked/ChDissectReassemble.html
//   tcp_dissect_pdus() in epan/dissectors/packet-tcp.h
// General description of the TCP/IP protocols : http://www.linuxjunkies.org/network/tcpip/intro2.html#1
//   Each datagram has a sequence number. TCP doesn't number the datagrams, but the octets. So if there are 500 octets of data in each datagram,
//   the first datagram might be numbered 0, the second 500, the next 1000, the next 1500, etc
// Networking: TCP reassembly : http://networking.itags.org/networking-tech/242494/
// SECURITY ASSESSMENT OF THE TRANSMISSION CONTROL PROTOCOL (TCP) : http://www.cpni.gov.uk/Docs/tn-03-09-security-assessment-TCP.pdf
//   Sequence number :
//     This field contains the sequence number of the first data octet in this segment. If the SYN flag
//     is set, the sequence number is the Initial Sequence Number (ISN) of the connection, and the
//     first data octet has the sequence number ISN+1.
// A TCP Tutorial : http://www.ssfnet.org/Exchange/tcp/tcpTutorialNotes.html
// TCP (Transmission Control Protocol) : http://www.linktionary.com/t/tcp.html
// tcp-reassembly-project : http://code.google.com/p/tcp-reassembly-project/
//   # Non-members may check out a read-only working copy anonymously over HTTP.
//   svn checkout http://tcp-reassembly-project.googlecode.com/svn/trunk/ tcp-reassembly-project-read-only
// Wireplay : http://code.google.com/p/wireplay/
//   A minimalist approach to replay pcap dumped TCP sessions with modification as required.
//   # Non-members may check out a read-only working copy anonymously over HTTP.
//   svn checkout http://wireplay.googlecode.com/svn/trunk/ wireplay-read-only
// Libnids : http://libnids.sourceforge.net/
//   Libnids is an implementation of an E-component of Network Intrusion Detection System. It emulates the IP stack of Linux 2.0.x.
//   Libnids offers IP defragmentation, TCP stream assembly and TCP port scan detection.
// Libnids Win32 : http://www.datanerds.net/~mike/libnids.html
// TCP Session Reconstruction Tool http://www.codeproject.com/Articles/20501/TCP-Session-Reconstruction-Tool
//


namespace Test_Winpcap_dn
{
    static partial class w
    {
        private static ITrace _tr = Trace.CurrentTrace;
        private static RunSource _rs = RunSource.CurrentRunSource;
        private static string _dataDir = null;
        private static PacketCommunicator __communicator = null;

        public static void Init()
        {
            XmlConfig.CurrentConfig = new XmlConfig("Test_Winpcap_dn");
            //string log = XmlConfig.CurrentConfig.Get("Log").zRootPath(zapp.GetAppDirectory());
            //if (log != null)
            //    _tr.SetLogFile(log, LogOptions.IndexedFile);
            Trace.CurrentTrace.SetWriter(XmlConfig.CurrentConfig.Get("Log"), XmlConfig.CurrentConfig.Get("Log/@option").zTextDeserialize(FileOption.None));
            _dataDir = XmlConfig.CurrentConfig.Get("DataDir");
        }

        public static void End()
        {
            _rs.OnAbortExecution = null;
        }

        public static string GetPath(string file)
        {
            if (file == null)
                return null;
            return zPath.Combine(_dataDir, file);
        }

        public static void Test_01()
        {
            _tr.WriteLine("Test_01");
        }

        public static void Test_ForceAbort_01()
        {
            _tr.WriteLine("Test_Abort_01");
            _tr.WriteLine("execute endless loop");
            int i = 0;
            while (true)
            {
                _tr.Write(".");
                if (i++ == 100)
                {
                    i = 0;
                    _tr.WriteLine();
                }
                Thread.Sleep(100);
            }
        }

        public static void Test_Abort_01()
        {
            _tr.WriteLine("Test_Abort_01");
            _tr.WriteLine("execute endless loop");
            int i = 0;
            while (true)
            {
                _tr.Write(".");
                if (i++ == 100)
                {
                    i = 0;
                    _tr.WriteLine();
                }
                Thread.Sleep(100);
                if (_rs.IsExecutionAborted())
                    break;
            }
        }

        public static void Test_GetDevicesList_01()
        {
            // from project ObtainingTheDeviceList and ObtainingAdvancedInformationAboutInstalledDevices
            _tr.WriteLine("Test_GetDevicesList_01");
            IList<LivePacketDevice> allDevices = LivePacketDevice.AllLocalMachine;
            if (allDevices.Count == 0)
            {
                _tr.WriteLine("No interfaces found! Make sure WinPcap is installed.");
                return;
            }

            // Print the list
            //for (int i = 0; i != allDevices.Count; ++i)
            int i = 1;
            foreach (LivePacketDevice device in allDevices)
            {
                //LivePacketDevice device = allDevices[i];
                //_tr.Write(i + ". " + device.Name);
                //if (device.Description != null)
                //    _tr.WriteLine(" (" + device.Description + ")");
                //else
                //    _tr.WriteLine(" (No description available)");
                _tr.WriteLine("{0} \"{1}\"  (\"{2}\")", i++, device.Name, device.Description);
                DevicePrint(device);
            }
        }

        private static void DevicePrint(IPacketDevice device)
        {
            // Name
            _tr.WriteLine(device.Name);

            // Description
            if (device.Description != null)
                _tr.WriteLine("\tDescription: " + device.Description);

            // Loopback Address
            _tr.WriteLine("\tLoopback: " + (((device.Attributes & DeviceAttributes.Loopback) == DeviceAttributes.Loopback) ? "yes" : "no"));

            // IP addresses
            foreach (DeviceAddress address in device.Addresses)
            {
                _tr.WriteLine("\tAddress Family: " + address.Address.Family);

                if (address.Address != null)
                    _tr.WriteLine(("\tAddress: " + address.Address));
                if (address.Netmask != null)
                    _tr.WriteLine(("\tNetmask: " + address.Netmask));
                if (address.Broadcast != null)
                    _tr.WriteLine(("\tBroadcast Address: " + address.Broadcast));
                if (address.Destination != null)
                    _tr.WriteLine(("\tDestination Address: " + address.Destination));
            }
            _tr.WriteLine();
        }

        public static PacketDevice SelectDevice(string deviceName = null)
        {
            if (deviceName == null)
                deviceName = @"rpcap://\Device\NPF_{BF8A52CB-F023-4F24-AA7E-958A8D1F3069}";
            Trace.WriteLine("select device \"{0}\"", deviceName);
            PacketDevice device = null;
            if (deviceName.StartsWith("rpcap://"))
                device = (from d in LivePacketDevice.AllLocalMachine where d.Name == deviceName select d).FirstOrDefault();
            else
                device = new OfflinePacketDevice(GetPath(deviceName));
            if (device == null)
                Trace.WriteLine("device not found");
            return device;
        }

        private static void OnAbortExecution()
        {
            if (__communicator != null)
                __communicator.Break();
        }

        public static ReceivePackets CreateReceivePackets(string deviceName, string dumpFile = null, string filter = null)
        {
            if (deviceName == null)
                deviceName = @"rpcap://\Device\NPF_{BF8A52CB-F023-4F24-AA7E-958A8D1F3069}";
            Trace.WriteLine("select device \"{0}\"", deviceName);
            if (!deviceName.StartsWith("rpcap://"))
                deviceName = GetPath(deviceName);
            if (dumpFile != null)
            {
                Trace.WriteLine("dump to file \"{0}\"", dumpFile);
                dumpFile = GetPath(dumpFile);
            }
            ReceivePackets receivePackets = ReceivePackets.CreateReceivePackets(deviceName, dumpFile, filter);
            _rs.OnAbortExecution = receivePackets.Break;
            return receivePackets;
        }

        public static void Test_CapturingThePackets_01()
        {
            // from project OpeningAnAdapterAndCapturingThePackets
            _tr.WriteLine("Test_CapturingThePackets_01");
            //rpcap://\Device\NPF_{BF8A52CB-F023-4F24-AA7E-958A8D1F3069}
            //IList<LivePacketDevice> allDevices = LivePacketDevice.AllLocalMachine;
            //string deviceName = @"rpcap://\Device\NPF_{BF8A52CB-F023-4F24-AA7E-958A8D1F3069}";
            //_tr.WriteLine("select device \"{0}\"", deviceName);
            //PacketDevice device = (from d in LivePacketDevice.AllLocalMachine where d.Name == deviceName select d).FirstOrDefault();
            //if (device == null)
            //{
            //    _tr.WriteLine("device not found");
            //    return;
            //}

            PacketDevice device = SelectDevice();
            if (device == null)
                return;

            __communicator = null;
            //_rs.OnAbortExecution += new OnAbortEvent(OnAbortExecution);
            _rs.OnAbortExecution = OnAbortExecution;
            try
            {
                // Open the device : device.Open()
                //   snapshotLength = 65536, portion of the packet to capture 65536 guarantees that the whole packet will be captured on all the link layers
                //   attributes = PacketDeviceOpenAttributes.Promiscuous, promiscuous mode
                //   readTimeout = 1000
                using (__communicator = device.Open(65536, PacketDeviceOpenAttributes.Promiscuous, 1000))
                {
                    _tr.WriteLine("Listening on " + device.Description + "...");
                    // start the capture
                    __communicator.ReceivePackets(0, CapturingPacketHandler);
                }
            }
            catch (Exception ex)
            {
                _tr.WriteLine(ex.Message);
            }
            finally
            {
                //_rs.OnAbortExecution -= new OnAbortEvent(OnAbortExecution);
            }
        }

        // Callback function invoked by Pcap.Net for every incoming packet
        private static void CapturingPacketHandler(Packet packet)
        {
            _tr.WriteLine(packet.Timestamp.ToString("yyyy-MM-dd hh:mm:ss.fff") + " length:" + packet.Length);
            //if (_wr.IsExecutionAborted())
            //    __communicator.Break();
        }

        public static void Test_CapturingThePacketsWithoutTheCallback_01()
        {
            // from project CapturingThePacketsWithoutTheCallback
            _tr.WriteLine("Test_CapturingThePacketsWithoutTheCallback_01");
            PacketDevice device = SelectDevice();
            if (device == null)
                return;
            try
            {
                // Open the device : device.Open()
                //   snapshotLength = 65536, portion of the packet to capture 65536 guarantees that the whole packet will be captured on all the link layers
                //   attributes = PacketDeviceOpenAttributes.Promiscuous, promiscuous mode
                //   readTimeout = 1000
                using (PacketCommunicator communicator = device.Open(65536, PacketDeviceOpenAttributes.Promiscuous, 1000))
                {
                    _tr.WriteLine("Listening on " + device.Description + "...");

                    // Retrieve the packets
                    Packet packet;
                    do
                    {
                        PacketCommunicatorReceiveResult result = communicator.ReceivePacket(out packet);
                        switch (result)
                        {
                            case PacketCommunicatorReceiveResult.Timeout:
                                // Timeout elapsed
                                continue;
                            case PacketCommunicatorReceiveResult.Ok:
                                _tr.WriteLine(packet.Timestamp.ToString("yyyy-MM-dd hh:mm:ss.fff") + " length:" + packet.Length);
                                break;
                            default:
                                throw new InvalidOperationException("The result " + result + " shoudl never be reached here");
                        }
                        if (_rs.IsExecutionAborted())
                            break;
                    } while (true);

                }
            }
            catch (Exception ex)
            {
                _tr.WriteLine(ex.Message);
            }
        }

        public static void Test_DumpPacketsToFile_01()
        {
            // from project CaptureInvalidPackets
            _tr.WriteLine("Test_DumpPacketsToFile_01");

            string dumpFile = @"dump\dump.pcap";
            dumpFile = GetPath(dumpFile);
            _tr.WriteLine("dump to file \"{0}\"", dumpFile);

            PacketDevice device = SelectDevice();
            if (device == null)
                return;
            try
            {
                // Open the device : device.Open()
                //   snapshotLength = 65536, portion of the packet to capture 65536 guarantees that the whole packet will be captured on all the link layers
                //   attributes = PacketDeviceOpenAttributes.Promiscuous, promiscuous mode
                //   readTimeout = 1000
                using (PacketCommunicator communicator = device.Open(65536, PacketDeviceOpenAttributes.Promiscuous, 1000))
                {
                    _tr.WriteLine("Listening on " + device.Description + "...");

                    if (communicator.DataLink.Kind != DataLinkKind.Ethernet)
                    {
                        _tr.WriteLine("This program works only on Ethernet networks.");
                        return;
                    }

                    // start the capture
                    var query = from packet in communicator.ReceivePackets() select packet; // where !packet.IsValid

                    using (PacketDumpFile dump = communicator.OpenDump(dumpFile))
                    {
                        foreach (Packet packet in query)
                        {
                            if (packet.Length <= 60 &&
                                packet.Ethernet.EtherType == EthernetType.IpV4 &&
                                packet.Ethernet.IpV4.Protocol == IpV4Protocol.Tcp &&
                                packet.Ethernet.IpV4.Tcp.ControlBits == (TcpControlBits.Synchronize | TcpControlBits.Acknowledgment))
                            {
                                _tr.WriteLine("Captured Packet " + packet.Timestamp);
                            }

                            _tr.WriteLine("dump packet " + packet.Timestamp);
                            dump.Dump(packet);
                            if (_rs.IsExecutionAborted())
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _tr.WriteLine(ex.Message);
            }
        }

        public static void Test_DumpPacketsToFile_02()
        {
            // from project SavingPacketsToADumpFile
            _tr.WriteLine("Test_DumpPacketsToFile_02");

            string dumpFile = @"dump\dump.pcap";
            dumpFile = GetPath(dumpFile);
            _tr.WriteLine("dump to file \"{0}\"", dumpFile);

            PacketDevice device = SelectDevice();
            if (device == null)
                return;
            __communicator = null;
            //_rs.OnAbortExecution += new OnAbortEvent(OnAbortExecution);
            _rs.OnAbortExecution = OnAbortExecution;
            try
            {
                // Open the device : device.Open()
                //   snapshotLength = 65536, portion of the packet to capture 65536 guarantees that the whole packet will be captured on all the link layers
                //   attributes = PacketDeviceOpenAttributes.Promiscuous, promiscuous mode
                //   readTimeout = 1000
                using (__communicator = device.Open(65536, PacketDeviceOpenAttributes.Promiscuous, 1000))
                {

                    //if (communicator.DataLink.Kind != DataLinkKind.Ethernet)
                    //{
                    //    _tr.WriteLine("This program works only on Ethernet networks.");
                    //    return;
                    //}

                    // start the capture
                    //var query = from packet in communicator.ReceivePackets() select packet; // where !packet.IsValid

                    using (PacketDumpFile dump = __communicator.OpenDump(dumpFile))
                    {
                        _tr.WriteLine("Listening on " + device.Description + "...");

                        // start the capture
                        __communicator.ReceivePackets(0, dump.Dump);
                    }
                }
            }
            catch (Exception ex)
            {
                _tr.WriteLine(ex.Message);
            }
            finally
            {
                //_rs.OnAbortExecution -= new OnAbortEvent(OnAbortExecution);
            }
        }

        public static void Test_ReadPacketsFromDumpFile_01()
        {
            // from project ReadingPacketsFromADumpFile
            _tr.WriteLine("Test_ReadPacketsFromDumpFile_01");

            string dumpFile = @"dump\dump.pcap";
            dumpFile = GetPath(dumpFile);
            _tr.WriteLine("read packets from dump file \"{0}\"", dumpFile);

            // Create the offline device
            OfflinePacketDevice device = new OfflinePacketDevice(dumpFile);

            __communicator = null;
            //_rs.OnAbortExecution += new OnAbortEvent(OnAbortExecution);
            _rs.OnAbortExecution = OnAbortExecution;
            try
            {
                // Open the capture file
                using (__communicator = device.Open(65536, PacketDeviceOpenAttributes.Promiscuous, 1000))
                {
                    // Read and dispatch packets until EOF is reached
                    __communicator.ReceivePackets(0, ReadPacketsFromDumpFile);
                }
            }
            finally
            {
                //_rs.OnAbortExecution -= new OnAbortEvent(OnAbortExecution);
            }
        }

        private static void ReadPacketsFromDumpFile(Packet packet)
        {
            // print packet timestamp and packet length
            _tr.WriteLine(packet.Timestamp.ToString("yyyy-MM-dd hh:mm:ss.fff") + " length:" + packet.Length);

            // Print the packet
            const int LineLength = 64;
            for (int i = 0; i != packet.Length; ++i)
            {
                _tr.Write((packet[i]).ToString("X2"));
                if ((i + 1) % LineLength == 0)
                    _tr.WriteLine();
            }
            _tr.WriteLine();
            _tr.WriteLine();
        }

        public static void Test_NetworkStatistics_01()
        {
            // from project GatheringStatisticsOnTheNetworkTraffic
            _tr.WriteLine("Test_NetworkStatistics_01");

            PacketDevice device = SelectDevice();
            if (device == null)
                return;
            __communicator = null;
            //_rs.OnAbortExecution += new OnAbortEvent(OnAbortExecution);
            _rs.OnAbortExecution = OnAbortExecution;
            try
            {
                // Open the device : device.Open()
                //   snapshotLength = 100 ???
                //   attributes = PacketDeviceOpenAttributes.Promiscuous, promiscuous mode
                //   readTimeout = 1000
                using (__communicator = device.Open(100, PacketDeviceOpenAttributes.Promiscuous, 1000))
                {
                    _tr.WriteLine("Listening on " + device.Description + "...");

                    // Compile and set the filter
                    __communicator.SetFilter("tcp");

                    // Put the interface in statstics mode
                    __communicator.Mode = PacketCommunicatorMode.Statistics;

                    _tr.WriteLine("TCP traffic summary:");

                    // Start the main loop
                    __communicator.ReceiveStatistics(0, StatisticsHandler);
                }
            }
            catch (Exception ex)
            {
                _tr.WriteLine(ex.Message);
            }
            finally
            {
                //_rs.OnAbortExecution -= new OnAbortEvent(OnAbortExecution);
            }
        }

        private static DateTime _lastTimestamp;
        private static void StatisticsHandler(PacketSampleStatistics statistics)
        {
            // Current sample time
            DateTime currentTimestamp = statistics.Timestamp;

            // Previous sample time
            DateTime previousTimestamp = _lastTimestamp;

            // Set _lastTimestamp for the next iteration
            _lastTimestamp = currentTimestamp;

            // If there wasn't a previous sample than skip this iteration (it's the first iteration)
            if (previousTimestamp == DateTime.MinValue)
                return;

            // Calculate the delay from the last sample
            double delayInSeconds = (currentTimestamp - previousTimestamp).TotalSeconds;

            // Calculate bits per second
            double bitsPerSecond = statistics.AcceptedBytes * 8 / delayInSeconds;

            // Calculate packets per second
            double packetsPerSecond = statistics.AcceptedPackets / delayInSeconds;

            // Print timestamp and samples
            _tr.WriteLine(statistics.Timestamp + " BPS: " + bitsPerSecond + " PPS: " + packetsPerSecond);

            //if (_wr.IsExecutionAborted())
            //    __communicator.Break();
        }

        public static void Test_InterpretingThePackets_01()
        {
            // from project InterpretingThePackets
            _tr.WriteLine("Test_InterpretingThePackets_01");
            PacketDevice device = SelectDevice();
            if (device == null)
                return;
            __communicator = null;
            //_rs.OnAbortExecution += new OnAbortEvent(OnAbortExecution);
            _rs.OnAbortExecution = OnAbortExecution;
            try
            {
                // Open the device : device.Open()
                //   snapshotLength = 65536, portion of the packet to capture 65536 guarantees that the whole packet will be captured on all the link layers
                //   attributes = PacketDeviceOpenAttributes.Promiscuous, promiscuous mode
                //   readTimeout = 1000
                using (__communicator = device.Open(65536, PacketDeviceOpenAttributes.Promiscuous, 1000))
                {
                    // Check the link layer. We support only Ethernet for simplicity.
                    if (__communicator.DataLink.Kind != DataLinkKind.Ethernet)
                    {
                        _tr.WriteLine("This program works only on Ethernet networks.");
                        return;
                    }

                    // Compile the filter
                    using (BerkeleyPacketFilter filter = __communicator.CreateFilter("ip and udp"))
                    {
                        // Set the filter
                        __communicator.SetFilter(filter);
                    }

                    _tr.WriteLine("Listening on " + device.Description + "...");

                    // start the capture
                    __communicator.ReceivePackets(0, InterpretingPacketHandler);
                }
            }
            catch (Exception ex)
            {
                _tr.WriteLine(ex.Message);
            }
            finally
            {
                //_rs.OnAbortExecution -= new OnAbortEvent(OnAbortExecution);
            }
        }

        // Callback function invoked by libpcap for every incoming packet
        private static void InterpretingPacketHandler(Packet packet)
        {
            // print timestamp and length of the packet
            _tr.WriteLine(packet.Timestamp.ToString("yyyy-MM-dd hh:mm:ss.fff") + " length:" + packet.Length);

            IpV4Datagram ip = packet.Ethernet.IpV4;
            UdpDatagram udp = ip.Udp;

            // print ip addresses and udp ports
            _tr.WriteLine(ip.Source + ":" + udp.SourcePort + " -> " + ip.Destination + ":" + udp.DestinationPort);

            //if (_wr.IsExecutionAborted())
            //    __communicator.Break();
        }
    }
}
