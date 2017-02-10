using System;
using System.IO;

namespace pb.IO
{
    public class ProgressionStream : Stream
    {
        //public delegate void ProgressionHandler(double progression);

        private Stream _sourceStream;
        private Action<double> _progression;

        public ProgressionStream(Stream sourceStream, Action<double> progression)
        {
            this._sourceStream = sourceStream;
            this._progression = progression;
        }

        public override int Read(byte[] array, int offset, int count)
        {
            this._progression?.Invoke(this.Position / (double)this.Length * 100);

            return this._sourceStream.Read(array, offset, count);
        }

        public override bool CanRead
        {
            get
            {
                return this._sourceStream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return this._sourceStream.CanSeek;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return this._sourceStream.CanWrite;
            }
        }

        public override long Length
        {
            get
            {
                return this._sourceStream.Length;
            }
        }

        public override long Position
        {
            get
            {
                return this._sourceStream.Position;
            }

            set
            {
                this._sourceStream.Position = value;
            }
        }

        public override void Flush()
        {
            this._sourceStream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return this._sourceStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            this._sourceStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this._sourceStream.Write(buffer, offset, count);
        }
    }
}
