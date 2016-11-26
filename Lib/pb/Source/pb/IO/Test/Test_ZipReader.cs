using pb.Data.Mongo;
using System;
using System.IO;

// Zip (file format) https://en.wikipedia.org/wiki/Zip_(file_format)
// End of central directory record (EOCD)
// little-endian like windows on x86
namespace pb.IO.Test
{
    public static class Test_ZipReader
    {
        public static void Test_ZipReader_01(string file)
        {
            using (FileStream fs = new FileStream(file, FileMode.Open))
            using (ZipReader zr = new ZipReader(fs))
            {
                zr.Read();
                Trace.WriteLine($"EocdPosition {zr.EocdPosition}   0x{zr.EocdPosition.zToHex()}");
                zr.ZipEocd.zTraceJson();
            }
        }

    }

    public class ZipEocd
    {
        // offset  0, 4 bytes, End of central directory signature = 0x06054b50
        public byte[] Signature;
        // offset  4, 2 bytes, Number of this disk
        public short DiskNumber;
        // offset  6, 2 bytes, Disk where central directory starts
        public short CDDiskNumber;
        // offset  8, 2 bytes, Number of central directory records on this disk
        public short CDRecordNumber;
        // offset 10, 2 bytes, Total number of central directory records
        public short CDTotalRecordNumber;
        // offset 12, 4 bytes, Size of central directory (bytes)
        public int CDSize;
        // offset 16, 4 bytes, Offset of start of central directory, relative to start of archive
        public int CDOffset;
        // offset 20, 2 bytes, Comment length (n)
        public short CommentLength;
        // offset 22, n bytes, Comment
        public byte[] Comment;
    }

    public class ZipReader : IDisposable
    {
        private BinaryReader _br = null;

        private static byte[] _eocdSignature = new byte[] { 0x50, 0x4b, 0x05, 0x06 };     // 0x06054b50 little-endian ordering
        private const int _eocdSearchStart = 150;

        private int _eocdPosition;
        private ZipEocd _zipEocd = null;

        public int EocdPosition { get { return _eocdPosition; } }
        public ZipEocd ZipEocd { get { return _zipEocd; } }

        public ZipReader(Stream stream)
        {
            if (!stream.CanSeek)
                throw new PBException("stream seek is not available");
            _br = new BinaryReader(stream);
        }

        public void Dispose()
        {
            if (_br != null)
            {
                _br.Dispose();
                _br = null;
            }
        }

        public void Read()
        {
            FindEocd();
            ReadEocd();
        }

        private void FindEocd()
        {
            _br.BaseStream.Seek(-_eocdSearchStart, SeekOrigin.End);
            _eocdPosition = (int)_br.BaseStream.Position;
            byte[] buffer = _br.ReadBytes(_eocdSearchStart);
            int i1 = FindBytes(buffer, _eocdSignature);
            if (i1 == -1)
                throw new PBException("eocd not found");
            _eocdPosition += i1;
        }

        private void ReadEocd()
        {
            _zipEocd = new ZipEocd();
            _br.BaseStream.Seek(_eocdPosition, SeekOrigin.Begin);
            _zipEocd.Signature = _br.ReadBytes(4);
            _zipEocd.DiskNumber = _br.ReadInt16();
            _zipEocd.CDDiskNumber = _br.ReadInt16();
            _zipEocd.CDRecordNumber = _br.ReadInt16();
            _zipEocd.CDTotalRecordNumber = _br.ReadInt16();
            _zipEocd.CDSize = _br.ReadInt32();
            _zipEocd.CDOffset = _br.ReadInt32();
            _zipEocd.CommentLength = _br.ReadInt16();
            _zipEocd.Signature = _br.ReadBytes(_zipEocd.CommentLength);
        }

        private static int FindBytes(byte[] source, byte[] pattern)
        {
            byte first = pattern[0];
            for (int i1 = source.Length - pattern.Length; i1 >= 0; i1--)
            {
                if (source[i1] == first)
                {
                    bool found = true;
                    for (int i2 = i1 + 1, i3 = 1; i2 < pattern.Length; i2++, i3++)
                    {
                        if (source[i2] != pattern[i3])
                        {
                            found = false;
                            break;
                        }
                    }
                    if (found)
                        return i1;
                }
            }
            return -1;
        }
    }
}
