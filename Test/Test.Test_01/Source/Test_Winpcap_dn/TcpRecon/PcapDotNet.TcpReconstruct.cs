﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using pb.IO;
using PcapDotNet.Core;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Transport;

// from TCP Session Reconstruction Tool http://www.codeproject.com/Articles/20501/TCP-Session-Reconstruction-Tool

//namespace Test_Winpcap_dn.PcapDotNet
namespace Test_Winpcap_dn.TcpRecon_PcapDotNet
{
    public class TcpReconstruct
    {
        // Holds the file streams for each tcp session in case we use SharpPcap
        private static Dictionary<Connection, TcpRecon> sharpPcapDict = new Dictionary<Connection, TcpRecon>();
        private static string _path = null;

        public static void ReconstructSingleFileSharpPcap(string capFile)
        {
            //PcapDevice device;
            PacketDevice device;

            //FileInfo fi = new FileInfo(capFile);
            var fi = zFile.CreateFileInfo(capFile);
            _path = fi.DirectoryName + "\\";
            try
            {
                //Get an offline file pcap device
                //device = SharpPcap.GetPcapOfflineDevice(capFile);
                device = new OfflinePacketDevice(capFile);
                //Open the device for capturing
                //device.PcapOpen();
                using (PacketCommunicator communicator = device.Open(65536, PacketDeviceOpenAttributes.Promiscuous, 1000))
                {
                    communicator.ReceivePackets(0, device_PcapOnPacketArrival);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            //Register our handler function to the 'packet arrival' event
            //device.PcapOnPacketArrival += new SharpPcap.PacketArrivalEvent(device_PcapOnPacketArrival);

            // FOR THIS LINE TO WORK YOU NEED TO CHANGE THE
            // SHARPPCAP LIBRARY MANUALLY AND REMOVE PcapSetFilter method
            // FROM PcapOfflineDevice
            //
            // add a filter so we get only tcp packets
            // device.PcapSetFilter("tcp");

            //Start capture 'INFINTE' number of packets
            //This method will return when EOF reached.
            //device.PcapCapture(SharpPcap.INFINITE);

            //Close the pcap device
            //device.PcapClose();

            // Clean up
            foreach (TcpRecon tr in sharpPcapDict.Values)
            {
                tr.Close();
            }
            sharpPcapDict.Clear();
        }

        private static void device_PcapOnPacketArrival(Packet packet)
        {
            //if (!(packet is TCPPacket)) return;
            //if (packet.IpV4 == null || packet.IpV4.Tcp == null)
            if (packet.Ethernet == null || packet.Ethernet.IpV4 == null || packet.Ethernet.IpV4.Tcp == null)
                return;

            //TCPPacket tcpPacket = (TCPPacket)packet;
            //TcpDatagram tcp = packet.Ethernet.IpV4.Tcp;
            // Creates a key for the dictionary
            //Connection c = new Connection(tcpPacket);
            Connection c = new Connection(packet);

            // create a new entry if the key does not exists
            if (!sharpPcapDict.ContainsKey(c))
            {
                string fileName = c.getFileName(_path);
                TcpRecon tcpRecon = new TcpRecon(fileName);
                sharpPcapDict.Add(c, tcpRecon);
            }

            // Use the TcpRecon class to reconstruct the session
            //sharpPcapDict[c].ReassemblePacket(tcpPacket);
            sharpPcapDict[c].ReassemblePacket(packet);
        }
    }

    public class Connection
    {
        private string m_srcIp;
        public string SourceIp
        {
            get { return m_srcIp; }
        }

        private ushort m_srcPort;
        public ushort SourcePort
        {
            get { return m_srcPort; }
        }

        private string m_dstIp;
        public string DestinationIp
        {
            get { return m_dstIp; }
        }

        private ushort m_dstPort;
        public ushort DestinationPort
        {
            get { return m_dstPort; }
        }

        public Connection(string sourceIP, UInt16 sourcePort, string destinationIP, UInt16 destinationPort)
        {
            m_srcIp = sourceIP;
            m_dstIp = destinationIP;
            m_srcPort = sourcePort;
            m_dstPort = destinationPort;
        }

        //public Connection(Tamir.IPLib.Packets.TCPPacket packet)
        public Connection(Packet packet)
        {
            //m_srcIp = packet.SourceAddress;
            m_srcIp = packet.Ethernet.IpV4.Source.ToString();
            //m_dstIp = packet.DestinationAddress;
            m_dstIp = packet.Ethernet.IpV4.Destination.ToString();
            //m_srcPort = (ushort)packet.SourcePort;
            m_srcPort = (ushort)packet.Ethernet.IpV4.Tcp.SourcePort;
            //m_dstPort = (ushort)packet.DestinationPort;
            m_dstPort = (ushort)packet.Ethernet.IpV4.Tcp.DestinationPort;
        }

        /// <summary>
        /// Overrided in order to catch both sides of the connection 
        /// with the same connection object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Connection))
                return false;
            Connection con = (Connection)obj;

            bool result = ((con.SourceIp.Equals(m_srcIp)) && (con.SourcePort == m_srcPort) && (con.DestinationIp.Equals(m_dstIp)) && (con.DestinationPort == m_dstPort)) ||
                ((con.SourceIp.Equals(m_dstIp)) && (con.SourcePort == m_dstPort) && (con.DestinationIp.Equals(m_srcIp)) && (con.DestinationPort == m_srcPort));

            return result;
        }

        public override int GetHashCode()
        {
            return ((m_srcIp.GetHashCode() ^ m_srcPort.GetHashCode()) as object).GetHashCode() ^
                ((m_dstIp.GetHashCode() ^ m_dstPort.GetHashCode()) as object).GetHashCode();
        }

        public string getFileName(string path)
        {
            return string.Format("{0}{1}.{2}-{3}.{4}.data", path, m_srcIp, m_srcPort, m_dstIp, m_dstPort);
        }
    }

    /// <summary>
    /// A class that represent a node in a linked list that holds partial Tcp session
    /// fragments
    /// </summary>
    internal class tcp_frag
    {
        public ulong seq = 0;
        public ulong len = 0;
        public ulong data_len = 0;
        public byte[] data = null;
        public tcp_frag next = null;
    };

    public class TcpRecon
    {
        // holds two linked list of the session data, one for each direction    
        tcp_frag[] frags = new tcp_frag[2];
        // holds the last sequence number for each direction
        ulong[] seq = new ulong[2];
        long[] src_addr = new long[2];
        uint[] src_port = new uint[2];
        bool empty_tcp_stream = true;
        uint[] tcp_port = new uint[2];
        uint[] bytes_written = new uint[2];
        System.IO.FileStream data_out_file = null;
        bool incomplete_tcp_stream = false;
        bool closed = false;

        public bool IncompleteStream
        {
            get { return incomplete_tcp_stream; }
        }
        public bool EmptyStream
        {
            get { return empty_tcp_stream; }
        }

        public TcpRecon(string filename)
        {
            reset_tcp_reassembly();
            data_out_file = new System.IO.FileStream(filename, System.IO.FileMode.Create);
        }

        /// <summary>
        /// Cleans up the class and frees resources
        /// </summary>
        public void Close()
        {
            if (!closed)
            {
                if (data_out_file != null)
                    data_out_file.Close();
                reset_tcp_reassembly();
                closed = true;
            }
        }

        ~TcpRecon()
        {
            Close();
        }

        /// <summary>
        /// The main function of the class receives a tcp packet and reconstructs the stream
        /// </summary>
        /// <param name="tcpPacket"></param>
        //public void ReassemblePacket(Tamir.IPLib.Packets.TCPPacket tcpPacket)
        public void ReassemblePacket(Packet packet)
        {
            TcpDatagram tcp = packet.Ethernet.IpV4.Tcp;

            // if the paylod length is zero bail out
            //ulong length = (ulong)(tcpPacket.TCPPacketByteLength - tcpPacket.TCPHeaderLength);
            ulong length = (ulong)tcp.PayloadLength;
            if (length == 0)
                return;

            //reassemble_tcp((ulong)tcpPacket.SequenceNumber, length, tcpPacket.TCPData, (ulong)tcpPacket.TCPData.Length, tcpPacket.Syn,
            //               tcpPacket.SourceAddressAsLong, tcpPacket.DestinationAddressAsLong, (uint)tcpPacket.SourcePort, (uint)tcpPacket.DestinationPort);
            reassemble_tcp((ulong)tcp.SequenceNumber, length, tcp.Payload.ToArray(), (ulong)tcp.PayloadLength, tcp.IsSynchronize,
                           packet.Ethernet.IpV4.Source.ToValue(), packet.Ethernet.IpV4.Destination.ToValue(), (uint)tcp.SourcePort, (uint)tcp.DestinationPort);
        }

        /// <summary>
        /// Writes the payload data to the file
        /// </summary>
        /// <param name="index"></param>
        /// <param name="data"></param>
        private void write_packet_data(int index, byte[] data)
        {
            // ignore empty packets
            if (data.Length == 0)
                return;

            data_out_file.Write(data, 0, data.Length);
            bytes_written[index] += (uint)data.Length;
            empty_tcp_stream = false;
        }

        /// <summary>
        /// Reconstructs the tcp session
        /// </summary>
        /// <param name="sequence">Sequence number of the tcp packet</param>
        /// <param name="length">The size of the original packet data</param>
        /// <param name="data">The captured data</param>
        /// <param name="data_length">The length of the captured data</param>
        /// <param name="synflag"></param>
        /// <param name="net_src">The source ip address</param>
        /// <param name="net_dst">The destination ip address</param>
        /// <param name="srcport">The source port</param>
        /// <param name="dstport">The destination port</param>
        //private void reassemble_tcp(ulong sequence, ulong length, IEnumerable<byte> data, ulong data_length, bool synflag, long net_src, long net_dst, uint srcport, uint dstport)
        private void reassemble_tcp(ulong sequence, ulong length, byte[] data, ulong data_length, bool synflag, long net_src, long net_dst, uint srcport, uint dstport)
        {
            long srcx, dstx;
            int src_index, j;
            bool first = false;
            ulong newseq;
            tcp_frag tmp_frag;

            src_index = -1;

            /* Now check if the packet is for this connection. */
            srcx = net_src;
            dstx = net_dst;

            /* Check to see if we have seen this source IP and port before.
            (Yes, we have to check both source IP and port; the connection
            might be between two different ports on the same machine.) */
            for (j = 0; j < 2; j++)
            {
                if (src_addr[j] == srcx && src_port[j] == srcport)
                {
                    src_index = j;
                }
            }
            /* we didn't find it if src_index == -1 */
            if (src_index < 0)
            {
                /* assign it to a src_index and get going */
                for (j = 0; j < 2; j++)
                {
                    if (src_port[j] == 0)
                    {
                        src_addr[j] = srcx;
                        src_port[j] = srcport;
                        src_index = j;
                        first = true;
                        break;
                    }
                }
            }
            if (src_index < 0)
            {
                throw new Exception("ERROR in reassemble_tcp: Too many addresses!");
            }

            if (data_length < length)
            {
                incomplete_tcp_stream = true;
            }

            /* now that we have filed away the srcs, lets get the sequence number stuff
            figured out */
            if (first)
            {
                /* this is the first time we have seen this src's sequence number */
                seq[src_index] = sequence + length;
                if (synflag)
                {
                    seq[src_index]++;
                }
                /* write out the packet data */
                write_packet_data(src_index, data);
                return;
            }
            /* if we are here, we have already seen this src, let's
            try and figure out if this packet is in the right place */
            if (sequence < seq[src_index])
            {
                /* this sequence number seems dated, but
                check the end to make sure it has no more
                info than we have already seen */
                newseq = sequence + length;
                if (newseq > seq[src_index])
                {
                    ulong new_len;

                    /* this one has more than we have seen. let's get the
                    payload that we have not seen. */

                    new_len = seq[src_index] - sequence;

                    if (data_length <= new_len)
                    {
                        data = null;
                        data_length = 0;
                        incomplete_tcp_stream = true;
                    }
                    else
                    {
                        data_length -= new_len;
                        byte[] tmpData = new byte[data_length];
                        for (ulong i = 0; i < data_length; i++)
                            tmpData[i] = data[i + new_len];
                        data = tmpData;
                    }
                    sequence = seq[src_index];
                    length = newseq - seq[src_index];

                    /* this will now appear to be right on time :) */
                }
            }
            if (sequence == seq[src_index])
            {
                /* right on time */
                seq[src_index] += length;
                if (synflag) seq[src_index]++;
                if (data != null)
                {
                    write_packet_data(src_index, data);
                }
                /* done with the packet, see if it caused a fragment to fit */
                while (check_fragments(src_index))
                    ;
            }
            else
            {
                /* out of order packet */
                if (data_length > 0 && sequence > seq[src_index])
                {
                    tmp_frag = new tcp_frag();
                    tmp_frag.data = data;
                    tmp_frag.seq = sequence;
                    tmp_frag.len = length;
                    tmp_frag.data_len = data_length;

                    if (frags[src_index] != null)
                    {
                        tmp_frag.next = frags[src_index];
                    }
                    else
                    {
                        tmp_frag.next = null;
                    }
                    frags[src_index] = tmp_frag;
                }
            }
        } /* end reassemble_tcp */

        /* here we search through all the frag we have collected to see if one fits */
        bool check_fragments(int index)
        {
            tcp_frag prev = null;
            tcp_frag current;
            current = frags[index];
            while (current != null)
            {
                if (current.seq == seq[index])
                {
                    /* this fragment fits the stream */
                    if (current.data != null)
                    {
                        write_packet_data(index, current.data);
                    }
                    seq[index] += current.len;
                    if (prev != null)
                    {
                        prev.next = current.next;
                    }
                    else
                    {
                        frags[index] = current.next;
                    }
                    current.data = null;
                    current = null;
                    return true;
                }
                prev = current;
                current = current.next;
            }
            return false;
        }

        // cleans the linked list
        void reset_tcp_reassembly()
        {
            tcp_frag current, next;
            int i;

            empty_tcp_stream = true;
            incomplete_tcp_stream = false;
            for (i = 0; i < 2; i++)
            {
                seq[i] = 0;
                src_addr[i] = 0;
                src_port[i] = 0;
                tcp_port[i] = 0;
                bytes_written[i] = 0;
                current = frags[i];
                while (current != null)
                {
                    next = current.next;
                    current.data = null;
                    current = null;
                    current = next;
                }
                frags[i] = null;
            }
        }
    }
}
