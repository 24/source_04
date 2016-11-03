using pb.IO;

namespace Download.Print
{
    public static class InfoFile
    {
        private const string _infoDirectory = ".i";
        private const string _infoSuffixFile = ".i";

        public static string InfoDirectory { get { return _infoDirectory; } }
        public static string InfoSuffixFile { get { return _infoSuffixFile; } }

        public static string GetInfoFile(string file)
        {
            // file      : "c:\pib\_dl\_dl\_pib\dl\print\.02_hebdo\Le journal du dimanche\Le journal du dimanche - 2016-10-16 - no 3640.pdf"
            // info file : "c:\pib\_dl\_dl\_pib\dl\print\.02_hebdo\Le journal du dimanche\.i\Le journal du dimanche - 2016-10-16 - no 3640.pdf.i"
            //
            // file      : "c:\pib\_dl\_dl\_pib\dl\print\.02_hebdo\Le journal du dimanche\Le journal du dimanche - 2016-10-16 - no 3640(1).pdf"
            // info file : "c:\pib\_dl\_dl\_pib\dl\print\.02_hebdo\Le journal du dimanche\.i\Le journal du dimanche - 2016-10-16 - no 3640(1).pdf.i"

            return zPath.Combine(zPath.GetDirectoryName(file), _infoDirectory, zPath.GetFileName(file)) + _infoSuffixFile;
        }

        // append file2 to file1
        public static void ConcatFiles(string file1, string file2)
        {
            if (zFile.Exists(file1))
            {
                if (zFile.Exists(file2))
                {
                    zfile.AppendFileToFile(file1, file2);
                    zFile.Delete(file2);
                }
            }
            else
            {
                if (zFile.Exists(file2))
                {
                    zfile.CreateFileDirectory(file1);
                    zFile.Move(file2, file1);
                }
            }
        }
    }
}
