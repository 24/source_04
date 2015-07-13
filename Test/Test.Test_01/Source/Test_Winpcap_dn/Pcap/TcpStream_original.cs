using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Transport;

namespace Pib.Pcap
{
    // not used, pas de test, ré-écriture intermédiaire
    public class TcpStream_original : IDisposable
    {
        // holds two linked list of the session data, one for each direction    
        TcpFragment[] _fragments = new TcpFragment[2];
        // holds the last sequence number for each direction
        uint[] _sequenceNumbers = new uint[2];
        uint[] _sourceAdresses = new uint[2];
        ushort[] _sourcePorts = new ushort[2];
        bool _emptyTcpStream = true;
        ushort[] _tcpPorts = new ushort[2];
        uint[] _bytesWritten = new uint[2];
        System.IO.FileStream _fileStream = null;
        bool _incompleteTcpStream = false;
        bool _closed = false;

        public bool IncompleteStream { get { return _incompleteTcpStream; } }
        public bool EmptyStream { get { return _emptyTcpStream; } }

        public TcpStream_original(string filename)
        {
            reset_tcp_reassembly();
            _fileStream = new System.IO.FileStream(filename, System.IO.FileMode.Create);
        }

        public void Dispose()
        {
            Close();
        }

        public void Close()
        {
            if (!_closed)
            {
                if (_fileStream != null)
                {
                    _fileStream.Close();
                    _fileStream = null;
                }
                reset_tcp_reassembly();
                _closed = true;
            }
        }

        void reset_tcp_reassembly()
        {
            TcpFragment current, next;
            int i;

            _emptyTcpStream = true;
            _incompleteTcpStream = false;
            for (i = 0; i < 2; i++)
            {
                _sequenceNumbers[i] = 0;
                _sourceAdresses[i] = 0;
                _sourcePorts[i] = 0;
                _tcpPorts[i] = 0;
                _bytesWritten[i] = 0;
                current = _fragments[i];
                while (current != null)
                {
                    next = current.next;
                    current.data = null;
                    current = null;
                    current = next;
                }
                _fragments[i] = null;
            }
        }

        public void ReassemblePacket(Packet packet)
        {
            TcpDatagram tcp = packet.Ethernet.IpV4.Tcp;

            // if the paylod length is zero bail out
            //ulong length = (ulong)(tcpPacket.TCPPacketByteLength - tcpPacket.TCPHeaderLength);
            int length = tcp.PayloadLength;
            if (length == 0)
                return;

            //reassemble_tcp((ulong)tcpPacket.SequenceNumber, length, tcpPacket.TCPData, (ulong)tcpPacket.TCPData.Length, tcpPacket.Syn,
            //               tcpPacket.SourceAddressAsLong, tcpPacket.DestinationAddressAsLong, (uint)tcpPacket.SourcePort, (uint)tcpPacket.DestinationPort);
            reassemble_tcp(tcp.SequenceNumber, (uint)length, tcp.Payload.ToArray(), (uint)tcp.PayloadLength, tcp.IsSynchronize,
                           packet.Ethernet.IpV4.Source.ToValue(), tcp.SourcePort, packet.Ethernet.IpV4.Destination.ToValue(), tcp.DestinationPort);
        }

        private void reassemble_tcp(uint sequenceNumber, uint length, byte[] data, uint dataLength, bool synflag, uint sourceAdress, ushort sourcePort, uint destinationAdress, ushort destinationPort)
        {
            //long srcx, dstx;
            int src_index, j;
            bool first = false;
            //ulong newseq;
            uint newSequenceNumber;
            TcpFragment tmp_frag;

            src_index = -1;

            /* Now check if the packet is for this connection. */
            uint srcx = sourceAdress;
            uint dstx = destinationAdress;

            /* Check to see if we have seen this source IP and port before.
            (Yes, we have to check both source IP and port; the connection
            might be between two different ports on the same machine.) */
            for (j = 0; j < 2; j++)
            {
                if (_sourceAdresses[j] == srcx && _sourcePorts[j] == sourcePort)
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
                    if (_sourcePorts[j] == 0)
                    {
                        _sourceAdresses[j] = srcx;
                        _sourcePorts[j] = sourcePort;
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

            if (dataLength < length)
            {
                _incompleteTcpStream = true;
            }

            /* now that we have filed away the srcs, lets get the sequence number stuff
            figured out */
            if (first)
            {
                /* this is the first time we have seen this src's sequence number */
                _sequenceNumbers[src_index] = sequenceNumber + length;
                if (synflag)
                {
                    _sequenceNumbers[src_index]++;
                }
                /* write out the packet data */
                write_packet_data(src_index, data);
                return;
            }
            /* if we are here, we have already seen this src, let's
            try and figure out if this packet is in the right place */
            if (sequenceNumber < _sequenceNumbers[src_index])
            {
                /* this sequence number seems dated, but
                check the end to make sure it has no more
                info than we have already seen */
                newSequenceNumber = sequenceNumber + length;
                if (newSequenceNumber > _sequenceNumbers[src_index])
                {
                    //ulong new_len;
                    uint new_len;

                    /* this one has more than we have seen. let's get the
                    payload that we have not seen. */

                    new_len = _sequenceNumbers[src_index] - sequenceNumber;

                    if (dataLength <= new_len)
                    {
                        data = null;
                        dataLength = 0;
                        _incompleteTcpStream = true;
                    }
                    else
                    {
                        dataLength -= new_len;
                        byte[] tmpData = new byte[dataLength];
                        for (ulong i = 0; i < dataLength; i++)
                            tmpData[i] = data[i + new_len];
                        data = tmpData;
                    }
                    sequenceNumber = _sequenceNumbers[src_index];
                    length = newSequenceNumber - _sequenceNumbers[src_index];

                    /* this will now appear to be right on time :) */
                }
            }
            if (sequenceNumber == _sequenceNumbers[src_index])
            {
                /* right on time */
                _sequenceNumbers[src_index] += length;
                if (synflag) _sequenceNumbers[src_index]++;
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
                if (dataLength > 0 && sequenceNumber > _sequenceNumbers[src_index])
                {
                    tmp_frag = new TcpFragment();
                    tmp_frag.data = data;
                    tmp_frag.sequenceNumber = sequenceNumber;
                    tmp_frag.length = length;
                    tmp_frag.dataLength = dataLength;

                    if (_fragments[src_index] != null)
                    {
                        tmp_frag.next = _fragments[src_index];
                    }
                    else
                    {
                        tmp_frag.next = null;
                    }
                    _fragments[src_index] = tmp_frag;
                }
            }
        }

        private void write_packet_data(int index, byte[] data)
        {
            // ignore empty packets
            if (data.Length == 0)
                return;

            _fileStream.Write(data, 0, data.Length);
            _bytesWritten[index] += (uint)data.Length;
            _emptyTcpStream = false;
        }

        /* here we search through all the frag we have collected to see if one fits */
        bool check_fragments(int index)
        {
            TcpFragment prev = null;
            TcpFragment current;
            current = _fragments[index];
            while (current != null)
            {
                if (current.sequenceNumber == _sequenceNumbers[index])
                {
                    /* this fragment fits the stream */
                    if (current.data != null)
                    {
                        write_packet_data(index, current.data);
                    }
                    _sequenceNumbers[index] += current.length;
                    if (prev != null)
                    {
                        prev.next = current.next;
                    }
                    else
                    {
                        _fragments[index] = current.next;
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
    }
}
