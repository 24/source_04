using pb.IO;
using System.IO;
using System.Text;

namespace pb.Text.Test
{
    public static class Test_Utf8
    {
        public static void Test_WriteBom(string file)
        {
            using (FileStream fs = zFile.Open(file, FileMode.Create, FileAccess.Write, FileShare.Read))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                Encoding encoding = Encoding.UTF8;
                bw.Write(encoding.GetBytes("toto"));
                bw.Write(new byte[] { 0xEF, 0xBB, 0xBF });
                bw.Write(encoding.GetBytes("zaza"));
            }
        }

        public static void Test_ReadBom(string file)
        {
            // bom read = 0xFEFF
            Encoding encoding = Encoding.UTF8;
            using (FileStream fs = zFile.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (StreamReader sr = new StreamReader(fs, encoding))
            {
                int index = 0;
                while (true)
                {
                    int i = sr.Read();
                    if (i == -1)
                        break;
                    Trace.WriteLine($"{index:0000} : 0x{i.zToHex()}");
                }
            }
        }
    }
}
