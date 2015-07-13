using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using pb;

namespace Test_BinaryToStruct
{
    public static partial class w
    {
        public static void Test_BinaryToStruct_01()
        {
            // from http://stackoverflow.com/questions/3863191/loading-binary-data-into-a-structure
            //GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            byte[] data = { 0x01, 0x02, 0x03, 0x04 };
            Trace.WriteLine("bytes  : {0}", data.zToStringValues(b => "0x" + b.zToHex()));
            Trace.WriteLine("Test01 : {0}", ByteArrayToTest01(data));
        }

        public unsafe static Test01 ByteArrayToTest01(byte[] data)
        {
            // from http://msdn.microsoft.com/en-us/library/y31yhkeb.aspx
            fixed (byte* p = &data[0])
            {
                return *(Test01*)p;
            }
        }
    }

    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct Test01
    {
        public short i1;
        public short i2;
        public override string  ToString()
        {
            return string.Format("i1 = {0}, i2 = {1}", i1.zToHex(), i2.zToHex());
        }
    }
}
