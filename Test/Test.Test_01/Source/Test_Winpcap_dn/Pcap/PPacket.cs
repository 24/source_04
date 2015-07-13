using System;
using System.Collections.Generic;
using System.Text;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Transport;
using pb.Text;

// todo
//   TcpPacketType

namespace Pib.Pcap
{
    public class PPacket
    {
        private int _groupNumber = 0;
        private long _packetNumber;
        private TimeSpan _relativeTime;
        private Packet _packet = null;
        private EthernetType? _ethernetType = null;
        private string _ethernetTypeCode = null;
        private IpV4Datagram _ipv4 = null;
        private TcpDatagram _tcp = null;
        private IpV4Address? _source = null;
        private ushort? _sourcePort = null;
        private IpV4Address? _destination = null;
        private ushort? _destinationPort = null;
        private IpV4Protocol? _ipProtocol = null;
        private string _ipProtocolCode = null;
        private TcpConnection _tcpConnection = null;

        public PPacket(Packet packet, long packetNumber, TimeSpan relativeTime)
        {
            _packet = packet;
            _packetNumber = packetNumber;
            _relativeTime = relativeTime;
            EthernetDatagram ethernet = packet.Ethernet;
            if (ethernet != null)
            {
                _ethernetType = ethernet.EtherType;
                _ethernetTypeCode = GetEthernetTypeCode(ethernet.EtherType);
                //_ipv4 = packet.Ethernet.IpV4;
                //if (_ipv4 != null)
                if (ethernet.EtherType == PcapDotNet.Packets.Ethernet.EthernetType.IpV4)
                {
                    _ipv4 = packet.Ethernet.IpV4;
                    _source = _ipv4.Source;
                    _destination = _ipv4.Destination;
                    _ipProtocol = _ipv4.Protocol;
                    _ipProtocolCode = GetIPProtocolCode(_ipv4.Protocol);
                    //_tcp = _ipv4.Tcp;
                    //if (_tcp != null)
                    if (_ipv4.Protocol == IpV4Protocol.Tcp)
                    {
                        _tcp = _ipv4.Tcp;
                        _sourcePort = _tcp.SourcePort;
                        _destinationPort = _tcp.DestinationPort;
                    }
                }
            }
        }

        public int GroupNumber { get { return _groupNumber; } }
        public long PacketNumber { get { return _packetNumber; } }
        public TimeSpan RelativeTime { get { return _relativeTime; } }
        public Packet Packet { get { return _packet; } }
        public EthernetType? EthernetType { get { return _ethernetType; } }
        public string EthernetTypeCode { get { return _ethernetTypeCode; } }
        public IpV4Datagram Ipv4 { get { return _ipv4; } }
        public TcpDatagram Tcp { get { return _tcp; } }
        public IpV4Address? Source { get { return _source; } }
        public ushort? SourcePort { get { return _sourcePort; } }
        public IpV4Address? Destination { get { return _destination; } }
        public ushort? DestinationPort { get { return _destinationPort; } }
        public IpV4Protocol? IpProtocol { get { return _ipProtocol; } }
        public string IpProtocolCode { get { return _ipProtocolCode; } }

        public TcpConnection GetTcpConnection()
        {
            if (_tcpConnection == null && _tcp != null)
                _tcpConnection = new TcpConnection(_packet);
            return _tcpConnection;
        }

        public static string GetEthernetTypeCode(EthernetType ethernetType)
        {
            switch (ethernetType)
            {
                case global::PcapDotNet.Packets.Ethernet.EthernetType.None: return "None";
                case global::PcapDotNet.Packets.Ethernet.EthernetType.IpV4: return "IPv4";
                case global::PcapDotNet.Packets.Ethernet.EthernetType.Arp: return "ARP";
                //case EtherType::WakeOnLan:                                                         return "WakeOnLan";
                //case EtherType::SYN3Heartbeat:                                                     return "SYNdog";
                case global::PcapDotNet.Packets.Ethernet.EthernetType.ReverseArp: return "RARP";
                case global::PcapDotNet.Packets.Ethernet.EthernetType.AppleTalk: return "AppleTalk";
                case global::PcapDotNet.Packets.Ethernet.EthernetType.AppleTalkArp: return "AARP";
                case global::PcapDotNet.Packets.Ethernet.EthernetType.VLanTaggedFrame: return "VLAN";
                case global::PcapDotNet.Packets.Ethernet.EthernetType.NovellInternetworkPacketExchange: return "NovellIPX";
                case global::PcapDotNet.Packets.Ethernet.EthernetType.Novell: return "Novell";
                case global::PcapDotNet.Packets.Ethernet.EthernetType.IpV6: return "IPv6";
                case global::PcapDotNet.Packets.Ethernet.EthernetType.MacControl: return "MACControl";
                case global::PcapDotNet.Packets.Ethernet.EthernetType.PointToPointProtocol: return "PPP";
                //case EtherType::SlowProtocols:                                                     return "SlowProtocols";
                case global::PcapDotNet.Packets.Ethernet.EthernetType.CobraNet: return "CobraNet";
                case global::PcapDotNet.Packets.Ethernet.EthernetType.MultiprotocolLabelSwitchingUnicast: return "MPLSUnicast";
                case global::PcapDotNet.Packets.Ethernet.EthernetType.MultiprotocolLabelSwitchingMulticast: return "MPLSMulticast";
                case global::PcapDotNet.Packets.Ethernet.EthernetType.PointToPointProtocolOverEthernetDiscoveryStage: return "PPPoEDiscoveryStage";
                case global::PcapDotNet.Packets.Ethernet.EthernetType.PointToPointProtocolOverEthernetSessionStage: return "PPPoESessionStage";
                //case EtherType::MicrosoftNLB:                                                      return "MicrosoftNLB";
                //case EtherType::JumboFrames:                                                       return "JumboFrames";
                case global::PcapDotNet.Packets.Ethernet.EthernetType.ExtensibleAuthenticationProtocolOverLan: return "EAPOverLAN";
                //case EtherType::Profinet:                                                          return "Profinet";
                case global::PcapDotNet.Packets.Ethernet.EthernetType.HyperScsi: return "HyperSCSI";
                case global::PcapDotNet.Packets.Ethernet.EthernetType.AtaOverEthernet: return "ATAOverEthernet";
                case global::PcapDotNet.Packets.Ethernet.EthernetType.EtherCatProtocol: return "EtherCAT";
                case global::PcapDotNet.Packets.Ethernet.EthernetType.ProviderBridging: return "ProviderBridging";
                case global::PcapDotNet.Packets.Ethernet.EthernetType.AvbTransportProtocol: return "AVBTP";
                //case EtherType::EthernetPowerlink:                                                 return "EthernetPowerlink";
                //case EtherType::LLDP:                                                              return "LLDP";
                case global::PcapDotNet.Packets.Ethernet.EthernetType.SerialRealTimeCommunicationSystemIii: return "SercosIII";
                case global::PcapDotNet.Packets.Ethernet.EthernetType.CircuitEmulationServicesOverEthernet: return "MEF-8";
                case global::PcapDotNet.Packets.Ethernet.EthernetType.HomePlug: return "HomePlug";
                case global::PcapDotNet.Packets.Ethernet.EthernetType.MacSecurity: return "MACSecurity";
                case global::PcapDotNet.Packets.Ethernet.EthernetType.PrecisionTimeProtocol: return "PrecisionTime";
                case global::PcapDotNet.Packets.Ethernet.EthernetType.ConnectivityFaultManagementOrOperationsAdministrationManagement: return "CFM";
                case global::PcapDotNet.Packets.Ethernet.EthernetType.FibreChannelOverEthernet: return "FibreChannel";
                case global::PcapDotNet.Packets.Ethernet.EthernetType.FibreChannelOverEthernetInitializationProtocol: return "FCoEInitialization";
                case global::PcapDotNet.Packets.Ethernet.EthernetType.QInQ: return "QinQ";
                case global::PcapDotNet.Packets.Ethernet.EthernetType.VeritasLowLatencyTransport: return "LLT";
                default: return "Unknown";
            }
        }

        public static string GetIPProtocolCode(IpV4Protocol protocol)
        {
            switch (protocol)
            {
                case IpV4Protocol.IpV6HopByHopOption: return "HOPOPT";                            //     IPv6 Hop-by-Hop Option RFC 2460
                case IpV4Protocol.InternetControlMessageProtocol: return "ICMP";                              //     Internet Control Message Protocol RFC 792
                case IpV4Protocol.InternetGroupManagementProtocol: return "IGMP";                              //     Internet Group Management Protocol RFC 1112
                case IpV4Protocol.GatewayToGateway: return "GGP";                               //     Gateway-to-Gateway Protocol RFC 823
                case IpV4Protocol.Ip: return "IP";                                //     IP in IP (encapsulation) RFC 2003
                case IpV4Protocol.Stream: return "ST";                                //     Internet Stream Protocol RFC 1190, RFC 1819
                case IpV4Protocol.Tcp: return "TCP";                               //     Transmission Control Protocol RFC 793
                case IpV4Protocol.Cbt: return "CBT";                               //     CBT
                case IpV4Protocol.ExteriorGatewayProtocol: return "EGP";                               //     Exterior Gateway Protocol RFC 888
                case IpV4Protocol.InteriorGatewayProtocol: return "IGP";                               //     Interior Gateway Protocol (any private interior gateway (used by Cisco for their IGRP))
                case IpV4Protocol.BbnRccMonitoring: return "BBN-RCC-MON";                       //     BBN RCC Monitoring
                case IpV4Protocol.NetworkVoice: return "NVP-II";                            //     Network Voice Protocol RFC 741
                case IpV4Protocol.Pup: return "PUP";                               //     Xerox PUP
                case IpV4Protocol.Argus: return "ARGUS";                             //     ARGUS
                case IpV4Protocol.Emcon: return "EMCON";                             //     EMCON
                case IpV4Protocol.CrossNetDebugger: return "XNET";                              //     Cross Net Debugger IEN 158
                case IpV4Protocol.Chaos: return "CHAOS";                             //     Chaos
                case IpV4Protocol.Udp: return "UDP";                               //     User Datagram Protocol RFC 768
                case IpV4Protocol.Multiplexing: return "MUX";                               //     Multiplexing IEN 90
                case IpV4Protocol.DcnMeasurement: return "DCN-MEAS";                          //     DCN Measurement Subsystems
                case IpV4Protocol.HostMonitoringProtocol: return "HMP";                               //     Host Monitoring Protocol RFC 869
                case IpV4Protocol.PacketRadioMeasurement: return "PRM";                               //     Packet Radio Measurement
                case IpV4Protocol.XeroxNsInternetDatagramProtocol: return "XNS-IDP";                           //     XEROX NS IDP
                case IpV4Protocol.Trunk1: return "TRUNK-1";                           //     Trunk-1
                case IpV4Protocol.Trunk2: return "TRUNK-2";                           //     Trunk-2
                case IpV4Protocol.Leaf1: return "LEAF-1";                            //     Leaf-1
                case IpV4Protocol.Leaf2: return "LEAF-2";                            //     Leaf-2
                case IpV4Protocol.ReliableDatagramProtocol: return "RDP";                               //     Reliable Datagram Protocol RFC 908
                case IpV4Protocol.InternetReliableTransactionProtocol: return "IRTP";                              //     Internet Reliable Transaction Protocol RFC 938
                case IpV4Protocol.IsoTransportProtocolClass4: return "ISO-TP4";                           //     ISO Transport Protocol Class 4 RFC 905
                case IpV4Protocol.BulkDataTransferProtocol: return "NETBLT";                            //     Bulk Data Transfer Protocol RFC 998
                case IpV4Protocol.MagneticFusionEnergyNetworkServicesProtocol: return "MFE-NSP";                           //     MFE Network Services Protocol
                case IpV4Protocol.MeritInternodalProtocol: return "MERIT-INP";                         //     MERIT Internodal Protocol
                case IpV4Protocol.DatagramCongestionControlProtocol: return "DCCP";                              //     Datagram Congestion Control Protocol RFC 4340
                case IpV4Protocol.ThirdPartyConnect: return "3PC";                               //     Third Party Connect Protocol
                case IpV4Protocol.InterDomainPolicyRoutingProtocol: return "IDPR";                              //     Inter-Domain Policy Routing Protocol RFC 1479
                case IpV4Protocol.XpressTransportProtocol: return "XTP";                               //     Xpress Transport Protocol
                case IpV4Protocol.DatagramDeliveryProtocol: return "DDP";                               //     Datagram Delivery Protocol
                case IpV4Protocol.InterDomainPolicyRoutingProtocolControlMessageTransportProtocol: return "IDPR-CMTP";                         //     IDPR Control Message Transport Protocol
                case IpV4Protocol.TransportProtocolPlusPlus: return "TP++";                              //     TP++ Transport Protocol
                case IpV4Protocol.Il: return "IL";                                //     IL Transport Protocol
                case IpV4Protocol.IpV6: return "IPv6";                              //     IPv6 RFC 2460
                case IpV4Protocol.SourceDemandRoutingProtocol: return "SDRP";                              //     Source Demand Routing Protocol
                case IpV4Protocol.IpV6Route: return "IPv6-ROUTE";                        //     Routing Header for IPv6 RFC 2460
                case IpV4Protocol.FragmentHeaderForIpV6: return "IPv6-FRAG";                         //     Fragment Header for IPv6 RFC 2460
                case IpV4Protocol.InterDomainRoutingProtocol: return "IDRP";                              //     Inter-Domain Routing Protocol
                case IpV4Protocol.Rsvp: return "RSVP";                              //     Resource Reservation Protocol
                case IpV4Protocol.Gre: return "GRE";                               //     Generic Routing Encapsulation
                case IpV4Protocol.MobileHostRoutingProtocol: return "MHRP";                              //     Mobile Host Routing Protocol
                case IpV4Protocol.Bna: return "BNA";                               //     BNA
                case IpV4Protocol.Esp: return "ESP";                               //     Encapsulating Security Payload RFC 2406
                case IpV4Protocol.AuthenticationHeader: return "AH";                                //     Authentication Header RFC 2402
                case IpV4Protocol.IntegratedNetLayerSecurityProtocol: return "I-NLSP";                            //     Integrated Net Layer Security Protocol TUBA
                case IpV4Protocol.Swipe: return "SWIPE";                             //     IP with Encryption
                case IpV4Protocol.NArp: return "NARP";                              //     NBMA Address Resolution Protocol RFC 1735
                case IpV4Protocol.Mobile: return "MOBILE";                            //     IP Mobility (Min Encap) RFC 2004
                case IpV4Protocol.TransportLayerSecurityProtocol: return "TLSP";                              //     Transport Layer Security Protocol (using Kryptonet key management)
                case IpV4Protocol.Skip: return "SKIP";                              //     Simple Key-Management for Internet Protocol RFC 2356
                case IpV4Protocol.InternetControlMessageProtocolForIpV6: return "IPv6-ICMP";                         //     ICMP for IPv6 RFC 2460
                case IpV4Protocol.NoNextHeaderForIpV6: return "IPv6-NONXT";                        //     No Next Header for IPv6 RFC 2460
                case IpV4Protocol.IpV6Opts: return "IPv6-OPTS";                         //     Destination Options for IPv6 RFC 2460
                case IpV4Protocol.AnyHostInternal: return "ANY-HOST";                          //     Any host internal protocol
                case IpV4Protocol.Cftp: return "CFTP";                              //     CFTP
                case IpV4Protocol.AnyLocalNetwork: return "ANY-LOCAL";                         //     Any local network
                case IpV4Protocol.SatnetAndBackroomExpak: return "SAT-EXPAK";                         //     SATNET and Backroom EXPAK
                case IpV4Protocol.Kryptolan: return "KRYPTOLAN";                         //     Kryptolan
                case IpV4Protocol.RemoteVirtualDiskProtocol: return "RVD";                               //     MIT Remote Virtual Disk Protocol
                case IpV4Protocol.InternetPluribusPacketCore: return "IPPC";                              //     Internet Pluribus Packet Core
                case IpV4Protocol.AnyDistributedFileSystem: return "ADFS";                              //     Any distributed file system
                case IpV4Protocol.SatMon: return "SAT-MON";                           //     SATNET Monitoring
                case IpV4Protocol.Visa: return "VISA";                              //     VISA Protocol
                case IpV4Protocol.InternetPacketCoreUtility: return "IPCV";                              //     Internet Packet Core Utility
                case IpV4Protocol.ComputerProtocolNetworkExecutive: return "CPNX";                              //     Computer Protocol Network Executive
                case IpV4Protocol.ComputerProtocolHeartbeat: return "CPHB";                              //     Computer Protocol Heart Beat
                case IpV4Protocol.WangSpanNetwork: return "WSN";                               //     Wang Span Network
                case IpV4Protocol.PacketVideoProtocol: return "PVP";                               //     Packet Video Protocol
                case IpV4Protocol.BackroomSatMon: return "BR-SAT-MON";                        //     Backroom SATNET Monitoring
                case IpV4Protocol.SunNd: return "SUN-ND";                            //     SUN ND PROTOCOL-Temporary
                case IpV4Protocol.WidebandMonitoring: return "WB-MON";                            //     WIDEBAND Monitoring
                case IpV4Protocol.WidebandExpak: return "WB-EXPAK";                          //     WIDEBAND EXPAK
                case IpV4Protocol.IsoIp: return "ISO-IP";                            //     International Organization for Standardization Internet Protocol
                case IpV4Protocol.VersatileMessageTransactionProtocol: return "VMTP";                              //     Versatile Message Transaction Protocol RFC 1045
                case IpV4Protocol.SecureVersatileMessageTransactionProtocol: return "SECURE-VMTP";                       //     Secure Versatile Message Transaction Protocol RFC 1045
                case IpV4Protocol.Vines: return "VINES";                             //     VINES
                case IpV4Protocol.Ttp: return "TTP";                               //     TTP
                case IpV4Protocol.NationalScienceFoundationNetworkInteriorGatewayProtocol: return "NSFNET-IGP";                        //     NSFNET-IGP
                case IpV4Protocol.DissimilarGatewayProtocol: return "DGP";                               //     Dissimilar Gateway Protocol
                case IpV4Protocol.Tcf: return "TCF";                               //     TCF
                case IpV4Protocol.EnhancedInteriorGatewayRoutingProtocol: return "EIGRP";                             //     Enhanced Interior Gateway Routing Protocol
                case IpV4Protocol.OpenShortestPathFirst: return "OSPF";                              //     Open Shortest Path First RFC 1583
                case IpV4Protocol.SpriteRpc: return "SPRITE-RPC";                        //     Sprite RPC Protocol
                case IpV4Protocol.LArp: return "LARP";                              //     Locus Address Resolution Protocol
                case IpV4Protocol.MulticastTransportProtocol: return "MTP";                               //     Multicast Transport Protocol
                case IpV4Protocol.Ax25: return "AX.25";                             //     AX.25
                case IpV4Protocol.IpIp: return "IPIP";                              //     IP-within-IP Encapsulation Protocol
                case IpV4Protocol.MobileInternetworkingControlProtocol: return "MICP";                              //     Mobile Internetworking Control Protocol
                case IpV4Protocol.SemaphoreCommunicationsSecondProtocol: return "SCC-SP";                            //     Semaphore Communications Sec. Pro
                case IpV4Protocol.EtherIp: return "ETHERIP";                           //     Ethernet-within-IP Encapsulation RFC 3378
                case IpV4Protocol.EncapsulationHeader: return "ENCAP";                             //     Encapsulation Header RFC 1241
                case IpV4Protocol.AnyPrivateEncryptionScheme: return "APES";                              //     Any private encryption scheme
                case IpV4Protocol.Gmtp: return "GMTP";                              //     GMTP
                case IpV4Protocol.IpsilonFlowManagementProtocol: return "IFMP";                              //     Ipsilon Flow Management Protocol
                case IpV4Protocol.PrivateNetworkToNetworkInterface: return "PNNI";                              //     PNNI over IP
                case IpV4Protocol.Pin: return "PIM";                               //     Protocol Independent Multicast
                case IpV4Protocol.Aris: return "ARIS";                              //     ARIS
                case IpV4Protocol.SpaceCommunicationsProtocolStandards: return "SCPS";                              //     SCPS (Space Communications Protocol Standards)
                case IpV4Protocol.Qnx: return "QNX";                               //     QNX
                case IpV4Protocol.ActiveNetworks: return "A/N";                               //     Active Networks
                case IpV4Protocol.IpComp: return "IPCOMP";                            //     IP Payload Compression Protocol RFC 3173
                case IpV4Protocol.SitaraNetworksProtocol: return "SNP";                               //     Sitara Networks Protocol
                case IpV4Protocol.CompaqPeer: return "COMPAQ-PEER";                       //     Compaq Peer Protocol
                case IpV4Protocol.InternetworkPacketExchangeInIp: return "IPX-in-IP";                         //     IPX in IP
                case IpV4Protocol.VirtualRouterRedundancyProtocol: return "VRRP";                              //     Virtual Router Redundancy Protocol, Common Address Redundancy Protocol (not IANA assigned) VRRP:RFC 3768
                case IpV4Protocol.PragmaticGeneralMulticastTransportProtocol: return "PGM";                               //     PGM Reliable Transport Protocol RFC 3208
                case IpV4Protocol.Any0HopProtocol: return "A0HP";                              //     Any 0-hop protocol
                case IpV4Protocol.LayerTwoTunnelingProtocol: return "L2TP";                              //     Layer Two Tunneling Protocol
                case IpV4Protocol.DiiDataExchange: return "DDX";                               //     D-II Data Exchange (DDX)
                case IpV4Protocol.InteractiveAgentTransferProtocol: return "IATP";                              //     Interactive Agent Transfer Protocol
                case IpV4Protocol.ScheduleTransferProtocol: return "STP";                               //     Schedule Transfer Protocol
                case IpV4Protocol.SpectraLinkRadioProtocol: return "SRP";                               //     SpectraLink Radio Protocol
                case IpV4Protocol.Uti: return "UTI";                               //     UTI
                case IpV4Protocol.SimpleMessageProtocol: return "SMP";                               //     Simple Message Protocol
                case IpV4Protocol.Sm: return "SM";                                //     SM
                case IpV4Protocol.PerformanceTransparencyProtocol: return "PTP";                               //     Performance Transparency Protocol
                case IpV4Protocol.IsIsOverIpV4: return "IS-IS-IPv4";                        //     IS-IS over IPv4
                case IpV4Protocol.Fire: return "FIRE";
                case IpV4Protocol.CombatRadioTransportProtocol: return "CRTP";                              //     Combat Radio Transport Protocol
                case IpV4Protocol.CombatRadioUserDatagram: return "CRUDP";                             //     Combat Radio User Datagram
                case IpV4Protocol.ServiceSpecificConnectionOrientedProtocolInAMultilinkAndConnectionlessEnvironment: return "SSCOPMCE";
                case IpV4Protocol.Iplt: return "IPLT";
                case IpV4Protocol.SecurePacketShield: return "SPS";                               //     Secure Packet Shield
                case IpV4Protocol.Pipe: return "PIPE";                              //     Private IP Encapsulation within IP Expired I-D draft-petri-mobileip-pipe-00.txt
                case IpV4Protocol.StreamControlTransmissionProtocol: return "SCTP";                              //     Stream Control Transmission Protocol
                case IpV4Protocol.FibreChannel: return "FC";                                //     Fibre Channel
                case IpV4Protocol.RsvpE2EIgnore: return "RSVP-E2E-IGNORE";                   //     RSVP-E2E-IGNORE RFC 3175
                case IpV4Protocol.MobilityHeader: return "MH";                                //     Mobility Header RFC 3775
                case IpV4Protocol.UdpLite: return "UDP-LITE";                          //     UDP Lite RFC 3828
                case IpV4Protocol.MultiprotocolLabelSwitchingInIp: return "MPLS-in-IP";                        //     MPLS-in-IP RFC 4023
                case IpV4Protocol.MobileAdHocNetwork: return "MANET";                             //     MANET Protocols I-D draft-ietf-manet-iana-07.txt
                case IpV4Protocol.Hip: return "HIP";                               //     Host Identity Protocol RFC 5201
                //case IPProtocol::SiteMultihomingByIPv6Intermediation:         return "SHIM6";
                // 0x8C    Site Multihoming by IPv6 Intermediation (Shim6) (RFC 5533)
                default: return "Unknown";
            }
        }

        public string GetTcpFlagsString()
        {
            if (_tcp == null)
                return null;
            StringBuilder flags = new StringBuilder();
            if (!_tcp.IsValid) flags.zAddValue("NVAL");
            if (_tcp.IsFin) flags.zAddValue("FIN");
            if (_tcp.IsSynchronize) flags.zAddValue("SYN");
            if (_tcp.IsReset) flags.zAddValue("RST");
            if (_tcp.IsPush) flags.zAddValue("PSH");
            if (_tcp.IsAcknowledgment) flags.zAddValue("ACK");
            if (_tcp.IsUrgent) flags.zAddValue("URG");
            if (_tcp.IsExplicitCongestionNotificationEcho) flags.zAddValue("ECE");
            if (_tcp.IsCongestionWindowReduced) flags.zAddValue("CWR");
            if (_tcp.IsChecksumOptional) flags.zAddValue("CO");
            return flags.ToString();
        }
    }

    public class PPacketManager
    {
        private long _packetCount = 0;
        private DateTime _timeFirstPacket;

        public PPacket CreatePPacket(Packet packet)
        {
            if (_packetCount == 0)
                _timeFirstPacket = packet.Timestamp;
            return new PPacket(packet, ++_packetCount, packet.Timestamp.Subtract(_timeFirstPacket));
        }
    }
}
