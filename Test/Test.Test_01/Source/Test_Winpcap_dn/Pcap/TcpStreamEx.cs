using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PcapDotNet.Packets;
using pb;
using pb.IO;

namespace Pib.Pcap
{
    public static partial class GlobalExtension
    {
        public static void zTcpStreamReassembledPacket(this TcpStream tcpStream, Action<TcpDirection, Packet> tcpPacketHandle)
        {
            new TcpStreamReassembledPacket(tcpStream).PacketHandle = tcpPacketHandle;
        }

        public static void zWriteTcpStreamToFile(this TcpStream tcpStream, string directory)
        {
            WriteTcpStreamToFile writeTcpStreamToFile = new WriteTcpStreamToFile(tcpStream, directory);
            tcpStream.OnDispose += () => { writeTcpStreamToFile.Close(); };
        }

        public static void zTraceTcpStream(this TcpStream tcpStream, string directory)
        {
            new TraceTcpStream(tcpStream, directory);
        }
    }

    public class WriteTcpStreamToFile : IDisposable
    {
        private string _file;
        private int _indexRequest = 1;
        private int _indexReply = 1;
        private FileStream _fileStreamRequest = null;
        private FileStream _fileStreamReply = null;

        public WriteTcpStreamToFile(TcpStream tcpStream, string directory)
        {
            tcpStream.WriteData += WriteData;
            TcpConnection tcpConnection = tcpStream.TcpConnection;
            //_file = Path.Combine(directory, string.Format("{0}.{1}-{2}.{3}", tcpConnection.Source.IpAddress, tcpConnection.Source.Port,
            //    tcpConnection.Destination.IpAddress, tcpConnection.Destination.Port));
            _file = Path.Combine(directory, tcpConnection.GetConnectionName());
        }

        public void Dispose()
        {
            Close();
        }

        public void Close()
        {
            CloseStreamRequest();
            CloseStreamReply();
        }

        public void CloseStreamRequest()
        {
            if (_fileStreamRequest != null)
            {
                _fileStreamRequest.Close();
                _fileStreamRequest = null;
            }
        }

        public void CloseStreamReply()
        {
            if (_fileStreamReply != null)
            {
                _fileStreamReply.Close();
                _fileStreamReply = null;
            }
        }

        private void WriteData(TcpDirection direction, TcpStreamOperation operation, byte[] data, TcpStreamWriteDataInfo info)
        {
            if (direction == TcpDirection.SourceToDestination)
            {
                if (operation == TcpStreamOperation.Data)
                {
                    if (_fileStreamRequest == null)
                        _fileStreamRequest = new FileStream(_file + "_request_" + _indexRequest++.ToString() + ".data", FileMode.Create, FileAccess.Write, FileShare.Read);
                    _fileStreamRequest.Write(data, 0, data.Length);
                }
                else if (operation == TcpStreamOperation.EndOfData)
                {
                    CloseStreamRequest();
                }
            }
            else
            {
                if (operation == TcpStreamOperation.Data)
                {
                    if (_fileStreamReply == null)
                        _fileStreamReply = new FileStream(_file + "_reply_" + _indexReply++.ToString() + ".data", FileMode.Create, FileAccess.Write, FileShare.Read);
                    _fileStreamReply.Write(data, 0, data.Length);
                }
                else if (operation == TcpStreamOperation.EndOfData)
                {
                    CloseStreamReply();
                }
            }
        }
    }

    public class TraceTcpStreamReassembledPacket
    {
    }

    public class TraceTcpStream
    {
        private string _connectionName;
        private ITrace _trace = null;

        public TraceTcpStream(TcpStream tcpStream, string directory)
        {
            TcpConnection tcpConnection = tcpStream.TcpConnection;
            //_connectionName = string.Format("{0}.{1}-{2}.{3}", tcpConnection.Source.IpAddress, tcpConnection.Source.Port, tcpConnection.Destination.IpAddress, tcpConnection.Destination.Port);
            _connectionName = tcpConnection.GetConnectionName();
            //_log = new Log(Path.Combine(directory, _connectionName + ".txt"), LogOptions.RazLogFile);
            _trace = new TTrace();
            _trace.SetWriter(Path.Combine(directory, _connectionName + ".txt"), FileOption.RazFile);
            tcpStream.WriteData += TraceData;
        }

        private void TraceData(TcpDirection direction, TcpStreamOperation operation, byte[] data, TcpStreamWriteDataInfo info)
        {
            string type;
            if (direction == TcpDirection.SourceToDestination)
                type = "request";
            else
                type = "reply";
            if (operation == TcpStreamOperation.Data)
                _trace.WriteLine("{0,-43}  {1,-7}  {2,-30}  0x{3} 0x{4}  {5,-5}   {6,10:0.000000}",
                    _connectionName, type, info.Message, info.PPacket.Packet.Ethernet.IpV4.Tcp.SequenceNumber.zToHex(), info.NextSequenceNumber.zToHex(),
                    info.PPacket.PacketNumber, info.PPacket.RelativeTime.TotalSeconds);
            else if (operation == TcpStreamOperation.EndOfData)
                _trace.WriteLine("{0,-43}  {1,-7}  end of data", _connectionName, type);
        }
    }
}
