using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using PcapDotNet.Packets;
using PcapDotNet.Packets.IpV4;
using pb;
using Pib.Pcap;

namespace Test_Winpcap_dn
{

    // Libpcap File Format http://wiki.wireshark.org/Development/LibpcapFileFormat

    static partial class w
    {
        public static void Test_ReadPcap_01(string file)
        {
            Trace.WriteLine("read file \"{0}\"", file);
            file = GetPath(file);
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                PcapHeader pcapHeader;
                if (!ReadPcapHeader(fs, out pcapHeader))
                    return;
                Trace.WriteLine("pcap header :");
                Trace.WriteLine("  magic number  0x{0}", pcapHeader.magicNumber.zToHex());
                Trace.WriteLine("  version       {0}.{1}", pcapHeader.versionMajor, pcapHeader.versionMinor);
                Trace.WriteLine("  time zone     {0}", pcapHeader.timeZone);
                Trace.WriteLine("  sigfigs       {0}", pcapHeader.sigfigs);
                Trace.WriteLine("  snaplen       {0}", pcapHeader.snaplen);
                Trace.WriteLine("  network       {0}", pcapHeader.network);


                PcapPacketHeader packetHeader;
                while (ReadPcapPacketHeader(fs, out packetHeader))
                {
                    Trace.WriteLine("pcap header :");
                    Trace.WriteLine("  timestamp         {0}:{1}", packetHeader.timestampSeconds, packetHeader.timestampMicroseconds);
                    Trace.WriteLine("  length            {0}", packetHeader.length);
                    Trace.WriteLine("  original length   {0}", packetHeader.originalLength);

                    uint l = packetHeader.length;
                    byte[] data = new byte[l];
                    if ((uint)fs.Read(data, 0, (int)l) != l)
                        throw new PBException("error reading file \"{0}\"", file);

                }
            }
        }

        public static void Test_ReadPcap_02(string file, bool detail = false)
        {
            Trace.WriteLine("read file \"{0}\"", file);
            using (PcapFileReader pcapFileReader = new PcapFileReader(GetPath(file)))
            {
                PPacketManager ppacketManager = new PPacketManager();
                foreach (PcapPacket packet in pcapFileReader)
                {
                    Packet packet2 = new Packet(packet.Data, packet.Timestamp, DataLinkKind.Ethernet);
                    //Trace.WriteLine("packet no {0}", packet.Index);
                    //PrintPacketHandler2(ppacketManager.CreatePPacket(packet2), detail);
                    PrintPacket.PrintPacket1(ppacketManager.CreatePPacket(packet2), detail);
                }
            }
        }

        public static void Test_ReadPcap_PrintPacketsDetail_01(string file)
        {
            Trace.WriteLine("read file \"{0}\"", file);
            using (PcapFileReader pcapFileReader = new PcapFileReader(GetPath(file)))
            {
                PPacketManager ppacketManager = new PPacketManager();
                int i = 0;
                foreach (PcapPacket packet in pcapFileReader)
                {
                    Packet packet2 = new Packet(packet.Data, packet.Timestamp, DataLinkKind.Ethernet);
                    IpV4Datagram ip = packet2.Ethernet.IpV4;
                    if (ip.Tcp == null || ip.Tcp.Http.Version == null)
                        continue;
                    if (++i > 5)
                        break;
                    PrintPacketDetail.PrintPacketDetailHandler(ppacketManager.CreatePPacket(packet2));
                }
            }
        }

        //public static unsafe T ReadValue<T>(Stream stream) where T : struct
        //{
        //    int l = Marshal.SizeOf(typeof(T));
        //    byte[] data = new byte[l];
        //    stream.Read(data, 0, l);
        //    fixed (byte* p = &data[0])
        //    {
        //        return *(T*)p;
        //    }
        //}

        public static unsafe bool ReadPcapHeader(Stream stream, out PcapHeader pcapHeader)
        {
            int l = sizeof(PcapHeader);
            byte[] data = new byte[l];
            if (stream.Read(data, 0, l) != l)
            {
                pcapHeader = new PcapHeader();
                return false;
            }
            fixed (byte* p = &data[0])
            {
                pcapHeader = *(PcapHeader*)p;
                return true;
            }
        }

        public static unsafe bool ReadPcapPacketHeader(Stream stream, out PcapPacketHeader packetHeader)
        {
            int l = sizeof(PcapPacketHeader);
            byte[] data = new byte[l];
            if (stream.Read(data, 0, l) != l)
            {
                packetHeader = new PcapPacketHeader();
                return false;
            }
            fixed (byte* p = &data[0])
            {
                packetHeader = *(PcapPacketHeader*)p;
                return true;
            }
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }

    public struct PcapHeader
    {
        public uint magicNumber;     // magic number
        public ushort versionMajor;  // major version number
        public ushort versionMinor;  // minor version number
        public int timeZone;        // GMT to local correction
        public uint sigfigs;         // accuracy of timestamps
        public uint snaplen;         // max length of captured packets, in octets
        public uint network;         // data link type
    }

    public struct PcapPacketHeader
    {
        public uint timestampSeconds;       // timestamp seconds
        public uint timestampMicroseconds;  // timestamp microseconds
        public uint length;                 // number of octets of packet saved in file
        public uint originalLength;         // actual length of packet
    }

    public class PcapPacket
    {
        public int Index;
        public DateTime Timestamp;
        //public uint TimestampSeconds;       // timestamp seconds
        //public uint TimestampMicroseconds;  // timestamp microseconds
        public uint Length;                 // number of octets of packet saved in file
        public uint OriginalLength;         // actual length of packet
        public byte[] Data;
    }

    public class PcapFileReader : IEnumerable<PcapPacket>, IEnumerator<PcapPacket>
    {
        private bool _opened = false;
        private string _file = null;
        private FileStream _fs = null;
        private PcapHeader _pcapHeader;
        //private PcapPacketHeader _packetHeader;
        private int _index;
        private PcapPacket _packet;
        private static DateTime _unixOriginDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

        public PcapFileReader(string file)
        {
            _file = file;
        }

        public void Dispose()
        {
            if (_fs != null)
            {
                _fs.Close();
                _fs = null;
            }
        }

        private void Open()
        {
            if (_opened)
                return;
            _fs = new FileStream(_file, FileMode.Open, FileAccess.Read, FileShare.Read);
            _index = 0;
            _pcapHeader = ReadPcapHeader();
            _opened = true;
        }

        private unsafe PcapHeader ReadPcapHeader()
        {
            int l = sizeof(PcapHeader);
            byte[] data = new byte[l];

            if (_fs.Read(data, 0, l) != l)
                throw new PBException("error reading file \"{0}\"", _file);

            fixed (byte* p = &data[0])
            {
                return *(PcapHeader*)p;
            }
        }

        private unsafe bool ReadPacketHeader(out PcapPacketHeader packetHeader)
        {
            int l = sizeof(PcapPacketHeader);
            byte[] data = new byte[l];

            if (_fs.Read(data, 0, l) != l)
            {
                packetHeader = new PcapPacketHeader();
                return false;
            }

            fixed (byte* p = &data[0])
            {
                packetHeader = *(PcapPacketHeader*)p;
                return true;
            }
        }

        private unsafe bool ReadPacket()
        {
            PcapPacketHeader packetHeader;
            if (!ReadPacketHeader(out packetHeader))
                return false;
            _packet = new PcapPacket();
            _packet.Index = ++_index;
            //_packet.TimestampSeconds = packetHeader.timestampSeconds;
            //_packet.TimestampMicroseconds = packetHeader.timestampMicroseconds;
            // nano 10-9, 100 nano 10-7, micro second 10-6
            _packet.Timestamp = _unixOriginDateTime.AddSeconds(packetHeader.timestampSeconds).AddTicks((long)packetHeader.timestampMicroseconds * 10).ToLocalTime();
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

            _packet.Length = packetHeader.length;
            _packet.OriginalLength = packetHeader.originalLength;
            uint l = packetHeader.length;
            _packet.Data = new byte[l];
            if ((uint)_fs.Read(_packet.Data, 0, (int)l) != l)
                throw new PBException("error reading file \"{0}\"", _file);
            return true;
        }

        public IEnumerator<PcapPacket> GetEnumerator()
        {
            return this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this;
        }

        public PcapPacket Current
        {
            get { return _packet; }
        }

        object System.Collections.IEnumerator.Current
        {
            get { return _packet; }
        }

        public bool MoveNext()
        {
            Open();
            return ReadPacket();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
