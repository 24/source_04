using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace pb.IO
{
    //public delegate void TraceDelegate(string msg);
    public delegate void TraceDelegate(string msg, params object[] prm);

    public class Reader : IDisposable
    {
        private string gFile = null;
        private Stream gs = null;
        private bool gStreamCreated = false;
        private BinaryReader gbr = null;
        private int gLineNumber = 0;
        public TraceDelegate Trace = null;

        public Reader(string file, FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read, FileShare share = FileShare.Read, Encoding encoding = null)
        {
            gFile = file;
            gs = new FileStream(file, mode, access, share);
            gStreamCreated = true;
            if (encoding == null) encoding = Encoding.Default;
            gbr = new BinaryReader(gs, encoding);
        }

        public Reader(byte[] data, Encoding encoding = null)
        {
            gs = new MemoryStream(data);
            gStreamCreated = true;
            if (encoding == null) encoding = Encoding.Default;
            gbr = new BinaryReader(gs, encoding);
        }

        public Reader(Stream s, Encoding encoding = null)
        {
            gs = s;
            gStreamCreated = false;
            if (encoding == null) encoding = Encoding.Default;
            gbr = new BinaryReader(gs, encoding);
        }

        public void Dispose()
        {
            Close();
        }

        public void Close()
        {
            if (gs != null && gStreamCreated)
                gs.Close();
            gs = null;
            gbr = null;
        }

        public string File { get { return gFile; } }
        public Stream stream { get { return gs; } }
        public long Length { get { return gs.Length; } }
        public long Position { get { return gs.Position; } }
        public int LineNumber { get { return gLineNumber; } set { gLineNumber = value; } }

        private void trace(string msg, params object[] prm)
        {
            if (Trace == null) return;
            if (prm.Length > 0)
                msg = string.Format(msg, prm);
            Trace(msg);
        }

        public long Seek(long offset, SeekOrigin origin = SeekOrigin.Begin)
        {
            return gs.Seek(offset, origin);
        }

        public string ReadLine()
        {
            gLineNumber++;
            return gbr.zReadLine();
        }

        public byte ReadByte()
        {
            return gbr.ReadByte();
        }

        public byte[] ReadBytes(int count)
        {
            return gbr.ReadBytes(count);
        }

        public byte[] ReadBytesUntil(string mark)
        {
            int markIndex = 0;
            List<byte> buffer = new List<byte>();
            List<byte> markBuffer = new List<byte>();
            while (true)
            {
                byte b = ReadByte();
                char c = (char)b;
                if (mark[markIndex] == c)
                {
                    markBuffer.Add(b);
                    if (++markIndex == mark.Length) break;
                }
                else
                {
                    if (markIndex != 0)
                    {
                        buffer.AddRange(markBuffer);
                        markBuffer.Clear();
                        markIndex = 0;
                    }
                    buffer.Add(b);
                }
            }
            return buffer.ToArray();
        }
    }

    public class Writer : IDisposable
    {
        private string gFile = null;
        private Stream gs = null;
        private bool gStreamCreated = false;
        private BinaryWriter gbw = null;

        public Writer(string file, FileMode mode = FileMode.CreateNew, FileAccess access = FileAccess.ReadWrite, FileShare share = FileShare.Read, Encoding encoding = null)
        {
            gFile = file;
            gs = new FileStream(file, mode, access, share);
            gStreamCreated = true;
            if (encoding == null) encoding = Encoding.Default;
            gbw = new BinaryWriter(gs, encoding);
        }

        public Writer(Stream s, Encoding encoding = null)
        {
            gs = s;
            gStreamCreated = false;
            if (encoding == null) encoding = Encoding.Default;
            gbw = new BinaryWriter(gs, encoding);
        }

        public void Dispose()
        {
            Close();
        }

        public void Close()
        {
            if (gs != null && gStreamCreated)
                gs.Close();
            gs = null;
            gbw = null;
        }

        public string File { get { return gFile; } }
        public Stream stream { get { return gs; } }
        public long Length { get { gbw.Flush(); return gs.Length; } }
        public long Position { get { gbw.Flush(); return gs.Position; } }

        public long Seek(long offset, SeekOrigin origin = SeekOrigin.Begin)
        {
            gbw.Flush();
            return gs.Seek(offset, origin);
        }

        public void WriteLine()
        {
            gbw.Write(new char[] { '\r', '\n' });
        }

        public void WriteLine(string value, params object[] prm)
        {
            Write(value, prm);
            WriteLine();
        }

        public void Write(string value, params object[] prm)
        {
            if (prm.Length > 0)
                value = string.Format(value, prm);
            gbw.Write(value.ToCharArray());
        }

        public void Write(byte[] buffer)
        {
            gbw.Write(buffer);
        }
    }
}
