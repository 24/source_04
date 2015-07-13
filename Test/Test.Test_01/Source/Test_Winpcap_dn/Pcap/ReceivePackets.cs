using System;
using System.Collections.Generic;
using System.Linq;
using PcapDotNet.Core;
using PcapDotNet.Packets;

namespace Pib.Pcap
{
    public delegate void OnPacketReceivedDelegate(Packet packet);
    public delegate void OnPPacketReceivedDelegate(PPacket packet);
    public class ReceivePackets
    {
        private PacketDevice _device = null;
        private PacketCommunicator _communicator = null;
        private PacketDumpFile _packetDumpFile = null;
        private string _filter = null;
        private string _dumpFile = null;
        private PPacketManager _ppacketManager = null;
        public OnPacketReceivedDelegate OnPacketReceived;
        public OnPPacketReceivedDelegate OnPPacketReceived;

        public ReceivePackets(string deviceName)
        {
            _device = SelectDevice(deviceName);
        }

        public static PacketDevice SelectDevice(string deviceName)
        {
            PacketDevice device = null;
            if (deviceName.StartsWith("rpcap://"))
                device = (from d in LivePacketDevice.AllLocalMachine where d.Name == deviceName select d).FirstOrDefault();
            else
                device = new OfflinePacketDevice(deviceName);
            return device;
        }

        public void SetFilter(string filter)
        {
            _filter = filter;
        }

        public void SetDumpFile(string dumpFile)
        {
            _dumpFile = dumpFile;
        }

        public void Receive(OnPacketReceivedDelegate onPacketReceived, int count = 0)
        {
            OnPacketReceived = onPacketReceived;
            _Receive(count);
        }

        public void Receive(OnPPacketReceivedDelegate onPPacketReceived, int count = 0)
        {
            OnPPacketReceived = onPPacketReceived;
            _Receive(count);
        }

        private void _Receive(int count = 0)
        {
            //using (_communicator = _device.Open(65536, PacketDeviceOpenAttributes.Promiscuous, 1000))
            //{
            //    _ppacketManager = new PPacketManager();
            //    if (_filter != null)
            //        _communicator.SetFilter(_filter);
            //    _communicator.ReceivePackets(0, ReceivePacketHandle);
            //}
            try
            {
                _communicator = _device.Open(65536, PacketDeviceOpenAttributes.Promiscuous, 1000);
                if (_dumpFile != null)
                    _packetDumpFile = _communicator.OpenDump(_dumpFile);
                _ppacketManager = new PPacketManager();
                if (_filter != null)
                    _communicator.SetFilter(_filter);
                _communicator.ReceivePackets(count, ReceivePacketHandle);
            }
            finally
            {
                if (_communicator != null)
                {
                    _communicator.Dispose();
                    _communicator = null;
                }
                if (_packetDumpFile != null)
                {
                    _packetDumpFile.Dispose();
                    _packetDumpFile = null;
                }
            }
        }

        private void ReceivePacketHandle(Packet packet)
        {
            if (_packetDumpFile != null)
                _packetDumpFile.Dump(packet);
            if (OnPacketReceived != null)
                OnPacketReceived(packet);
            if (OnPPacketReceived != null)
                OnPPacketReceived(_ppacketManager.CreatePPacket(packet));
        }

        public void Break()
        {
            if (_communicator != null)
                _communicator.Break();
        }

        //OnPacketReceivedDelegate onPacketReceived
        public static ReceivePackets CreateReceivePackets(string deviceName, string dumpFile = null, string filter = null)
        {
            ReceivePackets receivePackets = new ReceivePackets(deviceName);
            receivePackets.SetDumpFile(dumpFile);
            receivePackets.SetFilter(filter);
            //receivePackets.OnPacketReceived = onPacketReceived;
            return receivePackets;
        }

        //OnPPacketReceivedDelegate onPPacketReceived
        //public static ReceivePackets CreateReceivePackets(string deviceName)
        //{
        //    ReceivePackets receivePackets = new ReceivePackets(deviceName);
        //    //receivePackets.OnPPacketReceived = onPPacketReceived;
        //    return receivePackets;
        //}
    }
}
