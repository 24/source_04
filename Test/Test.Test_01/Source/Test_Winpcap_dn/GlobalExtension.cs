using System;
using System.Collections.Generic;
using System.Text;

namespace pb
{
    public static partial class GlobalExtension
    {
        //public static PacketCommunicatorReceiveResult zReceivePackets(this PacketCommunicator communicator, int count, HandlePPacket callback)
        //{
        //    return PPacketHandler.ReceivePackets(communicator, count, callback);
        //}

        //public static uint? zGetNextSequenceNumber(this TcpDatagram tcp)
        //{
        //    if (tcp != null)
        //    {
        //        if (tcp.PayloadLength > 0)
        //            return tcp.SequenceNumber + (uint)tcp.PayloadLength;
        //        else if (tcp.IsSynchronize)
        //            return tcp.SequenceNumber + 1;
        //    }
        //    return null;
        //}

        public static void zAddValue(this StringBuilder values, string value)
        {
            values.zAddValue(value, ", ");
        }

        public static void zAddValue(this StringBuilder values, string value, string separator)
        {
            if (values == null || value == null)
                return;
            if (values.Length != 0)
                values.Append(separator);
            values.Append(value);
        }

        //public static string zToString(this byte[] data, Encoding encoding = null, int limit = 0)
        //{
        //    if (encoding == null)
        //        encoding = Encoding.Default;
        //    data.zzToString();
        //    StringBuilder sb = new StringBuilder();
        //    foreach (byte b in data)
        //    {
        //    }
        //}

        public static string zPadCenter(this string text, int width, char paddingChar = ' ')
        {
            int l = text.Length;
            if (l >= width)
                return text;
            l = width - l;
            int l1 = l / 2;
            int l2 = l - l1;
            if (l1 > 0)
                text = "".PadRight(l1, paddingChar) + text;
            return text + "".PadRight(l2, paddingChar);
        }
    }
}
