using System.IO;

namespace pb.IO
{
    public class StreamTransfer
    {
        private bool gAbortTransfer = false;
        private Progress gProgress = null;
        private long gSourceLength = 0;

        #region constructor
        public StreamTransfer()
        {
            Init();
        }

        //public StreamTransfer(IProgressControl progressControl)
        //{
        //    Init(progressControl);
        //}
        #endregion

        #region Progress
        public Progress Progress
        {
            get { return gProgress; }
        }
        #endregion

        #region SourceLength
        public long SourceLength
        {
            get { return gSourceLength; }
            set { gSourceLength = value; }
        }
        #endregion

        #region Init
        //private void Init(IProgressControl progressControl)
        private void Init()
        {
            //gProgress = new Progress(progressControl);
            gProgress = new Progress();
        }
        #endregion

        #region AbortTransfer
        public void AbortTransfer()
        {
            gAbortTransfer = true;
        }
        #endregion

        #region CancelAbortTransfer
        public void CancelAbortTransfer()
        {
            gAbortTransfer = false;
        }
        #endregion

        #region Transfer
        public bool Transfer(Stream source, Stream destination)
        {
            int bufferLength = 32000;
            byte[] buffer = new byte[bufferLength];
            //long pos = source.Position;
            long pos = 0;
            //long length = source.Length;
            while (true)
            {
                if (gAbortTransfer) return false;
                gProgress.SetProgress(pos, gSourceLength);
                int readLength = source.Read(buffer, 0, bufferLength);
                if (readLength == 0) break;
                destination.Write(buffer, 0, readLength);
                pos += readLength;
            }
            return true;
        }
        #endregion
    }
}
