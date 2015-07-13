using System;
using System.Collections.Generic;
using System.Text;
using PcapDotNet.Packets.IpV4;

namespace Pib.Pcap
{
    public static partial class GlobalExtension
    {
        public static string zToFormatedString(this IpV4Address ipAddress)
        {
            uint add = ipAddress.ToValue();
            return string.Format("{0:000}.{1:000}.{2:000}.{3:000}", (byte)((add & 0xFF000000) >> 24), (byte)((add & 0x00FF0000) >> 16), (byte)((add & 0x0000FF00) >> 8), (byte)(add & 0x000000FF));
        }
    }
}
